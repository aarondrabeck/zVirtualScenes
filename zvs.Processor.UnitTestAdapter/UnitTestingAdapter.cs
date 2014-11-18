using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor.UnitTestAdapter
{
    [Export(typeof(ZvsAdapter))]
    public class UnitTestingAdapter : ZvsAdapter
    {
        public override Guid AdapterGuid
        {
            get { return Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"); }
        }

        public override string Name
        {
            get { return "UnitTesting Adapter for ZVS            Name too                                                                                                      long                              a                                                    a                                        a"; }
        }

        public override string Description
        {
            get { return "This adapter is for unit testing"; }
        }

        public override async Task OnDeviceTypesCreating(DeviceTypeBuilder deviceTypeBuilder)
        {
        }

        public override async Task OnSettingsCreating(AdapterSettingBuilder settingBuilder)
        {
            var testSetting = new AdapterSetting
            {
                Name = "Test setting",
                Value = (360).ToString(CultureInfo.InvariantCulture),
                ValueType = DataType.STRING,
                Description = "Unit testing only"
            };

            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(testSetting, o => o.TestSetting);
        }

        public override async Task StartAsync()
        {
        }

        public override async Task StopAsync()
        {
        }

        //Settings Cache 
        private string _testSetting = "10";
        public string TestSetting
        {
            get { return _testSetting; }
            set
            {
                if (value != _testSetting)
                {
                    _testSetting = value;
                }
            }
        }


        public override Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device, DeviceTypeCommand command, string argument)
        {
            throw new NotImplementedException();
        }

        public override Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument, string argument2)
        {
            throw new NotImplementedException();
        }

        public override Task RepollAsync(Device device)
        {
            throw new NotImplementedException();
        }

        public override Task ActivateGroupAsync(Group group)
        {
            throw new NotImplementedException();
        }

        public override Task DeactivateGroupAsync(Group group)
        {
            throw new NotImplementedException();
        }
    }
}
