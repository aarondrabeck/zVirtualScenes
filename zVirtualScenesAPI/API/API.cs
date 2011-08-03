using System;
using System.Data;
using System.Reflection;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;
using zVirtualScenesCommon.ValueCommon;
using System.Collections.Generic;
using zVirtualScenesAPI.Structs;
using zVirtualScenesCommon;
using zVirtualScenesApplication.Structs;
using System.ComponentModel;
using zVirtualScenesAPI.Events;

namespace zVirtualScenesAPI
{
    public class API
    {
        public static string ProgramVersion 
        {
            get { return CurrentVersion.Program; }
        }

        public static string DatabaseVersion
        {
            get { return CurrentVersion.Database; }
        }

        public static void Initialize()
        {
        }

        private string _plugin;
        
        public API(string plugin)
        {
            _plugin = plugin;
        }

        public static string GetProgramNameAndVersion
        {
            get { return "zVirtualScenes - v" + ProgramVersion + " | db" + DatabaseVersion; }
        }        

        #region Plugin Specific Commands (instantiated)

        public int NewObjectTypeCommand(string objectTypeName, string CommandName, string CommandFriendlyName, ParamType paramType, string help, string txt_custom_data1 = "", string txt_custom_data2 = "")
        {
            return DatabaseControl.NewObjectTypeCommand(_plugin, objectTypeName, CommandName, CommandFriendlyName, (int)paramType, help, txt_custom_data1, txt_custom_data2);
        }        

        public string GetPluginName()
        {
            return _plugin;
        }

        public void DefineSetting(string settingName, string defaultSetting, ParamType type, string description = "")
        {
            DatabaseControl.DefinePluginSetting(_plugin, settingName, defaultSetting, (int)type, description);
        }

        public void NewPluginSettingOption(string settingName, string Option)
        {
            DatabaseControl.NewPluginSettingOption(_plugin, settingName,  Option); 
        }       

        public string GetSetting(string settingName)
        {
            return DatabaseControl.GetPluginSetting(_plugin, settingName);
        }       

        public void WriteToLog(Urgency urgancy, string message)
        {
            Logger.WriteToLog((UrgencyLevel)urgancy, message, _plugin);
        }

        public static void WriteToLog(Urgency urgancy, string message, string API)
        {
            Logger.WriteToLog((UrgencyLevel)urgancy, message, API);
        }

        #region Objects

            public int InstallObjectType(string objectTypeName, bool showInList)
            {
                return DatabaseControl.NewObjectType(_plugin, objectTypeName, showInList);
            }

            public void NewObject(int objectId, string objectType, string objectName)
            {
                DatabaseControl.NewObject(_plugin, objectId, objectType, objectName);
            }

            /// <summary>
            /// Gets a objectID from node ID
            /// </summary>
            /// <param name="objectId"></param>
            /// <returns>-1 if not found</returns>
            public int GetObjectId(string nodeID)
            {
                return DatabaseControl.GetObjectId(_plugin, nodeID);
            }

            public int GetObjectNodeId(string objectID)
            {
                return DatabaseControl.GetNodeId(_plugin, objectID);
            }

        #endregion

        #endregion        

        public class Object
        {
            public static int GetNodeId(string APIName, int objectID)
            {
                return DatabaseControl.GetNodeId(APIName, objectID.ToString());
            }

            public static void SetLastHeardFrom(int objectId, DateTime datetime)
            {
                DatabaseControl.SetLastHeardFrom(objectId, datetime);
            }
                        
            public static DataTable GetObjectByNodeID(string objectNodeID)
            {
                return DatabaseControl.GetObjectByNodeID(objectNodeID);
            }

            public static string GetObjectName(int objectId)
            {
                return DatabaseControl.GetObjectName(objectId);
            }

            public static string SetObjectName(int objectId, string objectName)
            {
                return DatabaseControl.SetObjectName(objectId, objectName);
            }

            public static DataTable GetObjects(bool forList)
            {
                return DatabaseControl.GetObjects(forList);
            }

