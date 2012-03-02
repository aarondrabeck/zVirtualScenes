Ext.define('zvsMobile.model.Settings', {
    extend: 'Ext.data.Model',

    config: {
        fields: [
            { name: "id" },
            { name: "SettingName", type: "string" },
            { name: "Value", type: "string" }
        ],
        proxy: {
            type: 'localstorage',
            id: 'zvs-settings'
        }

    }
});