/**
 * @private
 */
Ext.define('Ext.form.ClearIcon', {
    extend: 'Ext.Component',
    xtype : 'clearicon',

    config: {
        // @inherit
        baseCls: Ext.baseCSSPrefix + 'field-clear-container',

        // @inherit
        docked: 'right'  
    },

    template: [{
        classList: [Ext.baseCSSPrefix + 'field-clear'],
        text     : 'x'
    }],

    initialize: function() {
        var me = this;

        me.callParent(arguments);

        me.renderElement.on({
            scope: me,
            tap  : 'onTap'
        });
    },

    /**
     * Called when this clear icon has been tapped on
     */
    onTap: function(e) {
        this.fireAction('tap', [this, e]);
    }
});
