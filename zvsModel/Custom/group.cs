using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class group
    {
        public override string ToString()
        {
            return this.name;
        }

        public static event onContextUpdatedEventHandler onContextUpdated;
        public static void CallOnContextUpdated()
        {
            if (onContextUpdated != null)
                onContextUpdated(null, new EventArgs());
        }
    }
}