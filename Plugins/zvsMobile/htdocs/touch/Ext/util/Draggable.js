/**
 * A core util class to bring Draggable behavior to any DOM element,
 * acts as a base class for Scroller and Sortable.
 */
Ext.define('Ext.util.Draggable', {
    isDraggable: true,

    mixins: [
        'Ext.mixin.Observable'
    ],

    config: {
        cls: Ext.baseCSSPrefix + 'draggable',

        draggingCls: Ext.baseCSSPrefix + 'dragging',

        proxyCls: Ext.baseCSSPrefix + 'draggable-proxy',

        element: null,

        constraint: 'container',

        disabled: null,

        /**
         * @cfg {String} direction
         * Possible values: 'vertical', 'horizontal', or 'both'
         * @accessor
         */
        direction: 'both',

        /**
         * @cfg {Number} delay
         * How many milliseconds a user must hold the draggable before starting a
         * drag operation.
         * @accessor
         */
        delay: 0,

        /**
         * @cfg {String} cancelSelector
         * A simple CSS selector that represents elements within the draggable
         * that should NOT initiate a drag.
         * @accessor
         */
        cancelSelector: null,

        /**
         * @cfg {Boolean} revert
         * Whether or not the element or it's proxy will be reverted back (with animation)
         * when it's not dropped and held by a Droppable
         * @accessor
         */
        revert: false,

        /**
         * @cfg {String} group
         * Draggable and Droppable objects can participate in a group which are
         * capable of interacting.
         * @accessor
         */
        group: 'base',

        translateMethod: 'auto',

        initialOffset: 'auto',

        containerConstraint: 'auto',

        autoRefresh: true,

        offset: {
            x: 0,
            y: 0
        }
    },

    CSS_POSITION: 'cssposition',

    CSS_TRANSFORM: 'csstransform',

    DIRECTION_BOTH: 'both',

    DIRECTION_VERTICAL: 'vertical',

    DIRECTION_HORIZONTAL: 'horizontal',

    /**
     * Creates new Draggable.
     * @param {Mixed} el The element you want to make draggable.
     * @param {Object} config The configuration object for this Draggable.
     */
    constructor: function(config) {
        var element;

        this.sizeMonitors = {};

        this.initialConfig = config;

        this.listeners = {
            dragstart: 'onDragStart',
            drag     : 'onDrag',
            dragend  : 'onDragEnd',

            scope: this
        };

        this.isAxisEnabledFlags = { x: false, y: false };

        if (config && config.element) {
            element = config.element;
            delete config.element;

            this.setElement(element);
        }

        return this;
    },

    applyElement: function(element) {
        if (!element) {
            this.container = null;
            return;
        }

        return Ext.get(element);
    },

    updateElement: function(element) {
        if (!this.initialized) {
            this.initialized = true;

            this.initConfig(this.initialConfig);

            element.on(this.listeners);

            if (this.getAutoRefresh()) {
                this.sizeMonitors.element = new Ext.util.SizeMonitor({
                    element: element,
                    callback: this.doRefresh,
                    scope: this
                });
            }

            this.onConfigUpdate('containerConstraint', 'refreshConstraint');
            this.onConfigUpdate('constraint', 'refreshOffset');
        }
        else {
            this.refresh();
        }

        element.addCls(this.getCls());

        return this;
    },

    applyConstraint: function(newConstraint, currentConstraint) {
        var initialOffset = this.getInitialOffset(),
            initialOffsetX, initialOffsetY, constraint, containerConstraint;

        this.givenConstraint = newConstraint;

        constraint = {
            min: { x: -Infinity, y: -Infinity },
            max: { x: Infinity, y: Infinity }
        };

        if (newConstraint === 'container') {
            containerConstraint = this.getContainerConstraint();

            initialOffsetX = initialOffset.x;
            initialOffsetY = initialOffset.y;

            constraint.min.x = containerConstraint.min.x - initialOffsetX;
            constraint.min.y = containerConstraint.min.y - initialOffsetY;
            constraint.max.x = containerConstraint.max.x - initialOffsetX;
            constraint.max.y = containerConstraint.max.y - initialOffsetY;
        }
        else if (Ext.isSimpleObject(newConstraint)) {
            Ext.merge(constraint, currentConstraint || {}, newConstraint);
        }

        return constraint;
    },

    applyContainerConstraint: function(newConstraint, currentConstraint) {
        var constraint, container,
            containerDom, dom, containerStyle, min, max,
            width, height, containerWidth, containerHeight,
            paddingTop, paddingLeft, paddingRight, paddingBottom,
            borderTop, borderLeft, borderRight, borderBottom;

        constraint = {
            min: { x: -Infinity, y: -Infinity },
            max: { x: Infinity, y: Infinity }
        };

        this.givenContainerConstraint = newConstraint;

        if (newConstraint === 'auto' && (container = this.getContainer())) {
            dom = this.getElement().dom;
            containerDom = container.dom;

            containerStyle = window.getComputedStyle(containerDom);

            min = constraint.min;
            max = constraint.max;

            width = dom.offsetWidth;
            height = dom.offsetHeight;

            paddingLeft   = this.getNumberValue(containerStyle.paddingLeft);
            paddingTop    = this.getNumberValue(containerStyle.paddingTop);
            paddingRight  = this.getNumberValue(containerStyle.paddingRight);
            paddingBottom = this.getNumberValue(containerStyle.paddingBottom);

            borderTop     = this.getNumberValue(containerStyle.borderTopWidth);
            borderLeft    = this.getNumberValue(containerStyle.borderLeftWidth);
            borderRight   = this.getNumberValue(containerStyle.borderRightWidth);
            borderBottom  = this.getNumberValue(containerStyle.borderBottomWidth);

            containerWidth = containerDom.offsetWidth;
            containerHeight = containerDom.offsetHeight;

            min.x = paddingLeft;
            min.y = paddingTop;

            max.x = Math.max(min.x, containerWidth - paddingRight - borderLeft - borderRight - width);
            max.y = Math.max(min.y, containerHeight - paddingBottom - borderTop - borderBottom - height);
        }
        else if (Ext.isSimpleObject(newConstraint)) {
            Ext.merge(constraint, currentConstraint || {}, newConstraint);
        }

        return constraint;
    },

    getNumberValue: function(value) {
        value = parseInt(value, 10);

        if (isNaN(value)) {
            value = 0;
        }

        return value;
    },

    applyTranslateMethod: function(method) {
        var cssPosition = this.CSS_POSITION,
            cssTransform = this.CSS_TRANSFORM;

        if (method === 'auto') {
            if (Ext.os.is.Android2) {
                method = cssPosition;
            }
            else {
                method = cssTransform;
            }
        }

        if (method !== cssPosition && method !== cssTransform) {
            method = cssTransform;
        }

        return method;
    },

    updateTranslateMethod: function(newMethod, oldMethod) {
        if (oldMethod) {
            if (oldMethod === this.CSS_TRANSFORM) {
                this.translateWithCssTransform(0, 0);
            }
            else {
                this.translateWithCssPosition(0, 0);
            }
        }

        this.updateOffset(this.getOffset());
    },

    getContainer: function() {
        var container = this.container;

        if (!container) {
            container = this.getElement().getParent();

            if (container) {
                if (this.getAutoRefresh()) {
                    this.sizeMonitors.container = new Ext.util.SizeMonitor({
                        element: container,
                        callback: this.doRefresh,
                        scope: this
                    });
                }

                this.container = container;
            }
        }

        return container;
    },

    applyInitialOffset: function(offset) {
        var dom, style;

        if (offset === 'auto') {
            dom = this.getElement().dom;
            style = window.getComputedStyle(dom);

            offset = {
                x: dom.offsetLeft - this.getNumberValue(style.marginLeft),
                y: dom.offsetTop - this.getNumberValue(style.marginTop)
            };
        }

        return offset;
    },

    detachListeners: function() {
        this.getElement().un(this.listeners);
    },

    updateDirection: function(direction) {
        var isAxisEnabled = this.isAxisEnabledFlags;

        isAxisEnabled.x = (direction === this.DIRECTION_BOTH || direction === this.DIRECTION_HORIZONTAL);
        isAxisEnabled.y = (direction === this.DIRECTION_BOTH || direction === this.DIRECTION_VERTICAL);
    },

    isAxisEnabled: function(axis) {
        this.getDirection();

        return this.isAxisEnabledFlags[axis];
    },

    onDragStart: function(e) {
        if (this.getDisabled()) {
            return false;
        }

        this.fireAction('dragstart', [this, e, this.getOffset()], 'initDragStart');
    },

    initDragStart: function(me, e, startOffset) {
        this.dragStartOffset = startOffset;

        this.isDragging = true;

        this.getElement().addCls(this.getDraggingCls());
    },

    onDrag: function(e) {
        if (!this.isDragging) {
            return;
        }

        var startOffset = this.dragStartOffset;

        this.setOffset({
            x: startOffset.x + e.deltaX,
            y: startOffset.y + e.deltaY
        });

        this.fireEvent('drag', this, e, this.getOffset());
    },

    onDragEnd: function(e) {
        if (!this.isDragging) {
            return;
        }

        this.onDrag(e);

        this.isDragging = false;

        this.fireEvent('dragend', this, e);

        this.getElement().removeCls(this.getDraggingCls());
    },

    applyOffset: function(offset, oldOffset) {
        var x = offset.x,
            y = offset.y,
            constraint = this.getConstraint(),
            minOffset = constraint.min,
            maxOffset = constraint.max,
            min = Math.min,
            max = Math.max;

        if (this.isAxisEnabled('x') && typeof x == 'number') {
            x = min(max(x, minOffset.x), maxOffset.x);
        }
        else {
            x = oldOffset ? oldOffset.x : 0;
        }

        if (this.isAxisEnabled('y') && typeof y == 'number') {
            y = min(max(y, minOffset.y), maxOffset.y);
        }
        else {
            y = oldOffset ? oldOffset.y : 0;
        }

        return {
            x: x,
            y: y
        };
    },

    updateOffset: function(offset, oldOffset) {
        var x = offset.x,
            y = offset.y;

        if (!oldOffset || x !== oldOffset.x || y !== oldOffset.y) {
            this.moveTo(x, y);
        }
    },

    moveTo: function(x, y) {
        var method = this.getTranslateMethod();

        if (method === this.CSS_POSITION) {
            return this.translateWithCssPosition.apply(this, arguments);
        }
        else {
            return this.translateWithCssTransform.apply(this, arguments);
        }
    },

    translateWithCssPosition: function(x, y) {
        var initialOffset = this.getInitialOffset(),
            domStyle = this.getElement().dom.style;

        domStyle.left = (x + initialOffset.x) + 'px';
        domStyle.top = (y + initialOffset.y) + 'px';
    },

    translateWithCssTransform: function(x, y) {
        var domStyle = this.getElement().dom.style;

        domStyle.webkitTransform = 'translate3d(' + x + 'px, ' + y + 'px, 0px)';
    },

    refreshConstraint: function() {
        this.setConstraint(this.givenConstraint);
    },

    refreshOffset: function() {
        this.setOffset(this.getOffset());
    },

    doRefresh: function() {
        this.setContainerConstraint(this.givenContainerConstraint);
        this.refreshConstraint();
        this.refreshOffset();
    },

    refresh: function() {
        var sizeMonitors = this.sizeMonitors;

        if (sizeMonitors.element) { 
            sizeMonitors.element.refresh();
        }
        
        if (sizeMonitors.container) {
            sizeMonitors.container.refresh();
        }

        this.doRefresh();
    },

    /**
     * Enable the Draggable.
     * @return {Ext.util.Draggable} This Draggable instance
     */
    enable: function() {
        return this.setDisabled(false);
    },

    /**
     * Disable the Draggable.
     * @return {Ext.util.Draggable} This Draggable instance
     */
    disable: function() {
        return this.setDisabled(true);
    },

    destroy: function() {
        var sizeMonitors = this.sizeMonitors;

        if (sizeMonitors.element) {
            sizeMonitors.element.destroy();
        }

        if (sizeMonitors.container) {
            sizeMonitors.container.destroy();
        }

        this.getElement().removeCls(this.getCls());
        this.detachListeners();
    }

}, function() {
    //<deprecated product=touch since=2.0>
    this.override({
        constructor: function(config) {
            if (config && config.constrain) {
                //<debug warn>
                Ext.Logger.deprecate("'constrain' config is deprecated, please use 'contraint' instead");
                //</debug>
                config.contraint = config.constrain;
                delete config.constrain;
            }

            return this.callOverridden(arguments);
        }
    });
    //</deprecated>
});

