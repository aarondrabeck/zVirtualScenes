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
