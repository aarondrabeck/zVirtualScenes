namespace zVirtualScenesApplication.Forms
{
    partial class AdvancedScriptEditor
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
            this.uc_script_editor1 = new zVirtualScenesApplication.UserControls.uc_script_editor();
            this.SuspendLayout();
            // 
            // uc_script_editor1
            // 
            this.uc_script_editor1.Location = new System.Drawing.Point(12, 54);
            this.uc_script_editor1.Name = "uc_script_editor1";
            this.uc_script_editor1.Size = new System.Drawing.Size(260, 196);
            this.uc_script_editor1.TabIndex = 0;
            // 
            // AdvancedScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.uc_script_editor1);
            this.Name = "AdvancedScriptEditor";
            this.Text = "AdvancedScriptEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.uc_script_editor uc_script_editor1;
    }
}