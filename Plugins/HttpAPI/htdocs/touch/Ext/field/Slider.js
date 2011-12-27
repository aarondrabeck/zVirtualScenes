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
    requires: ['Ext.slider.Slider'],
    alternateClassName: 'Ext.form.Slider',

    /**
     * @event change
     * Fires when an option selection has changed.
     * @param {Ext.field.Select} me
     * @param {Mixed} value
     */

    config: {
        // @inherit
        cls: Ext.baseCSSPrefix + 'slider',

        /**
         * @cfg {Array} thumbs An array of {@link Ext.slider.Thumb}'s to be used in this slider.
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
        increment: 1,

        component: {
            xtype: 'slider'
        }
    },

    // @private
    constructor: function(config) {
        config = config || {};

        if (config.hasOwnProperty('values')) {
            config.value = config.values;
        }

        this.callParent([config]);
    },

    // @private
    initialize: function() {
        var me = this;

        me.callParent(arguments);

        me.element.on({
            scope: this,
            tap: 'onTap'
        });

        me.on({
            scope: this,
            painted: 'onPainted',
            show: 'onPainted'
        });

        me.getComponent().on({
            scope: this,
            change: 'onChange'
        });
        
        this.sizeMonitor = Ext.create('Ext.util.SizeMonitor', {
            element : this.element,
            callback: this.onSizeChange,
            scope   : this
        });
    },

    onPainted: function() {
        this.sizeMonitor.refresh();
        this.onSizeChange();
    },

    onSizeChange: function() {
        this.getComponent().refreshThumbs();
    },

    // @private
    applyComponent: function(config) {
        Ext.applyIf(config, {
            value: this.getValue()
        });

        return Ext.factory(config);
    },

    onChange: function(me, thumb, value) {
        this.fireEvent('change', this, thumb, value);
    },

    // @private
    onTap: function(e) {
        var el = Ext.get(e.target);

        if (!el || el.hasCls(Ext.baseCSSPrefix + 'thumb')) {
            return;
        }

        var component = this.getComponent(),
            touchX = e.touch.point.x,
            parent = component.element,
            parentX = parent.getX(),
            offset = touchX - parentX,
            value  = component.getValueForOffset(offset, true),
            thumb  = component.getClosestThumb(value);

        thumb.setValue(value);
    },

    // @private
    updateMinValue: function(value) {
        this.getComponent().setMinValue(value);
    },

    // @private
    updateMaxValue: function(value) {
        this.getComponent().setMaxValue(value);
    },

    // @private
    updateIncrement: function(value) {
        this.getComponent().setIncrement(value);
    },

    // @inherit
    getValue: function() {
        var value;

        if (this.initialized) {
            value = this.getComponent().getValue();
        } else {
            value = this.getInitialConfig().value;
        }

        this._value = value;

        return value;
    },

    /**
     * Convience method. Calls {@link #getValue}
     */
    getValues: function() {
        return this.getValue();
    },

    /**
     * Convience method. Calls {@link #setValue}
     */
    setValues: function(value) {
        return this.setValue(value);
    },

    updateValue: function(value) {
        this.getComponent().setValue(value);
    },

    // Sets the {@link #increment} configuration
    applyIncrement: function(increment) {
        if (increment === 0) {
            increment = 1;
        }

        increment = Math.abs(increment);

        return increment;
    },

    /**
     * Returns an instance of a thumb for a specifed index
     * @param {Number} index The index of the thumb (defaults to 0)
     * @return {Ext.slider.Thumb} The thumb instance
     */
    getThumb: function(index) {
        return this.getComponent().getThumb(index);
    },

    /**
     * Finds the closest thumb for a specified value
     * @return {Ext.slider.Thumb} The thumb
     */
    getClosestThumb: function(value) {
        return this.getClosestThumb(value);
    },

    /**
     * Returns the index of a specified thumb
     * @param {Ext.slider.Thumb} thumb
     * @return {Number} Index of the thumb
     */
    indexOf: function(thumb) {
        return this.getComponent().indexOf(thumb);
    },

    /**
     * Disables the slider
     */
    disable: function() {
        this.callParent();
        this.getComponent().disable();
    },

    /**
     * Enables the slider
     */
    enable: function() {
        this.callParent();
        this.getComponent().enable();
    },

    // @inherit
    reset: function() {
        var component = this.getComponent();

        component.originalValue = this.originalValue;
        component.reset();
        
        this.getValues();
    }
});
