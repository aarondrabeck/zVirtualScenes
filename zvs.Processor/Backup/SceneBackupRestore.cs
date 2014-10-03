using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public class SceneBackupRestore : BackupRestore
    {
        public class SceneBackup
        {
            public string Name { get; set; }
            public List<SceneCMDBackup> Commands = new List<SceneCMDBackup>();
        }

        public class SceneCMDBackup
        {
            public StoredCMDBackup StoredCommand { get; set; }
            public int? Order { get; set; }
        }

        public override string Name
        {
            get { return "Scene"; }
        }

        public override string FileName
        {
            get { return "ScenesBackup.zvs"; }
        }

        public async override Task<ExportResult> ExportAsync(string fileName)
        {
            var CmdCount = 0;
            using (zvsContext context = new zvsContext())
            {
                var existingScenes = await context.Scenes
                    .Include(o => o.Commands)
                    .ToListAsync();

                var backupScenes = new List<SceneBackup>();
                foreach (var s in existingScenes)
                {
                    var sceneBackup = new SceneBackup();
                    sceneBackup.Name = s.Name;

                    foreach (SceneCommand scmd in s.Commands)
                    {
                        SceneCMDBackup SceneCmdBackup = new SceneCMDBackup();
                        SceneCmdBackup.Order = scmd.SortOrder;
                        SceneCmdBackup.StoredCommand = await StoredCMDBackup.ConvertToBackupCommand(scmd.StoredCommand);
                        sceneBackup.Commands.Add(SceneCmdBackup);
                        CmdCount++;
                    }
                    backupScenes.Add(sceneBackup);
                }

                var saveResult = await SaveAsXMLToDiskAsync(backupScenes, fileName);

                if (saveResult.HasError)
                    return new ExportResult(saveResult.Message, saveResult.HasError);

                return new ExportResult(string.Format("Exported {0} scenes with {1} scene commands to {2}", backupScenes.Count,
                    CmdCount,
                    Path.GetFileName(fileName)), false);
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName)
        {
            var result = await ReadAsXMLFromDiskAsync<List<SceneBackup>>(fileName);

            if (result.HasError)
                return new RestoreSettingsResult(result.Message);

            int SkippedCount = 0;
            var newScene = new List<Scene>();
            var ImportedCmdCount = 0;

            using (zvsContext context = new zvsContext())
            {
                var existingScenes = await context.Scenes.ToListAsync();
                foreach (var backupScene in result.Data)
                {
                    if (existingScenes.Any(o => o.Name == backupScene.Name))
                    {
                        SkippedCount++;
                        continue;
                    }

                    var s = new Scene();
                    s.Name = backupScene.Name;

                    foreach (var backupSceneCMD in backupScene.Commands)
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
                    newScene.Add(s);
                }

                context.Scenes.AddRange(newScene);

                if (newScene.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync();
                    if (saveResult.HasError)
                        return new RestoreSettingsResult(saveResult.Message);
                }
            }
            return new RestoreSettingsResult(string.Format("Imported {0} scenes with {1} scene commands. Skipped {2} scene from {3}", newScene.Count, ImportedCmdCount, SkippedCount, Path.GetFileName(fileName)), fileName);
        }
    }
}