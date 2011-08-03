using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HTTPControlPlugin
{
    public class HTTPCMDResult
    {
        public bool HadError;
        public string Description;

        [XmlIgnore]
        public bool DescriptionIsXML = false;
    }
}
