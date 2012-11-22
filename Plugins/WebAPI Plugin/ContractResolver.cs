using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI
{
    /// <summary>
    /// This works because EF Code First classes inherit from the POCO class that you actually want serialized, 
    /// so if we can identify when we are looking at an EF generated class (by checking the namespace) we are 
    /// able to just serialize using the properties from the base class, and therefore only serialize the POCO 
    /// properties that we were really after in the first place.
    /// </summary>
    public class ContractResolver : DefaultContractResolver
    {
        protected override List<System.Reflection.MemberInfo> GetSerializableMembers(Type objectType)
        {
            if (objectType.Namespace != null && objectType.Namespace.StartsWith("System.Data.Entity.Dynamic"))
            {
                return base.GetSerializableMembers(objectType.BaseType);
            }

            return base.GetSerializableMembers(objectType);
        }
    }
}
