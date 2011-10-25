Ext.define('Device', {
 extend: 'Ext.data.Model',
 fields: ['id', 'name', 'on_off', 'level', 'level_txt', 'type'] 
 });

 var DeviceStore = Ext.create('Ext.data.Store', {
	 model: 'Device',
	 proxy: {
			type: 'scripttag',
			url : 'http://10.1.0.56:9999/JSON/GetDeviceList',
			extraParams: {
				u: Math.random()
			},
			reader: {
				type: 'json',
				root: 'devices',
				idProperty: 'id',
				successProperty: 'success'				
			},			            
			callbackParam: 'callback'
		},
	autoLoad: true
 });


 Ext.define('zvsMobile.view.DeviceList', {
     extend: 'Ext.dataview.List',
     alias: 'widget.DeviceList',
     constructor: function (config) {
         var self = this;

         Ext.apply(config || {}, {
         scroll:'vertical',
             items: [{
                 xtype: 'toolbar',
                 docked: 'top',
                 title: 'Devices',
                 items: [{
                     xtype: 'button',
                     iconMask: true,
                     iconCls: 'refresh',
                     handler: function () {
                         DeviceStore.load();
                     }
                 }]
             }],

             listeners:
			{
			    scope: this,
			    selectionchange: function (list, records) {
			        if (records[0] !== undefined) {
			            var DeviceViewPort = self.parent;
			            var DimmmerDetails = DeviceViewPort.items.items[1];
			            var SwitchDetails = DeviceViewPort.items.items[2];

			            if (records[0].data.type === 'DIMMER') {
			                DimmmerDetails.loadDevice(records[0].data.id);
			                DeviceViewPort.setActiveItem(DimmmerDetails);
			            }

			            if (records[0].data.type === 'SWITCH') {
			                SwitchDetails.loadDevice(records[0].data.id);
			                DeviceViewPort.setActiveItem(SwitchDetails);
			            }
			        }
			    },
			    activate: function () {

			        self.deselectAll();
			    }
			}

         });
         this.callOverridden([config]);
     },
     config:
     {
         itemTpl: new Ext.XTemplate(
		            '<div class="device">',
			            '<div class="imageholder {type}_{on_off}"></div>',
			            '<h2>{name}</h2>',
			            '<div class="level">',
				            '<div class="meter">',
					            '<div class="progress" style="width:{level}%">',
					            '</div>',
				            '</div>',
				            '<div class="percent">{level_txt}</div>',
			            '</div>',
		            '</div>'),
         cls: 'DeviceListItem',
         store: DeviceStore
     }
 });

















// Ext.regModel('Device', {
    // feilds: ['id', 'name', 'on_off', 'level', 'level_txt', 'type']
// });

// zvsMobile.DeviceStore = new Ext.data.Store({
        // model: 'Device',
        // remoteFilter: true

// });	
	
 // zvsMobile.DeviceList = Ext.extend(Ext.List, {
		// AjaxLoad: function () {
			      // zvsMobile.mainTabPanel.setLoading(true);				  
				   // Ext.util.JSONP.request({
                    // url: 'http://10.1.0.55:9999/JSON/GetDeviceList',
                    // callbackKey: 'callback',
                    // params: {
                        // u: Math.random()
                    // },
                    // callback: function (data) {
                        // zvsMobile.DeviceStore.loadData(data);                        
                        // zvsMobile.mainTabPanel.setLoading(false);
						// console.log('Devices Reloaded');
                    // }                    
                // });				 
		// },

        // cls: 'DeviceListItem',
        // loadingText: 'Loading...',
        // emptyText: '<div class="emptyText">Loading...</div>',
        // itemTpl: new Ext.XTemplate(
		// '<div class="device">',
            // '<div class="imageholder {type}_{on_off}"></div>',
			// '<h2>{name}</h2>',
			// '<div class="level">',
				// '<div class="meter">',
					// '<div class="progress" style="width:{level}%">',
					// '</div>',
				// '</div>',
				// '<div class="percent">{level_txt}</div>',				
			// '</div>',
		// '</div>'),
        // scroll: false
    // });
 // Ext.reg('DeviceList',  zvsMobile.DeviceList);