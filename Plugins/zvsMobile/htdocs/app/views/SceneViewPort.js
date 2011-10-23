	
 zvsMobile.SceneViewPort = Ext.extend(Ext.Panel, {
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
							title: "Scenes",
							items: [{ 
										xtype: 'button',
										iconMask: true,
										iconCls: 'refresh',
										handler: function () { 
											var sceneList = self.items.items[0].items.items[0]; 
											sceneList.AjaxLoad();
										}
								   }]
						},
						items: [{
									xtype: 'SceneList',
									scroll: 'veritcal',						 
									store:  zvsMobile.SceneStore,
									listeners: {
										scope: this,		
										selectionchange: function (list, records) {
											if (records[0] !== undefined) {	
												var sceneId = records[0].data.id;
												Ext.Msg.confirm('Acitvate Scene', 'Are you sure you want to activate &quot;' + records[0].data.name + '&quot;?', 
												function (choice) {
													if(choice === 'yes')
													{
														Ext.util.JSONP.request({
														url: 'http://10.1.0.55:9999/JSON/ActivateScene',
														callbackKey: 'callback',
														params: {
															u: Math.random(),
															id: sceneId
														},
														callback: function (data) {	
															if(data.status === 'OK')
															{
																console.log(data.desc);
															}
															zvsMobile.mainTabPanel.setLoading(false);
														}                    
														});	
													}	
													var sceneList = self.items.items[0].items.items[0];							
													sceneList.getSelectionModel().deselectAll();														
												}
												);												
											}		
										}			
									}
						}]
					}				
					
					]
	
		});
		zvsMobile.SceneViewPort.superclass.initComponent.apply(this, arguments);
	}
});
Ext.reg('SceneViewPort', zvsMobile.SceneViewPort);
