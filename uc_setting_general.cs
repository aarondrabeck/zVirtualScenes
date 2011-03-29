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

            textBoxRepolling.LostFocus += new EventHandler(textBoxRepolling_LostFocus);
            txt_loglineslimit.LostFocus += new EventHandler(txt_loglineslimit_LostFocus);
            textBox_Longitude.LostFocus += new EventHandler(textBox_Longitude_LostFocus);
            textBox_Latitude.LostFocus += new EventHandler(textBox_Latitude_LostFocus);
            checkBoxEnableNOAA.CheckedChanged += new EventHandler(checkBoxEnableNOAA_CheckedChanged);
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

        private void textBoxRepolling_LostFocus(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.PollingInterval = Convert.ToInt32(textBoxRepolling.Text);

                if (formzVirtualScenesMain.zVScenesSettings.PollingInterval < 0)
                    throw new Exception("");

                formzVirtualScenesMain.refresher.UpdateInterval();
            }
            catch
            {                
                MessageBox.Show("Invalid Polling Interval.", formzVirtualScenesMain.ProgramName);
                textBoxRepolling.Text = formzVirtualScenesMain.zVScenesSettings.PollingInterval.ToString();
            }
        }

        private void txt_loglineslimit_LostFocus(object sender, EventArgs e)
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

        private void textBox_Longitude_LostFocus(object sender, EventArgs e)
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
 
        private void textBox_Latitude_LostFocus(object sender, EventArgs e)
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

        private void checkBoxEnableNOAA_CheckedChanged(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.EnableNOAA = checkBoxEnableNOAA.Checked;
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
    }
}
