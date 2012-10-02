Ext.define('zvsMobile.view.LogList', {
    extend: 'Ext.dataview.List',
    requires: ['Ext.plugin.PullRefresh'],
    xtype: 'LogList',
    config: {
        cls: 'LogList',
        store: 'LogEntries',
        scrollable: 'vertical',
        ui: 'round',
        scrollToTopOnRefresh: false,
        plugins: [{ xclass: 'Ext.plugin.PullRefresh' }],
        itemTpl: new Ext.XTemplate(
            '<div class="log">',
                '<h2 class="description">{Description}</h2>',
                '<h3>{DateTime} <span class="urgency">{Urgency}</span> <span class="source">{Source}</span>  </h3>',
			'</div>'
         ),
        items: {
            xtype: 'toolbar',
            docked: 'top',
            title: 'Activity Log'
        }

    }
});

