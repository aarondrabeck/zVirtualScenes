using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("PluginSettings", Schema = "ZVS")]
    public class PluginSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PluginId { get; set; }
        public virtual Plugin Plugin { get; set; }

        private ObservableCollection<PluginSettingOption> _options = new ObservableCollection<PluginSettingOption>();
        public virtual ObservableCollection<PluginSettingOption> Options
        {
            get { return _options; }
            set { _options = value; }
        }
    }
}
