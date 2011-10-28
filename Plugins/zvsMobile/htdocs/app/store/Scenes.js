Ext.require(['Ext.data.Store'], function () {
    Ext.define('zvsMobile.store.Scenes', {
        extend: 'Ext.data.Store',
        model: 'zvsMobile.model.Scene',
        requires: ['zvsMobile.model.Scene'],
        proxy: {
            type: 'scripttag',
            url: 'http://10.1.0.56:9999/JSON/GetSceneList',
            extraParams: {
                u: Math.random()
            },
            reader: {
                type: 'json',
                root: 'scenes',
                idProperty: 'id',
                successProperty: 'success'
            },

            callbackParam: 'callback'
        },
        autoLoad: true
    });

    SceneStore = Ext.create('zvsMobile.store.Scenes', {
    });
});