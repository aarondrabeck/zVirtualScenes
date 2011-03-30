namespace zVirtualScenesApplication
{
    partial class uc_setting_general
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
            this.txt_loglineslimit = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.Label_SunriseSet = new System.Windows.Forms.Label();
            this.checkBoxEnableNOAA = new System.Windows.Forms.CheckBox();
            this.textBox_Latitude = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Longitude = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxRepolling = new System.Windows.Forms.TextBox();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_loglineslimit
            // 
            this.txt_loglineslimit.Location = new System.Drawing.Point(226, 32);
            this.txt_loglineslimit.Name = "txt_loglineslimit";
            this.txt_loglineslimit.Size = new System.Drawing.Size(71, 20);
            this.txt_loglineslimit.TabIndex = 30;
            this.txt_loglineslimit.Leave += new System.EventHandler(this.txt_loglineslimit_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "ZWave Device Repolling Interval (seconds):";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(112, 35);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(108, 13);
            this.label33.TabIndex = 29;
            this.label33.Text = "Log Size Limit (lines): ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label32);
            this.groupBox4.Controls.Add(this.label31);
            this.groupBox4.Controls.Add(this.Label_SunriseSet);
            this.groupBox4.Controls.Add(this.checkBoxEnableNOAA);
            this.groupBox4.Controls.Add(this.textBox_Latitude);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.textBox_Longitude);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(6, 72);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(514, 110);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Automatic Sunrise and Sunsent Scene Activation";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label32.Location = new System.Drawing.Point(252, 72);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(249, 13);
            this.label32.TabIndex = 37;
            this.label32.Text = "Decimal notation.  Example: -113.06166666666667";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label31.Location = new System.Drawing.Point(252, 46);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(240, 13);
            this.label31.TabIndex = 36;
            this.label31.Text = "Decimal notation.  Example: 37.67722222222222";
            // 
            // Label_SunriseSet
            // 
            this.Label_SunriseSet.AutoSize = true;
            this.Label_SunriseSet.Location = new System.Drawing.Point(204, 21);
            this.Label_SunriseSet.Name = "Label_SunriseSet";
            this.Label_SunriseSet.Size = new System.Drawing.Size(10, 13);
            this.Label_SunriseSet.TabIndex = 17;
            this.Label_SunriseSet.Text = "-";
            // 
            // checkBoxEnableNOAA
            // 
            this.checkBoxEnableNOAA.AutoSize = true;
            this.checkBoxEnableNOAA.Location = new System.Drawing.Point(10, 20);
            this.checkBoxEnableNOAA.Name = "checkBoxEnableNOAA";
            this.checkBoxEnableNOAA.Size = new System.Drawing.Size(135, 17);
            this.checkBoxEnableNOAA.TabIndex = 7;
            this.checkBoxEnableNOAA.Text = "Enable Sunrise/Sunset";
            this.checkBoxEnableNOAA.UseVisualStyleBackColor = true;
            this.checkBoxEnableNOAA.Leave += new System.EventHandler(this.checkBoxEnableNOAA_Leave);
            // 
            // textBox_Latitude
            // 
            this.textBox_Latitude.Location = new System.Drawing.Point(70, 43);
            this.textBox_Latitude.Name = "textBox_Latitude";
            this.textBox_Latitude.Size = new System.Drawing.Size(176, 20);
            this.textBox_Latitude.TabIndex = 16;
            this.textBox_Latitude.Leave += new System.EventHandler(this.textBox_Latitude_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 26);
            this.label4.TabIndex = 15;
            this.label4.Text = "Latitude:\r\n\r\n";
            // 
            // textBox_Longitude
            // 
            this.textBox_Longitude.Location = new System.Drawing.Point(70, 69);
            this.textBox_Longitude.Name = "textBox_Longitude";
            this.textBox_Longitude.Size = new System.Drawing.Size(176, 20);
            this.textBox_Longitude.TabIndex = 14;
            this.textBox_Longitude.Leave += new System.EventHandler(this.textBox_Longitude_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Longitude:";
            // 
            // textBoxRepolling
            // 
            this.textBoxRepolling.Location = new System.Drawing.Point(226, 6);
            this.textBoxRepolling.Name = "textBoxRepolling";
            this.textBoxRepolling.Size = new System.Drawing.Size(71, 20);
            this.textBoxRepolling.TabIndex = 28;
            this.textBoxRepolling.Leave += new System.EventHandler(this.textBoxRepolling_Leave);
            // 
            // uc_setting_general
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt_loglineslimit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.textBoxRepolling);
            this.Name = "uc_setting_general";
            this.Size = new System.Drawing.Size(532, 198);
            this.Load += new System.EventHandler(this.uc_setting_general_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_loglineslimit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label Label_SunriseSet;
        private System.Windows.Forms.CheckBox checkBoxEnableNOAA;
        private System.Windows.Forms.TextBox textBox_Latitude;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_Longitude;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxRepolling;
    }
}
