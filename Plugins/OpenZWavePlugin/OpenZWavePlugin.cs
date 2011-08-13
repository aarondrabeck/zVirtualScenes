using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using OpenZWaveDotNet;
using zVirtualScenesAPI;
using System.Threading;
using System.ComponentModel;
using zVirtualScenesAPI.Structs;
using System.Windows.Forms;
using OZWForm;

namespace OpenZWavePlugin
{
    [Export(typeof(Plugin))]
    public class OpenZWavePlugin : Plugin
    {
        private ZWManager m_manager;
        private ZWOptions m_options;
        ZWNotification m_notification;
        UInt32 m_homeId = 0;
        List<Node> m_nodeList = new List<Node>();
        private bool FinishedInitialPoll = false;

        public OpenZWavePlugin() : base("OPENZWAVE")
        {
            PluginName = "OpenZWave";
        }

        protected override bool StartPlugin()
        {
            try
            {
                API.WriteToLog(Urgency.INFO, PluginName + " plugin started.");
                
                // Create the Options
                m_options = new ZWOptions();
                m_options.Create(Environment.CurrentDirectory + @"\plugins\config\", 
                                 Environment.CurrentDirectory + @"\plugins\", @"");
                m_options.Lock();
                m_manager = new ZWManager();
                m_manager.Create();
                m_manager.OnNotification += NotificationHandler;
                
                bool useHID = false;
                bool.TryParse(API.GetSetting("Use HID"), out useHID);

                if (!useHID)
                {
                    if (API.GetSetting("Com Port") != "0")
                    {
                        m_manager.AddDriver(@"\\.\COM" + API.GetSetting("Com Port"));
                    }
                }
                else
                {
                    //m_manager.AddHidDriver();
                }
                
                int pollint = 0;
                int.TryParse(API.GetSetting("Polling Interval"), out pollint);
                if (pollint != 0)
                {                    
                    m_manager.SetPollInterval(pollint);
                }

                IsReady = true;
            }
            catch (Exception e)
            {
                API.WriteToLog(Urgency.ERROR, e.Message);
                return false;
            }

            return true;
        }

        protected override bool StopPlugin()
        {
            API.WriteToLog(Urgency.INFO, PluginName + " plugin stopped.");            
            m_manager.OnNotification -= NotificationHandler;
            m_manager.Destroy();
            m_options.Destroy();
            IsReady = false;
            return true;            
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            switch (settingName)
            {
                case "Com Port":
                    // Set the port here
                    break;
                case "Polling Interval":
                    // Set the polling interval here
                    break;
            }
        }
        
        public override void ActivateGroup(string GroupName)
        {
            DataTable dt = API.Groups.GetGroupObjects(GroupName);

            foreach (DataRow dr in dt.Rows)
            {
                int objectID = 0;
                int.TryParse(dr["id"].ToString(), out objectID);

                DataRow drObj = API.Object.GetObject(objectID);

                switch (drObj["txt_object_type"].ToString())
                {
                    case "SWITCH":
                        m_manager.SetNodeOn(m_homeId, Convert.ToByte(drObj["node_id"].ToString()));
                        break;
                    case "DIMMER":
                        byte defaultonlevel = 99;
                        byte.TryParse(API.Object.Properties.GetObjectPropertyValue(objectID, "DEFAULONLEVEL"), out defaultonlevel);
                    
                        m_manager.SetNodeLevel(m_homeId, Convert.ToByte(drObj["node_id"].ToString()), defaultonlevel);
                        break;         
                }
            }
        }

        public override void DeactivateGroup(string GroupName)
        {
            DataTable dt = API.Groups.GetGroupObjects(GroupName);

            foreach (DataRow dr in dt.Rows)
            {
                int objectID = 0;
                int.TryParse(dr["id"].ToString(), out objectID);

                DataRow drObj = API.Object.GetObject(objectID);

                switch (drObj["txt_object_type"].ToString())
                {
                    case "SWITCH":
                        m_manager.SetNodeOff(m_homeId, Convert.ToByte(drObj["node_id"].ToString()));
                        break;
                    case "DIMMER":
                        m_manager.SetNodeLevel(m_homeId, Convert.ToByte(drObj["node_id"].ToString()),0);
                        break;
                }
            }
        }

        public override void Initialize()
        {
            API.DefineSetting("Com Port", "7", ParamType.INTEGER, "The COM port that your z-wave controller is assigned to.");
            API.DefineSetting("Use HID", "false", ParamType.BOOL, "Use HID rather than COM port. (use this for ControlThink Sticks)");
            API.DefineSetting("Polling Interval", "360", ParamType.INTEGER, "The frequency in which devices are polled for level status on your network.  Set high to avoid excessive network traffic. ");

            //Controller
            API.InstallObjectType("CONTROLLER", true);
            API.NewObjectTypeCommand("CONTROLLER", "RESET",                 "Reset Controller", ParamType.NONE, "Earses all Z-Wave netowrks from your controller.");
            API.NewObjectTypeCommand("CONTROLLER", "ADDDEVICE",             "Add Device to Network", ParamType.NONE, "Adds a ZWave Device to your network.");
            API.NewObjectTypeCommand("CONTROLLER", "AddController",         "Add Controller to Network", ParamType.NONE, "Adds a ZWave Controller to your network.");
            API.NewObjectTypeCommand("CONTROLLER", "CreateNewPrimary",      "Create New Primary", ParamType.NONE, "Puts the target controller into receive configuration mode.");
            API.NewObjectTypeCommand("CONTROLLER", "ReceiveConfiguration",  "Receive Configuration", ParamType.NONE, "Receives the network configuration from another controller.");
            API.NewObjectTypeCommand("CONTROLLER", "RemoveController",      "Remove Controller", ParamType.NONE, "Removes a Controller from your netowrk.");
            API.NewObjectTypeCommand("CONTROLLER", "RemoveDevice",          "Remove Device", ParamType.NONE, "Removes a Device from your netowrk.");
            API.NewObjectTypeCommand("CONTROLLER", "TransferPrimaryRole",   "Transfer Primary Role", ParamType.NONE, "Transfers the primary role\nto another controller.");
            API.NewObjectTypeCommand("CONTROLLER", "HasNodeFailed",         "Has Node Failed", ParamType.NONE, "Tests whether a node has failed.");
            API.NewObjectTypeCommand("CONTROLLER", "RemoveFailedNode",      "Remove Failed Node", ParamType.NONE, "Removes the failed node from the controller's list.");
            API.NewObjectTypeCommand("CONTROLLER", "ReplaceFailedNode",     "Replace Failed Node", ParamType.NONE, "Tests the failed node.");
            
            // Switch
            API.InstallObjectType("SWITCH", true);
            API.NewObjectTypeCommand("SWITCH", "TURNON", "Turn On", ParamType.NONE, "Sets a switch to 100%");
            API.NewObjectTypeCommand("SWITCH", "TURNOFF", "Turn Off", ParamType.NONE, "Sets a switch to 0%");
            API.NewObjectTypeCommand("SWITCH", "MOMENTARY", "Turn On for X milliseconds", ParamType.INTEGER, "Turns a device on for the specified number of milliseconds and then turns the device back off.");
            
            // Dimmer
            API.InstallObjectType("DIMMER", true);
            API.NewObjectTypeCommand("DIMMER", "TURNON", "Turn On", ParamType.NONE, "Sets a dimmer to 100%");
            API.NewObjectTypeCommand("DIMMER", "TURNOFF", "Turn Off", ParamType.NONE, "Sets a dimmer to 0%");
            int presetCmdId = API.NewObjectTypeCommand("DIMMER", "SETPRESETLEVEL", "Set Level", ParamType.LIST, "Sets a dimmer to a preset level.");
            API.Commands.NewObjectTypeCommandOption(presetCmdId, "0%");
            API.Commands.NewObjectTypeCommandOption(presetCmdId, "20%");
            API.Commands.NewObjectTypeCommandOption(presetCmdId, "40%");
            API.Commands.NewObjectTypeCommandOption(presetCmdId, "60%");
            API.Commands.NewObjectTypeCommandOption(presetCmdId, "80%");
            API.Commands.NewObjectTypeCommandOption(presetCmdId, "100%");
            API.Commands.NewObjectTypeCommandOption(presetCmdId, "255");

            //Thermostat
            API.InstallObjectType("THERMOSTAT", true);
            API.NewObjectTypeCommand("THERMOSTAT", "SETENERGYMODE", "Set Energy Mode", ParamType.NONE, "Set thermosat to Energy Mode.");
            API.NewObjectTypeCommand("THERMOSTAT", "SETCONFORTMODE", "Set Confort Mode", ParamType.NONE, "Set thermosat to Confort Mode. (Run)");

            //Secure Keypad Door Lock
            API.InstallObjectType("DOORLOCK", true);


            API.Object.Properties.NewObjectProperty("DEFAULONLEVEL", "Level that an object is set to when using the on command.", "99", ParamType.BYTE);
            API.Object.Properties.NewObjectProperty("ENABLEREPOLLONLEVELCHANGE", "Repoll dimmer 3 seconds after a level change is received?", "true", ParamType.BOOL);
            
            //TODO: Make a new ObjectAPIProperty that is API specific for types of settings that applies OpenZWave Devices

            
        }     

        public override void ProcessCommand(QuedCommand cmd)
        {
            Command cmdInfo = API.Commands.GetCommand(cmd.CommandId, cmd.cmdtype);
            byte NodeID = 0;
            byte.TryParse(API.Object.GetNodeId(PluginName, cmd.ObjectId).ToString(), out NodeID);

            switch (cmdInfo.Name)
            {
                //Controller Specific 
                case "RESET":
                    m_manager.ResetController(m_homeId);
                    break;
                case "ADDDEVICE":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.AddDevice, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "AddController":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.AddController, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }                
                case "CreateNewPrimary":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.CreateNewPrimary, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "ReceiveConfiguration":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReceiveConfiguration, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "RemoveController":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveController, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "RemoveDevice":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveDevice, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "TransferPrimaryRole":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.TransferPrimaryRole, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "HasNodeFailed":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.HasNodeFailed, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "RemoveFailedNode":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveFailedNode, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }
                case "ReplaceFailedNode":
                {
                    ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReplaceFailedNode, NodeID);
                    dlg.ShowDialog();
                    dlg.Dispose();
                    break;
                }

