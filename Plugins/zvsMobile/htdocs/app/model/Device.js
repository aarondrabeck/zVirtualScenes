Ext.require(['Ext.data.Model'], function () {
    Ext.define('zvsMobile.model.Device', {
        extend: 'Ext.data.Model',
        fields: ['id', 'name', 'on_off', 'level', 'level_txt', 'type']
    });
    console.log('1');
});