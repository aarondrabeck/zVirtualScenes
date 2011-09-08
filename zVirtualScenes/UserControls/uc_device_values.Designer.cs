namespace zVirtualScenesApplication.UserControls
{
    partial class uc_device_values
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.labelDeviceName = new System.Windows.Forms.Label();
            this.uc_object_values_grid1 = new zVirtualScenesApplication.UserControls.uc_device_values_grid();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxMain.Location = new System.Drawing.Point(42, 47);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxMain.TabIndex = 39;
            this.pictureBoxMain.TabStop = false;
            // 
            // labelDeviceName
            // 
            this.labelDeviceName.AutoEllipsis = true;
            this.labelDeviceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDeviceName.Location = new System.Drawing.Point(3, 6);
            this.labelDeviceName.Name = "labelDeviceName";
            this.labelDeviceName.Size = new System.Drawing.Size(202, 28);
            this.labelDeviceName.TabIndex = 41;
            this.labelDeviceName.Text = "Device";
            this.labelDeviceName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // uc_object_values_grid1
            // 
            this.uc_object_values_grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_object_values_grid1.Location = new System.Drawing.Point(212, -1);
            this.uc_object_values_grid1.Name = "uc_object_values_grid1";
            this.uc_object_values_grid1.Size = new System.Drawing.Size(567, 245);
            this.uc_object_values_grid1.TabIndex = 40;
            // 
            // uc_device_values
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelDeviceName);
            this.Controls.Add(this.uc_object_values_grid1);
            this.Controls.Add(this.pictureBoxMain);
            this.Name = "uc_device_values";
            this.Size = new System.Drawing.Size(778, 243);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxMain;
        private uc_device_values_grid uc_object_values_grid1;
        private System.Windows.Forms.Label labelDeviceName;
    }
}
