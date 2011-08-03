using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;

namespace zVirtualScenesApplication.UserControls
{
    public class btnInfo
    {
        public ParamType Param;
        public cmdType zCommandType;

        public btnInfo(ParamType param, cmdType zcommandType)
        {
            Param = param;
            zCommandType = zcommandType;
        }
    }
}
