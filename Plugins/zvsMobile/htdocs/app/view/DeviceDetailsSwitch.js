Ext.require(['Ext.Panel', 'Ext.util.JSONP', 'Ext.MessageBox'], function () {
    Ext.define('zvsMobile.view.DeviceDetailsSwitch', {
        extend: 'Ext.Panel',
        alias: 'widget.DeviceDetailsSwitch',

        constructor: function (config) {
            var self = this;
            var RepollTimer;
            self.deviceID = 0;
            Ext.apply(config || {}, {
                delayedReload: function () {
                    if (RepollTimer) { clearInterval(RepollTimer); }

                    RepollTimer = setTimeout(function () {
                        var id = self.items.items[0].items.items[1].getData().id;
                        self.loadDevice(self.deviceID);
                    }, 1500);
                },
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
                    data.level_txt = value > 0 ? 'ON' : 'OFF';
                    data.on_off = value > 0 ? 'ON' : 'OFF';
                    self.items.items[0].items.items[1].setData(data);

                    //Update the store 
                    data = DeviceStore.data.items;
                    for (i = 0, len = data.length; i < len; i++) {
                        if (data[i].data.id === self.items.items[0].items.items[1]._data.id) {
                            data[i].data.level = value;
                            data[i].data.level_txt = value > 0 ? 'On' : 'Off';
                            data[i].data.on_off = value > 0 ? 'ON' : 'OFF';
                        }
                    }
                    DeviceStore.loadRecords(data);
                    self.parent.items.items[0].refresh();

                },
                items: {
                    xtype: 'panel',
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
                                DeviceViewPort.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                                DeviceViewPort.setActiveItem(DeviceViewPort.items.items[0]);
                            }
                        }]
                    },
                        {
                            xtype: 'panel',
                            tpl: new Ext.XTemplate(
							    '<div class="device_info">',
							    '<div id="level_switch_img" class="imageholder {type}_{on_off}"></div>',
							    '<div id="level_switch_details" class="level">{level_txt}</div>',
								    '<h1>{name}</h1>',
								    '<h2>{type_txt}<h2>',
								    '<div class="overview"><strong>Groups: </strong>{groups}<br />',
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
                                        text: 'Set Level',
                                        ui: 'confirm',
                                        margin: 5,
                                        handler: function () {
                                            var toggleValue = self.items.items[0].items.items[2].items.items[0].getValue();
                                            console.log('AJAX: SendCmd SEt LEVEL' + toggleValue);
                                            Ext.util.JSONP.request({
                                                url: 'http://10.1.0.56:9999/JSON/SendCmd',
                                                callbackKey: 'callback',
                                                params: {
                                                    u: Math.random(),
                                                    id: self.deviceID,
                                                    cmd: toggleValue > 0 ? 'TURNON' : 'TURNOFF',
                                                    arg: 0,
                                                    type: 'device_type'

                                                },
                                                callback: function (data) {
                                                    if (data.success) {
                                                        self.delayedReload();
                                                        Ext.Msg.alert('Switch Command', 'Switch set to ' + (toggleValue > 0 ? 'On' : 'Off'));
                                                    }
                                                    else {
                                                        Ext.Msg.alert('Switch Command', 'Communication Error!');
                                                    }
                                                }
                                            });
                                        }
                                    }]
					    },
                                {
                                    xtype: 'button',
                                    label: 'Repoll',
                                    text: 'Repoll',
                                    ui: 'confirm',
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
                                                    self.delayedReload();
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
});