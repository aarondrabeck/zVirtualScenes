Ext.define('zvsMobile.view.SceneViewPort', {
	extend: 'Ext.Panel',
	alias: 'widget.SceneViewPort',
	initialize: function() {
			var self = this;	
			this.callParent();
	},
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
						xtype:'SceneList'						
					},
                    {
                        xtype:'SceneDetails'
                    }
		],		
	}		
});