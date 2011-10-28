Ext.define('Ext.ItemCollection', {
    extend: 'Ext.util.MixedCollection',

    getKey: function(item) {
        return item.getId();
    },

    has: function(item) {
        return this.map.hasOwnProperty(item.getId());
    }
});
