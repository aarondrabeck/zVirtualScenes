namespace zVirtualScenesApplication.Forms
{
    partial class ObjectProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectProperties));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxObjName = new System.Windows.Forms.TextBox();
            this.dataListViewMenu = new BrightIdeasSoftware.DataListView();
            this.colSettings = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.uc_device_basic1 = new zVirtualScenesApplication.UserControls.uc_device_basic();
            this.uc_device_values_grid2 = new zVirtualScenesApplication.UserControls.uc_device_values_grid();
            this.uc_device_commands1 = new zVirtualScenesApplication.UserControls.uc_device_commands();
            this.uc_device_properties1 = new zVirtualScenesApplication.UserControls.uc_device_properties();
            this.uc_device_groups1 = new zVirtualScenesApplication.UserControls.uc_device_groups();
            this.uc_device_type_commands1 = new zVirtualScenesApplication.UserControls.uc_device_type_commands();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name";
            // 
            // textBoxObjName
            // 
            this.textBoxObjName.Location = new System.Drawing.Point(54, 10);
            this.textBoxObjName.Name = "textBoxObjName";
            this.textBoxObjName.Size = new System.Drawing.Size(396, 20);
            this.textBoxObjName.TabIndex = 4;
            // 
            // dataListViewMenu
            // 
            this.dataListViewMenu.AllColumns.Add(this.colSettings);
            this.dataListViewMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataListViewMenu.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSettings});
            this.dataListViewMenu.DataSource = null;
            this.dataListViewMenu.FullRowSelect = true;
            this.dataListViewMenu.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.dataListViewMenu.HideSelection = false;
            this.dataListViewMenu.Location = new System.Drawing.Point(12, 13);
            this.dataListViewMenu.MultiSelect = false;
            this.dataListViewMenu.Name = "dataListViewMenu";
            this.dataListViewMenu.Size = new System.Drawing.Size(208, 583);
            this.dataListViewMenu.SortGroupItemsByPrimaryColumn = false;
            this.dataListViewMenu.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.dataListViewMenu.TabIndex = 28;
            this.dataListViewMenu.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.MenuHighlight;
            this.dataListViewMenu.UseCompatibleStateImageBehavior = false;
            this.dataListViewMenu.View = System.Windows.Forms.View.Details;
            this.dataListViewMenu.SelectedIndexChanged += new System.EventHandler(this.dataListViewMenu_SelectedIndexChanged_1);
            // 
            // colSettings
            // 
            this.colSettings.AspectName = "Name";
            this.colSettings.Text = "Settings";
            this.colSettings.Width = 175;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(882, 592);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 31;
            this.btnClose.Text = "Done";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(226, 577);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(731, 10);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            // 
            // uc_object_basic1
            // 
            this.uc_device_basic1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_device_basic1.AutoScroll = true;
            this.uc_device_basic1.Location = new System.Drawing.Point(238, 13);
            this.uc_device_basic1.Name = "uc_object_basic1";
            this.uc_device_basic1.Size = new System.Drawing.Size(716, 562);
            this.uc_device_basic1.TabIndex = 32;
            // 
            // uc_object_values_grid2
            // 
            this.uc_device_values_grid2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_device_values_grid2.Location = new System.Drawing.Point(238, 13);
            this.uc_device_values_grid2.Name = "uc_object_values_grid2";
            this.uc_device_values_grid2.Size = new System.Drawing.Size(716, 561);
            this.uc_device_values_grid2.TabIndex = 0;
            // 
            // uc_object_commands1
            // 
            this.uc_device_commands1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_device_commands1.AutoScroll = true;
            this.uc_device_commands1.Location = new System.Drawing.Point(238, 13);
            this.uc_device_commands1.Name = "uc_object_commands1";
            this.uc_device_commands1.Size = new System.Drawing.Size(716, 561);
            this.uc_device_commands1.TabIndex = 1;
            // 
            // uc_object_properties1
            // 
            this.uc_device_properties1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_device_properties1.AutoScroll = true;
            this.uc_device_properties1.Location = new System.Drawing.Point(238, 13);
            this.uc_device_properties1.Name = "uc_object_properties1";
            this.uc_device_properties1.Size = new System.Drawing.Size(716, 561);
            this.uc_device_properties1.TabIndex = 29;
            // 
            // uc_object_groups1
            // 
            this.uc_device_groups1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_device_groups1.Location = new System.Drawing.Point(238, 13);
            this.uc_device_groups1.Name = "uc_object_groups1";
            this.uc_device_groups1.Size = new System.Drawing.Size(716, 561);
            this.uc_device_groups1.TabIndex = 30;
            // 
            // uc_object_type_commands1
            // 
            this.uc_device_type_commands1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_device_type_commands1.Location = new System.Drawing.Point(238, 13);
            this.uc_device_type_commands1.Name = "uc_object_type_commands1";
            this.uc_device_type_commands1.Size = new System.Drawing.Size(716, 558);
            this.uc_device_type_commands1.TabIndex = 34;
            // 
            // ObjectProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(966, 623);
            this.Controls.Add(this.uc_device_type_commands1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.uc_device_basic1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dataListViewMenu);
            this.Controls.Add(this.uc_device_values_grid2);
            this.Controls.Add(this.uc_device_commands1);
            this.Controls.Add(this.uc_device_properties1);
            this.Controls.Add(this.uc_device_groups1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Name = "ObjectProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Object Properties";
            this.Load += new System.EventHandler(this.ObjectProperties_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ObjectProperties_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxObjName;
        private UserControls.uc_device_values_grid uc_device_values_grid2;
        private UserControls.uc_device_commands uc_device_commands1;
        private BrightIdeasSoftware.DataListView dataListViewMenu;
        private BrightIdeasSoftware.OLVColumn colSettings;
        private UserControls.uc_device_properties uc_device_properties1;
        private UserControls.uc_device_groups uc_device_groups1;
        private System.Windows.Forms.Button btnClose;
        private UserControls.uc_device_basic uc_device_basic1;
        private System.Windows.Forms.GroupBox groupBox1;
        private UserControls.uc_device_type_commands uc_device_type_commands1;

    }
}