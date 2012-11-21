using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;

namespace WebAPI.DTO
{
    public class DTOFactory<T> where T : IIdentity
    {
        private T Entity;

        public DTOFactory(T entity)
        {
            Entity = entity;
        }

        public virtual object getDTO()
        {
            if (Entity == null)
                return null;

            var EntityType = Entity.GetTypeEntityWrapperDetection();

            var dtoDictionary = new Dictionary<string, object>();

            dtoDictionary.Add("Id", Entity.Id);

            foreach (PropertyInfo pi in EntityType.GetProperties())
            {
                //Hide the ConfidentialData tagged properties
                if (Attribute.IsDefined(pi, typeof(ConfidentialDataAttribute)))
                    continue;

                //Skip Id, it was already taken care of...BTW ID is virtual somehow...entity changes it?
                if (pi.Name.ToLower().Equals("id"))
                    continue;

                //If the properties are not virtual and a built-in, display them!
                if (!pi.GetMethod.IsVirtual && pi.PropertyType.IsBulitin())
                {
                    dtoDictionary.Add(pi.Name, pi.GetValue(Entity));
                }
                //Virtual and custom complex properties need some special attention... 
                else
                {
                    #region Print the URI to virtual collections
                    //types could be mixed here so we wont print to user
                    ObservableCollection<object> oc = new ObservableCollection<object>();
                    if (typeof(ICollection).IsAssignableFrom(pi.PropertyType))
                    {
                        var innerDictionary = new Dictionary<string, object>();

                        //_uri
                        innerDictionary.Add("_uri", string.Format("/{0}/{1}/{2}/{3}",
                            EntityType.NamespaceLastSegment(),
                            EntityType.NamePlural(),
                            Entity.Id,
                            pi.Name));

                        #region Counts....
                        //TODO: FINISH COUNT..right now it is not doing an SQL count, it is retuning set in memory and doing count in memory 
                        //TODO: Check generic type for typeof(IExirerable).IsAssignableFrom(GenericType)

                        //var PropertyGenericTypeArg = pi.PropertyType.GenericTypeArguments.FirstOrDefault();
                        ////_active count
                        //var Count = typeof(Enumerable)
                        //.GetMethods()
                        //.Where(o => o.Name == "Count" &&
                        // o.IsGenericMethod &&
                        // o.GetParameters().Count() == 1)
                        //.SingleOrDefault()
                        //.MakeGenericMethod(new Type[] { PropertyGenericTypeArg })
                        //.Invoke(null, new object[] { pi.GetValue(Entity) });  //Count is an extension method
                        //innerDictionary.Add("_active", Count);
                        #endregion

                        dtoDictionary.Add(pi.Name, innerDictionary);
                        continue;
                    }
                    #endregion

                    #region Print URI and types of complex types
                    if (typeof(IIdentity).IsAssignableFrom(pi.PropertyType))
                    {
                        IIdentity NestedComplexObject = (IIdentity)pi.GetValue(Entity);

                        if (NestedComplexObject == null)
                        {
                            dtoDictionary.Add(pi.Name, null);
                        }
                        else
                        {
                            //Print the URI to virtual objects
                            var objectDictionary = new Dictionary<string, object>();
                            objectDictionary.Add("Id", NestedComplexObject.Id);
                            objectDictionary.Add("_type", NestedComplexObject.GetTypeEntityWrapperDetection().Name);
                            objectDictionary.Add("_uri",
                                string.Format("/{0}/{1}/{2}",
                                EntityType.NamespaceLastSegment(),
                                NestedComplexObject.GetTypeEntityWrapperDetection().NamePlural(),
                                NestedComplexObject.Id));

                            dtoDictionary.Add(pi.Name, objectDictionary);
                        }
                    }
                    #endregion
                }
            }

            //add the base _type
            dtoDictionary.Add("_type", Entity.GetTypeEntityWrapperDetection().Name);

            //add the base _uri
            dtoDictionary.Add("_uri", string.Format("/{0}/{1}/{2}", Entity.GetTypeEntityWrapperDetection().NamespaceLastSegment(), Entity.GetTypeEntityWrapperDetection().NamePlural(), Entity.Id));

            return dtoDictionary;
        }


    }
}