namespace WebAPI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using zvs.Entities;

    public class Documentation : Attribute
    {
        public string Name { get; private set; }
        public double Version { get; private set; }
        public string Notes { get; private set; }
        public bool isPrivate { get; private set; }
        public Type PocoType { get; private set; }

        public Documentation(Type pocoType, string name, double version, string notes, bool isPrivate = false)
        {
            PocoType = pocoType;
            Name = name;
            Version = version;
            Notes = notes;
            this.isPrivate = isPrivate;
        }

        public List<PropertyDetails> PocoProperties
        {
            get
            {
                if (PocoType == null)
                    return new List<PropertyDetails>();

                var allProperties = PocoType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !Attribute.IsDefined(p, typeof(ConfidentialDataAttribute)))
                    .ToList();

                var details = new List<PropertyDetails>();

                foreach (var Property in allProperties)
                {
                    var additionalDetails = new List<string>();

                    if (!Property.CanRead && Property.CanWrite)
                        additionalDetails.Add("Write-only. No getter.");
                    else if (Property.CanRead && !Property.CanWrite)
                        additionalDetails.Add("Read-only. No setter.");
                    else if (!Property.CanRead && !Property.CanWrite)
                        additionalDetails.Add("Cannot read or write.");

                    if (Attribute.IsDefined(Property, typeof(RequiredAttribute)))
                        additionalDetails.Add("Required.");

                    if (Attribute.IsDefined(Property, typeof(StringLengthAttribute)))
                    {
                        var strLenAttr = Property.GetCustomAttributes(typeof(StringLengthAttribute), false).Cast<StringLengthAttribute>().Single();
                        additionalDetails.Add(string.Format("Maximum Length {0}.", strLenAttr.MaximumLength));
                    }

                    details.Add(new PropertyDetails(Property.PropertyType.GetFullName(), Property.Name, additionalDetails));
                }

                return details;
            }
        }

        public class PropertyDetails
        {
            public string Type { get; private set; }
            public string Name { get; private set; }
            public List<string> AdditionalDetails { get; private set; }

            public PropertyDetails(string type, string name, List<string> additionalDetails)
            {
                Type = type;
                Name = name;
                AdditionalDetails = additionalDetails;
            }
        }
    }

    public static class Extensions
    {
        public static string GetFullName(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            StringBuilder sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append(t.GetGenericArguments().Aggregate("<",

                delegate(string aggregate, Type type)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(type);
                }
                ));
            sb.Append(">");

            return sb.ToString();
        }
    }
}