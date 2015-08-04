using System;
using System.Text;

namespace LightSwitchPlugin.LightSwitch
{
    internal static class LightSwitchProtocol
    {
        internal static void DecodeIncoming(string incomingMessage, ILightSwitchChannel protocol)
        {
            if (incomingMessage.Length == 0)
                return;

            string cmd = incomingMessage.TrimEnd(Environment.NewLine.ToCharArray()).ToUpper();
            string[] commandSegments = cmd.Split('~');

            if (commandSegments[0].Equals("IPHONE"))
                protocol.OnIphone();
            else if (commandSegments[0].Equals("PASSWORD") && commandSegments.Length == 2)
                protocol.OnPassword(commandSegments[1]);
            else if (commandSegments[0].Equals("VERSION"))
                protocol.OnVersion();
            else if (commandSegments[0].Equals("SERVER"))
                protocol.OnServer();
            else if (commandSegments[0].Equals("TERMINATE"))
                protocol.OnTerminate();
            else if (commandSegments[0].Equals("ALIST"))  //DEVICES, SCENES AND ZONES.
                protocol.OnAList();
            else if (commandSegments[0].Equals("LIST")) //DEVICES
                protocol.OnList();
            else if (commandSegments[0].Equals("SLIST"))  //SCENES
                protocol.OnSList();
            else if (commandSegments[0].Equals("ZLIST")) //ZONES
                protocol.OnZList();
            else if (commandSegments[0].Equals("DEVICE") && commandSegments.Length == 4)
                protocol.OnDevice(commandSegments[1], commandSegments[2], commandSegments[3]);
            else if (commandSegments[0].Equals("SCENE") && commandSegments.Length == 2)
                protocol.OnScene(commandSegments[1]);
            else if (commandSegments[0].Equals("ZONE") && commandSegments.Length == 3)
                protocol.OnZone(commandSegments[1], commandSegments[2]);
            else if (commandSegments[0].Equals("THERMMODE") && commandSegments.Length == 4)
                protocol.OnThermMode(commandSegments[1], commandSegments[2], commandSegments[3]);
            else if (commandSegments[0].Equals("THERMTEMP") && commandSegments.Length == 5)
                protocol.OnThermTemp(commandSegments[1], commandSegments[2], commandSegments[3], commandSegments[4]);
        }

        internal static LightSwitchCommand CreateMsgCmd(string msg)
        {
            return new LightSwitchCommand($"MSG~{msg}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateErrorMsgCmd(string msg)
        {
            return new LightSwitchCommand($"ERR~{msg}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateMsgCmdFormat(string msg, params object[] parameters)
        {
            return CreateMsgCmd(string.Format(msg, parameters));
        }

        internal static LightSwitchCommand CreateInfoCmd(string msg)
        {
            return new LightSwitchCommand(msg);
        }

        internal static LightSwitchCommand CreateInfoCmdFormat(string msg, params object[] parameters)
        {
            return CreateInfoCmd(string.Format(msg, parameters));
        }

        internal static LightSwitchCommand CreateCookieCmd(string nonce)
        {
            return new LightSwitchCommand($"COOKIE~{nonce}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateVersionCmd(string appNameAndVersion)
        {
            return new LightSwitchCommand($"VER~{appNameAndVersion}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateDeviceCmd(string name, string id, string level, DeviceTypes type)
        {
            return new LightSwitchCommand($"DEVICE~{name}~{id}~{level}~{type.ToString()}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateUpdateCmd(string name, string id, string level, DeviceTypes type)
        {
            return new LightSwitchCommand($"UPDATE~{name}~{id}~{level}~{type.ToString()}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateSceneCmd(string name, string id)
        {
            return new LightSwitchCommand($"SCENE~{name}~{id}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateZoneCmd(string name, string id)
        {
            return new LightSwitchCommand($"ZONE~{name}~{id}{Environment.NewLine}");
        }

        internal static LightSwitchCommand CreateEndListCmd()
        {
            return new LightSwitchCommand($"ENDLIST{Environment.NewLine}");
        }

        public enum DeviceTypes
        {
            BinarySwitch,
            MultiLevelSwitch,
            Thermostat,
            Sensor,
            WindowCovering,
            Status
        }
    }

    public sealed class LightSwitchCommand
    {
        public string RawCommand { get; }

        public LightSwitchCommand(string rawCommand)
        {
            RawCommand = rawCommand;
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(RawCommand);
        }
    }

    
}
