Ext.regModel('Device', {
    feilds: ['id', 'name', 'on_off', 'level', 'level_txt', 'type']
});

zvsMobile.DeviceStore = new Ext.data.Store({
        model: 'Device',
        remoteFilter: true

});	
	
 zvsMobile.DeviceList = Ext.extend(Ext.List, {
		AjaxLoad: function () {
			      zvsMobile.mainTabPanel.setLoading(true);				  
				   Ext.util.JSONP.request({
                    url: 'http://10.1.0.55:9999/JSON/GetDeviceList',
                    callbackKey: 'callback',
                    params: {
                        u: Math.random()
                    },
                    callback: function (data) {
                        zvsMobile.DeviceStore.loadData(data);                        
                        zvsMobile.mainTabPanel.setLoading(false);
						console.log('Devices Reloaded');
                    }                    
                });				 
		},

        cls: 'DeviceListItem',
        loadingText: 'Loading...',
        emptyText: '<div class="emptyText">Loading...</div>',
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
        scroll: false
    });
 Ext.reg('DeviceList',  zvsMobile.DeviceList);