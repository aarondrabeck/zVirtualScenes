Ext.define('zvsMobile.view.phone.DevicePhoneViewPort', {
    extend: 'Ext.Panel',
    xtype: 'DevicePhoneViewPort',
    requires: ['zvsMobile.view.DeviceList',
                      'zvsMobile.view.DeviceDetailsDimmer',
                      'zvsMobile.view.DeviceDetailsSwitch',
                      'zvsMobile.view.DeviceDetailsThermo'],
    initialize: function () {
        this.callParent(arguments);
        //   this.getEventDispatcher().addListener('element', '#DeviceViewPort', 'swipe', this.onTouchPadEvent, this);
    },
    // onTouchPadEvent: function (e, target, options, eventController) {
    // console.log(e.target.className.indexOf("x-slider-inner"));
    // console.log(e.target.className);
    // if (e.direction === 'left' && e.distance > 50 && e.target.className.indexOf("x-slider-inner") == -1) {
    //     zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(1));
    //
    //  }
    // },
    config: {
        layout: {
            type: 'card',
            animation: {
                type: 'slide',
                direction: 'left'
            }
        },
        items: [{
            xtype: 'DeviceList',
            id: 'DeviceList',
            listeners: {
                scope: this,
                selectionchange: function (list, records) {
                    if (records[0] !== undefined) {
                        var DevicePhoneViewPort = Ext.getCmp('DevicePhoneViewPort');
                        var DimmmerDetails = Ext.getCmp('DeviceDetailsDimmer');
                        var SwitchDetails = Ext.getCmp('DeviceDetailsSwitch');
                        var TempDetails = Ext.getCmp('DeviceDetailsThermo');

                        if (records[0].data.type === 'DIMMER') {
                            DimmmerDetails.loadDevice(records[0].data.id);
                            DevicePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'left' });
                            DevicePhoneViewPort.setActiveItem(Ext.getCmp('DimmerView'));
                        }

                        if (records[0].data.type === 'SWITCH') {
                            SwitchDetails.loadDevice(records[0].data.id);
                            DevicePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'left' });
                            DevicePhoneViewPort.setActiveItem(Ext.getCmp('SwitchView'));
                        }

                        if (records[0].data.type === 'THERMOSTAT') {
                            TempDetails.loadDevice(records[0].data.id);
                            DevicePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'left' });
                            DevicePhoneViewPort.setActiveItem(Ext.getCmp('ThermoView'));
                        }
                    }
                },
                activate: function () {
                    Ext.getCmp('DeviceList').deselectAll();
                }
            }
        }, {
            layout: 'card',
            id: 'DimmerView',
            items: [{
                xtype: 'DeviceDetailsDimmer',
                id: 'DeviceDetailsDimmer'
            }, {
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
                        var DevicePhoneViewPort = Ext.getCmp('DevicePhoneViewPort');
                        DevicePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                        DevicePhoneViewPort.setActiveItem(Ext.getCmp('DeviceList'));
                    }
                }]
            }]
        }, {
            layout: 'card',
            id: 'SwitchView',
            items: [{
                xtype: 'DeviceDetailsSwitch',
                id: 'DeviceDetailsSwitch'
            }, {
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
                        var DevicePhoneViewPort = Ext.getCmp('DevicePhoneViewPort');
                        DevicePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                        DevicePhoneViewPort.setActiveItem(Ext.getCmp('DeviceList'));
                    }
                }]
            }]
        },
            {
                layout: 'card',
                id: 'ThermoView',
                items: [{
                    xtype: 'DeviceDetailsThermo',
                    id: 'DeviceDetailsThermo'
                }, {
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
                            var DevicePhoneViewPort = Ext.getCmp('DevicePhoneViewPort');
                            DevicePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                            DevicePhoneViewPort.setActiveItem(Ext.getCmp('DeviceList'));
                        }
                    }]
                }]
            }]
    }
});


   