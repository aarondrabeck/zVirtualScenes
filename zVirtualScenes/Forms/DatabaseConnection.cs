using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI;
using zVirtualScenesCommon;

namespace zVirtualScenesApplication.Forms
{
    public partial class DatabaseConnection : Form
    {
        public DatabaseConnection()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void SaveUserInput()
        {
            DatabaseControl.db_server = textBoxIP.Text;
            DatabaseControl.db_port = textBoxPort.Text;
            DatabaseControl.db_database = cmbDatabases.Text;
            DatabaseControl.db_user = textBoxUsername.Text;
            DatabaseControl.db_password = textBoxPass.Text;
        }       

        private void RefreshDatabaseList()
        {
            BindingList<string> databases = DatabaseControl.GetDatabases();
            cmbDatabases.DataSource = databases;
            
            if(databases == null)    
                MessageBox.Show("Connection Error!\n\n" + DatabaseControl.LastDBError, API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                if (cmbDatabases.Items.Contains("zvirtualscenes"))
                    cmbDatabases.SelectedIndex = cmbDatabases.Items.IndexOf("zvirtualscenes"); 
            }
        }

        private void DatabaseConnection_Load(object sender, EventArgs e)
        {
            textBoxIP.Text = DatabaseControl.db_server;
            textBoxPort.Text = DatabaseControl.db_port;            
            textBoxUsername.Text = DatabaseControl.db_user;
            textBoxPass.Text = DatabaseControl.db_password;
            RefreshDatabaseList();
        }

        private void textBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                buttonOK_Click((object)sender, (EventArgs)e);
                e.Handled = true;
            }

            //allow only numbers for port
            if ((Keys)e.KeyChar != Keys.Back)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                    e.Handled = true;
            }
        }

        private void textBoxIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                buttonOK_Click((object)sender, (EventArgs)e);
                e.Handled = true;
            }
        }

        private void textBoxDBName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                buttonOK_Click((object)sender, (EventArgs)e);
                e.Handled = true;
            }
        }

        private void textBoxUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                buttonOK_Click((object)sender, (EventArgs)e);
                e.Handled = true;
            }

        }

        private void textBoxPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                buttonOK_Click((object)sender, (EventArgs)e);
                e.Handled = true;
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            SaveUserInput();

            switch (MessageBox.Show("Warning this will overwrite any existing 'zvirtualscenes' database!\n\nAre you sure you would like to continue?", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    string result = DatabaseControl.installBaseDB();
                    if (string.IsNullOrEmpty(result))
                    {
                        if (MessageBox.Show("Database Imported!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            RefreshDatabaseList();
                        }
                    }
                    else
                    {
                        MessageBox.Show(result, API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);                        
                    }
                    break;
            }
        }        

        private void buttonRefreshList_Click(object sender, EventArgs e)
        {
            SaveUserInput();
            RefreshDatabaseList();
        }

        private bool DBReady()
        {
            string errors = DatabaseControl.GetConnectionErrors();
            if (string.IsNullOrEmpty(errors))
            {
                string VerionOfDatabase = DatabaseControl.GetOutdatedDbVersion();
                if (VerionOfDatabase != null)
                {                    
                    //PROMT FOR UPGRADEs
                    #region Upgrade to DB 2.1
                    if (VerionOfDatabase.Equals("2.0 Base") && CurrentVersion.RequiredDatabaseVersion.Equals("2.1"))
                    {
                        if (MessageBox.Show("Would you like to upgrade your database to 2.1?", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            string error = DatabaseControl.upgradeBaseDB("20to21.sql");
                            if (string.IsNullOrEmpty(error))
                            {
                                MessageBox.Show("Upgrade to 2.1 Complete.", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return true;
                            }
                            else
                            {
                                MessageBox.Show(error, API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                        }
                        return false;
                    }
                    #endregion
                    #region Upgrade to DB 2.2
                    else if (VerionOfDatabase.Equals("2.1") && CurrentVersion.RequiredDatabaseVersion.Equals("2.2"))
                    {
                        if (MessageBox.Show("Would you like to upgrade your database to 2.2?", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            string error = DatabaseControl.upgradeBaseDB("21to22.sql");
                            if (string.IsNullOrEmpty(error))
                            {
                                MessageBox.Show("Upgrade to 2.2 Complete.", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return true;
                            }
                            else
                            {
                                MessageBox.Show(error, API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                        }
                        return false;
                    }
                    #endregion
                    #region Upgrade to DB 2.3
                    else if (VerionOfDatabase.Equals("2.2") && CurrentVersion.RequiredDatabaseVersion.Equals("2.3"))
                    {
                        if (MessageBox.Show("Would you like to upgrade your database to 2.3?", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            string error = DatabaseControl.upgradeBaseDB("22to23.sql");
                            if (string.IsNullOrEmpty(error))
                            {
                                MessageBox.Show("Upgrade to 2.3 Complete.", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return true;
                            }
                            else
                            {
                                MessageBox.Show(error, API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                        }
                        return false;
                    }
                    #endregion
                    //TODO: ADD FUTURE UPGRADE SCRIPTS HERE AS ELSE IF's
                    else
                    {
                        MessageBox.Show("This database version is not compatable with this program and there is no known upgrade file.", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }                    
                }
                
                else
                {
                    return true;
                }
            }
            else
            {
                MessageBox.Show("Error!\n\n" + errors, API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }      
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            SaveUserInput();

            if(DBReady())
                MessageBox.Show("Connection successful and database is up to date.", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveUserInput();

            if(DBReady())
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
    }
}
