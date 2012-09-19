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
    public partial class PluginSetting : BaseValue
    {
        public int PluginSettingId { get; set; }

        public PluginSetting()
        {
            this.Options = new ObservableCollection<PluginSettingOption>();
        }

        public virtual Plugin Plugin { get; set; }
        public virtual ObservableCollection<PluginSettingOption> Options { get; set; }
        
    }
}
