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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Invisible Switch");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Teleport Beam");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Object from Data");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Door");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Stone Door");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("with Half Wall");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("no accompaniment");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("with Foliage");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Object from Type", new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Switch");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("from Bush");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("from Pipe End");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Object Emerging", new System.Windows.Forms.TreeNode[] {
            treeNode11,
            treeNode12});
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Fixed Wind");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Engine Thruster");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Water");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Random Wind");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Mushrooms");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Background Objects", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode9,
            treeNode10,
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Splitter = new System.Windows.Forms.SplitContainer();
            this.map = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.panelProperties = new System.Windows.Forms.Panel();
            this.txtBackgroundObjectInfo = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.txtSpriteInfo = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtPaletteInfo = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtListOverrideInfo = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtMappedOrGeneratedInfo = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtCoordinates = new System.Windows.Forms.TextBox();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.cboHighlightBackground = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkHighlightDisplayElement = new System.Windows.Forms.CheckBox();
            this.chkHighlightBackgroundObjects = new System.Windows.Forms.CheckBox();
            this.BackgroundObjectTree = new System.Windows.Forms.TreeView();
            this.chkHighlightMappedData = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboZoomLevel = new System.Windows.Forms.ComboBox();
            this.tabSearch = new System.Windows.Forms.TabPage();
            this.AnimationTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.Splitter)).BeginInit();
            this.Splitter.Panel1.SuspendLayout();
            this.Splitter.Panel2.SuspendLayout();
            this.Splitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.map)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.panelProperties.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // Splitter
            // 
            this.Splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Splitter.Location = new System.Drawing.Point(0, 0);
            this.Splitter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Splitter.Name = "Splitter";
            // 
            // Splitter.Panel1
            // 
            this.Splitter.Panel1.Controls.Add(this.map);
            // 
            // Splitter.Panel2
            // 
            this.Splitter.Panel2.Controls.Add(this.tabControl1);
            this.Splitter.Size = new System.Drawing.Size(1200, 852);
            this.Splitter.SplitterDistance = 822;
            this.Splitter.SplitterWidth = 6;
            this.Splitter.TabIndex = 0;
            // 
            // map
            // 
            this.map.AllowUserToAddRows = false;
            this.map.AllowUserToDeleteRows = false;
            this.map.AllowUserToResizeColumns = false;
            this.map.AllowUserToResizeRows = false;
            this.map.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
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
            this.map.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.map.MultiSelect = false;
            this.map.Name = "map";
            this.map.ReadOnly = true;
            this.map.RowHeadersWidth = 50;
            this.map.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.map.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.map.ShowEditingIcon = false;
            this.map.ShowRowErrors = false;
            this.map.Size = new System.Drawing.Size(822, 852);
            this.map.TabIndex = 1;
            this.map.VirtualMode = true;
            this.map.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.map.SelectionChanged += new System.EventHandler(this.map_SelectionChanged);
            this.map.MouseDown += new System.Windows.Forms.MouseEventHandler(this.map_MouseDown);
            this.map.MouseMove += new System.Windows.Forms.MouseEventHandler(this.map_MouseMove);
            this.map.MouseUp += new System.Windows.Forms.MouseEventHandler(this.map_MouseUp);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabProperties);
            this.tabControl1.Controls.Add(this.tabOptions);
            this.tabControl1.Controls.Add(this.tabSearch);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(6, 30);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(372, 852);
            this.tabControl1.TabIndex = 7;
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.panelProperties);
            this.tabProperties.Location = new System.Drawing.Point(4, 88);
            this.tabProperties.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabProperties.Size = new System.Drawing.Size(364, 760);
            this.tabProperties.TabIndex = 1;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // panelProperties
            // 
            this.panelProperties.Controls.Add(this.txtBackgroundObjectInfo);
            this.panelProperties.Controls.Add(this.panel5);
            this.panelProperties.Controls.Add(this.txtSpriteInfo);
            this.panelProperties.Controls.Add(this.panel4);
            this.panelProperties.Controls.Add(this.txtPaletteInfo);
            this.panelProperties.Controls.Add(this.panel3);
            this.panelProperties.Controls.Add(this.txtListOverrideInfo);
            this.panelProperties.Controls.Add(this.panel2);
            this.panelProperties.Controls.Add(this.txtMappedOrGeneratedInfo);
            this.panelProperties.Controls.Add(this.panel1);
            this.panelProperties.Controls.Add(this.txtCoordinates);
            this.panelProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProperties.Location = new System.Drawing.Point(4, 5);
            this.panelProperties.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelProperties.Name = "panelProperties";
            this.panelProperties.Size = new System.Drawing.Size(356, 750);
            this.panelProperties.TabIndex = 2;
            // 
            // txtBackgroundObjectInfo
            // 
            this.txtBackgroundObjectInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBackgroundObjectInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtBackgroundObjectInfo.Location = new System.Drawing.Point(0, 251);
            this.txtBackgroundObjectInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBackgroundObjectInfo.Multiline = true;
            this.txtBackgroundObjectInfo.Name = "txtBackgroundObjectInfo";
            this.txtBackgroundObjectInfo.ReadOnly = true;
            this.txtBackgroundObjectInfo.Size = new System.Drawing.Size(356, 28);
            this.txtBackgroundObjectInfo.TabIndex = 9;
            this.txtBackgroundObjectInfo.Text = "background object info";
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 228);
            this.panel5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(356, 23);
            this.panel5.TabIndex = 14;
            // 
            // txtSpriteInfo
            // 
            this.txtSpriteInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSpriteInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSpriteInfo.Location = new System.Drawing.Point(0, 200);
            this.txtSpriteInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSpriteInfo.Multiline = true;
            this.txtSpriteInfo.Name = "txtSpriteInfo";
            this.txtSpriteInfo.ReadOnly = true;
            this.txtSpriteInfo.Size = new System.Drawing.Size(356, 28);
            this.txtSpriteInfo.TabIndex = 8;
            this.txtSpriteInfo.Text = "sprite info";
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 177);
            this.panel4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(356, 23);
            this.panel4.TabIndex = 13;
            // 
            // txtPaletteInfo
            // 
            this.txtPaletteInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPaletteInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtPaletteInfo.Location = new System.Drawing.Point(0, 149);
            this.txtPaletteInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPaletteInfo.Multiline = true;
            this.txtPaletteInfo.Name = "txtPaletteInfo";
            this.txtPaletteInfo.ReadOnly = true;
            this.txtPaletteInfo.Size = new System.Drawing.Size(356, 28);
            this.txtPaletteInfo.TabIndex = 7;
            this.txtPaletteInfo.Text = "palette info";
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 126);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(356, 23);
            this.panel3.TabIndex = 12;
            // 
            // txtListOverrideInfo
            // 
            this.txtListOverrideInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtListOverrideInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtListOverrideInfo.Location = new System.Drawing.Point(0, 98);
            this.txtListOverrideInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtListOverrideInfo.Multiline = true;
            this.txtListOverrideInfo.Name = "txtListOverrideInfo";
            this.txtListOverrideInfo.ReadOnly = true;
            this.txtListOverrideInfo.Size = new System.Drawing.Size(356, 28);
            this.txtListOverrideInfo.TabIndex = 6;
            this.txtListOverrideInfo.Text = "list overrides";
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 75);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(356, 23);
            this.panel2.TabIndex = 11;
            // 
            // txtMappedOrGeneratedInfo
            // 
            this.txtMappedOrGeneratedInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMappedOrGeneratedInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMappedOrGeneratedInfo.Location = new System.Drawing.Point(0, 47);
            this.txtMappedOrGeneratedInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMappedOrGeneratedInfo.Multiline = true;
            this.txtMappedOrGeneratedInfo.Name = "txtMappedOrGeneratedInfo";
            this.txtMappedOrGeneratedInfo.ReadOnly = true;
            this.txtMappedOrGeneratedInfo.Size = new System.Drawing.Size(356, 28);
            this.txtMappedOrGeneratedInfo.TabIndex = 5;
            this.txtMappedOrGeneratedInfo.Text = "mapped/generated background info";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(356, 23);
            this.panel1.TabIndex = 10;
            // 
            // txtCoordinates
            // 
            this.txtCoordinates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCoordinates.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCoordinates.Location = new System.Drawing.Point(0, 0);
            this.txtCoordinates.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCoordinates.Name = "txtCoordinates";
            this.txtCoordinates.ReadOnly = true;
            this.txtCoordinates.Size = new System.Drawing.Size(356, 24);
            this.txtCoordinates.TabIndex = 3;
            this.txtCoordinates.Text = "X:xx Y:yy";
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.cboHighlightBackground);
            this.tabOptions.Controls.Add(this.label2);
            this.tabOptions.Controls.Add(this.chkHighlightDisplayElement);
            this.tabOptions.Controls.Add(this.chkHighlightBackgroundObjects);
            this.tabOptions.Controls.Add(this.BackgroundObjectTree);
            this.tabOptions.Controls.Add(this.chkHighlightMappedData);
            this.tabOptions.Controls.Add(this.label1);
            this.tabOptions.Controls.Add(this.cboZoomLevel);
            this.tabOptions.Location = new System.Drawing.Point(4, 88);
            this.tabOptions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabOptions.Size = new System.Drawing.Size(364, 760);
            this.tabOptions.TabIndex = 0;
            this.tabOptions.Text = "Options";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // cboHighlightBackground
            // 
            this.cboHighlightBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHighlightBackground.FormattingEnabled = true;
            this.cboHighlightBackground.Location = new System.Drawing.Point(207, 658);
            this.cboHighlightBackground.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboHighlightBackground.Name = "cboHighlightBackground";
            this.cboHighlightBackground.Size = new System.Drawing.Size(133, 33);
            this.cboHighlightBackground.TabIndex = 7;
            this.cboHighlightBackground.SelectedIndexChanged += new System.EventHandler(this.cboHighlightBackground_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 663);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 25);
            this.label2.TabIndex = 6;
            this.label2.Text = "Background:";
            // 
            // chkHighlightDisplayElement
            // 
            this.chkHighlightDisplayElement.AutoSize = true;
            this.chkHighlightDisplayElement.Location = new System.Drawing.Point(15, 623);
            this.chkHighlightDisplayElement.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkHighlightDisplayElement.Name = "chkHighlightDisplayElement";
            this.chkHighlightDisplayElement.Size = new System.Drawing.Size(253, 29);
            this.chkHighlightDisplayElement.TabIndex = 5;
            this.chkHighlightDisplayElement.Text = "Highlight display element";
            this.chkHighlightDisplayElement.UseVisualStyleBackColor = true;
            this.chkHighlightDisplayElement.CheckedChanged += new System.EventHandler(this.chkHighlightDisplayElement_CheckedChanged);
            // 
            // chkHighlightBackgroundObjects
            // 
            this.chkHighlightBackgroundObjects.AutoSize = true;
            this.chkHighlightBackgroundObjects.Location = new System.Drawing.Point(15, 97);
            this.chkHighlightBackgroundObjects.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkHighlightBackgroundObjects.Name = "chkHighlightBackgroundObjects";
            this.chkHighlightBackgroundObjects.Size = new System.Drawing.Size(288, 29);
            this.chkHighlightBackgroundObjects.TabIndex = 3;
            this.chkHighlightBackgroundObjects.Text = "Highlight background objects";
            this.chkHighlightBackgroundObjects.UseVisualStyleBackColor = true;
            this.chkHighlightBackgroundObjects.CheckedChanged += new System.EventHandler(this.chkHighlightBackgroundObjects_CheckedChanged);
            // 
            // BackgroundObjectTree
            // 
            this.BackgroundObjectTree.CheckBoxes = true;
            this.BackgroundObjectTree.HideSelection = false;
            this.BackgroundObjectTree.Location = new System.Drawing.Point(34, 135);
            this.BackgroundObjectTree.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BackgroundObjectTree.Name = "BackgroundObjectTree";
            treeNode1.Name = "InvisibleSwitch";
            treeNode1.Tag = "0";
            treeNode1.Text = "Invisible Switch";
            treeNode2.Name = "TeleportBeam";
            treeNode2.Tag = "1";
            treeNode2.Text = "Teleport Beam";
            treeNode3.Name = "ObjectFromData";
            treeNode3.Tag = "2";
            treeNode3.Text = "Object from Data";
            treeNode4.Name = "Door";
            treeNode4.Tag = "3";
            treeNode4.Text = "Door";
            treeNode5.Name = "StoneDoor";
            treeNode5.Tag = "4";
            treeNode5.Text = "Stone Door";
            treeNode6.Name = "ObjectFromTypeWithHalfWall";
            treeNode6.Tag = "5";
            treeNode6.Text = "with Half Wall";
            treeNode7.Name = "ObjectFromTypeWithNoAccompaniment";
            treeNode7.Tag = "6";
            treeNode7.Text = "no accompaniment";
            treeNode8.Name = "ObjectFromTypeWithFoliage";
            treeNode8.Tag = "7";
            treeNode8.Text = "with Foliage";
            treeNode9.Name = "ObjectFromType";
            treeNode9.Text = "Object from Type";
            treeNode10.Name = "Switch";
            treeNode10.Tag = "8";
            treeNode10.Text = "Switch";
            treeNode11.Name = "ObjectEmergingFromBush";
            treeNode11.Tag = "9";
            treeNode11.Text = "from Bush";
            treeNode12.Name = "ObjectEmergingFromPipeEnd";
            treeNode12.Tag = "a";
            treeNode12.Text = "from Pipe End";
            treeNode13.Name = "ObjectEmerging";
            treeNode13.Text = "Object Emerging";
            treeNode14.Name = "FixedWind";
            treeNode14.Tag = "b";
            treeNode14.Text = "Fixed Wind";
            treeNode15.Name = "EngineThruster";
            treeNode15.Tag = "c";
            treeNode15.Text = "Engine Thruster";
            treeNode16.Name = "Water";
            treeNode16.Tag = "d";
            treeNode16.Text = "Water";
            treeNode17.Name = "RandomWind";
            treeNode17.Tag = "e";
            treeNode17.Text = "Random Wind";
            treeNode18.Name = "Mushrooms";
            treeNode18.Tag = "f";
            treeNode18.Text = "Mushrooms";
            treeNode19.Name = "AllBackgroundObjects";
            treeNode19.Text = "Background Objects";
            this.BackgroundObjectTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode19});
            this.BackgroundObjectTree.ShowRootLines = false;
            this.BackgroundObjectTree.Size = new System.Drawing.Size(304, 462);
            this.BackgroundObjectTree.TabIndex = 4;
            this.BackgroundObjectTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.BackgroundObjectTree_AfterCheck);
            // 
            // chkHighlightMappedData
            // 
            this.chkHighlightMappedData.AutoSize = true;
            this.chkHighlightMappedData.Location = new System.Drawing.Point(15, 57);
            this.chkHighlightMappedData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkHighlightMappedData.Name = "chkHighlightMappedData";
            this.chkHighlightMappedData.Size = new System.Drawing.Size(232, 29);
            this.chkHighlightMappedData.TabIndex = 2;
            this.chkHighlightMappedData.Text = "Highlight mapped data";
            this.chkHighlightMappedData.UseVisualStyleBackColor = true;
            this.chkHighlightMappedData.CheckedChanged += new System.EventHandler(this.chkHighlightMappedData_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zoom level:";
            // 
            // cboZoomLevel
            // 
            this.cboZoomLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZoomLevel.FormattingEnabled = true;
            this.cboZoomLevel.Location = new System.Drawing.Point(136, 11);
            this.cboZoomLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboZoomLevel.Name = "cboZoomLevel";
            this.cboZoomLevel.Size = new System.Drawing.Size(160, 33);
            this.cboZoomLevel.TabIndex = 1;
            this.cboZoomLevel.SelectedIndexChanged += new System.EventHandler(this.cboZoomLevel_SelectedIndexChanged);
            // 
            // tabSearch
            // 
            this.tabSearch.Location = new System.Drawing.Point(4, 88);
            this.tabSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabSearch.Name = "tabSearch";
            this.tabSearch.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabSearch.Size = new System.Drawing.Size(364, 760);
            this.tabSearch.TabIndex = 2;
            this.tabSearch.Text = "Search";
            this.tabSearch.UseVisualStyleBackColor = true;
            // 
            // AnimationTimer
            // 
            this.AnimationTimer.Enabled = true;
            this.AnimationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 852);
            this.Controls.Add(this.Splitter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "The Land of the Exile";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Splitter.Panel1.ResumeLayout(false);
            this.Splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Splitter)).EndInit();
            this.Splitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.map)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.panelProperties.ResumeLayout(false);
            this.panelProperties.PerformLayout();
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.ResumeLayout(false);

            }

        #endregion
        private System.Windows.Forms.SplitContainer Splitter;
        private System.Windows.Forms.DataGridView map;
        private System.Windows.Forms.Timer AnimationTimer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.Panel panelProperties;
        private System.Windows.Forms.TextBox txtBackgroundObjectInfo;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox txtSpriteInfo;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtPaletteInfo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtListOverrideInfo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtMappedOrGeneratedInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtCoordinates;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.ComboBox cboHighlightBackground;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkHighlightDisplayElement;
        private System.Windows.Forms.CheckBox chkHighlightBackgroundObjects;
        private System.Windows.Forms.TreeView BackgroundObjectTree;
        private System.Windows.Forms.CheckBox chkHighlightMappedData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboZoomLevel;
        private System.Windows.Forms.TabPage tabSearch;
        }
    }

