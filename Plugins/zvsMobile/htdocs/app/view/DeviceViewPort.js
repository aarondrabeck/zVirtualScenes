 Ext.define('zvsMobile.view.DeviceViewPort', {
	extend: 'Ext.Panel',
	alias: 'widget.DeviceViewPort',
	initialize: function() {
			var self = this;			
			this.callParent();
	},
	config: 
	{		
		layout: 'card',
		items: [   
					{				
						xtype:'DeviceList'
					},
					{
						xtype:'DeviceDetailsDimmer'							
					},
                    {
                       xtype:'DeviceDetailsSwitch'
                    }
		],		
	}		
});