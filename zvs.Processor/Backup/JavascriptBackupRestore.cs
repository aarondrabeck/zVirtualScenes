using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public class JavascriptBackupRestore : BackupRestore
    {
        public class JavaScriptBackup
        {
            public string Script { get; set; }
            public string Name { get; set; }
            public string UniqueIdentifier { get; set; }
            public int ArgumentType { get; set; }
            public string Description { get; set; }
            public string CustomData1 { get; set; }
            public string CustomData2 { get; set; }
            public string Help { get; set; }
            public int? SortOrder { get; set; }
        }

        public override string Name
        {
            get { return "JavaScript Commands"; }
        }

        public override string FileName
        {
            get { return "JSCommandsBackup.zvs"; }
        }


        public async override Task<Result> ExportAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext())
            {
                var backupJs = await context.JavaScriptCommands
                    .Select(o => new JavaScriptBackup()
                    {
                        Script = o.Script,
                        Name = o.Name,
                        UniqueIdentifier = o.UniqueIdentifier,
                        ArgumentType = (int)o.ArgumentType,
                        Description = o.Description,
                        CustomData1 = o.CustomData1,
                        CustomData2 = o.CustomData2,
                        Help = o.Help,
                        SortOrder = o.SortOrder
                    })
                    .ToListAsync(cancellationToken);

                var saveResult = await SaveAsXMLToDiskAsync(backupJs, fileName);

                if (saveResult.HasError)
                    return Result.ReportError(saveResult.Message);

                return Result.ReportSuccessFormat("Exported {0} JavaScript commands to {1}", backupJs.Count,
                    Path.GetFileName(fileName));
            }
        }

        public static void ExportJavaScriptAsync(string pathFileName, Action<string> callback)
        {
            var scripts = new List<JavaScriptBackup>();
            using (var context = new ZvsContext())
            {
                foreach (var script in context.JavaScriptCommands)
                {
                    var scriptBackup = new JavaScriptBackup
                    {
                        Script = script.Script,
                        Name = script.Name,
                        UniqueIdentifier = script.UniqueIdentifier,
                        ArgumentType = (int) script.ArgumentType,
                        Description = script.Description,
                        CustomData1 = script.CustomData1,
                        CustomData2 = script.CustomData2,
                        Help = script.Help,
                        SortOrder = script.SortOrder
                    };
                    scripts.Add(scriptBackup);
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(pathFileName, FileMode.Create);
                var xmlSerializer = new XmlSerializer(typeof(List<JavaScriptBackup>));
                xmlSerializer.Serialize(stream, scripts);
                callback(string.Format("Exported {0} JavaScript commands to '{1}'", scripts.Count, Path.GetFileName(pathFileName)));
            }
            catch (Exception e)
            {
                callback("Error saving " + pathFileName + ": (" + e.Message + ")");
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName, CancellationToken cancellationToken)
        {
            var result = await ReadAsXMLFromDiskAsync<List<JavaScriptBackup>>(fileName);

            if (result.HasError)
                return RestoreSettingsResult.ReportError(result.Message);

            var skippedCount = 0;

            var newJavaScriptCommands = result.Data.Select(o => new JavaScriptCommand()
            {
                Script = o.Script,
                Name = o.Name,
                UniqueIdentifier = o.UniqueIdentifier,
                ArgumentType = (DataType)o.ArgumentType,
                Description = o.Description,
                CustomData1 = o.CustomData1,
                CustomData2 = o.CustomData2,
                Help = o.Help,
                SortOrder = o.SortOrder
            }).ToList();

            using (var context = new ZvsContext())
            {
                foreach (var duplicateCommand in (await context.JavaScriptCommands.ToListAsync(cancellationToken)).Select(existingCommand => newJavaScriptCommands.FirstOrDefault(o => o.Name == existingCommand.Name)).Where(duplicateCommand => duplicateCommand != null))
                {
                    skippedCount++;
                    newJavaScriptCommands.Remove(duplicateCommand);
                }

                context.JavaScriptCommands.AddRange(newJavaScriptCommands);

                if (newJavaScriptCommands.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync(cancellationToken);
                    if (saveResult.HasError)
                        return RestoreSettingsResult.ReportError(saveResult.Message);
                }
            }
            return RestoreSettingsResult.ReportSuccess(string.Format("Imported {0} Javascript commands, skipped {1} from {2}", newJavaScriptCommands.Count, skippedCount, Path.GetFileName(fileName)));
        }
    }
}