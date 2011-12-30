/**
 * 
 */
Ext.define('Ext.event.Dom', {
    extend: 'Ext.event.Event',

    constructor: function(event) {
        this.browserEvent = this.event = event;
        this.target = this.delegatedTarget = event.target;
        this.type = event.type;
        this.pageX = event.pageX;
        this.pageY = event.pageY;

        this.timeStamp = this.time = event.timeStamp;

        if (typeof this.time != 'number') {
            this.time = new Date(this.time).getTime();
        }

        return this;
    },

    stopEvent: function() {
        this.preventDefault();

        return this.callParent();
    },

    preventDefault: function() {
        this.browserEvent.preventDefault();
    },

    getPageX: function() {
        return this.browserEvent.pageX;
    },

    getPageY: function() {
        return this.browserEvent.pageY;
    },

    /**
     * @deprecated
     */
    getXY: function() {
        if (!this.xy) {
            this.xy = [this.getPageX(), this.getPageY()];
        }

        return this.xy;
    },

    setDelegatedTarget: function(target) {
        this.delegatedTarget = target;
    },

    /**
     * Gets the target for the event.
     * @param {String} selector (optional) A simple selector to filter the target or look for an ancestor of the target
     * @param {Number/Mixed} maxDepth (optional) The max depth to
     search as a number or element (defaults to 10 || document.body)
     * @param {Boolean} returnEl (optional) True to return a Ext.Element object instead of DOM node
     * @return {HTMLElement}
     */
    getTarget: function(selector, maxDepth, returnEl) {
        if (arguments.length === 0) {
            return this.delegatedTarget;
        }

        return selector ? Ext.fly(this.target).findParent(selector, maxDepth, returnEl) : (returnEl ? Ext.get(this.target) : this.target);
    },

    getTime: function() {
        return this.time;
    }
});
