using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenZWaveDotNet;
using zVirtualScenesAPI;
using zVirtualScenesCommon;
using zVirtualScenesCommon.Util;
using zVirtualScenesCommon.Entity;

namespace OpenZWavePlugin.Forms
{
	public partial class ControllerCommandDlg : Form
	{
        static private ZWManager m_manager;
        static private ControllerCommandDlg _controllercommanddlg;
        static private UInt32 _homeid;
        static private ManagedControllerStateChangedHandler m_controllerStateChangedHandler = new ManagedControllerStateChangedHandler(ControllerCommandDlg.MyControllerStateChangedHandler);
		static private ZWControllerCommand _zwcontrollercommand;
        static private Byte _nodeid; 

        public ControllerCommandDlg(ZWManager _manager, UInt32 homeId, ZWControllerCommand _op, Byte nodeId)
		{            
            m_manager = _manager;
            _homeid = homeId;
            _zwcontrollercommand = _op;
            _nodeid = nodeId;
            _controllercommanddlg = this;

            InitializeComponent();

			switch (_zwcontrollercommand)
			{
                case ZWControllerCommand.AddController:
				{
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Add Controller";
					this.label1.Text = "Put the target controller into receive configuration mode.\nThe PC Z-Wave Controller must be within 2m of the controller being added.";
					break;
				}
                case ZWControllerCommand.AddDevice:
				{
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Add Device";
					this.label1.Text = "Press the program button on the Z-Wave device to add it to the network.\nFor security reasons, the PC Z-Wave Controller must be close to the device being added.";
					break;
				}
                case ZWControllerCommand.CreateNewPrimary:
				{
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Create New Primary Controller";
					this.label1.Text = "Put the target controller into receive configuration mode.\nThe PC Z-Wave Controller must be within 2m of the controller that is being made the primary.";
					break;
				}
                case ZWControllerCommand.ReceiveConfiguration:
				{
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Receive Configuration";
					this.label1.Text = "Transfering the network configuration\nfrom another controller.\n\nPlease bring the other controller within 2m of the PC controller and set it to send its network configuration.";
					break;
				}
                case ZWControllerCommand.RemoveController:
				{
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - RemoveController";
					this.label1.Text = "Put the target controller into receive configuration mode.\nThe PC Z-Wave Controller must be within 2m of the controller being added.";
					break;
				}
                case ZWControllerCommand.RemoveDevice:
				{
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Remove Device";
					this.label1.Text = "Press the program button on the Z-Wave device to remove it from the network.\nFor security reasons, the PC Z-Wave Controller must be close to the device being removed.";
					break;
				}
                case ZWControllerCommand.TransferPrimaryRole:
				{
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Transfer Primary Role";
					this.label1.Text = "Transfering the primary role\nto another controller.\n\nPlease bring the new controller within 2m of the PC controller and set it to receive the network configuration.";
					break;
				}
                case ZWControllerCommand.HasNodeFailed:
                {
                    this.ButtonCancel.Enabled = false;
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Has Node Failed";
                    this.label1.Text = "Testing whether the node has failed.\nThis command cannot be cancelled.";
                    break;
                }
                case ZWControllerCommand.RemoveFailedNode:
                {
                    this.ButtonCancel.Enabled = false;
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Remove Failed Node";
                    this.label1.Text = "Removing the failed node from the controller's list.\nThis command cannot be cancelled.";
					break;
                }
                case ZWControllerCommand.ReplaceFailedNode:
                {
                    this.ButtonCancel.Enabled = false;
                    this.Text = zvsEntityControl.GetProgramNameAndVersion + " - Replacing Failed Node";
                    this.label1.Text = "Testing the failed node.\nThis command cannot be cancelled.";
                    break;
                }
            }

            m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
            if (!m_manager.BeginControllerCommand(_homeid, _zwcontrollercommand, false, _nodeid))
            {
                m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
            }
		}

