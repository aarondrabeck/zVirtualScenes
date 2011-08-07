namespace zVirtualScenesApplication
{
    partial class ProgramSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgramSettings));
            this.toolTipNotificationLevel = new System.Windows.Forms.ToolTip(this.components);
            this.labelSaveStatus = new System.Windows.Forms.Label();
            this.btnDone = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataListViewMenu = new BrightIdeasSoftware.DataListView();
            this.colSettings = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.uc_plugin_properties_form1 = new zVirtualScenesApplication.UserControls.uc_plugin_properties_form();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTipNotificationLevel
            // 
            this.toolTipNotificationLevel.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipNotificationLevel.ToolTipTitle = "Notification Levels";
            // 
            // labelSaveStatus
            // 
            this.labelSaveStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSaveStatus.AutoSize = true;
            this.labelSaveStatus.Location = new System.Drawing.Point(21, 473);
            this.labelSaveStatus.Name = "labelSaveStatus";
            this.labelSaveStatus.Size = new System.Drawing.Size(10, 13);
            this.labelSaveStatus.TabIndex = 23;
            this.labelSaveStatus.Text = "-";
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.Location = new System.Drawing.Point(900, 502);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 20;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.buttonOK_Click);
            this.btnDone.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnDone_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(188, 483);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(787, 10);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
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
            this.dataListViewMenu.HideSelection = false;
            this.dataListViewMenu.Location = new System.Drawing.Point(3, 7);
            this.dataListViewMenu.MultiSelect = false;
            this.dataListViewMenu.Name = "dataListViewMenu";
            this.dataListViewMenu.Size = new System.Drawing.Size(180, 489);
            this.dataListViewMenu.SortGroupItemsByPrimaryColumn = false;
            this.dataListViewMenu.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.dataListViewMenu.TabIndex = 27;
            this.dataListViewMenu.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.MenuHighlight;
            this.dataListViewMenu.UseCompatibleStateImageBehavior = false;
            this.dataListViewMenu.View = System.Windows.Forms.View.Details;
            this.dataListViewMenu.SelectedIndexChanged += new System.EventHandler(this.dataListViewMenu_SelectedIndexChanged);
            // 
            // colSettings
            // 
            this.colSettings.AspectName = "Name";
            this.colSettings.Text = "Settings";
            this.colSettings.Width = 175;
            // 
            // uc_plugin_properties_form1
            // 
            this.uc_plugin_properties_form1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_plugin_properties_form1.Location = new System.Drawing.Point(189, 7);
            this.uc_plugin_properties_form1.Name = "uc_plugin_properties_form1";
            this.uc_plugin_properties_form1.Size = new System.Drawing.Size(786, 473);
            this.uc_plugin_properties_form1.TabIndex = 26;
            this.uc_plugin_properties_form1.Visible = false;
            // 
            // ProgramSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 535);
            this.Controls.Add(this.dataListViewMenu);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.labelSaveStatus);
            this.Controls.Add(this.uc_plugin_properties_form1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(720, 428);
            this.Name = "ProgramSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.ProgramSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipNotificationLevel;
        private System.Windows.Forms.Label labelSaveStatus;        
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.GroupBox groupBox1;
        private UserControls.uc_plugin_properties_form uc_plugin_properties_form1;
        private BrightIdeasSoftware.DataListView dataListViewMenu;
        private BrightIdeasSoftware.OLVColumn colSettings;
    }
}