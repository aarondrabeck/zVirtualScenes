Ext.define('zvsMobile.view.DeviceDetailsTemp', {
    extend: 'Ext.Panel',
    alias: 'widget.DeviceDetailsTemp',

    constructor: function (config) {
        var self = this;
        var RepollTimer;
        self.deviceID = 0;
        Ext.apply(config || {}, {
            loadDevice: function (deviceId) {
                self.deviceID = deviceId;
                //Get Device Details			
                console.log('AJAX: GetDeviceDetails');
                Ext.util.JSONP.request({
                    url: 'http://10.1.0.56:9999/JSON/GetDeviceDetails',
                    callbackKey: 'callback',
                    params: {
                        u: Math.random(),
                        id: deviceId
                    },
                    callback: function (data) {
                        console.log(data);

                        //Send data to panel TPL                            
                        self.items.items[0].items.items[1].setData(data);

                        //Update meter levels 
                        self.UpdateLevel(data.level);
                    }
                });
            },
            UpdateLevel: function (value) {

                var toggle = self.items.items[0].items.items[2].items.items[0];
                toggle.setValue(value);

                //Update panel TPL        
                var data = Ext.clone(self.items.items[0].items.items[1].getData());
                data.level = value;
                data.level_txt = value + 'F';
                //data.on_off = value > 0 ? 'ON' : 'OFF';
                self.items.items[0].items.items[1].setData(data);

                //Update the store 
                data = DeviceStore.data.items;
                for (i = 0, len = data.length; i < len; i++) {
                    if (data[i].data.id === self.items.items[0].items.items[1]._data.id) {
                        data[i].data.level = value;
                        data[i].data.level_txt = value + 'F';
                        //data[i].data.on_off = value > 0 ? 'ON' : 'OFF';
                    }
                }
                DeviceStore.loadRecords(data);
                self.parent.items.items[0].refresh();

            },
            items: {
                xtype: 'panel',
                scroll: 'vertical',
                items: [{
                    xtype: 'toolbar',
                    docked: 'top',
                    title: 'Device Details',
                    items: [{
                        xtype: 'button',
                        iconMask: true,
                        ui: 'back',
                        text: 'Back',
                        handler: function () {
                            var DeviceViewPort = self.parent;
                            DeviceViewPort.setActiveItem(DeviceViewPort.items.items[0], { type: 'slide', reverse: true });
                        }
                    }]
                },
                        {
                            xtype: 'panel',
                            tpl: new Ext.XTemplate(
							    '<div class="device_info">',
							    '<div id="level_temp_img" class="imageholder {type}"></div>',
							    '<div id="level_temp_details" class="level">{level_txt}</div>',
								    '<h1>{name}</h1>',
								    '<h2>{type_txt}<h2>',
								    '<div class="overview">',
                                    '<strong>Operating State: </strong>{op_state}<br />',
                                    '<strong>Fan State: </strong>{fan_state}<br /><br />',
                                    '<strong>Mode: </strong>{mode}<br />',
                                    '<strong>Fan Mode: </strong>{fan_mode}<br />',                                   
                                    '<strong>Heat Point: </strong>{heat_p}&deg;<br />',
                                    '<strong>Cool Point: </strong>{cool_p}&deg;<br /><br />',
                                    '<strong>Groups: </strong>{groups}<br />',
								    '<strong>Updated: </strong>{last_heard_from}</div>',
							    '</div>')
                        },
					    {
					        xtype: 'fieldset',
					        margin: 5,
					        defaults: {
					            labelAlign: 'right'
					        },
					        items: [{
					            xtype: 'togglefield',
					            label: 'OFF / ON'
					        },
                                    {
                                        xtype: 'button',
                                        text: 'Picker',
                                        ui: 'confirm',
                                        margin: 5,
                                        handler: function () {
                                            var toggle = self.items.items[0].items.items[2].items.items[0];
                                            //console.log('AJAX: SendCmd SEt LEVEL' + slider.getValue()[0]);
                                            self.UpdateLevel(toggle.getValue()[0]);
                                            var picker = Ext.create('Ext.Picker', {
                                                slots: [
                                                {
                                                    name: 'temperature',
                                                    title: 'Temperature',
                                                    data: [
                                                        { text: '40&deg;', value: 40 },
                                                        { text: '41&deg;', value: 41 },
                                                        { text: '42&deg;', value: 42 },
                                                        { text: '43&deg;', value: 43 },
                                                        { text: '44&deg;', value: 44 },
                                                        { text: '45&deg;', value: 45 },
                                                        { text: '46&deg;', value: 46 },
                                                        { text: '47&deg;', value: 47 },
                                                        { text: '48&deg;', value: 48 },
                                                        { text: '49&deg;', value: 49 },
                                                        { text: '50&deg;', value: 50 },
                                                        { text: '51&deg;', value: 51 },
                                                        { text: '52&deg;', value: 52 },
                                                        { text: '53&deg;', value: 53 },
                                                        { text: '54&deg;', value: 54 },
                                                        { text: '55&deg;', value: 55 },
                                                        { text: '56&deg;', value: 56 },
                                                        { text: '57&deg;', value: 57 },
                                                        { text: '58&deg;', value: 58 },
                                                        { text: '59&deg;', value: 59 },
                                                        { text: '60&deg;', value: 60 },
                                                        { text: '61&deg;', value: 61 },
                                                        { text: '62&deg;', value: 62 },
                                                        { text: '63&deg;', value: 63 },
                                                        { text: '64&deg;', value: 64 },
                                                        { text: '65&deg;', value: 65 },
                                                        { text: '66&deg;', value: 66 },
                                                        { text: '67&deg;', value: 67 },
                                                        { text: '68&deg;', value: 68 },
                                                        { text: '69&deg;', value: 69 },
                                                        { text: '70&deg;', value: 70 },
                                                        { text: '71&deg;', value: 71 },
                                                        { text: '72&deg;', value: 72 },
                                                        { text: '73&deg;', value: 73 },
                                                        { text: '74&deg;', value: 74 },
                                                        { text: '75&deg;', value: 75 },
                                                        { text: '76&deg;', value: 76 },
                                                        { text: '77&deg;', value: 77 },
                                                        { text: '78&deg;', value: 78 },
                                                        { text: '79&deg;', value: 79 },
                                                        { text: '80&deg;', value: 80 },
                                                        { text: '81&deg;', value: 81 },
                                                        { text: '82&deg;', value: 82 },
                                                        { text: '83&deg;', value: 83 },
                                                        { text: '84&deg;', value: 84 },
                                                        { text: '85&deg;', value: 85 },
                                                        { text: '86&deg;', value: 86 },
                                                        { text: '87&deg;', value: 87 },
                                                        { text: '88&deg;', value: 88 },
                                                        { text: '89&deg;', value: 89 },
                                                        { text: '90&deg;', value: 90 },
                                                        { text: '91&deg;', value: 91 },
                                                        { text: '92&deg;', value: 92 },
                                                        { text: '93&deg;', value: 93 },
                                                        { text: '94&deg;', value: 94 },
                                                        { text: '95&deg;', value: 95 },
                                                        { text: '96&deg;', value: 96 },
                                                        { text: '97&deg;', value: 97 },
                                                        { text: '98&deg;', value: 98 },
                                                        { text: '99&deg;', value: 99 }
                                                    ]
                                                }
                                            ]
                                            });
                                            picker.show();
                                        }
                                    }]
					    },
                                {
                                    xtype: 'button',
                                    label: 'Repoll',
                                    text: 'Repoll',
                                    ui: 'action',
                                    margin: '15 10 5 10',
                                    handler: function () {
                                        console.log('AJAX: SendCmd REPOLL_ME');
                                        Ext.util.JSONP.request({
                                            url: 'http://10.1.0.56:9999/JSON/SendCmd',
                                            callbackKey: 'callback',
                                            params: {
                                                u: Math.random(),
                                                cmd: 'REPOLL_ME',
                                                arg: self.deviceID,
                                                type: 'builtin'

                                            },
                                            callback: function (data) {

                                                if (data.success) {
                                                    console.log('OK');
                                                    if (RepollTimer) { clearInterval(RepollTimer); }

                                                    RepollTimer = setTimeout(function () {
                                                        var id = self.items.items[0].items.items[1].getData().id;
                                                        self.loadDevice(self.deviceID);
                                                    }, 1500);
                                                }
                                                else {
                                                    console.log('ERROR');
                                                }
                                            }
                                        });
                                    }
                                }]
            },
            listeners: {
                scope: this,
                deactivate: function () {
                    if (RepollTimer) { clearInterval(RepollTimer); }
                }
            }
        });
        this.callOverridden([config]);
    },
    config:
	{
	    layout: 'fit'
	}
});