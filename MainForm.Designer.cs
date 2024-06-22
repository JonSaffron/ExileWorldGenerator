namespace ExileWorldGenerator
    {
    partial class MainForm
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
        components = new System.ComponentModel.Container();
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
        System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Object from Type", new System.Windows.Forms.TreeNode[] { treeNode6, treeNode7, treeNode8 });
        System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Switch");
        System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("from Bush");
        System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("from Pipe End");
        System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Object Emerging", new System.Windows.Forms.TreeNode[] { treeNode11, treeNode12 });
        System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Fixed Wind");
        System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Engine Thruster");
        System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Water");
        System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Random Wind");
        System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Mushrooms");
        System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Background Objects", new System.Windows.Forms.TreeNode[] { treeNode1, treeNode2, treeNode3, treeNode4, treeNode5, treeNode9, treeNode10, treeNode13, treeNode14, treeNode15, treeNode16, treeNode17, treeNode18 });
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        Splitter = new System.Windows.Forms.SplitContainer();
        WorldMap = new System.Windows.Forms.DataGridView();
        TabControls = new System.Windows.Forms.TabControl();
        PropertiesTab = new System.Windows.Forms.TabPage();
        BackgroundObjectInfoPanel = new System.Windows.Forms.Panel();
        BackgroundObjectInfoTextBox = new System.Windows.Forms.TextBox();
        SpriteInfoPanel = new System.Windows.Forms.Panel();
        SpriteInfoTextBox = new System.Windows.Forms.TextBox();
        FinalStatePanel = new System.Windows.Forms.Panel();
        FinalStateTextBox = new System.Windows.Forms.TextBox();
        PaletteInfoPanel = new System.Windows.Forms.Panel();
        PaletteInfoTextBox = new System.Windows.Forms.TextBox();
        ListOverrideInfoPanel = new System.Windows.Forms.Panel();
        ListOverrideInfoTextBox = new System.Windows.Forms.TextBox();
        MappedOrGeneratedInfoPanel = new System.Windows.Forms.Panel();
        MappedOrGeneratedInfoTextBox = new System.Windows.Forms.TextBox();
        CoordinatesPanel = new System.Windows.Forms.Panel();
        CoordinatesTextBox = new System.Windows.Forms.TextBox();
        OptionsTab = new System.Windows.Forms.TabPage();
        HighlightBackgroundOverridesCheckBox = new System.Windows.Forms.CheckBox();
        ShowVisualOverlaysCheckBox = new System.Windows.Forms.CheckBox();
        BackgroundSceneryDropDown = new System.Windows.Forms.ComboBox();
        BackgroundLabel = new System.Windows.Forms.Label();
        HighlightBackgroundSceneryCheckBox = new System.Windows.Forms.CheckBox();
        HighlightBackgroundEventsCheckBox = new System.Windows.Forms.CheckBox();
        BackgroundObjectTree = new System.Windows.Forms.TreeView();
        HighlightMappedDataCheckBox = new System.Windows.Forms.CheckBox();
        ZoomLevelLabel = new System.Windows.Forms.Label();
        ZoomLevelDropDown = new System.Windows.Forms.ComboBox();
        SearchTab = new System.Windows.Forms.TabPage();
        AnimationTimer = new System.Windows.Forms.Timer(components);
        ((System.ComponentModel.ISupportInitialize)Splitter).BeginInit();
        Splitter.Panel1.SuspendLayout();
        Splitter.Panel2.SuspendLayout();
        Splitter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)WorldMap).BeginInit();
        TabControls.SuspendLayout();
        PropertiesTab.SuspendLayout();
        BackgroundObjectInfoPanel.SuspendLayout();
        OptionsTab.SuspendLayout();
        SuspendLayout();
        // 
        // Splitter
        // 
        Splitter.Dock = System.Windows.Forms.DockStyle.Fill;
        Splitter.Location = new System.Drawing.Point(0, 0);
        Splitter.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        Splitter.Name = "Splitter";
        // 
        // Splitter.Panel1
        // 
        Splitter.Panel1.Controls.Add(WorldMap);
        // 
        // Splitter.Panel2
        // 
        Splitter.Panel2.Controls.Add(TabControls);
        Splitter.Size = new System.Drawing.Size(1333, 1065);
        Splitter.SplitterDistance = 913;
        Splitter.SplitterWidth = 7;
        Splitter.TabIndex = 0;
        // 
        // WorldMap
        // 
        WorldMap.AllowUserToAddRows = false;
        WorldMap.AllowUserToDeleteRows = false;
        WorldMap.AllowUserToResizeColumns = false;
        WorldMap.AllowUserToResizeRows = false;
        WorldMap.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
        dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
        dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
        dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
        WorldMap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
        WorldMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
        dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
        dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
        WorldMap.DefaultCellStyle = dataGridViewCellStyle2;
        WorldMap.Dock = System.Windows.Forms.DockStyle.Fill;
        WorldMap.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
        WorldMap.Location = new System.Drawing.Point(0, 0);
        WorldMap.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        WorldMap.MultiSelect = false;
        WorldMap.Name = "WorldMap";
        WorldMap.ReadOnly = true;
        WorldMap.RowHeadersWidth = 50;
        WorldMap.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        WorldMap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
        WorldMap.ShowEditingIcon = false;
        WorldMap.ShowRowErrors = false;
        WorldMap.Size = new System.Drawing.Size(913, 1065);
        WorldMap.TabIndex = 0;
        WorldMap.VirtualMode = true;
        WorldMap.CellPainting += WorldMap_CellPainting;
        WorldMap.SelectionChanged += WorldMap_SelectionChanged;
        WorldMap.MouseDown += WorldMap_MouseDown;
        WorldMap.MouseMove += WorldMap_MouseMove;
        WorldMap.MouseUp += WorldMap_MouseUp;
        // 
        // TabControls
        // 
        TabControls.Controls.Add(PropertiesTab);
        TabControls.Controls.Add(OptionsTab);
        TabControls.Controls.Add(SearchTab);
        TabControls.Dock = System.Windows.Forms.DockStyle.Fill;
        TabControls.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        TabControls.Location = new System.Drawing.Point(0, 0);
        TabControls.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        TabControls.Name = "TabControls";
        TabControls.Padding = new System.Drawing.Point(6, 30);
        TabControls.SelectedIndex = 0;
        TabControls.Size = new System.Drawing.Size(413, 1065);
        TabControls.TabIndex = 1;
        // 
        // PropertiesTab
        // 
        PropertiesTab.Controls.Add(BackgroundObjectInfoPanel);
        PropertiesTab.Location = new System.Drawing.Point(4, 88);
        PropertiesTab.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        PropertiesTab.Name = "PropertiesTab";
        PropertiesTab.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
        PropertiesTab.Size = new System.Drawing.Size(405, 973);
        PropertiesTab.TabIndex = 1;
        PropertiesTab.Text = "Properties";
        PropertiesTab.UseVisualStyleBackColor = true;
        // 
        // BackgroundObjectInfoPanel
        // 
        BackgroundObjectInfoPanel.Controls.Add(BackgroundObjectInfoTextBox);
        BackgroundObjectInfoPanel.Controls.Add(SpriteInfoPanel);
        BackgroundObjectInfoPanel.Controls.Add(SpriteInfoTextBox);
        BackgroundObjectInfoPanel.Controls.Add(FinalStatePanel);
        BackgroundObjectInfoPanel.Controls.Add(FinalStateTextBox);
        BackgroundObjectInfoPanel.Controls.Add(PaletteInfoPanel);
        BackgroundObjectInfoPanel.Controls.Add(PaletteInfoTextBox);
        BackgroundObjectInfoPanel.Controls.Add(ListOverrideInfoPanel);
        BackgroundObjectInfoPanel.Controls.Add(ListOverrideInfoTextBox);
        BackgroundObjectInfoPanel.Controls.Add(MappedOrGeneratedInfoPanel);
        BackgroundObjectInfoPanel.Controls.Add(MappedOrGeneratedInfoTextBox);
        BackgroundObjectInfoPanel.Controls.Add(CoordinatesPanel);
        BackgroundObjectInfoPanel.Controls.Add(CoordinatesTextBox);
        BackgroundObjectInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        BackgroundObjectInfoPanel.Location = new System.Drawing.Point(4, 6);
        BackgroundObjectInfoPanel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        BackgroundObjectInfoPanel.Name = "BackgroundObjectInfoPanel";
        BackgroundObjectInfoPanel.Size = new System.Drawing.Size(397, 961);
        BackgroundObjectInfoPanel.TabIndex = 2;
        // 
        // BackgroundObjectInfoTextBox
        // 
        BackgroundObjectInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        BackgroundObjectInfoTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        BackgroundObjectInfoTextBox.Location = new System.Drawing.Point(0, 368);
        BackgroundObjectInfoTextBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        BackgroundObjectInfoTextBox.Multiline = true;
        BackgroundObjectInfoTextBox.Name = "BackgroundObjectInfoTextBox";
        BackgroundObjectInfoTextBox.ReadOnly = true;
        BackgroundObjectInfoTextBox.Size = new System.Drawing.Size(397, 35);
        BackgroundObjectInfoTextBox.TabIndex = 14;
        BackgroundObjectInfoTextBox.Text = "background object info";
        // 
        // SpriteInfoPanel
        // 
        SpriteInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
        SpriteInfoPanel.Location = new System.Drawing.Point(0, 339);
        SpriteInfoPanel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        SpriteInfoPanel.Name = "SpriteInfoPanel";
        SpriteInfoPanel.Size = new System.Drawing.Size(397, 29);
        SpriteInfoPanel.TabIndex = 13;
        // 
        // SpriteInfoTextBox
        // 
        SpriteInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        SpriteInfoTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        SpriteInfoTextBox.Location = new System.Drawing.Point(0, 304);
        SpriteInfoTextBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        SpriteInfoTextBox.Multiline = true;
        SpriteInfoTextBox.Name = "SpriteInfoTextBox";
        SpriteInfoTextBox.ReadOnly = true;
        SpriteInfoTextBox.Size = new System.Drawing.Size(397, 35);
        SpriteInfoTextBox.TabIndex = 12;
        SpriteInfoTextBox.Text = "sprite info";
        // 
        // FinalStatePanel
        // 
        FinalStatePanel.Dock = System.Windows.Forms.DockStyle.Top;
        FinalStatePanel.Location = new System.Drawing.Point(0, 275);
        FinalStatePanel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        FinalStatePanel.Name = "FinalStatePanel";
        FinalStatePanel.Size = new System.Drawing.Size(397, 29);
        FinalStatePanel.TabIndex = 11;
        // 
        // FinalStateTextBox
        // 
        FinalStateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        FinalStateTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        FinalStateTextBox.Location = new System.Drawing.Point(0, 245);
        FinalStateTextBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        FinalStateTextBox.Multiline = true;
        FinalStateTextBox.Name = "FinalStateTextBox";
        FinalStateTextBox.ReadOnly = true;
        FinalStateTextBox.Size = new System.Drawing.Size(397, 30);
        FinalStateTextBox.TabIndex = 10;
        FinalStateTextBox.Text = "final state";
        // 
        // PaletteInfoPanel
        // 
        PaletteInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
        PaletteInfoPanel.Location = new System.Drawing.Point(0, 216);
        PaletteInfoPanel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        PaletteInfoPanel.Name = "PaletteInfoPanel";
        PaletteInfoPanel.Size = new System.Drawing.Size(397, 29);
        PaletteInfoPanel.TabIndex = 9;
        // 
        // PaletteInfoTextBox
        // 
        PaletteInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        PaletteInfoTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        PaletteInfoTextBox.Location = new System.Drawing.Point(0, 181);
        PaletteInfoTextBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        PaletteInfoTextBox.Multiline = true;
        PaletteInfoTextBox.Name = "PaletteInfoTextBox";
        PaletteInfoTextBox.ReadOnly = true;
        PaletteInfoTextBox.Size = new System.Drawing.Size(397, 35);
        PaletteInfoTextBox.TabIndex = 8;
        PaletteInfoTextBox.Text = "palette info";
        // 
        // ListOverrideInfoPanel
        // 
        ListOverrideInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
        ListOverrideInfoPanel.Location = new System.Drawing.Point(0, 152);
        ListOverrideInfoPanel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        ListOverrideInfoPanel.Name = "ListOverrideInfoPanel";
        ListOverrideInfoPanel.Size = new System.Drawing.Size(397, 29);
        ListOverrideInfoPanel.TabIndex = 7;
        // 
        // ListOverrideInfoTextBox
        // 
        ListOverrideInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        ListOverrideInfoTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        ListOverrideInfoTextBox.Location = new System.Drawing.Point(0, 117);
        ListOverrideInfoTextBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        ListOverrideInfoTextBox.Multiline = true;
        ListOverrideInfoTextBox.Name = "ListOverrideInfoTextBox";
        ListOverrideInfoTextBox.ReadOnly = true;
        ListOverrideInfoTextBox.Size = new System.Drawing.Size(397, 35);
        ListOverrideInfoTextBox.TabIndex = 6;
        ListOverrideInfoTextBox.Text = "list overrides";
        // 
        // MappedOrGeneratedInfoPanel
        // 
        MappedOrGeneratedInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
        MappedOrGeneratedInfoPanel.Location = new System.Drawing.Point(0, 88);
        MappedOrGeneratedInfoPanel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        MappedOrGeneratedInfoPanel.Name = "MappedOrGeneratedInfoPanel";
        MappedOrGeneratedInfoPanel.Size = new System.Drawing.Size(397, 29);
        MappedOrGeneratedInfoPanel.TabIndex = 5;
        // 
        // MappedOrGeneratedInfoTextBox
        // 
        MappedOrGeneratedInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        MappedOrGeneratedInfoTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        MappedOrGeneratedInfoTextBox.Location = new System.Drawing.Point(0, 53);
        MappedOrGeneratedInfoTextBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        MappedOrGeneratedInfoTextBox.Multiline = true;
        MappedOrGeneratedInfoTextBox.Name = "MappedOrGeneratedInfoTextBox";
        MappedOrGeneratedInfoTextBox.ReadOnly = true;
        MappedOrGeneratedInfoTextBox.Size = new System.Drawing.Size(397, 35);
        MappedOrGeneratedInfoTextBox.TabIndex = 4;
        MappedOrGeneratedInfoTextBox.Text = "mapped/generated background info";
        // 
        // CoordinatesPanel
        // 
        CoordinatesPanel.Dock = System.Windows.Forms.DockStyle.Top;
        CoordinatesPanel.Location = new System.Drawing.Point(0, 24);
        CoordinatesPanel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        CoordinatesPanel.Name = "CoordinatesPanel";
        CoordinatesPanel.Size = new System.Drawing.Size(397, 29);
        CoordinatesPanel.TabIndex = 3;
        // 
        // CoordinatesTextBox
        // 
        CoordinatesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
        CoordinatesTextBox.Dock = System.Windows.Forms.DockStyle.Top;
        CoordinatesTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        CoordinatesTextBox.Location = new System.Drawing.Point(0, 0);
        CoordinatesTextBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        CoordinatesTextBox.Name = "CoordinatesTextBox";
        CoordinatesTextBox.ReadOnly = true;
        CoordinatesTextBox.Size = new System.Drawing.Size(397, 24);
        CoordinatesTextBox.TabIndex = 2;
        CoordinatesTextBox.Text = "X:xx Y:yy";
        // 
        // OptionsTab
        // 
        OptionsTab.Controls.Add(HighlightBackgroundOverridesCheckBox);
        OptionsTab.Controls.Add(ShowVisualOverlaysCheckBox);
        OptionsTab.Controls.Add(BackgroundSceneryDropDown);
        OptionsTab.Controls.Add(BackgroundLabel);
        OptionsTab.Controls.Add(HighlightBackgroundSceneryCheckBox);
        OptionsTab.Controls.Add(HighlightBackgroundEventsCheckBox);
        OptionsTab.Controls.Add(BackgroundObjectTree);
        OptionsTab.Controls.Add(HighlightMappedDataCheckBox);
        OptionsTab.Controls.Add(ZoomLevelLabel);
        OptionsTab.Controls.Add(ZoomLevelDropDown);
        OptionsTab.Location = new System.Drawing.Point(4, 88);
        OptionsTab.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        OptionsTab.Name = "OptionsTab";
        OptionsTab.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
        OptionsTab.Size = new System.Drawing.Size(405, 973);
        OptionsTab.TabIndex = 0;
        OptionsTab.Text = "Options";
        OptionsTab.UseVisualStyleBackColor = true;
        // 
        // HighlightBackgroundOverridesCheckBox
        // 
        HighlightBackgroundOverridesCheckBox.AutoSize = true;
        HighlightBackgroundOverridesCheckBox.Location = new System.Drawing.Point(17, 201);
        HighlightBackgroundOverridesCheckBox.Name = "HighlightBackgroundOverridesCheckBox";
        HighlightBackgroundOverridesCheckBox.Size = new System.Drawing.Size(306, 29);
        HighlightBackgroundOverridesCheckBox.TabIndex = 19;
        HighlightBackgroundOverridesCheckBox.Text = "Highlight background overrides";
        HighlightBackgroundOverridesCheckBox.UseVisualStyleBackColor = true;
        HighlightBackgroundOverridesCheckBox.CheckedChanged += HighlightBackgroundOverrides_CheckedChanged;
        // 
        // ShowVisualOverlaysCheckBox
        // 
        ShowVisualOverlaysCheckBox.AutoSize = true;
        ShowVisualOverlaysCheckBox.Checked = true;
        ShowVisualOverlaysCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        ShowVisualOverlaysCheckBox.Location = new System.Drawing.Point(17, 31);
        ShowVisualOverlaysCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        ShowVisualOverlaysCheckBox.Name = "ShowVisualOverlaysCheckBox";
        ShowVisualOverlaysCheckBox.Size = new System.Drawing.Size(221, 29);
        ShowVisualOverlaysCheckBox.TabIndex = 15;
        ShowVisualOverlaysCheckBox.Text = "Show visual overlays";
        ShowVisualOverlaysCheckBox.UseVisualStyleBackColor = true;
        ShowVisualOverlaysCheckBox.CheckedChanged += ShowVisualOverlaysCheckBox_CheckedChanged;
        // 
        // BackgroundSceneryDropDown
        // 
        BackgroundSceneryDropDown.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        BackgroundSceneryDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        BackgroundSceneryDropDown.FormattingEnabled = true;
        BackgroundSceneryDropDown.ItemHeight = 36;
        BackgroundSceneryDropDown.Location = new System.Drawing.Point(196, 822);
        BackgroundSceneryDropDown.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        BackgroundSceneryDropDown.Name = "BackgroundSceneryDropDown";
        BackgroundSceneryDropDown.Size = new System.Drawing.Size(182, 42);
        BackgroundSceneryDropDown.TabIndex = 24;
        BackgroundSceneryDropDown.DrawItem += BackgroundSceneryDropDown_DrawItem;
        BackgroundSceneryDropDown.SelectedIndexChanged += BackgroundSceneryDropDown_SelectedIndexChanged;
        // 
        // BackgroundLabel
        // 
        BackgroundLabel.AutoSize = true;
        BackgroundLabel.Location = new System.Drawing.Point(50, 829);
        BackgroundLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        BackgroundLabel.Name = "BackgroundLabel";
        BackgroundLabel.Size = new System.Drawing.Size(123, 25);
        BackgroundLabel.TabIndex = 23;
        BackgroundLabel.Text = "Background:";
        // 
        // HighlightBackgroundSceneryCheckBox
        // 
        HighlightBackgroundSceneryCheckBox.AutoSize = true;
        HighlightBackgroundSceneryCheckBox.Location = new System.Drawing.Point(17, 779);
        HighlightBackgroundSceneryCheckBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        HighlightBackgroundSceneryCheckBox.Name = "HighlightBackgroundSceneryCheckBox";
        HighlightBackgroundSceneryCheckBox.Size = new System.Drawing.Size(318, 29);
        HighlightBackgroundSceneryCheckBox.TabIndex = 22;
        HighlightBackgroundSceneryCheckBox.Text = "Highlight background scenery ♦";
        HighlightBackgroundSceneryCheckBox.UseVisualStyleBackColor = true;
        HighlightBackgroundSceneryCheckBox.CheckedChanged += HighlightBackgroundSceneryCheckBox_CheckedChanged;
        // 
        // HighlightBackgroundEventsCheckBox
        // 
        HighlightBackgroundEventsCheckBox.AutoSize = true;
        HighlightBackgroundEventsCheckBox.Location = new System.Drawing.Point(17, 258);
        HighlightBackgroundEventsCheckBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        HighlightBackgroundEventsCheckBox.Name = "HighlightBackgroundEventsCheckBox";
        HighlightBackgroundEventsCheckBox.Size = new System.Drawing.Size(308, 29);
        HighlightBackgroundEventsCheckBox.TabIndex = 20;
        HighlightBackgroundEventsCheckBox.Text = "Highlight background events: ○";
        HighlightBackgroundEventsCheckBox.UseVisualStyleBackColor = true;
        HighlightBackgroundEventsCheckBox.CheckedChanged += HighlightBackgroundEventsCheckBox_CheckedChanged;
        // 
        // BackgroundObjectTree
        // 
        BackgroundObjectTree.CheckBoxes = true;
        BackgroundObjectTree.HideSelection = false;
        BackgroundObjectTree.Location = new System.Drawing.Point(38, 299);
        BackgroundObjectTree.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        BackgroundObjectTree.Name = "BackgroundObjectTree";
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
        BackgroundObjectTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { treeNode19 });
        BackgroundObjectTree.ShowRootLines = false;
        BackgroundObjectTree.Size = new System.Drawing.Size(337, 446);
        BackgroundObjectTree.TabIndex = 21;
        BackgroundObjectTree.AfterCheck += BackgroundObjectTree_AfterCheck;
        // 
        // HighlightMappedDataCheckBox
        // 
        HighlightMappedDataCheckBox.AutoSize = true;
        HighlightMappedDataCheckBox.Location = new System.Drawing.Point(17, 144);
        HighlightMappedDataCheckBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        HighlightMappedDataCheckBox.Name = "HighlightMappedDataCheckBox";
        HighlightMappedDataCheckBox.Size = new System.Drawing.Size(249, 29);
        HighlightMappedDataCheckBox.TabIndex = 18;
        HighlightMappedDataCheckBox.Text = "Highlight mapped data □";
        HighlightMappedDataCheckBox.UseVisualStyleBackColor = true;
        HighlightMappedDataCheckBox.CheckedChanged += HighlightMappedDataCheckBox_CheckedChanged;
        // 
        // ZoomLevelLabel
        // 
        ZoomLevelLabel.AutoSize = true;
        ZoomLevelLabel.Location = new System.Drawing.Point(11, 90);
        ZoomLevelLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        ZoomLevelLabel.Name = "ZoomLevelLabel";
        ZoomLevelLabel.Size = new System.Drawing.Size(113, 25);
        ZoomLevelLabel.TabIndex = 16;
        ZoomLevelLabel.Text = "Zoom level:";
        // 
        // ZoomLevelDropDown
        // 
        ZoomLevelDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        ZoomLevelDropDown.FormattingEnabled = true;
        ZoomLevelDropDown.Location = new System.Drawing.Point(151, 90);
        ZoomLevelDropDown.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        ZoomLevelDropDown.Name = "ZoomLevelDropDown";
        ZoomLevelDropDown.Size = new System.Drawing.Size(177, 33);
        ZoomLevelDropDown.TabIndex = 17;
        ZoomLevelDropDown.SelectedIndexChanged += ZoomLevelDropDown_SelectedIndexChanged;
        // 
        // SearchTab
        // 
        SearchTab.Location = new System.Drawing.Point(4, 88);
        SearchTab.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        SearchTab.Name = "SearchTab";
        SearchTab.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
        SearchTab.Size = new System.Drawing.Size(405, 973);
        SearchTab.TabIndex = 2;
        SearchTab.Text = "Search";
        SearchTab.UseVisualStyleBackColor = true;
        // 
        // AnimationTimer
        // 
        AnimationTimer.Tick += AnimationTimer_Tick;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1333, 1065);
        Controls.Add(Splitter);
        Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
        Name = "MainForm";
        SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
        Text = "The Land of the Exile";
        Shown += MainForm_Shown;
        Splitter.Panel1.ResumeLayout(false);
        Splitter.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)Splitter).EndInit();
        Splitter.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)WorldMap).EndInit();
        TabControls.ResumeLayout(false);
        PropertiesTab.ResumeLayout(false);
        BackgroundObjectInfoPanel.ResumeLayout(false);
        BackgroundObjectInfoPanel.PerformLayout();
        OptionsTab.ResumeLayout(false);
        OptionsTab.PerformLayout();
        ResumeLayout(false);
            }

        #endregion
        private System.Windows.Forms.Timer AnimationTimer;
        private System.Windows.Forms.SplitContainer Splitter;
        private System.Windows.Forms.DataGridView WorldMap;

        private System.Windows.Forms.TabControl TabControls;
        private System.Windows.Forms.TabPage PropertiesTab;
        private System.Windows.Forms.TextBox CoordinatesTextBox;
        private System.Windows.Forms.Panel CoordinatesPanel;
        private System.Windows.Forms.TextBox MappedOrGeneratedInfoTextBox;
        private System.Windows.Forms.Panel MappedOrGeneratedInfoPanel;
        private System.Windows.Forms.TextBox ListOverrideInfoTextBox;
        private System.Windows.Forms.Panel ListOverrideInfoPanel;
        private System.Windows.Forms.TextBox PaletteInfoTextBox;
        private System.Windows.Forms.Panel PaletteInfoPanel;
        private System.Windows.Forms.TextBox SpriteInfoTextBox;
        private System.Windows.Forms.Panel SpriteInfoPanel;
        private System.Windows.Forms.TextBox BackgroundObjectInfoTextBox;
        private System.Windows.Forms.Panel BackgroundObjectInfoPanel;

        private System.Windows.Forms.TabPage OptionsTab;
        private System.Windows.Forms.CheckBox ShowVisualOverlaysCheckBox;
        private System.Windows.Forms.Label ZoomLevelLabel;
        private System.Windows.Forms.ComboBox ZoomLevelDropDown;
        private System.Windows.Forms.CheckBox HighlightMappedDataCheckBox;
        private System.Windows.Forms.CheckBox HighlightBackgroundEventsCheckBox;
        private System.Windows.Forms.TreeView BackgroundObjectTree;
        private System.Windows.Forms.CheckBox HighlightBackgroundSceneryCheckBox;
        private System.Windows.Forms.Label BackgroundLabel;
        private System.Windows.Forms.ComboBox BackgroundSceneryDropDown;

        private System.Windows.Forms.TabPage SearchTab;
        private System.Windows.Forms.TextBox FinalStateTextBox;
        private System.Windows.Forms.Panel FinalStatePanel;
        private System.Windows.Forms.CheckBox HighlightBackgroundOverridesCheckBox;
        }
    }

