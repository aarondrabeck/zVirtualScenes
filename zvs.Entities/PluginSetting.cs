using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("PluginSettings", Schema = "ZVS")]
    public class PluginSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PluginId { get; set; }
        public virtual Plugin Plugin { get; set; }

        private ObservableCollection<PluginSettingOption> _Options = new ObservableCollection<PluginSettingOption>();
        public virtual ObservableCollection<PluginSettingOption> Options
        {
            get { return _Options; }
            set { _Options = value; }
        }
    }
}
