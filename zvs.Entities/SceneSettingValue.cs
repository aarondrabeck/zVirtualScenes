using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Data.Entity;

namespace zvs.Entities
{
    [Table("SceneSettingValues", Schema = "ZVS")]
    public class SceneSettingValue : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SceneId { get; set; }
        public virtual Scene Scene { get; set; }

        public int SceneSettingId { get; set; }
        public virtual SceneSetting SceneSetting { get; set; }

        private string _value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == _value) return;
                _value = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //TODO: MOve to extension method...
        public async static Task<string> GetPropertyValueAsync(zvsContext Context, Scene scene, string scenePropertyUniqueIdentifier)
        {
            //Find the property
            SceneSetting property = await Context.SceneSettings.FirstOrDefaultAsync(p => p.UniqueIdentifier == scenePropertyUniqueIdentifier);
            
            if (property == null)
                return string.Empty;

            Scene s2 = await Context.Scenes.Include(o=> o.SettingValues).FirstOrDefaultAsync(o => o.Id == scene.Id);

            if (s2 == null)
                return string.Empty;

            SceneSettingValue spv = s2.SettingValues.FirstOrDefault(o => o.SceneSetting == property);
                
            //Check to see if the property has been set yet, otherwise return the default value for this property.
            if (spv != null)
                return spv.Value;

            return property.Value; //default value
        }
    }
}
