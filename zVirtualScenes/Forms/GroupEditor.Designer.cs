namespace zVirtualScenesApplication.Forms
{
    partial class GroupEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupEditor));
            this.cboGroups = new System.Windows.Forms.ComboBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnAddOne = new System.Windows.Forms.Button();
            this.btnRemoveOne = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtNewGroup = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstNotAdded = new BrightIdeasSoftware.DataListView();
            this.obj_Name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lstAdded = new BrightIdeasSoftware.DataListView();
            this.ObjName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label3 = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstNotAdded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstAdded)).BeginInit();
            this.SuspendLayout();
            // 
            // cboGroups
            // 
            this.cboGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGroups.FormattingEnabled = true;
            this.cboGroups.Location = new System.Drawing.Point(63, 13);
            this.cboGroups.Name = "cboGroups";
            this.cboGroups.Size = new System.Drawing.Size(294, 21);
            this.cboGroups.TabIndex = 0;
            this.cboGroups.SelectedIndexChanged += new System.EventHandler(this.cboGroups_SelectedIndexChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(461, 11);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(63, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAll.Location = new System.Drawing.Point(5, 45);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(75, 23);
            this.btnAddAll.TabIndex = 5;
            this.btnAddAll.Text = "> >";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnAddOne
            // 
            this.btnAddOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddOne.Location = new System.Drawing.Point(5, 74);
            this.btnAddOne.Name = "btnAddOne";
            this.btnAddOne.Size = new System.Drawing.Size(75, 23);
            this.btnAddOne.TabIndex = 6;
            this.btnAddOne.Text = ">";
            this.btnAddOne.UseVisualStyleBackColor = true;
            this.btnAddOne.Click += new System.EventHandler(this.btnAddOne_Click);
            // 
            // btnRemoveOne
            // 
            this.btnRemoveOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveOne.Location = new System.Drawing.Point(5, 103);
            this.btnRemoveOne.Name = "btnRemoveOne";
            this.btnRemoveOne.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveOne.TabIndex = 7;
            this.btnRemoveOne.Text = "<";
            this.btnRemoveOne.UseVisualStyleBackColor = true;
            this.btnRemoveOne.Click += new System.EventHandler(this.btnRemoveOne_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveAll.Location = new System.Drawing.Point(5, 132);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveAll.TabIndex = 8;
            this.btnRemoveAll.Text = "< <";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 10;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(456, 522);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Done";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtNewGroup
            // 
            this.txtNewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewGroup.Location = new System.Drawing.Point(63, 13);
            this.txtNewGroup.Name = "txtNewGroup";
            this.txtNewGroup.Size = new System.Drawing.Size(294, 20);
            this.txtNewGroup.TabIndex = 12;
            this.txtNewGroup.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 47);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstNotAdded);
            this.splitContainer1.Panel1MinSize = 150;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 200;
            this.splitContainer1.Size = new System.Drawing.Size(512, 469);
            this.splitContainer1.SplitterDistance = 203;
            this.splitContainer1.TabIndex = 13;
            // 
            // lstNotAdded
            // 
            this.lstNotAdded.AllColumns.Add(this.obj_Name);
            this.lstNotAdded.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.obj_Name});
            this.lstNotAdded.DataSource = null;
            this.lstNotAdded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstNotAdded.Location = new System.Drawing.Point(0, 0);
            this.lstNotAdded.Name = "lstNotAdded";
            this.lstNotAdded.ShowGroups = false;
            this.lstNotAdded.Size = new System.Drawing.Size(203, 469);
            this.lstNotAdded.TabIndex = 9;
            this.lstNotAdded.UseCompatibleStateImageBehavior = false;
            this.lstNotAdded.View = System.Windows.Forms.View.Details;
            // 
            // obj_Name
            // 
            this.obj_Name.AspectName = "txt_object_name";
            this.obj_Name.Text = "Object(s) Not in Group";
            this.obj_Name.Width = 170;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnAddAll);
            this.splitContainer2.Panel1.Controls.Add(this.btnAddOne);
            this.splitContainer2.Panel1.Controls.Add(this.btnRemoveOne);
            this.splitContainer2.Panel1.Controls.Add(this.btnRemoveAll);
            this.splitContainer2.Panel1MinSize = 83;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lstAdded);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Size = new System.Drawing.Size(305, 469);
            this.splitContainer2.SplitterDistance = 83;
            this.splitContainer2.TabIndex = 0;
            // 
            // lstAdded
            // 
            this.lstAdded.AllColumns.Add(this.ObjName);
            this.lstAdded.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ObjName});
            this.lstAdded.DataSource = null;
            this.lstAdded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAdded.Location = new System.Drawing.Point(0, 0);
            this.lstAdded.Name = "lstAdded";
            this.lstAdded.ShowGroups = false;
            this.lstAdded.Size = new System.Drawing.Size(218, 469);
            this.lstAdded.TabIndex = 11;
            this.lstAdded.UseCompatibleStateImageBehavior = false;
            this.lstAdded.View = System.Windows.Forms.View.Details;
            // 
            // ObjName
            // 
            this.ObjName.AspectName = "txt_object_name";
            this.ObjName.Text = "Object(s) in group";
            this.ObjName.Width = 170;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Group:";
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.Location = new System.Drawing.Point(363, 11);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(43, 23);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(412, 11);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(43, 23);
            this.btnEdit.TabIndex = 14;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click_1);
            // 
            // GroupEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 551);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.txtNewGroup);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.cboGroups);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(497, 589);
            this.Name = "GroupEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "zVirtualScenes - Group Editor";
            this.Load += new System.EventHandler(this.GroupEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GroupEditor_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstNotAdded)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstAdded)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboGroups;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnAddOne;
        private System.Windows.Forms.Button btnRemoveOne;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtNewGroup;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label3;
        private BrightIdeasSoftware.DataListView lstNotAdded;
        private BrightIdeasSoftware.DataListView lstAdded;
        private BrightIdeasSoftware.OLVColumn obj_Name;
        private BrightIdeasSoftware.OLVColumn ObjName;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnEdit;
    }
}