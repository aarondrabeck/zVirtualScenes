Ext.define('Ext.event.recognizer.Swipe', {
    extend: 'Ext.event.recognizer.SingleTouch',

    handledEvents: ['swipe'],

    inheritableStatics: {
        MAX_OFFSET_EXCEEDED: 0x10,
        MAX_DURATION_EXCEEDED: 0x11,
        DISTANCE_NOT_ENOUGH: 0x12
    },

    config: {
        minDistance: 80,
        maxOffset: 35,
        maxDuration: 1000
    },

    onTouchStart: function(e) {
        if (this.callParent(arguments) === false) {
            return false;
        }

        var touch = e.changedTouches[0];

        this.startTime = e.time;

        this.isHorizontal = true;
        this.isVertical = true;

        this.startX = touch.pageX;
        this.startY = touch.pageY;
    },

    onTouchMove: function(e) {
        var touch = e.changedTouches[0],
            x = touch.pageX,
            y = touch.pageY,
            absDeltaX = Math.abs(x - this.startX),
            absDeltaY = Math.abs(y - this.startY),
            time = e.time;

        if (time - this.startTime > this.getMaxDuration()) {
            return this.fail(this.self.MAX_DURATION_EXCEEDED);
        }

        if (this.isVertical && absDeltaX > this.getMaxOffset()) {
            this.isVertical = false;
        }

        if (this.isHorizontal && absDeltaY > this.getMaxOffset()) {
            this.isHorizontal = false;
        }

        if (!this.isHorizontal && !this.isVertical) {
            return this.fail(this.self.MAX_OFFSET_EXCEEDED);
        }
    },

    onTouchEnd: function(e) {
        if (this.onTouchMove(e) === false) {
            return false;
        }

        var touch = e.changedTouches[0],
            x = touch.pageX,
            y = touch.pageY,
            deltaX = x - this.startX,
            deltaY = y - this.startY,
            absDeltaX = Math.abs(deltaX),
            absDeltaY = Math.abs(deltaY),
            minDistance = this.getMinDistance(),
            duration = e.time - this.startTime,
            direction, distance;

        if (this.isVertical && absDeltaY < minDistance) {
            this.isVertical = false;
        }

        if (this.isHorizontal && absDeltaX < minDistance) {
            this.isHorizontal = false;
        }

        if (this.isHorizontal) {
            direction = (deltaX < 0) ? 'left' : 'right';
            distance = absDeltaX;
        }
        else if (this.isVertical) {
            direction = (deltaY < 0) ? 'up' : 'down';
            distance = absDeltaY;
        }
        else {
            return this.fail(this.self.DISTANCE_NOT_ENOUGH);
        }

        this.fire('swipe', e, [touch], {
            touch: touch,
            direction: direction,
            distance: distance,
            duration: duration
        });
    }
});
