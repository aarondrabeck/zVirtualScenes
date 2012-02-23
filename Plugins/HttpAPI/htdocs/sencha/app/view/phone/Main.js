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
            xtype: 'DeviceViewPort',
            id: 'DeviceViewPort',
            title: 'Devices',
            iconCls: "bulb"
        }, {
            xtype: 'SceneViewPort',
            id: 'SceneViewPort',
            title: "Scenes",
            iconCls: "equalizer2"

        }, {
            xtype: 'GroupViewPort',
            id: 'GroupViewPort',
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






