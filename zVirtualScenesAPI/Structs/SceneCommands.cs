using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace zVirtualScenesAPI.Structs
{
    public class SceneCommands : QuedCommand
    {
        public int order { get; set; }

        public string ObjectName()
        {
            Command cmd = API.Commands.GetCommand(CommandId, cmdtype);

            if (cmd != null)
            {
                switch (cmdtype)
                {
                    case cmdType.Builtin:
                        return cmd.FriendlyName;
                    case cmdType.Object:
                    case cmdType.ObjectType:
                        return API.Object.GetObjectName(ObjectId);
                }
            }
            return "Unknown Object";
        }

        public string CMDName()
        {
            string command_friendly_name = string.Empty;
            Command cmd = API.Commands.GetCommand(CommandId, cmdtype);

            if (cmd != null)
            {
                switch (cmdtype)
                {
                    case cmdType.Builtin:
                        switch (cmd.Name)
                        {
                            case "REPOLL_ME":
                                int id = 0;
                                int.TryParse(Argument, out id);
                                return API.Object.GetObjectName(id);
                            default:
                                return Argument; 
                        }                                               
                    default:
                        switch (cmd.paramType)
                        {
                            case ParamType.NONE:
                                command_friendly_name = cmd.FriendlyName + ".";
                                break;
                            case ParamType.SHORT:
                            case ParamType.STRING:
                            case ParamType.LIST:
                            case ParamType.INTEGER:
                            case ParamType.DECIMAL:
                            case ParamType.BYTE:
                            case ParamType.BOOL:
                                command_friendly_name = string.Format("{0} to '{1}'.", cmd.FriendlyName, Argument);
                                break;
                        }
                        break;
                }
            }
           

            if (string.IsNullOrEmpty(command_friendly_name))
                return "Unknown";
            else
                return command_friendly_name;           
        }

        public string DeviceIcon()
        {
            string Type = API.Object.GetObjectType(ObjectId);

            if (Type != null)
            {
                if (Type.Equals("THERMOSTAT"))
                    return "20zwave-thermostat.png";
                else if (Type.Equals("DIMMER"))
                    return "20bulb.png";
                else if (Type.Equals("SWITCH"))
                    return "20switch.png";
                else if (Type.Equals("CONTROLLER"))
                    return "controler320.png";
                else
                    return "20radio2.png";
            }

            return string.Empty;

        }

        public int Run()
        {
            return API.Commands.InstallQueCommand(new QuedCommand { Argument = Argument, CommandId = CommandId, cmdtype = cmdtype, ObjectId = ObjectId });
        }

    }
}
