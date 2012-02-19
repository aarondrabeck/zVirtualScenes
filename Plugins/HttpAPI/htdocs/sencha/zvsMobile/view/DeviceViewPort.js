Ext.require(['Ext.Panel',
'zvsMobile.view.DeviceList',
'zvsMobile.view.DeviceDetailsDimmer',
'zvsMobile.view.DeviceDetailsSwitch',
'zvsMobile.view.DeviceDetailsThermo'],
    function () {
        Ext.define('zvsMobile.view.DeviceViewPort', {
            extend: 'Ext.Panel',
            alias: 'widget.DeviceViewPort',
            initialize: function () {
                this.callParent(arguments);
                this.getEventDispatcher().addListener('element', '#DeviceViewPort', 'swipe', this.onTouchPadEvent, this);
            },
            onTouchPadEvent: function (e, target, options, eventController) {
                if (e.direction === 'left' && e.distance > 50) {
                    zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(1));
                }
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
					        xtype: 'DeviceList'
					    }, {
					        xtype: 'DeviceDetailsDimmer'
					    }, {
					        xtype: 'DeviceDetailsSwitch'
					    }, {
					        xtype: 'DeviceDetailsThermo'
					    }
		    ]
	    }
        });
    });