using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("SceneSettingValues", Schema = "ZVS")]
    public partial class SceneSettingValue : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SceneId { get; set; }
        public virtual Scene Scene { get; set; }

        public int ScenePropertyId { get; set; }
        public virtual SceneSetting SceneProperty { get; set; }

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
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //TODO: MOve to extension method...
        public static string GetPropertyValue(zvsContext Context, Scene scene, string scenePropertyUniqueIdentifier)
        {
            //Find the property
            SceneSetting property = Context.SceneSettings.FirstOrDefault(p => p.UniqueIdentifier == scenePropertyUniqueIdentifier);
            
            if (property == null)
                return string.Empty;

            Scene s2 = Context.Scenes.FirstOrDefault(o => o.Id == scene.Id);

            if (s2 == null)
                return string.Empty;

            SceneSettingValue spv = s2.SettingValues.FirstOrDefault(o => o.SceneProperty == property);
                
            //Check to see if the property has been set yet, otherwise return the default value for this property.
            if (spv != null)
                return spv.Value;

            return property.Value; //default value
        }
    }
}
