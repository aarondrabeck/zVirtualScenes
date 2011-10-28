/**
 * Field is the base class for all form fields used in Sencha Touch. It provides a lot of shared functionality to all
 * field subclasses (for example labels, simple validation, {@link #clearIcon clearing} and tab index management), but
 * is rarely used directly. Instead, it is much more common to use one of the field subclasses:
 *
<pre>
xtype            Class
---------------------------------------
textfield        {@link Ext.field.Text}
numberfield      {@link Ext.field.Number}
textareafield    {@link Ext.field.TextArea}
hiddenfield      {@link Ext.field.Hidden}
radiofield       {@link Ext.field.Radio}
checkboxfield    {@link Ext.field.Checkbox}
selectfield      {@link Ext.field.Select}
togglefield      {@link Ext.field.Toggle}
fieldset         {@link Ext.form.FieldSet}
</pre>
 *
 * Fields are normally used within the context of a form and/or fieldset. See the {@link Ext.form.Panel FormPanel}
 * and {@link Ext.form.FieldSet FieldSet} docs for examples on how to put those together, or the list of links above
 * for usage of individual field types. If you wish to create your own Field subclasses you can extend this class,
 * though it is sometimes more useful to extend {@link Ext.field.Text} as this provides additional text entry
 * functionality.
 */
