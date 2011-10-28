/**
 * Simple Select field wrapper. Example usage:
<pre><code>
new Ext.field.Select({
    options: [
        {text: 'First Option',  value: 'first'},
        {text: 'Second Option', value: 'second'},
        {text: 'Third Option',  value: 'third'}
    ]
});
</code></pre>
 */
Ext.define('Ext.field.Select', {
    extend: 'Ext.field.Text',
    alias : 'widget.selectfield',
    alternateClassName: 'Ext.form.Select',
    requires: [
        'Ext.Panel',
        'Ext.picker.Picker',
        'Ext.data.Store',
        'Ext.data.StoreManager'
    ],

    /**
     * @event change
     * Fires when an option selection has changed
     * @param {Ext.field.Select} this
     * @param {Mixed} value
     */

    config: {
        // @inherit
        ui: 'select',

        /**
         * @cfg {Number} tabIndex
         * @hide
         * @accessor
         */
        tabIndex: -1,

        /**
         * @cfg {Boolean} useClearIcon
         * @hide
         * @accessor
         */

        /**
         * @cfg {String/Number} valueField The underlying {@link Ext.data.Field#name data value name} (or numeric Array index) to bind to this
         * Select control. (defaults to 'value')
         * @accessor
         */
        valueField: 'value',

        /**
         * @cfg {String/Number} displayField The underlying {@link Ext.data.Field#name data value name} (or numeric Array index) to bind to this
         * Select control. This resolved value is the visibly rendered value of the available selection options.
         * (defaults to 'text')
         * @accessor
         */
        displayField: 'text',

        /**
         * @cfg {Ext.data.Store} store (Optional) store instance used to provide selection options data.
         * @accessor
         */
        store: null,

        /**
         * @cfg {Array} options (Optional) An array of select options.
    <pre><code>
        [
            {text: 'First Option',  value: 'first'},
            {text: 'Second Option', value: 'second'},
            {text: 'Third Option',  value: 'third'}
        ]
    </code></pre>
         * Note: option object member names should correspond with defined {@link #valueField valueField} and {@link #displayField displayField} values.
         * This config will be ignore if a {@link #store store} instance is provided
         * @accessor
         */
        options: null,

        /**
         * @cfg {String} hiddenName Specify a hiddenName if you're using the {@link Ext.form.Panel#standardSubmit standardSubmit} option.
         * This name will be used to post the underlying value of the select to the server.
         * @accessor
         */
        hiddenName: null,

        /**
         * @cfg {Object} input
         * @hide
         * @accessor
         */
        input: {
            useMask: true
        },

        /**
         * @cfg {Boolean} clearIcon
         * @hide
         * @accessor
         */
        clearIcon: false
    },

    /**
     * @private
     */
    record: null,

    // @private
    constructor: function(config) {
        config = config || {};

        if (!config.store) {
            config.store = Ext.create('Ext.data.Store', {
                fields: [config.valueField, config.displayField]
            });
        }

        this.callParent([config]);
    },

    // @private
    initialize: function() {
        this.on({
            scope   : this,
            delegate: 'input',

            masktap: 'onMaskTap'
        });

        this.callParent();
    },

    getValue: function() {
        var record = this.record;

        return (record) ? this.record.get(this.getValueField()) : null;
    },

    updateValue: function(newValue, oldValue) {
        this.callParent(arguments);
        this.fireAction('change', [this, newValue, oldValue]);
    },

    // @private
    getPicker: function() {
        if (!this.picker) {
            this.picker = Ext.create('Ext.picker.Picker', {
                slots: [{
                    align       : 'center',
                    name        : this.getName(),
                    valueField  : this.getValueField(),
                    displayField: this.getDisplayField(),
                    value       : this.getValue(),
                    store       : this.getStore()
                }],
                listeners: {
                    change: this.onPickerChange,
                    scope: this
                }
            });
        }

        return this.picker;
    },

    // @private
    getListPanel: function() {
        if (!this.listPanel) {
            this.listPanel = Ext.create('Ext.Panel', {
                top     : 0,
                left    : 0,
                height  : 200,
                modal   : true,
                cls     : Ext.baseCSSPrefix + 'select-overlay',
                layout  : 'fit',
                hideOnMaskTap: true,
                items: {
                    xtype: 'list',
                    store: this.getStore(),
                    itemTpl: '<span class="x-list-label">{' + this.getDisplayField() + '}</span>',
                    listeners: {
                        select : this.onListSelect,
                        itemtap: this.onListTap,
                        scope  : this
                    }
                }
            });
        }

        return this.listPanel;
    },

    // @private
    onMaskTap: function() {
        if (this.getDisabled()) {
            return false;
        }

        this.showComponent();

        return false;
    },

    // @private
    showComponent: function() {
        if (Ext.os.deviceType === 'Phone') {
            var picker = this.getPicker(),
                name   = this.getName(),
                value  = {};

            value[name] = this.getValue();
            picker.show();
            // picker.setValue(value);
        } else {
            var listPanel = this.getListPanel(),
                list = listPanel.down('list'),
                store = list.getStore(),
                index = store.find(this.getValueField(), this.getValue()),
                record = store.getAt((index == -1) ? 0 : index);

            listPanel.showBy(this);
            list.select(record);
        }
    },

    // @private
    onListSelect: function(item, record) {
        if (record) {
            this.record = record;
            this.setValue(record.get(this.getDisplayField()));
        }
    },

    onListTap: function() {
        this.listPanel.hide({
            type : 'fade',
            out  : true,
            scope: this
        });
    },

    // @private
    onPickerChange: function(picker, value) {
        var currentValue = this.getValue(),
            newValue = value[this.getName()],
            store = this.getStore(),
            index = store.find(this.getValueField(), newValue);
            record = store.getAt(index);
        
        this.record = record;
        this.setValue(record.get(this.getDisplayField()));
    },

    /**
     * Updates the underlying &lt;options&gt; list with new values.
     * @param {Array} options An array of options configurations to insert or append.
<pre><code>
selectBox.setOptions(
    [   {text: 'First Option',  value: 'first'},
        {text: 'Second Option', value: 'second'},
        {text: 'Third Option',  value: 'third'}
    ]).setValue('third');
</code></pre>
     * Note: option object member names should correspond with defined {@link #valueField valueField} and
     * {@link #displayField displayField} values.
     * @return {Ext.field.Select} this
     */
    updateOptions: function(newOptions) {
        var store = this.getStore(),
            record;

        if (!newOptions) {
            store.clearData();
            this.setValue(null);
        }
        else {
            store.loadData(newOptions);

            record = store.getAt(0);
            this.record = record;
            this.setValue(record.get(this.getDisplayField()));
        }
    },

    updateStore: function(newStore) {
        var record = (newStore) ? newStore.getAt(0) : null;

        if (newStore && record) {
            this.record = record;
            this.setValue(record.get(this.getDisplayField()));
        }
    },

    /**
     * Resets the Select field to the value of the first record in the store.
     * @return {Ext.field.Select} this
     */
    reset: function() {
        var store = this.getStore(),
            record = (store) ? store.getAt(0) : null;

        if (store && record) {
            this.record = record;
            this.setValue(record.get(this.getDisplayField()));
        }
        return this;
    },

    destroy: function() {
        this.callParent(arguments);
        Ext.destroy(this.listPanel, this.picker, this.hiddenField);
    }
});
