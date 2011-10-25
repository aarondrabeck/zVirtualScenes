Ext.define('zvsMobile.view.SceneViewPort', {
	extend: 'Ext.Panel',
	alias: 'widget.SceneViewPort',
	initialize: function() {
			var self = this;	
			this.callParent();
	},
	config: 
	{
		layout: 'fit',
		items: [   
					{								
						xtype:'SceneList'						
					}
		],		
	}		
});