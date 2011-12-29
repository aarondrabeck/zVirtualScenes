Ext.require(['Ext.data.Store'], function () {
    Ext.define('zvsMobile.store.Devices', {
        extend: 'Ext.data.Store',
        model: 'zvsMobile.model.Device',
        requires: ['zvsMobile.model.Device'],
        proxy: {
            type: 'scripttag',
            url: 'http://10.1.0.61/API/Devices/',
            extraParams: {
                u: Math.random()
            },
            reader: {
                type: 'json',
                root: 'devices',
                idProperty: 'id',
                successProperty: 'success'
            },
            callbackParam: 'callback'
        },
        autoLoad: true
    });

    DeviceStore = Ext.create('zvsMobile.store.Devices', {
    });
});

