/**
 * The slider is a way to allow the user to select a value from a given numerical range. You might use it for choosing
 * a percentage, combine two of them to get min and max values, or use three of them to specify the hex values for a
 * color. Each slider contains a single 'thumb' that can be dragged along the slider's length to change the value.
 * Sliders are equally useful inside {@link Ext.form.Panel forms} and standalone. Here's how to quickly create a
 * slider in form, in this case enabling a user to choose a percentage:
 *
 *     Ext.create('Ext.form.Panel', {
 *         items: [
 *             {
 *                 xtype: 'sliderfield',
 *                 label: 'Percentage',
 *                 value: 50,
 *                 {@link #minValue}: 0,
 *                 {@link #maxValue}: 100
 *             }
 *         ]
 *     });
 *
 * In this case we set a starting value of 50%, and defined the min and max values to be 0 and 100 respectively, giving
 * us a percentage slider. Because this is such a common use case, the defaults for {@link #minValue} and
 * {@link #maxValue} are already set to 0 and 100 so in the example above they could be removed.
 *
 * It's often useful to render sliders outside the context of a form panel too. In this example we create a slider that
 * allows a user to choose the waist measurement of a pair of jeans. Let's say the online store we're making this for
 * sells jeans with waist sizes from 24 inches to 60 inches in 2 inch increments - here's how we might achieve that:
 *
 *     var slider = Ext.create('Ext.field.Slider', {
 *         label: 'Waist Measurement',
 *         minValue: 24,
 *         maxValue: 60,
 *         increment: 2,
 *         value: 32
 *     });
 *
 * Now that we've got our slider, we can ask it what value it currently has and listen to events that it fires. For
 * example, if we wanted our app to show different images for different sizes, we can listen to the {@link #change}
 * event to be informed whenever the slider is moved:
 *
 *     slider.on('change', function(newValue, oldValue) {
 *         if (newValue > 40) {
 *             imgComponent.setSrc('large.png')
 *         } else {
 *             imgComponent.setSrc('small.png');
 *         }
 *     }, this);
 *
 * Here we listened to the {@link #change} event on the slider and updated the background image of an
 * {@link Ext.Img image component} based on what size the user selected. Of course, you can use any logic inside your
 * event listener.
 */
