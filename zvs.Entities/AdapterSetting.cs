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
    [Table("AdapterSettings", Schema = "ZVS")]
    public partial class AdapterSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AdapterId { get; set; }
        public virtual Adapter Adapter { get; set; }

        private ObservableCollection<AdapterSettingOption> _Options = new ObservableCollection<AdapterSettingOption>();
        public virtual ObservableCollection<AdapterSettingOption> Options
        {
            get { return _Options; }
            set { _Options = value; }
        }
    }
}
