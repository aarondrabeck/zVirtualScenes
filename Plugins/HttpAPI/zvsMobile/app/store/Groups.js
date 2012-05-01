Ext.define('zvsMobile.store.Groups', {
    extend: 'Ext.data.Store',        
    requires: ['zvsMobile.model.Group'],		
	config: {
		model: 'zvsMobile.model.Group'
	}
});


var GroupStore = Ext.create('zvsMobile.store.Groups', {
    id: 'GroupStore',
    requires: ['zvsMobile.store.Groups']
});