Ext.define('Ext.field.Field', {
    extend: 'Ext.Container',
    alternateClassName: 'Ext.form.Field',
    alias : 'widget.field',
    requires: [
        'Ext.form.Label',
        'Ext.field.Input',
        'Ext.form.ClearIcon'
    ],

    /**
     * Set to true on all Ext.field.Field subclasses. This is used by {@link Ext.form.Panel#getValues} to determine which
     * components inside a form are fields.
     * @property isField
     * @type Boolean
     */
    isField: true,

    // @private
    isFormField: true,

    config: {
        // @inherit
        baseCls: Ext.baseCSSPrefix + 'field',

        /**
         * @cfg {String} label The label to associate with this field.
         * @accessor
         */
        label: null,

        /**
         * @cfg {String} labelCls Optional CSS class to add to the Label element
         * @accessor
         */
        labelCls: null,

        /**
         * @cfg {String} labelAlign The position to render the label relative to the field input. Defaults to 'left'.
         * See {@link Ext.form.Label} for more information
         * @accessor
         */
        labelAlign: null,

        /**
         * @cfg {Number} labelWidth The width to make this field's label (defaults to 30%).
         * See {@link Ext.form.Label} for more information
         * @accessor
         */
        labelWidth: null,

        /**
         * @cfg {Boolean/Object/Ext.field.Input} input An instance of the inner input for this field, if one
         * has been defined.
         * @accessor
         */
        input: null,

        /**
         * @cfg {Boolean} useClearIcon True to use a clear icon in this field
         * @accessor
         */

        /**
         * @cfg {Boolean} clearIcon True to use a clear icon in this field
         * @accessor
         */
        clearIcon: null,

        /**
         * @cfg {Boolean} required True to make this field required. Note: this only causes a visual indication.
         * Doesn't prevent user from submitting the form.
         * @accessor
         */
        required: false,

        /**
         * @cfg {String} requiredCls The className to be applied to this Field when the {@link #required} configuration is set to true
         * @accessor
         */
        requiredCls: Ext.baseCSSPrefix + 'field-required',

        /**
         * <p>The label Element associated with this Field. <b>Only available if a {@link #label} is specified.</b></p>
         * <p>This has been deprecated. Please use {@link #getLabel} instead (which will return a {@link Ext.form.Label} instance).
         * @type Ext.Element
         * @property labelEl
         * @deprecated 2.0
         */

        /**
         * @cfg {String} inputType The type attribute for input fields -- e.g. radio, text, password, file (defaults
         * to 'text'). The types 'file' and 'password' must be used to render those field types currently -- there are
         * no separate Ext components for those.
         * This is now deprecated. Please use 'input.type' instead.
         * @deprecated 2.0
         * @accessor
         */
        inputType: null,

        /**
         * @cfg {String} name The field's HTML name attribute.
         * <b>Note</b>: this property must be set if this field is to be automatically included with
         * {@link Ext.form.Panel#submit form submit()}.
         * @accessor
         */
        name: null,

        /**
         * @cfg {Mixed} value A value to initialize this field with.
         * @accessor
         */
        value: null,

        /**
         * @cfg {Number} tabIndex The tabIndex for this field. Note this only applies to fields that are rendered,
         * not those which are built via applyTo.
         * @accessor
         */
        tabIndex: null
    },

    // @inherit
    constructor: function(config) {
        config = config || {};

        if (config.hasOwnProperty('useClearIcon')) {
            config.clearIcon = config.useClearIcon;
        }

        this.callParent([config]);
    },

    /**
     * Creates a new {@link Ext.form.Label} instance using {@link Ext.Factory}
     * @private
     */
    applyLabel: function(config) {
        if (typeof config == "string") {
            config = {
                text: config
            };
        }

        return Ext.factory(config, 'Ext.form.Label', this.getLabel());
    },

    /**
     * Adds the new {@link Ext.form.Label} instance into this field
     * @private
     */
    updateLabel: function(newLabel, oldLabel) {
        if (newLabel) {
            this.add(newLabel);

            //add a listener for any alignment changes
            newLabel.on('alignchange', this.onLabelAlignChange, this);

            //set the label class
            this.onLabelAlignChange(newLabel, newLabel.getAlign());
        }
    },

    /**
     * @private
     */
    updateLabelAlign: function(newLabelAlign) {
        var label = this.getLabel();
        if (label) {
            label.setAlign(newLabelAlign);
        }
    },

    /**
     * @private
     */
    updateLabelCls: function(newLabelCls) {
        var label = this.getLabel();
        if (label) {
            label.setCls(newLabelCls);
        }
    },

    /**
     * @private
     */
    updateLabelWidth: function(newLabelWidth) {
        var label = this.getLabel();
        if (label) {
            label.setWidth(newLabelWidth);
        }
    },

    /**
     * Updates the class on this field when the label alignment changes
     * @private
     */
    onLabelAlignChange: function(label, newAlign, oldAlign) {
        var prefix       = Ext.baseCSSPrefix + 'label-align-',
            element = this.element;

        if (oldAlign) {
            element.removeCls(prefix + oldAlign);
        }

        element.addCls(prefix + newAlign);
    },

    /**
     * Creates a new {@link Ext.field.Input} instance using {@link Ext.Factory}
     * @private
     */
    applyInput: function(config) {
        return Ext.factory(config, Ext.field.Input, this.getInput());
    },

    /**
     * Adds the new {@link Ext.field.Input} instance into this field
     * @private
     */
    updateInput: function(newInput) {
        if (newInput) {
            this.add(newInput);
        }
    },

    /**
     * Creates a new {@link Ext.form.ClearIcon} instance using {@link Ext.Factory}
     * @private
     */
    applyClearIcon: function(config) {
        if (config === true) {
            config = {
                hidden: true
            };
        }

        return Ext.factory(config, 'Ext.form.ClearIcon', this.getClearIcon());
    },

    /**
     * Adds the new {@link Ext.form.ClearIcon} instance into this field
     * @private
     */
    updateClearIcon: function(newClearIcon) {
        if (newClearIcon) {
            this.add(newClearIcon);
        }
    },

    /**
     * Updates the {@link #required} configuration
     * @private
     */
    updateRequired: function(newRequired) {
        this.element[newRequired ? 'addCls' : 'removeCls'](this.getRequiredCls());
    },

    /**
     * Updates the {@link #required} configuration
     * @private
     */
    updateRequiredCls: function(newRequiredCls, oldRequiredCls) {
        if (this.getRequired()) {
            this.element.replaceCls(oldRequiredCls, newRequiredCls);
        }
    },

    /**
     * @private
     */
    updateInputType: function(newInputType) {
        var input = this.getInput();
        if (input) {
            input.setType(newInputType);
        }
    },

    /**
     * @private
     */
    updateName: function(newName) {
        var input = this.getInput();
        if (input) {
            input.setName(newName);
        }
    },

    /**
     * Returns the field data value
     * @return {String} The field value
     */
    getValue: function() {
        var input = this.getInput();
        if (input) {
            // we need to get the latest value from the {@link #input} and then update the value
            var value = input.getValue();
            this._value = value;
        }

        return this._value;
    },

    /**
     * @private
     */
    updateValue: function(newValue) {
        var input = this.getInput();
        if (input) {
            input.setValue(newValue);
        }
    },

    /**
     * @private
     */
    updateTabIndex: function(newTabIndex) {
        var input = this.getInput();
        if (input) {
            input.setTabIndex(newTabIndex);
        }
    },

    // @inherit
    doSetDisabled: function(disabled) {
        this.callParent(arguments);

        var input = this.getInput(),
            label = this.getLabel();

        if (input) {
            input.setDisabled(disabled);
        }

        if (label) {
            label.setDisabled(disabled);
        }
    },

    // @private
    initialize: function() {
        var me = this;
        me.callParent(arguments);

        me.doInitValue();
    },

    /**
     * @private
     */
    doInitValue: function() {
        /**
         * @property {Mixed} originalValue
         * The original value of the field as configured in the {@link #value} configuration.
         * setting is <code>true</code>.
         */
        this.originalValue = this.getValue();
    },

    /**
     * Resets the current field value to the originally loaded value and clears any validation messages.
     */
    reset: function() {
        this.getInput().reset();
        //we need to call this to sync the input with this field
        this.getValue();
    },

    /**
     * <p>Returns true if the value of this Field has been changed from its {@link #originalValue}.
     * Will return false if the field is disabled or has not been rendered yet.</p>
     * @return {Boolean} True if this field has been changed from its original value (and
     * is not disabled), false otherwise.
     */
    isDirty: function() {
        var input = this.getInput();
        if (input) {
            return input.isDirty();
        }
        return false;
    }
}, function() {
    //<deprecated product=touch since=2.0>
    var prototype = this.prototype;

    this.override({
        constructor: function(config) {
            config = config || {};

            // helper method for deprecating a property
            var deprecateProperty = function(property, obj, newProperty) {
                if (config.hasOwnProperty(property)) {
                    if (obj) {
                        config[obj] = config[obj] || {};
                        config[obj][(newProperty) ? newProperty : property] = config[obj][(newProperty) ? newProperty : property] || config[property];
                    } else {
                        config[newProperty] = config[property];
                    }

                    delete config[property];

                    //<debug warn>
                    Ext.Logger.deprecate("'" + property + "' config is deprecated, use the '" + ((obj) ? obj + "." : "") + ((newProperty) ? newProperty : property) + "' config instead", 2);
                    //</debug>
                }
            };

            // {@link #input}
            /**
             * @member Ext.field.Field
             * @cfg {String} inputCls CSS class to add to the input element
             * @deprecated 2.0.0 Deprecated, please use {@link #input}.inputCls
             */
            deprecateProperty('inputCls', 'input', 'inputCls');
            
            /**
             * @member Ext.field.Field
             * @cfg {String} fieldCls CSS class to add to the field
             * @deprecated 2.0.0 Deprecated, please use {@link #input}.inputCls
             */
            deprecateProperty('fieldCls', 'input', 'inputCls');

            /**
             * @member Ext.field.Field
             * @cfg {String} fieldLabel The label for this Field
             * @deprecated 2.0.0 Please use the {@link #label} configuration instead
             */
            deprecateProperty('fieldLabel', null, 'label');

            //<debug warn>
            if (config.hasOwnProperty('autoCreateField')) {
                Ext.Logger.deprecate("'autoCreateField' config is deprecated. If you are subclassing Ext.field.Field and you do not want a Ext.field.Input, set the 'input' config to false.", this);
            }
            //</debug>

            this.callOverridden(arguments);
        }
    });

    prototype.__defineGetter__('fieldEl', function() {
        //<debug warn>
        Ext.Logger.deprecate("'fieldEl' is deprecated, please use getInput() to get an instance of Ext.field.Field instead", this);
        //</debug>

        return this.getInput().input;
    });

    prototype.__defineGetter__('labelEl', function() {
        //<debug warn>
        Ext.Logger.deprecate("'labelEl' is deprecated, please use getLabel() to get an instance of Ext.form.Label instead", this);
        //</debug>

        return this.getLabel().element;
    });
    //</deprecated>
});