            public static DataRow GetObject(int objectID)
            {
                return DatabaseControl.GetObject(objectID);
            }

            /// <summary>
            /// Get Object Type Name
            /// </summary>
            /// <param name="objectId"></param>
            /// <returns></returns>
            public static string GetObjectType(int objectId)
            {
                return DatabaseControl.GetObjectType(objectId);
            }

            public static int GetObjectTypeId(string objectName)
            {
                return DatabaseControl.GetObjectTypeId(objectName);
            }

            public static int GetObjectTypeId(int objectId)
            {
                return DatabaseControl.GetObjectTypeId(objectId);
            }

            public class Properties
            {
                //Object properties are per Object specific across and are not plugin API specific.
                public static string ClearObjectProperties()
                {
                    return DatabaseControl.ClearObjectProperties();
                } 

                /// <summary>
                /// Installs a new object property.  Object Properties are unique to each object.
                /// 
                /// All objects will return the default value set here unless overridden per object in the object properties dialog window. 
                /// </summary>
                /// <param name="UniquePropertyName">Must be unique across all plugins!</param>
                /// <param name="PropertyFriendlyName"></param>
                /// <param name="DefaultValue"></param>
                /// <param name="PropertyType"></param>
                public static void NewObjectProperty(string PropertyName, string PropertyFriendlyName, string DefaultValue, ParamType PropertyType)
                {
                    DatabaseControl.NewObjectProperty(PropertyName, PropertyFriendlyName, DefaultValue, (int)PropertyType);
                }

                /// <summary>
                /// Gets the property value for an object.  It not explicitly set, returns default value of the property. 
                /// </summary>
                /// <param name="objectId"></param>
                /// <param name="PropertyName"></param>
                /// <returns></returns>
                public static string GetObjectPropertyValue(int objectId, string PropertyName)
                {
                    return DatabaseControl.GetObjectPropertyValue(objectId, PropertyName);
                }

                /// <summary>
                /// Gets the dafault value of a property.  
                /// </summary>
                /// <param name="PropertyName"></param>
                /// <returns></returns>
                public static string GetObjectPropertyDefaultValue(string PropertyName)
                {
                    return DatabaseControl.GetObjectPropertyDefaultValue(PropertyName);
                }

                /// <summary>
                /// Gets the object property ID give the property name.  Return 0 if not found.
                /// </summary>
                /// <param name="PropertyName"></param>
                /// <returns></returns>
                public static int GetObjectPropertyId(string PropertyName)
                {
                    return DatabaseControl.GetObjectPropertySettingsId(PropertyName);
                }

                /// <summary>
                /// Sets the object property ID. Returns false if error. 
                /// </summary>
                /// <param name="PropertyName"></param>
                /// <param name="objectId"></param>
                /// <param name="value"></param>
                public static void SetObjectPropertyValue(string PropertyName, int objectId, string value)
                {
                    DatabaseControl.SetObjectPropertyValue(PropertyName, objectId, value);
                }

                /// <summary>
                /// Gets all object properties as a datatable.  
                /// 
                /// Columns:
                /// id
                /// txt_property_friendly_name
                /// txt_property_name
                /// txt_default_value
                /// property_type
                /// 
                /// </summary>
                /// <returns></returns>
                public static DataTable GetObjectProperties()
                {
                    return DatabaseControl.GetObjectProperties();
                }

                /// <summary>
                /// Install a new Object Property Setting Option for use with a object property setting of type LIST.
                /// </summary>
                /// <param name="PropertyName"></param>
                /// <param name="OptionValue"></param>
                public static void NewObjectPropertyOption(string PropertyName, string OptionValue)
                {
                    DatabaseControl.NewObjectPropertyOption(PropertyName, OptionValue);
                }

