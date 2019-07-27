using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExileMappedBackground
    {
    public partial class Form1 : Form
        {
        private readonly CalculateBackground _mapper = new CalculateBackground();
        private readonly SpriteBuilder _spriteBuilder = new SpriteBuilder();
        private readonly SquareProperties[,] _squareProperties = new SquareProperties[256, 256];
        private readonly byte[] _backgroundSpriteLookup = BuildBackgroundSpriteLookup();
        private readonly byte[] _backgroundYOffsetLookup = BuildBackgroundYOffsetLookup();
        private int zoom = 1;
        private byte lookFor = 0xff;

        public Form1()
            {
            InitializeComponent();

            this.map.ColumnCount = 256;
            this.map.RowCount = 256;
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
            this.map.SuspendLayout();

            for (int i = 0; i < 256; i++)
                {
                var value = i.ToString("X2");
                this.map.Rows[i].HeaderCell.Value = value;
                this.map.Columns[i].HeaderCell.Value = value;
                this.map.Rows[i].Height = 32 * zoom;
                this.map.Columns[i].Width = 32 * zoom;
                }
            
            this.map.ResumeLayout();

            for (int i = 0; i <= 0x3f; i++)
                {
                System.Diagnostics.Trace.WriteLine($"{i:x2}\t{_backgroundSpriteLookup[i]:x2}\t{_backgroundYOffsetLookup[i]:x2}\t{CalculateBackground.BackgroundPaletteLookup[i]:x2}");
                }
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
            data.BackgroundEventTypeName = CalculateBackground.GetBackgroundEventTypeName(backgroundProperties.background);

            byte background = (byte) (data.BackgroundAfterHashing & 0x3f);
            byte orientation = (byte) (data.BackgroundAfterHashing & 0xc0);
            (data.BackgroundPalette, data.DisplayedPalette) = _mapper.GetPalette(ref background, ref orientation, x, y);
            data.BackgroundAfterPalette = (byte) (background ^ orientation);

            this._squareProperties[x, y] = data;
            }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                return;
                }

            e.PaintBackground(e.CellBounds, true);

            var squareProperties = this._squareProperties[e.ColumnIndex, e.RowIndex];

            byte background = (byte) (squareProperties.BackgroundAfterPalette & 0x3f);
            byte sprite = (byte) (this._backgroundSpriteLookup[background] & 0x7f);

            bool rightAlign = (squareProperties.BackgroundAfterPalette & 0x80) != 0;

            bool bottomAlign = (squareProperties.BackgroundAfterPalette & 0x40) != 0;

            byte offsetAlongY = (byte) (this._backgroundYOffsetLookup[background] & 0xf0);
                
            var squarePalette = SquarePalette.FromByte(squareProperties.DisplayedPalette);
            var image = this._spriteBuilder.BuildSprite(sprite, squarePalette, rightAlign, bottomAlign, offsetAlongY);

            Rectangle destinationRectangle = new Rectangle(e.CellBounds.Left, e.CellBounds.Top - 1, 32 * zoom, 32 * zoom);
            e.Graphics.DrawImage(image, destinationRectangle);

            if (squareProperties.MappedDataPosition.HasValue)
                {
                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Orange)))
                    {
                    using (Pen pen = new Pen(brush, 2.0f))
                        {
                        Rectangle r = e.CellBounds;
                        r.Inflate(-1, -1);
                        e.Graphics.DrawRectangle(pen, r);
                        }
                    }
                }

            if ((squareProperties.BackgroundAfterPalette & 0x3f) == lookFor)
                {
                using (Brush brush = new SolidBrush(Color.FromArgb(0x80, Color.Orange)))
                    {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                    }
                }

            e.Handled = true;
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
            public string BackgroundEventTypeName;
            public byte BackgroundPalette;
            public byte DisplayedPalette;
            }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                return;
                }

            var squareValue = this._squareProperties[e.ColumnIndex, e.RowIndex];
            var text = $"({squareValue.X:X2},{squareValue.Y:X2}) ";
            if (squareValue.MappedDataPosition.HasValue)
                text += "Explicitly mapped using position " + squareValue.MappedDataPosition.Value;
            var calculatedBackground = squareValue.CalculatedBackground;
            var backgroundAfterHashing = squareValue.BackgroundAfterHashing;
            if ((calculatedBackground & 0x3f) < 0x9)
                {
                text += $"\r\nPre-Hash Result: {calculatedBackground:x2} (range {calculatedBackground & 0x3f:x2}{((calculatedBackground & 0xC0) == 0xC0 ? " flipped both ways" : (calculatedBackground & 0x40) != 0 ? " flipped vertically" : (calculatedBackground & 0x80) != 0 ? " flipped horizontally" : string.Empty)})";
                text += $"\r\nPost-Hash Result: {backgroundAfterHashing:x2} ({backgroundAfterHashing & 0x3f:x2}{((backgroundAfterHashing & 0xC0) == 0xC0 ? " flipped both ways" : (backgroundAfterHashing & 0x40) != 0 ? " flipped vertically" : (backgroundAfterHashing & 0x80) != 0 ? " flipped horizontally" : string.Empty)})";
                if (squareValue.IsHashDefault)
                    {
                    Debug.Assert(!squareValue.BackgroundObjectId.HasValue);
                    text += "\r\nUsing hash default value";
                    }
                else
                    {
                    Debug.Assert(squareValue.BackgroundObjectId.HasValue);
                    text += $"\r\nBackground object id = {squareValue.BackgroundObjectId.Value:X2}";
                    }
                }
            else
                {
                Debug.Assert(backgroundAfterHashing == calculatedBackground);
                Debug.Assert(!squareValue.IsHashDefault);
                Debug.Assert(!squareValue.BackgroundObjectId.HasValue);
                text += $"\r\nResult: {calculatedBackground:x2} ({calculatedBackground & 0x3f:x2}{((calculatedBackground & 0xC0) == 0xC0 ? " flipped both ways" : (calculatedBackground & 0x40) != 0 ? " flipped vertically" : (calculatedBackground & 0x80) != 0 ? " flipped horizontally" : string.Empty)})";
                }

            if (squareValue.BackgroundEventTypeName != null)
                text += "\r\nBackground event: " + squareValue.BackgroundEventTypeName;

            text += $"\r\nPalette for background: {squareValue.BackgroundPalette:x2}";
            var colours = SquarePalette.FromByte(squareValue.DisplayedPalette);
            text += $"\r\nDisplayed Palette: {squareValue.DisplayedPalette:x2} {colours.Colour1}, {colours.Colour2}, {colours.Colour3}";

            if (squareValue.BackgroundAfterHashing != squareValue.BackgroundAfterPalette)
                {
                text += "\r\nPalette changed background and/or orientation";
                text += $"\r\nFinal background: {squareValue.BackgroundAfterPalette:x2} ({squareValue.BackgroundAfterPalette & 0x3f:x2}{((squareValue.BackgroundAfterPalette & 0xC0) == 0xC0 ? " flipped both ways" : (squareValue.BackgroundAfterPalette & 0x40) != 0 ? " flipped vertically" : (squareValue.BackgroundAfterPalette & 0x80) != 0 ? " flipped horizontally" : string.Empty)})";
                }

            byte sprite = (byte) (_backgroundSpriteLookup[squareValue.BackgroundAfterPalette & 0x3f] & 0x7f);
            text += $"\r\nSprite: {sprite:x2}";
            text += $"\r\nFlip sprite horizontally: {this._spriteBuilder.FlipSpriteHorizontally[sprite]}";
            text += $"\r\nFlip sprite vertically: {this._spriteBuilder.FlipSpriteVertically[sprite]}";

            e.ToolTipText = text;
            }

        private static byte[] BuildBackgroundSpriteLookup()
            {
            var map = new byte[]
                {
                /* & &7f = sprite
                   & &80 = not relevant
                */
                0xC6,0xCE,0xC6,0xC6,0xC6,0xBB,0xC6,0x18,0x2D,0x70,0x6A,0xC6,0x23,0x39,0xC6,0x62,
                0xC0,0x8E,0x39,0x44,0x47,0x26,0x48,0x49,0xDF,0xC6,0x99,0x9A,0x25,0x2B,0x39,0x3B,
                0x3C,0x55,0x8E,0x43,0x34,0x35,0x27,0x28,0x29,0x2A,0x42,0xBF,0x40,0x3D,0x38,0x36,
                0x37,0x3E,0x33,0x31,0x2F,0x30,0x2C,0x24,0x32,0x41,0x45,0x3A,0x6A,0x23,0x60,0xCC
                };
            var result = map.Select(item => (byte) (item & 0x7f)).ToArray();
            return result;
            }

        private static byte[] BuildBackgroundYOffsetLookup()
            {
            var result = new byte[]
                {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xD0,0xC5,0xB0,0xC7,0x00,0x06,0x00,0x00,0xC0, // 00
                0xB0,0xA0,0x07,0x08,0x00,0x04,0x80,0xC0,0x70,0x00,0xB0,0x80,0x99,0x08,0x00,0x80, // 10
                0xC0,0x00,0xA0,0x03,0x02,0x82,0x01,0x41,0x81,0xC1,0x04,0xF0,0xB0,0x00,0x03,0x02, // 20
                0x82,0x70,0x06,0xC0,0xC5,0x04,0x80,0x06,0x80,0x04,0x99,0x30,0xC7,0x06,0xA9,0x00  // 30
                };
            return result;
            }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
            {
            var squareValue = this._squareProperties[this.map.CurrentCell.ColumnIndex, this.map.CurrentCell.RowIndex];
            
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
            if ((squareValue.BackgroundAfterPalette & 0x3f)  > 0xf)
                backgroundObjectInfo = "No background object";
            else if (!squareValue.BackgroundObjectId.HasValue)
                backgroundObjectInfo = "Background used as scenery";
            else
                {
                backgroundObjectInfo =
                    $"Background event: {squareValue.BackgroundEventTypeName}\r\n" +
                    $"Object type: tbd\r\n" +
                    $"Data: tbd";
                }

            this.txtBackgroundObjectInfo.Text = backgroundObjectInfo;
            SetTextBoxHeight(txtBackgroundObjectInfo);
            }

        private void SetSpriteInfo(SquareProperties squareValue)
            {
            byte sprite = (byte) (_backgroundSpriteLookup[squareValue.BackgroundAfterPalette & 0x3f] & 0x7f);
            string spriteInfo =
                $"Sprite number: {sprite:X2}\r\n" +
                "Orientation on sprite sheet: ";
            var isSpriteFlippedHorizontally = this._spriteBuilder.FlipSpriteHorizontally[sprite];
            var isSpriteFlippedVertically = this._spriteBuilder.FlipSpriteVertically[sprite];
            if (isSpriteFlippedHorizontally && isSpriteFlippedVertically)
                spriteInfo += "flipped both ways";
            else if (isSpriteFlippedHorizontally)
                spriteInfo += "flipped horizontally";
            else if (isSpriteFlippedVertically)
                spriteInfo += "flipped vertically";
            else
                spriteInfo += "not flipped";
            byte offsetAlongY = (byte) (this._backgroundYOffsetLookup[squareValue.BackgroundAfterPalette & 0x3f] & 0xf0);
            spriteInfo += "\r\n" +
                          $"Y offset: {offsetAlongY:X2}\r\n";

            bool rightAlign = (squareValue.BackgroundAfterPalette & 0x80) != 0;
            bool bottomAlign = (squareValue.BackgroundAfterPalette & 0x40) != 0;
            var isFlippedHorizontally = isSpriteFlippedHorizontally ^ rightAlign;
            var isFlippedVertically = isSpriteFlippedVertically ^ bottomAlign;
            spriteInfo +=
                $"Final orientation: ";
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
                paletteInfo += $" {colours.Colour1}, {colours.Colour2}, {colours.Colour3}";
                }
            else
                {
                paletteInfo += "\r\n" +
                               $"Derived palette: {squareValue.DisplayedPalette:X2}";
                var colours = SquarePalette.FromByte(squareValue.DisplayedPalette);
                paletteInfo += $" {colours.Colour1}, {colours.Colour2}, {colours.Colour3}";
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
                SetTextBoxHeight(this.txtListOverrideInfo);
                }
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
        }
    }
