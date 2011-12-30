Ext.require(['Ext.data.Model'], function () {
    Ext.define('zvsMobile.model.Group', {
        extend: 'Ext.data.Model',
        fields: ['id', 'name', 'count']
    });
});