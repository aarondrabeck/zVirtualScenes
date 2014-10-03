using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("DeviceCommands", Schema = "ZVS")]
    public partial class DeviceCommand : Command
    {
        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }
    }
}
