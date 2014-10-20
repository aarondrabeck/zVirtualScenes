using zvs.DataModel;

namespace zvs.Processor.Tests
{
    public static class UnitTesting
    {
        public static Adapter CreateFakeAdapter()
        {
            return new Adapter
            {
                Name = "Unit testing adapter",
            };
        }

        public static DeviceType CreateFakeDeviceType()
        {
            return new DeviceType
            {
                UniqueIdentifier = "1",
                Name = "DIMMER",
                Adapter = CreateFakeAdapter()
            };
        }

        public static Device CreateFakeDevice()
        {
            return new Device
            {
                Name = "Light Switch",
                Type = CreateFakeDeviceType()
            };

        }

    }
}
