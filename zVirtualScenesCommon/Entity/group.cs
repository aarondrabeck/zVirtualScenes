using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;
using zVirtualScenesCommon.Util;


namespace zVirtualScenesCommon.Entity
{   
    public partial class group : EntityObject
    {
        public override string ToString()
        {
            return this.name;
        }
    }
}