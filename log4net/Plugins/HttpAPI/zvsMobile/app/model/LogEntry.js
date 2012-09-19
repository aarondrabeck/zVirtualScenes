Ext.define('zvsMobile.model.LogEntry', {
    extend: 'Ext.data.Model',

    config: {
        fields: [
                 { name: 'id', type: 'int' },
				 { name: 'DateTime', type: 'string' },
				 { name: 'Description', type: 'string' },
				 { name: 'Source', type: 'string' },
				 { name: 'Urgency', type: 'string' }
        ]
    }
});