/**
 * Specialized Slider with a single thumb and only two values. By default the toggle component can
 * be switched between the values of 0 and 1.
 */
Ext.define('Ext.field.Toggle', {
    extend: 'Ext.field.Slider',
    alias : 'widget.togglefield',
    alternateClassName: 'Ext.form.Toggle',

    config: {
        // @inherit
        cls: 'x-toggle',

        // @inherit
        minValue: 0,

        // @inherit
        maxValue: 1,

        /**
         * @cfg {String} minValueCls CSS class added to the field when toggled to its minValue
         * @accessor
         */
        minValueCls: Ext.baseCSSPrefix + 'toggle-off',

        /**
         * @cfg {String} maxValueCls CSS class added to the field when toggled to its maxValue
         * @accessor
         */
        maxValueCls: Ext.baseCSSPrefix + 'toggle-on'

        // // @inherit
        // animationDuration: 70
    },

    /**
     * @private
     */
    onChange: function(thumb, newValue) {
        var me     = this,
            isOn   = newValue > 0,
            onCls  = me.getMaxValueCls(),
            offCls = me.getMinValueCls();

        this.addCls(isOn ? onCls : offCls);
        this.removeCls(isOn ? offCls : onCls);
    },

    onTap: function(e) {
        var value = (this.getValue() > 0) ? 0 : 1;
        this.setValue(value);
    }
});