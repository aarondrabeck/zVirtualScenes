namespace zVirtualScenesApplication
{
    partial class formAddEditTimeDelay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formAddEditTimeDelay));
            this.txtb_duration = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBoxAction = new System.Windows.Forms.GroupBox();
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.checkBoxSkipLight = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipDark = new System.Windows.Forms.CheckBox();
            this.groupBoxAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // txtb_duration
            // 
            this.txtb_duration.Location = new System.Drawing.Point(144, 20);
            this.txtb_duration.Name = "txtb_duration";
            this.txtb_duration.Size = new System.Drawing.Size(87, 20);
            this.txtb_duration.TabIndex = 1;
            this.txtb_duration.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtb_duration_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(132, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Delay Duration (seconds): ";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(361, 73);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(59, 24);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "&Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBoxAction
            // 
            this.groupBoxAction.Controls.Add(this.txtb_duration);
            this.groupBoxAction.Controls.Add(this.label12);
            this.groupBoxAction.Location = new System.Drawing.Point(72, 14);
            this.groupBoxAction.Name = "groupBoxAction";
            this.groupBoxAction.Size = new System.Drawing.Size(348, 53);
            this.groupBoxAction.TabIndex = 35;
            this.groupBoxAction.TabStop = false;
            this.groupBoxAction.Text = "Timer Properties";
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::zVirtualScenesApplication.Properties.Resources.scheduled_tasks48;
            this.pictureBox3.Location = new System.Drawing.Point(12, 12);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(54, 55);
            this.pictureBox3.TabIndex = 37;
            this.pictureBox3.TabStop = false;
            // 
            // checkBoxSkipLight
            // 
            this.checkBoxSkipLight.AutoSize = true;
            this.checkBoxSkipLight.Location = new System.Drawing.Point(165, 80);
            this.checkBoxSkipLight.Name = "checkBoxSkipLight";
            this.checkBoxSkipLight.Size = new System.Drawing.Size(138, 17);
            this.checkBoxSkipLight.TabIndex = 47;
            this.checkBoxSkipLight.Text = "Skip when light outside.";
            this.checkBoxSkipLight.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipDark
            // 
            this.checkBoxSkipDark.AutoSize = true;
            this.checkBoxSkipDark.Location = new System.Drawing.Point(12, 80);
            this.checkBoxSkipDark.Name = "checkBoxSkipDark";
            this.checkBoxSkipDark.Size = new System.Drawing.Size(140, 17);
            this.checkBoxSkipDark.TabIndex = 46;
            this.checkBoxSkipDark.Text = "Skip when dark outside.";
            this.checkBoxSkipDark.UseVisualStyleBackColor = true;
            // 
            // formAddEditTimeDelay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 101);
            this.Controls.Add(this.checkBoxSkipLight);
            this.Controls.Add(this.checkBoxSkipDark);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.groupBoxAction);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formAddEditTimeDelay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Timed Delay Action Properties";
            this.Load += new System.EventHandler(this.formAddEditTimeDelay_Load);
            this.groupBoxAction.ResumeLayout(false);
            this.groupBoxAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtb_duration;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBoxAction;
        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.CheckBox checkBoxSkipLight;
        private System.Windows.Forms.CheckBox checkBoxSkipDark;
    }
}