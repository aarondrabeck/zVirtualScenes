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
    public partial class PluginSetting : BaseValue, IIdentity
    {
        public int Id { get; set; }

        public PluginSetting()
        {
            this.Options = new ObservableCollection<PluginSettingOption>();
        }

        public int PluginId { get; set; }
        [Required]
        public virtual Plugin Plugin { get; set; }

        public virtual ObservableCollection<PluginSettingOption> Options { get; set; }

    }
}
