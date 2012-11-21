using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zvs.Entities
{
    /// <summary>
    /// API's should obey this attribute and never expose these properties.
    /// </summary>
    public class ConfidentialDataAttribute : Attribute { }
    
}