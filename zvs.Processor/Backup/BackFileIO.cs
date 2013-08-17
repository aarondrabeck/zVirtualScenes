using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace zvs.Processor.Backup
{
    public class BackFileIO
    {
        public static async Task<Result> SaveAsXMLToDiskAsync<T>(IEnumerable<T> collection, string fileName)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stream, collection);
                    stream.Position = 0;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var output = reader.ReadToEnd();

                        // Write the string to a file.
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
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

        public static async Task<ReadAsXMLFromDiskResult<T>> ReadAsXMLFromDiskAsync<T>(string fileName)
        {
            Debug.WriteLine(string.Format("Loading {0}", typeof(T).Name));

            try
            {
                if (!File.Exists(fileName))
                    return new ReadAsXMLFromDiskResult<T>(fileName + " not found");

                //Open the file written above and read values from it. 
                //http://stackoverflow.com/questions/1127431/xmlserializer-giving-filenotfoundexception-at-constructor
                XmlSerializer ScenesSerializer = new XmlSerializer(typeof(T));

                string fileData;
                using (StreamReader streamReader = new StreamReader(fileName))
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
    }
}
