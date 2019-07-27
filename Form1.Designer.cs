namespace ExileMappedBackground
    {
    partial class Form1
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.map = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtCoordinates = new System.Windows.Forms.TextBox();
            this.txtMappedOrGeneratedInfo = new System.Windows.Forms.TextBox();
            this.txtListOverrideInfo = new System.Windows.Forms.TextBox();
            this.txtPaletteInfo = new System.Windows.Forms.TextBox();
            this.txtSpriteInfo = new System.Windows.Forms.TextBox();
            this.txtBackgroundObjectInfo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.map)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.map);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1067, 550);
            this.splitContainer1.SplitterDistance = 733;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // map
            // 
            this.map.AllowUserToAddRows = false;
            this.map.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.map.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.map.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.map.DefaultCellStyle = dataGridViewCellStyle2;
            this.map.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.map.Location = new System.Drawing.Point(0, 0);
            this.map.Margin = new System.Windows.Forms.Padding(4);
            this.map.Name = "map";
            this.map.ReadOnly = true;
            this.map.RowHeadersWidth = 50;
            this.map.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.map.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.map.ShowEditingIcon = false;
            this.map.Size = new System.Drawing.Size(733, 550);
            this.map.TabIndex = 1;
            this.map.VirtualMode = true;
            this.map.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.map.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dataGridView1_CellToolTipTextNeeded);
            this.map.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txtCoordinates, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtMappedOrGeneratedInfo, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtListOverrideInfo, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtPaletteInfo, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtSpriteInfo, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtBackgroundObjectInfo, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(329, 550);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtCoordinates
            // 
            this.txtCoordinates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCoordinates.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCoordinates.Location = new System.Drawing.Point(3, 177);
            this.txtCoordinates.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtCoordinates.Name = "txtCoordinates";
            this.txtCoordinates.ReadOnly = true;
            this.txtCoordinates.Size = new System.Drawing.Size(100, 20);
            this.txtCoordinates.TabIndex = 0;
            this.txtCoordinates.Text = "X:xx Y:yy";
            // 
            // txtMappedOrGeneratedInfo
            // 
            this.txtMappedOrGeneratedInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMappedOrGeneratedInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMappedOrGeneratedInfo.Location = new System.Drawing.Point(3, 210);
            this.txtMappedOrGeneratedInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtMappedOrGeneratedInfo.Multiline = true;
            this.txtMappedOrGeneratedInfo.Name = "txtMappedOrGeneratedInfo";
            this.txtMappedOrGeneratedInfo.ReadOnly = true;
            this.txtMappedOrGeneratedInfo.Size = new System.Drawing.Size(323, 22);
            this.txtMappedOrGeneratedInfo.TabIndex = 1;
            this.txtMappedOrGeneratedInfo.Text = "mapped/generated background info";
            // 
            // txtListOverrideInfo
            // 
            this.txtListOverrideInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtListOverrideInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtListOverrideInfo.Location = new System.Drawing.Point(3, 245);
            this.txtListOverrideInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtListOverrideInfo.Multiline = true;
            this.txtListOverrideInfo.Name = "txtListOverrideInfo";
            this.txtListOverrideInfo.ReadOnly = true;
            this.txtListOverrideInfo.Size = new System.Drawing.Size(323, 22);
            this.txtListOverrideInfo.TabIndex = 2;
            this.txtListOverrideInfo.Text = "list overrides";
            // 
            // txtPaletteInfo
            // 
            this.txtPaletteInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPaletteInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPaletteInfo.Location = new System.Drawing.Point(3, 280);
            this.txtPaletteInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtPaletteInfo.Multiline = true;
            this.txtPaletteInfo.Name = "txtPaletteInfo";
            this.txtPaletteInfo.ReadOnly = true;
            this.txtPaletteInfo.Size = new System.Drawing.Size(323, 22);
            this.txtPaletteInfo.TabIndex = 3;
            this.txtPaletteInfo.Text = "palette info";
            // 
            // txtSpriteInfo
            // 
            this.txtSpriteInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSpriteInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSpriteInfo.Location = new System.Drawing.Point(3, 315);
            this.txtSpriteInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtSpriteInfo.Multiline = true;
            this.txtSpriteInfo.Name = "txtSpriteInfo";
            this.txtSpriteInfo.ReadOnly = true;
            this.txtSpriteInfo.Size = new System.Drawing.Size(323, 22);
            this.txtSpriteInfo.TabIndex = 4;
            this.txtSpriteInfo.Text = "sprite info";
            // 
            // txtBackgroundObjectInfo
            // 
            this.txtBackgroundObjectInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBackgroundObjectInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBackgroundObjectInfo.Location = new System.Drawing.Point(3, 350);
            this.txtBackgroundObjectInfo.Multiline = true;
            this.txtBackgroundObjectInfo.Name = "txtBackgroundObjectInfo";
            this.txtBackgroundObjectInfo.ReadOnly = true;
            this.txtBackgroundObjectInfo.Size = new System.Drawing.Size(323, 22);
            this.txtBackgroundObjectInfo.TabIndex = 5;
            this.txtBackgroundObjectInfo.Text = "background object info";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 550);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Exile";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.map)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

            }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView map;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtCoordinates;
        private System.Windows.Forms.TextBox txtMappedOrGeneratedInfo;
        private System.Windows.Forms.TextBox txtListOverrideInfo;
        private System.Windows.Forms.TextBox txtPaletteInfo;
        private System.Windows.Forms.TextBox txtSpriteInfo;
        private System.Windows.Forms.TextBox txtBackgroundObjectInfo;
    }
    }

