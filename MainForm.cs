using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private readonly SquareProperties[,] _squareProperties = new SquareProperties[256, 256];
        private static readonly string[] ObjectTypeList = BuildObjectTypeList();
        private static readonly IReadOnlyList<WaterLevelFromX> WaterLevelList = BuildWaterLevelsList();
        private static readonly List<WorldSquare> TeleportDestinations = BuildTeleportDestinations();
        private static readonly string[] SuckerTriggers = BuildSuckerTriggers();
        private static readonly SuckerAction[] SuckerActions = BuildSuckerActions();
        private static readonly byte[] GargoyleBulletList = BuildGargoyleBulletList();
        private static readonly List<GameObject> GameObjects = BuildGameObjectList();
        private static readonly List<byte[]> SwitchEffectsList = BuildSwitchEffectsList();
        private decimal _zoom;
        private byte _lookFor = 0xff;
        private byte _backgroundElementToHighlight;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private List<byte> _selectedBackgroundObjectTypes = new List<byte>();

        private Point _draggingStartPoint;
        private Point _draggingTopLeftCell;
        private bool _draggingInGrid;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref TV_ITEM lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, int lParam);

        public MainForm()
            {
            InitializeComponent();

            this.BackgroundObjectTree.Nodes[0].ExpandAll();
            this.BackgroundObjectTree.StateImageList = BuildStateImageList();

            this.WorldMap.ColumnCount = 256;
            this.WorldMap.RowCount = 256;

            var items = new Dictionary<decimal, string>
                {
                    { 0.5m, "50%" },
                    { 1.0m, "100%" },
                    { 2.0m, "200%" },
                    { 3.0m, "300%" },
                    { 4.0m, "400%" },
                    { 5.0m, "500%" },
                    { 6.0m, "600%" },
                    { 7.0m, "700%" },
                    { 8.0m, "800%" }
                };
            this.ZoomLevelDropDown.DisplayMember = "Value";
            this.ZoomLevelDropDown.ValueMember = "Key";
            this.ZoomLevelDropDown.DataSource = new BindingSource(items, null);
            this.ZoomLevelDropDown.SelectedIndex = 1;
            this._zoom = 1m;

            var backgroundItems = Enumerable.Range(0x10, 0x30).ToDictionary(item => (byte)item, item => $"0x{item:X}");
            this.BackgroundSceneryDropDown.DisplayMember = "Value";
            this.BackgroundSceneryDropDown.ValueMember = "Key";
            this.BackgroundSceneryDropDown.DataSource = new BindingSource(backgroundItems, null);
            this.HighlightBackgroundSceneryCheckBox.Checked = false;

            // turn on double-buffering for the data grid view to reduce flickering
            var type = this.WorldMap.GetType();
            var prop = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (prop != null)
                {
                prop.SetValue(this.WorldMap, true);
                }

            this._stopwatch.Start();
            this.AnimationTimer.Enabled = true;

            var checkOnMappedSquares = new ConcurrentBag<int>();
            //Parallel.For(0, 0x10000, i => { CalculateSquareContents(i, checkOnMappedSquares); });
            for (int i = 0; i < 0x10000; i++)
                {
                CalculateSquareContents(i, checkOnMappedSquares);
                }
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

        private void MainForm_Shown(object sender, EventArgs e)
            {
            ResetGrid();
            this.WorldMap.FirstDisplayedCell = this.WorldMap.Rows[0x39].Cells[0x97];
            this.WorldMap.CurrentCell = this.WorldMap.Rows[0x3b].Cells[0x9b];
            UpdateInfoOnCurrentCell();
            }

        private void ResetGrid()
            {
            // ReSharper disable InconsistentNaming
            // ReSharper disable IdentifierTypo
            const int WM_SETCURSOR = 0x20;
            const int HTCLIENT = 1;
            // ReSharper restore IdentifierTypo
            // ReSharper restore InconsistentNaming
            this.UseWaitCursor = true;
            Control ctrl = this.ActiveControl;
            SendMessage(new HandleRef(ctrl, ctrl.Handle), WM_SETCURSOR, ctrl.Handle, HTCLIENT);
            this.WorldMap.SuspendLayout();

            for (int i = 255; i >= 0; i--)
                {
                var value = i.ToString("X2");
                this.WorldMap.Rows[i].HeaderCell.Value = value;
                this.WorldMap.Columns[i].HeaderCell.Value = value;
                this.WorldMap.Rows[i].Height = (int)(32 * _zoom);
                this.WorldMap.Columns[i].Width = (int)(32 * _zoom);
                }

            this.WorldMap.ResumeLayout();
            this.UseWaitCursor = false;
            }

        private void CalculateSquareContents(int squareCoordinates, ConcurrentBag<int> hashOfPositions)
            {
            WorldSquare worldSquare = WorldSquare.FromInt(squareCoordinates);

            var generatedBackground = CalculateBackground.GetBackground(worldSquare.X, worldSquare.Y);
            if (generatedBackground.IsMappedData)
                {
                hashOfPositions.Add(generatedBackground.PositionInMappedData);
                }

            BackgroundOverride? backgroundOverride = CalculateBackgroundObjectData.GetBackgroundObjectData(generatedBackground.Result, worldSquare.X);

            var background = generatedBackground.Background;
            var orientation = generatedBackground.Orientation;
            if (backgroundOverride != null)
                {
                background = backgroundOverride.Background;
                orientation = backgroundOverride.Orientation;
                }

            var getPaletteResult = CalculatePalette.GetPalette(background, orientation, worldSquare.X, worldSquare.Y);

            var data = new SquareProperties(worldSquare, generatedBackground, backgroundOverride, getPaletteResult);

            if (data.FinalBackground <= 0xf)
                {
                var backgroundObjectType = (BackgroundObjectType) data.FinalBackground;
                data.BackgroundHandlerType = GetEnumDescription(backgroundObjectType);
                data.BackgroundDescription = DescribeBackgroundEvent(data);
                }

            var teleportDestination = TeleportDestinations.FindIndex(item => item == worldSquare);
            if (teleportDestination != -1)
                {
                data.BackgroundDescription = $"Teleport destination 0x{teleportDestination:X}\r\n" + (data.BackgroundDescription ?? string.Empty);
                }

            this._squareProperties[worldSquare.X, worldSquare.Y] = data;
            }

        private static string? DescribeBackgroundEvent(SquareProperties squareProperties)
            {
            var backgroundEvent = squareProperties.BackgroundOverride as BackgroundEvent;

            switch ((BackgroundObjectType) squareProperties.FinalBackground)
                {
                case BackgroundObjectType.InvisibleSwitch:
                    {
                    if (backgroundEvent == null)
                        throw new InvalidOperationException("BackgroundEvent not set for InvisibleSwitch");
                    if (backgroundEvent.ObjectType == null)
                        throw new InvalidOperationException("Type not set for InvisibleSwitch");
                    byte data = backgroundEvent.ObjectData;
                    byte type = backgroundEvent.ObjectType.Value;
                    var triggeredBy = type == 0x80 ? "anything" : ObjectTypeList[type];
                    var switchEffectsIndex = data >> 3;
                    var bitsToEffect = data >> 1 & 3;
                    Debug.Assert(bitsToEffect == 1 || bitsToEffect == 2);
                    string effect;
                    if ((data & 1) == 0)
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

                    byte[] objectsAffected = SwitchEffectsList[switchEffectsIndex];
                    string switchEffectsList = string.Join(", ", objectsAffected.Select(item => item.ToString("X2")));
                    return $"Triggered by {triggeredBy}, switch effects index 0x{switchEffectsIndex}, affecting {objectsAffected.Length} object(s) {switchEffectsList}, effect is to {effect}";
                    }

                case BackgroundObjectType.Teleport:
                    {
                    if (backgroundEvent == null)
                        throw new InvalidOperationException("BackgroundEvent not set for Teleport");
                    byte data = backgroundEvent.ObjectData;
                    var active = (data & 0x1) == 0 ? "active" : "inactive";
                    var destination = (data >> 4 & 0x7);
                    var key = ((data & 0x7f) + 0x60) >> 5;
                    Debug.Assert(key <= 6 && key >= 3);
                    var keyDescription = GetKeyDescription(key);
                    return $"teleport: {active}, destination = 0x{destination:X}, key = {keyDescription}";
                    }

                case BackgroundObjectType.ObjectFromData:
                    {
                    if (backgroundEvent == null)
                        throw new InvalidOperationException("BackgroundEvent not set for ObjectFromData");
                    byte data = backgroundEvent.ObjectData;
                    byte dataType = (byte)(data & 0x7f);
                    return ObjectTypeList[dataType];
                    }

                case BackgroundObjectType.Door:
                    {
                    if (backgroundEvent == null)
                        throw new InvalidOperationException("BackgroundEvent not set for Door");
                    byte data = backgroundEvent.ObjectData;
                    var locked = (data & 0x1) != 0;
                    var open = (data & 0x2) != 0;
                    var slowMoving = (data & 0x8) != 0;
                    var key = (data >> 4 & 0x7);
                    var keyDescription = GetKeyDescription(key);
                    var latchesOpen = (key & 0x3) != 0;
                    return $"door: locked = {locked}, open = {open}, slowMoving = {slowMoving}, key = {keyDescription}, latchesOpen = {latchesOpen}";
                    }

                case BackgroundObjectType.StoneDoor:
                    {
                    if (backgroundEvent == null)
                        throw new InvalidOperationException("BackgroundEvent not set for StoneDoor");
                    byte data = backgroundEvent.ObjectData;
                    var locked = (data & 0x1) != 0;
                    var open = (data & 0x2) != 0;
                    var slowMoving = (data & 0x8) != 0;
                    var key = (data >> 4 & 0x7);
                    Debug.Assert(key == 4 || key == 7);
                    var keyDescription = GetKeyDescription(key);
                    var latchesOpen = (key & 0x3) != 0;
                    return $"stone door: locked = {locked}, open = {open}, slowMoving = {slowMoving}, key = {keyDescription}, latchesOpen = {latchesOpen}";
                    }

                case BackgroundObjectType.ObjectFromTypeWithWall:
                case BackgroundObjectType.ObjectFromType:
                case BackgroundObjectType.ObjectFromTypeWithFoliage:
                    {
                    if (backgroundEvent == null)
                        throw new InvalidOperationException("BackgroundEvent not set for ObjectFromTypeXxx");
                    if (backgroundEvent.ObjectType == null)
                        throw new InvalidOperationException("Type not set for ObjectFromTypeXxx");
                    byte data = backgroundEvent.ObjectData;
                    byte type = backgroundEvent.ObjectType.Value;
                    var result = ObjectTypeList[type];
                    if (type == 0xd)
                        {
                        result += $" to {SuckerActions[data & 0x7f]}, {SuckerTriggers[data & 0x7f]}";
                        }
                    else if (type == 0x4 || type == 0x5)
                        {
                        bool isActive = (data & 0b11) == 0;
                        byte nestOccupants = (byte)((data & 0b1111100) >> 2);
                        result += $" containing {ObjectTypeList[nestOccupants]} ({(isActive ? "active" : "inactive")})";
                        }
                    else if (type == 0x1f || type == 0x20)
                        {
                        bool isActive = (data & 0b1) == 0;
                        byte fires = (byte)((data & 0b111110) >> 1);
                        result += $" fires {ObjectTypeList[fires]} ({(isActive ? "active" : "inactive")})";
                        }
                    else if (type == 0x28)
                        {
                        byte fires = GargoyleBulletList[data & 0x7f];
                        result += $" fires {ObjectTypeList[fires]}";
                        }

                    return result;
                    }

                case BackgroundObjectType.Switch:
                    {
                    if (backgroundEvent == null)
                        throw new InvalidOperationException("BackgroundEvent not set for Switch");
                    byte data = backgroundEvent.ObjectData;
                    var switchEffectsIndex = data >> 3 & 0xf;
                    var bitsToEffect = data >> 1 & 3;
                    Debug.Assert(bitsToEffect > 0 && bitsToEffect <= 3);
                    string? effect = null;
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

                    byte[] objectsAffected = SwitchEffectsList[switchEffectsIndex];
                    string switchEffectsList = string.Join(", ", objectsAffected.Select(item => item.ToString("X2")));
                    return $"Switch effects index 0x{switchEffectsIndex}, affecting {objectsAffected.Length} object(s) {switchEffectsList}, effect is to {effect}";
                    }

                case BackgroundObjectType.ObjectEmergingFromBush:
                case BackgroundObjectType.ObjectEmergingFromPipe:
                    {
                    if (backgroundEvent == null)
                        {
                        return "scenery only";
                        }
                    if (backgroundEvent.ObjectType == null)
                        throw new InvalidOperationException("Type not set for ObjectEmergingFromXxx");
                    byte data = backgroundEvent.ObjectData;
                    byte type = backgroundEvent.ObjectType.Value;
                    int countOfObjects = (data & 0x7c) >> 2;
                    string displayType;

                    bool spawnedObjectEmergesImmediately = (squareProperties.FinalOrientation & 0x80) != 0;
                    bool hasTree = (data & 0x80) != 0;
                    bool enabled = (data & 0x03) == 0;

                    if (spawnedObjectEmergesImmediately && hasTree)
                        displayType = "emerges immediately from tree";
                    else if (spawnedObjectEmergesImmediately)
                        displayType = "emerges immediately";
                    else if (hasTree)
                        displayType = "emerges sporadically from tree";
                    else
                        displayType = "emerges sporadically";
                    var enabledDisplay = enabled ? "enabled" : "disabled";

                    return $"{countOfObjects} x {ObjectTypeList[type]} {displayType} {enabledDisplay}";
                    }

                case BackgroundObjectType.FixedWind:
                    {
                    if (backgroundEvent == null)
                        {
                        return "scenery only";
                        }
                    byte data = backgroundEvent.ObjectData;
                    int yVelocity = data;
                    if (yVelocity >= 128)
                        yVelocity -= 256;
                    int xVelocity = (data << 4) & 0xff;
                    if (xVelocity >= 128)
                        xVelocity -= 256;
                    return $"Fixed wind velocity ({xVelocity}, {yVelocity})";
                    }

                case BackgroundObjectType.EngineThruster:
                    return "*** engine tbd ***";

                case BackgroundObjectType.Water:
                    return null; // water does not have data

                case BackgroundObjectType.RandomWind:
                    return null; // random wind does not have data

                case BackgroundObjectType.Mushrooms:
                    return null; // mushrooms do not have data
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

        private void WorldMap_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || (e.PaintParts & DataGridViewPaintParts.ContentForeground) == 0)
                {
                return;
                }

            var timeElapsed = this._stopwatch.Elapsed;

            var squareProperties = this._squareProperties[e.ColumnIndex, e.RowIndex];

            var waterLevelType = GetWaterLevelType(squareProperties.WorldSquare);
            var spriteBuilder = SpriteBuilder.Start(waterLevelType);

            var isTeleportDestination = TeleportDestinations.Contains(squareProperties.WorldSquare);
            if (isTeleportDestination && this.ShowVisualOverlaysCheckBox.Checked)
                {
                spriteBuilder = spriteBuilder.AddBitmap(Resources.TeleportTarget);
                }

            var backgroundObjectType = (BackgroundObjectType) squareProperties.FinalBackground;
            switch (backgroundObjectType)
                {
                case BackgroundObjectType.InvisibleSwitch:
                    {
                    if (this.ShowVisualOverlaysCheckBox.Checked)
                        {
                        spriteBuilder = spriteBuilder.AddBitmap(Resources.HiddenSwitch);
                        }

                    break;
                    }

                case BackgroundObjectType.ObjectFromData:
                    {
                    var backgroundEvent = squareProperties.BackgroundOverride as BackgroundEvent;
                    if (backgroundEvent == null)
                        throw new InvalidOperationException();
                    spriteBuilder = spriteBuilder.AddObjectFromDataSprite(backgroundEvent.ObjectData, squareProperties.FinalOrientation);
                    break;
                    }

                case BackgroundObjectType.Door:
                case BackgroundObjectType.StoneDoor:
                    {
                    var backgroundEvent = squareProperties.BackgroundOverride as BackgroundEvent;
                    if (backgroundEvent == null)
                        throw new InvalidOperationException();
                    spriteBuilder = spriteBuilder.BuildDoor(squareProperties.FinalBackground, squareProperties.FinalOrientation, backgroundEvent.ObjectData);
                    break;
                    }

                case BackgroundObjectType.ObjectFromTypeWithWall:
                case BackgroundObjectType.ObjectFromType:
                case BackgroundObjectType.ObjectFromTypeWithFoliage:
                    {
                    var backgroundEvent = squareProperties.BackgroundOverride as BackgroundEvent;
                    if (backgroundEvent == null || backgroundEvent.ObjectType == null)
                        throw new InvalidOperationException();
                    var objectData = backgroundEvent.ObjectData;
                    var objectType = backgroundEvent.ObjectType.Value;
                    spriteBuilder = spriteBuilder.AddObjectFromDataSprite(objectType, squareProperties.FinalOrientation, objectData);
                    if (backgroundObjectType != BackgroundObjectType.ObjectFromType)
                        {
                        spriteBuilder = spriteBuilder.AddBackgroundSprite(squareProperties.FinalBackground, squareProperties.FinalOrientation, squareProperties.GetPaletteResult.PaletteData.Palette);
                        }

                    if (objectType == 0x4 || objectType == 0x5)
                        {
                        byte nestOccupants = (byte)((objectData & 0b1111100) >> 2);
                        var orientation = (byte)(squareProperties.FinalOrientation & 0b0100_0000);
                        if (backgroundObjectType == BackgroundObjectType.ObjectFromTypeWithWall)
                            {
                            orientation ^= 0b0100_0000;
                            }

                        spriteBuilder = spriteBuilder.AddEmergingObjectSprite(nestOccupants, true, orientation);
                        }
                    else if (objectType == 0x1f || objectType == 0x20)
                        {
                        byte fires = (byte)((objectData & 0b111110) >> 1);
                        spriteBuilder = spriteBuilder.AddEmergingObjectSprite(fires, false, 0);
                        }
                    else if (objectType == 0x28)
                        {
                        byte fires = GargoyleBulletList[objectData & 0x7f];
                        var orientation = (byte)(squareProperties.FinalOrientation & 0b0100_0000);
                        orientation ^= 0b0100_0000;
                        spriteBuilder = spriteBuilder.AddEmergingObjectSprite(fires, false, orientation);
                        }
                    else if (objectType == 0xd && this.ShowVisualOverlaysCheckBox.Checked)
                        {
                        var suckerAction = SuckerActions[objectData & 0x7f];
                        var orientation = (squareProperties.FinalOrientation & 0x40);
                        if (suckerAction == SuckerAction.Blow && orientation == 0)
                            {
                            spriteBuilder = spriteBuilder.AddBitmap(Resources.BlowUp);
                            }
                        else if (suckerAction == SuckerAction.Blow)
                            {
                            spriteBuilder = spriteBuilder.AddBitmap(Resources.BlowDown);
                            }
                        else if (orientation == 0)
                            {
                            spriteBuilder = spriteBuilder.AddBitmap(Resources.SuckDown);
                            }
                        else
                            {
                            spriteBuilder = spriteBuilder.AddBitmap(Resources.SuckUp);
                            }
                        }

                    break;
                    }

                case BackgroundObjectType.Switch:
                    {
                    spriteBuilder = spriteBuilder.AddObjectFromDataSprite(0x42, squareProperties.FinalOrientation);
                    spriteBuilder = spriteBuilder.AddBackgroundSprite(squareProperties.FinalBackground, squareProperties.FinalOrientation, squareProperties.GetPaletteResult.PaletteData.Palette);
                    break;
                    }

                case BackgroundObjectType.ObjectEmergingFromBush:
                case BackgroundObjectType.ObjectEmergingFromPipe:
                    {
                    if (squareProperties.BackgroundOverride is BackgroundEvent backgroundEvent)
                        {
                        bool hasTree = (backgroundEvent.ObjectData & 0x80) != 0;
                        if (hasTree)
                            {
                            spriteBuilder = spriteBuilder.AddObjectSprite(0x40, squareProperties.FinalOrientation, (0x40, 0x40));
                            }

                        if (backgroundEvent.ObjectType == null)
                            throw new InvalidOperationException();
                        spriteBuilder = spriteBuilder.AddEmergingObjectSprite(backgroundEvent.ObjectType.Value, hasTree, squareProperties.FinalOrientation);
                        }

                    spriteBuilder = spriteBuilder.AddBackgroundSprite(squareProperties.FinalBackground, squareProperties.FinalOrientation, squareProperties.GetPaletteResult.PaletteData.Palette);
                    break;
                    }

                case BackgroundObjectType.FixedWind:
                    {
                    if (this.ShowVisualOverlaysCheckBox.Checked)
                        {
                        spriteBuilder = spriteBuilder.AddBitmap(Resources.FixedWind);
                        }

                    break;
                    }

                case BackgroundObjectType.RandomWind:
                    {
                    if (this.ShowVisualOverlaysCheckBox.Checked)
                        {
                        spriteBuilder = spriteBuilder.AddBitmap(Resources.RandomWind);
                        }

                    break;
                    }

                case (BackgroundObjectType)0x12:
                    {
                    spriteBuilder = spriteBuilder.AddBackgroundSprite(squareProperties.FinalBackground, squareProperties.FinalOrientation, squareProperties.GetPaletteResult.PaletteData.Palette);
                    if (this.ShowVisualOverlaysCheckBox.Checked)
                        {
                        spriteBuilder = spriteBuilder.AddBitmap(Resources.HollowPassage);
                        }

                    break;
                    }

                default:
                    {
                    spriteBuilder = spriteBuilder.AddBackgroundSprite(squareProperties.FinalBackground, squareProperties.FinalOrientation, squareProperties.GetPaletteResult.PaletteData.Palette);
                    break;
                    }
                }

            foreach (GameObject item in GameObjects.Where(go => go.X == e.ColumnIndex && go.Y == e.RowIndex))
                {
                spriteBuilder = spriteBuilder.AddObjectSprite(item.ObjectType, 0, (item.LowX, item.LowY));
                }

            Rectangle destinationRectangle = new Rectangle(e.CellBounds.Left, e.CellBounds.Top - 1, (int)(32 * _zoom), (int)(32 * _zoom));
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            e.Graphics.DrawImage(spriteBuilder.Result, destinationRectangle);

            if (squareProperties.NextAnimationFrame.HasValue || ShouldSquareBeAnimated(squareProperties))
                {
                AdvanceAnimation(squareProperties, timeElapsed);
                }

            if (squareProperties.NextAnimationFrame.HasValue)
                {
                // orange outline
                if (this.HighlightMappedDataCheckBox.Checked && squareProperties.GeneratedBackground.IsMappedData)
                    {
                    using Brush brush = new SolidBrush(Color.FromArgb(128, Color.Orange));
                    using Pen pen = new Pen(brush, squareProperties.AnimationFrame);
                    Rectangle r = e.CellBounds;
                    r.Inflate(-1, -1);
                    e.Graphics.DrawRectangle(pen, r);
                    }

                // green outline
                if (this.HighlightBackgroundOverridesCheckBox.Checked && squareProperties.BackgroundOverride != null)
                    {
                    using Brush brush = new SolidBrush(Color.FromArgb(128, Color.LightGreen));
                    using Pen pen = new Pen(brush, squareProperties.AnimationFrame);
                    Rectangle r = e.CellBounds;
                    r.Inflate(-2, -2);
                    e.Graphics.DrawRectangle(pen, r);
                    }

                // blue circle
                if (this.HighlightBackgroundEventsCheckBox.Checked && DoesSquareMatchBackgroundObjectCriteria(squareProperties))
                    {
                    using Brush brush = new SolidBrush(Color.FromArgb(128, Color.Aqua));
                    using Pen pen = new Pen(brush, squareProperties.AnimationFrame);
                    Rectangle r = e.CellBounds;
                    r.Inflate(-1, -1);
                    e.Graphics.DrawEllipse(pen, r);
                    }

                // white diamond
                if (this.HighlightBackgroundSceneryCheckBox.Checked && squareProperties.FinalBackground == this._backgroundElementToHighlight)
                    {
                    using Brush brush = new SolidBrush(Color.FromArgb(0x80, Color.White));
                    using Pen pen = new Pen(brush, squareProperties.AnimationFrame);
                    Point topMiddle = e.CellBounds.Location + new Size(e.CellBounds.Width / 2, 0);
                    Point leftMiddle = e.CellBounds.Location + new Size(0, e.CellBounds.Height / 2);
                    Point bottomMiddle = e.CellBounds.Location + new Size(e.CellBounds.Width / 2, e.CellBounds.Height);
                    Point rightMiddle = e.CellBounds.Location + new Size(e.CellBounds.Width, e.CellBounds.Height / 2);
                    e.Graphics.DrawLine(pen, topMiddle, leftMiddle);
                    e.Graphics.DrawLine(pen, leftMiddle, bottomMiddle);
                    e.Graphics.DrawLine(pen, bottomMiddle, rightMiddle);
                    e.Graphics.DrawLine(pen, rightMiddle, topMiddle);
                    }

                // white solid square
                if (this.ShowVisualOverlaysCheckBox.Checked && IsSquareAffectedByCurrentlySelectedSwitch(squareProperties))
                    {
                    var a = 0xc0;
                    using Brush brush = new SolidBrush(Color.FromArgb(a, Color.White));
                    Rectangle r = e.CellBounds;
                    int length = (int)(-4 * squareProperties.AnimationFrame * this._zoom);
                    r.Inflate(length, length);
                    e.Graphics.FillRectangle(brush, r);
                    }
                }

            if (this.WorldMap.SelectedCells.Count != 0)
                {
                if (e.RowIndex == this.WorldMap.SelectedCells[0].RowIndex && e.ColumnIndex == this.WorldMap.SelectedCells[0].ColumnIndex)
                    {
                    using Brush brush = new SolidBrush(Color.FromArgb(0x80, Color.DarkOrchid));
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                    }
                }

            if (squareProperties.FinalBackground == _lookFor)
                {
                // to be implemented
                }

            e.Handled = true;
            }

        private static WaterLevelType GetWaterLevelType(WorldSquare worldSquare)
            {
            var i = WaterLevelList.Count;
            while (true)
                {
                i--;
                var waterLevel = WaterLevelList[i];
                if (worldSquare.X >= waterLevel.FromX)
                    {
                    if (worldSquare.Y > waterLevel.WaterLevel)
                        return WaterLevelType.UnderWater;
                    if (worldSquare.Y == waterLevel.WaterLevel)
                        return WaterLevelType.OnWaterLine;
                    return WaterLevelType.AboveWater;
                    }
                }
            }

        // called from WorldMap_CallPainting
        private void AdvanceAnimation(SquareProperties squareProperties, TimeSpan timeElapsed)
            {
            TimeSpan frameLength = TimeSpan.FromMilliseconds(100);

            if (squareProperties.NextAnimationFrame.HasValue)
                {
                TimeSpan elapsed = timeElapsed - squareProperties.NextAnimationFrame.Value;
                int framesMoved = (int)elapsed.TotalMilliseconds / (int)frameLength.TotalMilliseconds;
                squareProperties.AnimationFrame = (squareProperties.AnimationFrame + framesMoved) % 5;
                if (ShouldSquareBeAnimated(squareProperties))
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

        // This determines if, given the current options in play, the specified square
        // should be animated. It does not consider whether it is being animated.
        private bool ShouldSquareBeAnimated(SquareProperties squareProperties)
            {
            if (this.HighlightMappedDataCheckBox.Checked && squareProperties.GeneratedBackground.IsMappedData)
                {
                return true;
                }

            if (this.HighlightBackgroundOverridesCheckBox.Checked && squareProperties.BackgroundOverride != null)
                {
                return true;
                }

            if (this.HighlightBackgroundEventsCheckBox.Checked && DoesSquareMatchBackgroundObjectCriteria(squareProperties))
                {
                return true;
                }

            if (this.HighlightBackgroundSceneryCheckBox.Checked && squareProperties.FinalBackground == this._backgroundElementToHighlight)
                {
                return true;
                }

            if (IsSquareAffectedByCurrentlySelectedSwitch(squareProperties))
                {
                return true;
                }

            return false;
            }

        private bool IsSquareAffectedByCurrentlySelectedSwitch(SquareProperties squareProperties)
            {
            var switchEffects = GetSwitchEffectsForSelectedSquare();
            if (!switchEffects.Any())
                return false;

            byte? backgroundObjectId = (squareProperties.BackgroundOverride as BackgroundEvent)?.DataIndex;
            if (!backgroundObjectId.HasValue)
                return false;

            var result = switchEffects.Contains(backgroundObjectId.Value);
            return result;
            }

        private byte[] GetSwitchEffectsForSelectedSquare()
            {
            byte[] emptyList = Array.Empty<byte>();
            if (this.WorldMap.SelectedCells.Count == 0)
                return emptyList;

            var rowIndex = this.WorldMap.SelectedCells[0].RowIndex;
            var columnIndex = this.WorldMap.SelectedCells[0].ColumnIndex;
            var selectedSquareProperties = this._squareProperties[columnIndex, rowIndex];
            var selectedBackground = selectedSquareProperties.FinalBackground;
            if (selectedBackground != (byte) BackgroundObjectType.InvisibleSwitch && selectedBackground != (byte) BackgroundObjectType.Switch)
                return emptyList;

            var backgroundEvent = (BackgroundEvent) selectedSquareProperties.BackgroundOverride!;
            var switchEffectsIndex = backgroundEvent.ObjectData >> 3;
            if (selectedBackground == 0x08)
                {
                switchEffectsIndex &= 0xf;
                }

            return SwitchEffectsList[switchEffectsIndex];
            }

        private bool DoesSquareMatchBackgroundObjectCriteria(SquareProperties squareProperties)
            {
            var type = squareProperties.FinalBackground;
            if (type > 0xf || !this._selectedBackgroundObjectTypes.Contains(type))
                {
                return false;
                }

            // further check that the event type either doesn't need state data, or that it has state data
            var result = type > 0xc || squareProperties.BackgroundOverride is BackgroundEvent;
            return result;
            }

        private List<byte> BuildSelectedBackgroundTypes()
            {
            var result = GetSelectedNodes(this.BackgroundObjectTree.Nodes[0]).ToList();
            return result;
            }

        private static IEnumerable<byte> GetSelectedNodes(TreeNode node)
            {
            if (node.Nodes.Count == 0)
                {
                if (node.Checked)
                    {
                    var result = byte.Parse((string)node.Tag, NumberStyles.HexNumber);
                    yield return result;
                    }

                yield break;
                }

            foreach (TreeNode child in node.Nodes)
                {
                foreach (byte result in GetSelectedNodes(child))
                    {
                    yield return result;
                    }
                }
            }

        private void WorldMap_SelectionChanged(object sender, EventArgs e)
            {
            UpdateInfoOnCurrentCell();
            Parallel.ForEach(this._squareProperties.Cast<SquareProperties>(), item =>
                {
                if (item.NextAnimationFrame.HasValue)
                    {
                    this.WorldMap.InvalidateCell(item.WorldSquare.X, item.WorldSquare.Y);
                    }
                });

            }

        private void UpdateInfoOnCurrentCell()
            {
            var squareValue = this._squareProperties[this.WorldMap.CurrentCell.ColumnIndex, this.WorldMap.CurrentCell.RowIndex];

            // ReSharper disable once LocalizableElement
            this.CoordinatesTextBox.Text = $"X:{squareValue.WorldSquare.X:X2} Y:{squareValue.WorldSquare.Y:X2}";

            SetMappedOrGeneratedInfo(squareValue.GeneratedBackground);

            SetListOverrideInfo(squareValue.BackgroundOverride);

            SetPaletteInfo(squareValue.GetPaletteResult);

            SetFinalState(squareValue);

            SetSpriteInfo(squareValue);

            SetBackgroundObjectInfo(squareValue);

            if (this.ShowVisualOverlaysCheckBox.Checked && GetSwitchEffectsForSelectedSquare().Any())
                AnimationTimer.Enabled = true;
            }

        private void SetFinalState(SquareProperties squareValue)
            {
            string finalState = 
                $"Background: {squareValue.FinalBackground:x2}" +
                $"\r\nOrientation: {squareValue.FinalOrientation:x2}";

            this.FinalStateTextBox.Text = finalState;
            SetTextBoxHeight(this.FinalStateTextBox);
            }

        private void SetBackgroundObjectInfo(SquareProperties squareValue)
            {
            string backgroundObjectInfo;
            byte background = squareValue.FinalBackground;
            if (background >= 0x10)
                {
                backgroundObjectInfo = "Not a background event";
                }
            else if (background >= 0xd)
                {
                backgroundObjectInfo = $"Background event: {squareValue.BackgroundHandlerType}";
                }
            else
                {
                var backgroundEvent = squareValue.BackgroundOverride as BackgroundEvent;
                if (backgroundEvent == null)
                    {
                    backgroundObjectInfo = $"Background event used as scenery: {squareValue.BackgroundHandlerType}";

                    if (squareValue.BackgroundDescription != null)
                        {
                        backgroundObjectInfo += "\r\n" + squareValue.BackgroundDescription;
                        }
                    }
                else
                    {
                    backgroundObjectInfo = $"Background object id: {backgroundEvent.DataIndex:X2}";
                    backgroundObjectInfo += $"\r\nBackground handler: {squareValue.BackgroundHandlerType}";
                    backgroundObjectInfo += $"\r\nState Data: {backgroundEvent.ObjectData:X2}";
                    if (backgroundEvent.ObjectType.HasValue)
                        {
                        backgroundObjectInfo += $"\r\nObject type: {backgroundEvent.ObjectType.Value:X2}";
                        }
                    backgroundObjectInfo += $"\r\nDescription: {squareValue.BackgroundDescription}";
                    }
                }

            this.BackgroundObjectInfoTextBox.Text = backgroundObjectInfo;
            SetTextBoxHeight(this.BackgroundObjectInfoTextBox);
            }

        private void SetSpriteInfo(SquareProperties squareValue)
            {
            byte sprite = (byte)(SpriteBuilder.BackgroundSpriteLookup[squareValue.FinalBackground] & 0x7f);
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
            byte offsetAlongY = (byte)(SpriteBuilder.BackgroundYOffsetLookup[squareValue.FinalBackground] & 0xf0);
            spriteInfo += "\r\n" +
                          $"Y offset: {offsetAlongY:X2}\r\n";

            bool rightAlign = (squareValue.FinalOrientation & 0x80) != 0;
            bool bottomAlign = (squareValue.FinalOrientation & 0x40) != 0;
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

            this.SpriteInfoTextBox.Text = spriteInfo;
            SetTextBoxHeight(this.SpriteInfoTextBox);
            }

        private void SetPaletteInfo(GetPaletteResult getPaletteResult)
            {
            string paletteInfo;
            if (getPaletteResult.PaletteData.BackgroundPalette > 6)
                {
                Debug.Assert(getPaletteResult.PaletteData.BackgroundPalette == getPaletteResult.PaletteData.Palette.PaletteByte);
                paletteInfo =
                    $"Invariable palette for background: {getPaletteResult.PaletteData.Palette}";
                }
            else
                {
                paletteInfo =
                    $"Palette algorithm for background: {getPaletteResult.PaletteData.BackgroundPalette:X2} \r\n" +
                    $"Derived palette: {getPaletteResult.PaletteData.Palette}";
                }

            if (getPaletteResult.Background.HasValue)
                {
                var background = getPaletteResult.Background.Value;
                paletteInfo +=
                    $"\r\nRevised background: {background:x2}";
                }

            if (getPaletteResult.Orientation.HasValue)
                {
                var orientation = getPaletteResult.Orientation.Value;
                paletteInfo +=
                    $"\r\nRevised orientation: {orientation:x2}";
                }

            this.PaletteInfoTextBox.Text = paletteInfo;
            SetTextBoxHeight(this.PaletteInfoTextBox);
            }

        private void SetListOverrideInfo(BackgroundOverride? backgroundOverride)
            {
            if (backgroundOverride == null)
                {
                // ReSharper disable once LocalizableElement
                this.ListOverrideInfoTextBox.Text = "No override";
                }
            else
                {
                var listOverride = $"Override list: {backgroundOverride.ListId}\r\n";
                if (backgroundOverride.IsHashDefault)
                    {
                    listOverride +=
                        $"Default scenery for list: {backgroundOverride.OverrideValue:X2}\r\n";
                    }
                else
                    {
                    var backgroundObject = backgroundOverride as BackgroundObject;
                    Debug.Assert(backgroundObject != null);
                    listOverride +=
                        $"Background object number: {backgroundObject.Index:X2}\r\n" +
                        $"Value from list: {backgroundObject.OverrideValue:x2}\r\n";
                    }

                byte contents = backgroundOverride.Background;
                byte orientation = backgroundOverride.Orientation;
                listOverride +=
                    $"{(contents <= 0xf ? "Handler" : "Appearance")}: {contents:X2} \r\n" +
                    $"Orientation: {orientation:X2} ({DescribeOrientation(orientation)})";
                this.ListOverrideInfoTextBox.Text = listOverride;
                }

            SetTextBoxHeight(this.ListOverrideInfoTextBox);
            }

        private void SetMappedOrGeneratedInfo(GeneratedBackground generatedBackground)
            {
            var mappedGeneratedInfo =
                generatedBackground.IsMappedData
                    ? $"Mapped location index: {generatedBackground.PositionInMappedData}\r\n"
                    : string.Empty;
            byte calculatedBackground = generatedBackground.Result;
            mappedGeneratedInfo +=
                $"{(generatedBackground.IsMappedData ? "Explicit" : "Generated")} background {calculatedBackground:X2}\r\n";
            if ((calculatedBackground & 0x3f) >= 0x9)
                {
                mappedGeneratedInfo +=
                    $"Appearance: {calculatedBackground & 0x3f:X2}, " +
                    $"Orientation: {calculatedBackground & 0xc0:X2} " +
                    $"({DescribeOrientation(calculatedBackground)})";
                }

            this.MappedOrGeneratedInfoTextBox.Text = mappedGeneratedInfo;
            SetTextBoxHeight(this.MappedOrGeneratedInfoTextBox);
            }

        private static string DescribeOrientation(byte background)
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

        private static string[] BuildObjectTypeList()
            {
            var result = new[]
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

        private static List<WorldSquare> BuildTeleportDestinations()
            {
            var x = new byte[] { 0x62, 0xad, 0x2a, 0x0b, 0x9d, 0xaf, 0x9e, 0x45, 0x89, 0x9d, 0xb5, 0xa2, 0x72, 0xa7, 0x9f, 0xb0 };
            var y = new byte[] { 0xc7, 0x62, 0xcd, 0x0b, 0x58, 0x62, 0x69, 0x57, 0x71, 0x3c, 0x66, 0x63, 0x54, 0x80, 0x49, 0x80 };
            Debug.Assert(x.Length == y.Length);
            var c = x.Length;
            var result = new List<WorldSquare>(c);
            for (int i = 0; i < c; i++)
                {
                var newPoint = new WorldSquare(x[i], y[i]);
                result.Add(newPoint);
                }

            return result;
            }

        private void AnimationTimer_Tick(object sender, EventArgs e)
            {
            var timeElapsed = this._stopwatch.Elapsed;

            bool isAnimating = false;
            Parallel.ForEach(this._squareProperties.Cast<SquareProperties>(), item =>
                {
                bool redrawCell = false;
                if (item.NextAnimationFrame.HasValue)
                    {
                    isAnimating = true;
                    redrawCell = timeElapsed >= item.NextAnimationFrame;
                    }
                else if (ShouldSquareBeAnimated(item))
                    {
                    isAnimating = true;
                    redrawCell = true;
                    }

                if (redrawCell)
                    {
                    this.WorldMap.InvalidateCell(item.WorldSquare.X, item.WorldSquare.Y);
                    }
                });

            if (!isAnimating)
                {
                this.AnimationTimer.Enabled = false;
                }
            }

        private void ZoomLevelDropDown_SelectedIndexChanged(object sender, EventArgs e)
            {
            this._zoom = (decimal)this.ZoomLevelDropDown.SelectedValue;
            if (this.WorldMap.FirstDisplayedCell == null)
                return;
            var row = this.WorldMap.FirstDisplayedCell.RowIndex;
            var column = this.WorldMap.FirstDisplayedCell.ColumnIndex;
            this.ResetGrid();
            this.WorldMap.FirstDisplayedCell = this.WorldMap.Rows[row].Cells[column];
            }

        private void HighlightMappedDataCheckBox_CheckedChanged(object sender, EventArgs e)
            {
            if (this.HighlightMappedDataCheckBox.Checked)
                this.AnimationTimer.Enabled = true;
            }

        private void HighlightBackgroundOverrides_CheckedChanged(object sender, EventArgs e)
            {
            if (this.HighlightBackgroundOverridesCheckBox.Checked)
                this.AnimationTimer.Enabled = true;
            }

        private void HighlightBackgroundEventsCheckBox_CheckedChanged(object sender, EventArgs e)
            {
            if (this.HighlightBackgroundEventsCheckBox.Checked)
                this.AnimationTimer.Enabled = true;
            }

        private void HighlightBackgroundSceneryCheckBox_CheckedChanged(object sender, EventArgs e)
            {
            if (this.HighlightBackgroundSceneryCheckBox.Checked)
                this.AnimationTimer.Enabled = true;
            }

        private void BackgroundObjectTree_AfterCheck(object sender, TreeViewEventArgs e)
            {
            if (e.Action == TreeViewAction.Unknown)
                return;
            var node = e.Node!;
            CheckOrUncheckChildren(node, node.Checked);
            UpdateParentNodeState(node);
            this._selectedBackgroundObjectTypes = BuildSelectedBackgroundTypes();
            if (!this.HighlightBackgroundEventsCheckBox.Checked)
                {
                this.HighlightBackgroundEventsCheckBox.Checked = true;
                }
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
                mask = 0x18, // TVIF_HANDLE | TVIF_STATE
                hItem = node.Handle,
                stateMask = 0xf000, // TVIS_STATEIMAGEMASK
                // ReSharper restore CommentTypo
                state = 0x3000
                };
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once IdentifierTypo
            const int TVM_SETITEM = 0x110d;
            SendMessage(new HandleRef(this.BackgroundObjectTree, this.BackgroundObjectTree.Handle), TVM_SETITEM, 0, ref lParam);
            }

        private static void CheckOrUncheckChildren(TreeNode node, bool checkNode)
            {
            foreach (TreeNode child in node.Nodes)
                {
                child.Checked = checkNode;
                CheckOrUncheckChildren(child, checkNode);
                }
            }

        private static bool? GetSiblingNodeState(TreeNodeCollection nodes)
            {
            var enumerator = nodes.GetEnumerator();
            using var _ = enumerator as IDisposable;
            enumerator.MoveNext();
            bool result = ((TreeNode)enumerator.Current!).Checked;
            while (enumerator.MoveNext())
                {
                bool isChecked = ((TreeNode)enumerator.Current).Checked;
                if (result != isChecked)
                    return null;
                }

            return result;
            }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
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

        private static ImageList BuildStateImageList()
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

        private void BackgroundSceneryDropDown_SelectedIndexChanged(object sender, EventArgs e)
            {
            this._backgroundElementToHighlight = (byte)this.BackgroundSceneryDropDown.SelectedValue;
            if (!this.HighlightBackgroundSceneryCheckBox.Checked)
                {
                this.HighlightBackgroundSceneryCheckBox.Checked = true;
                }
            }

        private void WorldMap_MouseDown(object sender, MouseEventArgs e)
            {
            if (e.Button == MouseButtons.Left)
                {
                this._draggingStartPoint = e.Location;
                this._draggingTopLeftCell = new Point(this.WorldMap.FirstDisplayedCell.ColumnIndex, this.WorldMap.FirstDisplayedCell.RowIndex);
                }
            }

        private void WorldMap_MouseUp(object sender, MouseEventArgs e)
            {
            this._draggingInGrid = false;
            this.WorldMap.Cursor = Cursors.Arrow;
            }

        private void WorldMap_MouseMove(object sender, MouseEventArgs e)
            {
            if (e.Button != MouseButtons.Left)
                {
                if (this._draggingInGrid)
                    {
                    this._draggingInGrid = false;
                    this.WorldMap.Cursor = Cursors.Arrow;
                    }

                return;
                }

            var diffX = e.X - this._draggingStartPoint.X;
            var diffY = e.Y - this._draggingStartPoint.Y;
            if (!this._draggingInGrid && (Math.Abs(diffX) > SystemInformation.DragSize.Width || Math.Abs(diffY) > SystemInformation.DragSize.Height))
                {
                this._draggingInGrid = true;
                this.WorldMap.Cursor = Cursors.SizeAll;
                }

            if (this._draggingInGrid)
                {
                var columnWidth = this.WorldMap.Columns[0].Width;
                var rowHeight = this.WorldMap.Rows[0].Height;

                var columnsDiff = diffX / columnWidth;
                var rowsDiff = diffY / rowHeight;

                int row = this._draggingTopLeftCell.Y - rowsDiff;
                if (row < 0)
                    {
                    row = 0;
                    }

                if (row >= this.WorldMap.RowCount)
                    {
                    row = this.WorldMap.RowCount - 1;
                    }

                int column = this._draggingTopLeftCell.X - columnsDiff;
                if (column < 0)
                    {
                    column = 0;
                    }

                if (column >= this.WorldMap.ColumnCount)
                    {
                    column = this.WorldMap.ColumnCount - 1;
                    }

                this.WorldMap.FirstDisplayedCell = this.WorldMap.Rows[row].Cells[column];
                }
            }

        private static string[] BuildSuckerTriggers()
            {
            return new[]
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

        private enum SuckerAction
            {
            Blow = 0,
            Suck = 1
            }

        private static SuckerAction[] BuildSuckerActions()
            {
            var palettesAndActions = SpriteBuilder.BuildSuckerPalettesAndAction();
            return palettesAndActions.Select(p => (p & 1) == 1 ? SuckerAction.Suck : SuckerAction.Blow).ToArray();
            }

        private static byte[] BuildGargoyleBulletList()
            {
            return new byte[] { 0x32, 0x19, 0x19, 0x19, 0x32 };
            }

        private void BackgroundSceneryDropDown_DrawItem(object sender, DrawItemEventArgs e)
            {
            e.DrawBackground();

            var item = (KeyValuePair<byte, string>)this.BackgroundSceneryDropDown.Items[e.Index];
            byte background = item.Key;
            e.Graphics.DrawString($"{item.Value}", e.Font!, new SolidBrush(e.ForeColor), e.Bounds.Left, e.Bounds.Top);

            var spriteBuilder = SpriteBuilder.Start(WaterLevelType.AboveWater);
            var palette = CalculatePalette.GetPalette(background, 0, 0xa0, 0x50);
            spriteBuilder = spriteBuilder.AddBackgroundSprite(item.Key, 0, palette.PaletteData.Palette);

            Rectangle destinationRectangle = new Rectangle(e.Bounds.Left + 40, e.Bounds.Top, 32, 32);
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            e.Graphics.DrawImage(spriteBuilder.Result, destinationRectangle);

            e.DrawFocusRectangle();
            }

        private void ShowVisualOverlaysCheckBox_CheckedChanged(object sender, EventArgs e)
            {
            if (this.ShowVisualOverlaysCheckBox.Checked)
                {
                this.AnimationTimer.Enabled = true;
                }

            // this may turn off the animation timer
            this.WorldMap.Invalidate();
            }

        private class GameObject
            {
            public byte ObjectType;
            public byte X;
            public byte Y;
            public byte LowX;
            public byte LowY;
            }

        private static List<GameObject> BuildGameObjectList()
            {
            var objects = new List<GameObject>(new[]
                {
                new GameObject { ObjectType = 0x00, X = 0x9b, Y = 0x3b, LowX = 0xc0, LowY = 0x20 }, // player
                new GameObject { ObjectType = 0x26, X = 0x99, Y = 0x3b, LowX = 0x64, LowY = 0x20 }, // triax

                new GameObject { ObjectType = 0x43, X = 0xa3, Y = 0x5d, LowX = 0x00, LowY = 0xc0 }, // chest
                new GameObject { ObjectType = 0x50, X = 0x98, Y = 0x4d, LowX = 0x00, LowY = 0x40 }, // grenade
                new GameObject { ObjectType = 0x50, X = 0x98, Y = 0x4d, LowX = 0x80, LowY = 0x40 }, // grenade
                new GameObject { ObjectType = 0x59, X = 0xa4, Y = 0x67, LowX = 0x80, LowY = 0xc0 }, // jetpack booster
                new GameObject { ObjectType = 0x50, X = 0x9f, Y = 0x49, LowX = 0x00, LowY = 0x80 }, // grenade
                new GameObject { ObjectType = 0x46, X = 0xa0, Y = 0x49, LowX = 0x40, LowY = 0x40 }, // cannon
                new GameObject { ObjectType = 0x50, X = 0xc0, Y = 0x4e, LowX = 0x40, LowY = 0xc0 }, // grenade
                new GameObject { ObjectType = 0x50, X = 0x48, Y = 0x56, LowX = 0x00, LowY = 0xc0 }, // grenade
                new GameObject { ObjectType = 0x4e, X = 0x83, Y = 0x78, LowX = 0x00, LowY = 0x80 }, // remote control device
                new GameObject { ObjectType = 0x45, X = 0xc5, Y = 0x60, LowX = 0x00, LowY = 0xc0 }, // rock
                new GameObject { ObjectType = 0x53, X = 0x87, Y = 0x59, LowX = 0x00, LowY = 0x40 }, // green/yellow/red key
                new GameObject { ObjectType = 0x1d, X = 0x97, Y = 0x5e, LowX = 0x00, LowY = 0x00 }, // red robot
                new GameObject { ObjectType = 0x03, X = 0xe1, Y = 0x61, LowX = 0x80, LowY = 0xc0 }, // fluffy
                new GameObject { ObjectType = 0x50, X = 0x84, Y = 0x5b, LowX = 0x00, LowY = 0x80 }, // grenade
                new GameObject { ObjectType = 0x50, X = 0x98, Y = 0x80, LowX = 0x00, LowY = 0x00 }, // grenade
                new GameObject { ObjectType = 0x4a, X = 0x99, Y = 0x3c, LowX = 0x80, LowY = 0xc0 }, // destinator
                new GameObject { ObjectType = 0x3a, X = 0xe7, Y = 0x80, LowX = 0x00, LowY = 0x00 }, // giant wall
                new GameObject { ObjectType = 0x4c, X = 0x7c, Y = 0x77, LowX = 0x00, LowY = 0x40 } // empty flask
                });
            return objects;
            }

        private static List<byte[]> BuildSwitchEffectsList()
            {
            var result = new List<byte[]>
                {
                new byte[] { 0xb0, 0xbb, 0x84 },
                new byte[] { 0x0f, 0x29 },
                new byte[] { 0xc5 },
                new byte[] { 0xe7, 0x8f },
                new byte[] { 0x8a },
                new byte[] { 0x13 },
                new byte[] { 0x8e, 0x32 },
                new byte[] { 0xc2 },
                new byte[] { 0x11, 0xaa, 0xbd },
                new byte[] { 0x58, 0xcc, 0x55, 0xbc },
                new byte[] { 0x55 },
                new byte[] { 0x46, 0xa9 },
                new byte[] { 0x6a, 0x8b },
                new byte[] { 0xe6, 0x85, 0xd8 },
                new byte[] { 0xc7, 0x88 },
                new byte[] { 0x68 },
                new byte[] { 0x14 },
                new byte[] { 0x28, 0x4c },
                new byte[] { 0x65 },
                new byte[] { 0x89 },
                new byte[] { 0x8d },
                new byte[] { 0x64, 0x2a },
                new byte[] { 0x6b },
                new byte[] { 0xa7, 0xb9, 0x10 }
                };
            return result;
            }

        private static string GetEnumDescription(Enum value)
            {
            FieldInfo fi = value.GetType().GetField(value.ToString())!;

            DescriptionAttribute[] attributes = (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[])!;

            return attributes.Any() ? attributes.First().Description : value.ToString();
            }
        }
    }
