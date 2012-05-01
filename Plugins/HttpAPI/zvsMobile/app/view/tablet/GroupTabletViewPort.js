Ext.define('zvsMobile.view.tablet.GroupTabletViewPort', {
    extend: 'Ext.Panel',
    xtype: 'GroupTabletViewPort',
    requires: ['zvsMobile.view.DeviceList',
                'zvsMobile.view.GroupList',
                'zvsMobile.view.GroupDetails'],
    config: {
        layout: 'hbox',
        items: [{
            xtype: 'GroupList',
            id: 'GroupList',
            flex: 1,
            listeners: {
                scope: this,
                selectionchange: function (list, records) {
                    if (records[0] !== undefined) {
                        var GroupDetails = Ext.getCmp('GroupDetails');
                        GroupDetails.loadGroup(records[0].data.id);

                        var groupDetailsPane = Ext.getCmp('groupDetailsPane');
                        groupDetailsPane.getLayout().setAnimation({ type: 'slide', direction: 'up' });
                        groupDetailsPane.setActiveItem(GroupDetails);
                    }
                }
            }
        }, {
            flex: 2,
            id: 'groupDetailsPane',
            layout: {
                type: 'card',
                animation: {
                    type: 'slide',
                    direction: 'left'
                }
            },
            items: [{
                cls: 'emptyDetail',
                html: "Select a group to see more details."
            }, {
                xtype: 'GroupDetails',
                id: 'GroupDetails'
            }, {
                xtype: 'toolbar',
                docked: 'top',
                title: 'Group Details'
            }]
        }
		]
    }
});  