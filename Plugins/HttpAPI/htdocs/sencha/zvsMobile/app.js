Ext.Loader.setPath('Ext', 'touch/Ext');

//       <script type="text/javascript" src="app/view/SceneList.js"></script>
//	 <script type="text/javascript" src="app/view/SceneViewPort.js"></script>
//	 <script type="text/javascript" src="app/view/DeviceViewPort.js"></script>
//	 <script type="text/javascript" src="app/view/DeviceList.js"></script>
//	 <script type="text/javascript" src="app/view/DeviceDetailsDimmer.js"></script>
//    <script src="app/view/DeviceDetailsThermo.js" type="text/javascript"></script>
//     <script src="app/view/DeviceDetailsSwitch.js" type="text/javascript"></script>
//    <script src="app/view/SceneDetails.js" type="text/javascript"></script>

Ext.application({
    name: 'zvsMobile',
    launch: function () {
        Ext.require(['Ext.tab.Panel', 'zvsMobile.view.DeviceViewPort', 'zvsMobile.view.SceneViewPort', 'zvsMobile.view.GroupViewPort', 'zvsMobile.view.SettingsViewPort'], function () {
            zvsMobile.tabPanel = Ext.create('Ext.tab.Panel', {
                fullscreen: true,
                tabBar: {
                    docked: 'bottom',
                    layout: {
                        pack: 'center'
                    }
                },
                items: [{
                    xtype: 'DeviceViewPort',
                    title: 'Devices',
                    iconCls: "bulb"
                }, {
                    xtype: 'SceneViewPort',
                    title: "Scenes",
                    iconCls: "equalizer2"

                }, {
                    xtype: 'GroupViewPort',
                    title: "Groups",
                    iconCls: "spaces2"
                }, {
                    xtype: 'SettingsViewPort',
                    title: 'Settings',                    
                    iconCls: 'settings'
                }]
            });

            //see if they are logged in 

            Ext.Ajax.request({
                url: '/API/login',
                method: 'GET',
                params: {
                    u: Math.random()
                },
                success: function (response, opts) {
                    var result = JSON.parse(response.responseText);                   

                    if (result.success && result.isLoggedIn) {
                      var settings = zvsMobile.tabPanel.items.items[4];
                      settings.items.items[1].fireEvent('loggedIn');                       
                    }
                    else {
                        var settings = zvsMobile.tabPanel.items.items[4];
                        zvsMobile.tabPanel.setActiveItem(settings);
                        settings.items.items[2].fireEvent('loggedOut');
                    }
                },
                failure: function (result, request) {
                    var settings = zvsMobile.tabPanel.items.items[4];
                    zvsMobile.tabPanel.setActiveItem(settings);
                    settings.items.items[2].fireEvent('loggedOut');
                }
            });
        });
    }
});


 
 
 

 
 


	
	