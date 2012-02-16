Ext.require(['Ext.data.Store'], function () {
    Ext.define('zvsMobile.store.Groups', {
        extend: 'Ext.data.Store',        
        requires: ['zvsMobile.model.Group'],		
		config: {
			model: 'zvsMobile.model.Group',
			proxy: {
				type: 'scripttag',
				url: '/API/Groups/',
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

    GroupsStore = Ext.create('zvsMobile.store.Groups', {
    });
});