        public static void MyControllerStateChangedHandler( ZWControllerState state )
	    {
		    // Handle the controller state notifications here.
            bool complete = false;
            String dlgText = "";
            bool buttonEnabled = true;

            switch (state)
		    {
		        case ZWControllerState.Waiting:
		        {
	                // Display a message to tell the user to press the include button on the controller
                    if (_zwcontrollercommand == ZWControllerCommand.ReplaceFailedNode)
                    {
                        dlgText = "Press the program button on the replacement Z-Wave device to add it to the network.\nFor security reasons, the PC Z-Wave Controller must be close to the device being added.\nThis command cannot be cancelled.";
                    }
                    break;
		        }
		        case ZWControllerState.InProgress:
		        {
		            // Tell the user that the controller has been found and the adding process is in progress.
                    Logger.WriteToLog(Urgency.INFO, _zwcontrollercommand.ToString() + " in progress...", "OPENZWAVE");
                    dlgText = "Please wait...";
                    buttonEnabled = false;
                    break;
		        }
		        case ZWControllerState.Completed:
		        {
		            // Tell the user that the controller has been successfully added.
		            // The command is now complete
                    Logger.WriteToLog(Urgency.INFO, _zwcontrollercommand.ToString() + " command complete.", "OPENZWAVE");
                    dlgText = "Command Completed OK.";
                    complete = true;
		            break;
		        }
		        case ZWControllerState.Failed:
		        {
		            // Tell the user that the controller addition process has failed.
		            // The command is now complete
                    Logger.WriteToLog(Urgency.INFO, _zwcontrollercommand.ToString() + " command failed.", "OPENZWAVE");
                    dlgText = "Command Failed.";
                    complete = true;
		            break;
		        }
                case ZWControllerState.NodeOK:
                {
                    Logger.WriteToLog(Urgency.INFO, _zwcontrollercommand.ToString() + " node has not failed.", "OPENZWAVE");
                    dlgText = "Node has not failed.";
                    complete = true;
                    break;
                }
                case ZWControllerState.NodeFailed:
                {
                    Logger.WriteToLog(Urgency.INFO, _zwcontrollercommand.ToString() + " node has failed.", "OPENZWAVE");
                    dlgText = "Node has failed.";
                    complete = true;
                    break;
                }
		    }

            if (dlgText != "")
            {
                _controllercommanddlg.SetDialogText(dlgText);
            }

            _controllercommanddlg.SetButtonEnabled(buttonEnabled);

            if (complete)
            {
                _controllercommanddlg.SetButtonText( "OK" );

                // Remove the event handler
                m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
            }
		}

        private void SetDialogText(String text)
        {
            if (_controllercommanddlg.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate() { SetDialogText(text); }));
            }
            else
            {
                _controllercommanddlg.label1.Text = text;
            }
        }

        private void SetButtonText(String text)
        {
            if (_controllercommanddlg.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate() { SetButtonText(text); }));
            }
            else
            {
                _controllercommanddlg.ButtonCancel.Text = text;
            }
        }

        private void SetButtonEnabled(bool enabled)
        {
            if (_controllercommanddlg.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate() { SetButtonEnabled(enabled); }));
            }
            else
            {
                _controllercommanddlg.ButtonCancel.Enabled = enabled;
            }
        }

		private void ButtonCancel_Click( object sender, EventArgs e )
		{
            if (ButtonCancel.Text != "OK")
            {
                // Remove the event handler
                m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;

                // Cancel the operation
                if (m_manager.CancelControllerCommand(_homeid))
                {
                    Logger.WriteToLog(Urgency.INFO, _zwcontrollercommand.ToString() + " cancelled by user.","OPENZWAVE");
                }
                else
                {
                    Logger.WriteToLog(Urgency.INFO, "Failed to cancel "+_zwcontrollercommand.ToString() + ".", "OPENZWAVE");
                }
            }

			// Close the dialog
			Close();
		}
 	}
}