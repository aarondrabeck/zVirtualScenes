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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataListEvents = new BrightIdeasSoftware.DataListView();
            this.eventNameCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.eventObjName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.triggerName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnEditEvent = new System.Windows.Forms.Button();
            this.btnDeleteEvent = new System.Windows.Forms.Button();
            this.btnNewEvent = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataListTasks = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.FreqCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.EnabledCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageList25Icons = new System.Windows.Forms.ImageList(this.components);
            this.groupBox_Seconds = new System.Windows.Forms.GroupBox();
            this.numericUpDownOccurSeconds = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
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
            this.uc_object_values1 = new zVirtualScenesApplication.UserControls.uc_object_values();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.dataListViewScenes = new BrightIdeasSoftware.DataListView();
            this.colScene = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnAddBltInCMD = new System.Windows.Forms.Button();
            this.btnAddObjCMD = new System.Windows.Forms.Button();
            this.lbl_sceneActions = new System.Windows.Forms.Label();
            this.dataListViewSceneCMDs = new BrightIdeasSoftware.DataListView();
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmsSceneCMD = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSceneCMDnull = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addCommandToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.builtInCommandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repollAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acitvateGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entireDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.pluginDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectPropertyDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aaronRestoreNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripScenes.SuspendLayout();
            this.contextMenuStripScenesNull.SuspendLayout();
            this.contextMenuStripActions.SuspendLayout();
            this.contextMenuStripTasks.SuspendLayout();
            this.contextMenuStripTasksNull.SuspendLayout();
            this.contextMenuStripDevicesNull.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewLog)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListEvents)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).BeginInit();
            this.groupBox_Seconds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox_Daily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurDays)).BeginInit();
            this.groupBox_Weekly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurWeeks)).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewSceneCMDs)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.cmsSceneCMD.SuspendLayout();
            this.cmsSceneCMDnull.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.contextMenuStripScenes.Size = new System.Drawing.Size(162, 120);
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
            this.tabPage2.Size = new System.Drawing.Size(1000, 673);
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
            this.dataListViewLog.Size = new System.Drawing.Size(994, 667);
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
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1000, 673);
            this.tabPage1.TabIndex = 6;
            this.tabPage1.Text = "Events";
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
            this.splitContainer1.Panel1.Controls.Add(this.dataListEvents);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnEditEvent);
            this.splitContainer1.Panel2.Controls.Add(this.btnDeleteEvent);
            this.splitContainer1.Panel2.Controls.Add(this.btnNewEvent);
            this.splitContainer1.Size = new System.Drawing.Size(994, 667);
            this.splitContainer1.SplitterDistance = 616;
            this.splitContainer1.TabIndex = 1;
            // 
            // dataListEvents
            // 
            this.dataListEvents.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.dataListEvents.AllColumns.Add(this.eventNameCol);
            this.dataListEvents.AllColumns.Add(this.eventObjName);
            this.dataListEvents.AllColumns.Add(this.triggerName);
            this.dataListEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.eventNameCol,
            this.eventObjName,
            this.triggerName});
            this.dataListEvents.DataSource = null;
            this.dataListEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListEvents.HasCollapsibleGroups = false;
            this.dataListEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListEvents.Location = new System.Drawing.Point(0, 0);
            this.dataListEvents.Name = "dataListEvents";
            this.dataListEvents.ShowGroups = false;
            this.dataListEvents.Size = new System.Drawing.Size(994, 616);
            this.dataListEvents.TabIndex = 0;
            this.dataListEvents.UseCompatibleStateImageBehavior = false;
            this.dataListEvents.View = System.Windows.Forms.View.Details;
            // 
            // eventNameCol
            // 
            this.eventNameCol.AspectName = "txt_event_name";
            this.eventNameCol.Text = "Name";
            this.eventNameCol.Width = 200;
            // 
            // eventObjName
            // 
            this.eventObjName.AspectName = "txt_object_name";
            this.eventObjName.Text = "Object";
            this.eventObjName.Width = 200;
            // 
            // triggerName
            // 
            this.triggerName.AspectName = "txt_event_name";
            this.triggerName.Text = "Event";
            this.triggerName.Width = 200;
            // 
            // btnEditEvent
            // 
            this.btnEditEvent.Location = new System.Drawing.Point(106, 3);
            this.btnEditEvent.Name = "btnEditEvent";
            this.btnEditEvent.Size = new System.Drawing.Size(95, 23);
            this.btnEditEvent.TabIndex = 2;
            this.btnEditEvent.Text = "Edit Event";
            this.btnEditEvent.UseVisualStyleBackColor = true;
            this.btnEditEvent.Click += new System.EventHandler(this.btnEditEvent_Click);
            // 
            // btnDeleteEvent
            // 
            this.btnDeleteEvent.Location = new System.Drawing.Point(207, 3);
            this.btnDeleteEvent.Name = "btnDeleteEvent";
            this.btnDeleteEvent.Size = new System.Drawing.Size(95, 23);
            this.btnDeleteEvent.TabIndex = 1;
            this.btnDeleteEvent.Text = "Delete Event";
            this.btnDeleteEvent.UseVisualStyleBackColor = true;
            this.btnDeleteEvent.Click += new System.EventHandler(this.btnDeleteEvent_Click);
            // 
            // btnNewEvent
            // 
            this.btnNewEvent.Location = new System.Drawing.Point(5, 3);
            this.btnNewEvent.Name = "btnNewEvent";
            this.btnNewEvent.Size = new System.Drawing.Size(95, 23);
            this.btnNewEvent.TabIndex = 0;
            this.btnNewEvent.Text = "New Event";
            this.btnNewEvent.UseVisualStyleBackColor = true;
            this.btnNewEvent.Click += new System.EventHandler(this.btnNewEvent_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1000, 673);
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
            this.splitContainer2.Panel2.Controls.Add(this.groupBox_Seconds);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
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
            this.splitContainer2.Panel2.Controls.Add(this.groupBox_Weekly);
            this.splitContainer2.Panel2MinSize = 553;
            this.splitContainer2.Size = new System.Drawing.Size(994, 667);
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
            this.dataListTasks.Size = new System.Drawing.Size(433, 663);
            this.dataListTasks.SmallImageList = this.imageList25Icons;
            this.dataListTasks.TabIndex = 46;
            this.dataListTasks.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dataListTasks.UnfocusedHighlightForegroundColor = System.Drawing.Color.Black;
            this.dataListTasks.UseCompatibleStateImageBehavior = false;
            this.dataListTasks.View = System.Windows.Forms.View.Details;
            this.dataListTasks.SelectedIndexChanged += new System.EventHandler(this.dataListTasks_SelectedIndexChanged_1);
            this.dataListTasks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataListTasks_KeyDown);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "ToString";
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
            this.EnabledCol.AspectName = "isEnabled";
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
            // groupBox_Seconds
            // 
            this.groupBox_Seconds.Controls.Add(this.numericUpDownOccurSeconds);
            this.groupBox_Seconds.Controls.Add(this.label1);
            this.groupBox_Seconds.Controls.Add(this.label2);
            this.groupBox_Seconds.Location = new System.Drawing.Point(17, 142);
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
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(404, 161);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(137, 130);
            this.pictureBox1.TabIndex = 44;
            this.pictureBox1.TabStop = false;
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
            this.groupBox_Daily.Location = new System.Drawing.Point(17, 142);
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
            this.checkBox_EnabledTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_EnabledTask.AutoSize = true;
            this.checkBox_EnabledTask.Location = new System.Drawing.Point(446, 14);
            this.checkBox_EnabledTask.Name = "checkBox_EnabledTask";
            this.checkBox_EnabledTask.Size = new System.Drawing.Size(92, 17);
            this.checkBox_EnabledTask.TabIndex = 39;
            this.checkBox_EnabledTask.Text = "Task Enabled";
            this.checkBox_EnabledTask.UseVisualStyleBackColor = true;
            // 
            // button_SaveTask
            // 
            this.button_SaveTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveTask.Location = new System.Drawing.Point(444, 631);
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
            this.groupBox_Weekly.Location = new System.Drawing.Point(17, 142);
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
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer3);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1000, 673);
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
            this.splitContainer3.Panel2.Controls.Add(this.uc_object_values1);
            this.splitContainer3.Panel2MinSize = 200;
            this.splitContainer3.Size = new System.Drawing.Size(994, 667);
            this.splitContainer3.SplitterDistance = 429;
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
            this.dataListViewDevices.EmptyListMsg = "Objects Loading...";
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
            this.dataListViewDevices.Size = new System.Drawing.Size(992, 427);
            this.dataListViewDevices.SmallImageList = this.imageListActionTypesSmall;
            this.dataListViewDevices.SortGroupItemsByPrimaryColumn = false;
            this.dataListViewDevices.TabIndex = 0;
            this.dataListViewDevices.UseCompatibleStateImageBehavior = false;
            this.dataListViewDevices.View = System.Windows.Forms.View.Details;
            this.dataListViewDevices.ItemsChanging += new System.EventHandler<BrightIdeasSoftware.ItemsChangingEventArgs>(this.dataListViewDevices_ItemsChanging);
            this.dataListViewDevices.SelectedIndexChanged += new System.EventHandler(this.dataListViewDevices_SelectedIndexChanged);
            this.dataListViewDevices.DoubleClick += new System.EventHandler(this.dataListViewDevices_DoubleClick);
            // 
            // nodeID
            // 
            this.nodeID.AspectName = "Node_ID";
            this.nodeID.DisplayIndex = 6;
            this.nodeID.Text = "Node";
            this.nodeID.Width = 40;
            // 
            // IDCol
            // 
            this.IDCol.AspectName = "ID";
            this.IDCol.DisplayIndex = 0;
            this.IDCol.ImageAspectName = "DeviceIcon";
            this.IDCol.IsEditable = false;
            this.IDCol.Text = "ID";
            this.IDCol.Width = 40;
            // 
            // NCol
            // 
            this.NCol.AspectName = "Name";
            this.NCol.DisplayIndex = 1;
            this.NCol.Text = "Name";
            this.NCol.Width = 150;
            // 
            // TCol
            // 
            this.TCol.AspectName = "Type";
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
            this.GroupsCol.AspectName = "Groups";
            this.GroupsCol.DisplayIndex = 5;
            this.GroupsCol.Text = "Groups / Zones";
            this.GroupsCol.Width = 200;
            // 
            // colLastHeardFrom
            // 
            this.colLastHeardFrom.AspectName = "LastHeardFrom";
            this.colLastHeardFrom.Text = "Last Heard From";
            this.colLastHeardFrom.Width = 200;
            // 
            // uc_object_values1
            // 
            this.uc_object_values1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc_object_values1.Location = new System.Drawing.Point(0, 0);
            this.uc_object_values1.Name = "uc_object_values1";
            this.uc_object_values1.Size = new System.Drawing.Size(992, 232);
            this.uc_object_values1.TabIndex = 0;
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
            this.MainTabControl.Size = new System.Drawing.Size(1008, 699);
            this.MainTabControl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1000, 673);
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
            this.splitContainer4.Panel2.Controls.Add(this.btnAddBltInCMD);
            this.splitContainer4.Panel2.Controls.Add(this.btnAddObjCMD);
            this.splitContainer4.Panel2.Controls.Add(this.lbl_sceneActions);
            this.splitContainer4.Panel2.Controls.Add(this.dataListViewSceneCMDs);
            this.splitContainer4.Size = new System.Drawing.Size(994, 667);
            this.splitContainer4.SplitterDistance = 331;
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
            this.dataListViewScenes.SelectColumnsOnRightClick = false;
            this.dataListViewScenes.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.dataListViewScenes.ShowGroups = false;
            this.dataListViewScenes.Size = new System.Drawing.Size(331, 667);
            this.dataListViewScenes.SmallImageList = this.imageList25Icons;
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
            this.colScene.AspectName = "txt_name";
            this.colScene.FillsFreeSpace = true;
            this.colScene.ImageAspectName = "DeviceIcon";
            this.colScene.Text = "Scenes";
            // 
            // btnAddBltInCMD
            // 
            this.btnAddBltInCMD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBltInCMD.Location = new System.Drawing.Point(393, 639);
            this.btnAddBltInCMD.Name = "btnAddBltInCMD";
            this.btnAddBltInCMD.Size = new System.Drawing.Size(131, 23);
            this.btnAddBltInCMD.TabIndex = 3;
            this.btnAddBltInCMD.Text = "Add Builtin Command";
            this.btnAddBltInCMD.UseVisualStyleBackColor = true;
            this.btnAddBltInCMD.Click += new System.EventHandler(this.btnAddBltInCMD_Click);
            // 
            // btnAddObjCMD
            // 
            this.btnAddObjCMD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddObjCMD.Location = new System.Drawing.Point(530, 639);
            this.btnAddObjCMD.Name = "btnAddObjCMD";
            this.btnAddObjCMD.Size = new System.Drawing.Size(124, 23);
            this.btnAddObjCMD.TabIndex = 2;
            this.btnAddObjCMD.Text = "Add Object Command";
            this.btnAddObjCMD.UseVisualStyleBackColor = true;
            this.btnAddObjCMD.Click += new System.EventHandler(this.btnAddCMD_Click);
            // 
            // lbl_sceneActions
            // 
            this.lbl_sceneActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_sceneActions.Location = new System.Drawing.Point(3, 6);
            this.lbl_sceneActions.Name = "lbl_sceneActions";
            this.lbl_sceneActions.Size = new System.Drawing.Size(651, 15);
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
            this.dataListViewSceneCMDs.Size = new System.Drawing.Size(651, 611);
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
            this.olvColumn2.AspectName = "ObjectName";
            this.olvColumn2.ImageAspectName = "DeviceIcon";
            this.olvColumn2.Text = "Command / Object";
            this.olvColumn2.Width = 200;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "CMDName";
            this.olvColumn3.FillsFreeSpace = true;
            this.olvColumn3.Text = "";
            this.olvColumn3.Width = 200;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 726);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
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
            this.addCommandToolStripMenuItem,
            this.editCommandToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteCommandToolStripMenuItem});
            this.cmsSceneCMD.Name = "cmsSceneCMD";
            this.cmsSceneCMD.Size = new System.Drawing.Size(168, 76);
            // 
            // addCommandToolStripMenuItem
            // 
            this.addCommandToolStripMenuItem.Name = "addCommandToolStripMenuItem";
            this.addCommandToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.addCommandToolStripMenuItem.Text = "&Add Command";
            this.addCommandToolStripMenuItem.Click += new System.EventHandler(this.addCommandToolStripMenuItem_Click);
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
            // cmsSceneCMDnull
            // 
            this.cmsSceneCMDnull.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCommandToolStripMenuItem1});
            this.cmsSceneCMDnull.Name = "cmsSceneCMDnull";
            this.cmsSceneCMDnull.Size = new System.Drawing.Size(157, 26);
            // 
            // addCommandToolStripMenuItem1
            // 
            this.addCommandToolStripMenuItem1.Name = "addCommandToolStripMenuItem1";
            this.addCommandToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.addCommandToolStripMenuItem1.Text = "&Add Command";
            this.addCommandToolStripMenuItem1.Click += new System.EventHandler(this.addCommandToolStripMenuItem1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.builtInCommandsToolStripMenuItem,
            this.groupsToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(89, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
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
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearDataToolStripMenuItem,
            this.toolStripSeparator7,
            this.setupToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.databaseToolStripMenuItem.Text = "&Database";
            // 
            // clearDataToolStripMenuItem
            // 
            this.clearDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.entireDatabaseToolStripMenuItem,
            this.toolStripSeparator6,
            this.pluginDataToolStripMenuItem,
            this.objectPropertyDataToolStripMenuItem,
            this.commandDataToolStripMenuItem,
            this.otherToolStripMenuItem});
            this.clearDataToolStripMenuItem.Name = "clearDataToolStripMenuItem";
            this.clearDataToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.clearDataToolStripMenuItem.Text = "&Clear Data";
            // 
            // entireDatabaseToolStripMenuItem
            // 
            this.entireDatabaseToolStripMenuItem.Name = "entireDatabaseToolStripMenuItem";
            this.entireDatabaseToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.entireDatabaseToolStripMenuItem.Text = "&All Data";
            this.entireDatabaseToolStripMenuItem.Click += new System.EventHandler(this.entireDatabaseToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(181, 6);
            // 
            // pluginDataToolStripMenuItem
            // 
            this.pluginDataToolStripMenuItem.Name = "pluginDataToolStripMenuItem";
            this.pluginDataToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.pluginDataToolStripMenuItem.Text = "&Plugin Data";
            this.pluginDataToolStripMenuItem.Click += new System.EventHandler(this.pluginDataToolStripMenuItem_Click);
            // 
            // objectPropertyDataToolStripMenuItem
            // 
            this.objectPropertyDataToolStripMenuItem.Name = "objectPropertyDataToolStripMenuItem";
            this.objectPropertyDataToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.objectPropertyDataToolStripMenuItem.Text = "&Object Property Data";
            this.objectPropertyDataToolStripMenuItem.Click += new System.EventHandler(this.objectPropertyDataToolStripMenuItem_Click);
            // 
            // commandDataToolStripMenuItem
            // 
            this.commandDataToolStripMenuItem.Name = "commandDataToolStripMenuItem";
            this.commandDataToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.commandDataToolStripMenuItem.Text = "&Command Data";
            this.commandDataToolStripMenuItem.Click += new System.EventHandler(this.commandDataToolStripMenuItem_Click);
            // 
            // otherToolStripMenuItem
            // 
            this.otherToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aaronRestoreNamesToolStripMenuItem});
            this.otherToolStripMenuItem.Name = "otherToolStripMenuItem";
            this.otherToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.otherToolStripMenuItem.Text = "&Other";
            // 
            // aaronRestoreNamesToolStripMenuItem
            // 
            this.aaronRestoreNamesToolStripMenuItem.Name = "aaronRestoreNamesToolStripMenuItem";
            this.aaronRestoreNamesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.aaronRestoreNamesToolStripMenuItem.Text = "Aaron Restore Names";
            this.aaronRestoreNamesToolStripMenuItem.Click += new System.EventHandler(this.aaronRestoreNamesToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(125, 6);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.setupToolStripMenuItem.Text = "&Setup";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 748);
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
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.contextMenuStripScenes.ResumeLayout(false);
            this.contextMenuStripScenesNull.ResumeLayout(false);
            this.contextMenuStripActions.ResumeLayout(false);
            this.contextMenuStripTasks.ResumeLayout(false);
            this.contextMenuStripTasksNull.ResumeLayout(false);
            this.contextMenuStripDevicesNull.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewLog)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListEvents)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).EndInit();
            this.groupBox_Seconds.ResumeLayout(false);
            this.groupBox_Seconds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox_Daily.ResumeLayout(false);
            this.groupBox_Daily.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurDays)).EndInit();
            this.groupBox_Weekly.ResumeLayout(false);
            this.groupBox_Weekly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurWeeks)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewSceneCMDs)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cmsSceneCMD.ResumeLayout(false);
            this.cmsSceneCMDnull.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private UserControls.uc_object_values uc_object_values1;
        private TabPage tabPage4;
        private SplitContainer splitContainer2;
        private BrightIdeasSoftware.DataListView dataListTasks;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn EnabledCol;
        private BrightIdeasSoftware.OLVColumn FreqCol;
        private PictureBox pictureBox1;
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
        private SplitContainer splitContainer1;
        private BrightIdeasSoftware.DataListView dataListEvents;
        private BrightIdeasSoftware.OLVColumn eventNameCol;
        private BrightIdeasSoftware.OLVColumn eventObjName;
        private BrightIdeasSoftware.OLVColumn triggerName;
        private Button btnEditEvent;
        private Button btnDeleteEvent;
        private Button btnNewEvent;
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
        private Button btnAddObjCMD;
        private ContextMenuStrip cmsSceneCMD;
        private ToolStripMenuItem addCommandToolStripMenuItem;
        private ToolStripMenuItem deleteCommandToolStripMenuItem;
        private ToolStripMenuItem editCommandToolStripMenuItem;
        private ContextMenuStrip cmsSceneCMDnull;
        private ToolStripMenuItem addCommandToolStripMenuItem1;
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
        private ToolStripMenuItem databaseToolStripMenuItem;
        private ToolStripMenuItem clearDataToolStripMenuItem;
        private ToolStripMenuItem entireDatabaseToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem pluginDataToolStripMenuItem;
        private ToolStripMenuItem objectPropertyDataToolStripMenuItem;
        private ToolStripMenuItem commandDataToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem setupToolStripMenuItem;
        private ToolStripMenuItem builtInCommandsToolStripMenuItem;
        private ToolStripMenuItem repollAllToolStripMenuItem;
        private ToolStripMenuItem otherToolStripMenuItem;
        private ToolStripMenuItem aaronRestoreNamesToolStripMenuItem;
        private Button btnAddBltInCMD;
        private ToolStripMenuItem duplicateSceneToolStripMenuItem;
        private GroupBox groupBox_Seconds;
        private NumericUpDown numericUpDownOccurSeconds;
        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDownOccurDays;
        private NumericUpDown numericUpDownOccurWeeks;
        private ImageList imageList25Icons;
    }
}

