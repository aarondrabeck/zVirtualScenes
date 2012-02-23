Ext.define('zvsMobile.store.Devices', {
    extend: 'Ext.data.Store',        
    requires: ['zvsMobile.model.Device'],
	config: {
		model: 'zvsMobile.model.Device',
		proxy: {
			type: 'jsonp',
			url: '/API/Devices/',
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
		}
		//autoLoad: true
		}
});

DeviceStore = Ext.create('zvsMobile.store.Devices', {
 requires: ['zvsMobile.store.Devices']
});


