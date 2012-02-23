Ext.define('zvsMobile.model.Scene', {
    extend: 'Ext.data.Model',

    config: {
        fields: [
				 { name: 'id', type: 'int' },
				 { name: 'name', type: 'string' },
				 { name: 'is_running', type: 'bool' },
				 { name: 'cmd_count', type: 'string' }
			 ]
    }
});