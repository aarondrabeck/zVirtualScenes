using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public abstract class Builder
    {
        protected Core Core { get; set; }
        protected zvsAdapter Adapter { get; set; }

        public Builder(zvsAdapter zvsAdapter, Core core)
        {
            Adapter = zvsAdapter;
            Core = core;
        }
    }
}
