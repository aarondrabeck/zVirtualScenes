using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;

namespace WebAPI
{
    public sealed class ObjectPatcher<TEntityType> where TEntityType : class
    {
        private List<string> _InvalidProperties { get; set; }
        private Dictionary<string, object> _InvalidPropertyValues { get; set; }
        private Dictionary<string, object> _PendingChanges { get; set; }
        private Dictionary<string, object> _NoChanges { get; set; }

        private TEntityType PocoObject { get; set; }
        private IEnumerable<PropertyInfo> PocoObjectSimpleProperties { get; set; }

        public ObjectPatcher(TEntityType pocoObject, Dictionary<string, object> patchObject)
        {
            if (pocoObject == null)
                throw new ArgumentNullException("pocoObject");

            if (patchObject == null)
                throw new ArgumentNullException("patchObject");

            _InvalidProperties = new List<string>();
            _InvalidPropertyValues = new Dictionary<string, object>();
            _NoChanges = new Dictionary<string, object>();
            _PendingChanges = new Dictionary<string, object>();
            PocoObject = pocoObject;

            PocoObjectSimpleProperties = pocoObject.GetType()
                .GetProperties()
                .Where(o => !Attribute.IsDefined(o, typeof(ConfidentialDataAttribute)))
                .Where(o => o.PropertyType.IsBulitin());

            foreach (var objProp in patchObject)
            {
                if (!PocoObjectSimpleProperties.Select(o => o.Name).ToList().Contains(objProp.Key))
                {
                    _InvalidProperties.Add(objProp.Key);
                    continue;
                }

                var pocoProp = PocoObjectSimpleProperties.First(o => o.Name == objProp.Key);
                var pocoPropType = pocoProp.PropertyType;
                var pocoPropValue = pocoProp.GetValue(pocoObject);
                var patchPropvalue = patchObject[objProp.Key];

                //special handling of Nullables
                if (pocoProp.PropertyType.IsGenericType && pocoProp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (patchPropvalue == null)
                    {
                        if (pocoPropValue == null) //its a nullable that was already null so no change
                            _NoChanges.Add(pocoProp.Name, patchPropvalue);
                        else
                            _PendingChanges.Add(pocoProp.Name, patchPropvalue);

                        continue;
                    }

                    pocoPropType = Nullable.GetUnderlyingType(pocoPropType) ?? pocoPropType;
                }

                //try to change each type
                try
                {
                    object safeValue = TypeDescriptor.GetConverter(pocoPropType).ConvertFromInvariantString(patchPropvalue == null ? null : patchPropvalue.ToString());

                    if (safeValue != null && pocoPropValue != null)
                    {
                        if (safeValue.ToString() == pocoPropValue.ToString())
                            _NoChanges.Add(pocoProp.Name, patchPropvalue);
                        else
                            _PendingChanges.Add(pocoProp.Name, safeValue);
                    }
                    else if (safeValue == null && pocoPropValue == null)
                        _NoChanges.Add(pocoProp.Name, patchPropvalue);
                    else
                        _PendingChanges.Add(pocoProp.Name, safeValue);
                }
                catch (Exception)
                {
                    _InvalidPropertyValues.Add(pocoProp.Name, patchPropvalue);
                }
            }
        }

        /// <summary>
        /// Applies the changes on the poco properties that have changes pending
        /// </summary>
        /// <returns></returns>
        public TEntityType Patch()
        {
            foreach (var property in PocoObjectSimpleProperties)
            {
                if (_PendingChanges.ContainsKey(property.Name))
                    property.SetValue(PocoObject, _PendingChanges[property.Name]);
            }

            return PocoObject;
        }

        /// <summary>
        /// Dictionary of PropertyName to Values that will change upon Patch.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyDictionary<string, object> PropertiesPendingChange
        {
            get { return new ReadOnlyDictionary<string, object>(_PendingChanges); }
        }

        /// <summary>
        ///  Dictionary of PropertyName to Values that are not valid and will not change.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyDictionary<string, object> InvalidPropertyValues
        {
            get { return new ReadOnlyDictionary<string, object>(_InvalidPropertyValues); }
        }

        public ReadOnlyDictionary<string, object> PropertiesNotChanged
        {
            get { return new ReadOnlyDictionary<string, object>(_NoChanges); }
        }

        /// <summary>
        /// Lists all the properties in the partialObject that do not match the properties in TEntityType
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> InvalidPropertyNames
        {
            get { return _InvalidProperties.AsEnumerable(); }
        }
    }
}
