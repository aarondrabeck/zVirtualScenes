zvsMobile.DeviceDetailsDimmer = Ext.extend(Ext.Panel, {
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
                        self.items.items[0].items.items[0].update(data);
						self.UpdateLevel(data.level, false);						
                        zvsMobile.mainTabPanel.setLoading(false);
                    }                    
                });
				self.show();
			},	
			UpdateLevel: function (value, ignoreSlider) {			
				Ext.get('level_dimmer_details').dom.innerHTML = value + '%';
				
				var ImgCls = 'imageholder DIMMER_DIM';
				if(value === 0) {
					ImgCls = 'imageholder DIMMER_OFF'; 
				} else if(value > 98) {
					ImgCls = 'imageholder DIMMER_ON'; 
				}				
				Ext.get('level_dimmer_img').dom.className =  ImgCls;
								
				//Update the store 
				var data = zvsMobile.DeviceStore.data.items;
				for (i = 0, len = data.length; i < len; i++) {
					if(data[i].data.id === self.items.items[0].items.items[0].data.id)
					{
							data[i].data.level_txt = value + '%'; 
							data[i].data.level = value;
							
							if(value === 0) {
								data[i].data.on_off = 'OFF';
							}
							else if(value > 98) {
								data[i].data.on_off = 'ON';
							}
							else {
								data[i].data.on_off = 'DIM';
							}					
							
					}				
				}
				zvsMobile.DeviceStore.loadData(data);
				
				if(!ignoreSlider)
				{
					var slider = self.items.items[0].items.items[1].items.items[0];	
					slider.setValue(value);
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
								'<div id="level_dimmer_img" class="imageholder {type}_{on_off}"></div>',
								'<div id="level_dimmer_details" class="level">{level_txt}</div>',								
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
											xtype: 'sliderfield',
											name:'level',
											label: 'Set Level',
											minValue: 0,
											maxValue: 99,
											listeners: {
												scope: this,
												change: function (slider, thumb, value, prevValue) {
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
															cmd: 'DYNAMIC_CMD_LEVEL',
															arg: value,
															type: 'device'
											
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
												drag: function (slider, thumb, value) {
													//We are dragging so ignore changes so we dont send multiple cmds
													userChange = false; 
												},
												dragend: function (slider, thumb, value) {
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
    
        zvsMobile.DeviceDetailsDimmer.superclass.initComponent.apply(this, arguments);
    }
});
 Ext.reg('DeviceDetailsDimmer',  zvsMobile.DeviceDetailsDimmer);