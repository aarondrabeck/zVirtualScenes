Ext.require(['Ext.Panel', 'zvsMobile.view.SceneList', 'zvsMobile.view.SceneDetails'], function () {
    Ext.define('zvsMobile.view.SceneViewPort', {
        extend: 'Ext.Panel',
        alias: 'widget.SceneViewPort',
        initialize: function () {
            this.callParent(arguments);
            this.getEventDispatcher().addListener('element', '#SceneViewPort', 'swipe', this.onTouchPadEvent, this);
        },
        onTouchPadEvent: function (e, target, options, eventController) {
            if (e.direction === 'right' && e.distance > 50) {
                zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(0));
            }
            else if (e.direction === 'left' && e.distance > 50) {
                zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(2));
            }
        },
        config: {
            layout: {
                type: 'card',
                animation: {
                    type: 'slide',
                    direction: 'left'
                }
            },
            items: [{
                xtype: 'SceneList',
                id: 'SceneList'
            }, {
                xtype: 'SceneDetails'
            }
		]
        }
    });
});