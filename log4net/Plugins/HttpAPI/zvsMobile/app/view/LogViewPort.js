Ext.define('zvsMobile.view.LogViewPort', {
    extend: 'Ext.Panel',
    xtype: 'LogViewPort',
    requires: ['zvsMobile.view.LogList'],
    initialize: function () {
        this.callParent(arguments);        
    },
    constructor: function (config) {        
        Ext.apply(config || {}, {
            items: [{
                        xtype: 'LogList',
                        id: 'LogList'                        
                    }
                    ]
        });
        this.callParent([config]);
    },
    config:
            {
                layout: 'card'
            }
});
