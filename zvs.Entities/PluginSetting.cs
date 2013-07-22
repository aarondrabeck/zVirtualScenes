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
