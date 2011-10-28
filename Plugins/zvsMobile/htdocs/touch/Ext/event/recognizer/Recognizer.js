/**
 * @private
 */
Ext.define('Ext.event.recognizer.Recognizer', {
    mixins: ['Ext.mixin.Identifiable'],

    handledEvents: [],

    config: {
        onRecognized: Ext.emptyFn,
        onFailed: Ext.emptyFn,
        callbackScope: null
    },

    constructor: function(config) {
        this.initConfig(config);

        return this;
    },

    getHandledEvents: function() {
        return this.handledEvents;
    },

    onStart: Ext.emptyFn,

    onEnd: Ext.emptyFn,

    fail: function() {
        this.getOnFailed().apply(this.getCallbackScope(), arguments);

        return false;
    },

    fire: function() {
        this.getOnRecognized().apply(this.getCallbackScope(), arguments);
    }
});
