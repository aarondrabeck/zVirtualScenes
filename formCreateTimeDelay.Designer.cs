namespace zVirtualScenesApplication
{
    partial class formCreateTimeDelay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formCreateTimeDelay));
            this.txtb_duration = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBoxDevice = new System.Windows.Forms.GroupBox();
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxDevice.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtb_duration
            // 
            this.txtb_duration.Location = new System.Drawing.Point(144, 20);
            this.txtb_duration.Name = "txtb_duration";
            this.txtb_duration.Size = new System.Drawing.Size(87, 20);
            this.txtb_duration.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(132, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Delay Duration (seconds): ";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(257, 20);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(79, 20);
            this.btn_Save.TabIndex = 34;
            this.btn_Save.Text = "Create";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBoxDevice
            // 
            this.groupBoxDevice.Controls.Add(this.txtb_duration);
            this.groupBoxDevice.Controls.Add(this.btn_Save);
            this.groupBoxDevice.Controls.Add(this.label12);
            this.groupBoxDevice.Location = new System.Drawing.Point(2, 4);
            this.groupBoxDevice.Name = "groupBoxDevice";
            this.groupBoxDevice.Size = new System.Drawing.Size(348, 53);
            this.groupBoxDevice.TabIndex = 35;
            this.groupBoxDevice.TabStop = false;
            this.groupBoxDevice.Text = "Timer Properties";
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // formCreateTimeDelay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 62);
            this.Controls.Add(this.groupBoxDevice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(366, 100);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(366, 100);
            this.Name = "formCreateTimeDelay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create a timer";
            this.groupBoxDevice.ResumeLayout(false);
            this.groupBoxDevice.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtb_duration;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBoxDevice;
        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
    }
}