using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("SceneSettings", Schema = "ZVS")]
    public class SceneSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<SceneSettingOption> _options = new ObservableCollection<SceneSettingOption>();
        public virtual ObservableCollection<SceneSettingOption> Options
        {
            get { return _options; }
            set { _options = value; }
        }
              
    }
}
