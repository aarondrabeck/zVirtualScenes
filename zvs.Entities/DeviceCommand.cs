using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("DeviceCommands", Schema = "ZVS")]
    public class DeviceCommand : Command
    {
        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }
    }
}