                /// <summary>
                /// Get all possible options for a object property setting of type LIST
                /// </summary>
                /// <param name="PropertyName"></param>
                /// <returns></returns>
                public static List<string> GetObjectPropertyOptions(string PropertyName)
                {
                    return DatabaseControl.GetObjectPropertyOptions(PropertyName);
                }
            }

            public class Value
            {
                /// <summary>
                /// Installs a new Object Value in the database.
                /// </summary>
                /// <param name="ObjectId"></param>
                /// <param name="ValueID"></param>
                /// <param name="LabelName"></param>
                /// <param name="Genre"></param>
                /// <param name="Index"></param>
                /// <param name="Type"></param>
                /// <param name="CommandClassID"></param>
                /// <param name="Value"></param>
                public static void New(int ObjectId, string ValueID, string LabelName, string Genre, string Index, string Type, string CommandClassID, string Value)
                {
                    DatabaseControl.NewObjectValue(ObjectId, ValueID, LabelName, Genre, Index, Type, CommandClassID, Value);
                }

                /// <summary>
                /// Update all value properties
                /// </summary>
                /// <param name="ObjectId"></param>
                /// <param name="ValueID"></param>
                /// <param name="LabelName"></param>
                /// <param name="Genre"></param>
                /// <param name="Index"></param>
                /// <param name="Type"></param>
                /// <param name="CommandClassID"></param>
                /// <param name="Value"></param>
                public static void Update(int ObjectId, string ValueID, string LabelName, string Genre, string Index, string Type, string CommandClassID, string Value)
                {
                    DatabaseControl.UpdateObjectValue(ObjectId, ValueID, LabelName, Genre, Index, Type, CommandClassID, Value);
                }

                /// <summary>
                /// Update just a Values value
                /// </summary>
                /// <param name="ObjectId"></param>
                /// <param name="ValueID"></param>
                /// <param name="Value"></param>
                public static void Update(int ObjectId, string ValueID, string Value)
                {
                    DatabaseControl.UpdateObjectValue(ObjectId, ValueID, Value);
                }

                public static string Get(int ObjectId, string Label)
                {
                    return DatabaseControl.GetObjectValue(ObjectId, Label);
                }
            }
        }

        public class Commands
        {
            //static
            public static string ClearAllCommands()
            {
                return DatabaseControl.ClearAllCommands();
            }
                     

            /// <summary>
            /// Use this if you want the QueCommand id returned before the command is processed.
            /// IMPORTANT: You must initiate the processing of the command yourself with 'zVirtualSceneEvents.ProcessCommandQue(id)'
            /// </summary>
            /// <param name="objectId"></param>
            /// <param name="zCommandType"></param>
            /// <param name="CommandId"></param>
            /// <param name="arg"></param>
            /// <returns></returns>
            public static int InstallQueCommand(QuedCommand QueCMD)
            {
                int id;
                id = DatabaseControl.QueCommand(QueCMD.ObjectId, (int)QueCMD.cmdtype, QueCMD.CommandId, QueCMD.Argument);
                return id;
            }

            public static void InstallQueCommandAndProcess(QuedCommand QueCMD)
            {
                int id;
                id = DatabaseControl.QueCommand(QueCMD.ObjectId, (int)QueCMD.cmdtype, QueCMD.CommandId, QueCMD.Argument);
                zVirtualSceneEvents.QueCommandAdded(id);                
            }

            /// <summary>
            /// Returns null if not found
            /// </summary>
            /// <param name="QueID"></param>
            /// <returns></returns>
            public static QuedCommand GetQuedCommand(int QueID)
            {
                DataTable dt = DatabaseControl.GetCommand(QueID);

                if (dt.Rows.Count > 0)
                    return CreateQueCommand(dt.Rows[0]);
                else 
                    return null;
            }

            public static List<QuedCommand> GetQuedCommands()
            {
                List<QuedCommand> zcmds = new List<QuedCommand>(); 
                foreach (DataRow dr_command in DatabaseControl.GetCommands().Rows)
                {
                    zcmds.Add(CreateQueCommand(dr_command));
                }
                return zcmds;
            }

