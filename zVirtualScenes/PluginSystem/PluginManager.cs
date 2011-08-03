using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Threading;
using zVirtualScenesAPI;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;
using System;
using zVirtualScenesAPI.Structs;
using zVirtualScenesAPI.Events;

namespace zVirtualScenesApplication.PluginSystem
{
    public class PluginManager
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Plugin> _plugins;
#pragma warning restore 649
        //private bool _runCommands;

        public PluginManager()
        {
            DirectoryCatalog catalog = new DirectoryCatalog("plugins");
            CompositionContainer compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);

            foreach (string zCommandType in Enum.GetNames(typeof(cmdType)))
                DatabaseControl.NewzCommandType(zCommandType);
            
            API.Commands.NewBuiltinCommand("REPOLL_ME", "Repoll Device", ParamType.INTEGER, false, "This will force a repoll on an object.", "", "");
            API.Commands.NewBuiltinCommand("REPOLL_ALL", "Repoll all Devices", ParamType.NONE, false, "This will force a repoll on all objects.", "", "");
            API.Commands.NewBuiltinCommand("GROUP_ON", "Turn Group On", ParamType.STRING, false, "Activates a group.", "", "");
            API.Commands.NewBuiltinCommand("GROUP_OFF", "Turn Group Off", ParamType.STRING, false, "Deactivates a group.", "", "");
            API.Commands.NewBuiltinCommand("TIMEDELAY", "Scene Time Delay (sec)", ParamType.INTEGER, false, "Pauses a scene execution for x seconds.", "", "");
            API.Object.Properties.NewObjectProperty("ENABLEPOLLING", "Enable polling for this device.", "false", ParamType.BOOL);

            // Iterate the plugins to make sure none of them are new...
            foreach (Plugin p in _plugins)
            {
                DatabaseControl.NewPlugin(p.ToString(), p.GetAPIName());

                p.Initialize();
                p.Start();
            }
                        
            zVirtualSceneEvents.CommandAddedEvent += new zVirtualSceneEvents.CommandAddedEventHandler(zVirtualSceneEvents_CommandAddedEvent);
        }

        void zVirtualSceneEvents_CommandAddedEvent(int QueCmdID)
        {    
            QuedCommand zCmd = API.Commands.GetQuedCommand(QueCmdID);
            if (zCmd != null)
            {
                Command CommandInfo = API.Commands.GetCommand(zCmd.CommandId, zCmd.cmdtype);
                if (CommandInfo != null)
                {
                    //Process built-in Commands seperatly
                    if (zCmd.cmdtype == cmdType.Builtin)
                    {
                        ProcessBuiltinCMD(zCmd.CommandId, zCmd.Argument);
                        API.Commands.RemoveQuedCommand(zCmd.Id, false, "");
                    }
                    else
                    {
                        #region process standard Commands
                        if (CommandInfo != null)
                        {
                            DataRow dr_obj = API.Object.GetObject(zCmd.ObjectId);
                            string zCMD_API_Name = dr_obj["txt_api_name"].ToString();

                            Console.WriteLine("[PROCESSING CMD] API:" + zCMD_API_Name +
                                                                ", CMD_NAME:" + CommandInfo.FriendlyName +
                                                                ", ARG:" + zCmd.Argument);

                            foreach (Plugin p in GetPlugins().Where(p => p.GetAPIName() == zCMD_API_Name))
                            {
                                if (p.Enabled)
                                {
                                    bool err = false;
                                    int iterations = 0;
                                    while (!p.IsReady)
                                    {
                                        if (iterations == 5)
                                        {
                                            err = true;
                                            Logger.WriteToLog(UrgencyLevel.ERROR, "Timed-out while trying to process " + CommandInfo.FriendlyName + " on '" + dr_obj["txt_object_name"].ToString() + "'. Plugin Not Ready.", p.GetAPIName());
                                            API.Commands.RemoveQuedCommand(zCmd.Id, err, "Timed-out while trying to process " + CommandInfo.FriendlyName + " on '" + dr_obj["txt_object_name"].ToString() + "'. Plugin Not Ready.");
                                            break;
                                        }
                                        iterations++;
                                        Thread.Sleep(1000);
                                    }

                                    if (!err)
                                    {
                                        p.ProcessCommand(zCmd);
                                        API.Commands.RemoveQuedCommand(zCmd.Id, err, string.Empty);
                                    }
                                }
                                else
                                {
                                    Logger.WriteToLog(UrgencyLevel.WARNING, "Attemped command " + CommandInfo.FriendlyName + " on '" + dr_obj["txt_object_name"].ToString() + "' on a disabled plugin. Removing command #" + QueCmdID + " from que...",
                                                        p.GetAPIName());
                                    API.Commands.RemoveQuedCommand(zCmd.Id, true, "Attemped command " + CommandInfo.FriendlyName + " on '" + dr_obj["txt_object_name"].ToString() + "' on a disabled plugin.");
                                }
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    Logger.WriteToLog(UrgencyLevel.ERROR, "Error processing command.", "PluginManager");
                    API.Commands.RemoveQuedCommand(zCmd.Id, true, "Error processing command.");
                }
            }
            else
            {
                Logger.WriteToLog(UrgencyLevel.ERROR, "Cannot locate command number '" + QueCmdID + "' in que.", "PluginManager");
            }
        }

        public IEnumerable<Plugin> GetPlugins()
        {
            return _plugins;
        }

        public Plugin GetPlugin(string pluginName)
        {
            return _plugins.FirstOrDefault(p => p.GetAPIName() == pluginName);
        }

        public void StopCommandThread()
        {
            //_runCommands = false;           
        }

        public void RunCommand()
        {
            //TODO:  Add this backin in some manor incase we have software talking to the DB directly that doesnt trigger the API event

            //_runCommands = true;

            //while (_runCommands)
            //{
            //List<QuedCommand> zCommands = API.Commands.GetQuedCommands();

            //foreach (QuedCommand zCmd in zCommands)
            //{
            //}
            //    Thread.Sleep(2000);
            //}
        }

        public void ProcessBuiltinCMD(int builtinCommandId, string arg)
        {
            DataTable dt = DatabaseControl.GetBuiltinCommand(builtinCommandId);

            if (dt.Rows.Count > 0)
            {
                switch (dt.Rows[0]["txt_command_name"].ToString())
                {
                    case "REPOLL_ME":
                        foreach (DataRow dr in DatabaseControl.GetObjects(false).Rows)
                        {
                            if (dr["id"].ToString().Equals(arg))
                            {
                                Plugin p = GetPlugin(dr["txt_api_name"].ToString());
                                if(p.Enabled)
                                    p.Repoll(dr["id"].ToString());                             
                            }
                        }
                        break;
                    case "REPOLL_ALL":
                        foreach (DataRow dr in DatabaseControl.GetObjects(false).Rows)
                        {
                            Plugin p =GetPlugin(dr["txt_api_name"].ToString()); 
                            if(p.Enabled)
                                    p.Repoll(dr["id"].ToString()); 
                        }
                        break;

                    case "GROUP_ON":
                        //EXECUTE ON ALL API's
                        foreach (Plugin p in GetPlugins())
                        {
                            if (p.Enabled)
                                p.ActivateGroup(arg);
                        }
                        break;
                    case "GROUP_OFF":
                        //EXECUTE ON ALL API's
                        foreach (Plugin p in GetPlugins())
                        {
                            if (p.Enabled)
                                p.DeactivateGroup(arg);
                        }
                        break;
                }
            }
        }
    }
}
         
