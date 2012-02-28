Ext.define('zvsMobile.store.Groups', {
    extend: 'Ext.data.Store',        
    requires: ['zvsMobile.model.Group'],		
	config: {
		model: 'zvsMobile.model.Group',
		proxy: {
			type: 'scripttag',
			url: zvsMobile.app.APIURL + '/Groups/',
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
		}
	}
});


var GroupStore = Ext.create('zvsMobile.store.Groups', {
    id: 'GroupStore',
    requires: ['zvsMobile.store.Groups']
});

