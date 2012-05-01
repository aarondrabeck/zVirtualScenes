Ext.define('zvsMobile.view.phone.Main', {
    extend: 'zvsMobile.view.Main',
    xtype: 'mainview',

    config: {
        fullscreen: true,
        tabBar: {
            docked: 'bottom',
            layout: {
                pack: 'center'
            }
        },
        items: [{
            xtype: 'DevicePhoneViewPort',
            id: 'DevicePhoneViewPort',
            title: 'Devices',
            iconCls: "bulb"
        }, {
            xtype: 'ScenePhoneViewPort',
            id: 'ScenePhoneViewPort',
            title: "Scenes",
            iconCls: "equalizer2"

        }, {
            xtype: 'GroupPhoneViewPort',
            id: 'GroupPhoneViewPort',
            title: "Groups",
            iconCls: "spaces2"
        }, {
            xtype: 'SettingsViewPort',
            id: 'SettingsViewPort',
            title: 'Settings',
            iconCls: 'settings'
        }]
    }
});






