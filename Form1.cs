using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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
            this.dataGridView1.ColumnCount = 256;
            this.dataGridView1.RowCount = 256;

            for (int i = 0; i < 256; i++)
                {
                this.dataGridView1.Rows[i].HeaderCell.Value = i.ToString("X");
                this.dataGridView1.Columns[i].HeaderCell.Value = i.ToString("X");
                }

            var hashOfPositions = new HashSet<int>();
            int c = 0;
            byte y = 0;
            var tasks = new ConcurrentBag<Task>();
            TaskScheduler uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            do
                {
                System.Diagnostics.Trace.WriteLine(y.ToString());

                byte x = 0;
                do
                    {
                    var x1 = x;
                    var y1 = y;
                    var task = Task.Factory.StartNew(() =>
                        {
                        string log = string.Empty;
                        Color? backgroundColour = null;

                        var mapResult = mapper.GetBackground(x1, y1);
                        if (mapResult.IsMappedData)
                            {
                            c++;
                            hashOfPositions.Add(mapResult.PositionInMappedData);
                            log = $"Position: {mapResult.PositionInMappedData}";
                            backgroundColour = Color.Cornsilk;
                            }

                        var squareValue = mapper.GetMapData(mapResult.Result, x1, ref log);
                        });
                    var continuedTask = task.ContinueWith(() =>
                        {
                        AddLocation(x1, y1, squareValue, backgroundColour, log);
                        }, uiContext);

                    tasks.Add(continuedTask);

                    x += 1;
                    if (x == 0)
                        break;
                    } while (true);
                
                y += 1;
                if (y == 0)
                    break;
                } while (true);

            MessageBox.Show("total of mapped data: " + c + "\r\nTotal unique positions:" + hashOfPositions.Count);
            }



        private void AddLocation(byte x, byte y, byte squareValue, Color? backgroundColour, string log)
            {
            if (this.dataGridView1.InvokeRequired)
                {
                this.dataGridView1.Invoke(AddLocation, new[] {x, y, squareValue, backgroundColour, log});
                return;
                }

            var cell = this.dataGridView1.Rows[y].Cells[x];

            this.dataGridView1.in
            cell.Value = string.Format("{0:x2}{1}", 
                                        squareValue & 0x3f, 
                                        (squareValue & 0xC0) == 0xC0 ? "+" :
                                            (squareValue & 0x40) != 0 ? "|" : 
                                                (squareValue & 0x80) != 0 ? "-" : 
                                                    string.Empty);
            if (backgroundColour.HasValue)
                cell.Style.BackColor = backgroundColour.Value;
            if (squareValue < 0x10)
                cell.Style.ForeColor = Color.Red;

            var coords = $"({x:X2}, {y:X2})\r\n{log}";
            cell.ToolTipText = coords;
            }
        }
    }
