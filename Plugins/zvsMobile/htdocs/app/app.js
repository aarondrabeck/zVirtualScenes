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
        Ext.require(['Ext.tab.Panel', 'zvsMobile.view.DeviceViewPort', 'zvsMobile.view.SceneViewPort', 'zvsMobile.view.GroupViewPort'], function () {
            var tabpanel = Ext.create('Ext.tab.Panel', {
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
                    title: 'Settings',
                    html: '<h1>Settings Card</h1>',
                    cls: 'card4',
                    iconCls: 'settings'
                }


                ]
            });
        });
    }
});


 
 
 

 
 


	
	