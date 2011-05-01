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
            this.imageListActionTypesSmall = new System.Windows.Forms.ImageList(this.components);
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
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.devicePropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTasks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTasksNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripAddTaks = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupsZonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripDevicesNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.findNewDevicesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.repollAllDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataListViewLog = new BrightIdeasSoftware.DataListView();
            this.dateTimeCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.urgencyColu = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.InterfaceCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.descCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataListTasks = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.EnabledCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.FreqCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxDaily = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox_DaysRecur = new System.Windows.Forms.TextBox();
            this.textBox_TaskName = new System.Windows.Forms.TextBox();
            this.checkBox_EnabledTask = new System.Windows.Forms.CheckBox();
            this.button_SaveTask = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBox_ActionsTask = new System.Windows.Forms.ComboBox();
            this.dateTimePickerStartTask = new System.Windows.Forms.DateTimePicker();
            this.comboBox_FrequencyTask = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
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
            this.label_devicecount = new System.Windows.Forms.Label();
            this.labelLastEvent = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.dataListViewScenes = new BrightIdeasSoftware.DataListView();
            this.SceneNamecol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.lbl_sceneActions = new System.Windows.Forms.Label();
            this.dataListViewActions = new BrightIdeasSoftware.DataListView();
            this.ColType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColAction = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btn_createnonzwaction = new System.Windows.Forms.Button();
            this.btn_AddAction = new System.Windows.Forms.Button();
            this.comboBoxNonZWAction = new System.Windows.Forms.ComboBox();
            this.labelSceneRunStatus = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.contextMenuStripScenes.SuspendLayout();
            this.contextMenuStripScenesNull.SuspendLayout();
            this.contextMenuStripActions.SuspendLayout();
            this.contextMenuStripDevices.SuspendLayout();
            this.contextMenuStripTasks.SuspendLayout();
            this.contextMenuStripTasksNull.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStripDevicesNull.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewLog)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxDaily.SuspendLayout();
            this.groupBox_Weekly.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDevices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewScenes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.MainTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListActionTypesSmall
            // 
            this.imageListActionTypesSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListActionTypesSmall.ImageStream")));
            this.imageListActionTypesSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListActionTypesSmall.Images.SetKeyName(0, "20delay.png");
            this.imageListActionTypesSmall.Images.SetKeyName(1, "20exe.png");
            this.imageListActionTypesSmall.Images.SetKeyName(2, "20zwave-thermostat.png");
            this.imageListActionTypesSmall.Images.SetKeyName(3, "20scene_icon.jpg");
            this.imageListActionTypesSmall.Images.SetKeyName(4, "20dimmer.png");
            this.imageListActionTypesSmall.Images.SetKeyName(5, "20event.png");
            this.imageListActionTypesSmall.Images.SetKeyName(6, "20radio2.png");
            this.imageListActionTypesSmall.Images.SetKeyName(7, "20switch.png");
            this.imageListActionTypesSmall.Images.SetKeyName(8, "chart_bar.png");
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
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
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
            this.toolStripSeparator5,
            this.devicePropertiesToolStripMenuItem});
            this.contextMenuStripDevices.Name = "contextMenuStripDevices";
            this.contextMenuStripDevices.Size = new System.Drawing.Size(318, 104);
            // 
            // adjustLevelToolStripMenuItem
            // 
            this.adjustLevelToolStripMenuItem.Name = "adjustLevelToolStripMenuItem";
            this.adjustLevelToolStripMenuItem.Size = new System.Drawing.Size(317, 22);
            this.adjustLevelToolStripMenuItem.Text = "Adjust Level / Create Action";
            this.adjustLevelToolStripMenuItem.Click += new System.EventHandler(this.adjustLevelToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(314, 6);
            // 
            // manuallyRepollToolStripMenuItem
            // 
            this.manuallyRepollToolStripMenuItem.Name = "manuallyRepollToolStripMenuItem";
            this.manuallyRepollToolStripMenuItem.Size = new System.Drawing.Size(317, 22);
            this.manuallyRepollToolStripMenuItem.Text = "Repoll This Device";
            this.manuallyRepollToolStripMenuItem.Click += new System.EventHandler(this.manuallyRepollToolStripMenuItem_Click);
            // 
            // findNewDevicesToolStripMenuItem
            // 
            this.findNewDevicesToolStripMenuItem.Name = "findNewDevicesToolStripMenuItem";
            this.findNewDevicesToolStripMenuItem.Size = new System.Drawing.Size(317, 22);
            this.findNewDevicesToolStripMenuItem.Text = "Reload ZWave Device List from USB Controller";
            this.findNewDevicesToolStripMenuItem.Click += new System.EventHandler(this.findNewDevicesToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(314, 6);
            // 
            // devicePropertiesToolStripMenuItem
            // 
            this.devicePropertiesToolStripMenuItem.Name = "devicePropertiesToolStripMenuItem";
            this.devicePropertiesToolStripMenuItem.Size = new System.Drawing.Size(317, 22);
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
            this.fileToolStripMenuItem,
            this.groupsZonesToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveSettingsToolStripMenuItem,
            this.forceSaveToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.saveSettingsToolStripMenuItem.Text = "&Save Settings";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // forceSaveToolStripMenuItem
            // 
            this.forceSaveToolStripMenuItem.Name = "forceSaveToolStripMenuItem";
            this.forceSaveToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.forceSaveToolStripMenuItem.Text = "&Export Settings...";
            this.forceSaveToolStripMenuItem.Click += new System.EventHandler(this.forceSaveToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(180, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(183, 22);
            this.exitToolStripMenuItem1.Text = "E&xit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // groupsZonesToolStripMenuItem
            // 
            this.groupsZonesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activateToolStripMenuItem});
            this.groupsZonesToolStripMenuItem.Name = "groupsZonesToolStripMenuItem";
            this.groupsZonesToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.groupsZonesToolStripMenuItem.Text = "&Groups / Zones";
            // 
            // activateToolStripMenuItem
            // 
            this.activateToolStripMenuItem.Name = "activateToolStripMenuItem";
            this.activateToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.activateToolStripMenuItem.Text = "Activate...";
            this.activateToolStripMenuItem.Click += new System.EventHandler(this.activateToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.settingsToolStripMenuItem.Text = "Settings...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // contextMenuStripDevicesNull
            // 
            this.contextMenuStripDevicesNull.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findNewDevicesToolStripMenuItem1,
            this.repollAllDevicesToolStripMenuItem});
            this.contextMenuStripDevicesNull.Name = "contextMenuStripDevicesNull";
            this.contextMenuStripDevicesNull.Size = new System.Drawing.Size(318, 48);
            // 
            // findNewDevicesToolStripMenuItem1
            // 
            this.findNewDevicesToolStripMenuItem1.Name = "findNewDevicesToolStripMenuItem1";
            this.findNewDevicesToolStripMenuItem1.Size = new System.Drawing.Size(317, 22);
            this.findNewDevicesToolStripMenuItem1.Text = "Reload ZWave Device List from USB Controller";
            this.findNewDevicesToolStripMenuItem1.Click += new System.EventHandler(this.findNewDevicesToolStripMenuItem1_Click);
            // 
            // repollAllDevicesToolStripMenuItem
            // 
            this.repollAllDevicesToolStripMenuItem.Name = "repollAllDevicesToolStripMenuItem";
            this.repollAllDevicesToolStripMenuItem.Size = new System.Drawing.Size(317, 22);
            this.repollAllDevicesToolStripMenuItem.Text = "Repoll All Devices";
            this.repollAllDevicesToolStripMenuItem.Click += new System.EventHandler(this.repollAllDevicesToolStripMenuItem_Click_1);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataListViewLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(776, 463);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataListViewLog
            // 
            this.dataListViewLog.AllColumns.Add(this.dateTimeCol);
            this.dataListViewLog.AllColumns.Add(this.urgencyColu);
            this.dataListViewLog.AllColumns.Add(this.InterfaceCol);
            this.dataListViewLog.AllColumns.Add(this.descCol);
            this.dataListViewLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataListViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dateTimeCol,
            this.urgencyColu,
            this.InterfaceCol,
            this.descCol});
            this.dataListViewLog.DataSource = null;
            this.dataListViewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewLog.FullRowSelect = true;
            this.dataListViewLog.HasCollapsibleGroups = false;
            this.dataListViewLog.HeaderMaximumHeight = 15;
            this.dataListViewLog.HideSelection = false;
            this.dataListViewLog.IsSimpleDragSource = true;
            this.dataListViewLog.Location = new System.Drawing.Point(3, 3);
            this.dataListViewLog.Name = "dataListViewLog";
            this.dataListViewLog.OwnerDraw = true;
            this.dataListViewLog.ShowCommandMenuOnRightClick = true;
            this.dataListViewLog.ShowGroups = false;
            this.dataListViewLog.Size = new System.Drawing.Size(770, 457);
            this.dataListViewLog.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewLog.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.dataListViewLog.TabIndex = 31;
            this.dataListViewLog.UseCompatibleStateImageBehavior = false;
            this.dataListViewLog.View = System.Windows.Forms.View.Details;
            // 
            // dateTimeCol
            // 
            this.dateTimeCol.AspectName = "datetime";
            this.dateTimeCol.Text = "Date & Time";
            this.dateTimeCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.datetimeWidth;
            // 
            // urgencyColu
            // 
            this.urgencyColu.AspectName = "urgency";
            this.urgencyColu.Text = "Urgency";
            this.urgencyColu.Width = global::zVirtualScenesApplication.Properties.Settings.Default.UrgencyWidth;
            // 
            // InterfaceCol
            // 
            this.InterfaceCol.AspectName = "InterfaceName";
            this.InterfaceCol.Text = "Interface";
            this.InterfaceCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.interfaceWidth;
            // 
            // descCol
            // 
            this.descCol.AspectName = "description";
            this.descCol.FillsFreeSpace = true;
            this.descCol.Text = "Description";
            this.descCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.descWidth;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(776, 463);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Scheduling";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::zVirtualScenesApplication.Properties.Settings.Default, "SpiltContainer2Distance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataListTasks);
            this.splitContainer2.Panel1MinSize = 100;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2.Controls.Add(this.label6);
            this.splitContainer2.Panel2.Controls.Add(this.groupBoxDaily);
            this.splitContainer2.Panel2.Controls.Add(this.textBox_TaskName);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox_EnabledTask);
            this.splitContainer2.Panel2.Controls.Add(this.button_SaveTask);
            this.splitContainer2.Panel2.Controls.Add(this.label18);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_ActionsTask);
            this.splitContainer2.Panel2.Controls.Add(this.dateTimePickerStartTask);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_FrequencyTask);
            this.splitContainer2.Panel2.Controls.Add(this.label17);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox_Weekly);
            this.splitContainer2.Panel2MinSize = 325;
            this.splitContainer2.Size = new System.Drawing.Size(770, 457);
            this.splitContainer2.SplitterDistance = global::zVirtualScenesApplication.Properties.Settings.Default.SpiltContainer2Distance;
            this.splitContainer2.TabIndex = 47;
            // 
            // dataListTasks
            // 
            this.dataListTasks.AllColumns.Add(this.olvColumn1);
            this.dataListTasks.AllColumns.Add(this.EnabledCol);
            this.dataListTasks.AllColumns.Add(this.FreqCol);
            this.dataListTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataListTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.EnabledCol,
            this.FreqCol});
            this.dataListTasks.DataSource = null;
            this.dataListTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListTasks.FullRowSelect = true;
            this.dataListTasks.HasCollapsibleGroups = false;
            this.dataListTasks.HeaderMaximumHeight = 15;
            this.dataListTasks.HideSelection = false;
            this.dataListTasks.Location = new System.Drawing.Point(0, 0);
            this.dataListTasks.Name = "dataListTasks";
            this.dataListTasks.OwnerDraw = true;
            this.dataListTasks.ShowCommandMenuOnRightClick = true;
            this.dataListTasks.ShowGroups = false;
            this.dataListTasks.Size = new System.Drawing.Size(371, 453);
            this.dataListTasks.TabIndex = 46;
            this.dataListTasks.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.InactiveCaption;
            this.dataListTasks.UseCompatibleStateImageBehavior = false;
            this.dataListTasks.View = System.Windows.Forms.View.Details;
            this.dataListTasks.SelectedIndexChanged += new System.EventHandler(this.dataListTasks_SelectedIndexChanged_1);
            this.dataListTasks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataListTasks_KeyDown);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "GetName";
            this.olvColumn1.ImageAspectName = "GetIcon";
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Text = "Task Name";
            this.olvColumn1.Width = global::zVirtualScenesApplication.Properties.Settings.Default.taskWidth;
            // 
            // EnabledCol
            // 
            this.EnabledCol.AspectName = "isEnabled";
            this.EnabledCol.IsEditable = false;
            this.EnabledCol.Text = "Enabled";
            this.EnabledCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.enabledWidth;
            // 
            // FreqCol
            // 
            this.FreqCol.AspectName = "FrequencyString";
            this.FreqCol.ImageAspectName = "";
            this.FreqCol.Text = "Frequency";
            this.FreqCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.freqWidth;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(51, 49);
            this.pictureBox1.TabIndex = 44;
            this.pictureBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(61, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Task Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Activate Scene:";
            // 
            // groupBoxDaily
            // 
            this.groupBoxDaily.Controls.Add(this.label19);
            this.groupBoxDaily.Controls.Add(this.label20);
            this.groupBoxDaily.Controls.Add(this.textBox_DaysRecur);
            this.groupBoxDaily.Location = new System.Drawing.Point(59, 154);
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
            // textBox_TaskName
            // 
            this.textBox_TaskName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TaskName.Location = new System.Drawing.Point(132, 16);
            this.textBox_TaskName.Name = "textBox_TaskName";
            this.textBox_TaskName.Size = new System.Drawing.Size(247, 20);
            this.textBox_TaskName.TabIndex = 0;
            // 
            // checkBox_EnabledTask
            // 
            this.checkBox_EnabledTask.AutoSize = true;
            this.checkBox_EnabledTask.Location = new System.Drawing.Point(62, 131);
            this.checkBox_EnabledTask.Name = "checkBox_EnabledTask";
            this.checkBox_EnabledTask.Size = new System.Drawing.Size(65, 17);
            this.checkBox_EnabledTask.TabIndex = 39;
            this.checkBox_EnabledTask.Text = "Enabled";
            this.checkBox_EnabledTask.UseVisualStyleBackColor = true;
            // 
            // button_SaveTask
            // 
            this.button_SaveTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveTask.Location = new System.Drawing.Point(287, 425);
            this.button_SaveTask.Name = "button_SaveTask";
            this.button_SaveTask.Size = new System.Drawing.Size(97, 25);
            this.button_SaveTask.TabIndex = 38;
            this.button_SaveTask.Text = "Save";
            this.button_SaveTask.UseVisualStyleBackColor = true;
            this.button_SaveTask.Click += new System.EventHandler(this.button_SaveTask_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(24, 108);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 13);
            this.label18.TabIndex = 36;
            this.label18.Text = "Start:";
            // 
            // comboBox_ActionsTask
            // 
            this.comboBox_ActionsTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_ActionsTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ActionsTask.FormattingEnabled = true;
            this.comboBox_ActionsTask.Location = new System.Drawing.Point(102, 72);
            this.comboBox_ActionsTask.Name = "comboBox_ActionsTask";
            this.comboBox_ActionsTask.Size = new System.Drawing.Size(277, 21);
            this.comboBox_ActionsTask.TabIndex = 42;
            // 
            // dateTimePickerStartTask
            // 
            this.dateTimePickerStartTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartTask.CustomFormat = "dddd,MMMM d, yyyy \'at\' h:mm:ss tt";
            this.dateTimePickerStartTask.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStartTask.Location = new System.Drawing.Point(62, 105);
            this.dateTimePickerStartTask.Name = "dateTimePickerStartTask";
            this.dateTimePickerStartTask.Size = new System.Drawing.Size(317, 20);
            this.dateTimePickerStartTask.TabIndex = 35;
            // 
            // comboBox_FrequencyTask
            // 
            this.comboBox_FrequencyTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_FrequencyTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_FrequencyTask.FormattingEnabled = true;
            this.comboBox_FrequencyTask.Location = new System.Drawing.Point(152, 42);
            this.comboBox_FrequencyTask.Name = "comboBox_FrequencyTask";
            this.comboBox_FrequencyTask.Size = new System.Drawing.Size(227, 21);
            this.comboBox_FrequencyTask.TabIndex = 33;
            this.comboBox_FrequencyTask.SelectedIndexChanged += new System.EventHandler(this.comboBox_FrequencyTask_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(59, 45);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(87, 13);
            this.label17.TabIndex = 34;
            this.label17.Text = "Task Frequency:";
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
            this.groupBox_Weekly.Location = new System.Drawing.Point(59, 154);
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
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.pictureBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(776, 463);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ZWave Devices / Scenes";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::zVirtualScenesApplication.Properties.Settings.Default, "SpiltContainer1Distance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer4);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Panel1MinSize = 75;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(770, 457);
            this.splitContainer1.SplitterDistance = global::zVirtualScenesApplication.Properties.Settings.Default.SpiltContainer1Distance;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 33;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.dataListViewDevices);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.label_devicecount);
            this.splitContainer4.Panel2.Controls.Add(this.labelLastEvent);
            this.splitContainer4.Size = new System.Drawing.Size(768, 198);
            this.splitContainer4.SplitterDistance = 168;
            this.splitContainer4.TabIndex = 32;
            // 
            // dataListViewDevices
            // 
            this.dataListViewDevices.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.dataListViewDevices.AllColumns.Add(this.NodeCol);
            this.dataListViewDevices.AllColumns.Add(this.NameCol);
            this.dataListViewDevices.AllColumns.Add(this.LevelCol);
            this.dataListViewDevices.AllColumns.Add(this.LevelTextCol);
            this.dataListViewDevices.AllColumns.Add(this.ModeCol);
            this.dataListViewDevices.AllColumns.Add(this.FanModeCol);
            this.dataListViewDevices.AllColumns.Add(this.SetPointCol);
            this.dataListViewDevices.AllColumns.Add(this.currStateCol);
            this.dataListViewDevices.AllColumns.Add(this.GroupCol);
            this.dataListViewDevices.BorderStyle = System.Windows.Forms.BorderStyle.None;
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
            this.dataListViewDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewDevices.EmptyListMsg = "Devices Loading...";
            this.dataListViewDevices.EmptyListMsgFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListViewDevices.FullRowSelect = true;
            this.dataListViewDevices.HasCollapsibleGroups = false;
            this.dataListViewDevices.HeaderMaximumHeight = 15;
            this.dataListViewDevices.HideSelection = false;
            this.dataListViewDevices.IsSimpleDragSource = true;
            this.dataListViewDevices.Location = new System.Drawing.Point(0, 0);
            this.dataListViewDevices.Name = "dataListViewDevices";
            this.dataListViewDevices.OwnerDraw = true;
            this.dataListViewDevices.ShowCommandMenuOnRightClick = true;
            this.dataListViewDevices.ShowGroups = false;
            this.dataListViewDevices.Size = new System.Drawing.Size(768, 168);
            this.dataListViewDevices.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewDevices.TabIndex = 30;
            this.dataListViewDevices.UseCompatibleStateImageBehavior = false;
            this.dataListViewDevices.View = System.Windows.Forms.View.Details;
            this.dataListViewDevices.ItemsChanging += new System.EventHandler<BrightIdeasSoftware.ItemsChangingEventArgs>(this.dataListViewDevices_ItemsChanging);
            this.dataListViewDevices.DoubleClick += new System.EventHandler(this.dataListViewDevices_DoubleClick);
            // 
            // NodeCol
            // 
            this.NodeCol.AspectName = "NodeID";
            this.NodeCol.ImageAspectName = "DeviceIcon";
            this.NodeCol.IsEditable = false;
            this.NodeCol.Text = "ID";
            this.NodeCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.NodeWidth;
            // 
            // NameCol
            // 
            this.NameCol.AspectName = "Name";
            this.NameCol.IsEditable = false;
            this.NameCol.Text = "Name";
            this.NameCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.nameWidth;
            // 
            // LevelCol
            // 
            this.LevelCol.AspectName = "GetLevelMeter";
            this.LevelCol.IsEditable = false;
            this.LevelCol.Text = "Level";
            this.LevelCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.levelWidth;
            // 
            // LevelTextCol
            // 
            this.LevelTextCol.AspectName = "GetLevelText";
            this.LevelTextCol.IsEditable = false;
            this.LevelTextCol.Text = "";
            this.LevelTextCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.levelmeterWidth;
            // 
            // ModeCol
            // 
            this.ModeCol.AspectName = "GetMode";
            this.ModeCol.IsEditable = false;
            this.ModeCol.Text = "Mode";
            this.ModeCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.modeWidth;
            // 
            // FanModeCol
            // 
            this.FanModeCol.AspectName = "GetFanMode";
            this.FanModeCol.IsEditable = false;
            this.FanModeCol.Text = "Fan Mode";
            this.FanModeCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.fanmodeWidth;
            // 
            // SetPointCol
            // 
            this.SetPointCol.AspectName = "GetSetPoint";
            this.SetPointCol.IsEditable = false;
            this.SetPointCol.Text = "Set Point";
            this.SetPointCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.SetPointColWidth;
            // 
            // currStateCol
            // 
            this.currStateCol.AspectName = "GetCurrentState";
            this.currStateCol.IsEditable = false;
            this.currStateCol.Text = "Currently";
            this.currStateCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.currStateWidth;
            // 
            // GroupCol
            // 
            this.GroupCol.AspectName = "GroupName";
            this.GroupCol.IsEditable = false;
            this.GroupCol.Text = "Groups / Zones";
            this.GroupCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.GroupColWidth;
            // 
            // label_devicecount
            // 
            this.label_devicecount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label_devicecount.AutoSize = true;
            this.label_devicecount.Location = new System.Drawing.Point(709, 7);
            this.label_devicecount.Name = "label_devicecount";
            this.label_devicecount.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_devicecount.Size = new System.Drawing.Size(55, 13);
            this.label_devicecount.TabIndex = 31;
            this.label_devicecount.Text = "0 Devices";
            // 
            // labelLastEvent
            // 
            this.labelLastEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLastEvent.AutoSize = true;
            this.labelLastEvent.Location = new System.Drawing.Point(6, 6);
            this.labelLastEvent.Name = "labelLastEvent";
            this.labelLastEvent.Size = new System.Drawing.Size(64, 13);
            this.labelLastEvent.TabIndex = 22;
            this.labelLastEvent.Text = "Last Event: ";
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::zVirtualScenesApplication.Properties.Settings.Default, "SpiltContainer3Distance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dataListViewScenes);
            this.splitContainer3.Panel1MinSize = 75;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.lbl_sceneActions);
            this.splitContainer3.Panel2.Controls.Add(this.dataListViewActions);
            this.splitContainer3.Panel2.Controls.Add(this.btn_createnonzwaction);
            this.splitContainer3.Panel2.Controls.Add(this.btn_AddAction);
            this.splitContainer3.Panel2.Controls.Add(this.comboBoxNonZWAction);
            this.splitContainer3.Panel2.Controls.Add(this.labelSceneRunStatus);
            this.splitContainer3.Panel2MinSize = 500;
            this.splitContainer3.Size = new System.Drawing.Size(770, 256);
            this.splitContainer3.SplitterDistance = global::zVirtualScenesApplication.Properties.Settings.Default.SpiltContainer3Distance;
            this.splitContainer3.TabIndex = 33;
            // 
            // dataListViewScenes
            // 
            this.dataListViewScenes.AllColumns.Add(this.SceneNamecol);
            this.dataListViewScenes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataListViewScenes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SceneNamecol});
            this.dataListViewScenes.DataSource = null;
            this.dataListViewScenes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewScenes.FullRowSelect = true;
            this.dataListViewScenes.HeaderMaximumHeight = 15;
            this.dataListViewScenes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListViewScenes.HideSelection = false;
            this.dataListViewScenes.Location = new System.Drawing.Point(0, 0);
            this.dataListViewScenes.MultiSelect = false;
            this.dataListViewScenes.Name = "dataListViewScenes";
            this.dataListViewScenes.OwnerDraw = true;
            this.dataListViewScenes.ShowGroups = false;
            this.dataListViewScenes.Size = new System.Drawing.Size(185, 254);
            this.dataListViewScenes.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewScenes.TabIndex = 32;
            this.dataListViewScenes.UnfocusedHighlightBackgroundColor = System.Drawing.Color.SkyBlue;
            this.dataListViewScenes.UseCompatibleStateImageBehavior = false;
            this.dataListViewScenes.UseExplorerTheme = true;
            this.dataListViewScenes.View = System.Windows.Forms.View.Details;
            this.dataListViewScenes.SelectedIndexChanged += new System.EventHandler(this.dataListViewScenes_SelectedIndexChanged);
            this.dataListViewScenes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataListViewScenes_KeyDown);
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
            // lbl_sceneActions
            // 
            this.lbl_sceneActions.AutoSize = true;
            this.lbl_sceneActions.Location = new System.Drawing.Point(3, 2);
            this.lbl_sceneActions.Name = "lbl_sceneActions";
            this.lbl_sceneActions.Size = new System.Drawing.Size(76, 13);
            this.lbl_sceneActions.TabIndex = 6;
            this.lbl_sceneActions.Text = "Scene Actions";
            // 
            // dataListViewActions
            // 
            this.dataListViewActions.AllColumns.Add(this.ColType);
            this.dataListViewActions.AllColumns.Add(this.ColName);
            this.dataListViewActions.AllColumns.Add(this.ColAction);
            this.dataListViewActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListViewActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataListViewActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColType,
            this.ColName,
            this.ColAction});
            this.dataListViewActions.DataSource = null;
            this.dataListViewActions.FullRowSelect = true;
            this.dataListViewActions.HeaderMaximumHeight = 15;
            this.dataListViewActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListViewActions.HideSelection = false;
            this.dataListViewActions.Location = new System.Drawing.Point(3, 18);
            this.dataListViewActions.Name = "dataListViewActions";
            this.dataListViewActions.OwnerDraw = true;
            this.dataListViewActions.ShowGroups = false;
            this.dataListViewActions.Size = new System.Drawing.Size(571, 192);
            this.dataListViewActions.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewActions.TabIndex = 31;
            this.dataListViewActions.UseCompatibleStateImageBehavior = false;
            this.dataListViewActions.UseExplorerTheme = true;
            this.dataListViewActions.View = System.Windows.Forms.View.Details;
            this.dataListViewActions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataListViewActions_KeyDown);
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
            this.ColName.Width = global::zVirtualScenesApplication.Properties.Settings.Default.ColNameWidth;
            // 
            // ColAction
            // 
            this.ColAction.AspectName = "ActionToString";
            this.ColAction.Text = "Action(s)";
            this.ColAction.Width = global::zVirtualScenesApplication.Properties.Settings.Default.ColActionwidth;
            // 
            // btn_createnonzwaction
            // 
            this.btn_createnonzwaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_createnonzwaction.Location = new System.Drawing.Point(2, 213);
            this.btn_createnonzwaction.Name = "btn_createnonzwaction";
            this.btn_createnonzwaction.Size = new System.Drawing.Size(131, 20);
            this.btn_createnonzwaction.TabIndex = 21;
            this.btn_createnonzwaction.Text = "Add Non-Zwave Action";
            this.btn_createnonzwaction.UseVisualStyleBackColor = true;
            this.btn_createnonzwaction.Click += new System.EventHandler(this.btn_createnonzwaction_Click);
            // 
            // btn_AddAction
            // 
            this.btn_AddAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_AddAction.Location = new System.Drawing.Point(441, 213);
            this.btn_AddAction.Name = "btn_AddAction";
            this.btn_AddAction.Size = new System.Drawing.Size(132, 20);
            this.btn_AddAction.TabIndex = 25;
            this.btn_AddAction.Text = "Add ZWave Action";
            this.btn_AddAction.UseVisualStyleBackColor = true;
            this.btn_AddAction.Click += new System.EventHandler(this.btn_AddAction_Click);
            // 
            // comboBoxNonZWAction
            // 
            this.comboBoxNonZWAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxNonZWAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNonZWAction.FormattingEnabled = true;
            this.comboBoxNonZWAction.Items.AddRange(new object[] {
            "Create Delay Timer",
            "Launch EXE"});
            this.comboBoxNonZWAction.Location = new System.Drawing.Point(139, 214);
            this.comboBoxNonZWAction.Name = "comboBoxNonZWAction";
            this.comboBoxNonZWAction.Size = new System.Drawing.Size(144, 21);
            this.comboBoxNonZWAction.TabIndex = 23;
            // 
            // labelSceneRunStatus
            // 
            this.labelSceneRunStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSceneRunStatus.AutoSize = true;
            this.labelSceneRunStatus.Location = new System.Drawing.Point(3, 238);
            this.labelSceneRunStatus.Name = "labelSceneRunStatus";
            this.labelSceneRunStatus.Size = new System.Drawing.Size(158, 13);
            this.labelSceneRunStatus.TabIndex = 23;
            this.labelSceneRunStatus.Text = "Last Scene Completetion Status";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(21, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(21, 22);
            this.pictureBox3.TabIndex = 32;
            this.pictureBox3.TabStop = false;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Controls.Add(this.tabPage4);
            this.MainTabControl.Controls.Add(this.tabPage2);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 24);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(784, 489);
            this.MainTabControl.TabIndex = 0;
            // 
            // formzVirtualScenes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 513);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.menuStrip1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zVirtualScenesApplication.Properties.Settings.Default, "MainFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::zVirtualScenesApplication.Properties.Settings.Default.MainFormLocation;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 551);
            this.Name = "formzVirtualScenes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.zVirtualScenes_Load);
            this.contextMenuStripScenes.ResumeLayout(false);
            this.contextMenuStripScenesNull.ResumeLayout(false);
            this.contextMenuStripActions.ResumeLayout(false);
            this.contextMenuStripDevices.ResumeLayout(false);
            this.contextMenuStripTasks.ResumeLayout(false);
            this.contextMenuStripTasksNull.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStripDevicesNull.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewLog)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxDaily.ResumeLayout(false);
            this.groupBoxDaily.PerformLayout();
            this.groupBox_Weekly.ResumeLayout(false);
            this.groupBox_Weekly.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDevices)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewScenes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.MainTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private Timer timer_TaskRunner;
        private Timer timerNOAA;
        private ImageList imageListActionTypesSmall;
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
        private ContextMenuStrip contextMenuStripTasks;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ContextMenuStrip contextMenuStripTasksNull;
        private ToolStripMenuItem toolStripAddTaks;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem forceSaveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator1;
        private ContextMenuStrip contextMenuStripDevicesNull;
        private ToolStripMenuItem findNewDevicesToolStripMenuItem1;
        private ToolStripMenuItem repollAllDevicesToolStripMenuItem;
        private ToolStripMenuItem groupsZonesToolStripMenuItem;
        private ToolStripMenuItem activateToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private TabPage tabPage2;
        private BrightIdeasSoftware.DataListView dataListViewLog;
        private BrightIdeasSoftware.OLVColumn dateTimeCol;
        private BrightIdeasSoftware.OLVColumn urgencyColu;
        private BrightIdeasSoftware.OLVColumn InterfaceCol;
        private BrightIdeasSoftware.OLVColumn descCol;
        private TabPage tabPage4;
        private BrightIdeasSoftware.DataListView dataListTasks;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn EnabledCol;
        private BrightIdeasSoftware.OLVColumn FreqCol;
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
        private TabPage tabPage1;
        private SplitContainer splitContainer1;
        private Label labelLastEvent;
        private BrightIdeasSoftware.DataListView dataListViewDevices;
        private BrightIdeasSoftware.OLVColumn NodeCol;
        private BrightIdeasSoftware.OLVColumn NameCol;
        private BrightIdeasSoftware.OLVColumn LevelCol;
        private BrightIdeasSoftware.OLVColumn LevelTextCol;
        private BrightIdeasSoftware.OLVColumn ModeCol;
        private BrightIdeasSoftware.OLVColumn FanModeCol;
        private BrightIdeasSoftware.OLVColumn SetPointCol;
        private BrightIdeasSoftware.OLVColumn currStateCol;
        private BrightIdeasSoftware.OLVColumn GroupCol;
        private BrightIdeasSoftware.DataListView dataListViewScenes;
        private BrightIdeasSoftware.OLVColumn SceneNamecol;
        private BrightIdeasSoftware.DataListView dataListViewActions;
        private BrightIdeasSoftware.OLVColumn ColType;
        private BrightIdeasSoftware.OLVColumn ColName;
        private BrightIdeasSoftware.OLVColumn ColAction;
        private Button btn_AddAction;
        private Label labelSceneRunStatus;
        private Button btn_createnonzwaction;
        private ComboBox comboBoxNonZWAction;
        private Label lbl_sceneActions;
        private PictureBox pictureBox3;
        private TabControl MainTabControl;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer3;
        private Label label_devicecount;
        private ToolStripMenuItem saveSettingsToolStripMenuItem;
        private SplitContainer splitContainer4;
    }
}