Ext.define('Ext.field.Slider', {
    extend  : 'Ext.field.Field',
    alias   : 'widget.sliderfield',
    requires: ['Ext.field.slider.Thumb'],
    alternateClassName: 'Ext.form.Slider',

    /**
     * @event change
     * Fires when an option selection has changed.
     * @param {Ext.field.Select} me
     * @param {Mixed} value
     */

    config: {
        // @inherit
        layout: null,

        // @inherit
        cls: Ext.baseCSSPrefix + 'slider',

        /**
         * @cfg {Array} thumbs An array of {@link Ext.field.slider.Thumb}'s to be used in this slider.
         * @accessor
         */
        thumbs: [],

        /**
         * @cfg {Number/String/Number[]/String[]} value The value(s) of the sliders {@link #thumbs}. If you pass
         * a number or a string, it will assume you have just 1 thumb.
         * @accessor
         */
        value: 0,

        /**
         * @cfg {Number} tabIndex
         * @hide
         * @accessor
         */
        tabIndex: -1,

        /**
         * @cfg {Number} minValue The lowest value any thumb on this slider can be set to.
         * @accessor
         */
        minValue: 0,

        /**
         * @cfg {Number} maxValue The highest value any thumb on this slider can be set to.
         * @accessor
         */
        maxValue: 100,

        /**
         * @cfg {Number} increment The increment by which to snap each thumb when its value changes. Defaults to 1. Any thumb movement
         * will be snapped to the nearest value that is a multiple of the increment (e.g. if increment is 10 and the user tries to move
         * the thumb to 67, it will be snapped to 70 instead)
         * @accessor
         */
        increment: 1
    },

    // @private
    constructor: function(config) {
        config = config || {};

        if (config.hasOwnProperty('values')) {
            config.value = config.values;
            config._values = config.value;
        }

        this.callParent([config]);
    },

    // @private
    initialize: function() {
        var me = this;

        me.callParent(arguments);

        me.on({
            scope: this,
            delegate: 'thumb',
            change: 'onChange'
        });

        me.on('painted', 'onPainted');

        me.element.on({
            scope: this,

            tap: 'onTap'
        });

        this.sizeMonitor = new Ext.util.SizeMonitor({
            element : this.element,
            callback: this.onSizeChange,
            scope   : this
        });
    },

    onPainted: function() {
        this.sizeMonitor.refresh();
    },

    onSizeChange: function() {
        var me = this;
        me.refreshThumbs();
    },

    // @private
    refreshThumbs: function() {
        var me = this;

        //loop through each of the thumbs and refresh their values
        var thumbs = this.getThumbs(),
            ln = thumbs.length,
            i;

        for (i = 0; i < ln; i++) {
            thumbs[i].refreshValue();
        }
    },

    // @private
    applyThumbs: function(thumbs) {
        var i, ln, config, currentThumb, instance,
            currentThumbs = this.getThumbs() || [],
            instances = [];

        if (thumbs && thumbs.length > 0) {
            //convert it into an array if it is not already
            if (!Ext.isArray(thumbs)) {
                thumbs = [thumbs];
            }

            ln = thumbs.length;

            for (i = 0; i < ln; i++) {
                config = thumbs[i];
                config.slider = this;
                currentThumb = currentThumbs[i];

                instance = Ext.factory(config, 'Ext.field.slider.Thumb', currentThumb);
                instances.push(instance);
            }

            return instances;
        }

        return thumbs;
    },

    // @private
    updateThumbs: function(newThumbs) {
        if (newThumbs) {
            this.add(newThumbs);
        }
    },

    // @private
    applyValue: function(value) {
        //sync the values from the thumbs
        this.getValue();

        //convert it into an array if it is not already
        if (!Ext.isArray(value)) {
            value = [value];
        }

        return value;
    },

    /**
     * Updates the slides {@link #thumbs} with their new value(s)
     */
    updateValue: function(newValue) {
        var thumbs = this.getThumbs(),
            newThumbs = [],
            ln = newValue.length,
            thumb, i;

        //if there are no thumbs defined, create them
        if (thumbs.length === 0) {
            for (i = 0; i < ln; i++) {
                newThumbs.push({
                    value: newValue[i]
                });
            }

            this.setThumbs(newThumbs);

            return;
        }

        //update the thumb values
        ln = newValue.length;
        for (i = 0; i < ln; i++) {
            thumb = thumbs[i];
            if (thumb) {
                thumbs[i].setValue(newValue);
            }
            //<debug>
            else {
                throw new Error("Ext.field.Slider: [setValue] calling setValue() with more values than there are thumbs (" + thumbs.length + " thumb(s), " + ln +" value(s)).");
            }
            //</debug>
        }
    },

    // @inherit
    getValue: function() {
        var thumbs = this.getThumbs(),
            ln = thumbs.length,
            value = [],
            i;

        //update the thumb values
        for (i = 0; i < ln; i++) {
            value.push(thumbs[i].getValue());
        }

        this._value = value;
        this._values = value;

        return value;
    },

    /**
     * Convience method. Calls {@link #setValue}
     */
    setValues: function() {
        this.updateValue(this.applyValue(value));
        this._values = this._value;
    },

    setValue: function(value) {
        this.updateValue(this.applyValue(value));
        this._value = this._values = value;
    },

    /**
     * Convience method. Calls {@link #getValue}
     */
    getValues: function() {
        return this.getValue();
    },

    // Sets the {@link #increment} configuration
    applyIncrement: function(increment) {
        if (increment === 0) {
            increment = 1;
        }

        increment = Math.abs(increment);

        return increment;
    },

    // @private
    updateMinValue: function(newMinValue) {
        this.refreshThumbs();
    },

    // @private
    updateMaxValue: function(newMaxValue) {
        this.refreshThumbs();
    },

    // @private
    updateIncrement: function(newIncrement) {
        this.refreshThumbs();
    },

    /**
     * Returns an instance of a thumb for a specifed index
     * @param {Number} index The index of the thumb (defaults to 0)
     * @return {Ext.field.slider.Thumb} The thumb instance
     */
    getThumb: function(index) {
        var thumbs = this.getThumbs();
        return thumbs[index || 0];
    },

    /**
     * Finds the closest thumb for a specified value
     * @return {Ext.field.slider.Thumb} The thumb
     */
    getClosestThumb: function(value) {
        var thumbs = this.getThumbs(),
            ln     = thumbs.length,
            thumb  = thumbs[0],
            difference = Infinity,
            thumbDifference, thumbValue, i;

        if (ln == 1) {
           return thumb;
        }

        // loop through each of thumbs and find the one with the least amount of difference
        for (i = 0; i < ln; i++) {
            thumbValue = thumbs[i].getValue();
            thumbDifference = Math.abs(thumbValue - value);
            if (thumbDifference < difference) {
                difference = thumbDifference;
                thumb = thumbs[i];
            }
        }

        return thumb;
    },

    /**
     * Returns the index of a specified thumb
     * @param {Ext.field.slider.Thumb} thumb
     * @return {Number} Index of the thumb
     */
    indexOf: function(thumb) {
        return this.getThumbs().indexOf(thumb);
    },

    /**
     * Returns the correct offset for a specified value, based on the {@link #minWidth}, {@link #maxWidth} and
     * {@link #increment} configurations
     * @private
     */
    getOffsetForValue: function(value) {
        var me = this,
            minValue   = me.getMinValue(),
            maxValue   = me.getMaxValue(),
            range      = maxValue - minValue,
            trackWidth = me.innerElement.getWidth(),
            thumbWidth = 0,
            thumb, ratio;

        thumb = me.getThumb();
        if (thumb) {
            thumbWidth = thumb.renderElement.getWidth();
        }
        trackWidth = trackWidth - thumbWidth;

        value = this.constrain(value);
        ratio = trackWidth / range;

        return Math.round((ratio * (value - minValue)));
    },

    /**
     * Returns the correct value for a specified offset, based on the {@link #minWidth}, {@link #maxWidth} and
     * {@link #increment} configurations. Rerverse of {@link #getOffsetForValue}
     * @private
     */
    getValueForOffset: function(offset, isTap) {
        var me = this,
            minValue   = me.getMinValue(),
            maxValue   = me.getMaxValue(),
            range      = maxValue - minValue,
            trackWidth = me.innerElement.getWidth(),
            thumbWidth = 0,
            thumb, ratio;

        thumb = me.getThumb();
        if (thumb) {
            thumbWidth = thumb.renderElement.getWidth();
        }
        trackWidth = trackWidth - ((isTap) ? 0 : thumbWidth);

        ratio = range / trackWidth;

        return Math.round(minValue + (ratio * (offset)));
    },

    /**
     * @private
     * Takes a desired value of a thumb and returns the nearest snap value. e.g if minValue = 0, maxValue = 100, increment = 10 and we
     * pass a value of 67 here, the returned value will be 70. The returned number is constrained within {@link minValue} and {@link maxValue},
     * so in the above example 68 would be returned if {@link maxValue} was set to 68.
     * @param {Number} value The value to snap
     * @return {Number} The snapped value
     */
    constrain: function(value) {
        var me = this,
            minValue  = me.getMinValue(),
            maxValue  = me.getMaxValue(),
            increment = me.getIncrement(),
            remainder = value % increment;

        value -= remainder;

        if (Math.abs(remainder) >= (increment / 2)) {
            value += (remainder > 0) ? increment : -increment;
        }

        value = Math.max(minValue, value);
        value = Math.min(maxValue, value);

        return value;
    },

    /**
     * @private
     * Loops through each of the sliders {@link #thumbs} and calls disable/enable on each of them depending
     * on the param specified.
     * @param {Boolean} disable True to disable, false to enable
     */
    setThumbsDisabled: function(disable) {
        var me = this,
            thumbs = me.thumbs,
            ln     = thumbs.length,
            i;

        for (i = 0; i < ln; i++) {
            thumbs[i][disable ? 'disable' : 'enable']();
        }
    },

    /**
     * Called when the value of any child {@link #thumbs} changes.
     * @private
     */
    onChange: function(thumb, value) {
        var thumbs = this.getThumbs(),
            ln = thumbs.length,
            thumbWidth = thumb.renderElement.getWidth(),
            previousThumb, offset, previousOffset, i, thumbDraggable, previousThumbDraggable;

        for (i = 0; i < ln; i++) {
            thumb         = thumbs[i];
            previousThumb = thumbs[i - 1];
            thumbDraggable         = (thumb) ? thumb.getDraggable() : null;
            previousThumbDraggable = (previousThumb) ? previousThumb.getDraggable() : null;

            if (previousThumb && thumbDraggable && previousThumbDraggable) {
                offset = thumbDraggable.getOffset().x;
                previousOffset = previousThumb.getDraggable().getOffset().x;

                thumbDraggable.setConstraint({
                    min: {
                        x: (previousOffset === 0) ? thumbWidth : previousOffset + thumbWidth
                    }
                });

                previousThumbDraggable.setConstraint({
                    max: {
                        x: offset - thumbWidth
                    }
                });
            }
        }

        this.fireEvent('change', this, thumb, value);
    },

    // @private
    onTap: function(e) {
        var el = Ext.get(e.target);

        if (el.hasCls(Ext.baseCSSPrefix + 'thumb')) {
            return;
        }

        var touchX = e.touch.point.x,
            parent = this.element,
            parentX = parent.getX(),
            offset = touchX - parentX,
            value  = this.getValueForOffset(offset, true),
            thumb  = this.getClosestThumb(value);

        thumb.setValue(value);
    },

    /**
     * Disables the slider
     */
    disable: function() {
        this.callParent();
        this.setThumbsDisabled(true);
    },

    /**
     * Enables the slider
     */
    enable: function() {
        this.callParent();
        this.setThumbsDisabled(false);
    },

    // @inherit
    reset: function() {
        this.setValue(this.originalValue);
    }
});
