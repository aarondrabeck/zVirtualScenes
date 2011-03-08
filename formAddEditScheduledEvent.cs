using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zVirtualScenesApplication
{
    public partial class formScheduledTasks : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;

        public formScheduledTasks(formzVirtualScenes zVirtualScenesMain)
        {
            InitializeComponent();
            this._zVirtualScenesMain = zVirtualScenesMain;
        }

        private void formScheduledTasks_Load(object sender, EventArgs e)
        {
            comboBox_Frequency.DataSource = Enum.GetNames(typeof(Task.frequencys));
            dataListTasks.DataSource = _zVirtualScenesMain.MasterTimerEvents;
            comboBox_Actions.DataSource = _zVirtualScenesMain.MasterScenes;

            //Add default item if list is empty
            if (_zVirtualScenesMain.MasterTimerEvents.Count < 1)
                AddNewTask();
            else
                dataListTasks.SelectedIndex = 0;
            
        }

        private void AddNewTask()
        {
            Task newevent = new Task();
            _zVirtualScenesMain.MasterTimerEvents.Add(newevent);    
        
            //Select it 
            dataListTasks.SelectedObject = newevent;
        }

        private void comboBox_Frequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Frequency.SelectedIndex)
            {
                case (int)Task.frequencys.Daily:
                    groupBoxDaily.Visible = true;
                    groupBox_Weekly.Visible = false;
                    break;
                case (int)Task.frequencys.Weekly:
                    groupBoxDaily.Visible = false;
                    groupBox_Weekly.Visible = true;
                    break;
                case (int)Task.frequencys.OneTime:
                    groupBoxDaily.Visible = false;
                    groupBox_Weekly.Visible = false;
                    break;
            }            
        }

        private void LoadGui(Task Task)
        {
            textBox_TaskName.Text = Task.Name;
            comboBox_Frequency.SelectedIndex = (int)Task.Frequency;
            textBox_DaysRecur.Text = Task.RecurDays.ToString();
            checkBox_Enabled.Checked = Task.Enabled;
            dateTimePickerStart.Value = Task.StartTime;
            textBox_RecurWeeks.Text = Task.RecurWeeks.ToString();
            checkBox_RecurMonday.Checked = Task.RecurMonday;
            checkBox_RecurTuesday.Checked = Task.RecurTuesday;
            checkBox_RecurWednesday.Checked = Task.RecurWednesday;
            checkBox_RecurThursday.Checked = Task.RecurThursday;
            checkBox_RecurFriday.Checked = Task.RecurFriday;
            checkBox_RecurSaturday.Checked = Task.RecurSaturday; 
            checkBox_RecurSunday.Checked = Task.RecurSunday;

            //Look for Scene in Master Scenes, if it was deleted then set index to -1
            bool found = false;
            foreach (Scene scene in _zVirtualScenesMain.MasterScenes)
            {
                if (Task.SceneID == scene.ID)
                {
                    found = true;
                    comboBox_Actions.SelectedItem = scene;
                }
            }
            if (!found)
                comboBox_Actions.SelectedIndex = -1;

            
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (dataListTasks.SelectedObject != null)
            {
                
                Task SelectedTask = (Task)dataListTasks.SelectedObject;
             
                //Task Name
                if (textBox_TaskName.Text != "")
                    SelectedTask.Name = textBox_TaskName.Text;
                else
                {
                    MessageBox.Show("Invalid Name.", _zVirtualScenesMain.ProgramName);
                    return;
                }

                //Frequency
                SelectedTask.Frequency = (Task.frequencys)Enum.Parse(typeof(Task.frequencys), comboBox_Frequency.SelectedItem.ToString());

                //Endabled 
                SelectedTask.Enabled = checkBox_Enabled.Checked;

                //DateTime
                SelectedTask.StartTime = dateTimePickerStart.Value;

                //Recur Days 
                if (comboBox_Frequency.SelectedValue.ToString() == Enum.GetName(typeof(Task.frequencys),Task.frequencys.Daily))
                {
                    try
                    {
                        SelectedTask.RecurDays = Convert.ToInt32(textBox_DaysRecur.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid Days.", _zVirtualScenesMain.ProgramName);
                        return;
                    }
                }
                else if (comboBox_Frequency.SelectedValue.ToString() == Enum.GetName(typeof(Task.frequencys),Task.frequencys.Weekly))
                {
                    #region Weekly 

                    try
                    {
                        SelectedTask.RecurWeeks = Convert.ToInt32(textBox_RecurWeeks.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid Weeks.", _zVirtualScenesMain.ProgramName);
                        return;
                    }
                    
                    SelectedTask.RecurMonday = checkBox_RecurMonday.Checked;
                    SelectedTask.RecurTuesday = checkBox_RecurTuesday.Checked;
                    SelectedTask.RecurWednesday = checkBox_RecurWednesday.Checked ;
                    SelectedTask.RecurThursday = checkBox_RecurThursday.Checked  ;
                    SelectedTask.RecurFriday = checkBox_RecurFriday.Checked    ;
                    SelectedTask.RecurSaturday = checkBox_RecurSaturday.Checked  ;
                    SelectedTask.RecurSunday = checkBox_RecurSunday.Checked   ;

                    #endregion

                }

                //Action
                if(comboBox_Actions.SelectedIndex != -1)
                {
                    Scene SelectedScene = (Scene)comboBox_Actions.SelectedItem;
                    SelectedTask.SceneID = SelectedScene.ID;
                }
                else
                {
                    MessageBox.Show("Please select a scene.", _zVirtualScenesMain.ProgramName);
                    return;
                }


                //Refresh List
                dataListTasks.DataSource = null;
                dataListTasks.DataSource = _zVirtualScenesMain.MasterTimerEvents;
                try
                {
                    dataListTasks.SelectedObject = SelectedTask;
                }
                catch {}
            }

        }

        private void dataListTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataListTasks.SelectedObject != null)
                LoadGui((Task)dataListTasks.SelectedObject);
        }

        private void button_NewTask_Click(object sender, EventArgs e)
        {
            AddNewTask();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            if (dataListTasks.SelectedIndex != -1)
            {
                int selectionIndex = dataListTasks.SelectedIndex;
                _zVirtualScenesMain.MasterTimerEvents.Remove((Task)dataListTasks.SelectedObject);
                try
                {
                    if(selectionIndex !=0)
                        dataListTasks.SelectedIndex = selectionIndex - 1;
                    else
                        dataListTasks.SelectedIndex = selectionIndex;
                }
                catch { }
            }

            else
                MessageBox.Show("Please select a task to delete.", _zVirtualScenesMain.ProgramName);
        }
    }       
}
