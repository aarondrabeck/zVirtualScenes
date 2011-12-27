Ext.define('Ext.event.recognizer.Drag', {
    extend: 'Ext.event.recognizer.SingleTouch',

    handledEvents: ['dragstart', 'drag', 'dragend'],

    isStarted: false,

    startPoint: null,

    previousPoint: null,

    lastPoint: null,

    onTouchStart: function(e) {
        var startTouches,
            startTouch;

        if (this.callParent(arguments) === false) {
            return false;
        }

        this.startTouches = startTouches = e.changedTouches;
        this.startTouch = startTouch = startTouches[0];
        this.startPoint = startTouch.point;
    },

    onTouchMove: function(e) {
        var touches = e.changedTouches,
            touch = touches[0],
            point = touch.point,
            time = e.time;

        if (this.lastPoint) {
            this.previousPoint = this.lastPoint;
        }

        if (this.lastTime) {
            this.previousTime = this.lastTime;
        }

        this.lastTime = time;
        this.lastPoint = point;

        if (!this.isStarted) {
            this.isStarted = true;

            this.startTime = time;
            this.previousTime = time;

            this.previousPoint = this.startPoint;

            this.fire('dragstart', e, this.startTouches, this.getInfo(e, this.startTouch));
        }
        else {
            this.fire('drag', e, touches, this.getInfo(e, touch));
        }
    },

    onTouchEnd: function(e) {
        if (this.isStarted) {
            var touches = e.changedTouches,
                touch = touches[0],
                point = touch.point;

            this.isStarted = false;

            this.lastPoint = point;

            this.fire('dragend', e, touches, this.getInfo(e, touch));

            this.startTime = 0;
            this.previousTime = 0;
            this.lastTime = 0;

            this.startPoint = null;
            this.previousPoint = null;
            this.lastPoint = null;
        }
    },

    getInfo: function(e, touch) {
        var time = e.time,
            startPoint = this.startPoint,
            previousPoint = this.previousPoint,
            startTime = this.startTime,
            previousTime = this.previousTime,
            point = this.lastPoint,
            deltaX = point.x - startPoint.x,
            deltaY = point.y - startPoint.y,
            info = {
                touch: touch,
                startX: startPoint.x,
                startY: startPoint.y,
                previousX: previousPoint.x,
                previousY: previousPoint.y,
                pageX: point.x,
                pageY: point.y,
                deltaX: deltaX,
                deltaY: deltaY,
                absDeltaX: Math.abs(deltaX),
                absDeltaY: Math.abs(deltaY),
                previousDeltaX: point.x - previousPoint.x,
                previousDeltaY: point.y - previousPoint.y,
                time: time,
                startTime: startTime,
                previousTime: previousTime,
                deltaTime: time - startTime,
                previousDeltaTime: time - previousTime
            };

        return info;
    }
});
