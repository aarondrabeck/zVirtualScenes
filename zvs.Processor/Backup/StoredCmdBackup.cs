using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor.Backup
{
    public enum Command_Types
    {
        Unknown = 0,
        Builtin = 1,
        Device = 2,
        DeviceType = 3,
        JavaScript = 4
    }

    [Serializable]
    public class StoredCMDBackup
    {
        public Command_Types CommandType;
        public string UniqueIdentifier;
        public string Argument;
        public string Argument2;
        public int NodeNumber;

        public static implicit operator StoredCMDBackup(StoredCommand m)
        {
            if (m == null)
                return null;

            StoredCMDBackup bcmd = new StoredCMDBackup();

            if (m.Command is BuiltinCommand)
                bcmd.CommandType = Command_Types.Builtin;
            else if (m.Command is DeviceTypeCommand)
            {
                bcmd.CommandType = Command_Types.DeviceType;
                using (zvsContext context = new zvsContext())
                {
                    int d_id = int.TryParse(m.Argument2, out d_id) ? d_id : 0;

                    //TODO: FIX BLOCKING CALL 
                    Device d = context.Devices.FirstOrDefault(o => o.Id == d_id);
                    if (d != null)
                        bcmd.NodeNumber = d.NodeNumber;
                }
            }
            else if (m.Command is DeviceCommand)
            {
                bcmd.CommandType = Command_Types.Device;
                bcmd.NodeNumber = ((DeviceCommand)m.Command).Device.NodeNumber;
            }
            else if (m.Command is JavaScriptCommand)
                bcmd.CommandType = Command_Types.JavaScript;

            if (m.Command != null)
                bcmd.UniqueIdentifier = m.Command.UniqueIdentifier;

            bcmd.Argument = m.Argument;

            return bcmd;
        }

        /// <summary>
        /// Return null if failed to restore command.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="backupStoredCMD"></param>
        /// <returns></returns>
        public async static Task<StoredCommand> RestoreStoredCommandAsync(zvsContext context, StoredCMDBackup backupStoredCMD)
        {
            if (backupStoredCMD == null)
                return null;

            if (backupStoredCMD.CommandType == Command_Types.Device ||
                backupStoredCMD.CommandType == Command_Types.DeviceType)
            {
                StoredCommand sc = new StoredCommand();

                var device = await context.Devices
                    .Include(o => o.Commands)
                    .Include(o => o.Type.Commands)
                    .FirstOrDefaultAsync(o => o.NodeNumber == backupStoredCMD.NodeNumber);
                if (device == null)
                    return null;

                Command c = null;
                if (backupStoredCMD.CommandType == Command_Types.Device)
                {
                    c = device.Commands.FirstOrDefault(o => o.UniqueIdentifier == backupStoredCMD.UniqueIdentifier);
                }
                if (backupStoredCMD.CommandType == Command_Types.DeviceType)
                {
                    c = device.Type.Commands.FirstOrDefault(o => o.UniqueIdentifier == backupStoredCMD.UniqueIdentifier);
                    sc.Argument2 = device.Id.ToString();
                }

                if (c == null)
                    return null;

                sc.Argument = backupStoredCMD.Argument;
                sc.Command = c;
                context.StoredCommands.Add(sc);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    return null;

                return sc;
            }
            else if (backupStoredCMD.CommandType == Command_Types.Builtin ||
                     backupStoredCMD.CommandType == Command_Types.JavaScript)
            {
                Command c = null;
                if (backupStoredCMD.CommandType == Command_Types.Builtin)
                    c = await context.BuiltinCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCMD.UniqueIdentifier);

                if (backupStoredCMD.CommandType == Command_Types.JavaScript)
                    c = await context.JavaScriptCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == backupStoredCMD.UniqueIdentifier);

                if (c == null)
                    return null;

                StoredCommand sc = new StoredCommand();
                sc.Argument = backupStoredCMD.Argument;
                sc.Command = c;
                context.StoredCommands.Add(sc);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    return null;

                return sc;
            }

            return null;
        }
    }
}
