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
        private readonly SpriteBuilder _spriteBuilder = new SpriteBuilder();
        private readonly SquareProperties[,] _squareProperties = new SquareProperties[256, 256];
        private static readonly string[] BackgroundHandlerTypeList = BuildBackgroundHandlerTypeList();
        private static readonly string[] ObjectTypeList = BuildObjectTypeList();
        private static readonly IReadOnlyList<WaterLevelFromX> WaterLevelList = BuildWaterLevelsList();
        private static readonly List<Point> TeleportDestinations = BuildTeleportDestinations();
        private static readonly string[] SuckerTriggers = BuildSuckerTriggers();
        private static readonly string[] SuckerActions = BuildSuckerActions();
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

            // turn on double-buffering for the data grid view to reduce flickering
            var type = map.GetType();
            var prop = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (prop != null)
                prop.SetValue(this.map, true);

            this._stopwatch.Start();

            var checkOnMappedSquares = new ConcurrentBag<int>();
            Parallel.For(0, 0x10000, i => { CalculateSquareContents(i, checkOnMappedSquares); });
            if (checkOnMappedSquares.Count != 1024)
                throw new InvalidOperationException("1024 positions are expected to be explicitly mapped, rather than " + checkOnMappedSquares.Count);
            if (checkOnMappedSquares.Distinct().Count() != 1024)
                throw new InvalidOperationException("All 1024 explicit map positions should be unique.");

            //string findResults = string.Empty;
            //for (int y = 0; y < 256; y++)
            //    {
            //    for (int x = 0; x < 256; x++)
            //        {
            //        var squareProperties = this._squareProperties[x, y];
            //        if ((squareProperties.BackgroundAfterPalette & 0x3f) == lookFor)
            //            {
            //            findResults += $"({x:x2},{y:x2})\r\n";
            //            }
            //        }
            //    }

            //Trace.WriteLine(findResults);
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

        private void CalculateSquareContents(int squareCoordinates, ConcurrentBag<int> hashOfPositions)
            {
            byte y = (byte) (squareCoordinates >> 8);
            byte x = (byte) (squareCoordinates & 0xFF);

            var generatedBackground = CalculateBackground.GetBackground(x, y);
            var data = new SquareProperties {X = x, Y = y, GeneratedBackground = generatedBackground};
            if (generatedBackground.IsMappedData)
                {
                hashOfPositions.Add(generatedBackground.PositionInMappedData);
                }

            var backgroundProperties = CalculateBackgroundObjectData.GetBackgroundObjectData(generatedBackground.Result, x);
            data.BackgroundObjectData = backgroundProperties;

            byte background = (byte) (data.BackgroundObjectData.Result & 0x3f);
            byte orientation = (byte) (data.BackgroundObjectData.Result & 0xc0);
            if (data.BackgroundObjectData.IsBackgroundEvent)
                {
                data.BackgroundHandlerType = BackgroundHandlerTypeList[background];
                if (backgroundProperties.Number.HasValue)
                    {
                    var backgroundObjectType = (BackgroundObjectType) background;
                    data.BackgroundDescription = DescribeBackground(backgroundObjectType, backgroundProperties.Type, backgroundProperties.Data, orientation);
                    //if (backgroundObjectType == BackgroundObjectType.InvisibleSwitch || backgroundObjectType == BackgroundObjectType.Switch)
                    //    {
                    //    var objectData = data.BackgroundObjectData.Data.Value;
                    //    string action;
                    //    if (backgroundObjectType == BackgroundObjectType.InvisibleSwitch)
                    //        {
                    //        action = (objectData & 1) == 0 ? "clear" : "set";
                    //        }
                    //    else
                    //        {
                    //        objectData = (byte) (objectData & 0x7f);
                    //        action = "toggle";
                    //        }
                    //    Debug.WriteLine($"{backgroundObjectType}\t{data.X:X},{data.Y:X}\t{data.BackgroundObjectData.Id:X}\t{objectData >> 3:X}\t{Convert.ToString(objectData >> 1 & 3, 2)}\t{action}");
                    //    }
                    }
                }

            data.PaletteData = CalculatePalette.GetPalette(ref background, ref orientation, x, y);
            data.Background = (byte) (background ^ orientation);

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
                    Debug.Assert(data.HasValue);
                    var triggeredBy = type.Value == 0x80 ? "anything" : ObjectTypeList[type.Value];
                    var switchEffectsIndex = data.Value >> 3;
                    var bitsToEffect = data.Value >> 1 & 3;
                    Debug.Assert(bitsToEffect == 1 || bitsToEffect == 2);
                    string effect;
                    if ((data.Value & 1) == 0)
                        {
                        // switch clears bits
                        effect = "clear bit ";
                        switch (bitsToEffect)
                            {
                            case 1:
                                effect += "0 (e.g. unlock doors)";
                                break;
                            case 2:
                                effect += "1 (e.g. close doors)";
                                break;
                            }
                        }
                    else
                        {
                        // switch sets bits
                        effect = "set bit ";
                        switch (bitsToEffect)
                            {
                            case 1:
                                effect += "0 (e.g. lock doors)";
                                break;
                            case 2:
                                effect += "1 (e.g. open doors)";
                                break;
                            }
                        }
                    return $"Triggered by {triggeredBy}, switch effects index 0x{switchEffectsIndex}, effect is to {effect}";
                    }
                
                case BackgroundObjectType.Teleport:
                    {
                    Debug.Assert(data.HasValue);
                    var active = (data.Value & 0x1) == 0 ? "active" : "inactive";
                    var destination = (data.Value >> 4 & 0x7);
                    var key = ((data.Value & 0x7f) + 0x60) >> 5;
                    Debug.Assert(key <= 6 && key >= 3);
                    var keyDescription = GetKeyDescription(key);
                    return $"teleport: {active}, destination = 0x{destination:X}, key = {keyDescription}";
                    }
                
                case BackgroundObjectType.ObjectFromData:
                    {
                    Debug.Assert(data.HasValue);
                    byte dataType = (byte) (data.Value & 0x7f);
                    if (dataType == 0xc)
                        Debugger.Break();
                    return ObjectTypeList[dataType];
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
                    var result = ObjectTypeList[type.Value];
                    if (type.Value == 0xd)
                        {
                        Debug.Assert(data.HasValue);
                        result += $" to {SuckerActions[data.Value & 0x7f]}, {SuckerTriggers[data.Value & 0x7f]}";
                        }

                    return result;
                    }

                case BackgroundObjectType.Switch:
                    {
                    Debug.Assert(data.HasValue);
                    var switchEffectsIndex = data.Value >> 3 & 0xf;
                    var bitsToEffect = data.Value >> 1 & 3;
                    Debug.Assert(bitsToEffect > 0 && bitsToEffect <= 3);
                    string effect = null;
                    switch (bitsToEffect)
                        {
                        case 1:
                            effect = "toggle bit 0 (e.g. lock/unlock doors)";
                            break;
                        case 2:
                            effect = "toggle bit 1 (e.g. open/close doors)";
                            break;
                        case 3:
                            effect = "toggle bits 0 and 1 (e.g. lock/unlock and open/close doors)";
                            break;
                        }
                    return $"Switch effects index 0x{switchEffectsIndex}, effect is to {effect}";
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
                    int xVelocity = (data.Value << 4) & 0xff;
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
                    return null;        // mushrooms do not have data
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

            var colourScheme = Palette.FromByte(SpriteBuilder.ObjectPaletteLookup[objectType]);
            result += $" {colourScheme}";
            return result;
            }

        private void map_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || (e.PaintParts & DataGridViewPaintParts.ContentForeground) == 0)
                {
                return;
                }

            var timeElapsed = this._stopwatch.Elapsed;

            var squareProperties = this._squareProperties[e.ColumnIndex, e.RowIndex];

            var waterLevelType = GetWaterLevelType(squareProperties.X, squareProperties.Y);
            this._spriteBuilder.Clear(waterLevelType);

            switch ((BackgroundObjectType) (squareProperties.Background & 0x3f))
                {
                case BackgroundObjectType.InvisibleSwitch:
                    {
                    this._spriteBuilder.AddBitmap(Resources.HiddenSwitch);
                    break;
                    }

                case BackgroundObjectType.ObjectFromData:
                    {
                    Debug.Assert(squareProperties.BackgroundObjectData.Data.HasValue);
                    this._spriteBuilder.BuildObjectFromDataSprite(squareProperties.BackgroundObjectData.Data.Value, squareProperties.Background);
                    break;
                    }
                
                case BackgroundObjectType.Door:
                case BackgroundObjectType.StoneDoor:
                    {
                    Debug.Assert(squareProperties.BackgroundObjectData.Data.HasValue);
                    this._spriteBuilder.BuildDoor(squareProperties.Background, squareProperties.BackgroundObjectData.Data.Value);
                    break;
                    }

                case BackgroundObjectType.ObjectFromTypeWithWall:
                case BackgroundObjectType.ObjectFromTypeWithFoliage:
                    {
                    Debug.Assert(squareProperties.BackgroundObjectData.Type.HasValue);
                    Debug.Assert(squareProperties.BackgroundObjectData.Data.HasValue);
                    this._spriteBuilder.BuildObjectFromDataSprite(squareProperties.BackgroundObjectData.Type.Value, squareProperties.Background, squareProperties.BackgroundObjectData.Data.Value);
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.Background, squareProperties.PaletteData.Palette);
                    break;
                    }

                case BackgroundObjectType.ObjectFromType:
                    {
                    Debug.Assert(squareProperties.BackgroundObjectData.Type.HasValue);
                    Debug.Assert(squareProperties.BackgroundObjectData.Data.HasValue);
                    this._spriteBuilder.BuildObjectFromDataSprite(squareProperties.BackgroundObjectData.Type.Value, squareProperties.Background, squareProperties.BackgroundObjectData.Data.Value);
                    break;
                    }
            
                case BackgroundObjectType.Switch:
                    {
                    this._spriteBuilder.BuildObjectFromDataSprite(0x42, squareProperties.Background);
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.Background, squareProperties.PaletteData.Palette);
                    break;
                    }

                case BackgroundObjectType.ObjectEmergingFromBush:
                case BackgroundObjectType.ObjectEmergingFromPipe:
                    {
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.Background, squareProperties.PaletteData.Palette);
                    if (squareProperties.BackgroundObjectData.Number.HasValue)
                        {
                        Debug.Assert(squareProperties.BackgroundObjectData.Data.HasValue);
                        bool hasTree = (squareProperties.BackgroundObjectData.Data.Value & 0x80) != 0;
                        if (hasTree)
                            {
                            this._spriteBuilder.BuildObjectSprite(0x40, squareProperties.Background, (0x40, 0x40));
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
                    this._spriteBuilder.BuildBackgroundSprite(squareProperties.Background, squareProperties.PaletteData.Palette);
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

                if (this._highlightMappedDataSquares && squareProperties.GeneratedBackground.IsMappedData)
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

                if (this._highlightDisplayElement && (squareProperties.Background & 0x3f) == this._displayElementToHighlight)
                    {
                    using (Brush brush = new SolidBrush(Color.FromArgb(0x80, Color.White)))
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

            if ((squareProperties.Background & 0x3f) == lookFor)
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
                        return WaterLevelType.UnderWater;
                    if (y == waterLevel.WaterLevel)
                        return WaterLevelType.OnWaterLine;
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
            if (this._highlightMappedDataSquares && squareProperties.GeneratedBackground.IsMappedData)
                return true;
            if (this._highlightBackgroundObjects && DoesSquareMatchBackgroundObjectCriteria(squareProperties))
                return true;
            if (this._highlightDisplayElement && (squareProperties.Background & 0x3f) == this._displayElementToHighlight)
                return true;
            return false;
            }

        private bool DoesSquareMatchBackgroundObjectCriteria(SquareProperties squareProperties)
            {
            var type = (byte) (squareProperties.Background & 0x3f);
            if (type > 0xf || !this._selectedBackgroundObjectTypes.Contains(type))
                {
                return false;
                }

            var result = type > 0xc || squareProperties.BackgroundObjectData.Number.HasValue;
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
            byte background = (byte) (squareValue.Background & 0x3f);
            if (!squareValue.BackgroundObjectData.Id.HasValue)
                {
                backgroundObjectInfo = background < 0x10 
                    ? $"Background event used as scenery: {squareValue.BackgroundHandlerType}" 
                    : "Scenery only";

                if (squareValue.BackgroundDescription != null)
                    {
                    backgroundObjectInfo += "\r\n" + squareValue.BackgroundDescription;
                    }
                }
            else
                {
                backgroundObjectInfo = $"Background object id: {squareValue.BackgroundObjectData.Id:X2}";
                backgroundObjectInfo += $"\r\nBackground handler: {squareValue.BackgroundHandlerType}";
                if (squareValue.BackgroundObjectData.Type.HasValue)
                    {
                    backgroundObjectInfo += "\r\n" +
                        $"Object type: {squareValue.BackgroundObjectData.Type.Value:X2}";
                    }
                backgroundObjectInfo += "\r\n" +
                    $"Description: {squareValue.BackgroundDescription}";

                if (squareValue.BackgroundObjectData.Data.HasValue)
                    {
                    backgroundObjectInfo += "\r\n" +
                                            $"Data: {squareValue.BackgroundObjectData.Data.Value:X2}";
                    }
                }

            this.txtBackgroundObjectInfo.Text = backgroundObjectInfo;
            SetTextBoxHeight(txtBackgroundObjectInfo);
            }

        private void SetSpriteInfo(SquareProperties squareValue)
            {
            byte sprite = (byte) (SpriteBuilder.BackgroundSpriteLookup[squareValue.Background & 0x3f] & 0x7f);
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
            byte offsetAlongY = (byte) (SpriteBuilder.BackgroundYOffsetLookup[squareValue.Background & 0x3f] & 0xf0);
            spriteInfo += "\r\n" +
                          $"Y offset: {offsetAlongY:X2}\r\n";

            bool rightAlign = (squareValue.Background & 0x80) != 0;
            bool bottomAlign = (squareValue.Background & 0x40) != 0;
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
            string paletteInfo;
            if (squareValue.PaletteData.BackgroundPalette > 6)
                {
                Debug.Assert(squareValue.PaletteData.BackgroundPalette == squareValue.PaletteData.Palette.PaletteByte);
                paletteInfo =
                    $"Invariable palette for background: {squareValue.PaletteData.Palette}";
                }
            else
                {
                paletteInfo = 
                    $"Palette algorithm for background: {squareValue.PaletteData.BackgroundPalette:X2} \r\n" +
                    $"Derived palette: {squareValue.PaletteData.Palette}";
                }

            if (squareValue.BackgroundObjectData.Result != squareValue.Background)
                {
                paletteInfo +=
                    $"\r\nRevised background: {squareValue.Background:x2}\r\n" +
                    $"Appearance: {squareValue.Background & 0x3f:X2}, " +
                    $"Orientation: {squareValue.Background & 0xc0:X2} " +
                    $"({DescribeOrientation(squareValue.Background)})";
                }

            this.txtPaletteInfo.Text = paletteInfo;
            SetTextBoxHeight(this.txtPaletteInfo);
            }

        private void SetListOverrideInfo(SquareProperties squareValue)
            {
            var backgroundOverrideList = squareValue.GeneratedBackground.Result & 0x3f;
            if (backgroundOverrideList >= 0x9)
                {
                // ReSharper disable once LocalizableElement
                this.txtListOverrideInfo.Text = "No override";
                }
            else
                {
                var listOverride =
                    $"Override list: {backgroundOverrideList}\r\n";
                if (squareValue.BackgroundObjectData.IsHashDefault)
                    {
                    listOverride +=
                        $"Default for list: {squareValue.BackgroundObjectData.Result:X2}\r\n";
                    }
                else
                    {
                    Debug.Assert(squareValue.BackgroundObjectData.Number.HasValue);
                    listOverride +=
                        $"Background object number: {squareValue.BackgroundObjectData.Number.Value:X2}\r\n" +
                        $"Value from list: {squareValue.BackgroundObjectData.Result:x2}\r\n";
                    }

                byte contents = (byte) (squareValue.BackgroundObjectData.Result & 0x3f);
                byte orientation = (byte) (squareValue.BackgroundObjectData.Result & 0xc0);
                listOverride +=
                    $"{(contents <= 0xf ? "Handler" : "Appearance")}: {contents:X2} \r\n" +
                    $"Orientation: {orientation:X2} ({DescribeOrientation(orientation)})";
                this.txtListOverrideInfo.Text = listOverride;
                }
            SetTextBoxHeight(this.txtListOverrideInfo);
            }

        private void SetMappedOrGeneratedInfo(SquareProperties squareValue)
            {
            var mappedGeneratedInfo =
                squareValue.GeneratedBackground.IsMappedData
                    ? $"Mapped location index: {squareValue.GeneratedBackground.PositionInMappedData}\r\n"
                    : string.Empty;
            byte calculatedBackground = squareValue.GeneratedBackground.Result;
            mappedGeneratedInfo +=
                $"{(squareValue.GeneratedBackground.IsMappedData ? "Explicit" : "Generated")} background {calculatedBackground:X2}\r\n";
            if (!squareValue.GeneratedBackground.IsMappedData || (squareValue.GeneratedBackground.Result & 0x3f) >= 0x9))
                {
                mappedGeneratedInfo +=
                    $"Appearance: {calculatedBackground & 0x3f:X2}, " +
                    $"Orientation: {calculatedBackground & 0xc0:X2} " +
                    $"({DescribeOrientation(calculatedBackground)})";
                }
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
                "tough bush",
                "sucker/blower",
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
                // ReSharper disable CommentTypo
                mask = 0x18,            // TVIF_HANDLE | TVIF_STATE
                hItem = node.Handle,
                stateMask = 0xf000,     // TVIS_STATEIMAGEMASK
                // ReSharper restore CommentTypo
                state = 0x3000
                };
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once IdentifierTypo
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

        private static string[] BuildSuckerTriggers()
            {
            return new []
                {
                "always active", 
                "triggered by horizontal stone door", 
                "triggered by wasp", 
                "triggered by coronium boulder & yellow ball", 
                "triggered by piranha", 
                "always active", 
                "triggered by coronium boulder & yellow ball", 
                "triggered by piranha", 
                "triggered by worm"
                };
            }

        private static string[] BuildSuckerActions()
            {
            var palettesAndActions = SpriteBuilder.BuildSuckerPalettesAndAction();
            return palettesAndActions.Select(p => (p & 1) == 1 ? "suck" : "blow").ToArray();
            }
        }
    }
