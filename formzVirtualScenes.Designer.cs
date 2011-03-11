using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System;
namespace zVirtualScenesApplication
{
    partial class formzVirtualScenes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formzVirtualScenes));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelLastEvent = new System.Windows.Forms.Label();
            this.dataListViewDevices = new BrightIdeasSoftware.DataListView();
            this.NodeCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.NameCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.LevelCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.LevelTextCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ModeCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.FanModeCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.SetPointCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.currStateCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.GroupCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageListActionTypesSmall = new System.Windows.Forms.ImageList(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataListViewScenes = new BrightIdeasSoftware.DataListView();
            this.SceneNamecol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.dataListViewActions = new BrightIdeasSoftware.DataListView();
            this.ColType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColAction = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btn_AddAction = new System.Windows.Forms.Button();
            this.labelSceneRunStatus = new System.Windows.Forms.Label();
            this.btn_createnonzwaction = new System.Windows.Forms.Button();
            this.comboBoxNonZWAction = new System.Windows.Forms.ComboBox();
            this.lbl_sceneActions = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dataListTasks = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.EnabledCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.FreqCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_TaskName = new System.Windows.Forms.TextBox();
            this.button_SaveTask = new System.Windows.Forms.Button();
            this.comboBox_ActionsTask = new System.Windows.Forms.ComboBox();
            this.comboBox_FrequencyTask = new System.Windows.Forms.ComboBox();
            this.groupBox_Weekly = new System.Windows.Forms.GroupBox();
            this.checkBox_RecurSunday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurSaturday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurFriday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurThursday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurWednesday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurTuesday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurMonday = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_RecurWeeks = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.dateTimePickerStartTask = new System.Windows.Forms.DateTimePicker();
            this.label18 = new System.Windows.Forms.Label();
            this.checkBox_EnabledTask = new System.Windows.Forms.CheckBox();
            this.groupBoxDaily = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox_DaysRecur = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.labelSaveStatus = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Label_SunriseSet = new System.Windows.Forms.Label();
            this.checkBoxEnableNOAA = new System.Windows.Forms.CheckBox();
            this.textBox_Latitude = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Longitude = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxRepolling = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSaveSettings = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.checkBox_HideJabberPassword = new System.Windows.Forms.CheckBox();
            this.textBoxJabberUserTo = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxJabberServer = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxJabberPassword = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxJabberUser = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxJabberVerbose = new System.Windows.Forms.CheckBox();
            this.checkBoxJabberEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox_HideLSPassword = new System.Windows.Forms.CheckBox();
            this.checkBoxLSDAuth = new System.Windows.Forms.CheckBox();
            this.textBoxLSLimit = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxLSport = new System.Windows.Forms.TextBox();
            this.textBoxLSPassword = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxLSDebugVerbose = new System.Windows.Forms.CheckBox();
            this.checkBoxLSEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtb_exampleURL = new System.Windows.Forms.TextBox();
            this.checkBoxHTTPEnable = new System.Windows.Forms.CheckBox();
            this.txtb_httpPort = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer_TaskRunner = new System.Windows.Forms.Timer(this.components);
            this.timerNOAA = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripScenes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripScenesNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripDevices = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.adjustLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.manuallyRepollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNewDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconnectToControlThinkUSBToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.devicePropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTasks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTasksNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripAddTaks = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripDevicesNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.findNewDevicesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.reconnectToControlThinkUSBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDevices)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewScenes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox_Weekly.SuspendLayout();
            this.groupBoxDaily.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.contextMenuStripScenes.SuspendLayout();
            this.contextMenuStripScenesNull.SuspendLayout();
            this.contextMenuStripActions.SuspendLayout();
            this.contextMenuStripDevices.SuspendLayout();
            this.contextMenuStripTasks.SuspendLayout();
            this.contextMenuStripTasksNull.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStripDevicesNull.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Controls.Add(this.tabPage4);
            this.MainTabControl.Controls.Add(this.tabPage3);
            this.MainTabControl.Controls.Add(this.tabPage2);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 24);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(784, 489);
            this.MainTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.pictureBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(776, 463);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Scenes";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.labelLastEvent);
            this.splitContainer1.Panel1.Controls.Add(this.dataListViewDevices);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(770, 457);
            this.splitContainer1.SplitterDistance = 172;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 33;
            // 
            // labelLastEvent
            // 
            this.labelLastEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLastEvent.AutoSize = true;
            this.labelLastEvent.Location = new System.Drawing.Point(1, 155);
            this.labelLastEvent.Name = "labelLastEvent";
            this.labelLastEvent.Size = new System.Drawing.Size(64, 13);
            this.labelLastEvent.TabIndex = 22;
            this.labelLastEvent.Text = "Last Event: ";
            // 
            // dataListViewDevices
            // 
            this.dataListViewDevices.AllColumns.Add(this.NodeCol);
            this.dataListViewDevices.AllColumns.Add(this.NameCol);
            this.dataListViewDevices.AllColumns.Add(this.LevelCol);
            this.dataListViewDevices.AllColumns.Add(this.LevelTextCol);
            this.dataListViewDevices.AllColumns.Add(this.ModeCol);
            this.dataListViewDevices.AllColumns.Add(this.FanModeCol);
            this.dataListViewDevices.AllColumns.Add(this.SetPointCol);
            this.dataListViewDevices.AllColumns.Add(this.currStateCol);
            this.dataListViewDevices.AllColumns.Add(this.GroupCol);
            this.dataListViewDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListViewDevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NodeCol,
            this.NameCol,
            this.LevelCol,
            this.LevelTextCol,
            this.ModeCol,
            this.FanModeCol,
            this.SetPointCol,
            this.currStateCol,
            this.GroupCol});
            this.dataListViewDevices.DataSource = null;
            this.dataListViewDevices.FullRowSelect = true;
            this.dataListViewDevices.HasCollapsibleGroups = false;
            this.dataListViewDevices.HeaderMaximumHeight = 15;
            this.dataListViewDevices.HideSelection = false;
            this.dataListViewDevices.IsSimpleDragSource = true;
            this.dataListViewDevices.Location = new System.Drawing.Point(0, 0);
            this.dataListViewDevices.MultiSelect = false;
            this.dataListViewDevices.Name = "dataListViewDevices";
            this.dataListViewDevices.OwnerDraw = true;
            this.dataListViewDevices.ShowCommandMenuOnRightClick = true;
            this.dataListViewDevices.ShowGroups = false;
            this.dataListViewDevices.Size = new System.Drawing.Size(770, 152);
            this.dataListViewDevices.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewDevices.TabIndex = 30;
            this.dataListViewDevices.UseCompatibleStateImageBehavior = false;
            this.dataListViewDevices.View = System.Windows.Forms.View.Details;
            this.dataListViewDevices.DoubleClick += new System.EventHandler(this.dataListViewDevices_DoubleClick);
            // 
            // NodeCol
            // 
            this.NodeCol.AspectName = "NodeID";
            this.NodeCol.ImageAspectName = "DeviceIcon";
            this.NodeCol.IsEditable = false;
            this.NodeCol.Text = "ID";
            this.NodeCol.Width = 45;
            // 
            // NameCol
            // 
            this.NameCol.AspectName = "Name";
            this.NameCol.IsEditable = false;
            this.NameCol.Text = "Name";
            this.NameCol.Width = 100;
            // 
            // LevelCol
            // 
            this.LevelCol.AspectName = "GetLevelMeter";
            this.LevelCol.IsEditable = false;
            this.LevelCol.Text = "Level";
            // 
            // LevelTextCol
            // 
            this.LevelTextCol.AspectName = "GetLevelText";
            this.LevelTextCol.IsEditable = false;
            this.LevelTextCol.Text = "";
            this.LevelTextCol.Width = 40;
            // 
            // ModeCol
            // 
            this.ModeCol.AspectName = "GetMode";
            this.ModeCol.IsEditable = false;
            this.ModeCol.Text = "Mode";
            this.ModeCol.Width = 80;
            // 
            // FanModeCol
            // 
            this.FanModeCol.AspectName = "GetFanMode";
            this.FanModeCol.IsEditable = false;
            this.FanModeCol.Text = "Fan Mode";
            this.FanModeCol.Width = 80;
            // 
            // SetPointCol
            // 
            this.SetPointCol.AspectName = "GetSetPoint";
            this.SetPointCol.IsEditable = false;
            this.SetPointCol.Text = "Set Point";
            this.SetPointCol.Width = 140;
            // 
            // currStateCol
            // 
            this.currStateCol.AspectName = "GetCurrentState";
            this.currStateCol.IsEditable = false;
            this.currStateCol.Text = "Currently";
            this.currStateCol.Width = 80;
            // 
            // GroupCol
            // 
            this.GroupCol.AspectName = "GroupName";
            this.GroupCol.IsEditable = false;
            this.GroupCol.Text = "Group";
            this.GroupCol.Width = 110;
            // 
            // imageListActionTypesSmall
            // 
            this.imageListActionTypesSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListActionTypesSmall.ImageStream")));
            this.imageListActionTypesSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListActionTypesSmall.Images.SetKeyName(0, "20delay.png");
            this.imageListActionTypesSmall.Images.SetKeyName(1, "20exe.png");
            this.imageListActionTypesSmall.Images.SetKeyName(2, "20zwave-default.jpg");
            this.imageListActionTypesSmall.Images.SetKeyName(3, "20zwave-thermostat.png");
            this.imageListActionTypesSmall.Images.SetKeyName(4, "20scene_icon.jpg");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataListViewScenes);
            this.groupBox2.Controls.Add(this.dataListViewActions);
            this.groupBox2.Controls.Add(this.btn_AddAction);
            this.groupBox2.Controls.Add(this.labelSceneRunStatus);
            this.groupBox2.Controls.Add(this.btn_createnonzwaction);
            this.groupBox2.Controls.Add(this.comboBoxNonZWAction);
            this.groupBox2.Controls.Add(this.lbl_sceneActions);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(770, 284);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scenes";
            // 
            // dataListViewScenes
            // 
            this.dataListViewScenes.AllColumns.Add(this.SceneNamecol);
            this.dataListViewScenes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.dataListViewScenes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SceneNamecol});
            this.dataListViewScenes.DataSource = null;
            this.dataListViewScenes.FullRowSelect = true;
            this.dataListViewScenes.HeaderMaximumHeight = 15;
            this.dataListViewScenes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListViewScenes.HideSelection = false;
            this.dataListViewScenes.Location = new System.Drawing.Point(6, 20);
            this.dataListViewScenes.MultiSelect = false;
            this.dataListViewScenes.Name = "dataListViewScenes";
            this.dataListViewScenes.OwnerDraw = true;
            this.dataListViewScenes.ShowGroups = false;
            this.dataListViewScenes.Size = new System.Drawing.Size(197, 261);
            this.dataListViewScenes.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewScenes.TabIndex = 32;
            this.dataListViewScenes.UnfocusedHighlightBackgroundColor = System.Drawing.Color.SkyBlue;
            this.dataListViewScenes.UseCompatibleStateImageBehavior = false;
            this.dataListViewScenes.UseExplorerTheme = true;
            this.dataListViewScenes.View = System.Windows.Forms.View.Details;
            this.dataListViewScenes.SelectedIndexChanged += new System.EventHandler(this.dataListViewScenes_SelectedIndexChanged);
            // 
            // SceneNamecol
            // 
            this.SceneNamecol.AspectName = "Name";
            this.SceneNamecol.FillsFreeSpace = true;
            this.SceneNamecol.ImageAspectName = "getIcon";
            this.SceneNamecol.IsEditable = false;
            this.SceneNamecol.Text = "Scene Name";
            this.SceneNamecol.Width = 180;
            // 
            // dataListViewActions
            // 
            this.dataListViewActions.AllColumns.Add(this.ColType);
            this.dataListViewActions.AllColumns.Add(this.ColName);
            this.dataListViewActions.AllColumns.Add(this.ColAction);
            this.dataListViewActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListViewActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColType,
            this.ColName,
            this.ColAction});
            this.dataListViewActions.DataSource = null;
            this.dataListViewActions.FullRowSelect = true;
            this.dataListViewActions.HeaderMaximumHeight = 15;
            this.dataListViewActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListViewActions.HideSelection = false;
            this.dataListViewActions.Location = new System.Drawing.Point(209, 36);
            this.dataListViewActions.MultiSelect = false;
            this.dataListViewActions.Name = "dataListViewActions";
            this.dataListViewActions.OwnerDraw = true;
            this.dataListViewActions.ShowGroups = false;
            this.dataListViewActions.Size = new System.Drawing.Size(555, 194);
            this.dataListViewActions.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewActions.TabIndex = 31;
            this.dataListViewActions.UseCompatibleStateImageBehavior = false;
            this.dataListViewActions.UseExplorerTheme = true;
            this.dataListViewActions.View = System.Windows.Forms.View.Details;
            // 
            // ColType
            // 
            this.ColType.AspectName = "";
            this.ColType.ImageAspectName = "TypeIcon";
            this.ColType.Text = "";
            this.ColType.Width = 25;
            // 
            // ColName
            // 
            this.ColName.AspectName = "Name";
            this.ColName.IsEditable = false;
            this.ColName.Text = "Device";
            this.ColName.Width = 110;
            // 
            // ColAction
            // 
            this.ColAction.AspectName = "ActionToString";
            this.ColAction.Text = "Action(s)";
            this.ColAction.Width = 380;
            // 
            // btn_AddAction
            // 
            this.btn_AddAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_AddAction.Location = new System.Drawing.Point(632, 233);
            this.btn_AddAction.Name = "btn_AddAction";
            this.btn_AddAction.Size = new System.Drawing.Size(132, 20);
            this.btn_AddAction.TabIndex = 25;
            this.btn_AddAction.Text = "Add ZWave Action";
            this.btn_AddAction.UseVisualStyleBackColor = true;
            this.btn_AddAction.Click += new System.EventHandler(this.btn_AddAction_Click);
            // 
            // labelSceneRunStatus
            // 
            this.labelSceneRunStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSceneRunStatus.AutoSize = true;
            this.labelSceneRunStatus.Location = new System.Drawing.Point(211, 263);
            this.labelSceneRunStatus.Name = "labelSceneRunStatus";
            this.labelSceneRunStatus.Size = new System.Drawing.Size(158, 13);
            this.labelSceneRunStatus.TabIndex = 23;
            this.labelSceneRunStatus.Text = "Last Scene Completetion Status";
            // 
            // btn_createnonzwaction
            // 
            this.btn_createnonzwaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_createnonzwaction.Location = new System.Drawing.Point(209, 233);
            this.btn_createnonzwaction.Name = "btn_createnonzwaction";
            this.btn_createnonzwaction.Size = new System.Drawing.Size(131, 20);
            this.btn_createnonzwaction.TabIndex = 21;
            this.btn_createnonzwaction.Text = "Add Non-Zwave Action";
            this.btn_createnonzwaction.UseVisualStyleBackColor = true;
            this.btn_createnonzwaction.Click += new System.EventHandler(this.btn_createnonzwaction_Click);
            // 
            // comboBoxNonZWAction
            // 
            this.comboBoxNonZWAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxNonZWAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNonZWAction.FormattingEnabled = true;
            this.comboBoxNonZWAction.Items.AddRange(new object[] {
            "Create Delay Timer",
            "Launch EXE"});
            this.comboBoxNonZWAction.Location = new System.Drawing.Point(345, 234);
            this.comboBoxNonZWAction.Name = "comboBoxNonZWAction";
            this.comboBoxNonZWAction.Size = new System.Drawing.Size(144, 21);
            this.comboBoxNonZWAction.TabIndex = 23;
            // 
            // lbl_sceneActions
            // 
            this.lbl_sceneActions.AutoSize = true;
            this.lbl_sceneActions.Location = new System.Drawing.Point(206, 20);
            this.lbl_sceneActions.Name = "lbl_sceneActions";
            this.lbl_sceneActions.Size = new System.Drawing.Size(76, 13);
            this.lbl_sceneActions.TabIndex = 6;
            this.lbl_sceneActions.Text = "Scene Actions";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::zVirtualScenesApplication.Properties.Resources._20zwave_default;
            this.pictureBox3.Location = new System.Drawing.Point(21, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(21, 22);
            this.pictureBox3.TabIndex = 32;
            this.pictureBox3.TabStop = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dataListTasks);
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(776, 463);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Scheduling";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dataListTasks
            // 
            this.dataListTasks.AllColumns.Add(this.olvColumn1);
            this.dataListTasks.AllColumns.Add(this.EnabledCol);
            this.dataListTasks.AllColumns.Add(this.FreqCol);
            this.dataListTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.EnabledCol,
            this.FreqCol});
            this.dataListTasks.DataSource = null;
            this.dataListTasks.FullRowSelect = true;
            this.dataListTasks.HasCollapsibleGroups = false;
            this.dataListTasks.HeaderMaximumHeight = 15;
            this.dataListTasks.HideSelection = false;
            this.dataListTasks.Location = new System.Drawing.Point(0, 1);
            this.dataListTasks.Name = "dataListTasks";
            this.dataListTasks.OwnerDraw = true;
            this.dataListTasks.ShowCommandMenuOnRightClick = true;
            this.dataListTasks.ShowGroups = false;
            this.dataListTasks.Size = new System.Drawing.Size(374, 455);
            this.dataListTasks.TabIndex = 46;
            this.dataListTasks.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.InactiveCaption;
            this.dataListTasks.UseCompatibleStateImageBehavior = false;
            this.dataListTasks.View = System.Windows.Forms.View.Details;
            this.dataListTasks.SelectedIndexChanged += new System.EventHandler(this.dataListTasks_SelectedIndexChanged_1);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "GetName";
            this.olvColumn1.ImageAspectName = "GetIcon";
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Text = "Task Name";
            this.olvColumn1.Width = 185;
            // 
            // EnabledCol
            // 
            this.EnabledCol.AspectName = "isEnabled";
            this.EnabledCol.IsEditable = false;
            this.EnabledCol.Text = "Enabled";
            this.EnabledCol.Width = 52;
            // 
            // FreqCol
            // 
            this.FreqCol.AspectName = "FrequencyString";
            this.FreqCol.ImageAspectName = "";
            this.FreqCol.Text = "Frequency";
            this.FreqCol.Width = 120;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBox_TaskName);
            this.groupBox1.Controls.Add(this.button_SaveTask);
            this.groupBox1.Controls.Add(this.comboBox_ActionsTask);
            this.groupBox1.Controls.Add(this.comboBox_FrequencyTask);
            this.groupBox1.Controls.Add(this.groupBox_Weekly);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.dateTimePickerStartTask);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.checkBox_EnabledTask);
            this.groupBox1.Controls.Add(this.groupBoxDaily);
            this.groupBox1.Location = new System.Drawing.Point(379, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(389, 383);
            this.groupBox1.TabIndex = 45;
            this.groupBox1.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::zVirtualScenesApplication.Properties.Resources._48schedule_icon;
            this.pictureBox1.Location = new System.Drawing.Point(5, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(51, 49);
            this.pictureBox1.TabIndex = 44;
            this.pictureBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(60, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Task Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Activate Action:";
            // 
            // textBox_TaskName
            // 
            this.textBox_TaskName.Location = new System.Drawing.Point(131, 19);
            this.textBox_TaskName.Name = "textBox_TaskName";
            this.textBox_TaskName.Size = new System.Drawing.Size(219, 20);
            this.textBox_TaskName.TabIndex = 0;
            // 
            // button_SaveTask
            // 
            this.button_SaveTask.Location = new System.Drawing.Point(286, 352);
            this.button_SaveTask.Name = "button_SaveTask";
            this.button_SaveTask.Size = new System.Drawing.Size(97, 25);
            this.button_SaveTask.TabIndex = 38;
            this.button_SaveTask.Text = "Save";
            this.button_SaveTask.UseVisualStyleBackColor = true;
            this.button_SaveTask.Click += new System.EventHandler(this.button_SaveTask_Click);
            // 
            // comboBox_ActionsTask
            // 
            this.comboBox_ActionsTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ActionsTask.FormattingEnabled = true;
            this.comboBox_ActionsTask.Location = new System.Drawing.Point(103, 75);
            this.comboBox_ActionsTask.Name = "comboBox_ActionsTask";
            this.comboBox_ActionsTask.Size = new System.Drawing.Size(277, 21);
            this.comboBox_ActionsTask.TabIndex = 42;
            // 
            // comboBox_FrequencyTask
            // 
            this.comboBox_FrequencyTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_FrequencyTask.FormattingEnabled = true;
            this.comboBox_FrequencyTask.Location = new System.Drawing.Point(153, 45);
            this.comboBox_FrequencyTask.Name = "comboBox_FrequencyTask";
            this.comboBox_FrequencyTask.Size = new System.Drawing.Size(95, 21);
            this.comboBox_FrequencyTask.TabIndex = 33;
            this.comboBox_FrequencyTask.SelectedIndexChanged += new System.EventHandler(this.comboBox_FrequencyTask_SelectedIndexChanged);
            // 
            // groupBox_Weekly
            // 
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurSunday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurSaturday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurFriday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurThursday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurWednesday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurTuesday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurMonday);
            this.groupBox_Weekly.Controls.Add(this.label7);
            this.groupBox_Weekly.Controls.Add(this.label12);
            this.groupBox_Weekly.Controls.Add(this.textBox_RecurWeeks);
            this.groupBox_Weekly.Location = new System.Drawing.Point(60, 157);
            this.groupBox_Weekly.Name = "groupBox_Weekly";
            this.groupBox_Weekly.Size = new System.Drawing.Size(206, 165);
            this.groupBox_Weekly.TabIndex = 41;
            this.groupBox_Weekly.TabStop = false;
            this.groupBox_Weekly.Text = "Weekly";
            // 
            // checkBox_RecurSunday
            // 
            this.checkBox_RecurSunday.AutoSize = true;
            this.checkBox_RecurSunday.Location = new System.Drawing.Point(124, 71);
            this.checkBox_RecurSunday.Name = "checkBox_RecurSunday";
            this.checkBox_RecurSunday.Size = new System.Drawing.Size(62, 17);
            this.checkBox_RecurSunday.TabIndex = 48;
            this.checkBox_RecurSunday.Text = "Sunday";
            this.checkBox_RecurSunday.UseVisualStyleBackColor = true;
            // 
            // checkBox_RecurSaturday
            // 
            this.checkBox_RecurSaturday.AutoSize = true;
            this.checkBox_RecurSaturday.Location = new System.Drawing.Point(124, 48);
            this.checkBox_RecurSaturday.Name = "checkBox_RecurSaturday";
            this.checkBox_RecurSaturday.Size = new System.Drawing.Size(68, 17);
            this.checkBox_RecurSaturday.TabIndex = 47;
            this.checkBox_RecurSaturday.Text = "Saturday";
            this.checkBox_RecurSaturday.UseVisualStyleBackColor = true;
            // 
            // checkBox_RecurFriday
            // 
            this.checkBox_RecurFriday.AutoSize = true;
            this.checkBox_RecurFriday.Location = new System.Drawing.Point(22, 140);
            this.checkBox_RecurFriday.Name = "checkBox_RecurFriday";
            this.checkBox_RecurFriday.Size = new System.Drawing.Size(54, 17);
            this.checkBox_RecurFriday.TabIndex = 46;
            this.checkBox_RecurFriday.Text = "Friday";
            this.checkBox_RecurFriday.UseVisualStyleBackColor = true;
            // 
            // checkBox_RecurThursday
            // 
            this.checkBox_RecurThursday.AutoSize = true;
            this.checkBox_RecurThursday.Location = new System.Drawing.Point(22, 117);
            this.checkBox_RecurThursday.Name = "checkBox_RecurThursday";
            this.checkBox_RecurThursday.Size = new System.Drawing.Size(70, 17);
            this.checkBox_RecurThursday.TabIndex = 45;
            this.checkBox_RecurThursday.Text = "Thursday";
            this.checkBox_RecurThursday.UseVisualStyleBackColor = true;
            // 
            // checkBox_RecurWednesday
            // 
            this.checkBox_RecurWednesday.AutoSize = true;
            this.checkBox_RecurWednesday.Location = new System.Drawing.Point(22, 94);
            this.checkBox_RecurWednesday.Name = "checkBox_RecurWednesday";
            this.checkBox_RecurWednesday.Size = new System.Drawing.Size(83, 17);
            this.checkBox_RecurWednesday.TabIndex = 44;
            this.checkBox_RecurWednesday.Text = "Wednesday";
            this.checkBox_RecurWednesday.UseVisualStyleBackColor = true;
            // 
            // checkBox_RecurTuesday
            // 
            this.checkBox_RecurTuesday.AutoSize = true;
            this.checkBox_RecurTuesday.Location = new System.Drawing.Point(22, 71);
            this.checkBox_RecurTuesday.Name = "checkBox_RecurTuesday";
            this.checkBox_RecurTuesday.Size = new System.Drawing.Size(67, 17);
            this.checkBox_RecurTuesday.TabIndex = 43;
            this.checkBox_RecurTuesday.Text = "Tuesday";
            this.checkBox_RecurTuesday.UseVisualStyleBackColor = true;
            // 
            // checkBox_RecurMonday
            // 
            this.checkBox_RecurMonday.AutoSize = true;
            this.checkBox_RecurMonday.Location = new System.Drawing.Point(22, 48);
            this.checkBox_RecurMonday.Name = "checkBox_RecurMonday";
            this.checkBox_RecurMonday.Size = new System.Drawing.Size(64, 17);
            this.checkBox_RecurMonday.TabIndex = 42;
            this.checkBox_RecurMonday.Text = "Monday";
            this.checkBox_RecurMonday.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(146, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 40;
            this.label7.Text = "weeks.";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "Recur every: ";
            // 
            // textBox_RecurWeeks
            // 
            this.textBox_RecurWeeks.Location = new System.Drawing.Point(90, 19);
            this.textBox_RecurWeeks.Name = "textBox_RecurWeeks";
            this.textBox_RecurWeeks.Size = new System.Drawing.Size(50, 20);
            this.textBox_RecurWeeks.TabIndex = 38;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(60, 48);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(87, 13);
            this.label17.TabIndex = 34;
            this.label17.Text = "Task Frequency:";
            // 
            // dateTimePickerStartTask
            // 
            this.dateTimePickerStartTask.CustomFormat = "dddd,MMMM d, yyyy \'at\' h:mm:ss tt";
            this.dateTimePickerStartTask.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStartTask.Location = new System.Drawing.Point(63, 108);
            this.dateTimePickerStartTask.Name = "dateTimePickerStartTask";
            this.dateTimePickerStartTask.Size = new System.Drawing.Size(271, 20);
            this.dateTimePickerStartTask.TabIndex = 35;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(25, 111);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 13);
            this.label18.TabIndex = 36;
            this.label18.Text = "Start:";
            // 
            // checkBox_EnabledTask
            // 
            this.checkBox_EnabledTask.AutoSize = true;
            this.checkBox_EnabledTask.Location = new System.Drawing.Point(63, 134);
            this.checkBox_EnabledTask.Name = "checkBox_EnabledTask";
            this.checkBox_EnabledTask.Size = new System.Drawing.Size(65, 17);
            this.checkBox_EnabledTask.TabIndex = 39;
            this.checkBox_EnabledTask.Text = "Enabled";
            this.checkBox_EnabledTask.UseVisualStyleBackColor = true;
            // 
            // groupBoxDaily
            // 
            this.groupBoxDaily.Controls.Add(this.label19);
            this.groupBoxDaily.Controls.Add(this.label20);
            this.groupBoxDaily.Controls.Add(this.textBox_DaysRecur);
            this.groupBoxDaily.Location = new System.Drawing.Point(60, 157);
            this.groupBoxDaily.Name = "groupBoxDaily";
            this.groupBoxDaily.Size = new System.Drawing.Size(206, 54);
            this.groupBoxDaily.TabIndex = 37;
            this.groupBoxDaily.TabStop = false;
            this.groupBoxDaily.Text = "Daily";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(146, 22);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 13);
            this.label19.TabIndex = 40;
            this.label19.Text = "days.";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(19, 22);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(71, 13);
            this.label20.TabIndex = 39;
            this.label20.Text = "Recur every: ";
            // 
            // textBox_DaysRecur
            // 
            this.textBox_DaysRecur.Location = new System.Drawing.Point(90, 19);
            this.textBox_DaysRecur.Name = "textBox_DaysRecur";
            this.textBox_DaysRecur.Size = new System.Drawing.Size(50, 20);
            this.textBox_DaysRecur.TabIndex = 38;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.labelSaveStatus);
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.buttonSaveSettings);
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(776, 463);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // labelSaveStatus
            // 
            this.labelSaveStatus.AutoSize = true;
            this.labelSaveStatus.Location = new System.Drawing.Point(15, 430);
            this.labelSaveStatus.Name = "labelSaveStatus";
            this.labelSaveStatus.Size = new System.Drawing.Size(0, 13);
            this.labelSaveStatus.TabIndex = 21;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.Label_SunriseSet);
            this.groupBox4.Controls.Add(this.checkBoxEnableNOAA);
            this.groupBox4.Controls.Add(this.textBox_Latitude);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.textBox_Longitude);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(8, 63);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(760, 47);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Automatic Sunrise and Sunsent Scene Activation";
            // 
            // Label_SunriseSet
            // 
            this.Label_SunriseSet.AutoSize = true;
            this.Label_SunriseSet.Location = new System.Drawing.Point(499, 28);
            this.Label_SunriseSet.Name = "Label_SunriseSet";
            this.Label_SunriseSet.Size = new System.Drawing.Size(10, 13);
            this.Label_SunriseSet.TabIndex = 17;
            this.Label_SunriseSet.Text = "-";
            // 
            // checkBoxEnableNOAA
            // 
            this.checkBoxEnableNOAA.AutoSize = true;
            this.checkBoxEnableNOAA.Location = new System.Drawing.Point(10, 20);
            this.checkBoxEnableNOAA.Name = "checkBoxEnableNOAA";
            this.checkBoxEnableNOAA.Size = new System.Drawing.Size(135, 17);
            this.checkBoxEnableNOAA.TabIndex = 7;
            this.checkBoxEnableNOAA.Text = "Enable Sunrise/Sunset";
            this.checkBoxEnableNOAA.UseVisualStyleBackColor = true;
            // 
            // textBox_Latitude
            // 
            this.textBox_Latitude.Location = new System.Drawing.Point(204, 18);
            this.textBox_Latitude.Name = "textBox_Latitude";
            this.textBox_Latitude.Size = new System.Drawing.Size(107, 20);
            this.textBox_Latitude.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 26);
            this.label4.TabIndex = 15;
            this.label4.Text = "Latitude:\r\n\r\n";
            this.toolTip1.SetToolTip(this.label4, "degrees,mins,seconds,direction{N or S]\r\n\r\nDEFAULT: 37,40,38,N");
            // 
            // textBox_Longitude
            // 
            this.textBox_Longitude.Location = new System.Drawing.Point(385, 18);
            this.textBox_Longitude.Name = "textBox_Longitude";
            this.textBox_Longitude.Size = new System.Drawing.Size(107, 20);
            this.textBox_Longitude.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(322, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Longitude:";
            this.toolTip1.SetToolTip(this.label2, "degrees,mins,seconds,direction{E or W]\r\n\r\nDEFAULT: 113,3,42,W\r\n\r\n\r\n");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxRepolling);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(8, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(342, 45);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "General Settings";
            // 
            // textBoxRepolling
            // 
            this.textBoxRepolling.Location = new System.Drawing.Point(229, 12);
            this.textBoxRepolling.Name = "textBoxRepolling";
            this.textBoxRepolling.Size = new System.Drawing.Size(107, 20);
            this.textBoxRepolling.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "ZWave Device Repolling Interval (seconds):";
            // 
            // buttonSaveSettings
            // 
            this.buttonSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveSettings.Location = new System.Drawing.Point(631, 405);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(137, 38);
            this.buttonSaveSettings.TabIndex = 7;
            this.buttonSaveSettings.Text = "Save Settings";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.checkBox_HideJabberPassword);
            this.groupBox7.Controls.Add(this.textBoxJabberUserTo);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Controls.Add(this.textBoxJabberServer);
            this.groupBox7.Controls.Add(this.label15);
            this.groupBox7.Controls.Add(this.textBoxJabberPassword);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.textBoxJabberUser);
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.checkBoxJabberVerbose);
            this.groupBox7.Controls.Add(this.checkBoxJabberEnabled);
            this.groupBox7.Location = new System.Drawing.Point(5, 272);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(763, 127);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Jabber/GTalk Interface";
            // 
            // checkBox_HideJabberPassword
            // 
            this.checkBox_HideJabberPassword.AutoSize = true;
            this.checkBox_HideJabberPassword.Checked = true;
            this.checkBox_HideJabberPassword.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_HideJabberPassword.Location = new System.Drawing.Point(590, 52);
            this.checkBox_HideJabberPassword.Name = "checkBox_HideJabberPassword";
            this.checkBox_HideJabberPassword.Size = new System.Drawing.Size(97, 17);
            this.checkBox_HideJabberPassword.TabIndex = 14;
            this.checkBox_HideJabberPassword.Text = "Hide Password";
            this.checkBox_HideJabberPassword.UseVisualStyleBackColor = true;
            this.checkBox_HideJabberPassword.CheckedChanged += new System.EventHandler(this.checkBox_HideJabberPassword_CheckedChanged);
            // 
            // textBoxJabberUserTo
            // 
            this.textBoxJabberUserTo.Location = new System.Drawing.Point(348, 99);
            this.textBoxJabberUserTo.Name = "textBoxJabberUserTo";
            this.textBoxJabberUserTo.Size = new System.Drawing.Size(409, 20);
            this.textBoxJabberUserTo.TabIndex = 20;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 103);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(338, 13);
            this.label16.TabIndex = 19;
            this.label16.Text = "Usernames of jabber users to send notifcations to (comma seperated): ";
            // 
            // textBoxJabberServer
            // 
            this.textBoxJabberServer.Location = new System.Drawing.Point(348, 73);
            this.textBoxJabberServer.Name = "textBoxJabberServer";
            this.textBoxJabberServer.Size = new System.Drawing.Size(236, 20);
            this.textBoxJabberServer.TabIndex = 18;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(269, 78);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(76, 13);
            this.label15.TabIndex = 17;
            this.label15.Text = "Jabber Server:";
            // 
            // textBoxJabberPassword
            // 
            this.textBoxJabberPassword.Location = new System.Drawing.Point(348, 47);
            this.textBoxJabberPassword.Name = "textBoxJabberPassword";
            this.textBoxJabberPassword.Size = new System.Drawing.Size(236, 20);
            this.textBoxJabberPassword.TabIndex = 16;
            this.textBoxJabberPassword.UseSystemPasswordChar = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(254, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "Jabber Password:";
            // 
            // textBoxJabberUser
            // 
            this.textBoxJabberUser.Location = new System.Drawing.Point(348, 21);
            this.textBoxJabberUser.Name = "textBoxJabberUser";
            this.textBoxJabberUser.Size = new System.Drawing.Size(236, 20);
            this.textBoxJabberUser.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(252, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Jabber Username:";
            // 
            // checkBoxJabberVerbose
            // 
            this.checkBoxJabberVerbose.AutoSize = true;
            this.checkBoxJabberVerbose.Location = new System.Drawing.Point(9, 42);
            this.checkBoxJabberVerbose.Name = "checkBoxJabberVerbose";
            this.checkBoxJabberVerbose.Size = new System.Drawing.Size(142, 17);
            this.checkBoxJabberVerbose.TabIndex = 13;
            this.checkBoxJabberVerbose.Text = "Enable Verbose Logging";
            this.checkBoxJabberVerbose.UseVisualStyleBackColor = true;
            // 
            // checkBoxJabberEnabled
            // 
            this.checkBoxJabberEnabled.AutoSize = true;
            this.checkBoxJabberEnabled.Location = new System.Drawing.Point(9, 19);
            this.checkBoxJabberEnabled.Name = "checkBoxJabberEnabled";
            this.checkBoxJabberEnabled.Size = new System.Drawing.Size(94, 17);
            this.checkBoxJabberEnabled.TabIndex = 13;
            this.checkBoxJabberEnabled.Text = "Enable Jabber";
            this.checkBoxJabberEnabled.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.checkBox_HideLSPassword);
            this.groupBox5.Controls.Add(this.checkBoxLSDAuth);
            this.groupBox5.Controls.Add(this.textBoxLSLimit);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.textBoxLSport);
            this.groupBox5.Controls.Add(this.textBoxLSPassword);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.checkBoxLSDebugVerbose);
            this.groupBox5.Controls.Add(this.checkBoxLSEnabled);
            this.groupBox5.Location = new System.Drawing.Point(5, 194);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(763, 72);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "LightSwitch Interface";
            // 
            // checkBox_HideLSPassword
            // 
            this.checkBox_HideLSPassword.AutoSize = true;
            this.checkBox_HideLSPassword.Checked = true;
            this.checkBox_HideLSPassword.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_HideLSPassword.Location = new System.Drawing.Point(660, 18);
            this.checkBox_HideLSPassword.Name = "checkBox_HideLSPassword";
            this.checkBox_HideLSPassword.Size = new System.Drawing.Size(97, 17);
            this.checkBox_HideLSPassword.TabIndex = 21;
            this.checkBox_HideLSPassword.Text = "Hide Password";
            this.checkBox_HideLSPassword.UseVisualStyleBackColor = true;
            this.checkBox_HideLSPassword.CheckedChanged += new System.EventHandler(this.checkBox_HideLSPassword_CheckedChanged);
            // 
            // checkBoxLSDAuth
            // 
            this.checkBoxLSDAuth.AutoSize = true;
            this.checkBoxLSDAuth.Location = new System.Drawing.Point(478, 40);
            this.checkBoxLSDAuth.Name = "checkBoxLSDAuth";
            this.checkBoxLSDAuth.Size = new System.Drawing.Size(132, 30);
            this.checkBoxLSDAuth.TabIndex = 13;
            this.checkBoxLSDAuth.Text = "Disable Authentication\r\n  (Not Recommened)";
            this.checkBoxLSDAuth.UseVisualStyleBackColor = true;
            // 
            // textBoxLSLimit
            // 
            this.textBoxLSLimit.Location = new System.Drawing.Point(272, 42);
            this.textBoxLSLimit.Name = "textBoxLSLimit";
            this.textBoxLSLimit.Size = new System.Drawing.Size(65, 20);
            this.textBoxLSLimit.TabIndex = 12;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(178, 46);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 13);
            this.label14.TabIndex = 11;
            this.label14.Text = "Connection Limit:";
            // 
            // textBoxLSport
            // 
            this.textBoxLSport.Location = new System.Drawing.Point(271, 16);
            this.textBoxLSport.Name = "textBoxLSport";
            this.textBoxLSport.Size = new System.Drawing.Size(66, 20);
            this.textBoxLSport.TabIndex = 10;
            // 
            // textBoxLSPassword
            // 
            this.textBoxLSPassword.Location = new System.Drawing.Point(478, 16);
            this.textBoxLSPassword.Name = "textBoxLSPassword";
            this.textBoxLSPassword.Size = new System.Drawing.Size(176, 20);
            this.textBoxLSPassword.TabIndex = 8;
            this.textBoxLSPassword.UseSystemPasswordChar = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(190, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(75, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Listen on Port:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(345, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(127, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Authentication Password:";
            // 
            // checkBoxLSDebugVerbose
            // 
            this.checkBoxLSDebugVerbose.AutoSize = true;
            this.checkBoxLSDebugVerbose.Location = new System.Drawing.Point(9, 46);
            this.checkBoxLSDebugVerbose.Name = "checkBoxLSDebugVerbose";
            this.checkBoxLSDebugVerbose.Size = new System.Drawing.Size(142, 17);
            this.checkBoxLSDebugVerbose.TabIndex = 8;
            this.checkBoxLSDebugVerbose.Text = "Enable Verbose Logging";
            this.checkBoxLSDebugVerbose.UseVisualStyleBackColor = true;
            // 
            // checkBoxLSEnabled
            // 
            this.checkBoxLSEnabled.AutoSize = true;
            this.checkBoxLSEnabled.Location = new System.Drawing.Point(9, 23);
            this.checkBoxLSEnabled.Name = "checkBoxLSEnabled";
            this.checkBoxLSEnabled.Size = new System.Drawing.Size(151, 17);
            this.checkBoxLSEnabled.TabIndex = 7;
            this.checkBoxLSEnabled.Text = "Enable LightSwitch Server";
            this.checkBoxLSEnabled.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.txtb_exampleURL);
            this.groupBox6.Controls.Add(this.checkBoxHTTPEnable);
            this.groupBox6.Controls.Add(this.txtb_httpPort);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Location = new System.Drawing.Point(8, 116);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(760, 72);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "HTTP Interface";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(133, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(110, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Example HTTP URL: ";
            // 
            // txtb_exampleURL
            // 
            this.txtb_exampleURL.Location = new System.Drawing.Point(249, 46);
            this.txtb_exampleURL.Name = "txtb_exampleURL";
            this.txtb_exampleURL.ReadOnly = true;
            this.txtb_exampleURL.Size = new System.Drawing.Size(505, 20);
            this.txtb_exampleURL.TabIndex = 5;
            // 
            // checkBoxHTTPEnable
            // 
            this.checkBoxHTTPEnable.AutoSize = true;
            this.checkBoxHTTPEnable.Location = new System.Drawing.Point(9, 29);
            this.checkBoxHTTPEnable.Name = "checkBoxHTTPEnable";
            this.checkBoxHTTPEnable.Size = new System.Drawing.Size(122, 17);
            this.checkBoxHTTPEnable.TabIndex = 5;
            this.checkBoxHTTPEnable.Text = "Enable HTTP Listen";
            this.checkBoxHTTPEnable.UseVisualStyleBackColor = true;
            // 
            // txtb_httpPort
            // 
            this.txtb_httpPort.Location = new System.Drawing.Point(249, 17);
            this.txtb_httpPort.Name = "txtb_httpPort";
            this.txtb_httpPort.Size = new System.Drawing.Size(62, 20);
            this.txtb_httpPort.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(168, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Listen on Port:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBoxLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(776, 463);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBoxLog
            // 
            this.listBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.Location = new System.Drawing.Point(3, 3);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(770, 432);
            this.listBoxLog.TabIndex = 1;
            // 
            // timer_TaskRunner
            // 
            this.timer_TaskRunner.Enabled = true;
            this.timer_TaskRunner.Interval = 1000;
            this.timer_TaskRunner.Tick += new System.EventHandler(this.timer_TaskRunner_Tick);
            // 
            // timerNOAA
            // 
            this.timerNOAA.Enabled = true;
            this.timerNOAA.Interval = 60000;
            this.timerNOAA.Tick += new System.EventHandler(this.timerNOAA_Tick);
            // 
            // contextMenuStripScenes
            // 
            this.contextMenuStripScenes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runSceneToolStripMenuItem,
            this.addSceneToolStripMenuItem,
            this.deleteSceneToolStripMenuItem,
            this.toolStripSeparator3,
            this.editSceneToolStripMenuItem});
            this.contextMenuStripScenes.Name = "contextMenuStripScenes";
            this.contextMenuStripScenes.Size = new System.Drawing.Size(162, 98);
            // 
            // runSceneToolStripMenuItem
            // 
            this.runSceneToolStripMenuItem.Name = "runSceneToolStripMenuItem";
            this.runSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.runSceneToolStripMenuItem.Text = "Run Scene Now";
            this.runSceneToolStripMenuItem.Click += new System.EventHandler(this.runSceneToolStripMenuItem_Click);
            // 
            // addSceneToolStripMenuItem
            // 
            this.addSceneToolStripMenuItem.Name = "addSceneToolStripMenuItem";
            this.addSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.addSceneToolStripMenuItem.Text = "Add Scene";
            this.addSceneToolStripMenuItem.Click += new System.EventHandler(this.addSceneToolStripMenuItem_Click);
            // 
            // deleteSceneToolStripMenuItem
            // 
            this.deleteSceneToolStripMenuItem.Name = "deleteSceneToolStripMenuItem";
            this.deleteSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.deleteSceneToolStripMenuItem.Text = "Delete Scene";
            this.deleteSceneToolStripMenuItem.Click += new System.EventHandler(this.deleteSceneToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(158, 6);
            // 
            // editSceneToolStripMenuItem
            // 
            this.editSceneToolStripMenuItem.Name = "editSceneToolStripMenuItem";
            this.editSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.editSceneToolStripMenuItem.Text = "Scene Properties";
            this.editSceneToolStripMenuItem.Click += new System.EventHandler(this.editSceneToolStripMenuItem_Click);
            // 
            // contextMenuStripScenesNull
            // 
            this.contextMenuStripScenesNull.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4});
            this.contextMenuStripScenesNull.Name = "contextMenuStripScenes";
            this.contextMenuStripScenesNull.Size = new System.Drawing.Size(131, 26);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(130, 22);
            this.toolStripMenuItem4.Text = "Add Scene";
            // 
            // contextMenuStripActions
            // 
            this.contextMenuStripActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editActionToolStripMenuItem,
            this.toolStripSeparator4,
            this.deleteActionToolStripMenuItem});
            this.contextMenuStripActions.Name = "contextMenuStripScenes";
            this.contextMenuStripActions.Size = new System.Drawing.Size(146, 54);
            this.contextMenuStripActions.Text = "test";
            // 
            // editActionToolStripMenuItem
            // 
            this.editActionToolStripMenuItem.Name = "editActionToolStripMenuItem";
            this.editActionToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.editActionToolStripMenuItem.Text = "Edit Action";
            this.editActionToolStripMenuItem.Click += new System.EventHandler(this.editActionToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(142, 6);
            // 
            // deleteActionToolStripMenuItem
            // 
            this.deleteActionToolStripMenuItem.Name = "deleteActionToolStripMenuItem";
            this.deleteActionToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.deleteActionToolStripMenuItem.Text = "Delete Action";
            this.deleteActionToolStripMenuItem.Click += new System.EventHandler(this.deleteActionToolStripMenuItem_Click);
            // 
            // contextMenuStripDevices
            // 
            this.contextMenuStripDevices.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adjustLevelToolStripMenuItem,
            this.toolStripSeparator1,
            this.manuallyRepollToolStripMenuItem,
            this.findNewDevicesToolStripMenuItem,
            this.reconnectToControlThinkUSBToolStripMenuItem1,
            this.toolStripSeparator5,
            this.devicePropertiesToolStripMenuItem});
            this.contextMenuStripDevices.Name = "contextMenuStripDevices";
            this.contextMenuStripDevices.Size = new System.Drawing.Size(242, 126);
            // 
            // adjustLevelToolStripMenuItem
            // 
            this.adjustLevelToolStripMenuItem.Name = "adjustLevelToolStripMenuItem";
            this.adjustLevelToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.adjustLevelToolStripMenuItem.Text = "Adjust Level / Create Action";
            this.adjustLevelToolStripMenuItem.Click += new System.EventHandler(this.adjustLevelToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(238, 6);
            // 
            // manuallyRepollToolStripMenuItem
            // 
            this.manuallyRepollToolStripMenuItem.Name = "manuallyRepollToolStripMenuItem";
            this.manuallyRepollToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.manuallyRepollToolStripMenuItem.Text = "Manually Repoll";
            this.manuallyRepollToolStripMenuItem.Click += new System.EventHandler(this.manuallyRepollToolStripMenuItem_Click);
            // 
            // findNewDevicesToolStripMenuItem
            // 
            this.findNewDevicesToolStripMenuItem.Name = "findNewDevicesToolStripMenuItem";
            this.findNewDevicesToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.findNewDevicesToolStripMenuItem.Text = "Find New Devices";
            this.findNewDevicesToolStripMenuItem.Click += new System.EventHandler(this.findNewDevicesToolStripMenuItem_Click);
            // 
            // reconnectToControlThinkUSBToolStripMenuItem1
            // 
            this.reconnectToControlThinkUSBToolStripMenuItem1.Name = "reconnectToControlThinkUSBToolStripMenuItem1";
            this.reconnectToControlThinkUSBToolStripMenuItem1.Size = new System.Drawing.Size(241, 22);
            this.reconnectToControlThinkUSBToolStripMenuItem1.Text = "Reconnect to ControlThink USB";
            this.reconnectToControlThinkUSBToolStripMenuItem1.Click += new System.EventHandler(this.reconnectToControlThinkUSBToolStripMenuItem1_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(238, 6);
            // 
            // devicePropertiesToolStripMenuItem
            // 
            this.devicePropertiesToolStripMenuItem.Name = "devicePropertiesToolStripMenuItem";
            this.devicePropertiesToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.devicePropertiesToolStripMenuItem.Text = "Device Properties";
            this.devicePropertiesToolStripMenuItem.Click += new System.EventHandler(this.devicePropertiesToolStripMenuItem_Click);
            // 
            // contextMenuStripTasks
            // 
            this.contextMenuStripTasks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStripTasks.Name = "contextMenuStripTasks";
            this.contextMenuStripTasks.Size = new System.Drawing.Size(151, 48);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.addToolStripMenuItem.Text = "Add New Task";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.deleteToolStripMenuItem.Text = "Delete Task";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // contextMenuStripTasksNull
            // 
            this.contextMenuStripTasksNull.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAddTaks});
            this.contextMenuStripTasksNull.Name = "contextMenuStripTasksNull";
            this.contextMenuStripTasksNull.Size = new System.Drawing.Size(151, 26);
            // 
            // toolStripAddTaks
            // 
            this.toolStripAddTaks.Name = "toolStripAddTaks";
            this.toolStripAddTaks.Size = new System.Drawing.Size(150, 22);
            this.toolStripAddTaks.Text = "Add New Task";
            this.toolStripAddTaks.Click += new System.EventHandler(this.toolStripAddTaks_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forceSaveToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // forceSaveToolStripMenuItem
            // 
            this.forceSaveToolStripMenuItem.Name = "forceSaveToolStripMenuItem";
            this.forceSaveToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.forceSaveToolStripMenuItem.Text = "Force Save";
            this.forceSaveToolStripMenuItem.Click += new System.EventHandler(this.forceSaveToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(127, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(130, 22);
            this.exitToolStripMenuItem1.Text = "Exit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // contextMenuStripDevicesNull
            // 
            this.contextMenuStripDevicesNull.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findNewDevicesToolStripMenuItem1,
            this.reconnectToControlThinkUSBToolStripMenuItem});
            this.contextMenuStripDevicesNull.Name = "contextMenuStripDevicesNull";
            this.contextMenuStripDevicesNull.Size = new System.Drawing.Size(242, 70);
            // 
            // findNewDevicesToolStripMenuItem1
            // 
            this.findNewDevicesToolStripMenuItem1.Name = "findNewDevicesToolStripMenuItem1";
            this.findNewDevicesToolStripMenuItem1.Size = new System.Drawing.Size(241, 22);
            this.findNewDevicesToolStripMenuItem1.Text = "Find New Devices";
            this.findNewDevicesToolStripMenuItem1.Click += new System.EventHandler(this.findNewDevicesToolStripMenuItem1_Click);
            // 
            // reconnectToControlThinkUSBToolStripMenuItem
            // 
            this.reconnectToControlThinkUSBToolStripMenuItem.Name = "reconnectToControlThinkUSBToolStripMenuItem";
            this.reconnectToControlThinkUSBToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.reconnectToControlThinkUSBToolStripMenuItem.Text = "Reconnect to ControlThink USB";
            this.reconnectToControlThinkUSBToolStripMenuItem.Click += new System.EventHandler(this.reconnectToControlThinkUSBToolStripMenuItem_Click);
            // 
            // formzVirtualScenes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 513);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 551);
            this.Name = "formzVirtualScenes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.zVirtualScenes_Load);
            this.MainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDevices)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewScenes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox_Weekly.ResumeLayout(false);
            this.groupBox_Weekly.PerformLayout();
            this.groupBoxDaily.ResumeLayout(false);
            this.groupBoxDaily.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.contextMenuStripScenes.ResumeLayout(false);
            this.contextMenuStripScenesNull.ResumeLayout(false);
            this.contextMenuStripActions.ResumeLayout(false);
            this.contextMenuStripDevices.ResumeLayout(false);
            this.contextMenuStripTasks.ResumeLayout(false);
            this.contextMenuStripTasksNull.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStripDevicesNull.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lbl_sceneActions;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtb_httpPort;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBoxHTTPEnable;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtb_exampleURL;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxLSPassword;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxLSDebugVerbose;
        private System.Windows.Forms.CheckBox checkBoxLSEnabled;
        private System.Windows.Forms.TextBox textBoxLSport;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxLSLimit;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btn_createnonzwaction;
        private System.Windows.Forms.ComboBox comboBoxNonZWAction;
        private System.Windows.Forms.Label labelLastEvent;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox textBoxJabberUserTo;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxJabberServer;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxJabberPassword;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxJabberUser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxJabberVerbose;
        private System.Windows.Forms.CheckBox checkBoxJabberEnabled;
        private System.Windows.Forms.Button buttonSaveSettings;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label labelSceneRunStatus;
        private System.Windows.Forms.Button btn_AddAction;
        private BrightIdeasSoftware.DataListView dataListViewDevices;
        private BrightIdeasSoftware.OLVColumn NodeCol;
        private BrightIdeasSoftware.OLVColumn NameCol;
        private BrightIdeasSoftware.OLVColumn GroupCol;
        private BrightIdeasSoftware.OLVColumn ModeCol;
        private BrightIdeasSoftware.OLVColumn LevelCol;
        private BrightIdeasSoftware.OLVColumn LevelTextCol;
        private BrightIdeasSoftware.OLVColumn FanModeCol;
        private BrightIdeasSoftware.OLVColumn SetPointCol;
        private BrightIdeasSoftware.OLVColumn currStateCol;
        private GroupBox groupBox3;
        private TextBox textBoxRepolling;
        private Label label1;
        private Timer timer_TaskRunner;
        private GroupBox groupBox4;
        private CheckBox checkBoxEnableNOAA;
        private TextBox textBox_Latitude;
        private Label label4;
        private TextBox textBox_Longitude;
        private Label label2;
        private Timer timerNOAA;
        private Label Label_SunriseSet;
        private CheckBox checkBoxLSDAuth;
        private Label labelSaveStatus;
        private CheckBox checkBox_HideJabberPassword;
        private CheckBox checkBox_HideLSPassword;
        private BrightIdeasSoftware.DataListView dataListViewActions;
        private BrightIdeasSoftware.OLVColumn ColName;
        private BrightIdeasSoftware.OLVColumn ColAction;
        private BrightIdeasSoftware.OLVColumn ColType;
        private ImageList imageListActionTypesSmall;
        private PictureBox pictureBox3;
        private BrightIdeasSoftware.DataListView dataListViewScenes;
        private BrightIdeasSoftware.OLVColumn SceneNamecol;
        private SplitContainer splitContainer1;
        private ContextMenuStrip contextMenuStripScenes;
        private ToolStripMenuItem editSceneToolStripMenuItem;
        private ToolStripMenuItem runSceneToolStripMenuItem;
        private ToolStripMenuItem deleteSceneToolStripMenuItem;
        private ToolStripMenuItem addSceneToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ContextMenuStrip contextMenuStripScenesNull;
        private ToolStripMenuItem toolStripMenuItem4;
        private ContextMenuStrip contextMenuStripActions;
        private ToolStripMenuItem editActionToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem deleteActionToolStripMenuItem;
        private ContextMenuStrip contextMenuStripDevices;
        private ToolStripMenuItem adjustLevelToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem devicePropertiesToolStripMenuItem;
        private ToolStripMenuItem manuallyRepollToolStripMenuItem;
        private ToolStripMenuItem findNewDevicesToolStripMenuItem;
        private TabPage tabPage4;
        private GroupBox groupBox1;
        private PictureBox pictureBox1;
        private Label label5;
        private Label label6;
        private TextBox textBox_TaskName;
        private Button button_SaveTask;
        private ComboBox comboBox_ActionsTask;
        private ComboBox comboBox_FrequencyTask;
        private GroupBox groupBox_Weekly;
        private CheckBox checkBox_RecurSunday;
        private CheckBox checkBox_RecurSaturday;
        private CheckBox checkBox_RecurFriday;
        private CheckBox checkBox_RecurThursday;
        private CheckBox checkBox_RecurWednesday;
        private CheckBox checkBox_RecurTuesday;
        private CheckBox checkBox_RecurMonday;
        private Label label7;
        private Label label12;
        private TextBox textBox_RecurWeeks;
        private Label label17;
        private DateTimePicker dateTimePickerStartTask;
        private Label label18;
        private CheckBox checkBox_EnabledTask;
        private GroupBox groupBoxDaily;
        private Label label19;
        private Label label20;
        private TextBox textBox_DaysRecur;
        private BrightIdeasSoftware.DataListView dataListTasks;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn EnabledCol;
        private BrightIdeasSoftware.OLVColumn FreqCol;
        private ContextMenuStrip contextMenuStripTasks;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ContextMenuStrip contextMenuStripTasksNull;
        private ToolStripMenuItem toolStripAddTaks;
        private ToolStripMenuItem reconnectToControlThinkUSBToolStripMenuItem1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem forceSaveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator1;
        private ContextMenuStrip contextMenuStripDevicesNull;
        private ToolStripMenuItem findNewDevicesToolStripMenuItem1;
        private ToolStripMenuItem reconnectToControlThinkUSBToolStripMenuItem;
    }
}

