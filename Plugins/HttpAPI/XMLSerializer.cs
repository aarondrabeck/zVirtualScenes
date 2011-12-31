using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;

namespace HttpAPI
{
        public static class XMLSerializer
        {
            /// <summary>
            /// Defines the simple types that is directly writeable to XML.
            /// </summary>
            private static readonly Type[] _writeTypes = new[] { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };

            /// <summary>
            /// Determines whether [is simple type] [the specified type].
            /// </summary>
            /// <param name="type">The type to check.</param>
            /// <returns>
            /// 	<c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsSimpleType(this Type type)
            {
                return type.IsPrimitive || _writeTypes.Contains(type);
            }

            /// <summary>
            /// Converts an anonymous type to an XElement.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns>Returns the object as it's XML representation in an XElement.</returns>
            public static XElement ToXml(this object input)
            {
                return input.ToXml(null);
            }

            /// <summary>
            /// Converts an anonymous type to an XElement.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <param name="element">The element name.</param>
            /// <returns>Returns the object as it's XML representation in an XElement.</returns>
            public static XElement ToXml(this object input, string element)
            {
                if (input == null)
                {
                    return null;
                }

                if (String.IsNullOrEmpty(element))
                {
                    element = "object";
                }

                element = XmlConvert.EncodeName(element);
                var ret = new XElement(element);

                if (input != null)
                {
                    var type = input.GetType();
                    var props = type.GetProperties();

                    var elements = from prop in props
                                   let name = XmlConvert.EncodeName(prop.Name)
                                   let val = prop.PropertyType.IsArray ? "array" : prop.GetValue(input, null)
                                   let value = prop.PropertyType.IsArray ? GetArrayElement(prop, (Array)prop.GetValue(input, null)) : (prop.PropertyType.IsSimpleType() ? new XElement(name, val) : val.ToXml(name))
                                   where value != null
                                   select value;

                    ret.Add(elements);
                }

                return ret;
            }

            /// <summary>
            /// Gets the array element.
            /// </summary>
            /// <param name="info">The property info.</param>
            /// <param name="input">The input object.</param>
            /// <returns>Returns an XElement with the array collection as child elements.</returns>
            private static XElement GetArrayElement(PropertyInfo info, Array input)
            {
                var name = XmlConvert.EncodeName(info.Name);

                XElement rootElement = new XElement(name);

                var arrayCount = input.GetLength(0);

                for (int i = 0; i < arrayCount; i++)
                {
                    var val = input.GetValue(i);
                    XElement childElement = val.GetType().IsSimpleType() ? new XElement(name + "Child", val) : val.ToXml();

                    rootElement.Add(childElement);
                }

                return rootElement;
            }
        }
    
}
