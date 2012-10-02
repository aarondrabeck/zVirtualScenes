Ext.define('zvsMobile.view.tablet.Main', {
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
            xtype: 'DeviceTabletViewPort',
            id: 'DeviceTabletViewPort',
            title: 'Devices',
            iconCls: "bulb"
        }, {
            xtype: 'SceneTabletViewPort',
            id: 'SceneTabletViewPort',
            title: "Scenes",
            iconCls: "equalizer2"

        }, {
            xtype: 'GroupTabletViewPort',
            id: 'GroupTabletViewPort',
            title: "Groups",
            iconCls: "groups"
        }, {
            xtype: 'LogViewPort',
            id: 'LogViewPort',
            title: "Activity Log",
            iconCls: "log"
        }, {
            xtype: 'SettingsViewPort',
            id: 'SettingsViewPort',
            title: 'Settings',
            iconCls: 'settings'
        }]
    }
});






