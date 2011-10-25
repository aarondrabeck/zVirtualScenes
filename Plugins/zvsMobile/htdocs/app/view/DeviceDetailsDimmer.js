Ext.define('zvsMobile.view.DeviceDetailsDimmer', {
	extend: 'Ext.Panel',
	alias: 'widget.DeviceDetailsDimmer',

    constructor: function (config) {
         var self = this;        
         var FirstTime = true; 
         var suspendAJAX = false;
         var timer;
         Ext.apply(config || {}, {
             loadDevice: function (deviceId) {	
                       //Get Device Details				       
				       Ext.util.JSONP.request({
                        url: 'http://10.1.0.55:9999/JSON/GetDeviceDetails',
                        callbackKey: 'callback',
                        params: {
                            u: Math.random(),
						    id: deviceId
                        },
                        callback: function (data) {                     
                            //Send data to panel TPL
                            self.items.items[0].items.items[1].setData(data);
						    
                            //Update meter levels to reflect this devices current state but don't send that change back as a AJAX cmd
                            self.UpdateLevel(data.level);
                            
                            suspendAJAX = true;
                            var slider = self.items.items[0].items.items[2].items.items[0];	
				            slider.setValue(data.level);
                        }                    
                    });				
	        },
             UpdateLevel: function (value) { 
                  var level = value > 99 ? 99 : value;  
                  
                  //Update panel TPL        
                  var data = Ext.clone(self.items.items[0].items.items[1].getData());
                  data.level = level; 
                  data.level_txt = level + '%';
                  data.on_off = 'DIM';
                  if(level === 0) {
					 data.on_off = 'OFF';  } 
                  else if(level > 98) {
					 data.on_off = 'ON'; 
				  }
                  self.items.items[0].items.items[1].setData(data); 
								
				 //Update the store 
				 var data = Ext.clone(DeviceStore.data.items);
				 for (i = 0, len = data.length; i < len; i++) {
					 if(data[i].data.id === self.items.items[0].items.items[1]._data.id)
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
				 DeviceStore.loadRecords(data);
                 self.parent.items.items[0].refresh();

			 },
             listeners: {
						 scope: this,
						 activate: function () {
						    FirstTime = true; 
						 }
					 },
         items: {
                xtype: 'panel',
                items: [ 
                            {
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
											    DeviceViewPort.setActiveItem(DeviceViewPort.items.items[0], { type: 'slide', reverse: true });			
										    }
								       }]
                            },
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
												change: function (slider, thumb, value) {     
                                                    
                                                   //console.log('change' + value + '. FirstTime: ' + FirstTime + '. suspendAJAX:' + suspendAJAX);  
												   if(!FirstTime && !suspendAJAX) //Ignore the first event because it was triggered on render.
												   {	
                                                        var level = value > 99 ? 99 : value;		
                                                        //Use a timer to prevent too many calls...
                                                        if (timer) { clearInterval(timer); }

                                                        //Contuniously recall the timer while dragged.
                                                        timer = setTimeout(function () {
                                                            var id = self.items.items[0].items.items[1].getData().id;
                                                            console.log('DimmerID '+id+' Change AJAX CMD SENT, change to ' + level);
                                                                                                            
                                                            Ext.util.JSONP.request({
														    url: 'http://10.1.0.55:9999/JSON/SendCmd',
														    callbackKey: 'callback',
														    params: {
															    u: Math.random(),
															    id: id,
															    cmd: 'DYNAMIC_CMD_LEVEL',
															    arg: level,
															    type: 'device'
											
														        },
														        callback: function (data) {
															
															        if(data.success)
															        {
															          console.log('OK');															          
															        }
															        else
															        {
																        console.log('ERROR');
															        }
														        }                    
													        });
                                                        }, 500);
                                                        
                                                       self.UpdateLevel(value);									    
													    
													    
													    
												   }
                                                   FirstTime = false;
                                                   suspendAJAX = false;   												
                                                }

											},
											
										}																				
										]
							}
                ]
            }
         });
         this.callOverridden([config]);
    },	
	config: 
	{
        layout: 'fit'		
	}		
});