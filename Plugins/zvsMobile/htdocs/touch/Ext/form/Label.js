/**
 * The Label class is a simple component used to render the labels for each field in your Sencha Touch application. It
 * is usually created for you when you create a field, like this:
 * 
 *     Ext.create('Ext.field.Text', {
 *         label: 'Your Name',
 *         value: 'Ed Spencer'
 *     });
 * 
 * You can also specify a few more configurations for a label by passing a label object instead of a string:
 * 
 *     Ext.create('Ext.field.Text', {
 *         label: {
 *             {@link #text}: 'Your Name',
 *             {@link #align}: 'right',
 *             {@link #width}: '50%'
 *         },
 *         value: 'Ed Spencer'
 *     });
 * 
 * It's rare to want to create a label without a form field but you can do it like this:
 * 
 *     Ext.create('Ext.form.Label', {
 *         text: 'My Label',
 *         width: 100
 *     });
 * 
 */
Ext.define('Ext.form.Label', {
    extend: 'Ext.Component',
    xtype : 'label',
    
    config: {
        // @inherit
        baseCls: Ext.baseCSSPrefix + 'form-label',

        /**
         * @cfg {String} text The text to display in the label
         * @accessor
         */
        text: '&nbsp',

        /**
         * @cfg {Mixed} width The width of the label, can be any valid CSS size. E.g '20%', '6em', '100px'.
         * @accessor
         */
        width: '30%'
    },

    eventedConfig: {
        /**
         * @cfg {String} align The location to render the label of the field. Acceptable values are 'top', 'left' and 'right'.
         * @accessor
         * @evented
         */
        align: 'left'
    },

    // @private
    template: [
        {
            reference: 'textEl',
            tag: 'span'
        }
    ],

    /**
     * Updates the {@link #text} configuration
     */
    updateText: function(newText) {
        this.textEl.setHtml(newText);
    },

    /**
     * Updates the {@link #align} configuration by changing the {@link #docked} configuration on the component
     * @private
     */
    doSetAlign: function(newAlign) {
        this.setDocked(newAlign);

        //reset the width of the label, has to be 100%
        if (newAlign && this.getWidth() && ['top', 'bottom'].indexOf(newAlign) != -1) {
            this.setWidth('100%');
        }
    }
});
