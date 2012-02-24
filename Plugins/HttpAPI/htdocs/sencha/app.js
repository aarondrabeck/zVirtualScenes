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

    profiles: ['Phone'],
    stores: ['Devices', 'Groups', 'Scenes'],
    views: ['DeviceViewPort', 'SceneViewPort', 'GroupViewPort', 'SettingsViewPort']   
});


 
 
 

 
 


	
	