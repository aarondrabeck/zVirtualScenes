Ext.require(['Ext.data.Store'], function () {
    Ext.define('zvsMobile.store.Groups', {
        extend: 'Ext.data.Store',
        model: 'zvsMobile.model.Group',
        requires: ['zvsMobile.model.Group'],
        proxy: {
            type: 'scripttag',
            url: 'http://10.1.0.56:9999/JSON/GetGroupList',
            extraParams: {
                u: Math.random()
            },
            reader: {
                type: 'json',
                root: 'groups',
                idProperty: 'id',
                successProperty: 'success'
            },
            callbackParam: 'callback'
        },
        autoLoad: true
    });

    GroupsStore = Ext.create('zvsMobile.store.Groups', {
    });
});

