Ext.define('zvsMobile.view.phone.ScenePhoneViewPort', {
    extend: 'Ext.Panel',
    xtype: 'ScenePhoneViewPort',
    requires: ['zvsMobile.view.SceneList', 'zvsMobile.view.SceneDetails'],
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
            id: 'SceneList',
            listeners: {
                scope: this,
                selectionchange: function (list, records) {
                    if (records[0] !== undefined) {
                        var SceneDetails = Ext.getCmp('SceneDetails');
                        SceneDetails.loadScene(records[0].data.id);

                        var ScenePhoneViewPort = Ext.getCmp('ScenePhoneViewPort');
                        ScenePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'left' });
                        ScenePhoneViewPort.setActiveItem(Ext.getCmp('SceneDetailContainer'));
                    }
                },
                activate: function () {
                    Ext.getCmp('DeviceList').deselectAll();
                }
            }
        }, {
            id: 'SceneDetailContainer',
            layout: 'card',
            items: [{
                xtype: 'SceneDetails',
                id: 'SceneDetails'
            }, {
                xtype: 'toolbar',
                docked: 'top',
                title: 'Scene Details',
                scrollable: false,
                items: [{
                    xtype: 'button',
                    iconMask: true,
                    ui: 'back',
                    text: 'Back',
                    handler: function () {
                        var ScenePhoneViewPort = Ext.getCmp('ScenePhoneViewPort');
                        ScenePhoneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                        ScenePhoneViewPort.setActiveItem(Ext.getCmp('SceneList'));
                    }
                }]
            }]
        }]
    }
});