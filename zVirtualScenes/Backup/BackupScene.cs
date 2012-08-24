using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.Entities;


namespace zVirtualScenes.Backup
{
    public partial class Backup
    {
        [Serializable]
        public class BackupScene
        {
            public string FreindlyName;
            public List<BackupSceneCMD> Commands = new List<BackupSceneCMD>();
        }

        public enum Command_Types
        {
            unknown = 0,
            builtin = 1,
            device_command = 2,
            device_type_command = 3
        }

        [Serializable]
        public class BackupSceneCMD
        {
            public Command_Types CommandType;
            public string CommandName;
            public string CommandArg;
            public int? Order;
            public int NodeID;
        }

        public static void ExportScenesAsyc(string PathFileName, Action<string> Callback)
        {
            List<BackupScene> scenes = new List<BackupScene>();
            int CmdCount = 0;
            using (zvsContext context = new zvsContext())
            {
                foreach (Scene s in context.Scenes)
                {
                    BackupScene SceneBackup = new BackupScene();
                    SceneBackup.FreindlyName = s.Name;

                    foreach (SceneCommand scmd in s.Commands)
                    {
                        BackupSceneCMD SceneCmdBackup = new BackupSceneCMD();
                        SceneCmdBackup.CommandArg = scmd.Argument;
                        SceneCmdBackup.Order = scmd.SortOrder;
                        SceneCmdBackup.CommandName = scmd.Command.UniqueIdentifier;

                        if (scmd.Command is BuiltinCommand)
                        {
                            SceneCmdBackup.CommandType = Command_Types.builtin;
                        }
                        else if (scmd.Command is DeviceTypeCommand)
                        {
                            SceneCmdBackup.CommandType = Command_Types.device_type_command;
                            SceneCmdBackup.NodeID = scmd.Device.NodeNumber;
                        }
                        else if (scmd.Command is DeviceCommand)
                        {
                            SceneCmdBackup.CommandType = Command_Types.device_command;
                            SceneCmdBackup.NodeID = scmd.Device.NodeNumber;
                        }

                        SceneBackup.Commands.Add(SceneCmdBackup);
                        CmdCount++;
                    }
                    scenes.Add(SceneBackup);
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<BackupScene>));
                xmlSerializer.Serialize(stream, scenes);
                stream.Close();
                Callback(string.Format("Exported {0} scenes and {1} scene commands to '{2}'", scenes.Count, CmdCount, Path.GetFileName(PathFileName)));
            }
            catch (Exception e)
            {
                Callback("Error saving " + PathFileName + ": (" + e.Message + ")");
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public static void ImportScenesAsyn(string PathFileName, Action<string> Callback)
        {
            List<BackupScene> scenes = new List<BackupScene>();
            FileStream myFileStream = null;

            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<BackupScene>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    scenes = (List<BackupScene>)ScenesSerializer.Deserialize(myFileStream);
                   
                    int ImportedCount = 0;
                    int ImportedCmdCount = 0;

                    using (zvsContext context = new zvsContext())
                    {
                        foreach (BackupScene backupScene in scenes)
                        {
                            Scene s = new Scene();
                            s.Name = backupScene.FreindlyName;

                            foreach (BackupSceneCMD backupSceneCMD in backupScene.Commands)
                            {
                                if (backupSceneCMD.CommandType == Command_Types.builtin)
                                {
                                    BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(o => o.UniqueIdentifier == backupSceneCMD.CommandName);
                                    if (cmd != null)
                                    {
                                        s.Commands.Add(new SceneCommand()
                                        {
                                            Argument = backupSceneCMD.CommandArg,
                                            Command = cmd,
                                            SortOrder = backupSceneCMD.Order
                                        });
                                        ImportedCmdCount++;
                                    }
                                }
                                else if (backupSceneCMD.CommandType == Command_Types.device_command)
                                {
                                    Device d = context.Devices.FirstOrDefault(o => o.NodeNumber == backupSceneCMD.NodeID);
                                    if (d != null)
                                    {
                                        DeviceCommand cmd = d.Commands.FirstOrDefault(o => o.UniqueIdentifier == backupSceneCMD.CommandName);
                                        if (cmd != null)
                                        {
                                            s.Commands.Add(new SceneCommand()
                                            {
                                                Argument = backupSceneCMD.CommandArg,
                                                Command = cmd,
                                                Device = d,
                                                SortOrder = backupSceneCMD.Order
                                            });
                                            ImportedCmdCount++;
                                        }
                                    }
                                }
                                else if (backupSceneCMD.CommandType == Command_Types.device_type_command)
                                {
                                    Device d = context.Devices.FirstOrDefault(o => o.NodeNumber == backupSceneCMD.NodeID);
                                    if (d != null)
                                    {
                                        DeviceTypeCommand cmd = d.Type.Commands.FirstOrDefault(o => o.UniqueIdentifier == backupSceneCMD.CommandName);
                                        if (cmd != null)
                                        {
                                            s.Commands.Add(new SceneCommand()
                                            {
                                                Argument = backupSceneCMD.CommandArg,
                                                Command = cmd,
                                                Device = d,
                                                SortOrder = backupSceneCMD.Order
                                            });
                                            ImportedCmdCount++;
                                        }
                                    }
                                }
                            }
                            context.Scenes.Add(s);
                            ImportedCount++;
                        }
                        context.SaveChanges();
                    }
                    Callback(string.Format("Imported {0} scenes and {1} scene commands from '{2}'", ImportedCount, ImportedCmdCount, Path.GetFileName(PathFileName)));
                }
                else
                    Callback(string.Format("File '{0}' not found.", PathFileName));

            }
            catch (Exception e)
            {
                Callback("Error importing " + PathFileName + ": (" + e.Message + ")");
            }
            finally
            {

                if (myFileStream != null)
                    myFileStream.Close();
            }
        }
    }


}