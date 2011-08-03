using System.Data;
using System.Text.RegularExpressions;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;
using zVirtualScenesAPI.Events;

namespace zVirtualScenesCommon.EventCommon
{
    public static class EventControl
    {
        public static void Setup()
        {
            zVirtualSceneEvents.ValueDataChangedEvent += EventTriggered;    
        }

        public static void EventTriggered(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            // Step 1: Get the list of the scripts that are related to this objId and evt
            DataTable dt = DatabaseControl.GetEventScriptsForObject(ObjectId, ValueID);

            Logger.WriteToLog(UrgencyLevel.INFO, ValueID, "Event Triggerd");

            foreach (DataRow dr in dt.Rows)
            {
                // Step 2: Execute each of these scripts.
                string[] scriptLines = Regex.Split(dr["txt_script"].ToString(), "\n");
                foreach (var scriptLine in scriptLines)
                {
                    // This will be a basic script reader for now. No if statements just yet
                    string actionObject = scriptLine.Substring(0, scriptLine.IndexOf("."));
                    string command = scriptLine.Substring(scriptLine.IndexOf(".") + 1);
                    string arg = "";

                    if (command.IndexOf(" ") > -1)
                    {
                        arg = command.Substring(command.IndexOf(" ") + 1);
                        command = command.Substring(0, command.IndexOf(" "));
                    }

                    // Check to see if actionObject actuall exists)
                    foreach (DataRow obj in DatabaseControl.GetObjectByName(actionObject).Rows)
                    {
                        // Get Command ID
                        int objId;
                        int.TryParse(obj["id"].ToString(), out objId);

                        int objTypeId;
                        int.TryParse(obj["object_type_id"].ToString(), out objTypeId);

                        //int commandId = DatabaseControl.GetCommandId(objId, objTypeId, command);

                        //if (objId > 0 && commandId > 0)
                            //DatabaseControl.AddCommand(objId, commandId, arg);
                    }
                }
            }
        }
    }
}
