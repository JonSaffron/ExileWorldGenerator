using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExileMappedBackground
    {
    public partial class Form1 : Form
        {
        private CalculateBackground mapper = new CalculateBackground();
        private readonly Image spriteSheet;
        private SquareProperties[,] squareProperties = new SquareProperties[256, 256];
        private readonly byte[] BackgroundSpriteLookup = BuildBackgroundSpriteLookup();
        private readonly Dictionary<int, Rectangle> sourceRectangles = BuildSourceRectangleKey();

        public Form1()
            {
            InitializeComponent();

            this.spriteSheet = Image.FromFile("ExileSpriteKey.png");
            this.dataGridView1.ColumnCount = 256;
            this.dataGridView1.RowCount = 256;
            var hashOfPositions = new ConcurrentBag<int>();
            Parallel.For(0, 0x10000, i => { NewMethod(i, hashOfPositions); });
            if (hashOfPositions.Count != 1024)
                throw new InvalidOperationException("1024 positions are expected to be explictly mapped, rather than " + hashOfPositions.Count);
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
                }



            this.dataGridView1.ResumeLayout();

            }

        private void NewMethod(int i, ConcurrentBag<int> hashOfPositions)
            {
            byte y = (byte) (i >> 8);
            byte x = (byte) (i & 0xFF);

            if (x == 0)
                System.Diagnostics.Trace.WriteLine(y.ToString());

            var mapResult = mapper.GetBackground(x, y);
            if (mapResult.IsMappedData)
                {
                hashOfPositions.Add(mapResult.PositionInMappedData);
                }

            var background = mapper.GetMapData(mapResult.Result, x);
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

            this.squareProperties[x, y] = data;
            }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                return;
                }

            var squareProperties = this.squareProperties[e.ColumnIndex, e.RowIndex];

            byte background = (byte) (squareProperties.Background & 0x3f);
            byte sprite = (byte) (this.BackgroundSpriteLookup[background] & 0x7f);
            if (!this.sourceRectangles.TryGetValue(sprite, out var sourceRectangle))
                throw new InvalidOperationException($"Required sprite number {sprite:x2}");

            Rectangle destinationRectangle = new Rectangle(e.CellBounds.Left, e.CellBounds.Top - 1, e.CellBounds.Width, e.CellBounds.Height);
            e.Graphics.DrawImage(this.spriteSheet, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);

            if (squareProperties.MappedDataPosition.HasValue)
                {
                using (Brush brush = new SolidBrush(Color.White))
                    {
                    using (Pen pen = new Pen(brush, 3.0f))
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

            var squareValue = this.squareProperties[e.ColumnIndex, e.RowIndex];
            var text = $"({squareValue.X:X2},{squareValue.Y:X2}) ";
            if (squareValue.MappedDataPosition.HasValue)
                text += "Explicitly mapped using position " + squareValue.MappedDataPosition.Value;
            var calculatedBackground = squareValue.CalculatedBackground;
            var background = squareValue.Background;
            if ((calculatedBackground & 0x3f) < 0x9)
                {
                text += $"\r\nPre-Hash Result: = {calculatedBackground & 0x3f:x2}{((calculatedBackground & 0xC0) == 0xC0 ? "+" : (calculatedBackground & 0x40) != 0 ? "|" : (calculatedBackground & 0x80) != 0 ? "-" : string.Empty)}";
                text += $"\r\nPost-Hash Result: = {background & 0x3f:x2}{((background & 0xC0) == 0xC0 ? "+" : (background & 0x40) != 0 ? "|" : (background & 0x80) != 0 ? "-" : string.Empty)}";
                if (squareValue.IsHashDefault)
                    text += "\r\nUsing hash default value";
                if (squareValue.BackgroundObjectId.HasValue)
                    text += $"\r\nBackground object id = {squareValue.BackgroundObjectId.Value:X2}";
                }
            else
                {
                Debug.Assert(background == calculatedBackground);
                Debug.Assert(!squareValue.IsHashDefault);
                Debug.Assert(!squareValue.BackgroundObjectId.HasValue);
                text += $"\r\nResult: = {calculatedBackground & 0x3f:x2}{((calculatedBackground & 0xC0) == 0xC0 ? "+" : (calculatedBackground & 0x40) != 0 ? "|" : (calculatedBackground & 0x80) != 0 ? "-" : string.Empty)}";
                }
            if (squareValue.BackgroundEventTypeName != null)
                text += "\r\nBackground event: " + squareValue.BackgroundEventTypeName;
            e.ToolTipText = text;
            }

        private static byte[] BuildBackgroundSpriteLookup()
            {
            return new byte[]
                {
                0xC6,0xCE,0xC6,0xC6,0xC6,0xBB,0xC6,0x18,0x2D,0x70,0x6A,0xC6,0x23,0x39,0xC6,0x62,
                0xC0,0x8E,0x39,0x44,0x47,0x26,0x48,0x49,0xDF,0xC6,0x99,0x9A,0x25,0x2B,0x39,0x3B,
                0x3C,0x55,0x8E,0x43,0x34,0x35,0x27,0x28,0x29,0x2A,0x42,0xBF,0x40,0x3D,0x38,0x36,
                0x37,0x3E,0x33,0x31,0x2F,0x30,0x2C,0x24,0x32,0x41,0x45,0x3A,0x6A,0x23,0x60,0xCC
                };
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
