//Ext.require(['Ext.data.Store'], function () {
    console.log('2');
    window.SceneStore = Ext.create('Ext.data.Store', {
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
    console.log('2');
    //zvsMobile.DeviceStore = Ext.create('zvsMobile.store.Device', {
    //});
//});