/**
 * @ignore
 * Utility class used by Ext.slider.Slider - should never need to be used directly.
 */
Ext.define('Ext.slider.Thumb', {
    extend: 'Ext.Component',
    xtype : 'thumb',

    config: {
        // @inherit
        baseCls: Ext.baseCSSPrefix + 'thumb',

        /**
         * @cfg {Number} value The value to initialize this thumb with (defaults to 0)
         * @accessor
         */
        value: 0,

        // @inherit
        draggable: {
            direction: 'horizontal'
        }
    },

    // @private
    slider: null,

    // @private
    initialize: function() {
        var me = this;

        me.callParent(arguments);

        me.on({
            scope: this,

            painted: 'onPainted'
        });
        
        // add a listener on the draggable instance for dragend
        me.getDraggable().on({
            scope: this,

            'dragstart': {
                before: 'onBeforeDragStart',
                after : 'onDragStart'
            },
            'drag'   : 'onDrag',
            'dragend': 'onDragEnd'
        });

        this.slider = this.config.slider;
    },

    // @private
    onPainted: function() {
        //refresh the values now that the slider has a width
        this.refreshValue();
    },

    /**
     * Updates the offset of this thumb
     */
    updateValue: function(newValue) {
        var slider = this.slider,
            offset, draggable;

        if (slider) {
            offset = slider.getOffsetForValue(newValue);
            draggable = this.getDraggable();

            draggable.setOffset({
                x: offset
            });
        }

        this.fireAction('change', [this, newValue]);
    },

    // @private
    refreshValue: function() {
        this.updateValue(this.getValue());
    },

    // @private
    onBeforeDragStart: function(draggable, e) {
        if (e.absDeltaX < 4) {
            return false;
        }
    },

    // @private
    onDragStart: function(draggable, e, offset) {
        this.addCls(Ext.baseCSSPrefix + 'dragging');
    },

    // @private
    onDrag: function(draggable, e, offset) {
        e.stopPropagation();

        var value = this.slider.getValueForOffset(offset.x);
        this.setValue(value);
        this.updateValue(value);
    },

    // @private
    onDragEnd: function() {
        var me     = this,
            offset = me.getDraggable().getOffset(),
            newValue;

        newValue = me.slider.getValueForOffset(offset.x);

        me.setValue(newValue);

        me.removeCls(Ext.baseCSSPrefix + 'dragging');
    }
});
