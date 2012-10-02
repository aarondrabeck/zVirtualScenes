Ext.define('zvsMobile.view.tablet.DeviceTabletViewPort', {
    extend: 'Ext.Panel',
    xtype: 'DeviceTabletViewPort',
    requires: ['zvsMobile.view.DeviceList',
                'zvsMobile.view.DeviceDetailsDimmer',
                'zvsMobile.view.DeviceDetailsSwitch',
                'zvsMobile.view.DeviceDetailsThermo'],
    config:
	    {
	        layout: 'hbox',
	        items: [
                    {
                        id: 'DeviceList',
                        xtype: 'DeviceList',
                        listeners:
		                {
		                    scope: this,
		                    selectionchange: function (list, records) {
		                        if (records[0] !== undefined) {
		                            var deviceDetailsPane = Ext.getCmp('deviceDetailsPane');


		                            if (records[0].data.type === 'DIMMER') {
		                                var DimmmerDetails = deviceDetailsPane.items.items[1];
		                                DimmmerDetails.loadDevice(records[0].data.id);
		                                deviceDetailsPane.getLayout().setAnimation({ type: 'slide', direction: 'up' });
		                                deviceDetailsPane.setActiveItem(DimmmerDetails);
		                                DimmmerDetails.ShowBackButton();
		                            }

		                            if (records[0].data.type === 'SWITCH') {
		                                var SWITCHDetails = deviceDetailsPane.items.items[2];
		                                SWITCHDetails.loadDevice(records[0].data.id);
		                                deviceDetailsPane.getLayout().setAnimation({ type: 'slide', direction: 'up' });
		                                deviceDetailsPane.setActiveItem(SWITCHDetails);
		                            }

		                            if (records[0].data.type === 'THERMOSTAT') {
		                                var ThermoDetails = deviceDetailsPane.items.items[3];
		                                ThermoDetails.loadDevice(records[0].data.id);
		                                deviceDetailsPane.getLayout().setAnimation({ type: 'slide', direction: 'up' });
		                                deviceDetailsPane.setActiveItem(ThermoDetails);
		                            }


		                        }
		                    }
		                },
                        flex: 1
                    },
                    {
                        flex: 2,
                        id: 'deviceDetailsPane',
                        layout: {
                            type: 'card',
                            animation: {
                                type: 'slide',
                                direction: 'left'
                            }
                        },
                        items: [{
                            cls: 'emptyDetail',
                            html: "Select a device to see more details."
                        }, {
                            xtype: 'DeviceDetailsDimmer',
                            id: 'DeviceDetailsDimmer'//,
                            //items: 
                        }, {
                            xtype: 'DeviceDetailsSwitch',
                            id: 'DeviceDetailsSwitch'
                        }, {
                            xtype: 'DeviceDetailsThermo',
                            id: 'DeviceDetailsThermo'
                        },
                         {
                             xtype: 'toolbar',
                             docked: 'top',
                             title: 'Device Details',
                             scrollable: false
                         }
                        ]
                    }

	        ]
	    }
});
