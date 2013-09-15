using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;

namespace WebAPI.DTO
{
    public static class DTOFactory
    {

        public static IEnumerable<object> GetDTO<T>(IEnumerable<T> Entities) where T : IIdentity
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            List<object> dtos = new List<object>();

            foreach (var entity in Entities)
                dtos.Add(GetDTO(entity));

#if DEBUG
            sw.Stop();
            Debug.WriteLine(string.Format("DTO Factory created IEnumerable of DTO in {0}", sw.Elapsed.ToString()));
#endif

            return dtos;
        }

        public static object GetDTO<T>(T Entity) where T : IIdentity
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            if (Entity == null)
                return null;

            var EntityType = Entity.GetTypeEntityWrapperDetection();

            var dtoDictionary = new Dictionary<string, object>();

            dtoDictionary.Add("Id", Entity.Id);
#if DEBUG
            Debug.WriteLine(string.Format("DTO Factory pre-property loop. {0}", sw.Elapsed.ToString()));
#endif
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
                        innerDictionary.Add("_uri", string.Format("/v2/{0}/{1}/{2}",
                            Cache.SigularToPluralEntityDictionary.ContainsKey(EntityType.Name) ? Cache.SigularToPluralEntityDictionary[EntityType.Name] : EntityType.Name,
                            Entity.Id,
                            pi.Name));

                        dtoDictionary.Add(pi.Name, innerDictionary);
                        continue;
                    }
                    #endregion

                    #region Print URI and types of complex types
                    if (typeof(IIdentity).IsAssignableFrom(pi.PropertyType))
                    {
                        //IIdentity NestedComplexObject = (IIdentity)pi.GetValue(Entity);

                        var idProperty = Entity.GetType().GetProperty(string.Format("{0}Id", pi.Name));
                        var nestedId = idProperty != null ? idProperty.GetValue(Entity) : Entity.Id;

                        if (nestedId == null)
                        {
                            dtoDictionary.Add(pi.Name, null);
                        }
                        else
                        {
                            //Print the URI to virtual objects
                            var objectDictionary = new Dictionary<string, object>();
                            objectDictionary.Add("Id", nestedId);
                            objectDictionary.Add("_type", pi.PropertyType.Name);
                            objectDictionary.Add("_uri",
                                string.Format("/{0}/{1}/{2}",
                                pi.PropertyType.NamespaceLastSegment(),
                                Cache.SigularToPluralEntityDictionary.ContainsKey(pi.PropertyType.Name) ? Cache.SigularToPluralEntityDictionary[pi.PropertyType.Name] : pi.PropertyType.Name,
                                nestedId));

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
#if DEBUG
            sw.Stop();
            Debug.WriteLine(string.Format("DTO Factory created single DTO in {0}", sw.Elapsed.ToString()));
#endif


            return dtoDictionary;


        }
    }
}