Ext.define('zvsMobile.view.phone.GroupPhoneViewPort', {
    extend: 'Ext.Panel',
    xtype: 'GroupPhoneViewPort',
    requires: ['zvsMobile.view.DeviceList',
                      'zvsMobile.view.GroupList',
                      'zvsMobile.view.GroupDetails'],
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
            id: 'GroupList',
            listeners: {
                scope: this,
                selectionchange: function (list, records) {
                    if (records[0] !== undefined) {
                        var GroupDetails = Ext.getCmp('GroupDetails');
                        GroupDetails.loadGroup(records[0].data.id);

                        var GroupPhoneViewPort = Ext.getCmp('GroupPhoneViewPort');
                        GroupPhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'left' });
                        GroupPhoneViewPort.setActiveItem(Ext.getCmp('GroupDetailContainer'));
                    }
                },
                activate: function () {
                    Ext.getCmp('GroupList').deselectAll();
                }
            }
        }, {
            id: 'GroupDetailContainer',
            layout: 'card',
            items: [{
                xtype: 'GroupDetails',
                id: 'GroupDetails'
            }, {
                xtype: 'toolbar',
                docked: 'top',
                title: 'Group Details',
                scrollable: false,
                items: [{
                    xtype: 'button',
                    iconMask: true,
                    ui: 'back',
                    text: 'Back',
                    handler: function () {
                        var GroupPhoneViewPort = Ext.getCmp('GroupPhoneViewPort');
                        GroupPhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                        GroupPhoneViewPort.setActiveItem(Ext.getCmp('GroupList'));
                    }
                }]
            }]
        }]
    }
});