                case "MOMENTARY":
                {
                    switch (API.Object.GetObjectType(cmd.ObjectId))
                    {                        
                        case "SWITCH":
                            int delay = 1000; 
                            int.TryParse(cmd.Argument, out delay);

                            m_manager.SetNodeOn(m_homeId, NodeID);
                            System.Timers.Timer t = new System.Timers.Timer();
                            t.Interval = delay;
                            t.Elapsed += (sender, e) =>
                            {
                                m_manager.SetNodeOff(m_homeId, NodeID);
                                t.Enabled = false;
                                t.Dispose();
                            };
                            t.Enabled = true;
                            break;
                    }
                    break;
                }
                case "TURNON":
                    switch (API.Object.GetObjectType(cmd.ObjectId))
                    {
                        case "SWITCH":
                            m_manager.SetNodeOn(m_homeId, NodeID);
                            break; 
                        case "DIMMER": 
                            byte defaultonlevel = 99;
                        byte.TryParse(API.Object.Properties.GetObjectPropertyValue(cmd.ObjectId, "DEFAULONLEVEL"), out defaultonlevel);
                        m_manager.SetNodeLevel(m_homeId, NodeID, defaultonlevel);
                            break;
                    }
                    break;
                case "TURNOFF":
                    m_manager.SetNodeOff(m_homeId, NodeID);
                    break;
                case "SETPRESETLEVEL":
                    switch (cmd.Argument)
                    {
                        case "0%":
                            m_manager.SetNodeLevel(m_homeId, NodeID, Convert.ToByte(0));
                            break;
                        case "20%":
                            m_manager.SetNodeLevel(m_homeId, NodeID, Convert.ToByte(20));
                            break;
                        case "40%":
                            m_manager.SetNodeLevel(m_homeId, NodeID, Convert.ToByte(40));
                            break;
                        case "60%":
                            m_manager.SetNodeLevel(m_homeId, NodeID, Convert.ToByte(60));
                            break;
                        case "80%":
                            m_manager.SetNodeLevel(m_homeId, NodeID, Convert.ToByte(80));
                            break;
                        case "100%":
                            m_manager.SetNodeLevel(m_homeId, NodeID, Convert.ToByte(100));
                            break;
                        case "255":
                            m_manager.SetNodeLevel(m_homeId, NodeID, Convert.ToByte(255));
                            break;   
                    }
                    break;                     
                case "SETENERGYMODE":
                    m_manager.SetNodeOff(m_homeId, NodeID);
                    break;
                case "SETCONFORTMODE":
                    m_manager.SetNodeOn(m_homeId, NodeID);
                    break;
            }

