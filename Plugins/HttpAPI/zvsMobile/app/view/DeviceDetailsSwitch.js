Ext.define('zvsMobile.view.DeviceDetailsSwitch', {
    extend: 'Ext.Panel',
    requires: ['Ext.field.Toggle', 'Ext.data.proxy.JsonP'],
    xtype: 'DeviceDetailsSwitch',

    constructor: function (config) {
        this.RepollTimer;
        this.deviceID = 0;
        var self = this;
        Ext.apply(config || {}, {
            xtype: 'panel',
            layout: 'vbox',
            scrollable: 'vertical',
            items: [{
                xtype: 'panel',
                id: 'switchDetailsTPL',
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
                    id: 'switchToggle',
                    label: 'OFF / ON',
                    listeners: {
                        scope: this,
                        change: function (slider, value) {
                            var switchToggle = Ext.getCmp('switchToggle');
                            if (switchToggle) {
                                var toggleValue = switchToggle.getValue();
                                console.log('AJAX: SendCmd SEt LEVEL' + toggleValue);
                                Ext.Ajax.request({
                                    url: zvsMobile.app.BaseURL() + '/device/' + self.deviceID + '/command/',
                                    method: 'POST',
                                    headers: {
                                        'zvstoken': zvsMobile.app.getToken()
                                    },
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
                                            //Ext.Msg.alert('Switch Command', 'Switch set to ' + (toggleValue > 0 ? 'On' : 'Off'));
                                        }
                                        else {
                                            Ext.Msg.alert('Switch Command', 'Communication Error!');
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
                        url: zvsMobile.app.BaseURL() + '/commands/',
                        method: 'POST',
                        headers: {
                            'zvstoken': zvsMobile.app.getToken()
                        },
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
        this.callSuper([config]); 
    },
    delayedReload: function () {
        var self = this;
        var switchDetailsTPL = Ext.getCmp('switchDetailsTPL');
        if (self.RepollTimer) { clearInterval(self.RepollTimer); }

        self.RepollTimer = setTimeout(function () {           
            self.loadDevice(self.deviceID);
        }, 1500);
    },
    loadDevice: function (deviceId) {
        var self = this;
        var switchDetailsTPL = Ext.getCmp('switchDetailsTPL');
        this.deviceID = deviceId;

        //Get Device Details			
        console.log('AJAX: GetDeviceDetails');
        Ext.Ajax.request({
            url: zvsMobile.app.BaseURL() + '/device/' + deviceId,
            method: 'GET',
            headers: {
                'zvstoken': zvsMobile.app.getToken()
            },
            params: {
                u: Math.random()
            },
            success: function (response) {
                var result = JSON.parse(response.responseText);
                if (result.success) {
                    //Send data to panel TPL                            
                    switchDetailsTPL.setData(result.details);

                    //Update meter levels 
                    self.UpdateLevel(result.details.level);
                }               
            }
        });
    },
    UpdateLevel: function (value) {
        var self = this;
        var switchToggle = Ext.getCmp('switchToggle');
        var switchDetailsTPL = Ext.getCmp('switchDetailsTPL');
        switchToggle.setValue(value);

        //Update panel TPL        
        var data = Ext.clone(switchDetailsTPL.getData());
        data.level = value;
        data.level_txt = value > 0 ? 'ON' : 'OFF';
        data.on_off = value > 0 ? 'ON' : 'OFF';
        switchDetailsTPL.setData(data);

        //Update the store 
        data = Ext.getStore('Devices').data.items;
        for (i = 0, len = data.length; i < len; i++) {
            if (data[i].data.id === switchDetailsTPL._data.id) {
                data[i].data.level = value;
                data[i].data.level_txt = value > 0 ? 'On' : 'Off';
                data[i].data.on_off = value > 0 ? 'ON' : 'OFF';
            }
        }
        Ext.getStore('Devices').add(data);
        //Refresh the DEvice list
        Ext.getCmp('DeviceList').refresh();
    }
});