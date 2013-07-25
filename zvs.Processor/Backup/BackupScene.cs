using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.Entities;


namespace zvs.Processor.Backup
{
    public partial class Backup
    {
        [Serializable]
        public class SceneBackup
        {
            public string Name;
            public List<SceneCMDBackup> Commands = new List<SceneCMDBackup>();
        }

        [Serializable]
        public class SceneCMDBackup
        {
            public StoredCMDBackup StoredCommand;
            public int? Order;
        }

        public async static void ExportScenesAsync(string PathFileName, Action<string> Callback)
        {
            List<SceneBackup> scenes = new List<SceneBackup>();
            int CmdCount = 0;
            using (zvsContext context = new zvsContext())
            {
                foreach (Scene s in context.Scenes)
                {
                    SceneBackup SceneBackup = new SceneBackup();
                    SceneBackup.Name = s.Name;

                    foreach (SceneCommand scmd in s.Commands)
                    {
                        SceneCMDBackup SceneCmdBackup = new SceneCMDBackup();
                        SceneCmdBackup.Order = scmd.SortOrder;
                        SceneCmdBackup.StoredCommand = await StoredCMDBackup.ConvertToBackupCommand(scmd.StoredCommand);
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
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SceneBackup>));
                xmlSerializer.Serialize(stream, scenes);
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

        public static async Task ImportScenesAsync(string PathFileName, Action<string> Callback)
        {
            List<SceneBackup> scenes = new List<SceneBackup>();
            FileStream myFileStream = null;

            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<SceneBackup>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    scenes = (List<SceneBackup>)ScenesSerializer.Deserialize(myFileStream);

                    int ImportedCount = 0;
                    int ImportedCmdCount = 0;

                    using (zvsContext context = new zvsContext())
                    {
                        foreach (SceneBackup backupScene in scenes)
                        {
                            Scene s = new Scene();
                            s.Name = backupScene.Name;

                            foreach (SceneCMDBackup backupSceneCMD in backupScene.Commands)
                            {
                                var sc = await StoredCMDBackup.RestoreStoredCommandAsync(context, backupSceneCMD.StoredCommand);
                                if (sc != null)
                                {
                                    s.Commands.Add(new SceneCommand()
                                    {
                                        StoredCommand = sc,
                                        SortOrder = backupSceneCMD.Order
                                    });
                                    ImportedCmdCount++;
                                }
                            }
                            context.Scenes.Add(s);
                            ImportedCount++;
                        }
                        await context.SaveChangesAsync();
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