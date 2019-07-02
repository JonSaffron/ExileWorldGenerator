using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExileMappedBackground
    {
    public partial class Form1 : Form
        {
        private readonly CalculateBackground _mapper = new CalculateBackground();
        private readonly Image _spriteSheet;
        private readonly SquareProperties[,] _squareProperties = new SquareProperties[256, 256];
        private readonly byte[] _backgroundSpriteLookup = BuildBackgroundSpriteLookup();
        private readonly Dictionary<int, Rectangle> _sourceRectangles = BuildSourceRectangleKey();
        private readonly bool[] _flipBackgroundSpriteVertically = BuildFlipBackgroundSpriteVertically();
        private readonly bool[] _flipSpriteHorizontally = BuildFlipSpriteHorizontally();
        private readonly bool[] _flipSpriteVertically = BuildFlipSpriteVertically();
        private readonly byte[] _backgroundYOffsetLookup = BuildBackgroundYOffsetLookup();
        private int zoom = 2;

        public Form1()
            {
            InitializeComponent();

            this._spriteSheet = Image.FromFile("ExileSpriteKey.png");
            this.dataGridView1.ColumnCount = 256;
            this.dataGridView1.RowCount = 256;
            var hashOfPositions = new ConcurrentBag<int>();
            Parallel.For(0, 0x10000, i => { NewMethod(i, hashOfPositions); });
            if (hashOfPositions.Count != 1024)
                throw new InvalidOperationException("1024 positions are expected to be explicitly mapped, rather than " + hashOfPositions.Count);
            if (hashOfPositions.Distinct().Count() != 1024)
                throw new InvalidOperationException("All 1024 explicit map positions should be unique.");
            }

        private void Form1_Shown(object sender, EventArgs e)
            {
            this.dataGridView1.SuspendLayout();

            for (int i = 0; i < 256; i++)
                {
                var value = i.ToString("X2");
                this.dataGridView1.Rows[i].HeaderCell.Value = value;
                this.dataGridView1.Columns[i].HeaderCell.Value = value;
                this.dataGridView1.Rows[i].Height = 32 * zoom;
                this.dataGridView1.Columns[i].Width = 16 * zoom;
                }



            this.dataGridView1.ResumeLayout();

            }

        private void NewMethod(int i, ConcurrentBag<int> hashOfPositions)
            {
            byte y = (byte) (i >> 8);
            byte x = (byte) (i & 0xFF);

            if (x == 0)
                System.Diagnostics.Trace.WriteLine(y.ToString());

            var mapResult = _mapper.GetBackground(x, y);
            if (mapResult.IsMappedData)
                {
                hashOfPositions.Add(mapResult.PositionInMappedData);
                }

            var background = _mapper.GetMapData(mapResult.Result, x);
            var data = new SquareProperties();
            data.X = x;
            data.Y = y;
            data.CalculatedBackground = mapResult.Result;
            if (mapResult.IsMappedData)
                data.MappedDataPosition = mapResult.PositionInMappedData;
            data.Background = background.background;
            data.BackgroundObjectId = background.backgroundObjectId;
            data.IsHashDefault = background.isHashDefault;
            data.BackgroundEventTypeName = CalculateBackground.GetBackgroundEventTypeName(background.background);

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

            byte background = (byte) (squareProperties.Background & 0x3f);
            byte sprite = (byte) (this._backgroundSpriteLookup[background] & 0x7f);
            if (!this._sourceRectangles.TryGetValue(sprite, out var sourceRectangle))
                throw new InvalidOperationException($"Required sprite number {sprite:x2}");

            bool rightAlign = (squareProperties.Background & 0x80) != 0;
            bool flipHorizontally = rightAlign ^ _flipSpriteHorizontally[sprite];

            bool bottomAlign = (squareProperties.Background & 0x40) != 0;
            bool flipVertically = bottomAlign;
            flipVertically ^= _flipBackgroundSpriteVertically[background];
            flipVertically ^= _flipSpriteVertically[sprite];

            byte offsetAlongY;
            if (!bottomAlign)
                {
                offsetAlongY = (byte) (this._backgroundYOffsetLookup[background] & 0xf0);
                offsetAlongY >>= 3;
                }
            else
                {
                offsetAlongY = this._backgroundYOffsetLookup[background];
                
                }
                
            var image = FlipImage(this._spriteSheet, sourceRectangle, flipHorizontally, flipVertically, rightAlign, bottomAlign);

            Rectangle destinationRectangle = new Rectangle(e.CellBounds.Left, e.CellBounds.Top - 1, 16 * zoom, 32 * zoom);
            e.Graphics.DrawImage(image, destinationRectangle);

            if (squareProperties.MappedDataPosition.HasValue)
                {
                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.White)))
                    {
                    using (Pen pen = new Pen(brush, 1.0f))
                        {
                        var topLeft = new Point(e.CellBounds.Left + 1, e.CellBounds.Top + 1);
                        var topRight = new Point(e.CellBounds.Right - 1, e.CellBounds.Top + 1);
                        var bottomRight = new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                        var bottomLeft = new Point(e.CellBounds.Left + 1, e.CellBounds.Bottom - 1);

                        e.Graphics.DrawLine(pen, topLeft, topRight);
                        e.Graphics.DrawLine(pen, topRight, bottomRight);
                        e.Graphics.DrawLine(pen, bottomRight, bottomLeft);
                        e.Graphics.DrawLine(pen, bottomLeft, topLeft);
                        }
                    }
                }

            e.Handled = true;
            }

        private Image FlipImage(Image image, Rectangle sourceRectangle, bool flipHorizontally, bool flipVertically, bool rightAlign, bool bottomAlign)
            {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (sourceRectangle.Height > 32 || sourceRectangle.Width > 16 || sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
                    throw new ArgumentException("Invalid source rectangle size");

            Bitmap flippedImage = new Bitmap(16 , 32 );
            using (Graphics g = Graphics.FromImage(flippedImage))
                {
                using (Matrix m = new Matrix(
                    flipHorizontally ? -1 : 1, 0, 0,
                    flipVertically ? -1 : 1, 0, 0))
                    {
                    m.Translate(
                        flipHorizontally ? sourceRectangle.Width : 0,
                        flipVertically ? sourceRectangle.Height : 0,
                        MatrixOrder.Append);
                    m.Translate(
                        rightAlign ? 16 - sourceRectangle.Width : 0,
                        bottomAlign ? 32 - sourceRectangle.Height : 0,
                        MatrixOrder.Append);

                    g.Transform = m;
                    g.DrawImage(image, new Rectangle(0, 0, sourceRectangle.Width, sourceRectangle.Height), sourceRectangle, GraphicsUnit.Pixel);
                    }
                }

            Debug.Assert(flippedImage.Height == 32 && flippedImage.Width == 16);

            return flippedImage;
            }

        private class SquareProperties
            {
            public byte X;
            public byte Y;
            public byte CalculatedBackground;
            public byte Background;
            public int? MappedDataPosition;
            public bool IsHashDefault;
            public int? BackgroundObjectId;
            public string BackgroundEventTypeName;
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
            var background = squareValue.Background;
            if ((calculatedBackground & 0x3f) < 0x9)
                {
                text += $"\r\nPre-Hash Result: {calculatedBackground:x2} (range {calculatedBackground & 0x3f:x2}{((calculatedBackground & 0xC0) == 0xC0 ? " flipped both ways" : (calculatedBackground & 0x40) != 0 ? " flipped vertically" : (calculatedBackground & 0x80) != 0 ? " flipped horizontally" : string.Empty)})";
                text += $"\r\nPost-Hash Result: {background:x2} ({background & 0x3f:x2}{((background & 0xC0) == 0xC0 ? " flipped both ways" : (background & 0x40) != 0 ? " flipped vertically" : (background & 0x80) != 0 ? " flipped horizontally" : string.Empty)})";
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
                Debug.Assert(background == calculatedBackground);
                Debug.Assert(!squareValue.IsHashDefault);
                Debug.Assert(!squareValue.BackgroundObjectId.HasValue);
                text += $"\r\nResult: {calculatedBackground:x2} ({calculatedBackground & 0x3f:x2}{((calculatedBackground & 0xC0) == 0xC0 ? " flipped both ways" : (calculatedBackground & 0x40) != 0 ? " flipped vertically" : (calculatedBackground & 0x80) != 0 ? " flipped horizontally" : string.Empty)})";
                }
            if (squareValue.BackgroundEventTypeName != null)
                text += "\r\nBackground event: " + squareValue.BackgroundEventTypeName;

            byte sprite = (byte) (_backgroundSpriteLookup[background & 0x3f] & 0x7f);
            text += $"\r\nSprite: {sprite:x2}";
            text += $"\r\nFlip sprite horizontally: {_flipSpriteHorizontally[sprite]}";
            text += $"\r\nFlip vertically for background: {_flipBackgroundSpriteVertically[background & 0x3f]}";
            text += $"\r\nFlip sprite vertically: {_flipSpriteVertically[sprite]}";

            e.ToolTipText = text;
            }

        private static bool[] BuildFlipBackgroundSpriteVertically()
            {
            return BuildBackgroundSpriteLookup().Select(s => (s & 0x80) != 0).ToArray();
            }

        private static byte[] BuildBackgroundSpriteLookup()
            {
            return new byte[]
                {
                /* & &7f = sprite
                   & &80 = vertically flipped
                */
                0xC6,0xCE,0xC6,0xC6,0xC6,0xBB,0xC6,0x18,0x2D,0x70,0x6A,0xC6,0x23,0x39,0xC6,0x62,
                0xC0,0x8E,0x39,0x44,0x47,0x26,0x48,0x49,0xDF,0xC6,0x99,0x9A,0x25,0x2B,0x39,0x3B,
                0x3C,0x55,0x8E,0x43,0x34,0x35,0x27,0x28,0x29,0x2A,0x42,0xBF,0x40,0x3D,0x38,0x36,
                0x37,0x3E,0x33,0x31,0x2F,0x30,0x2C,0x24,0x32,0x41,0x45,0x3A,0x6A,0x23,0x60,0xCC
                };
            }

        private static bool[] BuildFlipSpriteHorizontally()
            {
            return BuildSpriteWidthLookup().Select(w => (w & 0x1) != 0).ToArray();
            }

        private static byte[] BuildSpriteWidthLookup()
            {
            var result = new byte[]
                {
                // Format: wwww000h
                // wwww + 1 is the width of the sprite in pixels
                // h indicates a horizontal flip is required
                // 0    1    2    3    4    5    6    7    8    9    a    b    c    d    e    f
                0xC0,0xA0,0x50,0x90,0x40,0x50,0x60,0x40,0x21,0x20,0x20,0x21,0x11,0x11,0x50,0x30, // 00
                0x60,0x51,0x50,0x50,0x80,0x50,0x50,0xB0,0xB0,0x80,0x80,0x50,0x90,0x70,0x50,0x30, // 10
                0x40,0x20,0x10,0xF1,0xF1,0x71,0x30,0xF0,0xF0,0xF0,0xF0,0xF1,0xF0,0x30,0x60,0x30, // 20
                0x30,0xF0,0xF0,0xF1,0xF0,0xF0,0xF0,0xF0,0xF1,0xF0,0xF0,0xF0,0xF0,0xF0,0xF0,0xF0, // 30
                0xF0,0x30,0x60,0xF0,0xF0,0x70,0x00,0xF0,0xF0,0xF0,0xF0,0x30,0x31,0x41,0xF0,0x20, // 40
                0x40,0x20,0x50,0x50,0x30,0x70,0xC0,0x40,0x40,0x20,0x60,0x60,0x40,0xF0,0x70,0x20, // 50
                0x70,0x90,0xC0,0x30,0x40,0x50,0x60,0x40,0x40,0x30,0xF0,0x20,0x30,0x31,0x70,0xB1, // 60
                0xF0,0x70,0x51,0x41,0x50,0x51,0x30,0x80,0x30,0x30,0x50,0x50,0x40                 // 70
                };
            return result;
            }

        private static bool[] BuildFlipSpriteVertically()
            {
            return BuildSpriteHeightLookup().Select(h => (h & 0x1) != 0).ToArray();
            }

        private static byte[] BuildSpriteHeightLookup()
            {
            var result = new byte[]
                {
                // hhhhh00v
                // hhhhh + 1 is the sprite height in pixels
                // if v=1, the vertical flip flag is inverted when plotting the sprite
                //        0   1   2   3   4   5   6   7   8   9   a   b   c   d   e   f
                0x40,0x80,0x98,0x91,0xA8,0xA0,0xA0,0xA0,0x09,0x08,0x10,0x19,0x19,0x18,0x58,0x18, // 00
                0x60,0x78,0x81,0x98,0x78,0x70,0x98,0x90,0x28,0x48,0x78,0x69,0x20,0x28,0x38,0x48, // 10
                0x38,0x20,0x08,0xF8,0xF8,0x68,0xF9,0xF9,0xB9,0x79,0x39,0xF8,0x78,0x38,0x38,0x38, // 20
                0xF8,0x38,0x78,0xF8,0xF8,0x78,0xF8,0x78,0xF8,0xF8,0xC9,0x78,0x38,0xF8,0x89,0x09, // 30
                0x49,0xF8,0xF8,0xF9,0xF9,0x68,0x00,0xF8,0x78,0x38,0x39,0xF9,0xF9,0x28,0x48,0x18, // 40
                0x11,0x19,0x20,0x29,0x41,0xF8,0x70,0x30,0x20,0x20,0x19,0x18,0x28,0x60,0x48,0x80, // 50
                0x58,0x58,0x39,0x19,0x68,0x68,0x68,0x58,0x68,0x58,0x38,0x38,0x28,0x48,0x48,0x48, // 60
                0x48,0x08,0x30,0x20,0x28,0x39,0x70,0x38,0x30,0x20,0x20,0x20,0x20                 // 70
                };
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

        private static Dictionary<int, Rectangle> BuildSourceRectangleKey()
            {
            var result = new Dictionary<int, Rectangle>
                {
                    {0x0e, new Rectangle(178, 6, 6, 12)},
                    {0x18, new Rectangle(304, 6, 11, 6)},
                    {0x19, new Rectangle(0, 38, 9, 10)},
                    {0x1a, new Rectangle(13, 38, 9, 16)},
                    {0x23, new Rectangle(124, 38, 16, 32)},
                    {0x24, new Rectangle(144, 38, 16, 32)},
                    {0x25, new Rectangle(164, 38, 8, 16)},
                    {0x26, new Rectangle(176, 38, 4, 32)},
                    {0x27, new Rectangle(188, 38, 16, 32)},
                    {0x28, new Rectangle(208, 38, 16, 24)},
                    {0x29, new Rectangle(228, 38, 16, 16)},
                    {0x2a, new Rectangle(248, 38, 16, 8)},
                    {0x2b, new Rectangle(268, 38, 16, 32)},
                    {0x2c, new Rectangle(288, 38, 16, 16)},
                    {0x2d, new Rectangle(308, 38, 4, 8)},
                    {0x2e, new Rectangle(0, 80, 8, 8)},
                    {0x2f, new Rectangle(12, 80, 4, 8)},
                    {0x30, new Rectangle(24, 80, 4, 32)},
                    {0x31, new Rectangle(36, 80, 16, 8)},
                    {0x32, new Rectangle(56, 80, 16, 16)},
                    {0x33, new Rectangle(76, 80, 16, 32)},
                    {0x34, new Rectangle(96, 80, 16, 32)},
                    {0x35, new Rectangle(116, 80, 16, 16)},
                    {0x36, new Rectangle(136, 80, 16, 32)},
                    {0x37, new Rectangle(156, 80, 16, 16)},
                    {0x38, new Rectangle(176, 80, 16, 32)},
                    {0x39, new Rectangle(196, 80, 16, 32)},
                    {0x3a, new Rectangle(216, 80, 16, 26)},
                    {0x3b, new Rectangle(236, 80, 16, 16)},
                    {0x3c, new Rectangle(256, 80, 16, 8)},
                    {0x3d, new Rectangle(276, 80, 16, 32)},
                    {0x3e, new Rectangle(296, 80, 16, 18)},
                    {0x3f, new Rectangle(0, 122, 16, 2)},
                    {0x40, new Rectangle(20, 122, 16, 10)},
                    {0x41, new Rectangle(40, 122, 4, 32)},
                    {0x42, new Rectangle(52, 122, 8, 32)},
                    {0x43, new Rectangle(64, 122, 16, 32)},
                    {0x44, new Rectangle(84, 122, 16, 32)},
                    {0x45, new Rectangle(104, 122, 8, 14)},
                    {0x46, new Rectangle(116, 122, 4, 4)},
                    {0x47, new Rectangle(128, 122, 16, 32)},
                    {0x48, new Rectangle(148, 122, 16, 16)},
                    {0x49, new Rectangle(168, 122, 16, 8)},
                    {0x4a, new Rectangle(188, 122, 16, 8)},
                    {0x4b, new Rectangle(208, 122, 4, 32)},
                    {0x4c, new Rectangle(220, 122, 4, 32)},
                    {0x4e, new Rectangle(244, 122, 16, 10)},
                    {0x55, new Rectangle(24, 164, 8, 32)},
                    {0x5f, new Rectangle(157, 164, 3, 17)},
                    {0x60, new Rectangle(169, 164, 8, 12)},
                    {0x62, new Rectangle(195, 164, 14, 8)},
                    {0x6a, new Rectangle(296, 164, 16, 8)},
                    {0x70, new Rectangle(64, 206, 16, 10)}
                };
            return result;
            }
        }
    }
