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
    [Table("PluginSettingOptions", Schema = "ZVS")]
    public class PluginSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PluginSettingId { get; set; }
        public virtual PluginSetting Setting { get; set; }
    }
}
