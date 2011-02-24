namespace zVirtualScenesApplication
{
    partial class formSceneProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formSceneProperties));
            this.txtb_sceneName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBoxDevice = new System.Windows.Forms.GroupBox();
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxDevice.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtb_sceneName
            // 
            this.txtb_sceneName.Location = new System.Drawing.Point(90, 19);
            this.txtb_sceneName.Name = "txtb_sceneName";
            this.txtb_sceneName.Size = new System.Drawing.Size(206, 20);
            this.txtb_sceneName.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Scene  Name: ";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(249, 230);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(93, 25);
            this.btn_Save.TabIndex = 34;
            this.btn_Save.Text = "Save Changes";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBoxDevice
            // 
            this.groupBoxDevice.Controls.Add(this.txtb_sceneName);
            this.groupBoxDevice.Controls.Add(this.btn_Save);
            this.groupBoxDevice.Controls.Add(this.label12);
            this.groupBoxDevice.Location = new System.Drawing.Point(2, 4);
            this.groupBoxDevice.Name = "groupBoxDevice";
            this.groupBoxDevice.Size = new System.Drawing.Size(348, 261);
            this.groupBoxDevice.TabIndex = 35;
            this.groupBoxDevice.TabStop = false;
            this.groupBoxDevice.Text = "Scene";
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // formSceneProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 273);
            this.Controls.Add(this.groupBoxDevice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(366, 311);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(366, 311);
            this.Name = "formSceneProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Scene  Properties";
            this.groupBoxDevice.ResumeLayout(false);
            this.groupBoxDevice.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtb_sceneName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBoxDevice;
        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
    }
}