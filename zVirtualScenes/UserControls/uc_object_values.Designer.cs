namespace zVirtualScenesApplication.UserControls
{
    partial class uc_object_values
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.labelDeviceName = new System.Windows.Forms.Label();
            this.labelLevel = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.uc_object_values_grid1 = new zVirtualScenesApplication.UserControls.uc_object_values_grid();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(17, 181);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(127, 11);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            this.progressBar1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.progressBar1_MouseClick);
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxMain.Location = new System.Drawing.Point(17, 47);
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
            // labelLevel
            // 
            this.labelLevel.AutoEllipsis = true;
            this.labelLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLevel.Location = new System.Drawing.Point(17, 195);
            this.labelLevel.Name = "labelLevel";
            this.labelLevel.Size = new System.Drawing.Size(128, 48);
            this.labelLevel.TabIndex = 42;
            this.labelLevel.Text = "99";
            this.labelLevel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelLevel.Click += new System.EventHandler(this.labelLevel_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackBar1.LargeChange = 20;
            this.trackBar1.Location = new System.Drawing.Point(161, 47);
            this.trackBar1.Maximum = 99;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 145);
            this.trackBar1.SmallChange = 10;
            this.trackBar1.TabIndex = 43;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trackBar1_KeyUp);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
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
            // uc_object_values
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelLevel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelDeviceName);
            this.Controls.Add(this.uc_object_values_grid1);
            this.Controls.Add(this.pictureBoxMain);
            this.Controls.Add(this.trackBar1);
            this.Name = "uc_object_values";
            this.Size = new System.Drawing.Size(778, 243);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PictureBox pictureBoxMain;
        private uc_object_values_grid uc_object_values_grid1;
        private System.Windows.Forms.Label labelDeviceName;
        private System.Windows.Forms.Label labelLevel;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}
