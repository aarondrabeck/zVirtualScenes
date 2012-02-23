Ext.define('zvsMobile.view.GroupViewPort', {
        extend: 'Ext.Panel',
        xtype: 'GroupViewPort',
        requires: ['zvsMobile.view.DeviceList',
                      'zvsMobile.view.GroupList',
                      'zvsMobile.view.GroupDetails'],
        initialize: function () {
            this.callParent(arguments);
            this.getEventDispatcher().addListener('element', '#GroupViewPort', 'swipe', this.onTouchPadEvent, this);
        },
        onTouchPadEvent: function (e, target, options, eventController) {
            if (e.direction === 'right' && e.distance > 50) {
                zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(1));
            }
            else if (e.direction === 'left' && e.distance > 50) {
                zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(3));
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
                xtype: 'GroupList',
                id: 'GroupList'
            }, {
                xtype: 'GroupDetails'
            }

		]
        }
    });