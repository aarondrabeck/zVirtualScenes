Ext.require(['Ext.Panel', 'Ext.util.JSONP', 'Ext.MessageBox'], function () {
    Ext.define('zvsMobile.view.DeviceDetailsDimmer', {
        extend: 'Ext.Panel',
        alias: 'widget.DeviceDetailsDimmer',

        constructor: function (config) {
            var self = this;
            self.RepollTimer;
            self.deviceID = 0;
            Ext.apply(config || {}, {
                xtype: 'panel',
                layout: 'vbox',
                scrollable: 'vertical',
                items: [{
                    xtype: 'toolbar',
                    docked: 'top',
                    title: 'Device Details',
                    scrollable: false,
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
							    '<div id="level_dimmer_img" class="imageholder {type}_{on_off}"></div>',
							    '<div id="level_dimmer_details" class="level">{level_txt}</div>',
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
                                            xtype: 'sliderfield',
                                            id: 'sliderfield',
                                            label: 'Level',
                                            minValue: 0,
                                            maxValue: 99,
                                            listeners: {
                                                scope: this,
                                                change: function (slider, value) {
                                                    var details = Ext.get('sliderfield');
                                                    if (details) {
                                                        details.dom.childNodes[0].childNodes[1].childNodes[0].innerHTML = slider.getValue() + "%";
                                                    }
                                                }
                                            }
                                        }, {
                                            xtype: 'button',
                                            text: 'Set Level',
                                            ui: 'confirm',
                                            margin: 5,
                                            handler: function () {
                                                var sliderValue = self.items.items[2].items.items[0].getValue()[0]

                                                console.log('AJAX: SendCmd SEt LEVEL' + sliderValue);

                                                Ext.Ajax.request({
                                                    url: '/API/device/' + self.deviceID + '/command/',
                                                    method: 'POST',
                                                    params: {
                                                        u: Math.random(),
                                                        name: 'DYNAMIC_CMD_BASIC',
                                                        arg: sliderValue,
                                                        type: 'device'
                                                    },
                                                    success: function (response, opts) {
                                                        var result = JSON.parse(response.responseText);
                                                        if (result.success) {
                                                            self.delayedReload();
                                                            Ext.Msg.alert('Dimmer Command', 'Dimmer set to ' + sliderValue + '%');
                                                        }
                                                        else {
                                                            Ext.Msg.alert('Dimmer Command', 'Communication Error!');
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
                                    }
                                ]

                  ,
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
            }, 5000);
        },
        loadDevice: function (deviceId) {
            var self = this;
            var detailsTPL = self.items.items[1];
            self.deviceID = deviceId;
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
            var level = value > 99 ? 99 : value;
            var slider = self.items.items[2].items.items[0];
            var detailsTPL = self.items.items[1];

            slider.setValue(level);

            var details = Ext.get('sliderfield');
            details.dom.childNodes[0].childNodes[1].childNodes[0].innerHTML = level + "%";

            //Update panel TPL        
            var data = Ext.clone(detailsTPL.getData());
            data.level = level;
            data.level_txt = level + '%';
            if (level == 0) {
                data.on_off = 'OFF';
            }
            else if (level > 98) {
                data.on_off = 'ON';
            }
            else {
                data.on_off = 'DIM';
            }
            detailsTPL.setData(data);

            //Update the store 
            data = DeviceStore.data.items;
            for (i = 0, len = data.length; i < len; i++) {
                if (data[i].data.id === detailsTPL._data.id) {

                    data[i].data.level_txt = value + '%';
                    data[i].data.level = value;

                    if (value == 0) {
                        data[i].data.on_off = 'OFF';
                    }
                    else if (value > 98) {
                        data[i].data.on_off = 'ON';
                    }
                    else {
                        data[i].data.on_off = 'DIM';
                    }

                }
            }
            DeviceStore.add(data);
            self.parent.items.items[0].refresh();

        }
    });
});