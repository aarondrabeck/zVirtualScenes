using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor.ImportExport
{
    public enum CommandTypes
    {
        Unknown = 0,
        Builtin = 1,
        Device = 2,
        DeviceType = 3,
        JavaScript = 4
    }

    public class StoredCmdBackup
    {
        public CommandTypes CommandType;
        public string UniqueIdentifier;
        public string Argument;
        public string Argument2;
        public int NodeNumber;

        public async static Task<StoredCmdBackup> ConvertToBackupCommand(IStoredCommand m)
        {
            if (m == null)
                return null;

            var bcmd = new StoredCmdBackup();

            if (m.Command is BuiltinCommand)
                bcmd.CommandType = CommandTypes.Builtin;
            else if (m.Command is DeviceTypeCommand)
            {
                bcmd.CommandType = CommandTypes.DeviceType;
                using (var context = new ZvsContext())
                {
                    int dId = int.TryParse(m.Argument2, out dId) ? dId : 0;

                    var d = await context.Devices.FirstOrDefaultAsync(o => o.Id == dId);
                    if (d != null)
                        bcmd.NodeNumber = d.NodeNumber;
                }
            }
            else
            {
                var command = m.Command as DeviceCommand;
                if (command != null)
                {
                    bcmd.CommandType = CommandTypes.Device;
                    bcmd.NodeNumber = command.Device.NodeNumber;
                }
                else if (m.Command is JavaScriptCommand)
                    bcmd.CommandType = CommandTypes.JavaScript;
            }

            if (m.Command != null)
                bcmd.UniqueIdentifier = m.Command.UniqueIdentifier;

            bcmd.Argument = m.Argument;

            return bcmd;
        }

        /// <summary>
        /// Return null if failed to restore command.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="backupStoredCmd"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async static Task<IStoredCommand> RestoreStoredCommandAsync(ZvsContext context, StoredCmdBackup backupStoredCmd, CancellationToken cancellationToken)
        {
            if (backupStoredCmd == null)
                return null;

            switch (backupStoredCmd.CommandType)
            {
                case CommandTypes.Device:
                case CommandTypes.DeviceType:
                    {
                        var storedCommand = new StoredCommand();
                        Command c = null;

                        if (backupStoredCmd.CommandType == CommandTypes.Device)
                        {
                            c = await context.DeviceCommands
                                .FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier &&
                                o.Device.NodeNumber == backupStoredCmd.NodeNumber, cancellationToken);
                        }

                        if (backupStoredCmd.CommandType == CommandTypes.DeviceType)
                        {
                            c = await context.DeviceTypeCommands
                                .FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier &&
                                o.DeviceType.Devices.Any(p => p.NodeNumber == backupStoredCmd.NodeNumber), cancellationToken);

                            var dId = await context.Devices
                                .Where(o => o.NodeNumber == backupStoredCmd.NodeNumber)
                                .Select(o => o.Id)
                                .FirstOrDefaultAsync(cancellationToken);

                            storedCommand.Argument2 = dId.ToString();
                        }

                        if (c == null)
                            return null;

                        storedCommand.Argument = backupStoredCmd.Argument;
                        storedCommand.CommandId = c.Id;
                        return storedCommand;
                    }
                case CommandTypes.Builtin:
                case CommandTypes.JavaScript:
                    {
                        Command c = null;
                        if (backupStoredCmd.CommandType == CommandTypes.Builtin)
                            c = await context.BuiltinCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier, cancellationToken);

                        if (backupStoredCmd.CommandType == CommandTypes.JavaScript)
                            c = await context.JavaScriptCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier, cancellationToken);

                        if (c == null)
                            return null;

                        var sc = new StoredCommand {Argument = backupStoredCmd.Argument, CommandId = c.Id};
                        return sc;
                    }
            }

            return null;
        }

        public class StoredCommand : IStoredCommand
        {
            public int? CommandId { get; set; }
            public Command Command { get; set; }
            public string Argument { get; set; }
            public string Argument2 { get; set; }
            public string TargetObjectName { get; set; }
            public string Description { get; set; }
        }
    }
}
