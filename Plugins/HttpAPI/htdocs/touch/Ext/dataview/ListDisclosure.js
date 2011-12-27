/**
 * @private - To be made a sample
 */
Ext.define('Ext.dataview.ListDisclosure', {
    extend: 'Ext.Component',
    xtype : 'listdisclosure',

    config: {
        // @inherit
        baseCls: Ext.baseCSSPrefix + 'list-disclosure'
    },

    initialize: function() {
        var me = this;

        me.callParent();

        me.element.on({
            tap: 'onTap',
            scope: me
        });
    },

    onTap: function(e) {
        this.fireEvent('tap', this, e);
    }
});
