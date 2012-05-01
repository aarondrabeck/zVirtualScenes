Ext.define('zvsMobile.store.Devices', {
    extend: 'Ext.data.Store',
    requires: ['zvsMobile.model.Device'],
    config: {
        model: 'zvsMobile.model.Device'
    }
});

var DeviceStore = Ext.create('zvsMobile.store.Devices', {
    id: 'DeviceStore',
    requires: ['zvsMobile.store.Devices']
});



