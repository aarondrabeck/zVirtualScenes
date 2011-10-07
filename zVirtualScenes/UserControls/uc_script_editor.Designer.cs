namespace zVirtualScenesApplication.UserControls
{
    partial class uc_script_editor
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("LowerNode1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("test", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_script_editor));
            this.txtScript = new System.Windows.Forms.RichTextBox();
            this.textBoxTooltip = new System.Windows.Forms.TextBox();
            this.treeViewItems = new System.Windows.Forms.TreeView();
            this.listBoxAutoComplete = new zVirtualScenesApplication.UserControls.uc_intellisense_listbox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // txtScript
            // 
            this.txtScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtScript.Location = new System.Drawing.Point(0, 0);
            this.txtScript.Name = "txtScript";
            this.txtScript.Size = new System.Drawing.Size(150, 150);
            this.txtScript.TabIndex = 0;
            this.txtScript.Text = "";
            this.txtScript.TextChanged += new System.EventHandler(this.txtScript_TextChanged);
            this.txtScript.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtScript_KeyDown);
            // 
            // textBoxTooltip
            // 
            this.textBoxTooltip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.textBoxTooltip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTooltip.Location = new System.Drawing.Point(20, 108);
            this.textBoxTooltip.Multiline = true;
            this.textBoxTooltip.Name = "textBoxTooltip";
            this.textBoxTooltip.ReadOnly = true;
            this.textBoxTooltip.Size = new System.Drawing.Size(100, 20);
            this.textBoxTooltip.TabIndex = 2;
            this.textBoxTooltip.Visible = false;
            // 
            // treeViewItems
            // 
            this.treeViewItems.Location = new System.Drawing.Point(8, 8);
            this.treeViewItems.Name = "treeViewItems";
            treeNode1.Name = "LowerNode1";
            treeNode1.Text = "LowerNode1";
            treeNode2.Name = "test";
            treeNode2.Text = "test";
            this.treeViewItems.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeViewItems.Size = new System.Drawing.Size(98, 36);
            this.treeViewItems.TabIndex = 4;
            this.treeViewItems.Visible = false;
            // 
            // listBoxAutoComplete
            // 
            this.listBoxAutoComplete.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxAutoComplete.FormattingEnabled = true;
            this.listBoxAutoComplete.ImageList = this.imageList1;
            this.listBoxAutoComplete.Location = new System.Drawing.Point(8, 50);
            this.listBoxAutoComplete.Name = "listBoxAutoComplete";
            this.listBoxAutoComplete.Size = new System.Drawing.Size(112, 30);
            this.listBoxAutoComplete.TabIndex = 3;
            this.listBoxAutoComplete.Visible = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "class.bmp");
            this.imageList1.Images.SetKeyName(1, "event.bmp");
            this.imageList1.Images.SetKeyName(2, "method.bmp");
            this.imageList1.Images.SetKeyName(3, "namespace.bmp");
            this.imageList1.Images.SetKeyName(4, "property.bmp");
            // 
            // uc_script_editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewItems);
            this.Controls.Add(this.listBoxAutoComplete);
            this.Controls.Add(this.textBoxTooltip);
            this.Controls.Add(this.txtScript);
            this.Name = "uc_script_editor";
            this.Load += new System.EventHandler(this.uc_script_editor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtScript;
        private System.Windows.Forms.TextBox textBoxTooltip;
        private uc_intellisense_listbox listBoxAutoComplete;
        private System.Windows.Forms.TreeView treeViewItems;
        private System.Windows.Forms.ImageList imageList1;
    }
}
