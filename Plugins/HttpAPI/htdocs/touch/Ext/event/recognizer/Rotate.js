Ext.define('Ext.event.recognizer.Rotate', {
    extend: 'Ext.event.recognizer.MultiTouch',

    requiredTouchesCount: 2,

    handledEvents: ['rotatestart', 'rotate', 'rotateend'],

    startAngle: 0,

    lastTouches: null,

    onTouchMove: function(e) {
        if (!this.isTracking) {
            return;
        }

        var touches = Array.prototype.slice.call(e.touches),
            firstPoint, secondPoint, angle;

        firstPoint = touches[0].point;
        secondPoint = touches[1].point;

        angle = firstPoint.getAngleTo(secondPoint);

        if (!this.isStarted) {
            this.isStarted = true;

            this.startAngle = angle;

            this.fire('rotatestart', e, touches, {
                touches: touches,
                angle: angle,
                rotation: 0
            });
        }
        else {
            this.fire('rotate', e, touches, {
                touches: touches,
                angle: angle,
                rotation: angle - this.startAngle
            });
        }

        this.lastTouches = touches;
    },

    fireEnd: function(e) {
        this.fire('rotateend', e, this.lastTouches);
    }
});
