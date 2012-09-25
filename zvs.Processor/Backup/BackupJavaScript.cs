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
        public class JavaScriptBackup
        {
            public string Script;
            public string Name;
            public string UniqueIdentifier;
            public int ArgumentType;
            public string Description;
            public string CustomData1;
            public string CustomData2;
            public string Help;
            public int? SortOrder;
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
                stream.Close();
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

        public static void ImportJavaScriptAsync(string PathFileName, Action<string> Callback)
        {
            List<JavaScriptBackup> scripts = new List<JavaScriptBackup>();
            int ImportedCount = 0;

            FileStream myFileStream = null;
            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<JavaScriptBackup>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    scripts = (List<JavaScriptBackup>)ScenesSerializer.Deserialize(myFileStream);
                   
                    using (zvsContext context = new zvsContext())
                    {
                        foreach (JavaScriptBackup jsBackup in scripts)
                        {
                            JavaScriptCommand jsCommand = new JavaScriptCommand();
                            jsCommand.Script = jsBackup.Script;
                            jsCommand.Name = jsBackup.Name;
                            jsCommand.UniqueIdentifier = jsBackup.UniqueIdentifier;
                            jsCommand.ArgumentType = (DataType)jsBackup.ArgumentType;
                            jsCommand.Description = jsBackup.Description;
                            jsCommand.CustomData1 = jsBackup.CustomData1;
                            jsCommand.CustomData2 = jsBackup.CustomData2;
                            jsCommand.Help = jsBackup.Help;
                            jsCommand.SortOrder = jsBackup.SortOrder;
                            context.JavaScriptCommands.Add(jsCommand);
                            ImportedCount++; 
                            
                        }
                        context.SaveChanges();
                    }
                    Callback(string.Format("Imported {0} JavaScript commands from '{1}'", ImportedCount, Path.GetFileName(PathFileName)));
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