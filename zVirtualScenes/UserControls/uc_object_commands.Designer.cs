namespace zVirtualScenesApplication.UserControls
{
    partial class uc_object_commands
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
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.AutoSize = true;
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(233, 131);
            this.pnlSettings.TabIndex = 0;
            // 
            // uc_object_properties_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlSettings);
            this.Name = "uc_object_properties_form";
            this.Size = new System.Drawing.Size(233, 131);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlSettings;
    }
}
