using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public class SceneBackupRestore : BackupRestore
    {
        public class SceneBackup
        {
            public string Name { get; set; }
            public readonly List<SceneCmdBackup> Commands = new List<SceneCmdBackup>();
        }

        public class SceneCmdBackup
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

        public async override Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken)
        {
            var cmdCount = 0;
            using (var context = new ZvsContext())
            {
                var existingScenes = await context.Scenes
                    .Include(o => o.Commands)
                    .ToListAsync(cancellationToken);

                var backupScenes = new List<SceneBackup>();
                foreach (var s in existingScenes)
                {
                    var sceneBackup = new SceneBackup {Name = s.Name};

                    foreach (var scmd in s.Commands)
                    {
                        var sceneCmdBackup = new SceneCmdBackup
                        {
                            Order = scmd.SortOrder,
                            StoredCommand = await StoredCMDBackup.ConvertToBackupCommand(scmd.StoredCommand)
                        };
                        sceneBackup.Commands.Add(sceneCmdBackup);
                        cmdCount++;
                    }
                    backupScenes.Add(sceneBackup);
                }

                var saveResult = await SaveAsXMLToDiskAsync(backupScenes, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportSuccessFormat("Exported {0} scenes with {1} scene commands to {2}",
                    backupScenes.Count,
                    cmdCount,
                    Path.GetFileName(fileName));
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXMLFromDiskAsync<List<SceneBackup>>(fileName);

            if (result.HasError)
                return RestoreSettingsResult.ReportError(result.Message);

            var skippedCount = 0;
            var newScene = new List<Scene>();
            var importedCmdCount = 0;

            using (var context = new ZvsContext())
            {
                var existingScenes = await context.Scenes.ToListAsync(cancellationToken);
                foreach (var backupScene in result.Data)
                {
                    if (existingScenes.Any(o => o.Name == backupScene.Name))
                    {
                        skippedCount++;
                        continue;
                    }

                    var s = new Scene();
                    s.Name = backupScene.Name;

                    foreach (var backupSceneCmd in backupScene.Commands)
                    {
                        var sc = await StoredCMDBackup.RestoreStoredCommandAsync(context, backupSceneCmd.StoredCommand,cancellationToken);
                        if (sc != null)
                        {
                            s.Commands.Add(new SceneCommand()
                            {
                                StoredCommand = sc,
                                SortOrder = backupSceneCmd.Order
                            });
                            importedCmdCount++;
                        }
                    }
                    newScene.Add(s);
                }

                context.Scenes.AddRange(newScene);

                if (newScene.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return RestoreSettingsResult.ReportError(saveResult.Message);
                }
            }
            return RestoreSettingsResult.ReportSuccess(string.Format("Imported {0} scenes with {1} scene commands. Skipped {2} scene from {3}", newScene.Count, importedCmdCount, skippedCount, Path.GetFileName(fileName)));
        }
    }
}