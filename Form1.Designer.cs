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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.map = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtCoordinates = new System.Windows.Forms.TextBox();
            this.txtMappedOrGeneratedInfo = new System.Windows.Forms.TextBox();
            this.txtListOverrideInfo = new System.Windows.Forms.TextBox();
            this.txtPaletteInfo = new System.Windows.Forms.TextBox();
            this.txtSpriteInfo = new System.Windows.Forms.TextBox();
            this.txtBackgroundObjectInfo = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.cboZoomLevel = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.map)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.map);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(800, 447);
            this.splitContainer1.SplitterDistance = 549;
            this.splitContainer1.TabIndex = 0;
            // 
            // map
            // 
            this.map.AllowUserToAddRows = false;
            this.map.AllowUserToDeleteRows = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.map.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.map.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.map.DefaultCellStyle = dataGridViewCellStyle8;
            this.map.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.map.Location = new System.Drawing.Point(0, 0);
            this.map.Name = "map";
            this.map.ReadOnly = true;
            this.map.RowHeadersWidth = 50;
            this.map.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.map.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.map.ShowEditingIcon = false;
            this.map.Size = new System.Drawing.Size(549, 447);
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
            this.tableLayoutPanel1.Controls.Add(this.txtCoordinates, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtMappedOrGeneratedInfo, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtListOverrideInfo, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtPaletteInfo, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtSpriteInfo, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.txtBackgroundObjectInfo, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(247, 447);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtCoordinates
            // 
            this.txtCoordinates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCoordinates.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCoordinates.Location = new System.Drawing.Point(2, 205);
            this.txtCoordinates.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.txtCoordinates.Name = "txtCoordinates";
            this.txtCoordinates.ReadOnly = true;
            this.txtCoordinates.Size = new System.Drawing.Size(75, 16);
            this.txtCoordinates.TabIndex = 0;
            this.txtCoordinates.Text = "X:xx Y:yy";
            // 
            // txtMappedOrGeneratedInfo
            // 
            this.txtMappedOrGeneratedInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMappedOrGeneratedInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMappedOrGeneratedInfo.Location = new System.Drawing.Point(2, 231);
            this.txtMappedOrGeneratedInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.txtMappedOrGeneratedInfo.Multiline = true;
            this.txtMappedOrGeneratedInfo.Name = "txtMappedOrGeneratedInfo";
            this.txtMappedOrGeneratedInfo.ReadOnly = true;
            this.txtMappedOrGeneratedInfo.Size = new System.Drawing.Size(243, 18);
            this.txtMappedOrGeneratedInfo.TabIndex = 1;
            this.txtMappedOrGeneratedInfo.Text = "mapped/generated background info";
            // 
            // txtListOverrideInfo
            // 
            this.txtListOverrideInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtListOverrideInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtListOverrideInfo.Location = new System.Drawing.Point(2, 259);
            this.txtListOverrideInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.txtListOverrideInfo.Multiline = true;
            this.txtListOverrideInfo.Name = "txtListOverrideInfo";
            this.txtListOverrideInfo.ReadOnly = true;
            this.txtListOverrideInfo.Size = new System.Drawing.Size(243, 18);
            this.txtListOverrideInfo.TabIndex = 2;
            this.txtListOverrideInfo.Text = "list overrides";
            // 
            // txtPaletteInfo
            // 
            this.txtPaletteInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPaletteInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPaletteInfo.Location = new System.Drawing.Point(2, 287);
            this.txtPaletteInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.txtPaletteInfo.Multiline = true;
            this.txtPaletteInfo.Name = "txtPaletteInfo";
            this.txtPaletteInfo.ReadOnly = true;
            this.txtPaletteInfo.Size = new System.Drawing.Size(243, 18);
            this.txtPaletteInfo.TabIndex = 3;
            this.txtPaletteInfo.Text = "palette info";
            // 
            // txtSpriteInfo
            // 
            this.txtSpriteInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSpriteInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSpriteInfo.Location = new System.Drawing.Point(2, 315);
            this.txtSpriteInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.txtSpriteInfo.Multiline = true;
            this.txtSpriteInfo.Name = "txtSpriteInfo";
            this.txtSpriteInfo.ReadOnly = true;
            this.txtSpriteInfo.Size = new System.Drawing.Size(243, 18);
            this.txtSpriteInfo.TabIndex = 4;
            this.txtSpriteInfo.Text = "sprite info";
            // 
            // txtBackgroundObjectInfo
            // 
            this.txtBackgroundObjectInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBackgroundObjectInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBackgroundObjectInfo.Location = new System.Drawing.Point(2, 343);
            this.txtBackgroundObjectInfo.Margin = new System.Windows.Forms.Padding(2);
            this.txtBackgroundObjectInfo.Multiline = true;
            this.txtBackgroundObjectInfo.Name = "txtBackgroundObjectInfo";
            this.txtBackgroundObjectInfo.ReadOnly = true;
            this.txtBackgroundObjectInfo.Size = new System.Drawing.Size(243, 18);
            this.txtBackgroundObjectInfo.TabIndex = 5;
            this.txtBackgroundObjectInfo.Text = "background object info";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 86);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(241, 114);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cboZoomLevel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(233, 88);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Zoom level:";
            // 
            // cboZoomLevel
            // 
            this.cboZoomLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZoomLevel.FormattingEnabled = true;
            this.cboZoomLevel.Location = new System.Drawing.Point(77, 7);
            this.cboZoomLevel.Name = "cboZoomLevel";
            this.cboZoomLevel.Size = new System.Drawing.Size(121, 21);
            this.cboZoomLevel.TabIndex = 0;
            this.cboZoomLevel.SelectedIndexChanged += new System.EventHandler(this.cboZoomLevel_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(233, 88);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // animationTimer
            // 
            this.animationTimer.Enabled = true;
            this.animationTimer.Tick += new System.EventHandler(this.animationTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 447);
            this.Controls.Add(this.splitContainer1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
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
        private System.Windows.Forms.Timer animationTimer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboZoomLevel;
        private System.Windows.Forms.TabPage tabPage2;
    }
    }

