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
				tabBar: {
					dock: 'bottom',
					layout: {
						pack: 'center'
					}
				},
				items: [ {
							xtype: 'DeviceViewPort',
							title: "Devices",
							iconCls: "bulb"							
				},
				{
							xtype: 'SceneViewPort',
							title: "Scenes",
							iconCls: "equalizer2"							
				},
				
				]
			});
			
			var deviceList = zvsMobile.mainTabPanel.items.items[0].items.items[0].items.items[0];
			var sceneList = zvsMobile.mainTabPanel.items.items[1].items.items[0].items.items[0];
			deviceList.AjaxLoad();
			sceneList.AjaxLoad();
        }
});
 
 
 
 

 
 


	
	