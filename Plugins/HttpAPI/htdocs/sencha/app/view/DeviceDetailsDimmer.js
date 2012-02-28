Ext.define('zvsMobile.view.DeviceDetailsDimmer', {
    extend: 'Ext.Panel',
    xtype: 'DeviceDetailsDimmer',

    constructor: function (config) {
        var self = this;
        self.RepollTimer;
        self.deviceID = 0;
        Ext.apply(config || {}, {
            xtype: 'panel',
            layout: 'vbox',
            scrollable: 'vertical',
            items: [{
                xtype: 'panel',
                id: 'dimmerDetailsTPL',
                tpl: new Ext.XTemplate(
							    '<div class="device_info">',
							    '<div id="level_dimmer_img" class="imageholder {type}_{on_off}"></div>',
							    '<div id="level_dimmer_details" class="level">{level_txt}</div>',
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
                    xtype: 'sliderfield',
                    id: 'dimmerSlider',
                    label: 'Level',
                    minValue: 0,
                    maxValue: 99,
                    listeners: {
                        scope: this,
                        change: function (slider, value) {
                            var dimmerSlider = Ext.getCmp('dimmerSlider');
                            if (dimmerSlider) {
                                dimmerSlider.element.dom.childNodes[0].childNodes[0].innerHTML = slider.getValue() + "%";

                                var dimmerSlider = Ext.getCmp('dimmerSlider');
                                var sliderValue = dimmerSlider.getValue()[0]

                                console.log('AJAX: SendCmd SEt LEVEL' + sliderValue);

                                Ext.Ajax.request({
                                    url: zvsMobile.app.APIURL + '/device/' + self.deviceID + '/command/',
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
                                            //Ext.Msg.alert('Dimmer Command', 'Dimmer set to ' + sliderValue + '%');
                                        }
                                        else {
                                            Ext.Msg.alert('Dimmer Command', 'Communication Error!');
                                        }
                                    }
                                });
                            }
                        }
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
                                url: zvsMobile.app.APIURL + '/commands/',
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
            self.loadDevice(self.deviceID);
        }, 5000);
    }, ShowBackButton: function () {
        var self = this;
        self.items.items[0].visibility = false;
    },
    loadDevice: function (deviceId) {
        var self = this;
        var detailsTPL = Ext.getCmp('dimmerDetailsTPL');
        self.deviceID = deviceId;
        //Get Device Details			
        console.log('AJAX: GetDeviceDetails');

        Ext.data.JsonP.request({
            url: zvsMobile.app.APIURL + '/device/' + deviceId,
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
        var level = value > 99 ? 99 : value;
        var dimmerSlider = Ext.getCmp('dimmerSlider');
        var detailsTPL = Ext.getCmp('dimmerDetailsTPL');

        dimmerSlider.setValue(level);
        dimmerSlider.element.dom.childNodes[0].childNodes[0].innerHTML = level + "%";

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

        //Refresh the DEvice list
        Ext.getCmp('DeviceList').refresh();

    }
});