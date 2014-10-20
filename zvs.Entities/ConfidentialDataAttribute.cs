using System;

namespace zvs.DataModel
{
    /// <summary>
    /// API's should obey this attribute and never expose these properties.
    /// </summary>
    public class ConfidentialDataAttribute : Attribute { }
    
}