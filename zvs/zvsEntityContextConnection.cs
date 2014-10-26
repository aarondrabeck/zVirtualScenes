using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs
{
    public class zvsEntityContextConnection : IEntityContextConnection
    {
        public string NameOrConnectionString
        {
            get { return "zvsDBEFCF7"; }
        }
    }
}
