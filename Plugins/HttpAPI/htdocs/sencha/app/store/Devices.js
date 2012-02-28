Ext.define('zvsMobile.store.Devices', {
    extend: 'Ext.data.Store',        
    requires: ['zvsMobile.model.Device'],
	config: {
		model: 'zvsMobile.model.Device',
		proxy: {
			type: 'jsonp',
			url: zvsMobile.app.APIURL + '/Devices/',
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

var DeviceStore = Ext.create('zvsMobile.store.Devices', {
    id: 'DeviceStore',
    requires: ['zvsMobile.store.Devices']
});



