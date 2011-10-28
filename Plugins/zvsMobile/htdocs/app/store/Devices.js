Ext.require(['Ext.data.Store'], function () {
    Ext.define('zvsMobile.store.Devices', {
        extend: 'Ext.data.Store',
        model: 'zvsMobile.model.Device',
        requires: ['zvsMobile.model.Device'],

        config: {
            proxy: {
                type: 'scripttag',
                url: 'http://10.1.0.56:9999/JSON/GetDeviceList',
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
        }
    });
});