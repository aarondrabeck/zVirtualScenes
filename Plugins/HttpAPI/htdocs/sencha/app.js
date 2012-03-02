Ext.Loader.setPath({
    'zvsMobile': 'app'
});


Ext.Loader.setPath({
    'zvsMobile': 'app'
});


Ext.application({
    name: 'zvsMobile',

    // Setup the icons and startup images for the example
    icon: '/sencha/resources/img/icon.png',
    tabletStartupScreen: '/sencha/resources/img/tablet_startup.png',
    phoneStartupScreen: '/sencha/resources/img/phone_startup.png',
    glossOnIcon: false,

    profiles: ['Phone', 'Tablet'],
    stores: ['Settings', 'Devices', 'Groups', 'Scenes'],
    views: ['SettingsViewPort'],

    listeners: {
        ShowLoginScreen: function () {
            var settings = zvsMobile.tabPanel.items.items[4];
            settings.items.items[2].fireEvent('loggedOut');

        }
    },
    BaseURL: function () {
        appSettingsStore = Ext.getStore('appSettingsStore');

        var BaseURLSetting = appSettingsStore.findRecord('SettingName', 'BaseURL');
        if (BaseURLSetting === null) {
            return '';
        }
        else {
            return BaseURLSetting.data.Value;
        }
    },
    SetStoreProxys: function () {
        DeviceStore = Ext.getStore('DeviceStore');
        DeviceStore.setProxy({
            type: 'jsonp',
            url: zvsMobile.app.BaseURL() + '/Devices/',
            extraParams: {
                u: Math.random()
            },
            reader: {
                type: 'json',
                rootProperty: 'devices',
                idProperty: 'id',
                successProperty: 'success'
            },
            callbackParam: 'callback'
        });
        
        GroupStore = Ext.getStore('GroupStore');
        GroupStore.setProxy({
            type: 'jsonp',
            url: zvsMobile.app.BaseURL() + '/Groups/',
            extraParams: {
                u: Math.random()
            },
            reader: {
                type: 'json',
                rootProperty: 'groups',
                idProperty: 'id',
                successProperty: 'success'
            },
            callbackParam: 'callback'
        });

        SceneStore = Ext.getStore('SceneStore');
        SceneStore.setProxy({
            type: 'scripttag',
            url: zvsMobile.app.BaseURL() + '/Scenes/',
            extraParams: {
                u: Math.random()
            },
            reader: {
                type: 'json',
                rootProperty: 'scenes',
                idProperty: 'id',
                successProperty: 'success'
            },

            callbackParam: 'callback'
        });
    }
});



 
 
 

 
 


	
	