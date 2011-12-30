/**
 * A simple utility class for generically masking elements while loading data.  If the {@link #store}
 * config option is specified, the masking will be automatically synchronized with the store's loading
 * process and the mask element will be cached for reuse.
 * <p>Example usage:</p>
 *<pre><code>
// Basic mask:
var myMask = new Ext.LoadMask(Ext.getBody(), {msg:"Please wait..."});
myMask.show();
</code></pre>
 */
Ext.define('Ext.LoadMask', {
    requires: ['Ext.data.StoreManager'],

    mixins: {
        observable: 'Ext.util.Observable'
    },

    config: {
        /**
         * @cfg {String} msg
         * The text to display in a centered loading message box (defaults to 'Loading...')
         * @accessor
         */
        msg: 'Loading...',

        /**
         * @cfg {String} msgCls
         * The CSS class to apply to the loading message element (defaults to "x-mask-loading")
         * @accessor
         */
        msgCls: Ext.baseCSSPrefix + 'mask-loading'
    },

    /**
     * @cfg {Ext.data.Store} store
     * Optional Store to which the mask is bound. The mask is displayed when a load request is issued, and
     * hidden on either load sucess, or load fail.
     */

    /**
     * Read-only. True if the mask is currently disabled so that it will not be displayed
     * @type Boolean
     */
    disabled: false,

    /**
     * Sets the {@link #msg} configuration
     */
    applyMsg: function(msg) {
        if (this.el) {
            var me = this,
                dom = me.el.dom,
                mask = Ext.Element.data(dom, 'mask');

            //check if the mask exists
            if (mask) {
                var maskEl = el.child('.x-loading-msg');
                if (maskEl) {
                    maskEl.update(msg);
                }
            }
        }

        return msg;
    },

    /**
     * Sets the {@link #msgCls} configuration
     */
    applyMsgCls: function(msgCls) {
        if (this.el) {
            var me = this,
                dom = me.el.dom,
                mask = Ext.Element.data(dom, 'mask');

            //check if the mask exists
            if (mask) {
                // TODO this needs to check where the actual maskElement is. mask.addCls
                // var maskEl = el.child('.x-loading-msg');
                // if (maskEl) {
                //     maskEl.update(msg);
                // }
            }
        }

        return msgCls;
    },

    /**
     * Creates new LoadMask.
     * @param {Mixed} el The element or DOM node, or its id
     * @param {Object} config The config object
     */
    constructor: function(el, config) {
        var me = this;

        me.el = Ext.get(el);
        Ext.apply(me, config);

        me.addEvents('show', 'hide');
        if (me.store) {
            me.bindStore(me.store, true);
        }

        me.callParent();

        me.mixins.observable.constructor.call(me);
    },

    /**
     * Changes the data store bound to this LoadMask.
     * @param {Ext.data.Store} store The store to bind to this LoadMask
     */
    bindStore: function(store, initial) {
        if (!initial && this.store) {
            this.mun(this.store, {
                scope: this,
                beforeload: this.onBeforeLoad,
                load: this.onLoad,
                exception: this.onLoad
            });

            if (!store) {
                this.store = null;
            }
        }

        if (store) {
            store = Ext.StoreMgr.lookup(store);
            this.mon(store, {
                scope: this,
                beforeload: this.onBeforeLoad,
                load: this.onLoad,
                exception: this.onLoad
            });
        }

        this.store = store;
        if (store && store.isLoading()) {
            this.onBeforeLoad();
        }
    },

    /**
     * Disables the mask to prevent it from being displayed
     */
    disable: function() {
       this.setDisabled(true);
    },

    /**
     * Enables the mask so that it can be displayed
     */
    enable: function() {
        this.setDisabled(false);
    },

    /**
     * Method to determine whether this LoadMask is currently disabled.
     * @return {Boolean} the disabled state of this LoadMask.
     */
    isDisabled: function() {
        return this.getDisabled();
    },

    // @private
    onLoad: function() {
        this.el.unmask();
        this.fireEvent('hide', this, this.el, this.store);
    },

    // @private
    onBeforeLoad: function() {
        if (!this.disabled) {
            this.el.mask(Ext.LoadingSpinner + '<div class="x-loading-msg">' + this.msg + '</div>', this.msgCls, false);
            this.fireEvent('show', this, this.el, this.store);
        }
    },

    /**
     * Show this LoadMask over the configured Element.
     */
    show: function() {
        this.onBeforeLoad();
    },

    /**
     * Hide this LoadMask.
     */
    hide: function() {
        this.onLoad();
    },

    // private
    destroy: function() {
        this.hide();
        this.clearListeners();
    }
}, function() {
    Ext.LoadingSpinner = '<div class="x-loading-spinner"><span class="x-loading-top"></span><span class="x-loading-right"></span><span class="x-loading-bottom"></span><span class="x-loading-left"></span></div>';
});