            if (cmdInfo.Name.Contains("DYNAMIC_CMD_")) 
            {
                //Get more info from this Command
                Node node = GetNode(m_homeId, NodeID);

                switch (cmdInfo.paramType)
                {
                    case ParamType.BYTE:
                        byte b = 0;
                        byte.TryParse(cmd.Argument, out b);
                        
                        foreach (Value v in node.Values)
                            if (m_manager.GetValueLabel(v.ValueID).Equals(cmdInfo.CustomData1))
                                m_manager.SetValue(v.ValueID, b);
                        break;
                    case ParamType.BOOL:
                        bool bo = true;
                        bool.TryParse(cmd.Argument, out bo);

                        foreach (Value v in node.Values)
                            if (m_manager.GetValueLabel(v.ValueID).Equals(cmdInfo.CustomData1))
                                m_manager.SetValue(v.ValueID, bo);
                        break;
                    case ParamType.DECIMAL:
                        float value = Convert.ToSingle(cmd.Argument);

                        foreach (Value v in node.Values)
                            if (m_manager.GetValueLabel(v.ValueID).Equals(cmdInfo.CustomData1))
                                m_manager.SetValue(v.ValueID, value);
                        break;
                    case ParamType.LIST:    
                    case ParamType.STRING:
                        foreach (Value v in node.Values)
                            if (m_manager.GetValueLabel(v.ValueID).Equals(cmdInfo.CustomData1))
                                m_manager.SetValue(v.ValueID, cmd.Argument);
                        break;
                    case ParamType.INTEGER:
                        int i = 0;
                        int.TryParse(cmd.Argument, out i);
                        
                        foreach (Value v in node.Values)
                            if (m_manager.GetValueLabel(v.ValueID).Equals(cmdInfo.CustomData1))
                                m_manager.SetValue(v.ValueID, i);
                        break;
                }
            }
        }

        public override void Repoll(string ObjId)
        {
            int node_id = API.GetObjectNodeId(ObjId);
            m_manager.RequestNodeState(m_homeId, Convert.ToByte(node_id));
        }

        #region ZWaveInterface Stuff
        public void NotificationHandler(ZWNotification notification)
        {
            m_notification = notification;
            NotificationHandler();
            m_notification = null;
        }

        private void NotificationHandler()
        {
            //osae.AddToLog("Notification: " + m_notification.GetType().ToString(), false);
            switch (m_notification.GetType())
            {
                    #region ValueAdded

                case ZWNotification.Type.ValueAdded:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        int objectId = API.GetObjectId(node.ID.ToString());
                        ZWValueID vid = m_notification.GetValueID();
                        Value value = new Value();
                        value.ValueID = vid;
                        value.Label = m_manager.GetValueLabel(vid);
                        value.Genre = vid.GetGenre().ToString();
                        value.Index = vid.GetIndex().ToString();
                        value.Type = vid.GetType().ToString();
                        value.CommandClassID = vid.GetCommandClassId().ToString();
                        value.Help = m_manager.GetValueHelp(vid);
                        node.AddValue(value);

                        string data = "";
                        m_manager.GetValueAsString(vid, out data);

                        
                        Console.WriteLine("OpenZWave Plugin | [ValueAdded] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data);
                        API.Object.Value.New(objectId, vid.GetId().ToString(), value.Label, value.Genre, value.Index, value.Type, value.CommandClassID, data);

                        #region Install Dynamic Commands

                        if (value.Genre != "System")
                        {
                            string objectType = API.Object.GetObjectType(objectId);
                            int objectTypeId = API.Object.GetObjectTypeId(objectId);
                           
                            ParamType pType = ParamType.NONE;

                            //Set param types for command
                            switch (vid.GetType())
                            {
                                case ZWValueID.ValueType.List:
                                    pType = ParamType.LIST;
                                    break;
                                case ZWValueID.ValueType.Byte:
                                    pType = ParamType.BYTE;
                                    break;
                                case ZWValueID.ValueType.Decimal:
                                    pType = ParamType.DECIMAL;
                                    break;
                                case ZWValueID.ValueType.Int:
                                    pType = ParamType.INTEGER;
                                    break;
                                case ZWValueID.ValueType.String:
                                    pType = ParamType.STRING;
                                    break;
                                case ZWValueID.ValueType.Short:
                                    pType = ParamType.SHORT;
                                    break;
                                case ZWValueID.ValueType.Bool:
                                    pType = ParamType.BOOL;
                                    break;
                            }

                            //Install the Node Specific Command
                            int order;
                            switch (value.Genre)
                            {
                                case "User":
                                    order = 1;
                                    break;
                                case "Config":
                                    order = 2;
                                    break;
                                default:
                                    order = 99;
                                    break;
                            }

                            API.Commands.NewObjectCommand(objectId, "DYNAMIC_CMD_" + value.Label.ToUpper(), "Set " + value.Label, pType, value.Help, value.Label, vid.GetId().ToString(), order);

                            //Special case for lists add additional info
                            if (vid.GetType() == ZWValueID.ValueType.List)
                            {
                                //Get the ID
                                int CommandID = API.Commands.GetObjectCommandId(objectId, "DYNAMIC_CMD_" + m_manager.GetValueLabel(vid).ToUpper());

                                //Install the allowed options/values
                                String[] options;
                                if (m_manager.GetValueListItems(vid, out options))
                                    foreach (string option in options)
                                        API.Commands.NewObjectCommandOption(CommandID, option);
                            }

                        }
                        #endregion

                        break;
                    }

                        
                    #endregion

                    #region ValueRemoved

                case ZWNotification.Type.ValueRemoved:
                    {
                        try
                        {
                            Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());     
                            ZWValueID vid = m_notification.GetValueID();
                            Value val = node.GetValue(vid);                            

                            Console.WriteLine("OpenZWave Plugin | [ValueRemoved] Node:" + node.ID + ",Label:" + m_manager.GetValueLabel(vid));

                            node.RemoveValue(val);

                            //TODO: Remove from values and command table
                        }
                        catch (Exception ex)
                        {
                            API.WriteToLog(Urgency.ERROR, "ValueRemoved error: " + ex.Message);
                        }
                        break;
                    }

                    #endregion

                    #region ValueChanged

                case ZWNotification.Type.ValueChanged:
                    {
                        try
                        {
                            Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());     
                            int objectId = API.GetObjectId(node.ID.ToString());
                            ZWValueID vid = m_notification.GetValueID();
                            API.Object.SetLastHeardFrom(objectId, DateTime.Now);                     

                            Value value = new Value();
                            value.ValueID = vid;
                            value.Label = m_manager.GetValueLabel(vid);
                            value.Genre = vid.GetGenre().ToString();
                            value.Index = vid.GetIndex().ToString();
                            value.Type = vid.GetType().ToString();
                            value.CommandClassID = vid.GetCommandClassId().ToString();
                            value.Help = m_manager.GetValueHelp(vid);

                            string data;
                            m_manager.GetValueAsString(vid, out data);

                            Console.WriteLine("OpenZWave Plugin | [ValueChanged] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data);
                            
                            //Update commands which are basically user editable values.
                            if (value.Genre != "System")
                            {
                                DataTable objCMDs = API.Commands.GetAllObjectCommandsForObject(objectId);
                                foreach (DataRow cmd in objCMDs.Rows)
                                {
                                    string value_ID = vid.GetId().ToString();
                                    //After Value Added, Value Name other value properties can change so update.
                                    if (cmd["txt_custom_data2"].ToString().Equals(value_ID))
                                    {
                                        int cmdID = Convert.ToInt32(cmd["id"]);
                                        int pType = Convert.ToInt32(cmd["param_type"]);

                                        int order;
                                        switch (value.Genre)
                                        {
                                            case "User":
                                                order = 1;
                                                break;
                                            case "Config":
                                                order = 2;
                                                break;
                                            default:
                                                order = 99;
                                                break;
                                        }
                                        API.Commands.UpdateObjectCommand(cmdID, "DYNAMIC_CMD_" + value.Label.ToUpper(), "Set " + value.Label, pType, value.Help, value.Label, value_ID, order);
                                    }
                                }
                            }

                            #region Repoll Dimmers after a level change event
                            //Some dimmers take x number of seconds to dim to desired level.  Therefor the level recieved here initially is a 
                            //level between old level and new level. (if going from 0 to 100 we get 84 here).
                            //To get the real level repoll the device a second or two after a level change was recieved.     
                            bool EnableDimmerRepoll = false; 
                            bool.TryParse(API.Object.Properties.GetObjectPropertyValue(objectId, "ENABLEREPOLLONLEVELCHANGE"), out EnableDimmerRepoll);

                            if (FinishedInitialPoll && EnableDimmerRepoll)
                            {
                                switch (node.Label)
                                {
                                    case "Multilevel Switch":
                                    case "Multilevel Power Switch":
                                        {

                                            switch (value.Label)
                                            {
                                                case "Level":
                                                case "Basic":
                                                    string prevVal = API.Object.Value.Get(objectId, value.Label);

                                                    //If it is truely new
                                                    if (!prevVal.Equals(data))
                                                    {
                                                        BackgroundWorker DelayedRepoll = new BackgroundWorker();
                                                        DelayedRepoll.DoWork += new DoWorkEventHandler(DelayedRepoll_DoWork);
                                                        DelayedRepoll.RunWorkerAsync(objectId);
                                                    }
                                                    break;
                                            }
                                            break;
                                        }                                       
                                }
                            }
                            #endregion

                            if (objectId > 0)
                            {
                                //Notify API Value Changed
                                if (node.ID == 24)
                                    Console.WriteLine("Thermo");

                                API.UpdateValue(objectId, vid.GetId().ToString(), value.Label, value.Genre, value.Index, value.Type, value.CommandClassID, data);
                            }
                            else
                                API.WriteToLog(Urgency.WARNING, "Getting changes on an unknown device!");
                            
                        }
                        catch (Exception ex)
                        {
                            API.WriteToLog(Urgency.ERROR, "error: " + ex.Message);
                        }
                        break;
                    }

                    #endregion

                    #region Group

                case ZWNotification.Type.Group:
                    {
                        Console.WriteLine("OpenZWave Plugin | [Group]"); ;
                        break;
                    }

                    #endregion

                    #region NodeAdded
                case ZWNotification.Type.NodeAdded:
                    {
                        // if this node was in zwcfg*.xml, this is the first node notification
                        // if not, the NodeNew notification should already have been received
                        //if (GetNode(m_notification.GetHomeId(), m_notification.GetNodeId()) == null)
                        //{
                            Node node = new Node();
                            node.ID = m_notification.GetNodeId();
                            node.HomeID = m_notification.GetHomeId();
                            m_nodeList.Add(node);

                            Console.WriteLine("OpenZWave Plugin | [NodeAdded] ID:" + node.ID.ToString() + " Added");
                        //}
                        break;
                    }

                  
                //case ZWNotification.Type.NodeNew:
                //    {
                //        // Add the new node to our list (and flag as uninitialized)
                //        Node node = new Node();
                //        node.ID = m_notification.GetNodeId();
                //        node.HomeID = m_notification.GetHomeId();
                //        m_nodeList.Add(node);

                //        Console.WriteLine("OpenZWave Plugin | [NodeNew] ID:" + node.ID.ToString() + " Added");                        
                //        break;
                //    }

                    #endregion

                    #region NodeRemoved

                case ZWNotification.Type.NodeRemoved:
                    {
                        foreach (Node node in m_nodeList)
                        {
                            if (node.ID == m_notification.GetNodeId())
                            {
                                Console.WriteLine("OpenZWave Plugin | [NodeRemoved] ID:" + node.ID.ToString());
                                m_nodeList.Remove(node);
                                break;
                            }
                        }
                        break;
                    }

                    #endregion

                    #region NodeProtocolInfo

                case ZWNotification.Type.NodeProtocolInfo:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                        }
                        string objectName = "UNKNOWN";
                        string objectType = "UNKNOWN";
                        if (node != null)
                        {
                            Console.WriteLine("OpenZWave Plugin | [Node Protocol Info] " + node.Label);  

                            switch (node.Label)
                            {
                                case "Binary Switch":
                                case "Binary Power Switch":
                                    objectName = "OpenZWave Switch " + node.ID;
                                    objectType = "SWITCH";
                                    break;
                                case "Multilevel Switch":
                                case "Multilevel Power Switch":
                                    objectName = "OpenZWave Dimmer " + node.ID;
                                    objectType = "DIMMER";
                                    break;
                                case "General Thermostat V2":
                                    objectName = "OpenZWave Thermostat " + node.ID;
                                    objectType = "THERMOSTAT";
                                    break;
                                case "Static PC Controller":
                                case "Static Controller":
                                case "Portable Remote Controller":
                                     objectName = "OpenZWave Controller " + node.ID;
                                     objectType = "CONTROLLER"; 
                                     break;
                                case "Secure Keypad Door Lock":
                                    objectName = "Door Lock " + node.ID;
                                     objectType = "DOORLOCK"; 
                                    break;
                            }
                            if (objectType != "UNKNOWN")
                                API.NewObject(node.ID, objectType, objectName);
                            else
                                API.WriteToLog(Urgency.WARNING, "Found unknown device '"+node.Label+"'!");
                        }
                    }

                    break;

                    #endregion

                    #region NodeNameing

                case ZWNotification.Type.NodeNaming:
                    {                        
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID);
                            node.Product = m_manager.GetNodeProductName(m_homeId, node.ID);
                            node.Location = m_manager.GetNodeLocation(m_homeId, node.ID);
                            node.Name = m_manager.GetNodeName(m_homeId, node.ID);

                            Console.WriteLine("OpenZWave Plugin | [NodeNaming] Node:" + node.ID + ", Product:" + node.Product + ", Manufacturer:" + node.Manufacturer + ")");
                        }
                        break;
                    }

                    #endregion

                    #region NodeEvent

                case ZWNotification.Type.NodeEvent:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            Console.WriteLine("OpenZWave Plugin | [NodeEvent] Node:" + node.ID + ", Name:" + node.Name + ", ID:" + node.ID);
                        }
                        break;
                        
                    }

                    #endregion

                    #region PollingDisabled

                case ZWNotification.Type.PollingDisabled:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            Console.WriteLine("OpenZWave Plugin | [PollingDisabled] Node:" + node.ID);
                        }

                        break;
                    }

                    #endregion

                    #region PollingEnabled

                case ZWNotification.Type.PollingEnabled:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            Console.WriteLine("OpenZWave Plugin | [PollingEnabled] Node:" + node.ID);
                        }
                        break;
                    }

                    #endregion

                    #region DriverReady

                case ZWNotification.Type.DriverReady:
                    {
                        m_homeId = m_notification.GetHomeId();
                        API.WriteToLog(Urgency.INFO, "Initializing: Driver with Home ID 0x" + m_homeId);
                        Console.WriteLine("OpenZWave Plugin | [DriverReady] Initializing...driver with Home ID 0x" + m_homeId);                        
                        break;
                    }

                    #endregion

                    #region NodeQueriesComplete

                case ZWNotification.Type.NodeQueriesComplete:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());                                               

                        if (node != null)
                        {
                            int objectId = API.GetObjectId(node.ID.ToString());
                            API.Object.SetLastHeardFrom(objectId, DateTime.Now);
                            
                            API.WriteToLog(Urgency.INFO, "Initializing: Node " + node.ID + " query complete.");
                            Console.WriteLine("OpenZWave Plugin | [NodeQueriesComplete] Initializing...node " + node.ID + " query complete.");
                        }

                        break;
                    }

                    #endregion

                    #region AllNodesQueried

                case ZWNotification.Type.AllNodesQueried:
                    {
                        foreach (Node n in m_nodeList)
                        {
                           int objId = API.GetObjectId(n.ID.ToString());

                           if (objId > 0)
                           {
                               if(API.Object.Properties.GetObjectPropertyValue(objId,"ENABLEPOLLING").ToUpper().Equals("TRUE"))
                                   EnablePolling(n.ID);
                           }                           
                        }

                        API.WriteToLog(Urgency.INFO, "Ready:  All nodes queried.");
                        Console.WriteLine("OpenZWave Plugin | All nodes queried.");
                        
                        FinishedInitialPoll = true;
                        //TODO: FREEZE GUI UNTIL THIS IS HIT.  YOU CANNOT SUBMIT COMMANDS UNTIL THIS IS COMPLETE. 
                        break;
                    }

                    #endregion

                    #region AwakeNodesQueried

                case ZWNotification.Type.AwakeNodesQueried:
                    {
                        API.WriteToLog(Urgency.INFO, "Ready:  Awake nodes queried (but not some sleeping nodes).");
                        Console.WriteLine("OpenZWave Plugin | Ready:  Awake nodes queried (but not some sleeping nodes).");
                        break;
                    }

                    #endregion
            }
        }

        void DelayedRepoll_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(3000);
            byte NodeId = (byte)API.GetObjectNodeId(e.Argument.ToString());

            m_manager.RefreshNodeInfo(m_homeId, NodeId);  
        }

        private Node GetNode(UInt32 homeId, Byte nodeId)
        {
            foreach (Node node in m_nodeList)
            {
                if ((node.ID == nodeId) && (node.HomeID == homeId))
                {
                    return node;
                }
            }

            return new Node();
        }

        private void EnablePolling(byte nid)
        {
            try
            {
                Node n = GetNode(m_homeId, nid);
                ZWValueID zv = null;
                switch (n.Label)
                {
                    case "Binary Switch":
                    case "Binary Power Switch":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Switch")
                                zv = v.ValueID;
                        }
                        break;
                    case "Multilevel Switch":
                    case "Multilevel Power Switch":
                        foreach (Value v in n.Values)
                        {
                            if (v.Genre == "User" && v.Label == "Level")
                                zv = v.ValueID;
                        }
                        break;
                    case "General Thermostat V2":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Temperature")
                                zv = v.ValueID;
                        }
                        break;
                }
                if (zv != null)
                 m_manager.EnablePoll(zv);
            }
            catch (Exception ex)
            {
                API.WriteToLog(Urgency.ERROR, "Error attempting to enable polling: " + ex.Message);
            }
        }

        #endregion
    }
}
