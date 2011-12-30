Ext.define('Ext.event.recognizer.Tap', {

    handledEvents: ['tap'],

    extend: 'Ext.event.recognizer.SingleTouch',

    onTouchMove: function() {
        return this.fail(this.self.TOUCH_MOVED);
    },

    onTouchEnd: function(e) {
        var touch = e.changedTouches[0];

        this.fire('tap', e, [touch]);
    }

}, function() {
    //<deprecated product=touch since=2.0>
    this.override({
        handledEvents: ['tap', 'tapstart', 'tapcancel'],

        onTouchStart: function(e) {
            if (this.callOverridden(arguments) === false) {
                return false;
            }

            this.fire('tapstart', e, [e.changedTouches[0]]);
        },

        onTouchMove: function(e) {
            this.fire('tapcancel', e, [e.changedTouches[0]]);

            return this.callOverridden(arguments);
        }
    });
    //</deprecated>
});
