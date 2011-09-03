namespace zVirtualScenesApplication.UserControls
{
    partial class uc_device_values_grid
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
            this.dataListViewStates = new BrightIdeasSoftware.DataListView();
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewStates)).BeginInit();
            this.SuspendLayout();
            // 
            // dataListViewStates
            // 
            this.dataListViewStates.AllColumns.Add(this.olvColumn4);
            this.dataListViewStates.AllColumns.Add(this.olvColumn5);
            this.dataListViewStates.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataListViewStates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn4,
            this.olvColumn5});
            this.dataListViewStates.DataSource = null;
            this.dataListViewStates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewStates.FullRowSelect = true;
            this.dataListViewStates.HeaderMaximumHeight = 15;
            this.dataListViewStates.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataListViewStates.HideSelection = false;
            this.dataListViewStates.Location = new System.Drawing.Point(0, 0);
            this.dataListViewStates.Name = "dataListViewStates";
            this.dataListViewStates.OwnerDraw = true;
            this.dataListViewStates.ShowGroups = false;
            this.dataListViewStates.Size = new System.Drawing.Size(474, 200);
            this.dataListViewStates.TabIndex = 38;
            this.dataListViewStates.UseCompatibleStateImageBehavior = false;
            this.dataListViewStates.UseExplorerTheme = true;
            this.dataListViewStates.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "label_name";
            this.olvColumn4.IsEditable = false;
            this.olvColumn4.Text = "Label";
            this.olvColumn4.Width = global::zVirtualScenesApplication.Properties.Settings.Default.ColNameWidth;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "value";
            this.olvColumn5.Text = "Value";
            this.olvColumn5.Width = global::zVirtualScenesApplication.Properties.Settings.Default.ColActionwidth;
            // 
            // uc_object_values_grid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataListViewStates);
            this.Name = "uc_object_values_grid";
            this.Size = new System.Drawing.Size(474, 200);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewStates)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.DataListView dataListViewStates;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
