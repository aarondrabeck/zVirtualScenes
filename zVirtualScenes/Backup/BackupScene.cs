using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zVirtualScenesModel;

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
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                foreach (scene s in context.scenes)
                {
                    BackupScene SceneBackup = new BackupScene();
                    SceneBackup.FreindlyName = s.friendly_name;

                    foreach (scene_commands scmd in s.scene_commands)
                    {
                        BackupSceneCMD SceneCmdBackup = new BackupSceneCMD();
                        SceneCmdBackup.CommandType = (Command_Types)scmd.command_type_id;
                        SceneCmdBackup.CommandArg = scmd.arg;
                        SceneCmdBackup.Order = scmd.sort_order;

                        if (((Command_Types)scmd.command_type_id) == Command_Types.builtin)
                        {
                            builtin_commands bc = context.builtin_commands.FirstOrDefault(o => o.id == scmd.command_id);
                            if (bc != null)
                                SceneCmdBackup.CommandName = bc.name;
                        }

                        if (((Command_Types)scmd.command_type_id) == Command_Types.device_command)
                        {
                            device_commands dc = context.device_commands.FirstOrDefault(o => o.id == scmd.command_id);
                            if (dc != null)
                            {
                                SceneCmdBackup.NodeID = scmd.device.node_id;
                                SceneCmdBackup.CommandName = dc.name;
                            }
                        }

                        if (((Command_Types)scmd.command_type_id) == Command_Types.device_type_command)
                        {
                            device_type_commands dtc = context.device_type_commands.FirstOrDefault(o => o.id == scmd.command_id);
                            if (dtc != null)
                            {
                                SceneCmdBackup.NodeID = scmd.device.node_id;
                                SceneCmdBackup.CommandName = dtc.name;
                            }
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

                    using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                    {
                        foreach (BackupScene backupScene in scenes)
                        {
                            scene s = new scene();
                            s.friendly_name = backupScene.FreindlyName;

                            foreach (BackupSceneCMD backupSceneCMD in backupScene.Commands)
                            {
                                if (backupSceneCMD.CommandType == Command_Types.builtin)
                                {
                                    builtin_commands cmd = context.builtin_commands.FirstOrDefault(o => o.name == backupSceneCMD.CommandName);
                                    if (cmd != null)
                                    {
                                        s.scene_commands.Add(new scene_commands()
                                        {
                                            arg = backupSceneCMD.CommandArg,
                                            command_id = cmd.id,
                                            command_type_id = (int)Command_Types.builtin,
                                            sort_order = backupSceneCMD.Order
                                        });
                                        ImportedCmdCount++;
                                    }
                                }
                                else if (backupSceneCMD.CommandType == Command_Types.device_command)
                                {
                                    device d = context.devices.FirstOrDefault(o => o.node_id == backupSceneCMD.NodeID);
                                    if (d != null)
                                    {
                                        device_commands cmd = d.device_commands.FirstOrDefault(o => o.name == backupSceneCMD.CommandName);
                                        if (cmd != null)
                                        {
                                            s.scene_commands.Add(new scene_commands()
                                            {
                                                arg = backupSceneCMD.CommandArg,
                                                command_id = cmd.id,
                                                command_type_id = (int)Command_Types.device_command,
                                                device_id = d.id,
                                                sort_order = backupSceneCMD.Order
                                            });
                                            ImportedCmdCount++;
                                        }
                                    }
                                }
                                else if (backupSceneCMD.CommandType == Command_Types.device_type_command)
                                {
                                    device d = context.devices.FirstOrDefault(o => o.node_id == backupSceneCMD.NodeID);
                                    if (d != null)
                                    {
                                        device_type_commands cmd = d.device_types.device_type_commands.FirstOrDefault(o => o.name == backupSceneCMD.CommandName);
                                        if (cmd != null)
                                        {
                                            s.scene_commands.Add(new scene_commands()
                                            {
                                                arg = backupSceneCMD.CommandArg,
                                                command_id = cmd.id,
                                                command_type_id = (int)Command_Types.device_type_command,
                                                device_id = d.id,
                                                sort_order = backupSceneCMD.Order
                                            });
                                            ImportedCmdCount++;
                                        }
                                    }
                                }
                            }
                            context.scenes.Add(s);
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