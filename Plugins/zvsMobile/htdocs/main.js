new Ext.Application({
        name: 'zvsMobile',
        launch: function () 
		{
			zvsMobile.mainTabPanel = new Ext.TabPanel({        
				layout: 'card',
				fullscreen: true,
				cardSwitchAnimation: Ext.is.Android ? false :  {
					type: 'slide'
				},
				ActiveItem: 0,
				tabBar: {
					dock: 'bottom',
					layout: {
						pack: 'center'
					}
				},
				items: [
						{
							xtype: 'DeviceViewPort',
							title: "Devices",
							iconCls: "bulb",
							
						}
						]
			});
        }
});
	
	
 zvsMobile.DeviceViewPort = Ext.extend(Ext.Panel, {
 //layout: 'card',
 fullscreen: true, 
 initComponent: function () {
		var self = this;
		Ext.apply(this, {
			AjaxLoad: function () {
			      zvsMobile.mainTabPanel.setLoading(true);				  
				   Ext.util.JSONP.request({
                    url: 'http://localhost:9999/JSON/GetDeviceList',
                    callbackKey: 'callback',
                    params: {
                        u: Math.random()
                    },
                    callback: function (data) {
                        zvsMobile.DeviceStore.loadData(data);                        
                        zvsMobile.mainTabPanel.setLoading(false);
                    }                    
                });				 
			},
			items: [{			
						xtype: 'DeviceList',
						scroll: 'veritcal',
						fullscreen: true,
						store:  zvsMobile.DeviceStore,
						listeners: {
							scope: this,		
							selectionchange: function (list, records) {
								if (records[0] !== undefined) {
									//records[0].data.id
									var DeviceDetails =  Ext.getCmp('DeviceDetails');									
									DeviceDetails.loadDevice(records[0].data.id);								
								}		
							}			
						}
					}, 
					{
						id: 'DeviceDetails',
						xtype: 'DeviceDetails',
						listeners: {
							scope: this,
							hide : function() {
								self.items.items[0].getSelectionModel().deselectAll();
							}
						}
					}],
			dockedItems: {
					xtype: 'toolbar',
					dock: 'top',
					title: "Devices",
					items: [{ xtype: 'spacer' },
					{ xtype: 'button',
						iconMask: true,
						iconCls: 'refresh',
						handler: function () { self.AjaxLoad() }
					}]
				}
		});
		zvsMobile.DeviceViewPort.superclass.initComponent.apply(this, arguments);
	}
});
Ext.reg('DeviceViewPort', zvsMobile.DeviceViewPort);

Ext.regModel('Device', {
    feilds: ['id', 'name', 'on_off', 'level', 'level_txt', 'type']
});

zvsMobile.DeviceStore = new Ext.data.Store({
        model: 'Device',
        remoteFilter: true

});
	
 zvsMobile.DeviceList = Ext.extend(Ext.List, {
        cls: 'DeviceListItem',
        loadingText: 'Loading...',
        emptyText: '<div class="emptyText">Loading...</div>',
        itemTpl: new Ext.XTemplate(
		'<div class="device">',
            '<div class="imageholder {type}_{on_off}"></div>',
			'<h2>{name}</h2>',
			'<div class="level">',
				'<div class="meter">',
					'<div class="progress" style="width:{level}%">',
					'</div>',
				'</div>',
				'<div class="percent">{level_txt}</div>',				
			'</div>',
		'</div>'),
        scroll: false
    });
 Ext.reg('DeviceList',  zvsMobile.DeviceList);
 
 
 zvsMobile.DeviceDetails = Ext.extend(Ext.Sheet, {
    modal: true,
    centered : false,
    hideOnMaskTap : true,    
    layout: 'fit',
    enter: 'right',
    exit: 'right',
    
    // we always want the sheet to be 400px wide and to be as tall as the device allows
    width: 400,
    stretchY : true,
    
    initComponent: function() {
		var self = this;
        Ext.apply(this, {
		    loadDevice: function (deviceId) {	
				   zvsMobile.mainTabPanel.setLoading(true);
				   Ext.util.JSONP.request({
                    url: 'http://localhost:9999/JSON/GetDeviceDetails',
                    callbackKey: 'callback',
                    params: {
                        u: Math.random(),
						id: deviceId
                    },
                    callback: function (data) {
                        self.items.items[0].items.items[0].update(data);                        
                        zvsMobile.mainTabPanel.setLoading(false);
                    }                    
                });
				self.show();
			},			
            items: {
                xtype: 'panel',
                items: [ { tpl: new Ext.XTemplate(
							'<div class="device_info">',
								'Name: {name}</br>',
								'Level: {level_txt}</br>',
								'Type: {type_txt}</br>',
								'Last Heard From: {last_heard_from}</br>',
								'Groups: {groups}</br>',								
							'</div>'), } 
                ]
            },            
            dockedItems: [{
                xtype: 'button',
                text: 'Turn On',
                ui: 'action',
                dock: 'bottom'
            },
			{
                xtype: 'button',
                text: 'Turn Off',
                ui: 'action',
                dock: 'bottom'
            }]
        });
    
        zvsMobile.DeviceDetails.superclass.initComponent.apply(this, arguments);
    }
});
 Ext.reg('DeviceDetails',  zvsMobile.DeviceDetails);
 

 
 


	
	