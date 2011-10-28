/**
 * Navigation Layout...
 */
Ext.define('Ext.layout.Navigation', {
    extend: 'Ext.layout.HBox',

    alias: 'layout.navigation',

    constructor: function() {
        this.callParent(arguments);
        this.spacer = new Ext.Component();
        this.container.innerElement.append(this.spacer.renderElement);
        this.setItemFlex(this.spacer, 1);
    },

    doItemAdd: function(item, index) {
        var index = this.container.indexOf(this.spacer);
        if (item.config.align != 'right') {

        }
        this.callParent([item, index])
    }
});