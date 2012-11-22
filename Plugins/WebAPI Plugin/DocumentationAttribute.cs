namespace WebAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class Documentation : Attribute
    {
        public string Name { get; private set; }
        public double Version { get; private set; }
        public string Notes { get; private set; }
        public bool isPrivate { get; private set; }

        public Documentation(string name, double version, string notes, bool isPrivate = false)
        {
            Name = name;
            Version = version;
            Notes = notes;
            this.isPrivate = isPrivate;
        }
    }
}