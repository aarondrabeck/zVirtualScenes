using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;

namespace HTTPControlPlugin
{
    public class zvsObjectCommand
    {
        public string Name;
        public string FriendlyName;
        public string HelpText;
        public int CommandId;
        public Data_Types paramType;
        public commandScopeType cmdtype;
    }
}
