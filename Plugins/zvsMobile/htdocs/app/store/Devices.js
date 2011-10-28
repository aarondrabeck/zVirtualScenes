Ext.require(['zvsMobile.model.Device', 'Ext.data.Store'], function () {
    Ext.define('zvsMobile.store.Devices', {
        extend: 'Ext.data.Store',
        model: 'zvsMobile.model.Device',
       // requires: [],

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