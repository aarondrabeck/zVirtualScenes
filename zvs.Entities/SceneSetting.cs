using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("SceneSettings", Schema = "ZVS")]
    public partial class SceneSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<SceneSettingOption> _Options = new ObservableCollection<SceneSettingOption>();
        public virtual ObservableCollection<SceneSettingOption> Options
        {
            get { return _Options; }
            set { _Options = value; }
        }
              
    }
}
