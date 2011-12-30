Ext.define('Ext.event.recognizer.Pinch', {
    extend: 'Ext.event.recognizer.MultiTouch',

    requiredTouchesCount: 2,

    handledEvents: ['pinchstart', 'pinch', 'pinchend'],

    startDistance: 0,

    lastTouches: null,

    onTouchMove: function(e) {
        if (!this.isTracking) {
            return;
        }

        var touches = Array.prototype.slice.call(e.touches),
            firstPoint, secondPoint, distance;

        firstPoint = touches[0].point;
        secondPoint = touches[1].point;

        distance = firstPoint.getDistanceTo(secondPoint);

        if (distance === 0) {
            return;
        }

        if (!this.isStarted) {

            this.isStarted = true;

            this.startDistance = distance;

            this.fire('pinchstart', e, touches, {
                touches: touches,
                distance: distance,
                scale: 1
            });
        }
        else {
            this.fire('pinch', e, touches, {
                touches: touches,
                distance: distance,
                scale: distance / this.startDistance
            });
        }

        this.lastTouches = touches;
    },

    fireEnd: function(e) {
        this.fire('pinchend', e, this.lastTouches);
    },

    fail: function() {
        return this.callParent(arguments);
    }
});
