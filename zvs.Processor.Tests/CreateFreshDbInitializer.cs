using System.Data.Entity;
using zvs.DataModel;

namespace zvs.Processor.Tests
{
    public class CreateFreshDbInitializer : DropCreateDatabaseAlways<ZvsContext>
    { }
}
