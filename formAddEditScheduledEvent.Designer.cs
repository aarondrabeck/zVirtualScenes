namespace zVirtualScenesApplication
{
    partial class formScheduledTasks
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formScheduledTasks));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.textBox_TaskName = new System.Windows.Forms.TextBox();
            this.dataListTasks = new BrightIdeasSoftware.DataListView();
            this.NameCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.EnabledCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.FreqCol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_Frequency = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxDaily = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_DaysRecur = new System.Windows.Forms.TextBox();
            this.button_Save = new System.Windows.Forms.Button();
            this.checkBox_Enabled = new System.Windows.Forms.CheckBox();
            this.button_NewTask = new System.Windows.Forms.Button();
            this.button_Close = new System.Windows.Forms.Button();
            this.groupBox_Weekly = new System.Windows.Forms.GroupBox();
            this.checkBox_RecurSunday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurSaturday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurFriday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurThursday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurWednesday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurTuesday = new System.Windows.Forms.CheckBox();
            this.checkBox_RecurMonday = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_RecurWeeks = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox_Actions = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_Delete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).BeginInit();
            this.groupBoxDaily.SuspendLayout();
            this.groupBox_Weekly.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // textBox_TaskName
            // 
            this.textBox_TaskName.Location = new System.Drawing.Point(96, 13);
            this.textBox_TaskName.Name = "textBox_TaskName";
            this.textBox_TaskName.Size = new System.Drawing.Size(285, 20);
            this.textBox_TaskName.TabIndex = 0;
            // 
            // dataListTasks
            // 
            this.dataListTasks.AllColumns.Add(this.NameCol);
            this.dataListTasks.AllColumns.Add(this.EnabledCol);
            this.dataListTasks.AllColumns.Add(this.FreqCol);
            this.dataListTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameCol,
            this.EnabledCol,
            this.FreqCol});
            this.dataListTasks.DataSource = null;
            this.dataListTasks.FullRowSelect = true;
            this.dataListTasks.HasCollapsibleGroups = false;
            this.dataListTasks.HeaderMaximumHeight = 15;
            this.dataListTasks.HideSelection = false;
            this.dataListTasks.Location = new System.Drawing.Point(12, 12);
            this.dataListTasks.Name = "dataListTasks";
            this.dataListTasks.OwnerDraw = true;
            this.dataListTasks.ShowGroups = false;
            this.dataListTasks.Size = new System.Drawing.Size(368, 412);
            this.dataListTasks.TabIndex = 31;
            this.dataListTasks.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.InactiveCaption;
            this.dataListTasks.UseCompatibleStateImageBehavior = false;
            this.dataListTasks.View = System.Windows.Forms.View.Details;
            this.dataListTasks.SelectedIndexChanged += new System.EventHandler(this.dataListTasks_SelectedIndexChanged);
            // 
            // NameCol
            // 
            this.NameCol.AspectName = "GetName";
            this.NameCol.IsEditable = false;
            this.NameCol.Text = "Name";
            this.NameCol.Width = 185;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Task Name:";
            // 
            // comboBox_Frequency
            // 
            this.comboBox_Frequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Frequency.FormattingEnabled = true;
            this.comboBox_Frequency.Location = new System.Drawing.Point(96, 39);
            this.comboBox_Frequency.Name = "comboBox_Frequency";
            this.comboBox_Frequency.Size = new System.Drawing.Size(285, 21);
            this.comboBox_Frequency.TabIndex = 33;
            this.comboBox_Frequency.SelectedIndexChanged += new System.EventHandler(this.comboBox_Frequency_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Task Frequency:";
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "dddd,MMMM d, yyyy \'at\' h:mm:ss tt";
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(96, 70);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(285, 20);
            this.dateTimePickerStart.TabIndex = 35;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 36;
            this.label3.Text = "Start:";
            // 
            // groupBoxDaily
            // 
            this.groupBoxDaily.Controls.Add(this.label5);
            this.groupBoxDaily.Controls.Add(this.label4);
            this.groupBoxDaily.Controls.Add(this.textBox_DaysRecur);
            this.groupBoxDaily.Location = new System.Drawing.Point(93, 119);
            this.groupBoxDaily.Name = "groupBoxDaily";
            this.groupBoxDaily.Size = new System.Drawing.Size(206, 54);
            this.groupBoxDaily.TabIndex = 37;
            this.groupBoxDaily.TabStop = false;
            this.groupBoxDaily.Text = "Daily";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(146, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "days.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 39;
            this.label4.Text = "Recur every: ";
            // 
            // textBox_DaysRecur
            // 
            this.textBox_DaysRecur.Location = new System.Drawing.Point(90, 19);
            this.textBox_DaysRecur.Name = "textBox_DaysRecur";
            this.textBox_DaysRecur.Size = new System.Drawing.Size(50, 20);
            this.textBox_DaysRecur.TabIndex = 38;
            // 
            // button_Save
            // 
            this.button_Save.Location = new System.Drawing.Point(289, 336);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(97, 25);
            this.button_Save.TabIndex = 38;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // checkBox_Enabled
            // 
            this.checkBox_Enabled.AutoSize = true;
            this.checkBox_Enabled.Location = new System.Drawing.Point(96, 96);
            this.checkBox_Enabled.Name = "checkBox_Enabled";
            this.checkBox_Enabled.Size = new System.Drawing.Size(65, 17);
            this.checkBox_Enabled.TabIndex = 39;
            this.checkBox_Enabled.Text = "Enabled";
            this.checkBox_Enabled.UseVisualStyleBackColor = true;
            // 
            // button_NewTask
            // 
            this.button_NewTask.Location = new System.Drawing.Point(12, 425);
            this.button_NewTask.Name = "button_NewTask";
            this.button_NewTask.Size = new System.Drawing.Size(98, 20);
            this.button_NewTask.TabIndex = 40;
            this.button_NewTask.Text = "Add New Task";
            this.button_NewTask.UseVisualStyleBackColor = true;
            this.button_NewTask.Click += new System.EventHandler(this.button_NewTask_Click);
            // 
            // button_Close
            // 
            this.button_Close.Location = new System.Drawing.Point(698, 425);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 20);
            this.button_Close.TabIndex = 41;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
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
            this.groupBox_Weekly.Controls.Add(this.label6);
            this.groupBox_Weekly.Controls.Add(this.label7);
            this.groupBox_Weekly.Controls.Add(this.textBox_RecurWeeks);
            this.groupBox_Weekly.Location = new System.Drawing.Point(93, 119);
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(146, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "weeks.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 39;
            this.label7.Text = "Recur every: ";
            // 
            // textBox_RecurWeeks
            // 
            this.textBox_RecurWeeks.Location = new System.Drawing.Point(90, 19);
            this.textBox_RecurWeeks.Name = "textBox_RecurWeeks";
            this.textBox_RecurWeeks.Size = new System.Drawing.Size(50, 20);
            this.textBox_RecurWeeks.TabIndex = 38;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(50, 295);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 43;
            this.label8.Text = "Action:";
            // 
            // comboBox_Actions
            // 
            this.comboBox_Actions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Actions.FormattingEnabled = true;
            this.comboBox_Actions.Location = new System.Drawing.Point(96, 292);
            this.comboBox_Actions.Name = "comboBox_Actions";
            this.comboBox_Actions.Size = new System.Drawing.Size(285, 21);
            this.comboBox_Actions.TabIndex = 42;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox_TaskName);
            this.groupBox1.Controls.Add(this.button_Save);
            this.groupBox1.Controls.Add(this.comboBox_Actions);
            this.groupBox1.Controls.Add(this.comboBox_Frequency);
            this.groupBox1.Controls.Add(this.groupBox_Weekly);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.checkBox_Enabled);
            this.groupBox1.Controls.Add(this.groupBoxDaily);
            this.groupBox1.Location = new System.Drawing.Point(384, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(389, 367);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            // 
            // button_Delete
            // 
            this.button_Delete.Location = new System.Drawing.Point(116, 425);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(55, 20);
            this.button_Delete.TabIndex = 44;
            this.button_Delete.Text = "Delete";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // formScheduledTasks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 448);
            this.Controls.Add(this.button_Delete);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.button_NewTask);
            this.Controls.Add(this.dataListTasks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formScheduledTasks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scheduled Tasks";
            this.Load += new System.EventHandler(this.formScheduledTasks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataListTasks)).EndInit();
            this.groupBoxDaily.ResumeLayout(false);
            this.groupBoxDaily.PerformLayout();
            this.groupBox_Weekly.ResumeLayout(false);
            this.groupBox_Weekly.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.TextBox textBox_TaskName;
        private BrightIdeasSoftware.DataListView dataListTasks;
        private BrightIdeasSoftware.OLVColumn NameCol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_Frequency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxDaily;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_DaysRecur;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.CheckBox checkBox_Enabled;
        private BrightIdeasSoftware.OLVColumn EnabledCol;
        private System.Windows.Forms.Button button_NewTask;
        private BrightIdeasSoftware.OLVColumn FreqCol;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.GroupBox groupBox_Weekly;
        private System.Windows.Forms.CheckBox checkBox_RecurSunday;
        private System.Windows.Forms.CheckBox checkBox_RecurSaturday;
        private System.Windows.Forms.CheckBox checkBox_RecurFriday;
        private System.Windows.Forms.CheckBox checkBox_RecurThursday;
        private System.Windows.Forms.CheckBox checkBox_RecurWednesday;
        private System.Windows.Forms.CheckBox checkBox_RecurTuesday;
        private System.Windows.Forms.CheckBox checkBox_RecurMonday;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_RecurWeeks;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_Actions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Delete;
    }
}