            private static QuedCommand CreateQueCommand(DataRow dr)
            {
                QuedCommand zcmd = new QuedCommand();
                int id = 0;
                int.TryParse(dr["id"].ToString(), out id);
                zcmd.Id = id;

                int object_id = 0;
                int.TryParse(dr["object_id"].ToString(), out object_id);
                zcmd.ObjectId = object_id;

                int command_id = 0;
                int.TryParse(dr["command_id"].ToString(), out command_id);
                zcmd.CommandId = command_id;

                int command_type_id = 0;
                int.TryParse(dr["command_type_id"].ToString(), out command_type_id);
                zcmd.cmdtype = (cmdType)command_type_id;

                zcmd.Argument = dr["txt_arg"].ToString();
                return zcmd;
            }

            /// <summary>
            /// ID here is the ID in the command table
            /// </summary>
            /// <param name="queID"></param>
            /// <param name="withErrors"></param>
            public static void RemoveQuedCommand(int queID, bool withErrors, string txtError)
            {
                DatabaseControl.RemoveQuedCommand(queID.ToString());
                zVirtualSceneEvents.CommandRunComplete(queID, withErrors, txtError);
            }

            public static int GetBuiltinCommandId(string typeName)
            {
                return DatabaseControl.GetBuiltinCommandId(typeName);
            }

            /// <summary>
            /// Return 0 if not found
            /// </summary>
            /// <param name="objectId"></param>
            /// <param name="command"></param>
            /// <returns></returns>
            public static int GetObjectCommandId(int objectId, string command)
            {
                return DatabaseControl.GetObjectCommandId(objectId, command);
            }

            public static int GetObjectTypeCommandId(int objectTypeId, string command)
            {
                return DatabaseControl.GetObjectTypeCommandId(objectTypeId, command);
            }

            public static void NewObjectCommand(int objectId, string CommandName, string CommandFriendlyName, ParamType paramType, string help, string txt_custom_data1, string txt_custom_data2, int sort_order)
            {
                DatabaseControl.NewObjectCommand(objectId, CommandName, CommandFriendlyName, (int)paramType, help, txt_custom_data1, txt_custom_data2, sort_order);
            }

            public static void UpdateObjectCommand(int cmdId, string CommandName, string CommandFriendlyName, int paramType, string help, string txt_custom_data1, string txt_custom_data2, int sort_order)
            {
                DatabaseControl.UpdateObjectCommand(cmdId, CommandName, CommandFriendlyName, paramType, help, txt_custom_data1, txt_custom_data2, sort_order);
            }

            public static int NewBuiltinCommand(string CommandName, string CommandFriendlyName, ParamType paramType, bool ShowOnDynamicObjectList, string help, string txt_custom_data1, string txt_custom_data2)
            {
                return DatabaseControl.NewBuiltinCommand(CommandName, CommandFriendlyName, (int)paramType, ShowOnDynamicObjectList, help, txt_custom_data1, txt_custom_data2);
            }

            public static DataTable GetAllObjectCommandsForObject(int objectId)
            {
                return DatabaseControl.GetAllObjectCommandForObject(objectId);
            }

            public static List<Command> GetAllObjectCommandsForObjectasCMD(int objectId)
            {
                List<Command> commands = new List<Command>();

                foreach (DataRow dr in DatabaseControl.GetAllObjectCommandForObject(objectId).Rows)
                {
                    commands.Add(GetCommandObject(dr, cmdType.Object));
                }                

                return commands;
            }

            public static DataTable GetAllObjectTypeCommandForObject(int objectId)
            {
                return DatabaseControl.GetAllObjectTypeCommandForObject(objectId);
            }

            public static List<Command> GetAllObjectTypeCommandsForObjectasCMD(int objectId)
            {
                List<Command> commands = new List<Command>();

                foreach (DataRow dr in DatabaseControl.GetAllObjectTypeCommandForObject(objectId).Rows)
                {
                    commands.Add(GetCommandObject(dr, cmdType.ObjectType));
                }

                return commands;
            }

