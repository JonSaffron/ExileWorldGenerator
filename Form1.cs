using System;
using System.Collections.Concurrent;
using System.Data;
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

        public Form1()
            {
            InitializeComponent();

            this.spriteSheet = Image.FromFile("ExileSpriteKey.png");
            }

        private void Form1_Shown(object sender, EventArgs e)
            {
            this.dataGridView1.SuspendLayout();

            this.dataGridView1.ColumnCount = 256;
            this.dataGridView1.RowCount = 256;

            for (int i = 0; i < 256; i++)
                {
                var value = i.ToString("X2");
                this.dataGridView1.Rows[i].HeaderCell.Value = value;
                this.dataGridView1.Columns[i].HeaderCell.Value = value;
                this.dataGridView1.Columns[i].DataPropertyName = value;
                }

            var hashOfPositions = new ConcurrentBag<int>();

            Parallel.For(0, 0x10000, i => { NewMethod(i, hashOfPositions); });

            this.dataGridView1.ResumeLayout();

            if (hashOfPositions.Count != 1024)
                throw new InvalidOperationException("1024 positions are expected to be explictly mapped, rather than " + hashOfPositions.Count);
            if (hashOfPositions.Distinct().Count() != 1024)
                throw new InvalidOperationException("All 1024 explicit map positions should be unique.");
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

            Rectangle destinationRectangle = new Rectangle(e.CellBounds.Left, e.CellBounds.Top - 1, e.CellBounds.Width, e.CellBounds.Height);
            Rectangle sourceRectangle = new Rectangle(156, 80, 16, 16);
            e.Graphics.DrawImage(this.spriteSheet, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);

            using (Brush brush = new SolidBrush(Color.Pink))
                {
                using (Pen pen = new Pen(brush))
                    {
                    e.Graphics.DrawLine(pen, e.CellBounds.Left,
                        e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                        e.CellBounds.Bottom - 1);
                    e.Graphics.DrawLine(pen, e.CellBounds.Right - 1,
                        e.CellBounds.Top, e.CellBounds.Right - 1,
                        e.CellBounds.Bottom);
                    }

                //e.Graphics.DrawString(text, e.CellStyle.Font, Brushes.Blue, e.CellBounds.Left, e.CellBounds.Top, StringFormat.GenericDefault);
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

            var squareValue = this.squareProperties[e.ColumnIndex, e.RowIndex].Background;
            var text = $"{squareValue & 0x3f:x2}{((squareValue & 0xC0) == 0xC0 ? "+" : (squareValue & 0x40) != 0 ? "|" : (squareValue & 0x80) != 0 ? "-" : string.Empty)}";
            e.ToolTipText = text;
            }
        }
    }
