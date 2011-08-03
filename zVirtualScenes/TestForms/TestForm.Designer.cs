namespace zVirtualScenesApplication.TestForms
{
    partial class TestForm
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
            this.lstPlugins = new System.Windows.Forms.ListBox();
            this.lstObjects = new System.Windows.Forms.ListBox();
            this.btnUpdateObjectList = new System.Windows.Forms.Button();
            this.objectSettingsForm1 = new zVirtualScenesApplication.UserControls.uc_object_properties();
            this.SuspendLayout();
            // 
            // lstPlugins
            // 
            this.lstPlugins.FormattingEnabled = true;
            this.lstPlugins.Location = new System.Drawing.Point(12, 12);
            this.lstPlugins.Name = "lstPlugins";
            this.lstPlugins.Size = new System.Drawing.Size(120, 277);
            this.lstPlugins.TabIndex = 0;
            // 
            // lstObjects
            // 
            this.lstObjects.FormattingEnabled = true;
            this.lstObjects.Location = new System.Drawing.Point(138, 12);
            this.lstObjects.Name = "lstObjects";
            this.lstObjects.Size = new System.Drawing.Size(120, 251);
            this.lstObjects.TabIndex = 1;
            this.lstObjects.SelectedIndexChanged += new System.EventHandler(this.lstObjects_SelectedIndexChanged);
            // 
            // btnUpdateObjectList
            // 
            this.btnUpdateObjectList.Location = new System.Drawing.Point(138, 266);
            this.btnUpdateObjectList.Name = "btnUpdateObjectList";
            this.btnUpdateObjectList.Size = new System.Drawing.Size(120, 23);
            this.btnUpdateObjectList.TabIndex = 2;
            this.btnUpdateObjectList.Text = "Update List";
            this.btnUpdateObjectList.UseVisualStyleBackColor = true;
            this.btnUpdateObjectList.Click += new System.EventHandler(this.btnUpdateObjectList_Click);
            // 
            // objectSettingsForm1
            // 
            this.objectSettingsForm1.Location = new System.Drawing.Point(264, 12);
            this.objectSettingsForm1.Name = "objectSettingsForm1";
            this.objectSettingsForm1.Size = new System.Drawing.Size(465, 251);
            this.objectSettingsForm1.TabIndex = 3;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 305);
            this.Controls.Add(this.objectSettingsForm1);
            this.Controls.Add(this.btnUpdateObjectList);
            this.Controls.Add(this.lstObjects);
            this.Controls.Add(this.lstPlugins);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstPlugins;
        private System.Windows.Forms.ListBox lstObjects;
        private System.Windows.Forms.Button btnUpdateObjectList;
        private UserControls.uc_object_properties objectSettingsForm1;
    }
}