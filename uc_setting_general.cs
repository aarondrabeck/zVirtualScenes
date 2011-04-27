using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zVirtualScenesApplication
{
    public partial class uc_setting_general : UserControl
    {
        private formzVirtualScenes formzVirtualScenesMain;

        public uc_setting_general()
        { 
            InitializeComponent();
        }

        public void LoadSettings(formzVirtualScenes form)
        {
            this.formzVirtualScenesMain = form;

            //General Settings 
            textBoxRepolling.Text = formzVirtualScenesMain.zVScenesSettings.PollingInterval.ToString();
            txt_loglineslimit.Text = formzVirtualScenesMain.zVScenesSettings.LongLinesLimit.ToString();
            //NOAA
            checkBoxEnableNOAA.Checked = formzVirtualScenesMain.zVScenesSettings.EnableNOAA;
            textBox_Latitude.Text = formzVirtualScenesMain.zVScenesSettings.Latitude.ToString();
            textBox_Longitude.Text = formzVirtualScenesMain.zVScenesSettings.Longitude.ToString();
        }

        private void DisplaySunset()
        {
            DateTime date = DateTime.Today;
            bool isSunrise = false;
            bool isSunset = false;
            DateTime sunrise = DateTime.Now;
            DateTime sunset = DateTime.Now;

            SunTimes.Instance.CalculateSunRiseSetTimes(formzVirtualScenesMain.zVScenesSettings.Latitude, formzVirtualScenesMain.zVScenesSettings.Longitude, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);
            Label_SunriseSet.Text = "Today's Sunrise: " + sunrise.ToString("T") + ", Sunset: " + sunset.ToString("T");
        }

        private void checkBoxEnableNOAA_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.EnableNOAA = checkBoxEnableNOAA.Checked;
            DisplaySunset();
        }

        private void textBoxRepolling_Leave(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.PollingInterval = Convert.ToInt32(textBoxRepolling.Text);

                if (formzVirtualScenesMain.zVScenesSettings.PollingInterval < 0)
                    throw new Exception("");

                formzVirtualScenesMain.ControlThinkInt.UpdateInterval();
            }
            catch
            {
                MessageBox.Show("Invalid Polling Interval.", formzVirtualScenesMain.ProgramName);
                textBoxRepolling.Text = formzVirtualScenesMain.zVScenesSettings.PollingInterval.ToString();
            }
        }

        private void txt_loglineslimit_Leave(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.LongLinesLimit = Convert.ToInt32(txt_loglineslimit.Text);
            }
            catch
            {
                MessageBox.Show("Invalid Log Line Limit.", formzVirtualScenesMain.ProgramName);
                txt_loglineslimit.Text = formzVirtualScenesMain.zVScenesSettings.LongLinesLimit.ToString();
            }
        }

        private void textBox_Latitude_Leave(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.Latitude = Convert.ToDouble(textBox_Latitude.Text);
                DisplaySunset();
            }
            catch
            {
                MessageBox.Show("Invalid Latitude.", formzVirtualScenesMain.ProgramName);
                textBox_Latitude.Text = formzVirtualScenesMain.zVScenesSettings.Latitude.ToString();
            }
           
        }

        private void textBox_Longitude_Leave(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.Longitude = Convert.ToDouble(textBox_Longitude.Text);
                DisplaySunset();
            }
            catch
            {
                MessageBox.Show("Invalid Longitude.", formzVirtualScenesMain.ProgramName);
                textBox_Longitude.Text = formzVirtualScenesMain.zVScenesSettings.Longitude.ToString();
            }
        }

        private void uc_setting_general_Load(object sender, EventArgs e)
        {
            DisplaySunset();
        }
        
    }
}
