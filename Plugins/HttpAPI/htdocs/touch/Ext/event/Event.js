/**
 * Represents an event.
 * @class Ext.event.Event
 * @alternateClassName Ext.EventObject
 */
Ext.define('Ext.event.Event', {
    alternateClassName: 'Ext.EventObject',
    isStopped: false,
    alternativeClassName: 'Ext.EventObject',

    set: function(name, value) {
        if (arguments.length === 1 && typeof name != 'string') {
            var info = name;

            for (name in info) {
                if (info.hasOwnProperty(name)) {
                    this[name] = info[name];
                }
            }
        }
        else {
            this[name] = info[name];
        }
    },

    stopEvent: function() {
        return this.stopPropagation();
    },

    stopPropagation: function() {
        this.isStopped = true;

        return this;
    }
});
