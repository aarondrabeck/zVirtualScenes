Ext.require(['Ext.Panel', 'Ext.util.JSONP', 'Ext.MessageBox'], function () {
    Ext.define('zvsMobile.view.DeviceDetailsSwitch', {
        extend: 'Ext.Panel',
        alias: 'widget.DeviceDetailsSwitch',

        constructor: function (config) {
            this.RepollTimer;
            this.deviceID = 0;
            var self = this;
            Ext.apply(config || {}, {
                xtype: 'panel',
                layout: 'vbox',
                scrollable: 'vertical',
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
                }, {
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
                }, {
                    xtype: 'fieldset',
                    margin: 5,
                    defaults: {
                        labelAlign: 'right'
                    },
                    items: [{
                        xtype: 'togglefield',
                        label: 'OFF / ON'
                    }, {
                        xtype: 'button',
                        text: 'Set Level',
                        ui: 'confirm',
                        margin: 5,
                        handler: function () {
                            var toggleValue = self.items.items[2].items.items[0].getValue();
                            console.log('AJAX: SendCmd SEt LEVEL' + toggleValue);

                            Ext.Ajax.request({
                                url: '/API/device/' + self.deviceID + '/command/',
                                method: 'POST',
                                params: {
                                    u: Math.random(),
                                    name: toggleValue > 0 ? 'TURNON' : 'TURNOFF',
                                    arg: 0,
                                    type: 'device_type'
                                },
                                success: function (response, opts) {
                                    var result = JSON.parse(response.responseText);
                                    if (result.success) {
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
                }, {
                    xtype: 'button',
                    label: 'Repoll',
                    text: 'Repoll',
                    ui: 'confirm',
                    margin: '15 10 5 10',
                    handler: function () {
                        console.log('AJAX: SendCmd REPOLL_ME');

                        Ext.Ajax.request({
                            url: '/API/commands/',
                            method: 'POST',
                            params: {
                                u: Math.random(),
                                name: 'REPOLL_ME',
                                arg: self.deviceID
                            },
                            success: function (response, opts) {
                                var result = JSON.parse(response.responseText);
                                if (result.success) {
                                    self.delayedReload();
                                }
                                else {
                                    console.log('ERROR');
                                }
                            }
                        });
                    }

                }],
                listeners: {
                    scope: this,
                    deactivate: function () {
                        if (self.RepollTimer) { clearInterval(self.RepollTimer); }
                    }
                }
            });
            this.callOverridden([config]);
        },
        delayedReload: function () {
            var self = this;
            var detailsTPL = self.items.items[1];
            if (self.RepollTimer) { clearInterval(self.RepollTimer); }

            self.RepollTimer = setTimeout(function () {
                var id = detailsTPL.getData().id;
                self.loadDevice(self.deviceID);
            }, 1500);
        },
        loadDevice: function (deviceId) {
            var self = this;
            var detailsTPL = self.items.items[1];
            this.deviceID = deviceId;

            //Get Device Details			
            console.log('AJAX: GetDeviceDetails');
            Ext.data.JsonP.request({
                url: '/API/device/' + deviceId,
                callbackKey: 'callback',
                params: {
                    u: Math.random()
                },
                success: function (result) {
                    //Send data to panel TPL                            
                    detailsTPL.setData(result.details);

                    //Update meter levels 
                    self.UpdateLevel(result.details.level);
                }
            });
        },
        UpdateLevel: function (value) {
            var self = this;
            var detailsTPL = self.items.items[1];
            var toggle = self.items.items[2].items.items[0];
            toggle.setValue(value);

            //Update panel TPL        
            var data = Ext.clone(detailsTPL.getData());
            data.level = value;
            data.level_txt = value > 0 ? 'ON' : 'OFF';
            data.on_off = value > 0 ? 'ON' : 'OFF';
            detailsTPL.setData(data);

            //Update the store 
            data = DeviceStore.data.items;
            for (i = 0, len = data.length; i < len; i++) {
                if (data[i].data.id === detailsTPL._data.id) {
                    data[i].data.level = value;
                    data[i].data.level_txt = value > 0 ? 'On' : 'Off';
                    data[i].data.on_off = value > 0 ? 'ON' : 'OFF';
                }
            }
            DeviceStore.add(data);
            self.parent.items.items[0].refresh();
        }
    });
});