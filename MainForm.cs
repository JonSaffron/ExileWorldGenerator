using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExileWorldGenerator
    {
    public partial class MainForm : Form
        {
        private readonly CalculateBackground _mapper = new CalculateBackground();
        private readonly SpriteBuilder _spriteBuilder = new SpriteBuilder();
        private readonly SquareProperties[,] _squareProperties = new SquareProperties[256, 256];
        private static readonly string[] BackgroundHandlerTypeList = BuildBackgroundHandlerTypeList();
        private static readonly string[] ObjectTypeList = BuildObjectTypeList();
        private static readonly IReadOnlyList<WaterLevelFromX> WaterLevelList = BuildWaterLevelsList();
        private static readonly List<Point> TeleportDestinations = BuildTeleportDestinations();
        private decimal _zoom;
        private byte lookFor = 0xff;
        private bool _highlightMappedDataSquares;
        private bool _highlightBackgroundObjects;
        private bool _highlightDisplayElement;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private List<byte> _selectedBackgroundObjectTypes = new List<byte>();
        private int _displayElementToHighlight;

        private Point _draggingStartPoint;
        private Point _draggingTopLeftCell;
        private bool _draggingInGrid;

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref TV_ITEM lParam);

        public MainForm()
            {
            InitializeComponent();

            this.BackgroundObjectTree.Nodes[0].ExpandAll();
            this.BackgroundObjectTree.StateImageList = BuildStateImageList();

            this.map.ColumnCount = 256;
            this.map.RowCount = 256;

            var items = new Dictionary<decimal, string>
            {
                {0.5m, "50%"}, 
                {1.0m, "100%"}, 
                {2.0m, "200%"}, 
                {3.0m, "300%"}, 
                {4.0m, "400%"}, 
                {5.0m, "500%"}, 
                {6.0m, "600%"},
                {7.0m, "700%"},
                {8.0m, "800%"}
            };
            cboZoomLevel.DisplayMember = "Value";
            cboZoomLevel.ValueMember = "Key";
            cboZoomLevel.DataSource = new BindingSource(items, null);
            cboZoomLevel.SelectedIndex = 1;
            this._zoom = 1m;

            var backgroundItems = new Dictionary<int, string>();
            for (int i = 0x10; i <= 0x3f; i++)
                {
                backgroundItems.Add(i, "0x" + i.ToString("X"));
                }
            cboHighlightBackground.DisplayMember = "Value";
            cboHighlightBackground.ValueMember = "Key";
            cboHighlightBackground.DataSource = new BindingSource(backgroundItems, null);
            cboHighlightBackground.SelectedIndex = 1;

            var type = map.GetType();
            var prop = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (prop != null)
                prop.SetValue(this.map, true);

            this._stopwatch.Start();

            var hashOfPositions = new ConcurrentBag<int>();
            Parallel.For(0, 0x10000, i => { NewMethod(i, hashOfPositions); });
            if (hashOfPositions.Count != 1024)
                throw new InvalidOperationException("1024 positions are expected to be explicitly mapped, rather than " + hashOfPositions.Count);
            if (hashOfPositions.Distinct().Count() != 1024)
                throw new InvalidOperationException("All 1024 explicit map positions should be unique.");

            string findResults = string.Empty;
            for (int y = 0; y < 256; y++)
                {
                for (int x = 0; x < 256; x++)
                    {
                    var squareProperties = this._squareProperties[x, y];
                    if ((squareProperties.BackgroundAfterPalette & 0x3f) == lookFor)
                        {
                        findResults += $"({x:x2},{y:x2})\r\n";
                        }
                    }
                }

            Trace.WriteLine(findResults);
            }

        private void Form1_Shown(object sender, EventArgs e)
            {
            ResetGrid();
            map.FirstDisplayedCell = map.Rows[0x39].Cells[0x97];
            map.CurrentCell = map.Rows[0x3b].Cells[0x9b];
            map_SelectionChanged(null, null);
            }

        private void ResetGrid()
            {
            this.map.SuspendLayout();

            for (int i = 0; i < 256; i++)
                {
                var value = i.ToString("X2");
                this.map.Rows[i].HeaderCell.Value = value;
                this.map.Columns[i].HeaderCell.Value = value;
                this.map.Rows[i].Height = (int) (32 * _zoom);
                this.map.Columns[i].Width = (int) (32 * _zoom);
                }
            
            this.map.ResumeLayout();
            }

        private void NewMethod(int i, ConcurrentBag<int> hashOfPositions)
            {
            byte y = (byte) (i >> 8);
            byte x = (byte) (i & 0xFF);

            var mapResult = _mapper.GetBackground(x, y);
            if (mapResult.IsMappedData)
                {
                hashOfPositions.Add(mapResult.PositionInMappedData);
                }

            var data = new SquareProperties {X = x, Y = y, CalculatedBackground = mapResult.Result};
            if (mapResult.IsMappedData)
                data.MappedDataPosition = mapResult.PositionInMappedData;

            var backgroundProperties = _mapper.GetMapData(mapResult.Result, x);
            data.BackgroundAfterHashing = backgroundProperties.background;
            data.BackgroundObjectId = backgroundProperties.backgroundObjectId;
            data.IsHashDefault = backgroundProperties.isHashDefault;

            byte background = (byte) (data.BackgroundAfterHashing & 0x3f);
            byte orientation = (byte) (data.BackgroundAfterHashing & 0xc0);
            if (data.IsBackgroundEvent)
                {
                data.BackgroundHandlerType = BackgroundHandlerTypeList[background];
                if (backgroundProperties.backgroundObjectId.HasValue)
                    {
                    data.BackgroundType = backgroundProperties.type;
                    data.BackgroundData = backgroundProperties.data;
                    data.BackgroundDescription = DescribeBackground((BackgroundObjectType) background, backgroundProperties.type, backgroundProperties.data, orientation);
                    }
                }

            (data.BackgroundPalette, data.DisplayedPalette) = _mapper.GetPalette(ref background, ref orientation, x, y);
            data.BackgroundAfterPalette = (byte) (background ^ orientation);

            var teleportDestination = TeleportDestinations.FindIndex(item => item == new Point(x, y));
            if (teleportDestination != -1)
                {
                data.BackgroundDescription = $"Teleport destination 0x{teleportDestination:X}\r\n" + (data.BackgroundDescription ?? string.Empty);
                }

            this._squareProperties[x, y] = data;
            }

        private static string DescribeBackground(BackgroundObjectType backgroundHandler, byte? type, byte? data, byte orientation)
            {
            switch (backgroundHandler)
                {
                case BackgroundObjectType.InvisibleSwitch:
                    {
                    Debug.Assert(type.HasValue);
                    return $"Triggered by {(type.Value == 0x80 ? "anything" : ObjectTypeList[type.Value])}";
                    }
                
                case BackgroundObjectType.Teleport:
                    {
                    Debug.Assert(data.HasValue);
                    var active = (data.Value & 0x1) != 0;
                    var destination = (data.Value >> 4 & 0x7);
                    var key = ((data.Value & 0x7f) + 0x60) >> 5;
                    Debug.Assert(key <= 6 && key >= 3);
                    var keyDescription = GetKeyDescription(key);
                    Debug.WriteLine(keyDescription);
                    return $"teleport: active = {active}, destination = &{destination:X}, key = {keyDescription}";
                    }
                
                case BackgroundObjectType.ObjectFromData:
                    {
                    Debug.Assert(data.HasValue);
                    return ObjectTypeList[data.Value & 0x7f];
                    }
                
                case BackgroundObjectType.Door:
                    {
                    Debug.Assert(data.HasValue);
                    var locked = (data.Value & 0x1) != 0;
                    var open = (data.Value & 0x2) != 0;
                    var slowMoving = (data.Value & 0x8) != 0;
                    var key = (data.Value >> 4 & 0x7);
                    var keyDescription = GetKeyDescription(key);
                    var latchesOpen = (key & 0x3) != 0;
                    return $"door: locked = {locked}, open = {open}, slowMoving = {slowMoving}, key = {keyDescription}, latchesOpen = {latchesOpen}";
                    }
                
                case BackgroundObjectType.StoneDoor:
                    {
                    Debug.Assert(data.HasValue);
                    var locked = (data.Value & 0x1) != 0;
                    var open = (data.Value & 0x2) != 0;
                    var slowMoving = (data.Value & 0x8) != 0;
                    var key = (data.Value >> 4 & 0x7);
                    Debug.Assert(key == 4 || key == 7);
                    var keyDescription = GetKeyDescription(key);
                    var latchesOpen = (key & 0x3) != 0;
                    return $"stone door: locked = {locked}, open = {open}, slowMoving = {slowMoving}, key = {keyDescription}, latchesOpen = {latchesOpen}";
                    }

                case BackgroundObjectType.ObjectFromTypeWithWall:
                case BackgroundObjectType.ObjectFromType:
                case BackgroundObjectType.ObjectFromTypeWithFoliage:
                    {
                    Debug.Assert(type.HasValue);
                    return ObjectTypeList[type.Value];
                    }

                case BackgroundObjectType.Switch:
                    {
                    return "*** switch tbd ***";
                    }

                case BackgroundObjectType.ObjectEmergingFromBush:
                case BackgroundObjectType.ObjectEmergingFromPipe:
                    {
                    Debug.Assert(data.HasValue);
                    Debug.Assert(type.HasValue);
                    int countOfObjects = (data.Value & 0x7c) >> 2;
                    string displayType;

                    bool spawnedObjectEmergesImmediately = (orientation & 0x80) != 0;
                    bool hasTree = (data.Value & 0x80) != 0;
                    bool enabled = (data.Value & 0x03) == 0;

                    if (spawnedObjectEmergesImmediately && hasTree)
                        displayType = "emerges immediately from tree";
                    else if (spawnedObjectEmergesImmediately)
                        displayType = "emerges immediately";
                    else if (hasTree)
                        displayType = "emerges sporadically from tree";
                    else
                        displayType = "emerges sporadically";
                    var enabledDisplay = enabled ? "enabled" : "disabled";

                    return $"{countOfObjects} x {ObjectTypeList[type.Value]} {displayType} {enabledDisplay}";
                    }

                case BackgroundObjectType.FixedWind:
                    {
                    Debug.Assert(data.HasValue);
                    int yVelocity = data.Value;
                    if (yVelocity >= 128)
                        yVelocity -= 256;
                    int xVelocity = ((data.Value << 4) & 0xff);
                    if (xVelocity >= 128)
                        xVelocity -= 256;
                    return $"Fixed wind velocity ({xVelocity}, {yVelocity})";
                    }

                case BackgroundObjectType.EngineThruster:
                    return "*** engine tbd ***";

                case BackgroundObjectType.Water:
                    return null;        // water does not have data

                case BackgroundObjectType.RandomWind:
                    return null;        // random wind does not have data

                case BackgroundObjectType.Mushrooms:
                    return null;        // mushrooms does not have data
                }
            throw new InvalidOperationException();
            }

        private static string GetKeyDescription(int key)
            {
            const int startOfKeyObjects = 0x51;

            if (key < 0 || key > 7)
                throw new ArgumentOutOfRangeException(nameof(key));
            var result = key.ToString();
            int objectType = startOfKeyObjects + key;
            var objectSprite = SpriteBuilder.ObjectSpriteLookup[objectType];
            if (objectSprite != 0x4d)
                {
                result += " - does not exist";
                return result;
                }

            var colourScheme = SquarePalette.FromByte(SpriteBuilder.ObjectPaletteLookup[objectType]);
            result += $" {colourScheme.Colour1}, {colourScheme.Colour2}, {colourScheme.PrimaryColour}";
            return result;
            }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                return;
                }

            var timeElapsed = this._stopwatch.Elapsed;

            var squareProperties = this._squareProperties[e.ColumnIndex, e.RowIndex];

            var waterLevelType = GetWaterLevelType(squareProperties.X, squareProperties.Y);
            this._spriteBuilder.Clear(waterLevelType);

            switch ((BackgroundObjectType) (squareProperties.BackgroundAfterPalette & 0x3f))
                {
                case BackgroundObjectType.InvisibleSwitch:
                    {
                    this._spriteBuilder.AddBitmap(Resources.HiddenSwitch);
                    break;
                    }

                case BackgroundObjectType.ObjectFromData:
                    {
                    Debug.Assert(squareProperties.BackgroundData.HasValue);
                    this._spriteBuilder.BuildObjectFromDataSprite(squareProperties.BackgroundData.Value, squareProperties.BackgroundAfterPalette);
                    break;
                    }
                
                case BackgroundObjectType.Door:
                case BackgroundObjectType.StoneDoor:
                    {
                    Debug.Assert(squareProperties.BackgroundData.HasValue);
                    this._spriteBuilder.BuildDoor(squareProperties.BackgroundAfterPalette, squareProperties.BackgroundData.Value);
                    break;
                    }

                case BackgroundObjectType.ObjectFromTypeWithWall:
                case BackgroundObjectType.ObjectFromTypeWithFoliage:
                    {
                    Debug.Assert(squareProperties.BackgroundType.HasValue);
                    this._spriteBuilder.BuildObjectFromDataSprite(squareProperties.BackgroundType.Value, squareProperties.BackgroundAfterPalette);
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.BackgroundAfterPalette, squareProperties.DisplayedPalette);
                    break;
                    }

                case BackgroundObjectType.ObjectFromType:
                    {
                    Debug.Assert(squareProperties.BackgroundType.HasValue);
                    this._spriteBuilder.BuildObjectFromDataSprite(squareProperties.BackgroundType.Value, squareProperties.BackgroundAfterPalette);
                    break;
                    }
            
                case BackgroundObjectType.Switch:
                    {
                    this._spriteBuilder.BuildObjectFromDataSprite(0x42, squareProperties.BackgroundAfterPalette);
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.BackgroundAfterPalette, squareProperties.DisplayedPalette);
                    break;
                    }

                case BackgroundObjectType.ObjectEmergingFromBush:
                case BackgroundObjectType.ObjectEmergingFromPipe:
                    {
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.BackgroundAfterPalette, squareProperties.DisplayedPalette);
                    if (squareProperties.BackgroundObjectId.HasValue)
                        {
                        Debug.Assert(squareProperties.BackgroundData.HasValue);
                        if ((squareProperties.BackgroundData.Value & 0x80) != 0)
                            {
                            this._spriteBuilder.BuildObjectSprite(0x40, squareProperties.BackgroundAfterPalette, (0x40, 0x40));
                            }
                        }
                    break;
                    }

                case BackgroundObjectType.FixedWind:
                    {
                    this._spriteBuilder.AddBitmap(Resources.FixedWind);
                    break;
                    }

                case BackgroundObjectType.RandomWind:
                    {
                    this._spriteBuilder.AddBitmap(Resources.RandomWind);
                    break;
                    }

                default:
                    {
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.BackgroundAfterPalette, squareProperties.DisplayedPalette);
                    break;
                    }
                }

            var isTeleportDestination = TeleportDestinations.Contains(new Point(squareProperties.X, squareProperties.Y));
            if (isTeleportDestination)
                {
                this._spriteBuilder.AddBitmap(Resources.TeleportTarget);
                }

            Rectangle destinationRectangle = new Rectangle(e.CellBounds.Left, e.CellBounds.Top - 1, (int) (32 * _zoom), (int) (32 * _zoom));
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None; 
            e.Graphics.DrawImage(this._spriteBuilder.Sprite, destinationRectangle);

            if (DoesSquareRequireAnimating(squareProperties))
                {
                AdvanceAnimation(squareProperties, timeElapsed);

                if (this._highlightMappedDataSquares && squareProperties.MappedDataPosition.HasValue)
                    {
                    using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Orange)))
                        {
                        using (Pen pen = new Pen(brush, squareProperties.AnimationFrame))
                            {
                            Rectangle r = e.CellBounds;
                            r.Inflate(-1, -1);
                            e.Graphics.DrawRectangle(pen, r);
                            }
                        }
                    }

                if (this._highlightBackgroundObjects && DoesSquareMatchBackgroundObjectCriteria(squareProperties))
                    {
                    using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Aqua)))
                        {
                        using (Pen pen = new Pen(brush, squareProperties.AnimationFrame))
                            {
                            Rectangle r = e.CellBounds;
                            r.Inflate(-1, -1);
                            e.Graphics.DrawEllipse(pen, r);
                            }
                        }
                    }

                if (this._highlightDisplayElement && (squareProperties.BackgroundAfterPalette & 0x3f) == this._displayElementToHighlight)
                    {
                    using (Brush brush = new SolidBrush(Color.FromArgb(0x40, Color.Silver)))
                        {
                        using (Pen pen = new Pen(brush, squareProperties.AnimationFrame))
                            {
                            Point topMiddle = e.CellBounds.Location + new Size(e.CellBounds.Width / 2, 0);
                            Point leftMiddle = e.CellBounds.Location + new Size(0, e.CellBounds.Height / 2);
                            Point bottomMiddle = e.CellBounds.Location + new Size(e.CellBounds.Width / 2, e.CellBounds.Height);
                            Point rightMiddle = e.CellBounds.Location + new Size(e.CellBounds.Width, e.CellBounds.Height / 2);
                            e.Graphics.DrawLine(pen, topMiddle, leftMiddle);
                            e.Graphics.DrawLine(pen, leftMiddle, bottomMiddle);
                            e.Graphics.DrawLine(pen, bottomMiddle, rightMiddle);
                            e.Graphics.DrawLine(pen, rightMiddle, topMiddle);
                            }
                        }
                    }
                }

            if (this.map.SelectedCells.Count != 0)
                {
                if (e.RowIndex == this.map.SelectedCells[0].RowIndex && e.ColumnIndex == this.map.SelectedCells[0].ColumnIndex)
                    {
                    using (Brush brush = new SolidBrush(Color.FromArgb(0x80, Color.DarkOrchid)))
                        {
                        e.Graphics.FillRectangle(brush, e.CellBounds);
                        }
                    }
                }

            if ((squareProperties.BackgroundAfterPalette & 0x3f) == lookFor)
                {
                }

            e.Handled = true;
            }

        private static WaterLevelType GetWaterLevelType(byte x, byte y)
            {
            var i = WaterLevelList.Count;
            while (true)
                {
                i--;
                var waterLevel = WaterLevelList[i];
                if (x >= waterLevel.FromX)
                    {
                    if (y > waterLevel.WaterLevel)
                        return WaterLevelType.BelowWater;
                    if (y == waterLevel.WaterLevel)
                        return WaterLevelType.AtWaterLine;
                    return WaterLevelType.AboveWater;
                    }
                }
            }

        private void AdvanceAnimation(SquareProperties squareProperties, TimeSpan timeElapsed)
            {
            TimeSpan frameLength = TimeSpan.FromMilliseconds(100);

            if (squareProperties.NextAnimationFrame.HasValue)
                {
                TimeSpan elapsed = timeElapsed - squareProperties.NextAnimationFrame.Value;
                int framesMoved = (int) elapsed.TotalMilliseconds / (int) frameLength.TotalMilliseconds;
                squareProperties.AnimationFrame = (squareProperties.AnimationFrame + framesMoved) % 5;
                if (this._highlightMappedDataSquares || this._highlightBackgroundObjects || this._highlightDisplayElement)
                    {
                    TimeSpan timeToNextFrame = TimeSpan.FromMilliseconds(framesMoved * frameLength.TotalMilliseconds);
                    squareProperties.NextAnimationFrame = squareProperties.NextAnimationFrame.Value + timeToNextFrame;
                    }
                else
                    {
                    squareProperties.NextAnimationFrame = null;
                    }
                }
            else
                {
                squareProperties.AnimationFrame = 0;
                squareProperties.NextAnimationFrame = timeElapsed + frameLength;
                }
            }

        private bool DoesSquareRequireAnimating(SquareProperties squareProperties)
            {
            if (squareProperties.NextAnimationFrame.HasValue)
                return true;
            if (this._highlightMappedDataSquares && squareProperties.MappedDataPosition.HasValue)
                return true;
            if (this._highlightBackgroundObjects && DoesSquareMatchBackgroundObjectCriteria(squareProperties))
                return true;
            if (this._highlightDisplayElement && (squareProperties.BackgroundAfterPalette & 0x3f) == this._displayElementToHighlight)
                return true;
            return false;
            }

        private bool DoesSquareMatchBackgroundObjectCriteria(SquareProperties squareProperties)
            {
            var type = (byte) (squareProperties.BackgroundAfterPalette & 0x3f);
            if (type > 0xf || !this._selectedBackgroundObjectTypes.Contains(type))
                {
                return false;
                }

            var result = type > 0xc || squareProperties.BackgroundObjectId.HasValue;
            return result;
            }

        private List<byte> BuildSelectedBackgroundTypes()
            {
            var result = new List<byte>();
            AddSelectedNodes(result, this.BackgroundObjectTree.Nodes[0]);
            return result;
            }

        private void AddSelectedNodes(List<byte> result, TreeNode node)
            {
            if (node.Nodes.Count != 0)
                {
                foreach (TreeNode child in node.Nodes)
                    {
                    AddSelectedNodes(result, child);
                    }
                }
            else
                {
                if (node.Checked)
                    result.Add(byte.Parse((string) node.Tag, NumberStyles.HexNumber));
                }
            }

        private class SquareProperties
            {
            public byte X;
            public byte Y;
            public byte CalculatedBackground;
            public byte BackgroundAfterHashing;
            public byte BackgroundAfterPalette;

            public int? MappedDataPosition;
            public bool IsHashDefault;
            public int? BackgroundObjectId;
            public string BackgroundHandlerType;
            public byte BackgroundPalette;
            public byte DisplayedPalette;

            public byte? BackgroundType;
            public byte? BackgroundData;
            public string BackgroundDescription;

            public TimeSpan? NextAnimationFrame;
            public int AnimationFrame;

            public bool IsBackgroundEvent
                {
                get 
                    {
                    var background = this.BackgroundAfterHashing & 0x3f;
                    if (this.BackgroundObjectId.HasValue & background <= 0xc)
                        return true;
                    return background <= 0xf;
                    }
                }
            }

        private void map_SelectionChanged(object sender, EventArgs e)
            {
            var squareValue = this._squareProperties[this.map.CurrentCell.ColumnIndex, this.map.CurrentCell.RowIndex];
            
            // ReSharper disable once LocalizableElement
            this.txtCoordinates.Text = $"X:{squareValue.X:X2} Y:{squareValue.Y:X2}";

            SetMappedOrGeneratedInfo(squareValue);

            SetListOverrideInfo(squareValue);

            SetPaletteInfo(squareValue);

            SetSpriteInfo(squareValue);

            SetBackgroundObjectInfo(squareValue);
            }

        private void SetBackgroundObjectInfo(SquareProperties squareValue)
            {
            string backgroundObjectInfo;
            byte background = (byte) (squareValue.BackgroundAfterPalette & 0x3f); 
            if (background >= 0x10)
                {
                backgroundObjectInfo = "No background event";
                if (squareValue.BackgroundDescription != null)
                    {
                    backgroundObjectInfo += "\r\n" + squareValue.BackgroundDescription;
                    }
                }
            else 
                {
                backgroundObjectInfo =
                    $"Background handler: {squareValue.BackgroundHandlerType}";

                if (!squareValue.BackgroundObjectId.HasValue)
                    {
                    backgroundObjectInfo += "\r\n" + "Scenery only";
                    if (squareValue.BackgroundDescription != null)
                        {
                        backgroundObjectInfo += "\r\n" + squareValue.BackgroundDescription;
                        }
                    }
                else 
                    {
                    if (squareValue.BackgroundType.HasValue)
                        {
                        backgroundObjectInfo += "\r\n" +
                            $"Object type: {squareValue.BackgroundType.Value:X2}";
                        }
                    Debug.Assert(squareValue.BackgroundData.HasValue);
                    backgroundObjectInfo += "\r\n" +
                        $"Data: {squareValue.BackgroundData.Value:X2}";
                    backgroundObjectInfo += "\r\n" +
                        $"Description: {squareValue.BackgroundDescription}";
                    }
                }

            this.txtBackgroundObjectInfo.Text = backgroundObjectInfo;
            SetTextBoxHeight(txtBackgroundObjectInfo);
            }

        private void SetSpriteInfo(SquareProperties squareValue)
            {
            byte sprite = (byte) (SpriteBuilder.BackgroundSpriteLookup[squareValue.BackgroundAfterPalette & 0x3f] & 0x7f);
            string spriteInfo =
                $"Sprite number: {sprite:X2}\r\n" +
                "Orientation on sprite sheet: ";
            var isSpriteFlippedHorizontally = SpriteBuilder.FlipSpriteHorizontally[sprite];
            var isSpriteFlippedVertically = SpriteBuilder.FlipSpriteVertically[sprite];
            if (isSpriteFlippedHorizontally && isSpriteFlippedVertically)
                spriteInfo += "flipped both ways";
            else if (isSpriteFlippedHorizontally)
                spriteInfo += "flipped horizontally";
            else if (isSpriteFlippedVertically)
                spriteInfo += "flipped vertically";
            else
                spriteInfo += "not flipped";
            byte offsetAlongY = (byte) (SpriteBuilder.BackgroundYOffsetLookup[squareValue.BackgroundAfterPalette & 0x3f] & 0xf0);
            spriteInfo += "\r\n" +
                          $"Y offset: {offsetAlongY:X2}\r\n";

            bool rightAlign = (squareValue.BackgroundAfterPalette & 0x80) != 0;
            bool bottomAlign = (squareValue.BackgroundAfterPalette & 0x40) != 0;
            var isFlippedHorizontally = isSpriteFlippedHorizontally ^ rightAlign;
            var isFlippedVertically = isSpriteFlippedVertically ^ bottomAlign;
            spriteInfo +=
                "Final orientation: ";
            if (isFlippedHorizontally && isFlippedVertically)
                spriteInfo += "flipped both ways";
            else if (isFlippedHorizontally)
                spriteInfo += "flipped horizontally";
            else if (isFlippedVertically)
                spriteInfo += "flipped vertically";
            else
                spriteInfo += "not flipped";

            spriteInfo += "\r\n" +
                          $"Alignment: {(bottomAlign ? "bottom" : "top")} {(rightAlign ? "right" : "left")}";

            this.txtSpriteInfo.Text = spriteInfo;
            SetTextBoxHeight(this.txtSpriteInfo);
            }

        private void SetPaletteInfo(SquareProperties squareValue)
            {
            string paletteInfo =
                $"Palette for background: {squareValue.BackgroundPalette:X2}";
            if (squareValue.BackgroundPalette > 6)
                {
                var colours = SquarePalette.FromByte(squareValue.BackgroundPalette);
                paletteInfo += $" {colours.Colour1}, {colours.Colour2}, {colours.PrimaryColour}";
                }
            else
                {
                paletteInfo += "\r\n" +
                               $"Derived palette: {squareValue.DisplayedPalette:X2}";
                var colours = SquarePalette.FromByte(squareValue.DisplayedPalette);
                paletteInfo += $" {colours.Colour1}, {colours.Colour2}, {colours.PrimaryColour}";
                }

            if (squareValue.BackgroundAfterHashing != squareValue.BackgroundAfterPalette)
                {
                paletteInfo +=
                    $"\r\nRevised background: {squareValue.BackgroundAfterPalette:x2}\r\n" +
                    $"Appearance: {squareValue.BackgroundAfterPalette & 0x3f:X2}, " +
                    $"Orientation: {squareValue.BackgroundAfterPalette & 0xc0:X2} " +
                    $"({DescribeOrientation(squareValue.BackgroundAfterPalette)})";
                }

            this.txtPaletteInfo.Text = paletteInfo;
            SetTextBoxHeight(this.txtPaletteInfo);
            }

        private void SetListOverrideInfo(SquareProperties squareValue)
            {
            if ((squareValue.CalculatedBackground & 0x3f) >= 0x9)
                {
                // ReSharper disable once LocalizableElement
                this.txtListOverrideInfo.Text = "No override";
                }
            else
                {
                var listOverride =
                    $"Override list: {squareValue.CalculatedBackground & 0x3f}\r\n";
                if (squareValue.IsHashDefault)
                    {
                    listOverride +=
                        $"Default for list: {squareValue.BackgroundAfterHashing:X2}\r\n";
                    }
                else
                    {
                    Debug.Assert(squareValue.BackgroundObjectId.HasValue);
                    listOverride +=
                        $"Background object index = {squareValue.BackgroundObjectId.Value:X2}\r\n" +
                        $"Value from list: {squareValue.BackgroundAfterHashing:x2}\r\n";
                    }

                listOverride +=
                    $"{((squareValue.BackgroundAfterHashing & 0x3f) <= 0xf ? "Handler" : "Appearance")}: {squareValue.BackgroundAfterHashing & 0x3f:X2}, " +
                    $"Orientation: {squareValue.BackgroundAfterHashing & 0xc0:X2} " +
                    $"({DescribeOrientation(squareValue.BackgroundAfterHashing)})";
                this.txtListOverrideInfo.Text = listOverride;
                }
            SetTextBoxHeight(this.txtListOverrideInfo);
            }

        private void SetMappedOrGeneratedInfo(SquareProperties squareValue)
            {
            var mappedGeneratedInfo =
                squareValue.MappedDataPosition.HasValue
                    ? $"Mapped location index: {squareValue.MappedDataPosition.Value}\r\n"
                    : string.Empty;
            mappedGeneratedInfo +=
                $"{(squareValue.MappedDataPosition.HasValue ? "Explicit" : "Generated")} background {squareValue.CalculatedBackground:X2}\r\n";
            mappedGeneratedInfo +=
                $"Appearance: {squareValue.CalculatedBackground & 0x3f:X2}, " +
                $"Orientation: {squareValue.CalculatedBackground & 0xc0:X2} " +
                $"({DescribeOrientation(squareValue.CalculatedBackground)})";
            this.txtMappedOrGeneratedInfo.Text = mappedGeneratedInfo;
            SetTextBoxHeight(this.txtMappedOrGeneratedInfo);
            }

        private string DescribeOrientation(byte background)
            {
            var orientation = background & 0xc0;
            switch (orientation)
                {
                case 0: return "no change";
                case 0x40: return "flipped vertically";
                case 0x80: return "flipped horizontally";
                case 0xc0: return "flipped both ways";
                }
            throw new InvalidOperationException();
            }

        private void SetTextBoxHeight(TextBox textBox)
            {
            Size sz = new Size(textBox.ClientSize.Width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak;
            int padding = 3;
            int borders = textBox.Height - textBox.ClientSize.Height;
            sz = TextRenderer.MeasureText(textBox.Text, textBox.Font, sz, flags);
            int h = sz.Height + borders + padding;
            if (textBox.Top + h > this.ClientSize.Height - 10) 
                {
                h = this.ClientSize.Height - 10 - textBox.Top;
                }
            textBox.Height = h;
            }

        private static List<WaterLevelFromX> BuildWaterLevelsList()
            {
            var result = new List<WaterLevelFromX>
                {
                new WaterLevelFromX(0x0, 0xce),
                new WaterLevelFromX(0x54, 0xdf),
                new WaterLevelFromX(0x74, 0xc1),
                new WaterLevelFromX(0xa0, 0xc1)
                };
            return result;
            }

        private readonly struct WaterLevelFromX
            {
            public readonly byte FromX;
            public readonly byte WaterLevel;

            public WaterLevelFromX(byte fromX, byte waterLevel)
                {
                this.FromX = fromX;
                this.WaterLevel = waterLevel;
                }
            }

        private static string[] BuildBackgroundHandlerTypeList()
            {
            var result = new[]
                {
                "background_invisible_switch",
                "background_teleport_beam",
                "background_object_from_data",
                "background_door",
                "background_stone_door",
                "background_object_from_type (half wall)",
                "background_object_from_type",
                "background_object_from_type (foliage)",
                "background_switch",
                "background_object_emerging (bush)",
                "background_object_emerging (pipe end)",
                "background_object_fixed_wind",
                "background_engine_thruster",
                "background_object_water",
                "background_object_random_wind",
                "background_mushrooms"
                };
            return result;
            }

        private static string[] BuildObjectTypeList()
            {
            var result = new []
                {
                "player",
                "active chatter",
                "pericles crew member",
                "fluffy",
                "small nest",
                "big nest",
                "red frogman",
                "green frogman",
                "cyan frogman",
                "red slime",
                "green slime",
                "yellow ball",
                "sucker",
                "deadly sucker",
                "big fish",
                "worm",
                "piranha",
                "wasp",
                "active grenade",		
                "icer bullet",				
                "tracer bullet",				
                "cannonball",				
                "blue death ball",			
                "red bullet",				
                "pistol bullet",				
                "plasma ball",				
                "hover ball",				
                "invisible hover ball",		
                "magenta robot",				
                "red robot",				
                "blue robot",				
                "green/white turret",		
                "cyan/red turret",			
                "hovering robot",			
                "magenta clawed robot",		
                "cyan clawed robot",			
                "green clawed robot",		
                "red clawed robot",			
                "triax",						
                "maggot",					
                "gargoyle",					
                "red/magenta imp",			
                "red/yellow imp",			
                "blue/cyan imp",				
                "cyan/yellow imp",			
                "red/cyan imp",				
                "green/yellow bird",			
                "white/yellow bird",			
                "red/magenta bird",			
                "invisible bird",			
                "lightning",					
                "red mushroom ball",			
                "blue mushroom ball",		
                "engine fire",				
                "red drop",					
                "flames",					
                "inactive chatter",			
                "moving fireball",			
                "giant wall",				
                "engine thruster",			
                "horizontal door",			
                "vertical door",				
                "horizontal stone door",		
                "vertical stone door",		
                "bush",						
                "teleport beam",				
                "switch",					
                "chest",					
                "explosion",					
                "rock",						
                "cannon",					
                "mysterious weapon",			
                "maggot machine",			
                "placeholder",				
                "destinator",				
                "energy capsule",			
                "empty flask",				
                "full flask",				
                "remote control device",		
                "cannon control device",		
                "inactive grenade",			
                "key 0 (cyan/yellow/green)",		
                "key 1 (red/yellow/green)",		
                "key 2 (green/yellow/red)",		
                "key 3 (yellow/white/red)",		
                "coronium boulder",			
                "key 5 (red/magenta/red)",		
                "key 6 (blue/cyan/green)",		
                "coronium crystal",			
                "jetpack booster",			
                "pistol",					
                "icer",						
                "discharge device",			
                "plasma gun",				
                "protection suit",			
                "fire immunity device",		
                "mushroom immunity pill",	
                "whistle 1",				
                "whistle 2",					
                "radiation immunity pill"	
                };
            return result;
            }

        private static List<Point> BuildTeleportDestinations()
            {
            var x = new[] {0x62, 0xad, 0x2a, 0x0b, 0x9d, 0xaf, 0x9e, 0x45, 0x89, 0x9d, 0xb5, 0xa2, 0x72, 0xa7, 0x9f, 0xb0};
            var y = new[] {0xc7, 0x62, 0xcd, 0x0b, 0x58, 0x62, 0x69, 0x57, 0x71, 0x3c, 0x66, 0x63, 0x54, 0x80, 0x49, 0x80};
            Debug.Assert(x.Length == y.Length);
            var c = x.Length;
            var result = new List<Point>(c);
            for (int i = 0; i < c; i++)
                {
                var newPoint = new Point(x[i], y[i]);
                result.Add(newPoint);
                }

            return result;
            }

        private void AnimationTimer_Tick(object sender, EventArgs e)
            {
            var timeElapsed = this._stopwatch.Elapsed;

            bool isAnimationBeingDisabled = !this._highlightMappedDataSquares && !this._highlightBackgroundObjects && !this._highlightDisplayElement;

            Parallel.ForEach(this._squareProperties.Cast<SquareProperties>(), item => 
                {
                if (DoesSquareRequireAnimating(item))
                    {
                    if (isAnimationBeingDisabled || !item.NextAnimationFrame.HasValue || timeElapsed >= item.NextAnimationFrame)
                        {
                        this.map.InvalidateCell(item.X, item.Y);
                        }
                    }
                });

            if (isAnimationBeingDisabled)
                this.AnimationTimer.Enabled = false;
            }

        private void cboZoomLevel_SelectedIndexChanged(object sender, EventArgs e)
            {
            this._zoom = (decimal) this.cboZoomLevel.SelectedValue;
            if (map.FirstDisplayedCell == null)
                return;
            var row = map.FirstDisplayedCell.RowIndex;
            var column = map.FirstDisplayedCell.ColumnIndex;
            this.ResetGrid();
            map.FirstDisplayedCell = map.Rows[row].Cells[column];
            }

        private void chkHighlightMappedData_CheckedChanged(object sender, EventArgs e)
            {
            this._highlightMappedDataSquares = this.chkHighlightMappedData.Checked;
            if (this._highlightMappedDataSquares)
                this.AnimationTimer.Enabled = true;
            }

        private void chkHighlightBackgroundObjects_CheckedChanged(object sender, EventArgs e)
            {
            this._highlightBackgroundObjects = this.chkHighlightBackgroundObjects.Checked;
            if (this._highlightBackgroundObjects)
                this.AnimationTimer.Enabled = true;
            }

        private void BackgroundObjectTree_AfterCheck(object sender, TreeViewEventArgs e)
            {
            if (e.Action == TreeViewAction.Unknown)
                return;
            CheckOrUncheckChildren(e.Node, e.Node.Checked);
            UpdateParentNodeState(e.Node);
            this._selectedBackgroundObjectTypes = BuildSelectedBackgroundTypes();
            }

        private void UpdateParentNodeState(TreeNode node)
            {
            TreeNode parent = node.Parent;
            while (parent != null)
                {
                bool? siblingState = GetSiblingNodeState(parent.Nodes);
                if (siblingState.HasValue)
                    {
                    parent.Checked = siblingState.Value;
                    }
                else
                    {
                    SetNodeStateToMixed(parent);
                    }
                
                parent = parent.Parent;
                }
            }

        private void SetNodeStateToMixed(TreeNode node)
            {
            var lParam = new TV_ITEM 
                {
                mask = 0x18,            // TVIF_HANDLE | TVIF_STATE
                hItem = node.Handle,
                stateMask = 0xf000,     // TVIS_STATEIMAGEMASK
                state = 0x3000
                };
            // ReSharper disable once InconsistentNaming
            const int TVM_SETITEM = 0x110d;
            SendMessage(new HandleRef(this.BackgroundObjectTree, this.BackgroundObjectTree.Handle), TVM_SETITEM, 0, ref lParam);
            }

        private void CheckOrUncheckChildren(TreeNode node, bool checkNode)
            {
            foreach (TreeNode child in node.Nodes)
                {
                child.Checked = checkNode;
                CheckOrUncheckChildren(child, checkNode);
                }
            }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private bool? GetSiblingNodeState(TreeNodeCollection nodes)
            {
            var enumerator = nodes.GetEnumerator();
            enumerator.MoveNext();
            bool result = ((TreeNode) enumerator.Current).Checked;
            while (enumerator.MoveNext())
                {
                bool isChecked = ((TreeNode) enumerator.Current).Checked;
                if (result != isChecked)
                    return null;
                }
            return result;
            }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        // ReSharper disable once InconsistentNaming
        private struct TV_ITEM
            {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
            }

        private ImageList BuildStateImageList()
            {
            var stateImageList = new ImageList();

            var checkBoxStates = new[]
                {
                System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal,
                System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal,
                System.Windows.Forms.VisualStyles.CheckBoxState.MixedNormal
                };
            foreach (var checkBoxState in checkBoxStates)
                {
                Bitmap bmp = new Bitmap(16, 16);
                Graphics chkGraphics = Graphics.FromImage(bmp);
                CheckBoxRenderer.DrawCheckBox(chkGraphics, new Point(0, 1), checkBoxState);
                stateImageList.Images.Add(bmp);
                }

            return stateImageList;
            }

        private void chkHighlightDisplayElement_CheckedChanged(object sender, EventArgs e)
            {
            this._highlightDisplayElement = this.chkHighlightDisplayElement.Checked;
            if (this._highlightDisplayElement)
                this.AnimationTimer.Enabled = true;
            }

        private void cboHighlightBackground_SelectedIndexChanged(object sender, EventArgs e)
            {
            this._displayElementToHighlight = (int) this.cboHighlightBackground.SelectedValue;
            }

        private void map_MouseDown(object sender, MouseEventArgs e)
            {
            if (e.Button == MouseButtons.Left)
                {
                this._draggingStartPoint = e.Location;
                this._draggingTopLeftCell = new Point(this.map.FirstDisplayedCell.ColumnIndex, this.map.FirstDisplayedCell.RowIndex);
                }
            }

        private void map_MouseUp(object sender, MouseEventArgs e)
            {
            this._draggingInGrid = false;
            map.Cursor = Cursors.Arrow;
            }

        private void map_MouseMove(object sender, MouseEventArgs e)
            {
            if (e.Button != MouseButtons.Left)
                {
                if (this._draggingInGrid)
                    {
                    this._draggingInGrid = false;
                    map.Cursor = Cursors.Arrow;
                    }
                return;
                }

            var diffX = e.X - this._draggingStartPoint.X;
            var diffY = e.Y - this._draggingStartPoint.Y;
            if (!this._draggingInGrid && (Math.Abs(diffX) > SystemInformation.DragSize.Width || Math.Abs(diffY) > SystemInformation.DragSize.Height))
                {
                this._draggingInGrid = true;
                map.Cursor = Cursors.SizeAll;
                }

            if (this._draggingInGrid)
                {
                var columnWidth = map.Columns[0].Width;
                var rowHeight = map.Rows[0].Height;

                var columnsDiff = diffX / columnWidth;
                var rowsDiff = diffY / rowHeight;

                int row = this._draggingTopLeftCell.Y - rowsDiff;
                if (row < 0)
                    {
                    row = 0;
                    }
                if (row >= this.map.RowCount)
                    {
                    row = this.map.RowCount - 1;
                    }

                int column = this._draggingTopLeftCell.X - columnsDiff;
                if (column < 0)
                    {
                    column = 0;
                    }
                if (column >= this.map.ColumnCount)
                    {
                    column = this.map.ColumnCount - 1;
                    }

                this.map.FirstDisplayedCell = this.map.Rows[row].Cells[column];
                }
            }
        }
    }
