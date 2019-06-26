using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExileMappedBackground
    {
    public partial class Form1 : Form
        {
        const float size = 1.0f / 256.0f;
        private CalculateBackground mapper = new CalculateBackground();

        public Form1()
            {
            InitializeComponent();
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

            var dt = BuildDataTable();

            Parallel.For(0, 0x10000, i => { NewMethod(i, hashOfPositions, dt); });

            this.dataGridView1.DataSource = dt;
            this.dataGridView1.ResumeLayout();
            MessageBox.Show("total of mapped data: " + hashOfPositions.Count + "\r\nTotal unique positions:" + hashOfPositions.Distinct().Count());
            }

        private void NewMethod(int i, ConcurrentBag<int> hashOfPositions, DataTable dt)
            {
            byte y = (byte) (i >> 8);
            byte x = (byte) (i & 0xFF);

            if (x == 0)
                System.Diagnostics.Trace.WriteLine(y.ToString());

            string log = string.Empty;
            Color? backgroundColour = null;

            var mapResult = mapper.GetBackground(x, y);
            if (mapResult.IsMappedData)
                {
                hashOfPositions.Add(mapResult.PositionInMappedData);
                log = $"Position: {mapResult.PositionInMappedData}";
                backgroundColour = Color.Cornsilk;
                }

            var squareValue = mapper.GetMapData(mapResult.Result, x, ref log);

            if (dt.Rows.Count <= y)
                System.Diagnostics.Debugger.Break();
            if (dt.Columns.Count <= x)
                System.Diagnostics.Debugger.Break();

            var dataRow = dt.Rows[y];
            var column = x.ToString("X2");
            var text = $"{squareValue & 0x3f:x2}{((squareValue & 0xC0) == 0xC0 ? "+" : (squareValue & 0x40) != 0 ? "|" : (squareValue & 0x80) != 0 ? "-" : string.Empty)}";
            lock (dt.Rows.SyncRoot)
                {
                dataRow[column] = text;
                }
            AddLocation(x, y, squareValue, backgroundColour, log);
            }

        private DataTable BuildDataTable()
            {
            var result = new DataTable();
            for (int i = 0; i < 0x100; i++)
                {
                result.Columns.Add(i.ToString("X2"), typeof(string));
                }
            for (int i = 0; i < 0x100; i++)
                {
                var dr = result.NewRow();
                result.Rows.Add(dr);
                }
            return result;
            }

        delegate void AddLocationDelegate(byte x, byte y, byte squareValue, Color? backgroundColour, string log);
        private void AddLocation(byte x, byte y, byte squareValue, Color? backgroundColour, string log)
            {
            //if (this.dataGridView1.InvokeRequired)
            //    {
            //    AddLocationDelegate thisMethodDelegated = AddLocation;
            //    object[] args = {x, y, squareValue, backgroundColour, log};
            //    this.dataGridView1.Invoke(thisMethodDelegated, args);
            //    return;
            //    }

            var cell = this.dataGridView1.Rows[y].Cells[x];

            //cell.Value = string.Format("{0:x2}{1}", 
            //                            squareValue & 0x3f, 
            //                            (squareValue & 0xC0) == 0xC0 ? "+" :
            //                                (squareValue & 0x40) != 0 ? "|" : 
            //                                    (squareValue & 0x80) != 0 ? "-" : 
            //                                        string.Empty);
            if (backgroundColour.HasValue)
                cell.Style.BackColor = backgroundColour.Value;
            if ((squareValue & 0x3f) < 0x10)
                cell.Style.ForeColor = Color.Red;

            var coords = $"({x:X2}, {y:X2})\r\n{log}";
            cell.ToolTipText = coords;
            }
        }
    }
