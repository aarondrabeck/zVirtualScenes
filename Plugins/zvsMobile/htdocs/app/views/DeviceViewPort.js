	
 zvsMobile.DeviceViewPort = Ext.extend(Ext.Panel, {
 layout: 'card',
 fullscreen: true,
 initComponent: function () {
		var self = this;		
		Ext.apply(this, {			
			items: [ {	
						xtype: 'panel',	
						layout: 'card',						
						dockedItems: {
							xtype: 'toolbar',
							dock: 'top',
							title: "Devices",
							items: [{ 
										xtype: 'button',
										iconMask: true,
										iconCls: 'refresh',
										handler: function () { 
											var deviceList = self.items.items[0].items.items[0]; 
											deviceList.AjaxLoad();
										}
								   }]
						},
						items: [{
									xtype: 'DeviceList',
									scroll: 'veritcal',						 
									store:  zvsMobile.DeviceStore,
									listeners: {
										scope: this,		
										selectionchange: function (list, records) {
											if (records[0] !== undefined) {	
												var DimmmerDetails = self.items.items[1];
												var SwitchDetails = self.items.items[2];

												if(records[0].data.type === 'DIMMER')
												{												
													DimmmerDetails.loadDevice(records[0].data.id);
													self.setActiveItem(DimmmerDetails, Ext.is.Android ? undefined : { type: 'slide', direction: 'left' }); 									
												}
												
												if(records[0].data.type === 'SWITCH')
												{												
													SwitchDetails.loadDevice(records[0].data.id);
													self.setActiveItem(SwitchDetails, Ext.is.Android ? undefined : { type: 'slide', direction: 'left' }); 									
												}
											}		
										}			
									}
						}],
						listeners: {
							scope: this,
							activate: function () {
								var deviceList = self.items.items[0].items.items[0];							
								deviceList.getSelectionModel().deselectAll();
							}
						}
					},					
					{
						xtype: 'DeviceDetailsDimmer',
						scroll: 'veritcal',							
						dockedItems: {
                        xtype: 'toolbar',
                        dock: 'top',
                        title: "Dimmer Details",
                        items: [{ xtype: 'button',
                                    iconMask: true,
                                    ui: 'back',
                                    text: 'Back',
                                    handler: function () { self.setActiveItem(self.items.items[0], Ext.is.Android ? undefined : { type: 'slide', direction: 'right' }); }
                                }]
						}
					},					
					{
						xtype: 'DeviceDetailsSwitch',
						scroll: 'veritcal',							
						dockedItems: {
                        xtype: 'toolbar',
                        dock: 'top',
                        title: "Switch Details",
                        items: [{ xtype: 'button',
                                    iconMask: true,
                                    ui: 'back',
                                    text: 'Back',
                                    handler: function () { self.setActiveItem(self.items.items[0], Ext.is.Android ? undefined : { type: 'slide', direction: 'right' }); }
                                }]
						}
					}
					
					]
	
		});
		zvsMobile.DeviceViewPort.superclass.initComponent.apply(this, arguments);
	}
});
Ext.reg('DeviceViewPort', zvsMobile.DeviceViewPort);
