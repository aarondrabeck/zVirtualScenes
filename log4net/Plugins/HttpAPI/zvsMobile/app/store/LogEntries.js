Ext.define('zvsMobile.store.LogEntries', {
    extend: 'Ext.data.Store',        
    requires: ['zvsMobile.model.LogEntry'],
	config: {
	    model: 'zvsMobile.model.LogEntry'
	}
});

LogEntryStore = Ext.create('zvsMobile.store.LogEntries', {
    id: 'LogEntryStore',
    requires: ['zvsMobile.store.LogEntries']
});



