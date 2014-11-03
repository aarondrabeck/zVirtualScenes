namespace zvs.DataModel
{
    public class DeviceCommand : Command
    {
        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }
    }
}
