//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;
//using Xunit;
//using zvs.Processor.Backup;

//namespace zvs.Processor.Tests
//{
//    public class FileIOTests
//    {

//        [Fact]
//        public async Task ReadWriteRoundTripTest()
//        {
//            SampleBackupRestore sampleBackupRestore = new SampleBackupRestore();
//            var file = Path.Combine(AssemblyDirectory, sampleBackupRestore.FileName);            
//            Assert.False((await sampleBackupRestore.ExportAsync(file)).HasError);
//            Assert.False((await sampleBackupRestore.ImportAsync(file)).HasError);
//        }

//        public class SampleBackupRestore : BackupRestore
//        {
//            private List<SampleClass> samples = new List<SampleClass>();
//            public SampleBackupRestore()
//            {
//                samples.Add(new SampleClass() { Number = 5312, Text = "Hello World.", Field = "abc123" });
//                samples.Add(new SampleClass() { Number = 4587, Text = "Testing 12345", Field = "fieldtest" });                
//            }

//            public override async Task<Result> ExportAsync(string fileName)
//            {
//                //Write to disk
//                var result = await SaveAsXMLToDiskAsync(samples, fileName);
//                Assert.False(result.HasError);

//                return new Result(result.Message, result.HasError);
//            }

//            public override async Task<BackupRestore.RestoreSettingsResult> ImportAsync(string fileName)
//            {
//                //Read to disk
//                var readResult = await ReadAsXMLFromDiskAsync<List<SampleClass>>(fileName);

//                Assert.False(readResult.HasError);
//                Assert.Equal(2, readResult.Data.Count());

//                Assert.Equal(samples[0].Number, readResult.Data[0].Number);
//                Assert.Equal(samples[0].Text, readResult.Data[0].Text);
//                Assert.Equal(samples[0].Field, readResult.Data[0].Field);

//                Assert.Equal(samples[1].Number, readResult.Data[1].Number);
//                Assert.Equal(samples[1].Text, readResult.Data[1].Text);
//                Assert.Equal(samples[1].Field, readResult.Data[1].Field);

//                return new RestoreSettingsResult("Ok", fileName);

//            }

//            public class SampleClass
//            {
//                public int Number { get; set; }
//                public string Text { get; set; }

//                public string Field = string.Empty;
//            }

//            public override string Name
//            {
//                get { return "Sample Objects"; }
//            }

//            public override string FileName
//            {
//                get { return "ReadWriteTestUnitTest.xml"; }
//            }
//        }

//        static public string AssemblyDirectory
//        {
//            get
//            {
//                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
//                UriBuilder uri = new UriBuilder(codeBase);
//                string path = Uri.UnescapeDataString(uri.Path);
//                return Path.GetDirectoryName(path);
//            }
//        }
//    }
//}
