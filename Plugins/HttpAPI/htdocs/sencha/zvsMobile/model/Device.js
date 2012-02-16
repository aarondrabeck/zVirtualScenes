Ext.define('zvsMobile.model.Device', {
    extend: 'Ext.data.Model',

    config: {
        fields: ['id', 'name', 'on_off', 'level', 'level_txt', 'type']
    }
});