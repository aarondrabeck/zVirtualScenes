namespace zVirtualScenesApplication.Forms
{
    partial class AddEditSceneDeviceCMD
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditSceneDeviceCMD));
            this.btnSave = new System.Windows.Forms.Button();
            this.comboBoxCommands = new System.Windows.Forms.ComboBox();
            this.panelUserInputControls = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.radioBtnDeviceCMD = new System.Windows.Forms.RadioButton();
            this.radioBtnTypeCommand = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(256, 142);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // comboBoxCommands
            // 
            this.comboBoxCommands.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCommands.FormattingEnabled = true;
            this.comboBoxCommands.Location = new System.Drawing.Point(84, 48);
            this.comboBoxCommands.Name = "comboBoxCommands";
            this.comboBoxCommands.Size = new System.Drawing.Size(241, 21);
            this.comboBoxCommands.TabIndex = 14;
            this.comboBoxCommands.SelectedIndexChanged += new System.EventHandler(this.comboBoxTypeCommands_SelectedIndexChanged);
            // 
            // panelUserInputControls
            // 
            this.panelUserInputControls.Location = new System.Drawing.Point(84, 84);
            this.panelUserInputControls.Name = "panelUserInputControls";
            this.panelUserInputControls.Size = new System.Drawing.Size(332, 38);
            this.panelUserInputControls.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Commands";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(341, 142);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // radioBtnDeviceCMD
            // 
            this.radioBtnDeviceCMD.AutoSize = true;
            this.radioBtnDeviceCMD.Checked = true;
            this.radioBtnDeviceCMD.Location = new System.Drawing.Point(21, 12);
            this.radioBtnDeviceCMD.Name = "radioBtnDeviceCMD";
            this.radioBtnDeviceCMD.Size = new System.Drawing.Size(150, 17);
            this.radioBtnDeviceCMD.TabIndex = 20;
            this.radioBtnDeviceCMD.TabStop = true;
            this.radioBtnDeviceCMD.Text = "Device Specific Command";
            this.radioBtnDeviceCMD.UseVisualStyleBackColor = true;
            this.radioBtnDeviceCMD.CheckedChanged += new System.EventHandler(this.radioBtnDeviceCMD_CheckedChanged);
            // 
            // radioBtnTypeCommand
            // 
            this.radioBtnTypeCommand.AutoSize = true;
            this.radioBtnTypeCommand.Location = new System.Drawing.Point(244, 12);
            this.radioBtnTypeCommand.Name = "radioBtnTypeCommand";
            this.radioBtnTypeCommand.Size = new System.Drawing.Size(136, 17);
            this.radioBtnTypeCommand.TabIndex = 21;
            this.radioBtnTypeCommand.Text = "Device Type Command";
            this.radioBtnTypeCommand.UseVisualStyleBackColor = true;
            this.radioBtnTypeCommand.CheckedChanged += new System.EventHandler(this.radioBtnTypeCommand_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioBtnTypeCommand);
            this.groupBox2.Controls.Add(this.radioBtnDeviceCMD);
            this.groupBox2.Location = new System.Drawing.Point(12, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(404, 36);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Arg";
            // 
            // AddEditSceneDeviceCMD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 171);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panelUserInputControls);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxCommands);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(351, 195);
            this.Name = "AddEditSceneDeviceCMD";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scene Command";
            this.Load += new System.EventHandler(this.AddEditSceneCMD_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox comboBoxCommands;
        private System.Windows.Forms.Panel panelUserInputControls;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton radioBtnDeviceCMD;
        private System.Windows.Forms.RadioButton radioBtnTypeCommand;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
    }
}