namespace MQTTPlugin
{
    public class DeviceCommand
    {
        public int NodeId { get; set; }
        public int CommandId { get; set; }
        public string Argument1 { get; set; }
        public string Argument2 { get; set; }
    }
}
