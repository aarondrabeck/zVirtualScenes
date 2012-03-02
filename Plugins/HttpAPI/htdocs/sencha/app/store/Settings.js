Ext.define('zvsMobile.store.Settings', {
    extend: 'Ext.data.Store',
    requires: ['zvsMobile.model.Settings', 'Ext.data.Batch'],
    config: {
        model: 'zvsMobile.model.Settings',
        autoLoad: true
     }
});

Ext.create('zvsMobile.store.Settings', {
    id: 'appSettingsStore',
    model: 'zvsMobile.model.Settings'
});
