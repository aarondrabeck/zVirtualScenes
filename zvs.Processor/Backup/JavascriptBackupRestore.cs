using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.Entities;
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


        public async override Task<ExportResult> ExportAsync(string fileName)
        {
            using (zvsContext context = new zvsContext())
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
                    .ToListAsync();

                var saveResult = await SaveAsXMLToDiskAsync(backupJs, fileName);

                if (saveResult.HasError)
                    return new ExportResult(saveResult.Message, saveResult.HasError);

                return new ExportResult(string.Format("Exported {0} JavaScript commands to {1}", backupJs.Count,
                    Path.GetFileName(fileName)), false);
            }
        }

        public static void ExportJavaScriptAsync(string PathFileName, Action<string> Callback)
        {
            List<JavaScriptBackup> scripts = new List<JavaScriptBackup>();
            using (zvsContext context = new zvsContext())
            {
                foreach (JavaScriptCommand script in context.JavaScriptCommands)
                {
                    JavaScriptBackup scriptBackup = new JavaScriptBackup();
                    scriptBackup.Script = script.Script;
                    scriptBackup.Name = script.Name;
                    scriptBackup.UniqueIdentifier = script.UniqueIdentifier;
                    scriptBackup.ArgumentType = (int)script.ArgumentType;
                    scriptBackup.Description = script.Description;
                    scriptBackup.CustomData1 = script.CustomData1;
                    scriptBackup.CustomData2 = script.CustomData2;
                    scriptBackup.Help = script.Help;
                    scriptBackup.SortOrder = script.SortOrder;
                    scripts.Add(scriptBackup);
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<JavaScriptBackup>));
                xmlSerializer.Serialize(stream, scripts);
                Callback(string.Format("Exported {0} JavaScript commands to '{1}'", scripts.Count, Path.GetFileName(PathFileName)));
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

        public async override Task<RestoreSettingsResult> ImportAsync(string fileName)
        {
            var result = await ReadAsXMLFromDiskAsync<List<JavaScriptBackup>>(fileName);

            if (result.HasError)
                return new RestoreSettingsResult(result.Message);

            int SkippedCount = 0;

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

            using (zvsContext context = new zvsContext())
            {
                foreach (var existingCommand in await context.JavaScriptCommands.ToListAsync())
                {
                    var duplicateCommand = newJavaScriptCommands.FirstOrDefault(o => o.Name == existingCommand.Name);
                    if (duplicateCommand != null)
                    {
                        SkippedCount++;
                        newJavaScriptCommands.Remove(duplicateCommand);
                        continue;
                    }
                }

                context.JavaScriptCommands.AddRange(newJavaScriptCommands);

                if (newJavaScriptCommands.Count > 0)
                {
                    var saveResult = await context.TrySaveChangesAsync();
                    if (saveResult.HasError)
                        return new RestoreSettingsResult(saveResult.Message);
                }
            }
            return new RestoreSettingsResult(string.Format("Imported {0} Javascript commands, skipped {1} from {2}", newJavaScriptCommands.Count, SkippedCount, Path.GetFileName(fileName)), fileName);
        }
    }
}