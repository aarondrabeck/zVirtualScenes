using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Processor
{
    internal class NativeMethods
    {
        public enum INSTALLSTATE
        {
            INSTALLSTATE_NOTUSED = -7,  // component disabled
            INSTALLSTATE_BADCONFIG = -6,  // configuration datacorrupt
            INSTALLSTATE_INCOMPLETE = -5,  // installationsuspended or in progress
            INSTALLSTATE_SOURCEABSENT = -4,  // run from source,source is unavailable
            INSTALLSTATE_MOREDATA = -3,  // return bufferoverflow
            INSTALLSTATE_INVALIDARG = -2,  // invalid functionargument
            INSTALLSTATE_UNKNOWN = -1,  // unrecognized productor feature
            INSTALLSTATE_BROKEN = 0,  // broken
            INSTALLSTATE_ADVERTISED = 1,  // advertised feature
            INSTALLSTATE_REMOVED = 1,  // component being removed(action state, not settable)
            INSTALLSTATE_ABSENT = 2,  // uninstalled (or actionstate absent but clients remain)
            INSTALLSTATE_LOCAL = 3,  // installed on local drive
            INSTALLSTATE_SOURCE = 4,  // run from source, CD ornet
            INSTALLSTATE_DEFAULT = 5,  // use default, local orsource
        }

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern INSTALLSTATE MsiQueryProductState(string product);

    }
}
