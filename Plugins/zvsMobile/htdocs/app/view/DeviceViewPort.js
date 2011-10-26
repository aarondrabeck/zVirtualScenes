 Ext.define('zvsMobile.view.DeviceViewPort', {
	extend: 'Ext.Panel',
	alias: 'widget.DeviceViewPort',	
	config: 
	{		
		layout: {
            type: 'card',
            animation: {
                type: 'slide',
                direction: 'left'
            }
        },
		items: [   
					{				
						xtype:'DeviceList'
					},
					{
						xtype:'DeviceDetailsDimmer'							
					},
                    {
                       xtype:'DeviceDetailsSwitch'
                    },
                    {
                       xtype:'DeviceDetailsTemp'
                    }
		],		
	}		
});