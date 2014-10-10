using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace zvs.Processor.Backup
{
    public abstract class BackupRestore
    {
        public abstract string Name { get; }
        public abstract string FileName { get; }

        public abstract Task<ExportResult> ExportAsync(string fileName);

        public abstract Task<RestoreSettingsResult> ImportAsync(string fileName);

        protected async Task<Result> SaveAsXMLToDiskAsync<T>(T objCollection, string fileName) where T : IEnumerable
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stream, objCollection);
                    stream.Position = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        var output = reader.ReadToEnd();

                        // Write the string to a file.
                        using (var file = new System.IO.StreamWriter(fileName))
                            await file.WriteLineAsync(output);
                    }
                }
                return new Result();
            }
            catch (Exception e)
            {
                Debug.WriteLine("SaveSettingFileAsync Error: " + e.Message);
                return new Result(string.Format("Error saving {0}: {1}", fileName, e.Message));
            }
        }

        protected async Task<ReadAsXMLFromDiskResult<T>> ReadAsXMLFromDiskAsync<T>(string fileName)
        {
            Debug.WriteLine(string.Format("Loading {0}", typeof(T).Name));

            try
            {
                if (!File.Exists(fileName))
                    return new ReadAsXMLFromDiskResult<T>(fileName + " not found");

                //Open the file written above and read values from it. 
                //http://stackoverflow.com/questions/1127431/xmlserializer-giving-filenotfoundexception-at-constructor
                var ScenesSerializer = new XmlSerializer(typeof(T));

                string fileData;
                using (var streamReader = new StreamReader(fileName))
                    fileData = await streamReader.ReadToEndAsync();

                return new ReadAsXMLFromDiskResult<T>((T)ScenesSerializer.Deserialize(new StringReader(fileData)));
            }
            catch (Exception e)
            {
                return new ReadAsXMLFromDiskResult<T>(string.Format("Error reading {0}: {1}", fileName, e.Message));
            }
        }

        public class ExportResult : Result
        {
            public ExportResult(string message, bool hasError)
            {
                this.Message = message;
                this.HasError = hasError;
            }
        }

        public class ReadAsXMLFromDiskResult<T> : Result
        {
            public T Data { get; private set; }

            public ReadAsXMLFromDiskResult(T data)
                : base()
            {
                Data = data;
            }

            public ReadAsXMLFromDiskResult(string message)
                : base(message)
            {
            }
        }

        public class RestoreSettingsResult : Result
        {
            public string FilePath { get; private set; }

            public RestoreSettingsResult(string message, string filePath)
                : base()
            {
                Message = message;
                FilePath = filePath;
            }

            public RestoreSettingsResult(string error)
                : base(error)
            {
            }
        }
    }
}
