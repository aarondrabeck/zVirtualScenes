//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using zvs.DataModel;
//using System.Data.Entity;

//namespace zvs.Processor.Backup
//{
//    public enum Command_Types
//    {
//        Unknown = 0,
//        Builtin = 1,
//        Device = 2,
//        DeviceType = 3,
//        JavaScript = 4
//    }

//    public class StoredCMDBackup
//    {
//        public Command_Types CommandType;
//        public string UniqueIdentifier;
//        public string Argument;
//        public string Argument2;
//        public int NodeNumber;

//        public async static Task<StoredCMDBackup> ConvertToBackupCommand(StoredCommand m)
//        {
//            if (m == null)
//                return null;

//            var bcmd = new StoredCMDBackup();

//            if (m.Command is BuiltinCommand)
//                bcmd.CommandType = Command_Types.Builtin;
//            else if (m.Command is DeviceTypeCommand)
//            {
//                bcmd.CommandType = Command_Types.DeviceType;
//                using (var context = new ZvsContext())
//                {
//                    int d_id = int.TryParse(m.Argument2, out d_id) ? d_id : 0;

//                    var d = await context.Devices.FirstOrDefaultAsync(o => o.Id == d_id);
//                    if (d != null)
//                        bcmd.NodeNumber = d.NodeNumber;
//                }
//            }
//            else if (m.Command is DeviceCommand)
//            {
//                bcmd.CommandType = Command_Types.Device;
//                bcmd.NodeNumber = ((DeviceCommand)m.Command).Device.NodeNumber;
//            }
//            else if (m.Command is JavaScriptCommand)
//                bcmd.CommandType = Command_Types.JavaScript;

//            if (m.Command != null)
//                bcmd.UniqueIdentifier = m.Command.UniqueIdentifier;

//            bcmd.Argument = m.Argument;

//            return bcmd;
//        }

//        /// <summary>
//        /// Return null if failed to restore command.
//        /// </summary>
//        /// <param name="context"></param>
//        /// <param name="backupStoredCmd"></param>
//        /// <returns></returns>
//        public async static Task<StoredCommand> RestoreStoredCommandAsync(ZvsContext context, StoredCMDBackup backupStoredCmd, CancellationToken cancellationToken)
//        {
//            if (backupStoredCmd == null)
//                return null;

//            if (backupStoredCmd.CommandType == Command_Types.Device ||
//                backupStoredCmd.CommandType == Command_Types.DeviceType)
//            {
//                var sc = new StoredCommand();

//                var device = await context.Devices
//                    .Include(o => o.Commands)
//                    .Include(o => o.Type.Commands)
//                    .FirstOrDefaultAsync(o => o.NodeNumber == backupStoredCmd.NodeNumber, cancellationToken);

//                if (device == null)
//                    return null;

//                Command c = null;
//                if (backupStoredCmd.CommandType == Command_Types.Device)
//                {
//                    c = device.Commands.FirstOrDefault(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier);
//                }
//                if (backupStoredCmd.CommandType == Command_Types.DeviceType)
//                {
//                    c = device.Type.Commands.FirstOrDefault(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier);
//                    sc.Argument2 = device.Id.ToString();
//                }

//                if (c == null)
//                    return null;

//                sc.Argument = backupStoredCmd.Argument;
//                sc.Command = c;
//                context.StoredCommands.Add(sc);

//                var result = await context.TrySaveChangesAsync(cancellationToken);
//                if (result.HasError)
//                    return null;

//                return sc;
//            }
//            else if (backupStoredCmd.CommandType == Command_Types.Builtin ||
//                     backupStoredCmd.CommandType == Command_Types.JavaScript)
//            {
//                Command c = null;
//                if (backupStoredCmd.CommandType == Command_Types.Builtin)
//                    c = await context.BuiltinCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier, cancellationToken);

//                if (backupStoredCmd.CommandType == Command_Types.JavaScript)
//                    c = await context.JavaScriptCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCmd.UniqueIdentifier, cancellationToken);

//                if (c == null)
//                    return null;

//                var sc = new StoredCommand();
//                sc.Argument = backupStoredCmd.Argument;
//                sc.Command = c;
//                context.StoredCommands.Add(sc);

//                var result = await context.TrySaveChangesAsync(cancellationToken);
//                if (result.HasError)
//                    return null;

//                return sc;
//            }

//            return null;
//        }
//    }
//}
