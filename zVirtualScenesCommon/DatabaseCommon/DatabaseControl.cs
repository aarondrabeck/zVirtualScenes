using System;
using System.Data;
using MySql.Data.MySqlClient;
using zVirtualScenesCommon.Util;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace zVirtualScenesCommon.DatabaseCommon
{
    public static class DatabaseControl
    {
        public static void nQuery(string sql)
        {
            NonQuery(sql);
        }

        public static string ClearDatabase()
        {
            return NonQuery("TRUNCATE TABLE builtin_command_options;" +
                            "TRUNCATE TABLE builtin_commands;" +
                            "TRUNCATE TABLE command_types;" +
                            "TRUNCATE TABLE commands;" +
                            "TRUNCATE TABLE event_scripts;" +
                            "TRUNCATE TABLE group_objects;" +
                            "TRUNCATE TABLE groups;" +
                            "TRUNCATE TABLE object_commands;" +
                            "TRUNCATE TABLE object_commands_options;" +
                            "TRUNCATE TABLE object_properties;" +
                            "TRUNCATE TABLE object_property_options;" +
                            "TRUNCATE TABLE object_property_settings;" +
                            "TRUNCATE TABLE object_type_commands;" +
                            "TRUNCATE TABLE object_type_commands_options;" +
                            "TRUNCATE TABLE object_type_events;" +
                            "TRUNCATE TABLE object_types;" +
                            "TRUNCATE TABLE object_values;" +
                            "TRUNCATE TABLE objects;" +
                            "TRUNCATE TABLE plugin_settings;" +
                            "TRUNCATE TABLE plugin_settings_options;" +
                            "TRUNCATE TABLE plugins;" +
                            "TRUNCATE TABLE scenes_cmds;" +
                            "TRUNCATE TABLE scenes;" +
                            "TRUNCATE TABLE scheduled_tasks;" +
                            "TRUNCATE TABLE scene_properties;" +
                            "TRUNCATE TABLE scene_property_options;" 
                            );
        }  

        public delegate void ObjectModifiedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Fired when a object is modified such as Group Change, Name Change, ext..
        /// </summary>
        public static event ObjectModifiedEventHandler ObjectModified;

        private static string _connectionString 
        {
            get
            { 
                return "server=" + Properties.Settings.Default.db_server +
                ";user=" + Properties.Settings.Default.db_user +
                ";database=" + Properties.Settings.Default.db_database +
                ";port=" + Properties.Settings.Default.db_port +
                ";password=" + Properties.Settings.Default.db_password + ";";
            }
        }

        private static string _firsttimeconnectionString
        {
            get
            {
                return "server=" + Properties.Settings.Default.db_server +
                ";user=" + Properties.Settings.Default.db_user +
                ";port=" + Properties.Settings.Default.db_port +
                ";password=" + Properties.Settings.Default.db_password + ";";
            }
        }

        public static string db_server
        {
            get { return Properties.Settings.Default.db_server; }
            set
            {
                Properties.Settings.Default.db_server = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string db_user
        {
            get { return Properties.Settings.Default.db_user; }
            set
            {
                Properties.Settings.Default.db_user = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string db_database
        {
            get { return Properties.Settings.Default.db_database; }
            set
            {
                Properties.Settings.Default.db_database = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string db_port
        {
            get { return Properties.Settings.Default.db_port; }
            set
            {
                Properties.Settings.Default.db_port = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string db_password
        {
            get { return Properties.Settings.Default.db_password; }
            set
            {
                Properties.Settings.Default.db_password = value; 
                Properties.Settings.Default.Save();
            }
        }

        #region Plugins
        public static string ClearAllPluginData()
        {
            return NonQuery("TRUNCATE TABLE plugin_settings;TRUNCATE TABLE plugins;");
        }  

        public static void NewPlugin(string pluginName, string APIName)
        {
            if (!HasRows("SELECT * FROM plugins WHERE txt_api_name = '" + GetSafeText(APIName) + "';"))
                NonQuery("INSERT INTO plugins (txt_plugin_name, txt_api_name) VALUES ('" + GetSafeText(pluginName) + "', '" +
                       GetSafeText(APIName) + "');");
        }

        public static string GetPluginSetting(string APIName, string settingName)
        {
            int pluginId = GetPluginId(APIName);
            if (pluginId != 0)
            {
                DataTable dt =
                    GetDataTable("SELECT txt_setting_value FROM plugin_settings WHERE plugin_id = " + pluginId +
                                 " AND txt_setting_name = '" + settingName + "';");
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["txt_setting_value"].ToString();
                }
            }
            return String.Empty;
        }

        public static string SetPluginSetting(string APIName, string settingName, string settingValue)
        {
            int pluginId = GetPluginId(APIName);
            if (pluginId != 0)
            {  
                if (!HasRows("SELECT txt_setting_value FROM plugin_settings WHERE plugin_id = " + pluginId + " AND txt_setting_name = '" + settingName + "';"))
                {
                    return NonQuery("INSERT INTO plugin_settings (plugin_id, txt_setting_name, txt_setting_value) VALUES ("+ pluginId +", '"+settingName+"', '"+settingValue+"');");
                }
                else
                {
                    return NonQuery("UPDATE plugin_settings SET txt_setting_value='" + settingValue + "' WHERE plugin_id='" + pluginId + "' AND txt_setting_name='" + settingName + "';");
                }
            }
            return String.Empty;
        }

        public static bool GetPluginEnabled(string APIName)
        {
             int pluginId = GetPluginId(APIName);
             if (pluginId != 0)
             {
                 DataTable dt = GetDataTable("SELECT enabled FROM plugins WHERE id = '" + pluginId + "';");

                 if (dt.Rows.Count > 0)
                 {
                     if (dt.Rows[0]["enabled"].ToString().Equals("1"))
                         return true;                    
                 }
             }
             return false;
        }

        public static void SetPluginEnabled(string APIName, bool enabled)
        {
            int pluginId = GetPluginId(APIName);
            if (pluginId != 0)
            {
                NonQuery("UPDATE plugins SET enabled='" + (enabled ? "1" : "0") + "' WHERE id = '" + pluginId + "';");                
            }
        }

        public static void DefinePluginSetting(string APIName, string settingName, string defaultValue, int type, string description = "")
        {
            int pluginId = GetPluginId(APIName);
            if (pluginId != -1)
            {
                if (!HasRows("SELECT id FROM plugin_settings WHERE plugin_id = " + pluginId + " AND txt_setting_name = '" +  GetSafeText(settingName) + "';"))
                {
                    NonQuery("INSERT INTO plugin_settings (plugin_id, txt_setting_name, txt_setting_value, plugin_settings_type_id, txt_setting_description) VALUES (" + pluginId + ", '" + GetSafeText(settingName)  + "', '" + defaultValue + "', '" + type + "', '" + GetSafeText(description) + "');");
                }
            }
        }

        public static void RemovePluginSetting(string APIName, string PropertyName)
        {
             int pluginId = GetPluginId(APIName);
            if (pluginId != -1)
            {
                int id = 0;
                string existssql = string.Format("SELECT id FROM plugin_settings WHERE txt_setting_name = '{0}' and plugin_id = '{1}';", GetSafeText(PropertyName), pluginId);
                DataTable dt = GetDataTable(existssql);

                if (dt.Rows.Count > 0)
                    int.TryParse(dt.Rows[0]["id"].ToString(), out id);

                if (id > 0)
                {
                    string sql = string.Format("DELETE FROM `plugin_settings` WHERE `id`='{0}';",
                        id);
                    NonQuery(sql);
                }
            }
        }

        public static DataTable GetAllPluginSettings(string APIName)
        {
            int pluginId = GetPluginId(APIName);

            if (pluginId != 0)
            {
                DataTable dt =
                    GetDataTable("SELECT * FROM plugin_settings" +
                    " WHERE plugin_settings.plugin_id = " + pluginId + ";");
                
                if (dt.Rows.Count > 0)
                    return dt;
            }
            return null;
        }

        #region Plugin Setting Options

        public static int GetPluginSettingId(string plugin_API, string settingName)
        {
            int APIid = GetPluginId(plugin_API);

            int id = 0;
            string sql = string.Format("SELECT id FROM `plugin_settings` WHERE txt_setting_name = '{0}' AND plugin_id = '{1}';",
                                        settingName,
                                        APIid);

            DataTable dt = GetDataTable(sql);
            if (dt.Rows.Count > 0)
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);

            return id;

        }

        public static void NewPluginSettingOption(string plugin_API, string settingName, string Option)
        {
            int pluginSettingId = GetPluginSettingId(plugin_API, settingName);

            string sqlisalreadyinstalled = string.Format("SELECT id FROM `plugin_settings_options` WHERE plugin_settings_id = '{0}' AND txt_option = '{1}';",
                                                        pluginSettingId,
                                                        Option);

            if (GetDataTable(sqlisalreadyinstalled).Rows.Count == 0)
            {
                string inserrtsql = string.Format("INSERT INTO `plugin_settings_options` (`plugin_settings_id`, `txt_option`) VALUES ({0}, '{1}');",
                                        pluginSettingId,
                                        Option);

                NonQuery(inserrtsql);
            }
        }

        public static List<string> GetPluginSettingOptions(string plugin_API, string settingName)
        {
            int pluginSettingId = GetPluginSettingId(plugin_API, settingName);
            List<string> options = new List<string>();
            string sql = string.Format("SELECT txt_option FROM `plugin_settings_options` WHERE plugin_settings_id = '{0}';",
                                                        pluginSettingId);

            foreach (DataRow dr in GetDataTable(sql).Rows)
            {
                options.Add(dr["txt_option"].ToString());
            }
            return options;
        }
        #endregion

        /// <summary>
        /// Gets a Plugin Setting Type for a the specified plugin and setting
        /// </summary>
        /// <param name="APIName"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static int GetPluginSettingType(string APIName, string settingName)
        {
            int plugintype = 0;
            int pluginId = GetPluginId(APIName);

            if (pluginId != 0)
            {
                string sql = string.Format("SELECT * FROM `plugin_settings` WHERE plugin_id = '{0}' AND txt_setting_name = '{1}';",
                                            pluginId,
                                            settingName);

                DataTable dt = GetDataTable(sql);

                if (dt.Rows.Count > 0)
                    int.TryParse(dt.Rows[0]["plugin_settings_type_id"].ToString(), out plugintype);

            }
            return plugintype;
        }

        private static int GetPluginId(string APIName)
        {
            DataTable dt = GetDataTable("SELECT id FROM plugins WHERE txt_api_name = '" + APIName + "'");
            if (dt.Rows.Count == 1)
            {
                int pluginId;
                int.TryParse(dt.Rows[0]["id"].ToString(), out pluginId);
                return pluginId;
            }
            if (dt.Rows.Count == 0)
                Logger.WriteToLog(UrgencyLevel.ERROR, "Tried to access a plugin that doesnt exist!", "GETPLUGINID");
            else
                Logger.WriteToLog(UrgencyLevel.ERROR, "There seems to be too many plugins with the same name!", "GETPLUGINID");

            return -1;
        }
        #endregion

        #region Object Values

        public static void NewObjectValue(int ObjectId, string ValueID, string LabelName, string Genre, string Index, string Type, string CommandClassID, string Value )
        {

            if (!HasRows("SELECT * FROM object_values WHERE txt_value_id = '" + GetSafeText( ValueID )+ "' AND ObjectId = '" + ObjectId + "';"))
                {
                    NonQuery(
                        "INSERT INTO object_values (ObjectId, txt_value_id, txt_label_name, txt_genre, txt_index, txt_type, txt_commandclassid, txt_value) VALUES " +
                        "( '" +
                        ObjectId + "', '" + 
                        GetSafeText(ValueID) +  "', '" +
                         GetSafeText(LabelName) +  "', '" +
                          GetSafeText(Genre) +  "', '" +
                           GetSafeText(Index) +  "', '" +
                            GetSafeText(Type) +  "', '" +
                             GetSafeText(CommandClassID) +  "', '" +
                             GetSafeText(Value) +  "');");
                }
                return;           
        }

        public static void UpdateObjectValue(int ObjectId, string ValueID, string LabelName, string Genre, string Index, string Type, string CommandClassID, string Value)
        {

            if (HasRows("SELECT * FROM object_values WHERE txt_value_id = '" + GetSafeText(ValueID) + "' AND ObjectId = '" + ObjectId + "';"))
            {
                NonQuery(
                    "UPDATE object_values SET " +
                                         "txt_label_name='" + GetSafeText(LabelName) + "', " +
                                         "txt_genre='" + GetSafeText(Genre) + "', " +
                                         "txt_index='" + GetSafeText(Index) + "', " +
                                         "txt_type='" + GetSafeText(Type) + "', " +                                         
                                         "txt_commandclassid='" + GetSafeText(CommandClassID) + "', " +                                         
                                         "txt_value='" + GetSafeText(Value) + "' " +
                                         "WHERE txt_value_id = '" + GetSafeText(ValueID) + "' AND ObjectId = '" + ObjectId + "';");
            }
            return;
        }        

        public static void UpdateObjectValue(int ObjectId, string ValueID, string Value)
        {

            if (HasRows("SELECT * FROM object_values WHERE txt_value_id = '" + GetSafeText(ValueID) + "' AND ObjectId = '" + ObjectId + "';"))
            {
                NonQuery(
                    "UPDATE object_values SET txt_value='" + GetSafeText(Value) + "' " +
                                         "WHERE txt_value_id = '" + GetSafeText(ValueID) + "' AND ObjectId = '" + ObjectId + "';");
            }
            return;
        }

        public static string GetObjectValue(int ObjectId, string Label)
        {  
             DataTable dt = GetDataTable("SELECT * FROM object_values " +                                        
                                        "WHERE ObjectId = '" + ObjectId +
                                        "' AND txt_label_name = '" + Label + "'");
             if (dt.Rows.Count > 0)
             {
                 return dt.Rows[0]["txt_value"].ToString();
             }
            return String.Empty; 
        }

        public static DataTable GetObjectValues(int objectId)
        {
            return GetDataTable("SELECT * FROM object_values " +
                                        "WHERE ObjectId = '" + objectId + "'");            
        }

        #endregion

        #region Object Properties 
        //Object properties are per Object specific across and are not plugin API specific.

        public static string ClearObjectProperties()
        {
            return NonQuery("TRUNCATE TABLE object_properties;TRUNCATE TABLE object_property_settings;TRUNCATE TABLE object_property_options;");
        } 

        public static void NewObjectProperty(string PropertyName, string PropertyFriendlyName, string DefaultValue, int PropertyType)
        {
            if (!HasRows("SELECT * FROM object_properties WHERE txt_property_name = '" + PropertyName + "';"))
            {
                string sql = string.Format("INSERT INTO object_properties (`txt_property_friendly_name`, `txt_property_name`, `txt_default_value`, `property_type`) VALUES ('{0}', '{1}', '{2}', {3});",
                    GetSafeText(PropertyFriendlyName),
                    GetSafeText(PropertyName),
                    GetSafeText(DefaultValue),
                    PropertyType);

                NonQuery(sql);
            }
        }

        public static void RemoveObjectProperty(string PropertyName)
        {
            int id = 0;
            string existssql = string.Format("SELECT id FROM object_properties WHERE txt_property_name = '{0}';", GetSafeText(PropertyName));
            DataTable dt = GetDataTable(existssql);

            if (dt.Rows.Count > 0)
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);

            if (id > 0)
            {
                string sql = string.Format("DELETE FROM `object_properties` WHERE `id`='{0}';",
                    id);
                NonQuery(sql);
            }
        }

        public static string GetObjectPropertyValue(int objectId, string PropertyName)
        {
            int objectPropertyId = GetObjectPropertySettingsId(PropertyName);
            //TRY TO FIND SETTING FOR THIS SPECIFIC DEIVCE
            string sql = String.Format("SELECT txt_property_value FROM `object_property_settings` WHERE object_id = {0} AND object_property_id = {1};",
                                objectId,
                                objectPropertyId);

            DataTable dt = GetDataTable(sql);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["txt_property_value"].ToString();
            }
            else  //GET DEFAULT VALUE
            {
                return GetObjectPropertyDefaultValue(PropertyName);
            }
        }

        public static string GetObjectPropertyDefaultValue(string PropertyName)
        {
            string sql = String.Format("SELECT txt_default_value FROM `object_properties` WHERE txt_property_name = '{0}';",
                                PropertyName);

            DataTable dt = GetDataTable(sql);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["txt_default_value"].ToString();
            }
            return String.Empty;
        }

        public static int GetObjectPropertySettingsId(string PropertyName)
        {
            int id = 0;
            
            string sql = String.Format("SELECT id FROM `object_properties` WHERE txt_property_name = '{0}';",
                                PropertyName);

            DataTable dt = GetDataTable(sql);

            if (dt.Rows.Count > 0)
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);

            if (id == 0)
                Logger.WriteToLog(UrgencyLevel.ERROR, "Object Property Not Found!", "DatabaseControl");

            return id;
        }

        public static string SetObjectPropertyValue(string PropertyName, int objectId, string value)
        {
            int objPropertyId = GetObjectPropertySettingsId(PropertyName);

            if (objPropertyId > 0)
            {
                string istheresql = string.Format("SELECT id FROM `object_property_settings` WHERE object_id = {0} AND object_property_id = {1};",
                                                    objectId,
                                                    objPropertyId);
                 
                DataTable dt = GetDataTable(istheresql);
                 if (dt.Rows.Count > 0)
                 {
                     string updatesql = string.Format("UPDATE `object_property_settings` SET `txt_property_value`='{0}' WHERE `id`={1};",
                         GetSafeText(value),
                         dt.Rows[0]["id"].ToString());

                     return NonQuery(updatesql);
                 }
                 else
                 {
                     string insertsql = string.Format("INSERT INTO `object_property_settings` (`object_id`, `object_property_id`, `txt_property_value`) VALUES ({0}, {1}, '{2}');",
                     objectId,
                     objPropertyId,
                     GetSafeText(value));

                     return NonQuery(insertsql);
                 }
                
            }
            else
                Logger.WriteToLog(UrgencyLevel.ERROR, "Could not set object property. Object Property Not Found!", "DatabaseControl");
            
            return string.Empty;
        }

        public static DataTable GetObjectProperties()
        {
            return GetDataTable("SELECT * FROM `object_properties`;");
        }

        public static void NewObjectPropertyOption(string PropertyName, string OptionValue)
        {
            int objPropertySettingsId = GetObjectPropertySettingsId(PropertyName);

            if (objPropertySettingsId > 0)
            {
                string isalreadyinstalledsql = string.Format("SELECT id FROM `object_property_options` WHERE object_property_id = {0} AND txt_option = '{1}';",
                                                            objPropertySettingsId,
                                                            GetSafeText(OptionValue));
                if(!HasRows(isalreadyinstalledsql))
                {
                    string newoptionsql = string.Format("INSERT INTO `object_property_options` (`object_property_id`, `txt_option`) VALUES ({0}, '{1}');",
                                                        objPropertySettingsId,
                                                        GetSafeText(OptionValue));
                    NonQuery(newoptionsql);                        
                }
            }
            else
                Logger.WriteToLog(UrgencyLevel.ERROR, "Could install new object property option. Object Property Not Found!", "DatabaseControl");
        }

        public static List<string> GetObjectPropertyOptions(string PropertyName)
        {
            List<string> options = new List<string>();
            int objPropertySettingsId = GetObjectPropertySettingsId(PropertyName);

            if (objPropertySettingsId > 0)
            {
                string sql = string.Format("SELECT txt_option FROM `object_property_options` WHERE object_property_id = '{0}';",
                    objPropertySettingsId);
                
                foreach(DataRow dr in GetDataTable(sql).Rows)
                {
                    options.Add(dr["txt_option"].ToString());
                }                
            }
            else
                Logger.WriteToLog(UrgencyLevel.ERROR, "Could get object property options. Object Property Not Found!", "DatabaseControl");

            return options;
        }

        #endregion

        #region Object Types
        public static int NewObjectType(string APIName, string objectTypeName, bool showInList)
        {
            int pluginId = GetPluginId(APIName);
            if (pluginId != -1)
            {
                string sql = "SELECT id FROM object_types WHERE plugin_id = " + pluginId + " AND txt_object_type = '" +
                             objectTypeName + "';";


                if (!HasRows(sql))
                {
                    NonQuery("INSERT INTO object_types (plugin_id, txt_object_type, show_in_list) VALUES (" + pluginId + ", '" +
                           objectTypeName + "', " + Convert.ToInt32(showInList) + ");");

                    int id = 0;
                    int.TryParse(GetDataTable(sql).Rows[0]["id"].ToString(),out id);

                    return id; 
                }
                
            }
            return 0;
        }

        public static int GetObjectTypeId(int objectId)
        {
            DataTable dt = GetDataTable("SELECT object_type_id FROM objects WHERE id = " + objectId);
            int id = 0;
            if (dt.Rows.Count == 1)
            {
                int.TryParse(dt.Rows[0]["object_type_id"].ToString(), out id);
            }
            return id;
        }

        public static int GetObjectTypeId(string objectName)
        {
            DataTable dt = GetDataTable("SELECT id FROM object_types WHERE txt_object_type = '" + objectName + "';");
            int id = 0;
            if (dt.Rows.Count == 1)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }
            return id;
        }

        public static int GetObjectTypeId(string APIName, string objectTypeName)
        {
            DataTable dt = GetDataTable("SELECT object_types.id FROM plugins " +
                                            "INNER JOIN object_types ON plugins.id = object_types.plugin_id " +
                                            "WHERE plugins.txt_api_name = '" + APIName +
                                            "' AND object_types.txt_object_type = '" + objectTypeName + "'");            
            int id = 0;
            if (dt.Rows.Count == 1)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }
            return id;            
        }

        public static int GetObjectTypeEventId(int objectTypeId, string evt)
        {
            DataTable dt =
                GetDataTable("SELECT id FROM object_type_events WHERE object_type_id = " + objectTypeId +
                             " AND txt_event = '" + evt + "'");
            int id = 0;
            if (dt.Rows.Count == 1)
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            return id;
        }

        public static DataTable GetObjectTypeEvents(int objectTypeId)
        {
            return GetDataTable("SELECT * FROM object_type_events WHERE object_type_id = " + objectTypeId);
        }
        
        #endregion

        #region Objects
        public static DataTable GetObjects(bool forList)
        {
            string sqlCommand = "SELECT objects.id, objects.txt_object_name, objects.node_id, object_types.txt_object_type, objects.object_type_id, plugins.txt_api_name, objects.last_heard_from FROM objects INNER JOIN object_types ON object_types.id = objects.object_type_id INNER JOIN plugins ON object_types.plugin_id = plugins.id WHERE plugins.txt_api_name != 'BUILTIN'";
            if (forList)
                sqlCommand += " AND show_in_list = 1";
            
            return GetDataTable(sqlCommand);
        }

        public static DataRow GetObject(int objectID)
        {
            string sqlCommand = "SELECT objects.id, objects.txt_object_name, objects.node_id, object_types.txt_object_type, objects.object_type_id, plugins.txt_api_name, objects.last_heard_from FROM objects INNER JOIN object_types ON object_types.id = objects.object_type_id INNER JOIN plugins ON object_types.plugin_id = plugins.id WHERE plugins.txt_api_name != 'BUILTIN' AND objects.id = " + objectID + ";";
            
            DataTable dt =  GetDataTable(sqlCommand); 
            if(dt.Rows.Count > 0) 
                return GetDataTable(sqlCommand).Rows[0];

            return null;
        }

        public static void SetLastHeardFrom(int objectId, DateTime datetime)
        {
            NonQuery("UPDATE objects SET last_heard_from='" + datetime.ToString() + "' WHERE id='" + objectId + "';");
            ObjectModified("DatabaseControl", new EventArgs());
        }

        public static string GetObjectName(int objectId)
        {
            string sqlCommand = "SELECT objects.txt_object_name FROM objects WHERE objects.id = " + objectId;
            
           DataTable dt=  GetDataTable(sqlCommand);

           if (dt.Rows.Count > 0)
               return dt.Rows[0]["txt_object_name"].ToString();
           else
               return "Object #" + objectId.ToString();                      
        }

        /// <summary>
        /// Returns null if not found
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static string GetObjectType(int objectId)
        {
            string sqlCommand = "SELECT object_types.txt_object_type FROM object_types " +
                "INNER JOIN objects ON objects.object_type_id = object_types.id " +
                                "WHERE objects.id = " + objectId;

            DataTable dt = GetDataTable(sqlCommand);

            if (dt.Rows.Count > 0)
                return dt.Rows[0]["txt_object_type"].ToString();

            return null;
        }

        public static string SetObjectName(int objectId, string objectName)
        {
            string sqlCommand = "UPDATE objects SET txt_object_name='"+GetSafeText(objectName)+"' WHERE id=" + objectId;

            string result = NonQuery(sqlCommand);

            if(String.IsNullOrEmpty(result))
                ObjectModified("DatabaseControl", new EventArgs());

            return result;
        }

        public static void NewObject(string APIName, int nodeId, string objectType, string objectName)
        {
            int pluginId = GetPluginId(APIName);
            if (pluginId > 0)
            {
                DataTable dt =
                    GetDataTable("SELECT object_types.id FROM object_types WHERE plugin_id = " + pluginId +
                                 " AND txt_object_type = '" + objectType + "';");
                if (dt.Rows.Count > 0)
                {
                    if (
                        !HasRows("SELECT id FROM objects WHERE object_type_id = " + dt.Rows[0]["id"] + " AND node_id = '" +
                                 nodeId + "';"))
                    {
                        if (!HasRows("SELECT * FROM objects WHERE txt_object_name = '" + objectName + "'"))
                        {
                            int objId =
                                InsertGetId("INSERT INTO objects (object_type_id, node_id, txt_object_name) VALUES (" +
                                            dt.Rows[0]["id"] + ", " + nodeId + ", '" + objectName + "');");
                            ObjectModified("DatabaseControl", new EventArgs());
                        }
                        else
                        {
                            Logger.WriteToLog(UrgencyLevel.ERROR, "Plugin tried to add a object with a duplicate name!", APIName);
                        }
                    }
                }
            }
        }     

        public static int GetObjectId(string APIName, string nodeID)
        {
            return GetObjectId(GetPluginId(APIName), nodeID);
        }

        public static int GetNodeId(string APIName, string objectID)
        {
            int nodeId = 0;
            DataTable dt =
                GetDataTable(
                    "SELECT objects.node_id FROM objects INNER JOIN object_types ON object_types.id = objects.object_type_id WHERE objects.id = " +
                    objectID + " AND object_types.plugin_id = " + GetPluginId(APIName));
            if (dt.Rows.Count == 1)
            {
                int.TryParse(dt.Rows[0]["node_id"].ToString(), out nodeId);
            }
            return nodeId;
        }

        public static int GetObjectId(int pluginId, string nodeId)
        {
            DataTable dt =
                GetDataTable(
                    "SELECT objects.id FROM objects INNER JOIN object_types ON object_types.id = objects.object_type_id WHERE object_types.plugin_id = " +
                    pluginId + " AND objects.node_id = " + nodeId);
            if (dt.Rows.Count == 1)
            {
                int objId = 0;
                int.TryParse(dt.Rows[0]["id"].ToString(), out objId);
                return objId;
            }
            if (dt.Rows.Count == 0)
                Logger.WriteToLog(UrgencyLevel.ERROR, "Tried to access a plugin that doesnt exist!", "GETOBJECTID");
            else
                Logger.WriteToLog(UrgencyLevel.ERROR, "There seems to be too many plugins with the same name!",
                                  "GETOBJECTID");

            return -1;
        }

        public static DataTable GetObjectByName(string objectName)
        {
            return GetDataTable("SELECT objects.*, plugins.txt_api_name FROM objects INNER JOIN object_types ON object_types.id = objects.object_type_id INNER JOIN plugins ON plugins.id = object_types.plugin_id WHERE txt_object_name = '" + objectName + "'");
        }

        public static DataTable GetObjectByNodeID(string objectNodeID)
        {
            return GetDataTable("SELECT objects.*, object_types.txt_object_type, objects.object_type_id, plugins.txt_api_name FROM objects INNER JOIN object_types ON object_types.id = objects.object_type_id INNER JOIN plugins ON plugins.id = object_types.plugin_id WHERE objects.node_id = '" + objectNodeID + "'");
        }

        public static string GetObjectPluginAPIName(int objectId)
        {
            DataTable dt =
                GetDataTable(
                    "SELECT plugins.txt_api_name FROM objects INNER JOIN object_types ON object_types.id = objects.object_type_id INNER JOIN plugins ON plugins.id = object_types.plugin_id WHERE objects.id = " +
                    objectId);
            if (dt.Rows.Count == 1)
                return dt.Rows[0]["txt_api_name"].ToString();
            return string.Empty;
        }
        #endregion

        #region Commands

        public static string ClearAllCommands()
        {
            return NonQuery("TRUNCATE TABLE object_type_commands;"+
                            "TRUNCATE TABLE object_commands;"+
                            "TRUNCATE TABLE commands;"+
                            "TRUNCATE TABLE object_type_commands_options;"+
                             "TRUNCATE TABLE builtin_commands;" +
                             "TRUNCATE TABLE builtin_command_options;" +
                            "TRUNCATE TABLE object_commands_options;");
        }

        public static void NewzCommandType(string typeName)
        {
            string hasrowssql = string.Format("SELECT id FROM `command_types`WHERE txt_type = '{0}';", typeName);

            if (!HasRows(hasrowssql))
            {
                string insertsql = string.Format("INSERT INTO `command_types` (`txt_type`) VALUES ('{0}');", typeName);
                NonQuery(insertsql);
            }
        }

        public static int GetzCommandTypeId(string typeName)
        {
            string sql = string.Format("SELECT id FROM `command_types`WHERE txt_type = '{0}';", typeName);
            int zCommandTypeId = 0;
            int.TryParse(GetDataTable(sql).Rows[0]["id"].ToString(), out zCommandTypeId);
            return zCommandTypeId;
        }

        public static void UpdateObjectCommand(int cmdId, string CommandName, string CommandFriendlyName, int paramType, string help, string txt_custom_data1, string txt_custom_data2, int sort_order)
        {
            string sql = string.Format("UPDATE `object_commands` SET `txt_command_name`='{0}', `txt_command_friendly_name`='{1}', `param_type`={2}, `txt_custom_data1`='{3}', `txt_custom_data2`='{4}', `txt_cmd_help`='{5}', `sort_order`='{6}' WHERE `id`='{7}';",                                               
                                                  GetSafeText(CommandName),
                                                  GetSafeText(CommandFriendlyName),
                                                  paramType,
                                                  GetSafeText(txt_custom_data1),
                                                  GetSafeText(txt_custom_data2),
                                                  GetSafeText(help),
                                                  sort_order,
                                                  cmdId);
            NonQuery(sql);
        }

        public static int NewObjectCommand(int objectId, string CommandName, string CommandFriendlyName, int paramType, string help, string txt_custom_data1, string txt_custom_data2, int sort_order)
        {
            string isalreadyinstalledsql = string.Format("SELECT id FROM `object_commands` WHERE txt_command_name = '{0}' AND object_id = '{1}';",
                CommandName,
                objectId);

            DataTable cmd_dt = GetDataTable(isalreadyinstalledsql);

            if (cmd_dt.Rows.Count == 0)
            {
                string insertsql = string.Format("INSERT INTO `object_commands` (`object_id`, `txt_command_name`, `txt_command_friendly_name`, `param_type`, `txt_custom_data1`, `txt_custom_data2`, `txt_cmd_help`, `sort_order`) VALUES ({0}, '{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}');",
                                                  objectId,
                                                  GetSafeText(CommandName),
                                                  GetSafeText(CommandFriendlyName),
                                                  paramType,
                                                  GetSafeText(txt_custom_data1),
                                                  GetSafeText(txt_custom_data2),
                                                  GetSafeText(help),
                                                  sort_order);
                NonQuery(insertsql);
                cmd_dt = GetDataTable(isalreadyinstalledsql);
            }


            int NewCmdID = 0;
            int.TryParse(cmd_dt.Rows[0]["id"].ToString(), out NewCmdID);
            return NewCmdID;
        }

        public static int NewBuiltinCommand(string CommandName, string CommandFriendlyName, int paramType, bool ShowOnDynamicObjectList, string help, string txt_custom_data1, string txt_custom_data2)
        {
            string isalreadyinstalledsql = string.Format("SELECT id FROM `builtin_commands` WHERE txt_command_name = '{0}';",
                CommandName);

            DataTable cmd_dt = GetDataTable(isalreadyinstalledsql);

            if (cmd_dt.Rows.Count == 0)
            {
                string insertsql = string.Format("INSERT INTO `builtin_commands` (`txt_command_name`, `txt_command_friendly_name`, `param_type`, `txt_custom_data1`, `txt_custom_data2`, `txt_cmd_help`, `show_on_dynamic_obj_list`) VALUES ('{0}', '{1}', {2}, '{3}', '{4}', '{5}', '{6}');",                                                  
                                                  GetSafeText(CommandName),
                                                  GetSafeText(CommandFriendlyName),
                                                  paramType,
                                                  GetSafeText(txt_custom_data1),
                                                  GetSafeText(txt_custom_data2),
                                                  GetSafeText(help),
                                                  ShowOnDynamicObjectList.ToString());
                NonQuery(insertsql);
                cmd_dt = GetDataTable(isalreadyinstalledsql);
            }


            int NewCmdID = 0;
            int.TryParse(cmd_dt.Rows[0]["id"].ToString(), out NewCmdID);
            return NewCmdID;
        }

        public static int NewObjectTypeCommand(string APIName, string objectTypeName, string CommandName, string CommandFriendlyName, int paramType, string help, string txt_custom_data1, string txt_custom_data2)
        {
            //Get objectTypeId First
            int objTypeId = GetObjectTypeId(APIName, objectTypeName);
            if (objTypeId == 0)
            {
                Logger.WriteToLog(UrgencyLevel.ERROR, "Tried to access a plugin or object type that doesn't exist while installing a ObjectTypeCommand!",
                                     "DatabaseControl");
                return 0;
            }

            //Install Command
            string isalreadyinstalledsql = string.Format("SELECT id FROM `object_type_commands` WHERE txt_command_name = '{0}' AND object_type_id = '{1}';",
                CommandName,
                objTypeId);

            DataTable cmd_dt = GetDataTable(isalreadyinstalledsql);

            if (cmd_dt.Rows.Count == 0)
            {
                string insertsql = string.Format("INSERT INTO `object_type_commands` (`object_type_id`, `txt_command_name`, `txt_command_friendly_name`, `param_type`, `txt_custom_data1`, `txt_custom_data2`, `txt_cmd_help`) VALUES ({0}, '{1}', '{2}', {3}, '{4}', '{5}', '{6}');",
                                                  objTypeId,
                                                  GetSafeText(CommandName),
                                                  GetSafeText(CommandFriendlyName),
                                                  paramType,
                                                  GetSafeText(txt_custom_data1),
                                                  GetSafeText(txt_custom_data2),
                                                  GetSafeText(help));
                NonQuery(insertsql);
                cmd_dt = GetDataTable(isalreadyinstalledsql);                
            }

            int NewCmdID = 0;
            int.TryParse(GetDataTable(isalreadyinstalledsql).Rows[0]["id"].ToString(), out NewCmdID);
            return NewCmdID;
        }

        public static DataTable GetCommand(int id)
        {
            return GetDataTable(string.Format("SELECT * FROM `commands` WHERE id='{0}'", id));
        }
        
        public static DataTable GetCommands()
        {            
            return GetDataTable("SELECT * FROM `commands`;");
        }        

        public static DataTable GetObjectCommand(string CommandId)
        {
            return GetDataTable("SELECT commands.id,txt_command_name,objects.node_id,objects.id as objectId,object_types.txt_object_type,commands.txt_arg,param_type,txt_command_name,txt_command_friendly_name,txt_custom_data1,txt_custom_data2,txt_cmd_help, plugins.txt_api_name FROM commands" +
                                " INNER JOIN object_commands ON object_commands.id = commands.command_id" +
                                " INNER JOIN objects ON objects.id = commands.object_id" +
                                " INNER JOIN object_types ON object_types.id = objects.object_type_id" +
                                " INNER JOIN plugins ON plugins.id = object_types.plugin_id" +
                                " WHERE object_commands.id = " + CommandId + ";");
        }

        public static DataTable GetObjectTypeCommand(string ObjectCmdTypeId)
        {
            return GetDataTable("SELECT commands.id,txt_command_name,objects.node_id,objects.id as objectId,object_types.txt_object_type,commands.txt_arg,param_type,txt_command_name,txt_command_friendly_name,txt_custom_data1,txt_custom_data2,txt_cmd_help, plugins.txt_api_name FROM commands" +
                                " INNER JOIN object_type_commands ON object_type_commands.id = commands.command_id" +
                                " INNER JOIN objects ON objects.id = commands.object_id" +
                                " INNER JOIN object_types ON object_types.id = objects.object_type_id" +
                                " INNER JOIN plugins ON plugins.id = object_types.plugin_id" +
                                " WHERE object_type_commands.id = " + ObjectCmdTypeId + ";");
        }

        public static int GetBuiltinCommandId(string typeName)
        {
            string sql = string.Format("SELECT id FROM `builtin_commands` WHERE txt_command_name = '{0}';", typeName);
            int BuiltinCommandId = 0;
            int.TryParse(GetDataTable(sql).Rows[0]["id"].ToString(), out BuiltinCommandId);
            return BuiltinCommandId;
        }

        public static DataTable GetBuiltinCommand(int CommandId)
        {
            string sql = string.Format("SELECT * FROM `builtin_commands` WHERE id = {0};", CommandId); 
            return GetDataTable(sql);
        }

        public static DataTable GetObjectCommand(int CommandId)
        {
            string sql = string.Format("SELECT * FROM `object_commands` WHERE id = {0};", CommandId);
            return GetDataTable(sql);
        }

        public static DataTable GetObjectTypeCommand(int CommandId)
        {
            string sql = string.Format("SELECT * FROM `object_type_commands` WHERE id = {0};", CommandId);
            return GetDataTable(sql);
        }

        public static DataTable GetBuiltinCommands()
        {
            string sql = string.Format("SELECT * FROM `builtin_commands`;");
            return GetDataTable(sql);
        }

        public static void RemoveQuedCommand(string commandId)
        {
            NonQuery("DELETE FROM commands WHERE id = " + commandId);
        }

        /// <summary>
        /// Returns the id of the new entry in the command table
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="CommandTypeId"></param>
        /// <param name="CommandId"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static int QueCommand(int objectId, int CommandTypeId, int CommandId, string arg)
        {
            int id = 0;
            string sql = string.Format("INSERT INTO `commands` (`object_id`, `command_type_id`, `command_id`, `txt_arg`) VALUES ({0}, {1}, {2}, '{3}');SELECT LAST_INSERT_ID() as id", 
                                        objectId,
                                        CommandTypeId,
                                        CommandId,
                                        arg);
            DataTable dt = GetDataTable(sql);

            if (dt.Rows.Count > 0)
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);                      

            return id;
        }

        public static DataTable GetAllObjectCommandForObject(int objectId)
        {
            string sql = string.Format("SELECT * FROM `object_commands` WHERE object_id = '{0}' ORDER BY sort_order ASC;", objectId);
            return GetDataTable(sql);
        }

        public static DataTable GetAllObjectTypeCommandForObject(int objectId)
        {
            int objectTypeId = GetObjectTypeId(objectId);

            string sql = string.Format("SELECT * FROM `object_type_commands` WHERE object_type_id = '{0}';", objectTypeId);
            return GetDataTable(sql);
        }

        #region Object Command Options
        public static void NewObjectCommandOption(int CommandId, string CommandOption)
        {
            string sqlisalreadyinstalled = string.Format("SELECT id FROM `object_commands_options` WHERE object_command_id = '{0}' AND txt_option = '{1}';",
                                                        CommandId, 
                                                        CommandOption);

            if (GetDataTable(sqlisalreadyinstalled).Rows.Count == 0)
            {
                string inserrtsql = string.Format("INSERT INTO `object_commands_options` (`object_command_id`, `txt_option`) VALUES ({0}, '{1}');",
                                        CommandId,
                                        CommandOption);

                NonQuery(inserrtsql);
            }
        }
        
        public static List<string> GetObjectCommandOptions(int CommandId)
        {
            List<string> options = new List<string>();
            string sql = string.Format("SELECT txt_option FROM `object_commands_options` WHERE object_command_id = '{0}';",
                                                        CommandId);

            foreach (DataRow dr in GetDataTable(sql).Rows)
            {
                options.Add(dr["txt_option"].ToString());
            }
            return options;
        }
        #endregion

        #region Object Type Command Options
        public static void NewObjectTypeCommandOption(int ObjectTypeCommandId, string CommandOption)
        {
            string sqlisalreadyinstalled = string.Format("SELECT id FROM `object_type_commands_options` WHERE object_type_command_id = '{0}' AND txt_option = '{1}';",
                                                        ObjectTypeCommandId,
                                                        CommandOption);

            if (GetDataTable(sqlisalreadyinstalled).Rows.Count == 0)
            {
                string inserrtsql = string.Format("INSERT INTO `object_type_commands_options` (`object_type_command_id`, `txt_option`) VALUES ({0}, '{1}');",
                                        ObjectTypeCommandId,
                                        CommandOption);

                NonQuery(inserrtsql);
            }
        }

        public static List<string> GetObjectTypeCommandOptions(int ObjectTypeCommandId)
        {
            List<string> options = new List<string>();
            string sql = string.Format("SELECT txt_option FROM `object_type_commands_options` WHERE object_type_command_id = '{0}';",
                                                        ObjectTypeCommandId);

            foreach (DataRow dr in GetDataTable(sql).Rows)
            {
                options.Add(dr["txt_option"].ToString());
            }
            return options;
        }
        #endregion

        #region Builtin Command Options
        public static void NewBuiltinCommandOption(int BuiltinCommandId, string CommandOption)
        {
            string sqlisalreadyinstalled = string.Format("SELECT id FROM `builtin_command_options` WHERE builtin_command_id = '{0}' AND txt_option = '{1}';",
                                                        BuiltinCommandId,
                                                        CommandOption);

            if (GetDataTable(sqlisalreadyinstalled).Rows.Count == 0)
            {
                string inserrtsql = string.Format("INSERT INTO `builtin_command_options` (`builtin_command_id`, `txt_option`) VALUES ({0}, '{1}');",
                                        BuiltinCommandId,
                                        CommandOption);

                NonQuery(inserrtsql);
            }
        }

        public static List<string> GetBuiltinCommandOptions(int BuiltinCommandId)
        {
            List<string> options = new List<string>();
            string sql = string.Format("SELECT txt_option FROM `builtin_command_options` WHERE builtin_command_id = '{0}';",
                                                        BuiltinCommandId);

            foreach (DataRow dr in GetDataTable(sql).Rows)
            {
                options.Add(dr["txt_option"].ToString());
            }
            return options;
        }
        #endregion

        public static int GetObjectCommandId(int objectId, string command)
        {
            string sql = string.Format("SELECT id FROM `object_commands` WHERE txt_command_name = '{0}' AND object_id = '{1}';",
                                        command,
                                        objectId
                );
            DataTable dt = GetDataTable(sql);

            int id = 0;
            if (dt.Rows.Count == 1)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }
            return id;
        }

        public static int GetObjectTypeCommandId(int objectTypeId, string command)
        {
            string sql = string.Format("SELECT id FROM `object_type_commands` WHERE txt_command_name = '{0}' AND object_type_id = '{1}';",
                                        command,
                                        objectTypeId);
            DataTable dt = GetDataTable(sql);

            int id = 0;
            if (dt.Rows.Count == 1)
            {
                int.TryParse(dt.Rows[0]["id"].ToString(), out id);
            }
            return id;
        }       

        #endregion

        #region Scripts
        public static DataTable GetEventScriptsForObject(int objectId, string evt)
        {
            //int objId = GetObjectId(APIName, objectId);
            int objTypeId = GetObjectTypeId(objectId);
            int objTypeEventId = GetObjectTypeEventId(objTypeId, evt);

            return
                GetDataTable("SELECT * FROM event_scripts WHERE object_id = " + objectId + " AND object_type_event_id = " +
                             objTypeEventId);
        }

        public static DataTable GetEventScripts()
        {
            return
                GetDataTable("SELECT * FROM event_scripts INNER JOIN objects ON objects.id = event_scripts.object_id INNER JOIN object_type_events ON object_type_events.id = event_scripts.object_type_event_id");
        }

        public static void AddEventScript(int objectId, int eventId, string eventName, string script)
        {
            NonQuery("INSERT INTO event_scripts (object_id, object_type_event_id, txt_event_name, txt_script) VALUES (" + objectId + ", " + eventId + ", '" + eventName + "', '" + script + "')");
        }

        public static void UpdateEventScript(int scriptId, int objectId, int eventId, string eventName, string script)
        {
             NonQuery("UPDATE event_scripts SET object_id = " + objectId + ", object_type_event_id = " + eventId + ", txt_event_name = '" + eventName + "', txt_script = '" + script + "' WHERE id = " + scriptId);
        }

        public static DataTable GetEventScriptById(int scriptId)
        {
            return
                GetDataTable(
                    "SELECT objects.txt_object_name, object_type_events.txt_event_name AS evt_name, event_scripts.txt_event_name, event_scripts.txt_script FROM event_scripts INNER JOIN objects ON objects.id = event_scripts.object_id INNER JOIN object_type_events ON object_type_events.id = event_scripts.object_type_event_id WHERE event_scripts.id = " +
                    scriptId);
        }

        public static void DeleteEventScript(int scriptId)
        {
            NonQuery("DELETE FROM event_scripts WHERE id = " + scriptId);
        }
        #endregion

        #region Groups
        public static DataTable GetGroups()
        {
            return GetDataTable("SELECT * FROM groups");
        }

        public static void DeleteGroup(int groupId)
        {
            NonQuery("DELETE FROM groups WHERE id = " + groupId);
            NonQuery("DELETE FROM group_objects WHERE group_id = " + groupId);
            ObjectModified("DatabaseControl", new EventArgs());
        }

        public static DataTable GetObjectGroups(int ObjectID)
        {
            return
                GetDataTable(
                    "SELECT group_objects.id, group_objects.group_id, groups.txt_group_name FROM group_objects INNER JOIN groups ON groups.id = group_objects.group_id WHERE group_objects.object_id = " +
                    ObjectID);
        }

        public static DataTable GetGroupObjects(int groupId)
        {
            return
                GetDataTable(
                    "SELECT objects.id, objects.txt_object_name FROM group_objects INNER JOIN objects ON objects.id = group_objects.object_id WHERE group_objects.group_id = " +
                    groupId);
        }

        public static DataTable GetGroupObjects(string groupName)
        {
            return
                GetDataTable(
                    "SELECT objects.id FROM group_objects INNER JOIN objects ON objects.id = group_objects.object_id INNER JOIN groups ON groups.id = group_objects.group_id WHERE groups.txt_group_name = '" +
                    groupName + "';");
        }

        public static DataTable GetGroupLeftOverObjects(int groupId)
        {
            return
                GetDataTable(
                    "SELECT objects.id, objects.txt_object_name FROM objects INNER JOIN object_types ON objects.object_type_id = object_types.id WHERE objects.id NOT IN (SELECT object_id FROM group_objects WHERE group_id = " +
                    groupId + ") AND object_types.show_in_list = 1");
        }

        public static void AddObjectToGroup(int groupId, int objectId)
        {
            if (!HasRows("SELECT * FROM group_objects WHERE group_id = " + groupId + " AND object_id = " + objectId))
            {
                NonQuery("INSERT INTO group_objects (group_id, object_id) VALUES (" + groupId + ", " + objectId + ")");
                ObjectModified("DatabaseControl", new EventArgs());
            }
        }

        public static void RemoveObjectFromGroup(int groupId, int objectId)
        {
           NonQuery("DELETE FROM group_objects WHERE group_id = " + groupId + " AND object_id = " + objectId);
           ObjectModified("DatabaseControl", new EventArgs());
        }

        public static void AddGroup(string groupName)
        {
            NonQuery("INSERT INTO groups (txt_group_name) VALUES ('" + groupName + "')");
            ObjectModified("DatabaseControl", new EventArgs());
        }

        public static void SetGroupName(int GroupID, string groupName)
        {
            NonQuery("UPDATE groups SET txt_group_name ='" + GetSafeText(groupName) + "' WHERE id=" + GroupID);
            ObjectModified("DatabaseControl", new EventArgs());
        }

        public static string GetGroupName(int GroupID)
        {
            DataTable dt = GetDataTable("SELECT txt_group_name FROM groups WHERE id = " + GroupID + ";");            
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0]["txt_group_name"].ToString();
            }
            return String.Empty;
        }
        
        #endregion

        #region Common
        private static MySqlDataReader GetDataReader(string command)
        {
            try
            {
                var conn = new MySqlConnection(_connectionString);
                var cmd = new MySqlCommand(command, conn);
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                Logger.WriteToLog(UrgencyLevel.ERROR, e.Message, "DATABASE");                
                throw;
            }
        }

        private static DataTable GetDataTable(string command)
        {
            MySqlDataReader reader = GetDataReader(command);
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            return dt;
        }

        private static bool HasRows(string command)
        {
            try
            {
                var conn = new MySqlConnection(_connectionString);
                var cmd = new MySqlCommand(command, conn);
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                bool hasRows = reader.HasRows;
                reader.Close();
                return hasRows;
            }
            catch (Exception e)
            {
                Logger.WriteToLog(UrgencyLevel.ERROR, e.Message, "DATABASE");
                throw;
            }
        }

        private static int InsertGetId(string command)
        {
            try
            {
                var conn = new MySqlConnection(_connectionString);
                var cmd =
                    new MySqlCommand(command + " select last_insert_id();", conn);
                conn.Open();
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                return id;
            }
            catch (Exception e)
            {
                Logger.WriteToLog(UrgencyLevel.ERROR, e.Message, "DATABASE");
                throw;
            }
        }

        private static string GetSafeText(string txt)
        {
            return txt.Trim().Replace("'", "''");
        }

        private static string NonQuery(string command)
        {
            try
            {
                var conn = new MySqlConnection(_connectionString);
                var cmd =
                    new MySqlCommand(command, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
               
            }
            catch (Exception e)
            {
                Logger.WriteToLog(UrgencyLevel.ERROR, e.Message, "DATABASE");
                return e.Message + Environment.NewLine;
                throw;
            }
            return string.Empty;
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        /// <returns>Empty if success otherwise return the error.</returns>
        public static string ExamineDatabase()
        {
            try
            {                
                MySqlDataReader reader = null;                
                var conn = new MySqlConnection(_connectionString);
                var cmd = new MySqlCommand("SHOW TABLES;", conn);
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(reader);

                if (dt.Rows.Count < 10)
                {
                    return "The selected database does not appear to be a vaild zvirtualscenes database.";
                }
                reader.Close();                
                conn.Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns null if current else the current database version number
        /// </summary>
        /// <returns></returns>
        public static string GetOutdatedDbVersion()
        {
            try
            {
                MySqlDataReader reader = null;
                var conn = new MySqlConnection(_connectionString);
                var cmd = new MySqlCommand("SELECT version FROM version;", conn);
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(reader);
                reader.Close();

                if (dt.Rows.Count == 1)            
                    if (!dt.Rows[0]["version"].ToString().Equals(CurrentVersion.RequiredDatabaseVersion))                    
                        return dt.Rows[0]["version"].ToString();
            
                conn.Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }


        #endregion

        #region Scenes
        public static class Scenes
        {
            public static DataTable GetScenes()
            {
                return GetDataTable("SELECT * FROM scenes ORDER BY sort_order ASC;");
            }

            public static DataTable GetScene(int sceneID)
            {
                return GetDataTable(string.Format("SELECT * FROM scenes WHERE id = {0};",sceneID));
            }

            public static DataTable GetSceneCMDs(int sceneID)
            {
                return GetDataTable(string.Format("SELECT * FROM scenes_cmds WHERE scene_id = '{0}' ORDER BY sort_order ASC;", sceneID));
            }

            public static void DeleteScene(int sceneID)
            {
                NonQuery(string.Format("DELETE FROM scenes WHERE id='{0}';", sceneID));
                NonQuery(string.Format("DELETE FROM scenes_cmds WHERE scene_id='{0}';",sceneID));
            }

            public static int AddScene(string name)
            {
                int id = 0;
                string sql = string.Format("INSERT INTO scenes (`txt_name`,`sort_order`) VALUES ('{0}', 99);SELECT LAST_INSERT_ID() as id;", 
                                            name);
                DataTable dt = GetDataTable(sql);

                if (dt.Rows.Count > 0)
                    int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                return id;
            }

            public static void SetOrder(int SceneID, int order)
            {
                NonQuery(string.Format("UPDATE scenes SET `sort_order`={0} WHERE `id`='{1}';", order, SceneID));
            }

            public static void UpdateName(int SceneID, string Name)
            {
                NonQuery(string.Format("UPDATE scenes SET `txt_name`='{0}' WHERE `id`='{1}';", Name, SceneID));
            }

            public static void AddSceneCommand(int SceneID, int ObjID, int commandTypeID, int commandID, string arg, int sort)
            {                
                NonQuery(string.Format("INSERT INTO scenes_cmds (`scene_id`, `object_id`, `command_type_id`, `command_id`, `txt_arg`, `sort_order`) VALUES ({0}, {1}, {2}, {3}, '{4}', {5});", 
                    SceneID,
                    ObjID,
                    commandTypeID,
                    commandID,
                    arg,
                    sort));
            }

            public static void UpdateSceneCommand(int SceneID, int SceneCommandID, int ObjID, int commandTypeID, int commandID, string arg)
            {
                NonQuery(string.Format("UPDATE scenes_cmds SET `scene_id`={0}, `object_id`={1}, `command_type_id`={2}, `command_id`={3}, `txt_arg`='{4}' WHERE `id`='{5}';",
                    SceneID,
                    ObjID,
                    commandTypeID,
                    commandID,
                    arg,
                    SceneCommandID));
            }

            public static void RemoveSceneCommand(int SceneID, int SceneCmdID)
            {
                NonQuery(string.Format("DELETE FROM scenes_cmds WHERE `id`='{0}' and scene_id = '{1}';",
                    SceneCmdID,
                    SceneID ));
            }

            public static void SetCMDOrder(int SceneCmdID, int order)
            {
                NonQuery(string.Format("UPDATE scenes_cmds SET `sort_order`={0} WHERE `id`='{1}';", order, SceneCmdID));
            }

            public static void SetIsRunning(int SceneID, bool isRunning)
            {
                NonQuery(string.Format("UPDATE scenes SET `is_running`='{0}' WHERE `id`='{1}';", isRunning.ToString(), SceneID));
            }            

            public static class Properties
            {
                public static DataTable GetSceneProperties()
                {
                    return GetDataTable("SELECT * FROM `scene_properties`;");
                }

                public static List<string> GetScenePropertyOptions(string PropertyName)
                {
                    List<string> options = new List<string>();
                    int scenePropertyID = GetPropertyID(GetSafeText(PropertyName));

                    if (scenePropertyID > 0)
                    {
                        string sql = string.Format("SELECT txt_option FROM `scene_property_options` WHERE scene_property_id = '{0}';",
                            scenePropertyID);

                        foreach (DataRow dr in GetDataTable(sql).Rows)
                        {
                            options.Add(dr["txt_option"].ToString());
                        }
                    }
                    else
                        Logger.WriteToLog(UrgencyLevel.ERROR, "Could get scene property options. Scene Property Not Found!", "DatabaseControl");

                    return options;
                }

                public static string SetScenePropertyValue(string PropertyName, int sceneID, string value)
                {
                    int scenePropertyID = GetPropertyID(PropertyName);

                    if (scenePropertyID > 0)
                    {
                        string istheresql = string.Format("SELECT id FROM `scene_property_values` WHERE scene_id = {0} AND scene_property_id = {1};",
                                                            sceneID,
                                                            scenePropertyID);

                        DataTable dt = GetDataTable(istheresql);
                        if (dt.Rows.Count > 0)
                        {
                            string updatesql = string.Format("UPDATE `scene_property_values` SET `txt_property_value`='{0}' WHERE `id`={1};",
                                GetSafeText(value),
                                dt.Rows[0]["id"].ToString());

                            return NonQuery(updatesql);
                        }
                        else
                        {
                            string insertsql = string.Format("INSERT INTO `scene_property_values` (`scene_id`, `scene_property_id`, `txt_property_value`) VALUES ({0}, {1}, '{2}');",
                            sceneID,
                            scenePropertyID,
                            GetSafeText(value));

                            return NonQuery(insertsql);
                        }

                    }
                    else
                        Logger.WriteToLog(UrgencyLevel.ERROR, "Could not set scene property. Scene Property Not Found!", "DatabaseControl");

                    return string.Empty;
                }

                public static void New(string PropertyName, string PropertyDescription, string DefaultValue, int PropertyVauleTypeID)
                {
                    if (!HasRows(string.Format("SELECT * FROM scene_properties WHERE txt_property_name = '{0}';", GetSafeText(PropertyName))))
                    {
                        string sql = string.Format("INSERT INTO `scene_properties` (`txt_property_name`, `txt_property_defualt_value`, `property_type_id`, `txt_property_description`) VALUES ('{0}', '{1}', {2}, '{3}');",
                            GetSafeText(PropertyName),
                            GetSafeText(DefaultValue),
                            PropertyVauleTypeID,
                            GetSafeText(PropertyDescription));

                        NonQuery(sql);
                    }
                }

                public static void Remove(string PropertyName)
                {
                    int id = 0;
                    string existssql = string.Format("SELECT id FROM scene_properties WHERE txt_property_name = '{0}';", GetSafeText(PropertyName));
                    DataTable dt = GetDataTable(existssql);

                    if (dt.Rows.Count > 0)
                        int.TryParse(dt.Rows[0]["id"].ToString(), out id);

                    if (id > 0)
                    {
                        string sql = string.Format("DELETE FROM `scene_properties` WHERE `id`='{0}';",
                            id);
                        NonQuery(sql);
                    }
                }

                public static int GetPropertyID(string PropertyName)
                {
                    int id = 0;

                    string sql = String.Format("SELECT id FROM `scene_properties` WHERE txt_property_name = '{0}';",
                                        GetSafeText(PropertyName));

                    DataTable dt = GetDataTable(sql);

                    if (dt.Rows.Count > 0)
                        int.TryParse(dt.Rows[0]["id"].ToString(), out id);

                    if (id == 0)
                        Logger.WriteToLog(UrgencyLevel.ERROR, "Scene Property Not Found!", "DatabaseControl");

                    return id;
                }

                public static string GetScenePropertyValue(int sceneID, string PropertyName)
                {
                    int scenePropertyID = GetPropertyID(PropertyName);
                    //TRY TO FIND SETTING FOR THIS SPECIFIC DEIVCE
                    string sql = String.Format("SELECT txt_property_value FROM `scene_property_values` WHERE scene_id = {0} AND scene_property_id = {1};",
                                        sceneID,
                                        scenePropertyID);

                    DataTable dt = GetDataTable(sql);

                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]["txt_property_value"].ToString();
                    }
                    else  //GET DEFAULT VALUE
                    {
                        return GetScenePropertyDefaultValue(PropertyName);
                    }
                }

                public static string GetScenePropertyDefaultValue(string PropertyName)
                {
                    string sql = String.Format("SELECT txt_property_defualt_value FROM `scene_properties` WHERE txt_property_name = '{0}';",
                                        GetSafeText(PropertyName));

                    DataTable dt = GetDataTable(sql);

                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]["txt_property_defualt_value"].ToString();
                    }
                    return String.Empty;
                }

                public static void NewPropertyOption(string PropertyName, string OptionValue)
                {
                    int PropertyID = GetPropertyID(PropertyName);

                    if (PropertyID > 0)
                    {
                        string isalreadyinstalledsql = string.Format("SELECT id FROM `scene_property_options` WHERE scene_property_id = {0} AND txt_option = '{1}';",
                                                                    PropertyID,
                                                                    GetSafeText(OptionValue));
                        if (!HasRows(isalreadyinstalledsql))
                        {
                            string insertOption = string.Format("INSERT INTO `scene_property_options` (`scene_property_id`, `txt_option`) VALUES ({0}, '{1}');",
                                                                PropertyID,
                                                                GetSafeText(OptionValue));
                            NonQuery(insertOption);
                        }
                    }
                    else
                        Logger.WriteToLog(UrgencyLevel.ERROR, "Could install new scene property option '"+PropertyName+"'. Object Property Not Found!", "DatabaseControl");
                }                

            }
        }
        #endregion
        
        #region scheduled_tasks
        public static class ScheduledTasks
        {
            public static int Add(string Name,
                                    int Frequency,
                                    string Enabled,
                                    int SceneID,
                                    string RecurMonday,
                                    string RecurTuesday,
                                    string RecurWednesday,
                                    string RecurThursday,
                                    string RecurFriday,
                                    string RecurSaturday,
                                    string RecurSunday,
                                    int RecurDays,
                                    int RecurWeeks,
                                    int RecurMonth,
                                    int RecurDayofMonth,
                                    int RecurSeconds,
                                    string StartTime,
                                    int sort_order    )
            {
                int id = 0;
                string sql = string.Format("INSERT INTO scheduled_tasks (`Name`,`Frequency`, `Enabled`, `Scene_id`, `RecurMonday`, `RecurTuesday`, `RecurWednesday`, `RecurThursday`, `RecurFriday`, `RecurSaturday`, `RecurSunday`, `RecurDays`, `RecurWeeks`, `RecurMonth`, `RecurDayofMonth`, `RecurSeconds`, `StartTime`, `sort_order`) "+
                                            "VALUES ('{0}', '{1}', {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}');SELECT LAST_INSERT_ID() as id;",
                                            Name,
                                            Frequency,
                                            Enabled,
                                            SceneID,
                                            RecurMonday,
                                            RecurTuesday,
                                            RecurWednesday,
                                            RecurThursday,
                                            RecurFriday,
                                            RecurSaturday,
                                            RecurSunday,
                                            RecurDays,
                                            RecurWeeks,
                                            RecurMonth,
                                            RecurDayofMonth,
                                            RecurSeconds,
                                            StartTime,
                                            sort_order);

                DataTable dt = GetDataTable(sql);

                if (dt.Rows.Count > 0)
                    int.TryParse(dt.Rows[0]["id"].ToString(), out id);
                return id;
            }

            public static DataTable GetTask(int TaskID)
            {
                return GetDataTable(string.Format("SELECT * FROM scheduled_tasks WHERE id = {0} ORDER BY sort_order ASC;", TaskID));
            }

            public static DataTable GetTasks()
            {
                return GetDataTable("SELECT * FROM scheduled_tasks ORDER BY sort_order ASC;");
            }

            public static void Remove(int TaskID)
            {
                NonQuery(string.Format("DELETE FROM scheduled_tasks WHERE `id`='{0}';",TaskID));
            }

            public static void Update(int id,
                                    string Name,
                                    int Frequency,
                                    string Enabled,
                                    int SceneID,
                                    string RecurMonday,
                                    string RecurTuesday,
                                    string RecurWednesday,
                                    string RecurThursday,
                                    string RecurFriday,
                                    string RecurSaturday,
                                    string RecurSunday,
                                    int RecurDays,
                                    int RecurWeeks,
                                    int RecurMonth,
                                    int RecurDayofMonth,
                                    int RecurSeconds,
                                    string StartTime,
                                    int sort_order)
            {
                string sql = string.Format("UPDATE scheduled_tasks SET `Name`='{0}', `Enabled`='{1}', `Scene_id`={2}, `Frequency`={3}, `RecurMonday`='{4}', `RecurTuesday`='{5}', `RecurWednesday`='{6}', `RecurThursday`='{7}', `RecurFriday`='{8}', `RecurSaturday`='{9}', `RecurSunday`='{10}', `RecurDays`={11}, `RecurWeeks`={12}, `RecurMonth`={13}, `RecurDayofMonth`={14}, `RecurSeconds`={15}, `StartTime`='{16}', `sort_order`={17} WHERE `id`='{18}';",
                                            Name,                                            
                                            Enabled,
                                            SceneID,
                                            Frequency,
                                            RecurMonday,
                                            RecurTuesday,
                                            RecurWednesday,
                                            RecurThursday,
                                            RecurFriday,
                                            RecurSaturday,
                                            RecurSunday,
                                            RecurDays,
                                            RecurWeeks,
                                            RecurMonth,
                                            RecurDayofMonth,
                                            RecurSeconds,
                                            StartTime,
                                            sort_order,
                                            id);
                NonQuery(sql);
            }
        }
        #endregion

        public static string installBaseDB()
        {
            string sql = String.Empty;
            var conn = new MySqlConnection(_firsttimeconnectionString);

            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(DatabaseControl)).Location;
            string sqlFile = Path.GetDirectoryName(fullPath) + @"\base_db.sql";

            if (File.Exists(sqlFile))
            {
                try
                {
                    sql = System.IO.File.ReadAllText(sqlFile);

                    if (!string.IsNullOrEmpty(sql))
                    {
                        var cmd = new MySqlCommand(sql, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    else
                        return "The SQL file is empty!";
                }
                catch (Exception e)
                {
                    return "A problem has occured importing the Database. \n\n" + e.Message;
                }
            }

            return string.Empty;            
        }

        public static string upgradeBaseDB(string filename)
        {
            string sql = String.Empty;
            var conn = new MySqlConnection(_firsttimeconnectionString);

            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(DatabaseControl)).Location;
            string sqlFile = Path.GetDirectoryName(fullPath) + @"\" + filename;

            if (File.Exists(sqlFile))
            {
                try
                {
                    sql = System.IO.File.ReadAllText(sqlFile);

                    if (!string.IsNullOrEmpty(sql))
                    {
                        var cmd = new MySqlCommand(sql, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    else
                        return "The upgrade SQL file is empty!";
                }
                catch (Exception e)
                {
                    return "A problem has occured upgrading the Database. \n\n" + e.Message;
                }
            }
            else
                return "Upgrade Failed.\n\nCould not find the upgrade file.";
            return string.Empty;
        }

        public static string LastDBError
        {
            get;
            private set;
        }
        public static BindingList<string> GetDatabases()
        {
            BindingList<string> options = new BindingList<string>();
            string sql = "show databases;";
            MySqlDataReader reader = null;
            try
            {
                var conn = new MySqlConnection(_firsttimeconnectionString);
                var cmd = new MySqlCommand(sql, conn);
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                LastDBError = e.Message;
                return null;           
            }

            DataTable dt = new DataTable();
            if (reader != null)
            {
                dt.Load(reader);
                reader.Close();
            }
            else
            {
                LastDBError = "Error reading database list!";
                return null;   
            }

            foreach (DataRow dr in dt.Rows)
            {
                options.Add(dr["Database"].ToString());
            }
            return options;
        }

       

    }
}
