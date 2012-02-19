namespace zVirtualScenesApplication
{
    partial class formPropertiesScene
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formPropertiesScene));
            this.txtb_sceneName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pnlSceneProperties = new System.Windows.Forms.Panel();
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblResetSceneRunning = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // txtb_sceneName
            // 
            this.txtb_sceneName.Location = new System.Drawing.Point(175, 15);
            this.txtb_sceneName.Name = "txtb_sceneName";
            this.txtb_sceneName.Size = new System.Drawing.Size(205, 20);
            this.txtb_sceneName.TabIndex = 0;
            this.txtb_sceneName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtb_sceneName_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(94, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Scene Name: ";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(386, 13);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(122, 22);
            this.btn_Save.TabIndex = 5;
            this.btn_Save.Text = "&Save Scene Name";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(752, 261);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 22);
            this.button1.TabIndex = 41;
            this.button1.Text = "&Done";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pnlSceneProperties
            // 
            this.pnlSceneProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSceneProperties.AutoScroll = true;
            this.pnlSceneProperties.Location = new System.Drawing.Point(91, 40);
            this.pnlSceneProperties.Name = "pnlSceneProperties";
            this.pnlSceneProperties.Size = new System.Drawing.Size(721, 215);
            this.pnlSceneProperties.TabIndex = 40;
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::zVirtualScenesApplication.Properties.Resources.zvirtualscenes72;
            this.pictureBox2.Location = new System.Drawing.Point(8, 7);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(77, 62);
            this.pictureBox2.TabIndex = 39;
            this.pictureBox2.TabStop = false;
            // 
            // lblResetSceneRunning
            // 
            this.lblResetSceneRunning.AutoSize = true;
            this.lblResetSceneRunning.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblResetSceneRunning.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblResetSceneRunning.Location = new System.Drawing.Point(12, 269);
            this.lblResetSceneRunning.Name = "lblResetSceneRunning";
            this.lblResetSceneRunning.Size = new System.Drawing.Size(136, 13);
            this.lblResetSceneRunning.TabIndex = 42;
            this.lblResetSceneRunning.Text = "Set scene running to false. ";
            this.toolTip1.SetToolTip(this.lblResetSceneRunning, "Only use this when the app crashed while a scene was running and the isRunning bo" +
        "ol is falsely set to true. ");
            this.lblResetSceneRunning.Click += new System.EventHandler(this.lblResetSceneRunning_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.toolTip1.ToolTipTitle = "Only use this if you know what you are doing!";
            // 
            // formPropertiesScene
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 291);
            this.Controls.Add(this.lblResetSceneRunning);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pnlSceneProperties);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtb_sceneName);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(534, 325);
            this.Name = "formPropertiesScene";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Scene Properties";
            this.Load += new System.EventHandler(this.formSceneProperties_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtb_sceneName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel pnlSceneProperties;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblResetSceneRunning;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}