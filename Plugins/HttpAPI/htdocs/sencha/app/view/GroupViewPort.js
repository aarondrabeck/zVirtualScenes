Ext.require(['Ext.Panel', 'zvsMobile.view.GroupList', 'zvsMobile.view.GroupDetails'], function () {
    Ext.define('zvsMobile.view.GroupViewPort', {
        extend: 'Ext.Panel',
        xtype: 'GroupViewPort',
        config: {
            layout: {
                type: 'card',
                animation: {
                    type: 'slide',
                    direction: 'left'
                }
            },
            items: [{
                xtype: 'GroupList'
            }, {
                xtype: 'GroupDetails'
            }

		]
        }
    });
});