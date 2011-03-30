namespace zVirtualScenesApplication
{
    partial class formAddEditEXEC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formAddEditEXEC));
            this.txtb_path = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBoxAction = new System.Windows.Forms.GroupBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.checkBoxSkipLight = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipDark = new System.Windows.Forms.CheckBox();
            this.groupBoxAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // txtb_path
            // 
            this.txtb_path.Location = new System.Drawing.Point(36, 19);
            this.txtb_path.Name = "txtb_path";
            this.txtb_path.ReadOnly = true;
            this.txtb_path.Size = new System.Drawing.Size(338, 20);
            this.txtb_path.TabIndex = 24;
            this.txtb_path.WordWrap = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(0, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Path: ";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(379, 103);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(64, 23);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "&Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBoxAction
            // 
            this.groupBoxAction.Controls.Add(this.buttonBrowse);
            this.groupBoxAction.Controls.Add(this.txtb_path);
            this.groupBoxAction.Controls.Add(this.label12);
            this.groupBoxAction.Location = new System.Drawing.Point(63, 12);
            this.groupBoxAction.Name = "groupBoxAction";
            this.groupBoxAction.Size = new System.Drawing.Size(380, 71);
            this.groupBoxAction.TabIndex = 35;
            this.groupBoxAction.TabStop = false;
            this.groupBoxAction.Text = "Launch EXE";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(297, 45);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(77, 20);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "&Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::zVirtualScenesApplication.Properties.Resources.programs_icon48;
            this.pictureBox3.Location = new System.Drawing.Point(3, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(54, 55);
            this.pictureBox3.TabIndex = 36;
            this.pictureBox3.TabStop = false;
            // 
            // checkBoxSkipLight
            // 
            this.checkBoxSkipLight.AutoSize = true;
            this.checkBoxSkipLight.Location = new System.Drawing.Point(158, 109);
            this.checkBoxSkipLight.Name = "checkBoxSkipLight";
            this.checkBoxSkipLight.Size = new System.Drawing.Size(138, 17);
            this.checkBoxSkipLight.TabIndex = 47;
            this.checkBoxSkipLight.Text = "Skip when light outside.";
            this.checkBoxSkipLight.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipDark
            // 
            this.checkBoxSkipDark.AutoSize = true;
            this.checkBoxSkipDark.Location = new System.Drawing.Point(12, 109);
            this.checkBoxSkipDark.Name = "checkBoxSkipDark";
            this.checkBoxSkipDark.Size = new System.Drawing.Size(140, 17);
            this.checkBoxSkipDark.TabIndex = 46;
            this.checkBoxSkipDark.Text = "Skip when dark outside.";
            this.checkBoxSkipDark.UseVisualStyleBackColor = true;
            // 
            // formAddEditEXEC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 131);
            this.Controls.Add(this.checkBoxSkipLight);
            this.Controls.Add(this.checkBoxSkipDark);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.groupBoxAction);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formAddEditEXEC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Executable Action Properties";
            this.Load += new System.EventHandler(this.formAddEditEXEC_Load);
            this.groupBoxAction.ResumeLayout(false);
            this.groupBoxAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtb_path;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBoxAction;
        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.CheckBox checkBoxSkipLight;
        private System.Windows.Forms.CheckBox checkBoxSkipDark;
    }
}