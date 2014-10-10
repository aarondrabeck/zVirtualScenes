using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("AdapterSettings", Schema = "ZVS")]
    public class AdapterSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AdapterId { get; set; }
        public virtual Adapter Adapter { get; set; }

        private ObservableCollection<AdapterSettingOption> _options = new ObservableCollection<AdapterSettingOption>();
        public virtual ObservableCollection<AdapterSettingOption> Options
        {
            get { return _options; }
            set { _options = value; }
        }
    }
}
