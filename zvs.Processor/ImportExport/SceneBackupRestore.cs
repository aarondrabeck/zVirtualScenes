using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor.ImportExport
{
    public class SceneBackupRestore : BackupRestore
    {
        public SceneBackupRestore(IEntityContextConnection entityContextConnection) : base(entityContextConnection) { }

        public class SceneBackup
        {
            public string Name { get; set; }
            public readonly List<SceneCMDBackup> Commands = new List<SceneCMDBackup>();
        }

        // ReSharper disable once InconsistentNaming
        public class SceneCMDBackup
        {
            public StoredCmdBackup StoredCommand { get; set; }
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
            using (var context = new ZvsContext(EntityContextConnection))
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
                        var sceneCmdBackup = new SceneCMDBackup
                        {
                            Order = scmd.SortOrder,
                            StoredCommand = await StoredCmdBackup.ConvertToBackupCommand(scmd)
                        };
                        sceneBackup.Commands.Add(sceneCmdBackup);
                        cmdCount++;
                    }
                    backupScenes.Add(sceneBackup);
                }

                var saveResult = await SaveAsXmlToDiskAsync(backupScenes, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportSuccessFormat("Exported {0} scenes with {1} scene commands to {2}",
                    backupScenes.Count,
                    cmdCount,
                    Path.GetFileName(fileName));
            }
        }

        public async override Task<Result> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXmlFromDiskAsync<List<SceneBackup>>(fileName);

            if (result.HasError)
                return Result.ReportError(result.Message);

            var skippedCount = 0;
            var newScene = new List<Scene>();
            var importedCmdCount = 0;

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingScenes = await context.Scenes.ToListAsync(cancellationToken);
                foreach (var backupScene in result.Data)
                {
                    if (existingScenes.Any(o => o.Name == backupScene.Name))
                    {
                        skippedCount++;
                        continue;
                    }

                    var s = new Scene {Name = backupScene.Name};

                    foreach (var backupSceneCmd in backupScene.Commands)
                    {
                        var sc = await StoredCmdBackup.RestoreStoredCommandAsync(context, backupSceneCmd.StoredCommand,cancellationToken);
                        if (sc == null) continue;
                        s.Commands.Add(new SceneStoredCommand
                        {
                            Argument = sc.Argument,
                            Argument2 = sc.Argument2,
                            CommandId = sc.CommandId,
                            SortOrder = backupSceneCmd.Order
                        });
                        importedCmdCount++;
                    }
                    newScene.Add(s);
                }

                context.Scenes.AddRange(newScene);

                if (newScene.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return Result.ReportError(saveResult.Message);
                }
            }
            return Result.ReportSuccess(string.Format("Imported {0} scenes with {1} scene commands. Skipped {2} scene from {3}", newScene.Count, importedCmdCount, skippedCount, Path.GetFileName(fileName)));
        }
    }
}