using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("ScenePropertyValues", Schema = "ZVS")]
    public partial class ScenePropertyValue : INotifyPropertyChanged
    {
        public int ScenePropertyValueId { get; set; }

        [Required]
        public virtual Scene Scene { get; set; }

        [Required]
        public virtual SceneProperty SceneProperty { get; set; }

        private string _Value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public static string GetPropertyValue(zvsContext Context, Scene scene, string scenePropertyUniqueIdentifier)
        {
            //Find the property
            SceneProperty property = Context.SceneProperties.FirstOrDefault(p => p.UniqueIdentifier == scenePropertyUniqueIdentifier);
            
            if (property == null)
                return string.Empty;

            Scene s2 = Context.Scenes.FirstOrDefault(o => o.SceneId == scene.SceneId);

            if (s2 == null)
                return string.Empty;

            ScenePropertyValue spv = s2.PropertyValues.FirstOrDefault(o => o.SceneProperty == property);
                
            //Check to see if the property has been set yet, otherwise return the default value for this property.
            if (spv != null)
                return spv.Value;

            return property.Value; //default value
        }
    }
}
