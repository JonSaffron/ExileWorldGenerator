using System;
using System.Collections.Generic;
using System.Drawing;
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
            this.tableLayoutPanel1.SuspendLayout();

            this.tableLayoutPanel1.ColumnCount = 256;
            this.tableLayoutPanel1.RowCount = 256;

            for (int i = 0; i < 256; i++)
                {
                if (this.tableLayoutPanel1.RowStyles.Count < i)
                    {
                    this.tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                    this.tableLayoutPanel1.RowStyles[i].Height = 15f;
                    }
                else 
                    {
                    this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 15f));
                    }

                if (this.tableLayoutPanel1.ColumnStyles.Count < i)
                    {
                    this.tableLayoutPanel1.ColumnStyles[i].SizeType = SizeType.Absolute;
                    this.tableLayoutPanel1.ColumnStyles[i].Width = 30f;
                    }
                else
                    {
                    this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30f));
                    }
                }

            var hashOfPositions = new HashSet<int>();
            int c = 0;
            byte y = 0;
            do
                {
                byte x = 0;
                do
                    {
                    var position = mapper.GetBackground(x, y);
                    if (position.HasValue)
                        {
                        c++;
                        hashOfPositions.Add(position.Value);
                        var squareValue = mapper.GetMapData(position.Value, x, out string log);
                        AddLocation(x, y, squareValue, log);
                        }

                    x += 1;
                    if (x == 0)
                        break;
                    } while (true);
                
                y += 1;
                if (y == 0)
                    break;
                } while (true);

            this.tableLayoutPanel1.ResumeLayout();

            MessageBox.Show("total of mapped data: " + c + "\r\nTotal unique positions:" + hashOfPositions.Count);
            }

        private void AddLocation(byte x, byte y, byte squareValue, string log)
            {
            var label = new Label();
            this.tableLayoutPanel1.Controls.Add(label, x, y);
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Text = string.Format("{0:x2}{1}", 
                                        squareValue & 0x3f, 
                                        (squareValue & 0xC0) == 0xC0 ? "+" :
                                            (squareValue & 0x40) != 0 ? "|" : 
                                                (squareValue & 0x80) != 0 ? "-" : 
                                                    string.Empty);
            label.Font = new Font("Arial Narrow", 7f);

            var tooltip = new ToolTip();
            var coords = $"({x:X2}, {y:X2}\r\n{log}";
            tooltip.SetToolTip(label, coords);
            tooltip.ShowAlways = true;
            }
        }
    }
