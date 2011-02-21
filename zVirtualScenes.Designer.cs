namespace zVirtualScenesApplication
{
    partial class zVirtualScenes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(zVirtualScenes));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_sceneMoveDown = new System.Windows.Forms.Button();
            this.btn_SceneMoveUp = new System.Windows.Forms.Button();
            this.buttonDelScene = new System.Windows.Forms.Button();
            this.buttonAddScene = new System.Windows.Forms.Button();
            this.btn_MoveDown = new System.Windows.Forms.Button();
            this.btn_MoveUp = new System.Windows.Forms.Button();
            this.btn_AddtoScene = new System.Windows.Forms.Button();
            this.scenename_txtbx = new System.Windows.Forms.TextBox();
            this.btn_DelAction = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxScenes = new System.Windows.Forms.ListBox();
            this.lbl_sceneActions = new System.Windows.Forms.Label();
            this.listBoxSceneActions = new System.Windows.Forms.ListBox();
            this.SaveScene_btn = new System.Windows.Forms.Button();
            this.btn_runScene = new System.Windows.Forms.Button();
            this.groupBoxCommands = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtbox_level = new System.Windows.Forms.TextBox();
            this.btn_RunCommand = new System.Windows.Forms.Button();
            this.groupBoxtTemp = new System.Windows.Forms.GroupBox();
            this.checkBoxeditHP = new System.Windows.Forms.CheckBox();
            this.checkBoxeditCP = new System.Windows.Forms.CheckBox();
            this.labelCurrentTemp = new System.Windows.Forms.Label();
            this.textBoxCoolPoint = new System.Windows.Forms.TextBox();
            this.comboBoxEnergyMode = new System.Windows.Forms.ComboBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxFanMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtbx_HeatPoint = new System.Windows.Forms.TextBox();
            this.comboBoxHeatCoolMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtb_deviceName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_SetDeviceName = new System.Windows.Forms.Button();
            this.btn_RefreshDevices = new System.Windows.Forms.Button();
            this.listBoxDevices = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxLSLimit = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxLSport = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxLSPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxLSDebugVerbose = new System.Windows.Forms.CheckBox();
            this.checkBoxLSEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtb_exampleURL = new System.Windows.Forms.TextBox();
            this.checkBoxHTTPEnable = new System.Windows.Forms.CheckBox();
            this.label1HTTPmoreinfo = new System.Windows.Forms.Label();
            this.txtb_httpPort = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_SaveSettings = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.MainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxCommands.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxtTemp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Controls.Add(this.tabPage3);
            this.MainTabControl.Controls.Add(this.tabPage2);
            this.MainTabControl.Location = new System.Drawing.Point(2, 1);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(785, 624);
            this.MainTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(777, 598);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Scenes";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBoxCommands);
            this.groupBox1.Controls.Add(this.txtb_deviceName);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btn_SetDeviceName);
            this.groupBox1.Controls.Add(this.btn_RefreshDevices);
            this.groupBox1.Controls.Add(this.listBoxDevices);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(765, 588);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ZWave Devices";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_sceneMoveDown);
            this.groupBox2.Controls.Add(this.btn_SceneMoveUp);
            this.groupBox2.Controls.Add(this.buttonDelScene);
            this.groupBox2.Controls.Add(this.buttonAddScene);
            this.groupBox2.Controls.Add(this.btn_MoveDown);
            this.groupBox2.Controls.Add(this.btn_MoveUp);
            this.groupBox2.Controls.Add(this.scenename_txtbx);
            this.groupBox2.Controls.Add(this.btn_DelAction);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.listBoxScenes);
            this.groupBox2.Controls.Add(this.lbl_sceneActions);
            this.groupBox2.Controls.Add(this.listBoxSceneActions);
            this.groupBox2.Controls.Add(this.SaveScene_btn);
            this.groupBox2.Controls.Add(this.btn_runScene);
            this.groupBox2.Location = new System.Drawing.Point(0, 382);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(760, 210);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scenes";
            // 
            // btn_sceneMoveDown
            // 
            this.btn_sceneMoveDown.Location = new System.Drawing.Point(182, 64);
            this.btn_sceneMoveDown.Name = "btn_sceneMoveDown";
            this.btn_sceneMoveDown.Size = new System.Drawing.Size(17, 20);
            this.btn_sceneMoveDown.TabIndex = 20;
            this.btn_sceneMoveDown.Text = "-";
            this.btn_sceneMoveDown.UseVisualStyleBackColor = true;
            this.btn_sceneMoveDown.Click += new System.EventHandler(this.btn_sceneMoveDown_Click);
            // 
            // btn_SceneMoveUp
            // 
            this.btn_SceneMoveUp.Location = new System.Drawing.Point(182, 41);
            this.btn_SceneMoveUp.Name = "btn_SceneMoveUp";
            this.btn_SceneMoveUp.Size = new System.Drawing.Size(17, 20);
            this.btn_SceneMoveUp.TabIndex = 19;
            this.btn_SceneMoveUp.Text = "+";
            this.btn_SceneMoveUp.UseVisualStyleBackColor = true;
            this.btn_SceneMoveUp.Click += new System.EventHandler(this.btn_SceneMoveUp_Click);
            // 
            // buttonDelScene
            // 
            this.buttonDelScene.Location = new System.Drawing.Point(87, 178);
            this.buttonDelScene.Name = "buttonDelScene";
            this.buttonDelScene.Size = new System.Drawing.Size(92, 20);
            this.buttonDelScene.TabIndex = 18;
            this.buttonDelScene.Text = "Delete Scene";
            this.buttonDelScene.UseVisualStyleBackColor = true;
            this.buttonDelScene.Click += new System.EventHandler(this.buttonDelScene_Click);
            // 
            // buttonAddScene
            // 
            this.buttonAddScene.Location = new System.Drawing.Point(6, 178);
            this.buttonAddScene.Name = "buttonAddScene";
            this.buttonAddScene.Size = new System.Drawing.Size(75, 20);
            this.buttonAddScene.TabIndex = 17;
            this.buttonAddScene.Text = "Add Scene";
            this.buttonAddScene.UseVisualStyleBackColor = true;
            this.buttonAddScene.Click += new System.EventHandler(this.buttonAddScene_Click);
            // 
            // btn_MoveDown
            // 
            this.btn_MoveDown.Location = new System.Drawing.Point(739, 76);
            this.btn_MoveDown.Name = "btn_MoveDown";
            this.btn_MoveDown.Size = new System.Drawing.Size(17, 20);
            this.btn_MoveDown.TabIndex = 16;
            this.btn_MoveDown.Text = "-";
            this.btn_MoveDown.UseVisualStyleBackColor = true;
            this.btn_MoveDown.Click += new System.EventHandler(this.btn_MoveDown_Click);
            // 
            // btn_MoveUp
            // 
            this.btn_MoveUp.Location = new System.Drawing.Point(739, 54);
            this.btn_MoveUp.Name = "btn_MoveUp";
            this.btn_MoveUp.Size = new System.Drawing.Size(17, 20);
            this.btn_MoveUp.TabIndex = 15;
            this.btn_MoveUp.Text = "+";
            this.btn_MoveUp.UseVisualStyleBackColor = true;
            this.btn_MoveUp.Click += new System.EventHandler(this.btn_MoveUp_Click);
            // 
            // btn_AddtoScene
            // 
            this.btn_AddtoScene.Location = new System.Drawing.Point(462, 132);
            this.btn_AddtoScene.Name = "btn_AddtoScene";
            this.btn_AddtoScene.Size = new System.Drawing.Size(75, 54);
            this.btn_AddtoScene.TabIndex = 9;
            this.btn_AddtoScene.Text = "Add This Action to Scene";
            this.btn_AddtoScene.UseVisualStyleBackColor = true;
            this.btn_AddtoScene.Click += new System.EventHandler(this.btn_AddtoScene_Click);
            // 
            // scenename_txtbx
            // 
            this.scenename_txtbx.Location = new System.Drawing.Point(78, 13);
            this.scenename_txtbx.Name = "scenename_txtbx";
            this.scenename_txtbx.Size = new System.Drawing.Size(133, 20);
            this.scenename_txtbx.TabIndex = 11;
            // 
            // btn_DelAction
            // 
            this.btn_DelAction.Location = new System.Drawing.Point(657, 178);
            this.btn_DelAction.Name = "btn_DelAction";
            this.btn_DelAction.Size = new System.Drawing.Size(81, 20);
            this.btn_DelAction.TabIndex = 14;
            this.btn_DelAction.Text = "Delete Action";
            this.btn_DelAction.UseVisualStyleBackColor = true;
            this.btn_DelAction.Click += new System.EventHandler(this.btn_DelAction_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Scene Name: ";
            // 
            // listBoxScenes
            // 
            this.listBoxScenes.FormattingEnabled = true;
            this.listBoxScenes.Location = new System.Drawing.Point(7, 41);
            this.listBoxScenes.Name = "listBoxScenes";
            this.listBoxScenes.Size = new System.Drawing.Size(172, 134);
            this.listBoxScenes.TabIndex = 3;
            this.listBoxScenes.SelectedIndexChanged += new System.EventHandler(this.listBoxScenes_SelectedIndexChanged);
            // 
            // lbl_sceneActions
            // 
            this.lbl_sceneActions.AutoSize = true;
            this.lbl_sceneActions.Location = new System.Drawing.Point(206, 38);
            this.lbl_sceneActions.Name = "lbl_sceneActions";
            this.lbl_sceneActions.Size = new System.Drawing.Size(76, 13);
            this.lbl_sceneActions.TabIndex = 6;
            this.lbl_sceneActions.Text = "Scene Actions";
            // 
            // listBoxSceneActions
            // 
            this.listBoxSceneActions.FormattingEnabled = true;
            this.listBoxSceneActions.Location = new System.Drawing.Point(208, 54);
            this.listBoxSceneActions.Name = "listBoxSceneActions";
            this.listBoxSceneActions.Size = new System.Drawing.Size(528, 121);
            this.listBoxSceneActions.TabIndex = 5;
            // 
            // SaveScene_btn
            // 
            this.SaveScene_btn.Location = new System.Drawing.Point(220, 13);
            this.SaveScene_btn.Name = "SaveScene_btn";
            this.SaveScene_btn.Size = new System.Drawing.Size(47, 20);
            this.SaveScene_btn.TabIndex = 12;
            this.SaveScene_btn.Text = "Set";
            this.SaveScene_btn.UseVisualStyleBackColor = true;
            this.SaveScene_btn.Click += new System.EventHandler(this.SaveScene_btn_Click);
            // 
            // btn_runScene
            // 
            this.btn_runScene.Location = new System.Drawing.Point(652, 12);
            this.btn_runScene.Name = "btn_runScene";
            this.btn_runScene.Size = new System.Drawing.Size(102, 20);
            this.btn_runScene.TabIndex = 10;
            this.btn_runScene.Text = "Run Scene Now";
            this.btn_runScene.UseVisualStyleBackColor = true;
            this.btn_runScene.Click += new System.EventHandler(this.btn_runScene_Click);
            // 
            // groupBoxCommands
            // 
            this.groupBoxCommands.Controls.Add(this.groupBox3);
            this.groupBoxCommands.Controls.Add(this.btn_RunCommand);
            this.groupBoxCommands.Controls.Add(this.groupBoxtTemp);
            this.groupBoxCommands.Controls.Add(this.btn_AddtoScene);
            this.groupBoxCommands.Location = new System.Drawing.Point(9, 184);
            this.groupBoxCommands.Name = "groupBoxCommands";
            this.groupBoxCommands.Size = new System.Drawing.Size(747, 192);
            this.groupBoxCommands.TabIndex = 20;
            this.groupBoxCommands.TabStop = false;
            this.groupBoxCommands.Text = "Create Action";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtbox_level);
            this.groupBox3.Location = new System.Drawing.Point(6, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(141, 85);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Switches";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(63, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Dim = 0 to 99";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::zVirtualScenesApplication.Properties.Resources.switch_icon32;
            this.pictureBox1.InitialImage = global::zVirtualScenesApplication.Properties.Resources.switch_icon32;
            this.pictureBox1.Location = new System.Drawing.Point(6, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 36);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Level";
            // 
            // txtbox_level
            // 
            this.txtbox_level.Enabled = false;
            this.txtbox_level.Location = new System.Drawing.Point(82, 20);
            this.txtbox_level.Name = "txtbox_level";
            this.txtbox_level.Size = new System.Drawing.Size(51, 20);
            this.txtbox_level.TabIndex = 8;
            // 
            // btn_RunCommand
            // 
            this.btn_RunCommand.Location = new System.Drawing.Point(625, 12);
            this.btn_RunCommand.Name = "btn_RunCommand";
            this.btn_RunCommand.Size = new System.Drawing.Size(116, 20);
            this.btn_RunCommand.TabIndex = 15;
            this.btn_RunCommand.Text = "Run Action Now.";
            this.btn_RunCommand.UseVisualStyleBackColor = true;
            this.btn_RunCommand.Click += new System.EventHandler(this.btn_RunCommand_Click);
            // 
            // groupBoxtTemp
            // 
            this.groupBoxtTemp.Controls.Add(this.checkBoxeditHP);
            this.groupBoxtTemp.Controls.Add(this.checkBoxeditCP);
            this.groupBoxtTemp.Controls.Add(this.labelCurrentTemp);
            this.groupBoxtTemp.Controls.Add(this.textBoxCoolPoint);
            this.groupBoxtTemp.Controls.Add(this.comboBoxEnergyMode);
            this.groupBoxtTemp.Controls.Add(this.pictureBox2);
            this.groupBoxtTemp.Controls.Add(this.label7);
            this.groupBoxtTemp.Controls.Add(this.comboBoxFanMode);
            this.groupBoxtTemp.Controls.Add(this.label6);
            this.groupBoxtTemp.Controls.Add(this.txtbx_HeatPoint);
            this.groupBoxtTemp.Controls.Add(this.comboBoxHeatCoolMode);
            this.groupBoxtTemp.Controls.Add(this.label2);
            this.groupBoxtTemp.Location = new System.Drawing.Point(153, 21);
            this.groupBoxtTemp.Name = "groupBoxtTemp";
            this.groupBoxtTemp.Size = new System.Drawing.Size(303, 165);
            this.groupBoxtTemp.TabIndex = 14;
            this.groupBoxtTemp.TabStop = false;
            this.groupBoxtTemp.Text = "Thermostat";
            // 
            // checkBoxeditHP
            // 
            this.checkBoxeditHP.AutoSize = true;
            this.checkBoxeditHP.Enabled = false;
            this.checkBoxeditHP.Location = new System.Drawing.Point(173, 142);
            this.checkBoxeditHP.Name = "checkBoxeditHP";
            this.checkBoxeditHP.Size = new System.Drawing.Size(97, 17);
            this.checkBoxeditHP.TabIndex = 22;
            this.checkBoxeditHP.Text = "Edit Heat Point";
            this.checkBoxeditHP.UseVisualStyleBackColor = true;
            this.checkBoxeditHP.CheckedChanged += new System.EventHandler(this.checkBoxeditHP_CheckedChanged);
            // 
            // checkBoxeditCP
            // 
            this.checkBoxeditCP.AutoSize = true;
            this.checkBoxeditCP.Enabled = false;
            this.checkBoxeditCP.Location = new System.Drawing.Point(47, 142);
            this.checkBoxeditCP.Name = "checkBoxeditCP";
            this.checkBoxeditCP.Size = new System.Drawing.Size(95, 17);
            this.checkBoxeditCP.TabIndex = 21;
            this.checkBoxeditCP.Text = "Edit Cool Point";
            this.checkBoxeditCP.UseVisualStyleBackColor = true;
            this.checkBoxeditCP.CheckedChanged += new System.EventHandler(this.checkBoxeditCP_CheckedChanged);
            // 
            // labelCurrentTemp
            // 
            this.labelCurrentTemp.AutoSize = true;
            this.labelCurrentTemp.Location = new System.Drawing.Point(90, 13);
            this.labelCurrentTemp.Name = "labelCurrentTemp";
            this.labelCurrentTemp.Size = new System.Drawing.Size(110, 13);
            this.labelCurrentTemp.TabIndex = 20;
            this.labelCurrentTemp.Text = "Current Temperature: ";
            // 
            // textBoxCoolPoint
            // 
            this.textBoxCoolPoint.Enabled = false;
            this.textBoxCoolPoint.Location = new System.Drawing.Point(69, 116);
            this.textBoxCoolPoint.Name = "textBoxCoolPoint";
            this.textBoxCoolPoint.Size = new System.Drawing.Size(51, 20);
            this.textBoxCoolPoint.TabIndex = 19;
            // 
            // comboBoxEnergyMode
            // 
            this.comboBoxEnergyMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEnergyMode.Enabled = false;
            this.comboBoxEnergyMode.FormattingEnabled = true;
            this.comboBoxEnergyMode.Location = new System.Drawing.Point(104, 86);
            this.comboBoxEnergyMode.Name = "comboBoxEnergyMode";
            this.comboBoxEnergyMode.Size = new System.Drawing.Size(133, 21);
            this.comboBoxEnergyMode.TabIndex = 16;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::zVirtualScenesApplication.Properties.Resources.thermometer;
            this.pictureBox2.Location = new System.Drawing.Point(262, 16);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(35, 34);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(26, 92);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Energy Mode:";
            // 
            // comboBoxFanMode
            // 
            this.comboBoxFanMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFanMode.Enabled = false;
            this.comboBoxFanMode.FormattingEnabled = true;
            this.comboBoxFanMode.Items.AddRange(new object[] {
            "",
            "AutoLow",
            "OnLow"});
            this.comboBoxFanMode.Location = new System.Drawing.Point(105, 59);
            this.comboBoxFanMode.Name = "comboBoxFanMode";
            this.comboBoxFanMode.Size = new System.Drawing.Size(132, 21);
            this.comboBoxFanMode.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Fan Mode: ";
            // 
            // txtbx_HeatPoint
            // 
            this.txtbx_HeatPoint.Enabled = false;
            this.txtbx_HeatPoint.Location = new System.Drawing.Point(195, 116);
            this.txtbx_HeatPoint.Name = "txtbx_HeatPoint";
            this.txtbx_HeatPoint.Size = new System.Drawing.Size(51, 20);
            this.txtbx_HeatPoint.TabIndex = 10;
            // 
            // comboBoxHeatCoolMode
            // 
            this.comboBoxHeatCoolMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHeatCoolMode.Enabled = false;
            this.comboBoxHeatCoolMode.FormattingEnabled = true;
            this.comboBoxHeatCoolMode.Items.AddRange(new object[] {
            "",
            "Off",
            "Auto",
            "Heat",
            "Cool"});
            this.comboBoxHeatCoolMode.Location = new System.Drawing.Point(105, 32);
            this.comboBoxHeatCoolMode.Name = "comboBoxHeatCoolMode";
            this.comboBoxHeatCoolMode.Size = new System.Drawing.Size(132, 21);
            this.comboBoxHeatCoolMode.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Heat/Cool Mode: ";
            // 
            // txtb_deviceName
            // 
            this.txtb_deviceName.Location = new System.Drawing.Point(77, 18);
            this.txtb_deviceName.Name = "txtb_deviceName";
            this.txtb_deviceName.Size = new System.Drawing.Size(133, 20);
            this.txtb_deviceName.TabIndex = 17;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 19;
            this.label12.Text = "Device Name: ";
            // 
            // btn_SetDeviceName
            // 
            this.btn_SetDeviceName.Location = new System.Drawing.Point(217, 17);
            this.btn_SetDeviceName.Name = "btn_SetDeviceName";
            this.btn_SetDeviceName.Size = new System.Drawing.Size(47, 20);
            this.btn_SetDeviceName.TabIndex = 18;
            this.btn_SetDeviceName.Text = "Set";
            this.btn_SetDeviceName.UseVisualStyleBackColor = true;
            this.btn_SetDeviceName.Click += new System.EventHandler(this.btn_SetDeviceName_Click);
            // 
            // btn_RefreshDevices
            // 
            this.btn_RefreshDevices.Location = new System.Drawing.Point(610, 17);
            this.btn_RefreshDevices.Name = "btn_RefreshDevices";
            this.btn_RefreshDevices.Size = new System.Drawing.Size(149, 20);
            this.btn_RefreshDevices.TabIndex = 10;
            this.btn_RefreshDevices.Text = "Refresh Devices and Levels";
            this.btn_RefreshDevices.UseVisualStyleBackColor = true;
            this.btn_RefreshDevices.Click += new System.EventHandler(this.btn_RefreshDevices_Click);
            // 
            // listBoxDevices
            // 
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.Location = new System.Drawing.Point(6, 44);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.Size = new System.Drawing.Size(753, 134);
            this.listBoxDevices.TabIndex = 0;
            this.listBoxDevices.SelectedIndexChanged += new System.EventHandler(this.listBoxDevices_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Controls.Add(this.btn_SaveSettings);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(777, 598);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxLSLimit);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.textBoxLSport);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.textBoxLSPassword);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.checkBoxLSDebugVerbose);
            this.groupBox5.Controls.Add(this.checkBoxLSEnabled);
            this.groupBox5.Location = new System.Drawing.Point(3, 139);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(595, 72);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "LightSwitch Settings";
            // 
            // textBoxLSLimit
            // 
            this.textBoxLSLimit.Location = new System.Drawing.Point(537, 39);
            this.textBoxLSLimit.Name = "textBoxLSLimit";
            this.textBoxLSLimit.Size = new System.Drawing.Size(47, 20);
            this.textBoxLSLimit.TabIndex = 12;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(397, 46);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(134, 13);
            this.label14.TabIndex = 11;
            this.label14.Text = "# of Connection Limited to ";
            // 
            // textBoxLSport
            // 
            this.textBoxLSport.Location = new System.Drawing.Point(282, 40);
            this.textBoxLSport.Name = "textBoxLSport";
            this.textBoxLSport.Size = new System.Drawing.Size(47, 20);
            this.textBoxLSport.TabIndex = 10;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(201, 46);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(75, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Listen on Port:";
            // 
            // textBoxLSPassword
            // 
            this.textBoxLSPassword.Location = new System.Drawing.Point(282, 16);
            this.textBoxLSPassword.Name = "textBoxLSPassword";
            this.textBoxLSPassword.Size = new System.Drawing.Size(302, 20);
            this.textBoxLSPassword.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(191, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Client Password:";
            // 
            // checkBoxLSDebugVerbose
            // 
            this.checkBoxLSDebugVerbose.AutoSize = true;
            this.checkBoxLSDebugVerbose.Location = new System.Drawing.Point(9, 42);
            this.checkBoxLSDebugVerbose.Name = "checkBoxLSDebugVerbose";
            this.checkBoxLSDebugVerbose.Size = new System.Drawing.Size(142, 17);
            this.checkBoxLSDebugVerbose.TabIndex = 8;
            this.checkBoxLSDebugVerbose.Text = "Enable Verbose Logging";
            this.checkBoxLSDebugVerbose.UseVisualStyleBackColor = true;
            // 
            // checkBoxLSEnabled
            // 
            this.checkBoxLSEnabled.AutoSize = true;
            this.checkBoxLSEnabled.Location = new System.Drawing.Point(9, 19);
            this.checkBoxLSEnabled.Name = "checkBoxLSEnabled";
            this.checkBoxLSEnabled.Size = new System.Drawing.Size(151, 17);
            this.checkBoxLSEnabled.TabIndex = 7;
            this.checkBoxLSEnabled.Text = "Enable LightSwitch Server";
            this.checkBoxLSEnabled.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.txtb_exampleURL);
            this.groupBox6.Controls.Add(this.checkBoxHTTPEnable);
            this.groupBox6.Controls.Add(this.label1HTTPmoreinfo);
            this.groupBox6.Controls.Add(this.txtb_httpPort);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Location = new System.Drawing.Point(6, 14);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(523, 117);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "ZVirtualScene HTTP Listen ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(110, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Example HTTP URL: ";
            // 
            // txtb_exampleURL
            // 
            this.txtb_exampleURL.Location = new System.Drawing.Point(9, 84);
            this.txtb_exampleURL.Name = "txtb_exampleURL";
            this.txtb_exampleURL.ReadOnly = true;
            this.txtb_exampleURL.Size = new System.Drawing.Size(505, 20);
            this.txtb_exampleURL.TabIndex = 5;
            // 
            // checkBoxHTTPEnable
            // 
            this.checkBoxHTTPEnable.AutoSize = true;
            this.checkBoxHTTPEnable.Location = new System.Drawing.Point(392, 15);
            this.checkBoxHTTPEnable.Name = "checkBoxHTTPEnable";
            this.checkBoxHTTPEnable.Size = new System.Drawing.Size(122, 17);
            this.checkBoxHTTPEnable.TabIndex = 5;
            this.checkBoxHTTPEnable.Text = "Enable HTTP Listen";
            this.checkBoxHTTPEnable.UseVisualStyleBackColor = true;
            // 
            // label1HTTPmoreinfo
            // 
            this.label1HTTPmoreinfo.AutoSize = true;
            this.label1HTTPmoreinfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1HTTPmoreinfo.Location = new System.Drawing.Point(22, 29);
            this.label1HTTPmoreinfo.Name = "label1HTTPmoreinfo";
            this.label1HTTPmoreinfo.Size = new System.Drawing.Size(191, 13);
            this.label1HTTPmoreinfo.TabIndex = 4;
            this.label1HTTPmoreinfo.Text = "HOVER HERE FOR ALL COMMANDS";
            // 
            // txtb_httpPort
            // 
            this.txtb_httpPort.Location = new System.Drawing.Point(392, 38);
            this.txtb_httpPort.Name = "txtb_httpPort";
            this.txtb_httpPort.Size = new System.Drawing.Size(111, 20);
            this.txtb_httpPort.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(312, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Listen on Port:";
            // 
            // btn_SaveSettings
            // 
            this.btn_SaveSettings.Location = new System.Drawing.Point(642, 570);
            this.btn_SaveSettings.Name = "btn_SaveSettings";
            this.btn_SaveSettings.Size = new System.Drawing.Size(132, 23);
            this.btn_SaveSettings.TabIndex = 1;
            this.btn_SaveSettings.Text = "Save Settings";
            this.btn_SaveSettings.UseVisualStyleBackColor = true;
            this.btn_SaveSettings.Click += new System.EventHandler(this.btn_SaveSettings_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBoxLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(777, 598);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.Location = new System.Drawing.Point(3, 3);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(771, 589);
            this.listBoxLog.TabIndex = 1;
            // 
            // zVirtualScenes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 628);
            this.Controls.Add(this.MainTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 666);
            this.MinimumSize = new System.Drawing.Size(800, 666);
            this.Name = "zVirtualScenes";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.zVirtualScenes_Load);
            this.MainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxCommands.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxtTemp.ResumeLayout(false);
            this.groupBoxtTemp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        
        

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox listBoxDevices;
        private System.Windows.Forms.Label lbl_sceneActions;
        private System.Windows.Forms.ListBox listBoxSceneActions;
        private System.Windows.Forms.ListBox listBoxScenes;
        private System.Windows.Forms.TextBox txtbox_level;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_AddtoScene;
        private System.Windows.Forms.Button btn_runScene;
        private System.Windows.Forms.Button SaveScene_btn;
        private System.Windows.Forms.TextBox scenename_txtbx;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_DelAction;
        private System.Windows.Forms.Button btn_RefreshDevices;
        private System.Windows.Forms.GroupBox groupBoxtTemp;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxHeatCoolMode;
        private System.Windows.Forms.TextBox txtbx_HeatPoint;
        private System.Windows.Forms.Button btn_MoveUp;
        private System.Windows.Forms.Button btn_MoveDown;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btn_SaveSettings;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label1HTTPmoreinfo;
        private System.Windows.Forms.TextBox txtb_httpPort;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBoxHTTPEnable;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtb_exampleURL;
        private System.Windows.Forms.Button btn_RunCommand;
        private System.Windows.Forms.TextBox txtb_deviceName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_SetDeviceName;
        private System.Windows.Forms.GroupBox groupBoxCommands;
        private System.Windows.Forms.ComboBox comboBoxEnergyMode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxFanMode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Button buttonAddScene;
        private System.Windows.Forms.Button buttonDelScene;
        private System.Windows.Forms.Button btn_SceneMoveUp;
        private System.Windows.Forms.Button btn_sceneMoveDown;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxLSPassword;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxLSDebugVerbose;
        private System.Windows.Forms.CheckBox checkBoxLSEnabled;
        private System.Windows.Forms.TextBox textBoxLSport;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxLSLimit;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxCoolPoint;
        private System.Windows.Forms.Label labelCurrentTemp;
        private System.Windows.Forms.CheckBox checkBoxeditHP;
        private System.Windows.Forms.CheckBox checkBoxeditCP;
    }
}