            private static Command GetCommandObject(DataRow dr, cmdType CmdType)
            {
                Command cmd = new Command();
                int id = 0;
                int.TryParse(dr["id"].ToString(), out id);
                cmd.CommandId = id;
                cmd.Name = dr["txt_command_name"].ToString();
                cmd.FriendlyName = dr["txt_command_friendly_name"].ToString();
                int type = 0;
                int.TryParse(dr["param_type"].ToString(), out type);
                cmd.paramType = (ParamType)type;
                cmd.CustomData1 = dr["txt_custom_data1"].ToString();
                cmd.CustomData2 = dr["txt_custom_data2"].ToString();
                cmd.HelpText = dr["txt_cmd_help"].ToString();
                cmd.cmdtype = CmdType;
                return cmd;
            }

            public static DataTable GetBuiltinCommand(int CommandId)
            {
                return DatabaseControl.GetBuiltinCommand(CommandId);
            }

            public static DataTable GetObjectCommand(int CommandId)
            {
                return DatabaseControl.GetObjectCommand(CommandId);
            }

            public static DataTable GetObjectTypeCommand(int CommandId)
            {
                return DatabaseControl.GetObjectTypeCommand(CommandId);
            }

            /// <summary>
            /// Returns null if not found
            /// </summary>
            /// <param name="CommandId"></param>
            /// <param name="cmdType"></param>
            /// <returns></returns>
            public static Command GetCommand(int CommandId, cmdType ctype)
            {  
                DataTable dt = new DataTable();
                switch (ctype)
                {
                    case cmdType.Builtin:
                        dt = API.Commands.GetBuiltinCommand(CommandId);
                        break;
                    case cmdType.Object:
                        dt = API.Commands.GetObjectCommand(CommandId);
                        break;
                    case cmdType.ObjectType:
                        dt = API.Commands.GetObjectTypeCommand(CommandId);
                        break;
                }

                if (dt.Rows.Count > 0)
                {
                    Command cmd = GetCommandObject(dt.Rows[0], ctype);
                    return cmd;
                }
                return null;
            }

            public static DataTable GetBuiltinCommands()
            {
                return DatabaseControl.GetBuiltinCommands();
            }

            public static List<Command> GetBuiltinCommandsasCMD()
            {
                List<Command> commands = new List<Command>();

                foreach (DataRow dr in DatabaseControl.GetBuiltinCommands().Rows)
                {
                    commands.Add(GetCommandObject(dr, cmdType.Builtin));
                }

                return commands;
            }

            public static void NewObjectCommandOption(int CommandId, string CommandOption)
            {
                DatabaseControl.NewObjectCommandOption(CommandId, CommandOption);
            }

            public static List<string> GetObjectCommandOptions(int CommandId)
            {
                return DatabaseControl.GetObjectCommandOptions(CommandId);
            }

            public static void NewObjectTypeCommandOption(int ObjectTypeCommandId, string CommandOption)
            {
                DatabaseControl.NewObjectTypeCommandOption(ObjectTypeCommandId, CommandOption);
            }

            public static List<string> GetObjectTypeCommandOptions(int ObjectTypeCommandId)
            {
                return DatabaseControl.GetObjectTypeCommandOptions(ObjectTypeCommandId);
            }

            #region Builtin Command Options
            public static void NewBuiltinCommandOption(int BuiltinCommandId, string CommandOption)
            {
                DatabaseControl.NewBuiltinCommandOption(BuiltinCommandId, CommandOption);
            }

            public static List<string> GetBuiltinCommandOptions(int BuiltinCommandId)
            {
                return DatabaseControl.GetBuiltinCommandOptions(BuiltinCommandId);
            }
            #endregion
        }

        public class Groups
        {
            /// <summary>
            /// Gets all groups
            /// </summary>
            /// <returns></returns>
            public static DataTable GetGroups()
            {
                return DatabaseControl.GetGroups();
            }

