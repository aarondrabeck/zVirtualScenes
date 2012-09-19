Ext.define('zvsMobile.store.Scenes', {
    extend: 'Ext.data.Store',        
    requires: ['zvsMobile.model.Scene'],
	config: {
		model: 'zvsMobile.model.Scene'		
	}
});

SceneStore = Ext.create('zvsMobile.store.Scenes', {
    id: 'SceneStore',
    requires: ['zvsMobile.store.Scenes']
});



