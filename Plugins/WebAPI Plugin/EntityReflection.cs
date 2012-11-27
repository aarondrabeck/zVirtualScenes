using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace WebAPI
{
    public static class  EntityReflection
    {
        /// <summary>
        /// Returns the actual type of entity if object is wrapped with entities Dynamic wrapper
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetTypeEntityWrapperDetection(this Object obj)
        {
            //Check for entity types here as they get wrapped in another dynamic type
            if (obj.GetType().Namespace != null && obj.GetType().Namespace.StartsWith("System.Data.Entity.Dynamic"))
                return obj.GetType().BaseType;
            else
                return obj.GetType();
        }

        public static string NamespaceLastSegment(this Type type)
        {
            string @namespace = type.Namespace;

            if (@namespace == null)
                return string.Empty;

            string[] namespaceSegments = @namespace.Split('.');
            return namespaceSegments.Last();
        }

        public static string NamePlural(this Type type)
        {
            var service = PluralizationService.CreateService(new CultureInfo("en-US"));
            return service.Pluralize(type.Name);
        }

        public static string NameSingular(this Type type)
        {
            var service = PluralizationService.CreateService(new CultureInfo("en-US"));
            return service.Singularize(type.Name);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in coll) c.Add(e);
            return c;
        }

        private static readonly HashSet<Type> BuiltInTypes = new HashSet<Type>()
        {
            typeof(object), 
            typeof(string), 
            typeof(Decimal),
            typeof(Decimal?),
            typeof(Guid),
            typeof(Guid?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(double?),
            typeof(double),            
            typeof(bool?),            
            typeof(int?)
        };

        public static bool IsBulitin(this Type type)
        {
            return (type.IsPrimitive && type != typeof(IntPtr) && type != typeof(UIntPtr))
                  || BuiltInTypes.Contains(type);
        }
       

        public static HttpResponseMessage CreateResponse(this HttpRequestMessage request, ResponseStatus responseStatus, HttpStatusCode statusCode, string message, object data = null, Dictionary<string, object> additionalKeyValues = null)
        {
            Dictionary<string, object> ResponseDictionary = new Dictionary<string, object>();
            ResponseDictionary.Add("Error", responseStatus == ResponseStatus.Error ? true : false);
            ResponseDictionary.Add("Message", message);
            ResponseDictionary.Add("StatusCode", (int)statusCode);
            ResponseDictionary.Add("StatusDescription", statusCode);

            if (additionalKeyValues != null)
                foreach (KeyValuePair<string, object> entry in additionalKeyValues)
                    if (!ResponseDictionary.ContainsKey(entry.Key))
                        ResponseDictionary.Add(entry.Key, entry.Value);

            if (data != null)
                ResponseDictionary.Add("Data", data);

            return request.CreateResponse(statusCode, ResponseDictionary);
        }
    }
}
namespace System.Net.Http
{
    public enum ResponseStatus
    {
        Error,
        Success
    }
}