            /// <summary>
            /// Gets all objects in a group given a groupID
            /// </summary>
            /// <param name="groupId"></param>
            /// <returns></returns>
            public static DataTable GetGroupObjects(int groupId)
            {
                return DatabaseControl.GetGroupObjects(groupId);
            }

            /// <summary>
            /// Gets all objects in a group given a Group Name
            /// </summary>
            /// <param name="groupName"></param>
            /// <returns></returns>
            public static DataTable GetGroupObjects(string groupName)
            {
                return DatabaseControl.GetGroupObjects(groupName);
            }

            /// <summary>
            /// Gets the name of a group given an groupID
            /// </summary>
            /// <param name="GroupID"></param>
            /// <returns></returns>
            public static string GetGroupName(int GroupID)
            {
                return DatabaseControl.GetGroupName(GroupID);
            }

            /// <summary>
            /// Gets all groups an object is in
            /// </summary>
            /// <param name="ObjectID"></param>
            /// <returns></returns>
            public static DataTable GetObjectGroups(int ObjectID)
            {
                return DatabaseControl.GetObjectGroups(ObjectID);
            }
        }

        public class Database
        {
            public static string ClearDatabase()
            {
                return DatabaseControl.ClearDatabase();
            }

            public static void NonQuery(string sql)
            {
                DatabaseControl.nQuery(sql);
            }
        }

        public class PluginSettings
        {
            public static List<string> GetPluginSettingOptions(string PluginAPIName, string settingName)
            {
                return DatabaseControl.GetPluginSettingOptions(PluginAPIName, settingName);
            }

            public static ParamType GetPluginSettingType(string APIName, string settingName)
            {
                return (ParamType)DatabaseControl.GetPluginSettingType(APIName, settingName);
            }

            public static string SetPluginSetting(string APIName, string settingName, string settingValue)
            {
                return DatabaseControl.SetPluginSetting(APIName, settingName, settingValue);
            }

            public static string ClearAllPluginData()
            {
                return DatabaseControl.ClearAllPluginData();
            }
        }

        
        public static class Scenes
        {
            public static DataTable GetScenes()
            {
                return DatabaseControl.Scenes.GetScenes();
            }
           
            public static Scene GetScene(int sceneID)
            {
                Scene scene = new Scene();
                DataTable dt = DatabaseControl.Scenes.GetScene(sceneID);
                
                if (dt.Rows.Count == 1)
                {
                    int id = 0;
                    int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                    scene.id = id;
                    scene.txt_name = dt.Rows[0]["txt_name"].ToString();

                    //Get Commands
                    scene.scene_commands.Clear();
                    DataTable dt_cmds = DatabaseControl.Scenes.GetSceneCMDs(sceneID);

                    foreach (DataRow dr in dt_cmds.Rows)
                    {
                        
                        SceneCommands scmds = new SceneCommands();

                        int cmd_id = 0;
                        int.TryParse(dr["id"].ToString(), out cmd_id);
                        scmds.Id = cmd_id;

                        int object_id = 0;
                        int.TryParse(dr["object_id"].ToString(), out object_id);
                        scmds.ObjectId = object_id;

                        int command_type_id = 0;
                        int.TryParse(dr["command_type_id"].ToString(), out command_type_id);
                        scmds.cmdtype = (cmdType)command_type_id;

                        int command_id = 0;
                        int.TryParse(dr["command_id"].ToString(), out command_id);
                        scmds.CommandId = command_id;

                        int order = 0;
                        int.TryParse(dr["sort_order"].ToString(), out order);
                        scmds.order = order;

                        scmds.Argument = dr["txt_arg"].ToString();

                        scene.scene_commands.Add(scmds);
                    }
                    return scene;
                }
                return null;
                
            }

