namespace zvs.DataModel
{
    public class DeviceTypeCommand : Command
    {
        public int DeviceTypeId { get; set; }
        public virtual DeviceType DeviceType { get; set; }
    }
}
