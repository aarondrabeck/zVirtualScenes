using System.IO;
using System.Runtime.CompilerServices;

namespace zvs.DataModel.Tests
{
    public class UnitTestDbConnection : IEntityContextConnection
    {
        public string NameOrConnectionString { get; }

        public UnitTestDbConnection([CallerMemberName] string nameOrConnectionString = "", [CallerFilePath] string sourceFilePath = "")
        {
            var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            NameOrConnectionString = $@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog={fileName}-{nameOrConnectionString}";
        }
    }
}
