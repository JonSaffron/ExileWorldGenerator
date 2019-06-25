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
            this.dataGridView1.ColumnCount = 256;
            this.dataGridView1.RowCount = 256;

            var hashOfPositions = new HashSet<int>();
            int c = 0;
            byte y = 0;
            do
                {
                byte x = 0;
                do
                    {
                    string log = string.Empty;
                    Color? backgroundColour = null;
                    
                    var mapResult = mapper.GetBackground(x, y);
                    if (mapResult.IsMappedData)
                        {
                        c++;
                        hashOfPositions.Add(mapResult.PositionInMappedData.Value);
                        log = $"Position: {mapResult.PositionInMappedData.Value}";
                        backgroundColour = Color.Cornsilk;
                        }
                    var squareValue = mapper.GetMapData(mapResult.Result, x, ref log);
                    AddLocation(x, y, squareValue, backgroundColour, log);

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
            var cell = this.dataGridView1.Rows[y].Cells[x];
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
