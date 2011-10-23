zvsMobile.DeviceDetailsSwitch = Ext.extend(Ext.Panel, {
    fullscreen: true,    
    layout: 'fit',
    
    initComponent: function() {
		var self = this;
		var userChange = false; 
        Ext.apply(this, {
		    loadDevice: function (deviceId) {	
				   zvsMobile.mainTabPanel.setLoading(true);
				   Ext.util.JSONP.request({
                    url: 'http://10.1.0.55:9999/JSON/GetDeviceDetails',
                    callbackKey: 'callback',
                    params: {
                        u: Math.random(),
						id: deviceId
                    },
                    callback: function (data) {
                        var detailPanel = self.items.items[0].items.items[0];
						detailPanel.update(data);
						
						var toggle = self.items.items[0].items.items[1].items.items[0]; 
						toggle.setValue(data.level);	
						
                        zvsMobile.mainTabPanel.setLoading(false);
                    }                    
                });
				self.show();
			},	
			UpdateLevel: function (value, ignoreToggle) {	
				Ext.get('level_switch_details').dom.innerHTML = value > 0 ? 'ON' : 'OFF';				
				Ext.get('level_switch_img').dom.className =  value > 0 ? 'imageholder SWITCH_ON' : 'imageholder SWITCH_OFF'
				
				//Update the store 
				var data = zvsMobile.DeviceStore.data.items;
				for (i = 0, len = data.length; i < len; i++) {
					if(data[i].data.id === self.items.items[0].items.items[0].data.id)
					{
							data[i].data.level_txt = value > 0 ? 'ON' : 'OFF'; 
							data[i].data.on_off = value > 0 ? 'ON' : 'OFF'; 
							data[i].data.level = value
					}				
				}
				zvsMobile.DeviceStore.loadData(data);
				
				if(!ignoreToggle)
				{
					var toggle = self.items.items[0].items.items[1].items.items[0];
					toggle.setValue(value);
				}
			},
            listeners: {
						scope: this,
						activate: function () {
						   userChange = true; 
						},
						deactivate: function () {							
							userChange = false; 							
						}
			},
			items: {
                xtype: 'panel',
                items: [ 
							{ 
								xtype: 'panel',
								tpl: new Ext.XTemplate(
								'<div class="device_info">',
								'<div id="level_switch_img" class="imageholder {type}_{on_off}"></div>',
								'<div id="level_switch_details" class="level">{level_txt}</div>',								
									'<h1>{name}</h1>',
									'{type_txt}</br></br>',		
									'Groups: {groups}</br>',								
									'{last_heard_from}</br>',																
								'</div>'), 
							},							
							{
								xtype: 'fieldset',
								defaults: {
									labelAlign: 'right'
								},								
								items: [
										{	
											xtype: 'togglefield',											
											name:'toggle',
											label: 'Off | On',
											listeners: {
												scope: this,
												change: function (toggle, thumb, value, prevValue) {
												   if(userChange)
												   {													    
													   self.UpdateLevel(value, true);
													   self.setLoading(true);
													    Ext.util.JSONP.request({
														url: 'http://10.1.0.55:9999/JSON/SendCmd',
														callbackKey: 'callback',
														params: {
															u: Math.random(),
															id: self.items.items[0].items.items[0].data.id,
															cmd: value > 0 ? 'TURNON' : 'TURNOFF' ,
															arg: 0,
															type: 'device_type'
											
														},
														callback: function (data) {
															
															if(data.status === 'OK')
															{
															  console.log('OK');
															  self.setLoading(false);
															}
															else
															{
																console.log('ERROR');
																self.setLoading(false);
															}
														}                    
													});
												   }
												},
												drag: function (toggle, thumb, value) {
													//We are dragging so ignore changes so we dont send multiple cmds
													userChange = false; 
												},
												dragend: function (toggle, thumb, value) {
													userChange = true;													
												    self.UpdateLevel(value);
												}
											},
										
										}											
										]
							}
                ]
            }           
            
        });
    
        zvsMobile.DeviceDetailsSwitch.superclass.initComponent.apply(this, arguments);
    }
});
 Ext.reg('DeviceDetailsSwitch',  zvsMobile.DeviceDetailsSwitch);