            public static BindingList<Scene> GetSceneList()
            {
                BindingList<Scene> scenes = new BindingList<Scene>();
                DataTable dt = DatabaseControl.Scenes.GetScenes();

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int id = 0;
                        int.TryParse(dr["id"].ToString(), out id);

                        Scene s = new Scene();
                        
                        s = GetScene(id);
                        if (s != null)
                        {
                            scenes.Add(s);
                        }
                    }
                }
                return scenes;
            }

            public static void SaveOrder(BindingList<Scene> scenes)
            {
                foreach(Scene scene in scenes)
                    DatabaseControl.Scenes.SetOrder(scene.id, scenes.IndexOf(scene));
            }

            public static void DeleteScene(int sceneID)
            {
                DatabaseControl.Scenes.DeleteScene(sceneID);
                zVirtualSceneEvents.SceneChanged(sceneID);
            }

            public static void AddBlankScene()
            {
                zVirtualSceneEvents.SceneChanged(DatabaseControl.Scenes.AddScene("New Scene"));
            }

            public static int AddScene(string name)
            {                
                int id =  DatabaseControl.Scenes.AddScene(name);
                zVirtualSceneEvents.SceneChanged(id);
                return id;
            }

            public static void UpdateName(int SceneID, string Name)
            {
                DatabaseControl.Scenes.UpdateName(SceneID, Name);
                zVirtualSceneEvents.SceneChanged(SceneID);
            }

            public static void AddSceneCommand(int SceneID, SceneCommands cmd)
            {
                DatabaseControl.Scenes.AddSceneCommand(SceneID, cmd.ObjectId, (int)cmd.cmdtype, cmd.CommandId, cmd.Argument, cmd.order);
                zVirtualSceneEvents.SceneCMDChanged(SceneID);
            }

            public static void RemoveSceneCommand(int SceneID, SceneCommands cmd)
            {
                DatabaseControl.Scenes.RemoveSceneCommand(SceneID,cmd.Id);
                zVirtualSceneEvents.SceneCMDChanged(SceneID);
            }

            public static void UpdateSceneCommand(int SceneID, SceneCommands cmd)
            {
                DatabaseControl.Scenes.UpdateSceneCommand(SceneID, cmd.Id, cmd.ObjectId, (int)cmd.cmdtype, cmd.CommandId, cmd.Argument);
                zVirtualSceneEvents.SceneCMDChanged(SceneID);
            }

            public static void SaveCMDOrder(BindingList<SceneCommands> scene_commands)
            {
                foreach (SceneCommands sceneCMD in scene_commands)
                    DatabaseControl.Scenes.SetCMDOrder(sceneCMD.Id, scene_commands.IndexOf(sceneCMD));
            }

            public static void SetIsRunning(int SceneID, bool isRunning)
            {
                DatabaseControl.Scenes.SetIsRunning(SceneID, isRunning);
            }

            public static bool GetIsRunning(int SceneID)
            {
                bool isrunning = false; 
                DataTable dt = DatabaseControl.Scenes.GetScene(SceneID);

                if (dt.Rows.Count > 0)
                {
                    bool.TryParse(dt.Rows[0]["is_running"].ToString(), out isrunning);
                }
                return isrunning;
            }
        }

        //event triggering

     

        /// <summary>
        /// Call this to update database values have been updated. It will automatically call the proper valuechange events.
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <param name="ValueID"></param>
        /// <param name="label"></param>
        /// <param name="Genre"></param>
        /// <param name="Index"></param>
        /// <param name="type"></param>
        /// <param name="CommandClassID"></param>
        /// <param name="Value"></param>
        public static void UpdateValue(int ObjectId, string ValueID, string label, string Genre, string Index, string type, string CommandClassID, string Value)
        {
            ValueControl.UpdateValue(ObjectId,  ValueID,  label,  Genre,  Index,  type,  CommandClassID,  Value);
        }
    }

    public enum Urgency
    {
        INFO = 1,
        ERROR = 2,
        WARNING = 3
    }

    public enum PluginSettingType
    {
        STRING = 1,
        INT = 2,
        BOOL = 3,
        PASSWORD = 4
    }
    

}
