Ext.require(['Ext.tab.Panel']);

Ext.application({
    name: 'zvsMobile',
	
	launch: function () 
		{
			var tabpanel = Ext.create('Ext.tab.Panel', {   
				fullscreen: true,
				tabBar: {
					docked: 'bottom',
					layout: {
						pack: 'center'
					}
				},		
				items: [ 
				//{
							// xtype: 'DeviceViewPort',
							// title: "Devices",
							// iconCls: "bulb"							
				// },
				// {
							// xtype: 'SceneViewPort',
							// title: "Scenes",
							// iconCls: "equalizer2"							
				// },
				{
					id: 'DeviceViewPort',
					xtype: 'DeviceViewPort',
					title: 'Devices',
					iconCls: "bulb"
				},
				{
					xtype: 'SceneViewPort',
					title: "Scenes",
					iconCls: "equalizer2"
				},				
				{
                    title: 'Settings',
                    html: '<h1>Settings Card</h1>',
                    cls: 'card4',
                    iconCls: 'settings'
                },
				
				]
			});
		}
    
});


 
 
 

 
 


	
	