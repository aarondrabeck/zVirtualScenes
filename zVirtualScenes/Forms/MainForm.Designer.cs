using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System;
namespace zVirtualScenesApplication
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.imageListActionTypesSmall = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer_TaskRunner = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripScenes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripScenesNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTasks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTasksNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripAddTaks = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripDevicesNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.repollAllDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.process1 = new System.Diagnostics.Process();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataListViewLog = new BrightIdeasSoftware.DataListView();
            this.dateTimeCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.urgencyColu = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.SourceCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.descCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataListTriggers = new BrightIdeasSoftware.DataListView();
            this.eventNameCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.triggerFriendlyName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnEnabled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataListTasks = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.FreqCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.EnabledCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageList25Icons = new System.Windows.Forms.ImageList(this.components);
            this.groupBox_Montly = new System.Windows.Forms.GroupBox();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_even = new System.Windows.Forms.Button();
            this.btn_odd = new System.Windows.Forms.Button();
            this.checkBoxDay31 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay30 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay29 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay28 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay27 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay26 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay25 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay24 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay23 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay22 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay21 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay20 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay19 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay18 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay17 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay16 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay15 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay14 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay13 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay12 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay11 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay10 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay09 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay08 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay07 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay06 = new System.Windows.Forms.CheckBox();
            this.numericUpDownOccurMonths = new System.Windows.Forms.NumericUpDown();
            this.checkBoxDay05 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay04 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay03 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay02 = new System.Windows.Forms.CheckBox();
            this.checkBoxDay01 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox_Weekly = new System.Windows.Forms.GroupBox();
            this.numericUpDownOccurWeeks = new System.Windows.Forms.NumericUpDown();
            this.checkBox_RecurSunday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurSaturday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurFriday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurThursday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurWednesday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurTuesday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurMonday = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox_Seconds = new System.Windows.Forms.GroupBox();
            this.numericUpDownOccurSeconds = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox_Daily = new System.Windows.Forms.GroupBox();
            this.numericUpDownOccurDays = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox_TaskName = new System.Windows.Forms.TextBox();
            this.checkBox_EnabledTask = new System.Windows.Forms.CheckBox();
            this.button_SaveTask = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBox_ActionsTask = new System.Windows.Forms.ComboBox();
            this.dateTimePickerStartTask = new System.Windows.Forms.DateTimePicker();
            this.comboBox_FrequencyTask = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.dataListViewDevices = new BrightIdeasSoftware.DataListView();
            this.nodeID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.IDCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.NCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.TCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.MeterCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.LevelCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.GroupsCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLastHeardFrom = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.uc_device_values1 = new zVirtualScenesApplication.UserControls.uc_device_values();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.dataListViewScenes = new BrightIdeasSoftware.DataListView();
            this.colScene = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label3 = new System.Windows.Forms.Label();
            this.dataListViewDeviceSmallList = new BrightIdeasSoftware.DataListView();
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnAddBltInCMD = new System.Windows.Forms.Button();
            this.lbl_sceneActions = new System.Windows.Forms.Label();
            this.dataListViewSceneCMDs = new BrightIdeasSoftware.DataListView();
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmsSceneCMD = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogsAndDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.builtInCommandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repollAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acitvateGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripnotifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showZVirtualSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.exitZVSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTrigger = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTriggerNull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEventsNull = new System.Windows.Forms.ToolStripMenuItem();
            this.createEventAdvancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripScenes.SuspendLayout();
            this.contextMenuStripScenesNull.SuspendLayout();
            this.contextMenuStripActions.SuspendLayout();
            this.contextMenuStripTasks.SuspendLayout();
            this.contextMenuStripTasksNull.SuspendLayout();
            this.contextMenuStripDevicesNull.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewLog)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListTriggers)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).BeginInit();
            this.groupBox_Montly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurMonths)).BeginInit();
            this.groupBox_Weekly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurWeeks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox_Seconds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurSeconds)).BeginInit();
            this.groupBox_Daily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurDays)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDevices)).BeginInit();
            this.MainTabControl.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewScenes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDeviceSmallList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewSceneCMDs)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.cmsSceneCMD.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStripnotifyIcon.SuspendLayout();
            this.contextMenuStripTrigger.SuspendLayout();
            this.contextMenuStripTriggerNull.SuspendLayout();
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
            this.imageListActionTypesSmall.Images.SetKeyName(9, "controler320.png");
            this.imageListActionTypesSmall.Images.SetKeyName(10, "20bulb.png");
            this.imageListActionTypesSmall.Images.SetKeyName(11, "doorlock20");
            this.imageListActionTypesSmall.Images.SetKeyName(12, "Task");
            // 
            // timer_TaskRunner
            // 
            this.timer_TaskRunner.Enabled = true;
            this.timer_TaskRunner.Interval = 1000;
            this.timer_TaskRunner.Tick += new System.EventHandler(this.timer_TaskRunner_Tick);
            // 
            // contextMenuStripScenes
            // 
            this.contextMenuStripScenes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runSceneToolStripMenuItem,
            this.addSceneToolStripMenuItem,
            this.deleteSceneToolStripMenuItem,
            this.duplicateSceneToolStripMenuItem,
            this.toolStripSeparator3,
            this.editSceneToolStripMenuItem});
            this.contextMenuStripScenes.Name = "contextMenuStripScenes";
            this.contextMenuStripScenes.Size = new System.Drawing.Size(162, 142);
            // 
            // runSceneToolStripMenuItem
            // 
            this.runSceneToolStripMenuItem.Name = "runSceneToolStripMenuItem";
            this.runSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.runSceneToolStripMenuItem.Text = "&Run Scene Now";
            this.runSceneToolStripMenuItem.Click += new System.EventHandler(this.runSceneToolStripMenuItem_Click);
            // 
            // addSceneToolStripMenuItem
            // 
            this.addSceneToolStripMenuItem.Name = "addSceneToolStripMenuItem";
            this.addSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.addSceneToolStripMenuItem.Text = "&Add Scene";
            this.addSceneToolStripMenuItem.Click += new System.EventHandler(this.addSceneToolStripMenuItem_Click);
            // 
            // deleteSceneToolStripMenuItem
            // 
            this.deleteSceneToolStripMenuItem.Name = "deleteSceneToolStripMenuItem";
            this.deleteSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.deleteSceneToolStripMenuItem.Text = "&Delete Scene";
            this.deleteSceneToolStripMenuItem.Click += new System.EventHandler(this.deleteSceneToolStripMenuItem_Click);
            // 
            // duplicateSceneToolStripMenuItem
            // 
            this.duplicateSceneToolStripMenuItem.Name = "duplicateSceneToolStripMenuItem";
            this.duplicateSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.duplicateSceneToolStripMenuItem.Text = "D&uplicate Scene";
            this.duplicateSceneToolStripMenuItem.Click += new System.EventHandler(this.duplicateSceneToolStripMenuItem_Click);
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
            this.editSceneToolStripMenuItem.Text = "Scene &Properties";
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
            // contextMenuStripDevicesNull
            // 
            this.contextMenuStripDevicesNull.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.repollAllDevicesToolStripMenuItem});
            this.contextMenuStripDevicesNull.Name = "contextMenuStripDevicesNull";
            this.contextMenuStripDevicesNull.Size = new System.Drawing.Size(168, 26);
            // 
            // repollAllDevicesToolStripMenuItem
            // 
            this.repollAllDevicesToolStripMenuItem.Name = "repollAllDevicesToolStripMenuItem";
            this.repollAllDevicesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.repollAllDevicesToolStripMenuItem.Text = "Repoll All Devices";
            this.repollAllDevicesToolStripMenuItem.Click += new System.EventHandler(this.repollAllDevicesToolStripMenuItem_Click_1);
            // 
            // process1
            // 
            this.process1.StartInfo.Domain = "";
            this.process1.StartInfo.LoadUserProfile = false;
            this.process1.StartInfo.Password = null;
            this.process1.StartInfo.StandardErrorEncoding = null;
            this.process1.StartInfo.StandardOutputEncoding = null;
            this.process1.StartInfo.UserName = "";
            this.process1.SynchronizingObject = this;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataListViewLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(776, 438);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataListViewLog
            // 
            this.dataListViewLog.AllColumns.Add(this.dateTimeCol);
            this.dataListViewLog.AllColumns.Add(this.urgencyColu);
            this.dataListViewLog.AllColumns.Add(this.SourceCol);
            this.dataListViewLog.AllColumns.Add(this.descCol);
            this.dataListViewLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataListViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dateTimeCol,
            this.urgencyColu,
            this.SourceCol,
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
            this.dataListViewLog.Size = new System.Drawing.Size(770, 432);
            this.dataListViewLog.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewLog.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.dataListViewLog.TabIndex = 31;
            this.dataListViewLog.UseCompatibleStateImageBehavior = false;
            this.dataListViewLog.View = System.Windows.Forms.View.Details;
            // 
            // dateTimeCol
            // 
            this.dateTimeCol.AspectName = "DatetimeLog";
            this.dateTimeCol.Text = "Date & Time";
            this.dateTimeCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.datetimeWidth;
            // 
            // urgencyColu
            // 
            this.urgencyColu.AspectName = "Urgency";
            this.urgencyColu.Text = "Urgency";
            this.urgencyColu.Width = global::zVirtualScenesApplication.Properties.Settings.Default.UrgencyWidth;
            // 
            // SourceCol
            // 
            this.SourceCol.AspectName = "Source";
            this.SourceCol.Text = "Source";
            this.SourceCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.interfaceWidth;
            // 
            // descCol
            // 
            this.descCol.AspectName = "Description";
            this.descCol.FillsFreeSpace = true;
            this.descCol.Text = "Description";
            this.descCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.descWidth;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataListTriggers);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(776, 438);
            this.tabPage1.TabIndex = 6;
            this.tabPage1.Text = "Triggers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataListTriggers
            // 
            this.dataListTriggers.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.dataListTriggers.AllColumns.Add(this.eventNameCol);
            this.dataListTriggers.AllColumns.Add(this.triggerFriendlyName);
            this.dataListTriggers.AllColumns.Add(this.olvColumnEnabled);
            this.dataListTriggers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.eventNameCol,
            this.triggerFriendlyName,
            this.olvColumnEnabled});
            this.dataListTriggers.DataSource = null;
            this.dataListTriggers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListTriggers.EmptyListMsg = "Right-Click to add an Event.";
            this.dataListTriggers.EmptyListMsgFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListTriggers.FullRowSelect = true;
            this.dataListTriggers.HasCollapsibleGroups = false;
            this.dataListTriggers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListTriggers.Location = new System.Drawing.Point(3, 3);
            this.dataListTriggers.MultiSelect = false;
            this.dataListTriggers.Name = "dataListTriggers";
            this.dataListTriggers.ShowGroups = false;
            this.dataListTriggers.Size = new System.Drawing.Size(770, 432);
            this.dataListTriggers.TabIndex = 0;
            this.dataListTriggers.UseCompatibleStateImageBehavior = false;
            this.dataListTriggers.View = System.Windows.Forms.View.Details;
            this.dataListTriggers.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.dataListEvents_CellRightClick);
            this.dataListTriggers.DoubleClick += new System.EventHandler(this.dataListEvents_DoubleClick);
            // 
            // eventNameCol
            // 
            this.eventNameCol.AspectName = "Name";
            this.eventNameCol.Text = "Name";
            this.eventNameCol.Width = 200;
            // 
            // triggerFriendlyName
            // 
            this.triggerFriendlyName.AspectName = "FriendlyName";
            this.triggerFriendlyName.FillsFreeSpace = true;
            this.triggerFriendlyName.Text = "Trigger";
            this.triggerFriendlyName.Width = 200;
            // 
            // olvColumnEnabled
            // 
            this.olvColumnEnabled.AspectName = "enabled";
            this.olvColumnEnabled.Text = "Enabled";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(776, 438);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Scene Scheduling";
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
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.splitContainer2.Panel2.Controls.Add(this.groupBox_Montly);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox_Weekly);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox_Seconds);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2.Controls.Add(this.label6);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox_Daily);
            this.splitContainer2.Panel2.Controls.Add(this.textBox_TaskName);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox_EnabledTask);
            this.splitContainer2.Panel2.Controls.Add(this.button_SaveTask);
            this.splitContainer2.Panel2.Controls.Add(this.label18);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_ActionsTask);
            this.splitContainer2.Panel2.Controls.Add(this.dateTimePickerStartTask);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_FrequencyTask);
            this.splitContainer2.Panel2.Controls.Add(this.label17);
            this.splitContainer2.Panel2MinSize = 553;
            this.splitContainer2.Size = new System.Drawing.Size(770, 432);
            this.splitContainer2.SplitterDistance = global::zVirtualScenesApplication.Properties.Settings.Default.SpiltContainer2Distance;
            this.splitContainer2.TabIndex = 47;
            // 
            // dataListTasks
            // 
            this.dataListTasks.AllColumns.Add(this.olvColumn1);
            this.dataListTasks.AllColumns.Add(this.FreqCol);
            this.dataListTasks.AllColumns.Add(this.EnabledCol);
            this.dataListTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataListTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.FreqCol,
            this.EnabledCol});
            this.dataListTasks.DataSource = null;
            this.dataListTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListTasks.FullRowSelect = true;
            this.dataListTasks.HasCollapsibleGroups = false;
            this.dataListTasks.HideSelection = false;
            this.dataListTasks.Location = new System.Drawing.Point(0, 0);
            this.dataListTasks.Name = "dataListTasks";
            this.dataListTasks.OwnerDraw = true;
            this.dataListTasks.ShowCommandMenuOnRightClick = true;
            this.dataListTasks.ShowGroups = false;
            this.dataListTasks.Size = new System.Drawing.Size(208, 428);
            this.dataListTasks.SmallImageList = this.imageList25Icons;
            this.dataListTasks.TabIndex = 46;
            this.dataListTasks.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dataListTasks.UnfocusedHighlightForegroundColor = System.Drawing.Color.Black;
            this.dataListTasks.UseCompatibleStateImageBehavior = false;
            this.dataListTasks.View = System.Windows.Forms.View.Details;
            this.dataListTasks.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.dataListTasks_CellRightClick_1);
            this.dataListTasks.SelectedIndexChanged += new System.EventHandler(this.dataListTasks_SelectedIndexChanged_1);
            this.dataListTasks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataListTasks_KeyDown);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "friendly_name";
            this.olvColumn1.FillsFreeSpace = true;
            this.olvColumn1.ImageAspectName = "IconName";
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Text = "Task Name";
            this.olvColumn1.Width = global::zVirtualScenesApplication.Properties.Settings.Default.taskWidth;
            // 
            // FreqCol
            // 
            this.FreqCol.AspectName = "FrequencyString";
            this.FreqCol.ImageAspectName = "";
            this.FreqCol.Text = "Frequency";
            this.FreqCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.freqWidth;
            // 
            // EnabledCol
            // 
            this.EnabledCol.AspectName = "isEnabledString";
            this.EnabledCol.IsEditable = false;
            this.EnabledCol.Text = "Enabled";
            this.EnabledCol.Width = global::zVirtualScenesApplication.Properties.Settings.Default.enabledWidth;
            // 
            // imageList25Icons
            // 
            this.imageList25Icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList25Icons.ImageStream")));
            this.imageList25Icons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList25Icons.Images.SetKeyName(0, "Task");
            this.imageList25Icons.Images.SetKeyName(1, "Scene");
            this.imageList25Icons.Images.SetKeyName(2, "SceneRun");
            // 
            // groupBox_Montly
            // 
            this.groupBox_Montly.Controls.Add(this.btn_clear);
            this.groupBox_Montly.Controls.Add(this.btn_even);
            this.groupBox_Montly.Controls.Add(this.btn_odd);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay31);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay30);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay29);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay28);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay27);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay26);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay25);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay24);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay23);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay22);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay21);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay20);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay19);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay18);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay17);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay16);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay15);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay14);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay13);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay12);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay11);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay10);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay09);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay08);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay07);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay06);
            this.groupBox_Montly.Controls.Add(this.numericUpDownOccurMonths);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay05);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay04);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay03);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay02);
            this.groupBox_Montly.Controls.Add(this.checkBoxDay01);
            this.groupBox_Montly.Controls.Add(this.label4);
            this.groupBox_Montly.Controls.Add(this.label8);
            this.groupBox_Montly.Location = new System.Drawing.Point(18, 172);
            this.groupBox_Montly.Name = "groupBox_Montly";
            this.groupBox_Montly.Size = new System.Drawing.Size(335, 247);
            this.groupBox_Montly.TabIndex = 46;
            this.groupBox_Montly.TabStop = false;
            this.groupBox_Montly.Text = "Monthly";
            this.groupBox_Montly.Enter += new System.EventHandler(this.groupBox_Montly_Enter);
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(283, 218);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(46, 23);
            this.btn_clear.TabIndex = 87;
            this.btn_clear.Text = "Clear";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_even
            // 
            this.btn_even.Location = new System.Drawing.Point(231, 12);
            this.btn_even.Name = "btn_even";
            this.btn_even.Size = new System.Drawing.Size(46, 23);
            this.btn_even.TabIndex = 86;
            this.btn_even.Text = "Even";
            this.btn_even.UseVisualStyleBackColor = true;
            this.btn_even.Click += new System.EventHandler(this.btn_even_Click);
            // 
            // btn_odd
            // 
            this.btn_odd.Location = new System.Drawing.Point(283, 12);
            this.btn_odd.Name = "btn_odd";
            this.btn_odd.Size = new System.Drawing.Size(46, 23);
            this.btn_odd.TabIndex = 85;
            this.btn_odd.Text = "Odd";
            this.btn_odd.UseVisualStyleBackColor = true;
            this.btn_odd.Click += new System.EventHandler(this.btn_odd_Click);
            // 
            // checkBoxDay31
            // 
            this.checkBoxDay31.AutoSize = true;
            this.checkBoxDay31.Location = new System.Drawing.Point(198, 190);
            this.checkBoxDay31.Name = "checkBoxDay31";
            this.checkBoxDay31.Size = new System.Drawing.Size(46, 17);
            this.checkBoxDay31.TabIndex = 84;
            this.checkBoxDay31.Text = "31st";
            this.checkBoxDay31.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay30
            // 
            this.checkBoxDay30.AutoSize = true;
            this.checkBoxDay30.Location = new System.Drawing.Point(198, 167);
            this.checkBoxDay30.Name = "checkBoxDay30";
            this.checkBoxDay30.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay30.TabIndex = 83;
            this.checkBoxDay30.Text = "30th";
            this.checkBoxDay30.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay29
            // 
            this.checkBoxDay29.AutoSize = true;
            this.checkBoxDay29.Location = new System.Drawing.Point(198, 145);
            this.checkBoxDay29.Name = "checkBoxDay29";
            this.checkBoxDay29.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay29.TabIndex = 82;
            this.checkBoxDay29.Text = "29th";
            this.checkBoxDay29.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay28
            // 
            this.checkBoxDay28.AutoSize = true;
            this.checkBoxDay28.Location = new System.Drawing.Point(198, 122);
            this.checkBoxDay28.Name = "checkBoxDay28";
            this.checkBoxDay28.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay28.TabIndex = 81;
            this.checkBoxDay28.Text = "28th";
            this.checkBoxDay28.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay27
            // 
            this.checkBoxDay27.AutoSize = true;
            this.checkBoxDay27.Location = new System.Drawing.Point(198, 99);
            this.checkBoxDay27.Name = "checkBoxDay27";
            this.checkBoxDay27.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay27.TabIndex = 80;
            this.checkBoxDay27.Text = "27th";
            this.checkBoxDay27.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay26
            // 
            this.checkBoxDay26.AutoSize = true;
            this.checkBoxDay26.Location = new System.Drawing.Point(198, 76);
            this.checkBoxDay26.Name = "checkBoxDay26";
            this.checkBoxDay26.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay26.TabIndex = 79;
            this.checkBoxDay26.Text = "26th";
            this.checkBoxDay26.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay25
            // 
            this.checkBoxDay25.AutoSize = true;
            this.checkBoxDay25.Location = new System.Drawing.Point(198, 53);
            this.checkBoxDay25.Name = "checkBoxDay25";
            this.checkBoxDay25.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay25.TabIndex = 78;
            this.checkBoxDay25.Text = "25th";
            this.checkBoxDay25.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay24
            // 
            this.checkBoxDay24.AutoSize = true;
            this.checkBoxDay24.Location = new System.Drawing.Point(136, 213);
            this.checkBoxDay24.Name = "checkBoxDay24";
            this.checkBoxDay24.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay24.TabIndex = 77;
            this.checkBoxDay24.Text = "24th";
            this.checkBoxDay24.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay23
            // 
            this.checkBoxDay23.AutoSize = true;
            this.checkBoxDay23.Location = new System.Drawing.Point(136, 190);
            this.checkBoxDay23.Name = "checkBoxDay23";
            this.checkBoxDay23.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay23.TabIndex = 76;
            this.checkBoxDay23.Text = "23rd";
            this.checkBoxDay23.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay22
            // 
            this.checkBoxDay22.AutoSize = true;
            this.checkBoxDay22.Location = new System.Drawing.Point(136, 167);
            this.checkBoxDay22.Name = "checkBoxDay22";
            this.checkBoxDay22.Size = new System.Drawing.Size(50, 17);
            this.checkBoxDay22.TabIndex = 75;
            this.checkBoxDay22.Text = "22nd";
            this.checkBoxDay22.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay21
            // 
            this.checkBoxDay21.AutoSize = true;
            this.checkBoxDay21.Location = new System.Drawing.Point(136, 145);
            this.checkBoxDay21.Name = "checkBoxDay21";
            this.checkBoxDay21.Size = new System.Drawing.Size(46, 17);
            this.checkBoxDay21.TabIndex = 74;
            this.checkBoxDay21.Text = "21st";
            this.checkBoxDay21.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay20
            // 
            this.checkBoxDay20.AutoSize = true;
            this.checkBoxDay20.Location = new System.Drawing.Point(136, 122);
            this.checkBoxDay20.Name = "checkBoxDay20";
            this.checkBoxDay20.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay20.TabIndex = 73;
            this.checkBoxDay20.Text = "20th";
            this.checkBoxDay20.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay19
            // 
            this.checkBoxDay19.AutoSize = true;
            this.checkBoxDay19.Location = new System.Drawing.Point(136, 99);
            this.checkBoxDay19.Name = "checkBoxDay19";
            this.checkBoxDay19.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay19.TabIndex = 72;
            this.checkBoxDay19.Text = "19th";
            this.checkBoxDay19.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay18
            // 
            this.checkBoxDay18.AutoSize = true;
            this.checkBoxDay18.Location = new System.Drawing.Point(136, 76);
            this.checkBoxDay18.Name = "checkBoxDay18";
            this.checkBoxDay18.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay18.TabIndex = 71;
            this.checkBoxDay18.Text = "18th";
            this.checkBoxDay18.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay17
            // 
            this.checkBoxDay17.AutoSize = true;
            this.checkBoxDay17.Location = new System.Drawing.Point(136, 53);
            this.checkBoxDay17.Name = "checkBoxDay17";
            this.checkBoxDay17.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay17.TabIndex = 70;
            this.checkBoxDay17.Text = "17th";
            this.checkBoxDay17.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay16
            // 
            this.checkBoxDay16.AutoSize = true;
            this.checkBoxDay16.Location = new System.Drawing.Point(72, 213);
            this.checkBoxDay16.Name = "checkBoxDay16";
            this.checkBoxDay16.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay16.TabIndex = 69;
            this.checkBoxDay16.Text = "16th";
            this.checkBoxDay16.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay15
            // 
            this.checkBoxDay15.AutoSize = true;
            this.checkBoxDay15.Location = new System.Drawing.Point(72, 190);
            this.checkBoxDay15.Name = "checkBoxDay15";
            this.checkBoxDay15.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay15.TabIndex = 68;
            this.checkBoxDay15.Text = "15th";
            this.checkBoxDay15.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay14
            // 
            this.checkBoxDay14.AutoSize = true;
            this.checkBoxDay14.Location = new System.Drawing.Point(72, 167);
            this.checkBoxDay14.Name = "checkBoxDay14";
            this.checkBoxDay14.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay14.TabIndex = 67;
            this.checkBoxDay14.Text = "14th";
            this.checkBoxDay14.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay13
            // 
            this.checkBoxDay13.AutoSize = true;
            this.checkBoxDay13.Location = new System.Drawing.Point(72, 145);
            this.checkBoxDay13.Name = "checkBoxDay13";
            this.checkBoxDay13.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay13.TabIndex = 66;
            this.checkBoxDay13.Text = "13th";
            this.checkBoxDay13.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay12
            // 
            this.checkBoxDay12.AutoSize = true;
            this.checkBoxDay12.Location = new System.Drawing.Point(72, 122);
            this.checkBoxDay12.Name = "checkBoxDay12";
            this.checkBoxDay12.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay12.TabIndex = 65;
            this.checkBoxDay12.Text = "12th";
            this.checkBoxDay12.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay11
            // 
            this.checkBoxDay11.AutoSize = true;
            this.checkBoxDay11.Location = new System.Drawing.Point(72, 99);
            this.checkBoxDay11.Name = "checkBoxDay11";
            this.checkBoxDay11.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay11.TabIndex = 64;
            this.checkBoxDay11.Text = "11th";
            this.checkBoxDay11.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay10
            // 
            this.checkBoxDay10.AutoSize = true;
            this.checkBoxDay10.Location = new System.Drawing.Point(72, 76);
            this.checkBoxDay10.Name = "checkBoxDay10";
            this.checkBoxDay10.Size = new System.Drawing.Size(47, 17);
            this.checkBoxDay10.TabIndex = 63;
            this.checkBoxDay10.Text = "10th";
            this.checkBoxDay10.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay09
            // 
            this.checkBoxDay09.AutoSize = true;
            this.checkBoxDay09.Location = new System.Drawing.Point(72, 53);
            this.checkBoxDay09.Name = "checkBoxDay09";
            this.checkBoxDay09.Size = new System.Drawing.Size(41, 17);
            this.checkBoxDay09.TabIndex = 62;
            this.checkBoxDay09.Text = "9th";
            this.checkBoxDay09.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay08
            // 
            this.checkBoxDay08.AutoSize = true;
            this.checkBoxDay08.Location = new System.Drawing.Point(17, 213);
            this.checkBoxDay08.Name = "checkBoxDay08";
            this.checkBoxDay08.Size = new System.Drawing.Size(41, 17);
            this.checkBoxDay08.TabIndex = 61;
            this.checkBoxDay08.Text = "8th";
            this.checkBoxDay08.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay07
            // 
            this.checkBoxDay07.AutoSize = true;
            this.checkBoxDay07.Location = new System.Drawing.Point(17, 190);
            this.checkBoxDay07.Name = "checkBoxDay07";
            this.checkBoxDay07.Size = new System.Drawing.Size(41, 17);
            this.checkBoxDay07.TabIndex = 60;
            this.checkBoxDay07.Text = "7th";
            this.checkBoxDay07.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay06
            // 
            this.checkBoxDay06.AutoSize = true;
            this.checkBoxDay06.Location = new System.Drawing.Point(17, 167);
            this.checkBoxDay06.Name = "checkBoxDay06";
            this.checkBoxDay06.Size = new System.Drawing.Size(41, 17);
            this.checkBoxDay06.TabIndex = 59;
            this.checkBoxDay06.Text = "6th";
            this.checkBoxDay06.UseVisualStyleBackColor = true;
            // 
            // numericUpDownOccurMonths
            // 
            this.numericUpDownOccurMonths.Location = new System.Drawing.Point(86, 21);
            this.numericUpDownOccurMonths.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numericUpDownOccurMonths.Name = "numericUpDownOccurMonths";
            this.numericUpDownOccurMonths.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownOccurMonths.TabIndex = 56;
            // 
            // checkBoxDay05
            // 
            this.checkBoxDay05.AutoSize = true;
            this.checkBoxDay05.Location = new System.Drawing.Point(17, 145);
            this.checkBoxDay05.Name = "checkBoxDay05";
            this.checkBoxDay05.Size = new System.Drawing.Size(41, 17);
            this.checkBoxDay05.TabIndex = 55;
            this.checkBoxDay05.Text = "5th";
            this.checkBoxDay05.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay04
            // 
            this.checkBoxDay04.AutoSize = true;
            this.checkBoxDay04.Location = new System.Drawing.Point(17, 122);
            this.checkBoxDay04.Name = "checkBoxDay04";
            this.checkBoxDay04.Size = new System.Drawing.Size(41, 17);
            this.checkBoxDay04.TabIndex = 54;
            this.checkBoxDay04.Text = "4th";
            this.checkBoxDay04.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay03
            // 
            this.checkBoxDay03.AutoSize = true;
            this.checkBoxDay03.Location = new System.Drawing.Point(17, 99);
            this.checkBoxDay03.Name = "checkBoxDay03";
            this.checkBoxDay03.Size = new System.Drawing.Size(41, 17);
            this.checkBoxDay03.TabIndex = 53;
            this.checkBoxDay03.Text = "3rd";
            this.checkBoxDay03.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay02
            // 
            this.checkBoxDay02.AutoSize = true;
            this.checkBoxDay02.Location = new System.Drawing.Point(17, 76);
            this.checkBoxDay02.Name = "checkBoxDay02";
            this.checkBoxDay02.Size = new System.Drawing.Size(44, 17);
            this.checkBoxDay02.TabIndex = 52;
            this.checkBoxDay02.Text = "2nd";
            this.checkBoxDay02.UseVisualStyleBackColor = true;
            // 
            // checkBoxDay01
            // 
            this.checkBoxDay01.AutoSize = true;
            this.checkBoxDay01.Location = new System.Drawing.Point(17, 53);
            this.checkBoxDay01.Name = "checkBoxDay01";
            this.checkBoxDay01.Size = new System.Drawing.Size(40, 17);
            this.checkBoxDay01.TabIndex = 51;
            this.checkBoxDay01.Text = "1st";
            this.checkBoxDay01.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 50;
            this.label4.Text = "months.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 49;
            this.label8.Text = "Recur every";
            // 
            // groupBox_Weekly
            // 
            this.groupBox_Weekly.Controls.Add(this.numericUpDownOccurWeeks);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurSunday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurSaturday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurFriday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurThursday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurWednesday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurTuesday);
            this.groupBox_Weekly.Controls.Add(this.checkBox_RecurMonday);
            this.groupBox_Weekly.Controls.Add(this.label7);
            this.groupBox_Weekly.Controls.Add(this.label12);
            this.groupBox_Weekly.Location = new System.Drawing.Point(17, 172);
            this.groupBox_Weekly.Name = "groupBox_Weekly";
            this.groupBox_Weekly.Size = new System.Drawing.Size(206, 165);
            this.groupBox_Weekly.TabIndex = 41;
            this.groupBox_Weekly.TabStop = false;
            this.groupBox_Weekly.Text = "Weekly";
            // 
            // numericUpDownOccurWeeks
            // 
            this.numericUpDownOccurWeeks.Location = new System.Drawing.Point(91, 16);
            this.numericUpDownOccurWeeks.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numericUpDownOccurWeeks.Name = "numericUpDownOccurWeeks";
            this.numericUpDownOccurWeeks.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownOccurWeeks.TabIndex = 47;
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
            this.label7.Location = new System.Drawing.Point(156, 22);
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
            this.label12.Size = new System.Drawing.Size(65, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "Recur every";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::zVirtualScenesApplication.Properties.Resources.task;
            this.pictureBox1.Location = new System.Drawing.Point(381, 195);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(154, 142);
            this.pictureBox1.TabIndex = 44;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox_Seconds
            // 
            this.groupBox_Seconds.Controls.Add(this.numericUpDownOccurSeconds);
            this.groupBox_Seconds.Controls.Add(this.label1);
            this.groupBox_Seconds.Controls.Add(this.label2);
            this.groupBox_Seconds.Location = new System.Drawing.Point(17, 172);
            this.groupBox_Seconds.Name = "groupBox_Seconds";
            this.groupBox_Seconds.Size = new System.Drawing.Size(234, 54);
            this.groupBox_Seconds.TabIndex = 41;
            this.groupBox_Seconds.TabStop = false;
            this.groupBox_Seconds.Text = "Per Second";
            // 
            // numericUpDownOccurSeconds
            // 
            this.numericUpDownOccurSeconds.Location = new System.Drawing.Point(95, 19);
            this.numericUpDownOccurSeconds.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.numericUpDownOccurSeconds.Name = "numericUpDownOccurSeconds";
            this.numericUpDownOccurSeconds.Size = new System.Drawing.Size(76, 20);
            this.numericUpDownOccurSeconds.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "seconds.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "Recur every";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Task Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Activate Scene:";
            // 
            // groupBox_Daily
            // 
            this.groupBox_Daily.Controls.Add(this.numericUpDownOccurDays);
            this.groupBox_Daily.Controls.Add(this.label19);
            this.groupBox_Daily.Controls.Add(this.label20);
            this.groupBox_Daily.Location = new System.Drawing.Point(17, 172);
            this.groupBox_Daily.Name = "groupBox_Daily";
            this.groupBox_Daily.Size = new System.Drawing.Size(206, 54);
            this.groupBox_Daily.TabIndex = 37;
            this.groupBox_Daily.TabStop = false;
            this.groupBox_Daily.Text = "Daily";
            // 
            // numericUpDownOccurDays
            // 
            this.numericUpDownOccurDays.Location = new System.Drawing.Point(90, 19);
            this.numericUpDownOccurDays.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numericUpDownOccurDays.Name = "numericUpDownOccurDays";
            this.numericUpDownOccurDays.Size = new System.Drawing.Size(76, 20);
            this.numericUpDownOccurDays.TabIndex = 46;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(168, 22);
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
            this.label20.Size = new System.Drawing.Size(65, 13);
            this.label20.TabIndex = 39;
            this.label20.Text = "Recur every";
            // 
            // textBox_TaskName
            // 
            this.textBox_TaskName.Location = new System.Drawing.Point(111, 13);
            this.textBox_TaskName.Name = "textBox_TaskName";
            this.textBox_TaskName.Size = new System.Drawing.Size(297, 20);
            this.textBox_TaskName.TabIndex = 0;
            // 
            // checkBox_EnabledTask
            // 
            this.checkBox_EnabledTask.AutoSize = true;
            this.checkBox_EnabledTask.Location = new System.Drawing.Point(112, 143);
            this.checkBox_EnabledTask.Name = "checkBox_EnabledTask";
            this.checkBox_EnabledTask.Size = new System.Drawing.Size(92, 17);
            this.checkBox_EnabledTask.TabIndex = 39;
            this.checkBox_EnabledTask.Text = "Task Enabled";
            this.checkBox_EnabledTask.UseVisualStyleBackColor = true;
            // 
            // button_SaveTask
            // 
            this.button_SaveTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveTask.Location = new System.Drawing.Point(438, 376);
            this.button_SaveTask.Name = "button_SaveTask";
            this.button_SaveTask.Size = new System.Drawing.Size(97, 25);
            this.button_SaveTask.TabIndex = 38;
            this.button_SaveTask.Text = "Save Task";
            this.button_SaveTask.UseVisualStyleBackColor = true;
            this.button_SaveTask.Click += new System.EventHandler(this.button_SaveTask_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(14, 112);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 13);
            this.label18.TabIndex = 36;
            this.label18.Text = "Start:";
            // 
            // comboBox_ActionsTask
            // 
            this.comboBox_ActionsTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ActionsTask.FormattingEnabled = true;
            this.comboBox_ActionsTask.Location = new System.Drawing.Point(111, 75);
            this.comboBox_ActionsTask.Name = "comboBox_ActionsTask";
            this.comboBox_ActionsTask.Size = new System.Drawing.Size(297, 21);
            this.comboBox_ActionsTask.TabIndex = 42;
            // 
            // dateTimePickerStartTask
            // 
            this.dateTimePickerStartTask.CustomFormat = "dddd,MMMM d, yyyy \'at\' h:mm:ss tt";
            this.dateTimePickerStartTask.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStartTask.Location = new System.Drawing.Point(111, 108);
            this.dateTimePickerStartTask.Name = "dateTimePickerStartTask";
            this.dateTimePickerStartTask.Size = new System.Drawing.Size(297, 20);
            this.dateTimePickerStartTask.TabIndex = 35;
            // 
            // comboBox_FrequencyTask
            // 
            this.comboBox_FrequencyTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_FrequencyTask.FormattingEnabled = true;
            this.comboBox_FrequencyTask.Location = new System.Drawing.Point(111, 44);
            this.comboBox_FrequencyTask.Name = "comboBox_FrequencyTask";
            this.comboBox_FrequencyTask.Size = new System.Drawing.Size(297, 21);
            this.comboBox_FrequencyTask.TabIndex = 33;
            this.comboBox_FrequencyTask.SelectedIndexChanged += new System.EventHandler(this.comboBox_FrequencyTask_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(14, 47);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(87, 13);
            this.label17.TabIndex = 34;
            this.label17.Text = "Task Frequency:";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer3);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(776, 438);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "Devices";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dataListViewDevices);
            this.splitContainer3.Panel1MinSize = 100;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.uc_device_values1);
            this.splitContainer3.Panel2MinSize = 200;
            this.splitContainer3.Size = new System.Drawing.Size(770, 432);
            this.splitContainer3.SplitterDistance = 228;
            this.splitContainer3.TabIndex = 1;
            // 
            // dataListViewDevices
            // 
            this.dataListViewDevices.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.dataListViewDevices.AllColumns.Add(this.nodeID);
            this.dataListViewDevices.AllColumns.Add(this.IDCol);
            this.dataListViewDevices.AllColumns.Add(this.NCol);
            this.dataListViewDevices.AllColumns.Add(this.TCol);
            this.dataListViewDevices.AllColumns.Add(this.MeterCol);
            this.dataListViewDevices.AllColumns.Add(this.LevelCol);
            this.dataListViewDevices.AllColumns.Add(this.GroupsCol);
            this.dataListViewDevices.AllColumns.Add(this.colLastHeardFrom);
            this.dataListViewDevices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataListViewDevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nodeID,
            this.IDCol,
            this.NCol,
            this.TCol,
            this.MeterCol,
            this.LevelCol,
            this.GroupsCol,
            this.colLastHeardFrom});
            this.dataListViewDevices.DataSource = null;
            this.dataListViewDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewDevices.EmptyListMsg = "No Devices";
            this.dataListViewDevices.FullRowSelect = true;
            this.dataListViewDevices.HasCollapsibleGroups = false;
            this.dataListViewDevices.HideSelection = false;
            this.dataListViewDevices.Location = new System.Drawing.Point(0, 0);
            this.dataListViewDevices.MultiSelect = false;
            this.dataListViewDevices.Name = "dataListViewDevices";
            this.dataListViewDevices.OwnerDraw = true;
            this.dataListViewDevices.RenderNonEditableCheckboxesAsDisabled = true;
            this.dataListViewDevices.SelectAllOnControlA = false;
            this.dataListViewDevices.SelectColumnsMenuStaysOpen = false;
            this.dataListViewDevices.SelectColumnsOnRightClick = false;
            this.dataListViewDevices.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.dataListViewDevices.ShowCommandMenuOnRightClick = true;
            this.dataListViewDevices.ShowGroups = false;
            this.dataListViewDevices.Size = new System.Drawing.Size(768, 226);
            this.dataListViewDevices.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewDevices.SortGroupItemsByPrimaryColumn = false;
            this.dataListViewDevices.TabIndex = 0;
            this.dataListViewDevices.UseCompatibleStateImageBehavior = false;
            this.dataListViewDevices.View = System.Windows.Forms.View.Details;
            this.dataListViewDevices.SelectedIndexChanged += new System.EventHandler(this.dataListViewDevices_SelectedIndexChanged);
            this.dataListViewDevices.DoubleClick += new System.EventHandler(this.dataListViewDevices_DoubleClick);
            // 
            // nodeID
            // 
            this.nodeID.AspectName = "node_id";
            this.nodeID.DisplayIndex = 6;
            this.nodeID.Text = "Node";
            // 
            // IDCol
            // 
            this.IDCol.AspectName = "id";
            this.IDCol.DisplayIndex = 0;
            this.IDCol.ImageAspectName = "DeviceIcon";
            this.IDCol.IsEditable = false;
            this.IDCol.Text = "ID";
            // 
            // NCol
            // 
            this.NCol.AspectName = "friendly_name";
            this.NCol.DisplayIndex = 1;
            this.NCol.FillsFreeSpace = true;
            this.NCol.Text = "Name";
            this.NCol.Width = 150;
            // 
            // TCol
            // 
            this.TCol.AspectName = "device_types.friendly_name";
            this.TCol.DisplayIndex = 2;
            this.TCol.IsEditable = false;
            this.TCol.Text = "Type";
            this.TCol.Width = 150;
            // 
            // MeterCol
            // 
            this.MeterCol.AspectName = "GetLevelMeter";
            this.MeterCol.DisplayIndex = 3;
            this.MeterCol.Text = "";
            // 
            // LevelCol
            // 
            this.LevelCol.AspectName = "GetLevelText";
            this.LevelCol.DisplayIndex = 4;
            this.LevelCol.Text = "Level";
            this.LevelCol.Width = 40;
            // 
            // GroupsCol
            // 
            this.GroupsCol.AspectName = "GetGroups";
            this.GroupsCol.DisplayIndex = 5;
            this.GroupsCol.Text = "Groups / Zones";
            this.GroupsCol.Width = 100;
            // 
            // colLastHeardFrom
            // 
            this.colLastHeardFrom.AspectName = "last_heard_from";
            this.colLastHeardFrom.Text = "Last Queried";
            this.colLastHeardFrom.Width = 150;
            // 
            // uc_device_values1
            // 
            this.uc_device_values1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc_device_values1.Location = new System.Drawing.Point(0, 0);
            this.uc_device_values1.Name = "uc_device_values1";
            this.uc_device_values1.Size = new System.Drawing.Size(768, 198);
            this.uc_device_values1.TabIndex = 0;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabControl.Controls.Add(this.tabPage5);
            this.MainTabControl.Controls.Add(this.tabPage3);
            this.MainTabControl.Controls.Add(this.tabPage4);
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Controls.Add(this.tabPage2);
            this.MainTabControl.Location = new System.Drawing.Point(0, 24);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(784, 464);
            this.MainTabControl.TabIndex = 0;
            this.MainTabControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainTabControl_KeyDown);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(776, 438);
            this.tabPage3.TabIndex = 7;
            this.tabPage3.Text = "Scenes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.dataListViewScenes);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.label3);
            this.splitContainer4.Panel2.Controls.Add(this.dataListViewDeviceSmallList);
            this.splitContainer4.Panel2.Controls.Add(this.btnAddBltInCMD);
            this.splitContainer4.Panel2.Controls.Add(this.lbl_sceneActions);
            this.splitContainer4.Panel2.Controls.Add(this.dataListViewSceneCMDs);
            this.splitContainer4.Size = new System.Drawing.Size(770, 432);
            this.splitContainer4.SplitterDistance = 255;
            this.splitContainer4.TabIndex = 0;
            // 
            // dataListViewScenes
            // 
            this.dataListViewScenes.AllColumns.Add(this.colScene);
            this.dataListViewScenes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataListViewScenes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colScene});
            this.dataListViewScenes.DataSource = null;
            this.dataListViewScenes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewScenes.FullRowSelect = true;
            this.dataListViewScenes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListViewScenes.HideSelection = false;
            this.dataListViewScenes.IsSimpleDragSource = true;
            this.dataListViewScenes.Location = new System.Drawing.Point(0, 0);
            this.dataListViewScenes.MultiSelect = false;
            this.dataListViewScenes.Name = "dataListViewScenes";
            this.dataListViewScenes.OwnerDraw = true;
            this.dataListViewScenes.ShowGroups = false;
            this.dataListViewScenes.Size = new System.Drawing.Size(255, 432);
            this.dataListViewScenes.SmallImageList = this.imageList25Icons;
            this.dataListViewScenes.SortGroupItemsByPrimaryColumn = false;
            this.dataListViewScenes.TabIndex = 0;
            this.dataListViewScenes.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dataListViewScenes.UnfocusedHighlightForegroundColor = System.Drawing.Color.Black;
            this.dataListViewScenes.UseCompatibleStateImageBehavior = false;
            this.dataListViewScenes.UseExplorerTheme = true;
            this.dataListViewScenes.View = System.Windows.Forms.View.Details;
            this.dataListViewScenes.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.dataListViewScenes_CellRightClick_1);
            this.dataListViewScenes.ModelCanDrop += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.dataListViewScenes_ModelCanDrop_1);
            this.dataListViewScenes.ModelDropped += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.dataListViewScenes_ModelDropped_1);
            this.dataListViewScenes.SelectedIndexChanged += new System.EventHandler(this.dataListViewScenes_SelectedIndexChanged_1);
            this.dataListViewScenes.DoubleClick += new System.EventHandler(this.dataListViewScenes_DoubleClick_1);
            this.dataListViewScenes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataListViewScenes_KeyDown_1);
            // 
            // colScene
            // 
            this.colScene.AspectName = "friendly_name";
            this.colScene.FillsFreeSpace = true;
            this.colScene.ImageAspectName = "DeviceIcon";
            this.colScene.Text = "Scenes";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(380, 281);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 100);
            this.label3.TabIndex = 6;
            this.label3.Text = "Drag and drop device to add them to scenes or add a builtin command with the butt" +
    "on below.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataListViewDeviceSmallList
            // 
            this.dataListViewDeviceSmallList.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.dataListViewDeviceSmallList.AllColumns.Add(this.olvColumn5);
            this.dataListViewDeviceSmallList.AllColumns.Add(this.olvColumn6);
            this.dataListViewDeviceSmallList.AllColumns.Add(this.olvColumn4);
            this.dataListViewDeviceSmallList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListViewDeviceSmallList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataListViewDeviceSmallList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn4});
            this.dataListViewDeviceSmallList.DataSource = null;
            this.dataListViewDeviceSmallList.EmptyListMsg = "No Devices";
            this.dataListViewDeviceSmallList.FullRowSelect = true;
            this.dataListViewDeviceSmallList.HasCollapsibleGroups = false;
            this.dataListViewDeviceSmallList.HideSelection = false;
            this.dataListViewDeviceSmallList.IsSimpleDragSource = true;
            this.dataListViewDeviceSmallList.Location = new System.Drawing.Point(3, 253);
            this.dataListViewDeviceSmallList.Name = "dataListViewDeviceSmallList";
            this.dataListViewDeviceSmallList.OwnerDraw = true;
            this.dataListViewDeviceSmallList.RenderNonEditableCheckboxesAsDisabled = true;
            this.dataListViewDeviceSmallList.SelectAllOnControlA = false;
            this.dataListViewDeviceSmallList.SelectColumnsMenuStaysOpen = false;
            this.dataListViewDeviceSmallList.SelectColumnsOnRightClick = false;
            this.dataListViewDeviceSmallList.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.dataListViewDeviceSmallList.ShowCommandMenuOnRightClick = true;
            this.dataListViewDeviceSmallList.ShowGroups = false;
            this.dataListViewDeviceSmallList.Size = new System.Drawing.Size(366, 174);
            this.dataListViewDeviceSmallList.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewDeviceSmallList.SortGroupItemsByPrimaryColumn = false;
            this.dataListViewDeviceSmallList.TabIndex = 5;
            this.dataListViewDeviceSmallList.UseCompatibleStateImageBehavior = false;
            this.dataListViewDeviceSmallList.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "id";
            this.olvColumn5.ImageAspectName = "DeviceIcon";
            this.olvColumn5.IsEditable = false;
            this.olvColumn5.Text = "ID";
            this.olvColumn5.Width = 50;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "friendly_name";
            this.olvColumn6.FillsFreeSpace = true;
            this.olvColumn6.Text = "Name";
            this.olvColumn6.Width = 150;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "device_types.friendly_name";
            this.olvColumn4.Text = "Type";
            this.olvColumn4.Width = 200;
            // 
            // btnAddBltInCMD
            // 
            this.btnAddBltInCMD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBltInCMD.Location = new System.Drawing.Point(375, 404);
            this.btnAddBltInCMD.Name = "btnAddBltInCMD";
            this.btnAddBltInCMD.Size = new System.Drawing.Size(131, 23);
            this.btnAddBltInCMD.TabIndex = 3;
            this.btnAddBltInCMD.Text = "Add Builtin Command";
            this.btnAddBltInCMD.UseVisualStyleBackColor = true;
            this.btnAddBltInCMD.Click += new System.EventHandler(this.btnAddBltInCMD_Click);
            // 
            // lbl_sceneActions
            // 
            this.lbl_sceneActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_sceneActions.Location = new System.Drawing.Point(3, 6);
            this.lbl_sceneActions.Name = "lbl_sceneActions";
            this.lbl_sceneActions.Size = new System.Drawing.Size(503, 15);
            this.lbl_sceneActions.TabIndex = 1;
            this.lbl_sceneActions.Text = "Scene 1";
            // 
            // dataListViewSceneCMDs
            // 
            this.dataListViewSceneCMDs.AllColumns.Add(this.olvColumn2);
            this.dataListViewSceneCMDs.AllColumns.Add(this.olvColumn3);
            this.dataListViewSceneCMDs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListViewSceneCMDs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataListViewSceneCMDs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn2,
            this.olvColumn3});
            this.dataListViewSceneCMDs.DataSource = null;
            this.dataListViewSceneCMDs.FullRowSelect = true;
            this.dataListViewSceneCMDs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListViewSceneCMDs.HideSelection = false;
            this.dataListViewSceneCMDs.IsSimpleDragSource = true;
            this.dataListViewSceneCMDs.Location = new System.Drawing.Point(3, 24);
            this.dataListViewSceneCMDs.Name = "dataListViewSceneCMDs";
            this.dataListViewSceneCMDs.OwnerDraw = true;
            this.dataListViewSceneCMDs.SelectColumnsOnRightClick = false;
            this.dataListViewSceneCMDs.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.dataListViewSceneCMDs.ShowGroups = false;
            this.dataListViewSceneCMDs.Size = new System.Drawing.Size(503, 223);
            this.dataListViewSceneCMDs.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewSceneCMDs.TabIndex = 0;
            this.dataListViewSceneCMDs.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dataListViewSceneCMDs.UnfocusedHighlightForegroundColor = System.Drawing.Color.Black;
            this.dataListViewSceneCMDs.UseCompatibleStateImageBehavior = false;
            this.dataListViewSceneCMDs.UseExplorerTheme = true;
            this.dataListViewSceneCMDs.View = System.Windows.Forms.View.Details;
            this.dataListViewSceneCMDs.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.dataListViewSceneCMDs_CellRightClick);
            this.dataListViewSceneCMDs.ModelCanDrop += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.dataListViewSceneCMDs_ModelCanDrop);
            this.dataListViewSceneCMDs.ModelDropped += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.dataListViewSceneCMDs_ModelDropped);
            this.dataListViewSceneCMDs.DoubleClick += new System.EventHandler(this.dataListViewSceneCMDs_DoubleClick);
            this.dataListViewSceneCMDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataListViewSceneCMDs_KeyDown);
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Actionable_Object";
            this.olvColumn2.ImageAspectName = "DeviceIcon";
            this.olvColumn2.Text = "Command / Action";
            this.olvColumn2.Width = 200;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Action_Description";
            this.olvColumn3.FillsFreeSpace = true;
            this.olvColumn3.Text = "";
            this.olvColumn3.Width = 200;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 491);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // cmsSceneCMD
            // 
            this.cmsSceneCMD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCommandToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteCommandToolStripMenuItem});
            this.cmsSceneCMD.Name = "cmsSceneCMD";
            this.cmsSceneCMD.Size = new System.Drawing.Size(168, 54);
            // 
            // editCommandToolStripMenuItem
            // 
            this.editCommandToolStripMenuItem.Name = "editCommandToolStripMenuItem";
            this.editCommandToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.editCommandToolStripMenuItem.Text = "&Edit Command";
            this.editCommandToolStripMenuItem.Click += new System.EventHandler(this.editCommandToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // deleteCommandToolStripMenuItem
            // 
            this.deleteCommandToolStripMenuItem.Name = "deleteCommandToolStripMenuItem";
            this.deleteCommandToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.deleteCommandToolStripMenuItem.Text = "&Delete Command";
            this.deleteCommandToolStripMenuItem.Click += new System.EventHandler(this.deleteCommandToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.builtInCommandsToolStripMenuItem,
            this.groupsToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewDatabaseToolStripMenuItem,
            this.viewLogsAndDatabaseToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // viewLogsAndDatabaseToolStripMenuItem
            // 
            this.viewLogsAndDatabaseToolStripMenuItem.Name = "viewLogsAndDatabaseToolStripMenuItem";
            this.viewLogsAndDatabaseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewLogsAndDatabaseToolStripMenuItem.Text = "View &Logs";
            this.viewLogsAndDatabaseToolStripMenuItem.Click += new System.EventHandler(this.viewLogsAndDatabaseToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // builtInCommandsToolStripMenuItem
            // 
            this.builtInCommandsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.repollAllToolStripMenuItem});
            this.builtInCommandsToolStripMenuItem.Name = "builtInCommandsToolStripMenuItem";
            this.builtInCommandsToolStripMenuItem.Size = new System.Drawing.Size(123, 20);
            this.builtInCommandsToolStripMenuItem.Text = "&Built-In Commands";
            // 
            // repollAllToolStripMenuItem
            // 
            this.repollAllToolStripMenuItem.Name = "repollAllToolStripMenuItem";
            this.repollAllToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.repollAllToolStripMenuItem.Text = "&Repoll All";
            this.repollAllToolStripMenuItem.Click += new System.EventHandler(this.repollAllToolStripMenuItem_Click);
            // 
            // groupsToolStripMenuItem
            // 
            this.groupsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.acitvateGroupsToolStripMenuItem,
            this.editGroupsToolStripMenuItem});
            this.groupsToolStripMenuItem.Name = "groupsToolStripMenuItem";
            this.groupsToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.groupsToolStripMenuItem.Text = "&Groups";
            // 
            // acitvateGroupsToolStripMenuItem
            // 
            this.acitvateGroupsToolStripMenuItem.Name = "acitvateGroupsToolStripMenuItem";
            this.acitvateGroupsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.acitvateGroupsToolStripMenuItem.Text = "&Acitvate Groups";
            this.acitvateGroupsToolStripMenuItem.Click += new System.EventHandler(this.acitvateGroupsToolStripMenuItem_Click);
            // 
            // editGroupsToolStripMenuItem
            // 
            this.editGroupsToolStripMenuItem.Name = "editGroupsToolStripMenuItem";
            this.editGroupsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.editGroupsToolStripMenuItem.Text = "&Edit Groups";
            this.editGroupsToolStripMenuItem.Click += new System.EventHandler(this.editGroupsToolStripMenuItem_Click_1);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator5,
            this.pluginsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.optionsToolStripMenuItem.Text = "&Tools";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(113, 6);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.pluginsToolStripMenuItem.Text = "&Options";
            this.pluginsToolStripMenuItem.Click += new System.EventHandler(this.pluginsToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStripnotifyIcon;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "zVirtualScenes";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStripnotifyIcon
            // 
            this.contextMenuStripnotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showZVirtualSceneToolStripMenuItem,
            this.toolStripSeparator6,
            this.exitZVSToolStripMenuItem});
            this.contextMenuStripnotifyIcon.Name = "contextMenuStripnotifyIcon";
            this.contextMenuStripnotifyIcon.Size = new System.Drawing.Size(182, 54);
            // 
            // showZVirtualSceneToolStripMenuItem
            // 
            this.showZVirtualSceneToolStripMenuItem.Name = "showZVirtualSceneToolStripMenuItem";
            this.showZVirtualSceneToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.showZVirtualSceneToolStripMenuItem.Text = "Show zVirtualScenes";
            this.showZVirtualSceneToolStripMenuItem.Click += new System.EventHandler(this.showZVirtualSceneToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(178, 6);
            // 
            // exitZVSToolStripMenuItem
            // 
            this.exitZVSToolStripMenuItem.Name = "exitZVSToolStripMenuItem";
            this.exitZVSToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.exitZVSToolStripMenuItem.Text = "E&xit  zVirtualScenes";
            this.exitZVSToolStripMenuItem.Click += new System.EventHandler(this.exitZVSToolStripMenuItem_Click);
            // 
            // contextMenuStripTrigger
            // 
            this.contextMenuStripTrigger.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem1,
            this.editToolStripMenuItem,
            this.deleteEventToolStripMenuItem});
            this.contextMenuStripTrigger.Name = "contextMenuStripEvent";
            this.contextMenuStripTrigger.Size = new System.Drawing.Size(150, 70);
            // 
            // addToolStripMenuItem1
            // 
            this.addToolStripMenuItem1.Name = "addToolStripMenuItem1";
            this.addToolStripMenuItem1.Size = new System.Drawing.Size(149, 22);
            this.addToolStripMenuItem1.Text = "&Create Trigger";
            this.addToolStripMenuItem1.Click += new System.EventHandler(this.addToolStripMenuItem1_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.editToolStripMenuItem.Text = "&Edit Trigger";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteEventToolStripMenuItem
            // 
            this.deleteEventToolStripMenuItem.Name = "deleteEventToolStripMenuItem";
            this.deleteEventToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.deleteEventToolStripMenuItem.Text = "&Delete Trigger";
            this.deleteEventToolStripMenuItem.Click += new System.EventHandler(this.deleteEventToolStripMenuItem_Click);
            // 
            // contextMenuStripTriggerNull
            // 
            this.contextMenuStripTriggerNull.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEventsNull,
            this.createEventAdvancedToolStripMenuItem});
            this.contextMenuStripTriggerNull.Name = "contextMenuStripEvent";
            this.contextMenuStripTriggerNull.Size = new System.Drawing.Size(206, 48);
            // 
            // toolStripMenuItemEventsNull
            // 
            this.toolStripMenuItemEventsNull.Name = "toolStripMenuItemEventsNull";
            this.toolStripMenuItemEventsNull.Size = new System.Drawing.Size(205, 22);
            this.toolStripMenuItemEventsNull.Text = "&Create Basic Trigger";
            this.toolStripMenuItemEventsNull.Click += new System.EventHandler(this.toolStripMenuItemEventsNull_Click);
            // 
            // createEventAdvancedToolStripMenuItem
            // 
            this.createEventAdvancedToolStripMenuItem.Name = "createEventAdvancedToolStripMenuItem";
            this.createEventAdvancedToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.createEventAdvancedToolStripMenuItem.Text = "Create &Advanced Trigger";
            this.createEventAdvancedToolStripMenuItem.Click += new System.EventHandler(this.createEventAdvancedToolStripMenuItem_Click);
            // 
            // viewDatabaseToolStripMenuItem
            // 
            this.viewDatabaseToolStripMenuItem.Name = "viewDatabaseToolStripMenuItem";
            this.viewDatabaseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewDatabaseToolStripMenuItem.Text = "View &Database";
            this.viewDatabaseToolStripMenuItem.Click += new System.EventHandler(this.viewDatabaseToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 513);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.MainTabControl);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zVirtualScenesApplication.Properties.Settings.Default, "MainFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::zVirtualScenesApplication.Properties.Settings.Default.MainFormLocation;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 551);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.zVirtualScenes_FormClosing);
            this.Load += new System.EventHandler(this.zVirtualScenes_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStripScenes.ResumeLayout(false);
            this.contextMenuStripScenesNull.ResumeLayout(false);
            this.contextMenuStripActions.ResumeLayout(false);
            this.contextMenuStripTasks.ResumeLayout(false);
            this.contextMenuStripTasksNull.ResumeLayout(false);
            this.contextMenuStripDevicesNull.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewLog)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListTriggers)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).EndInit();
            this.groupBox_Montly.ResumeLayout(false);
            this.groupBox_Montly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurMonths)).EndInit();
            this.groupBox_Weekly.ResumeLayout(false);
            this.groupBox_Weekly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurWeeks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox_Seconds.ResumeLayout(false);
            this.groupBox_Seconds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurSeconds)).EndInit();
            this.groupBox_Daily.ResumeLayout(false);
            this.groupBox_Daily.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurDays)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDevices)).EndInit();
            this.MainTabControl.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewScenes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewDeviceSmallList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewSceneCMDs)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cmsSceneCMD.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStripnotifyIcon.ResumeLayout(false);
            this.contextMenuStripTrigger.ResumeLayout(false);
            this.contextMenuStripTriggerNull.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private Timer timer_TaskRunner;
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
        private ContextMenuStrip contextMenuStripTasks;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ContextMenuStrip contextMenuStripTasksNull;
        private ToolStripMenuItem toolStripAddTaks;
        private ContextMenuStrip contextMenuStripDevicesNull;
        private ToolStripMenuItem repollAllDevicesToolStripMenuItem;
        private System.Diagnostics.Process process1;
        private TabControl MainTabControl;
        private TabPage tabPage5;
        private SplitContainer splitContainer3;
        private BrightIdeasSoftware.DataListView dataListViewDevices;
        private BrightIdeasSoftware.OLVColumn IDCol;
        private BrightIdeasSoftware.OLVColumn NCol;
        private BrightIdeasSoftware.OLVColumn TCol;
        private BrightIdeasSoftware.OLVColumn MeterCol;
        private BrightIdeasSoftware.OLVColumn LevelCol;
        private BrightIdeasSoftware.OLVColumn nodeID;
        private BrightIdeasSoftware.OLVColumn GroupsCol;
        private BrightIdeasSoftware.OLVColumn colLastHeardFrom;
        private TabPage tabPage4;
        private SplitContainer splitContainer2;
        private BrightIdeasSoftware.DataListView dataListTasks;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn EnabledCol;
        private BrightIdeasSoftware.OLVColumn FreqCol;
        private Label label5;
        private Label label6;
        private GroupBox groupBox_Daily;
        private Label label19;
        private Label label20;
        private TextBox textBox_TaskName;
        private CheckBox checkBox_EnabledTask;
        private Button button_SaveTask;
        private Label label18;
        private ComboBox comboBox_ActionsTask;
        private DateTimePicker dateTimePickerStartTask;
        private ComboBox comboBox_FrequencyTask;
        private Label label17;
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
        private TabPage tabPage1;
        private BrightIdeasSoftware.DataListView dataListTriggers;
        private BrightIdeasSoftware.OLVColumn eventNameCol;
        private BrightIdeasSoftware.OLVColumn triggerFriendlyName;
        private TabPage tabPage2;
        private BrightIdeasSoftware.DataListView dataListViewLog;
        private BrightIdeasSoftware.OLVColumn dateTimeCol;
        private BrightIdeasSoftware.OLVColumn urgencyColu;
        private BrightIdeasSoftware.OLVColumn SourceCol;
        private BrightIdeasSoftware.OLVColumn descCol;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private TabPage tabPage3;
        private SplitContainer splitContainer4;
        private BrightIdeasSoftware.DataListView dataListViewScenes;
        private BrightIdeasSoftware.DataListView dataListViewSceneCMDs;
        private BrightIdeasSoftware.OLVColumn colScene;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private Label lbl_sceneActions;
        private ContextMenuStrip cmsSceneCMD;
        private ToolStripMenuItem deleteCommandToolStripMenuItem;
        private ToolStripMenuItem editCommandToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem pluginsToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem groupsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem editGroupsToolStripMenuItem;
        private ToolStripMenuItem acitvateGroupsToolStripMenuItem;
        private ToolStripMenuItem builtInCommandsToolStripMenuItem;
        private ToolStripMenuItem repollAllToolStripMenuItem;
        private ToolStripMenuItem duplicateSceneToolStripMenuItem;
        private GroupBox groupBox_Seconds;
        private NumericUpDown numericUpDownOccurSeconds;
        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDownOccurDays;
        private NumericUpDown numericUpDownOccurWeeks;
        private ImageList imageList25Icons;
        private BrightIdeasSoftware.DataListView dataListViewDeviceSmallList;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private Button btnAddBltInCMD;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private Label label3;
        private PictureBox pictureBox1;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStripTrigger;
        private ToolStripMenuItem addToolStripMenuItem1;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem deleteEventToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn olvColumnEnabled;
        private ContextMenuStrip contextMenuStripTriggerNull;
        private ToolStripMenuItem toolStripMenuItemEventsNull;
        private ContextMenuStrip contextMenuStripnotifyIcon;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem exitZVSToolStripMenuItem;
        private ToolStripMenuItem showZVirtualSceneToolStripMenuItem;
        private GroupBox groupBox_Montly;
        private NumericUpDown numericUpDownOccurMonths;
        private CheckBox checkBoxDay05;
        private CheckBox checkBoxDay04;
        private CheckBox checkBoxDay03;
        private CheckBox checkBoxDay02;
        private CheckBox checkBoxDay01;
        private Label label4;
        private Label label8;
        private CheckBox checkBoxDay08;
        private CheckBox checkBoxDay07;
        private CheckBox checkBoxDay06;
        private CheckBox checkBoxDay31;
        private CheckBox checkBoxDay30;
        private CheckBox checkBoxDay29;
        private CheckBox checkBoxDay28;
        private CheckBox checkBoxDay27;
        private CheckBox checkBoxDay26;
        private CheckBox checkBoxDay25;
        private CheckBox checkBoxDay24;
        private CheckBox checkBoxDay23;
        private CheckBox checkBoxDay22;
        private CheckBox checkBoxDay21;
        private CheckBox checkBoxDay20;
        private CheckBox checkBoxDay19;
        private CheckBox checkBoxDay18;
        private CheckBox checkBoxDay17;
        private CheckBox checkBoxDay16;
        private CheckBox checkBoxDay15;
        private CheckBox checkBoxDay14;
        private CheckBox checkBoxDay13;
        private CheckBox checkBoxDay12;
        private CheckBox checkBoxDay11;
        private CheckBox checkBoxDay10;
        private CheckBox checkBoxDay09;
        private Button btn_even;
        private Button btn_odd;
        private Button btn_clear;
        private ToolStripMenuItem createEventAdvancedToolStripMenuItem;
        private UserControls.uc_device_values uc_device_values1;
        private ToolStripMenuItem viewLogsAndDatabaseToolStripMenuItem;
        private ToolStripMenuItem viewDatabaseToolStripMenuItem;
    }
}

