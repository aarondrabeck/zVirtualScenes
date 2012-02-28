Ext.define('zvsMobile.store.Scenes', {
    extend: 'Ext.data.Store',        
    requires: ['zvsMobile.model.Scene'],
	config: {
		model: 'zvsMobile.model.Scene',
		proxy: {
			type: 'scripttag',
			url: zvsMobile.app.APIURL + '/Scenes/',
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
		}
	}
});

SceneStore = Ext.create('zvsMobile.store.Scenes', {
    id: 'SceneStore',
    requires: ['zvsMobile.store.Scenes']
});



