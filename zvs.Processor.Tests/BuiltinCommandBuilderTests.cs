using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class BuiltinCommandBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new BuiltinCommandBuilder(null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            //act
            var bcb = new BuiltinCommandBuilder(dbConnection);
            //assert 
            Assert.IsNotNull(bcb);
        }

        [TestMethod]
        public async Task RegisterAsyncNullBuiltinCommandTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            var bcb = new BuiltinCommandBuilder(dbConnection);

            //act
            var result = await bcb.RegisterAsync(null, CancellationToken.None);

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsTrue(result.HasError, result.Message);
        }

        [TestMethod]
        public async Task RegisterAsyncNewTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var bcb = new BuiltinCommandBuilder(dbConnection);

            var builtinCommand = new BuiltinCommand
            {
                Name = "Unit Test Builtin Command",
                UniqueIdentifier = "BUILTIN_COMMAND1"
            };

            //act
            var result = await bcb.RegisterAsync(builtinCommand, CancellationToken.None);

            BuiltinCommand setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.BuiltinCommands.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new builtin command setting saved to DB");
        }

        [TestMethod]
        public async Task RegisterAsyncUpdateTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var bcb = new BuiltinCommandBuilder(dbConnection);

            var builtinCommand = new BuiltinCommand
            {
                Name = "Unit Test Builtin Command",
                UniqueIdentifier = "BUILTIN_COMMAND1"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.BuiltinCommands.Add(builtinCommand);
                await context.SaveChangesAsync();
            }

            builtinCommand.Name = "New Name";

            //act
            var result = await bcb.RegisterAsync(builtinCommand, CancellationToken.None);

            BuiltinCommand setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.BuiltinCommands.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new builtin command setting saved to DB");
            Assert.IsTrue(setting.Name == "New Name");
        }

        [TestMethod]
        public async Task RegisterAsyncOptionAddTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var bcb = new BuiltinCommandBuilder(dbConnection);

            var builtinCommand = new BuiltinCommand
            {
                Name = "Unit Test Builtin Command",
                UniqueIdentifier = "BUILTIN_COMMAND1"
            };

            var option1 = new CommandOption
            {
                Name = "Option 1"
            };
            var option2 = new CommandOption
            {
                Name = "Option 2"
            };
            builtinCommand.Options.Add(option1);

            using (var context = new ZvsContext(dbConnection))
            {
                context.BuiltinCommands.Add(builtinCommand);
                await context.SaveChangesAsync();
            }

            builtinCommand.Options.Add(option2);

            //act
            var result = await bcb.RegisterAsync(builtinCommand, CancellationToken.None);

            BuiltinCommand setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.BuiltinCommands
                        .Include(o => o.Options)
                        .FirstOrDefaultAsync(o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new builtin command setting saved to DB");
            Assert.IsTrue(setting.Options.Count == 2, "Expected 2 options and got " + setting.Options.Count);
            Assert.IsTrue(setting.Options[1].Name == option2.Name, "");
        }

        [TestMethod]
        public async Task RegisterAsyncOptionRemoveTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var bcb = new BuiltinCommandBuilder(dbConnection);

            var builtinCommand = new BuiltinCommand
            {
                Name = "Unit Test Builtin Command",
                UniqueIdentifier = "BUILTIN_COMMAND1",

            };

            var option1 = new CommandOption
            {
                Name = "Option 1"
            };
            var option2 = new CommandOption
            {
                Name = "Option 2"
            };
            builtinCommand.Options.Add(option1);
            builtinCommand.Options.Add(option2);

            using (var context = new ZvsContext(dbConnection))
            {
                context.BuiltinCommands.Add(builtinCommand);
                await context.SaveChangesAsync();
            }

            builtinCommand.Options.Remove(option2);

            //act
            var result = await bcb.RegisterAsync(builtinCommand, CancellationToken.None);

            BuiltinCommand setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.BuiltinCommands
                        .Include(o => o.Options)
                        .FirstOrDefaultAsync(o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new builtin command setting saved to DB");
            Assert.IsTrue(setting.Options.Count == 1, "Expected 1 option!");
            Assert.IsTrue(setting.Options[0].Name == option1.Name);
        }

        [TestMethod]
        public async Task RegisterAsyncNoUpdateTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var bcb = new BuiltinCommandBuilder(dbConnection);

            var builtinCommand = new BuiltinCommand
            {
                Name = "Unit Test Builtin Command",
                UniqueIdentifier = "BUILTIN_COMMAND1"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.BuiltinCommands.Add(builtinCommand);
                await context.SaveChangesAsync();
            }

            //act
            var result = await bcb.RegisterAsync(builtinCommand, CancellationToken.None);

            BuiltinCommand setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.BuiltinCommands.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new builtin command setting saved to DB");
        }
    }
}
