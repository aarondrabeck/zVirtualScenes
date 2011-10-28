Ext.require(['Ext.Panel', 'zvsMobile.view.SceneList', 'zvsMobile.view.SceneDetails'], function () {
    Ext.define('zvsMobile.view.SceneViewPort', {
        extend: 'Ext.Panel',
        alias: 'widget.SceneViewPort',        
        config:
	{
	    layout: {
	        type: 'card',
	        animation: {
	            type: 'slide',
	            direction: 'left'
	        }
	    },
	    items: [{
	        xtype: 'SceneList'
	    }, {
	        xtype: 'SceneDetails'
	    }
		]
	}
    });
});