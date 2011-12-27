/**
 * List is a custom styled DataView which allows Grouping, Indexing, Icons, and a Disclosure.
 *
 * # Example:
 *
 * Here is an example of the usage in a {@link Ext.List}:
 *
 *     @example miniphone preview
 *     Ext.regModel('Contact', {
 *        fields: ['firstName', 'lastName']
 *     });
 *
 *     var store = new Ext.data.JsonStore({
 *        model: 'Contact',
 *        sorters: 'lastName',
 *
 *        getGroupString: function(record) {
 *            return record.get('lastName')[0];
 *        },
 *
 *        data: [
 *            {firstName: 'Tommy',   lastName: 'Maintz'},
 *            {firstName: 'Rob',     lastName: 'Dougan'},
 *            {firstName: 'Ed',      lastName: 'Spencer'},
 *            {firstName: 'Jamie',   lastName: 'Avins'},
 *            {firstName: 'Aaron',   lastName: 'Conran'},
 *            {firstName: 'Dave',    lastName: 'Kaneda'},
 *            {firstName: 'Jacky',   lastName: 'Nguyen'},
 *            {firstName: 'Abraham', lastName: 'Elias'},
 *            {firstName: 'Jay',     lastName: 'Robinson'},
 *            {firstName: 'Nigel',   lastName: 'White'},
 *            {firstName: 'Don',     lastName: 'Griffin'},
 *            {firstName: 'Nico',    lastName: 'Ferrero'},
 *            {firstName: 'Nicolas', lastName: 'Belmonte'},
 *            {firstName: 'Jason',   lastName: 'Johnston'}
 *        ]
 *     });
 *
 *     var list = new Ext.List({
 *        fullscreen: true,
 *        itemTpl: '<div class="contact">{firstName} <strong>{lastName}</strong></div>',
 *        store: store
 *     });
 *
*/
Ext.define('Ext.dataview.List', {
    alternateClassName: 'Ext.List',
    extend: 'Ext.dataview.DataView',
    xtype : 'list',

    requires: [
        'Ext.dataview.IndexBar',
        'Ext.dataview.ListItemHeader'
    ],

    config: {
        /**
         * @cfg {Boolean/Object} indexBar
         * True to render an alphabet IndexBar docked on the right.
         * This can also be a config object that will be passed to {@link Ext.IndexBar}
         * (defaults to false)
         * @accessor
         */
        indexBar: false,

        disclosure: null,

        icon: null,

        /**
         * @cfg {Boolean} clearSelectionOnDeactivate
         * True to clear any selections on the list when the list is deactivated (defaults to true).
         * @accessor
         */
        clearSelectionOnDeactivate: true,

        /**
         * @cfg {Boolean} preventSelectionOnDisclose True to prevent the item selection when the user
         * taps a disclose icon. Defaults to <tt>true</tt>
         * @accessor
         */
        preventSelectionOnDisclose: true,

        // @inherit
        baseCls: Ext.baseCSSPrefix + 'list',

        /**
         * @cfg {Boolean} pinHeaders
         * Whether or not to pin headers on top of item groups while scrolling for an iPhone native list experience.
         * Defaults to <tt>false</tt> on Android and Blackberry (for performance reasons)
         * Defaults to <tt>true</tt> on other devices.
         * @accessor
         */
        pinHeaders: true,

        grouped: false,

        /**
         * @cfg {Boolean/Function/Object} onItemDisclosure
         * True to display a disclosure icon on each list item.
         * This won't bind a listener to the tap event. The list
         * will still fire the disclose event though.
         * By setting this config to a function, it will automatically
         * add a tap event listeners to the disclosure buttons which
         * will fire your function.
         * Finally you can specify an object with a 'scope' and 'handler'
         * property defined. This will also be bound to the tap event listener
         * and is useful when you want to change the scope of the handler.
         * @accessor
         */
        onItemDisclosure: null
    },

    constructor: function() {
        this.translateHeader = (Ext.os.is.Android2) ? this.translateHeaderCssPosition : this.translateHeaderTransform;
        this.callParent(arguments);
    },

    initialize: function() {
        var me = this;
        me.callParent(arguments);
        me.elementContainer.element.on({
            delegate: '.' + this.getBaseCls() + '-disclosure',
            tap: 'handleItemDisclosure',
            scope: me
        });
    },

    applyIndexBar: function(indexBar) {
        if (this.getGrouped()) {
            return Ext.factory(indexBar, Ext.dataview.IndexBar, this.getIndexBar());
        }
    },

    updateIndexBar: function(indexBar) {
        if (indexBar && this.getScrollable()) {
            this.getScrollableBehavior().getScrollView().getElement().appendChild(indexBar.renderElement);

            indexBar.on({
                index: 'onIndex',
                scope: this
            });

            this.addCls(this.getBaseCls() + '-indexed');
        }
    },

    updatePinHeaders: function(pinnedHeaders) {
        var scrollable = this.getScrollable(),
            store = this.getStore(),
            scrollView = this.getScrollableBehavior().getScrollView(),
            scrollViewElement = scrollView.getElement(),
            header, scroller;

        if (scrollable && this.getGrouped()) {
            scroller = scrollable.getScroller();
            if (pinnedHeaders) {
                scroller.on({
                    refresh: 'doRefreshHeaders',
                    scroll: 'onScroll',
                    scope: this
                });

                store.on({
                    datachanged: 'doRefreshHeaders',
                    scope: this
                });

                this.header = header = Ext.create('Ext.dataview.ListItemHeader', {html: ' ', cls: 'x-list-header-swap'});
                scrollViewElement.dom.insertBefore(header.element.dom, scroller.getContainer().dom.nextSibling);
            } else {
                scroller.un({
                    refresh: 'onScrollerRefresh',
                    scroll: 'onScroll',
                    scope: this
                });

                store.un({
                    datachanged: 'doRefreshHeaders',
                    scope: this
                });

                if (this.header) {
                    this.header.destroy();
                }
            }
        }
    },

    onStoreClear: function() {
        this.callParent();
        if (this.header) {
            this.header.destroy();
        }
    },

    // @private
    getClosestGroups : function() {
        var groups = this.pinHeaderInfo.offsets,
            pos = this.getScrollable().getScroller().position,
            ln = groups.length,
            i = 0,
            group, current, next;

        for (; i < ln; i++) {
            group = groups[i];
            if (group.offset > pos.y) {
                next = group;
                break;
            }
            current = group;
        }

        return {
            current: current,
            next: next
        };
    },

    doRefreshHeaders: function() {
        var headerIndices = this.findGroupHeaderIndices(),
            ln = headerIndices.length,
            items = this.getViewItems(),
            headerInfo = this.pinHeaderInfo = {offsets: []},
            headerOffsets = headerInfo.offsets,
            i, headerItem, header;

        if (ln) {
            for (i = 0; i < ln; i++) {
                headerItem = items[headerIndices[i]];
                if (headerItem) {
                    header = this.getItemHeader(headerItem);

                    headerOffsets.push({
                        header: header,
                        offset: headerItem.offsetTop
                    });
                }
            }

            headerInfo.closest = this.getClosestGroups();
            this.setActiveGroup(headerInfo.closest.current);
            if (header) {
                headerInfo.headerHeight = Ext.get(header).getHeight();
            }
        }
    },

    getItemHeader: function(item) {
        return item.childNodes[0];
    },

    onScroll: function(scroller, x, y) {
        var me = this,
            headerInfo = me.pinHeaderInfo,
            closest = headerInfo.closest,
            activeGroup = me.activeGroup,
            headerHeight = headerInfo.headerHeight,
            next = closest.next,
            current = closest.current;

        if (y <= 0) {
            if (activeGroup) {
                me.setActiveGroup(false);
                closest.next = current;
            }
            return;
        }
        else if ((next && y > next.offset) || (current && y < current.offset)) {
            closest = headerInfo.closest = this.getClosestGroups();
            next = closest.next;
            current = closest.current;
            this.setActiveGroup(current);
        }

        if (next && y > 0 && next.offset - y <= headerHeight) {
            var headerOffset = headerHeight - (next.offset - y);
            this.translateHeader(headerOffset);
        }
        else {
            this.translateHeader(null);
        }
    },

    translateHeaderTransform: function(offset) {
        this.header.renderElement.dom.style.webkitTransform = (offset === null) ? null : 'translate3d(0px, -' + offset + 'px, 0px)';
    },

    translateHeaderCssPosition: function(offset) {
        this.header.renderElement.dom.style.top = (offset === null) ? null : '-' + Math.round(offset) + 'px';
    },

    /**
     * Set the current active group
     * @param {Object} group The group to set active
     * @private
     */
    setActiveGroup : function(group) {
        var me = this;
        if (group) {
            if (!me.activeGroup || me.activeGroup.header != group.header) {
                me.header.setHtml(group.header.innerHTML);
                me.header.show();
            }
        } else {
            me.header.hide();
        }

        this.activeGroup = group;
    },

    onIndex: function(index) {
        var key = index.toLowerCase(),
            store = this.getStore(),
            groups = store.getGroups(),
            ln = groups.length,
            scroller = this.getScrollable().getScroller(),
            group, i, closest, id, item;

        for (i = 0; i < ln; i++) {
            group = groups[i];
            id = group.name.toLowerCase();
            if (id == key || id > key) {
                closest = group;
                break;
            }
            else {
                closest = group;
            }
        }

        item = this.getViewItems()[store.indexOf(closest.children[0])];

        //stop the scroller from scrolling
        scroller.stopAnimation();

        //make sure the new offsetTop is not out of bounds for the scroller
        var containerSize = scroller.getContainerSize().y,
            size = scroller.getSize().y,
            maxOffset = size - containerSize,
            offset = (item.offsetTop > maxOffset) ? maxOffset : item.offsetTop;

        scroller.scrollTo(0, offset);
    },

    applyOnItemDisclosure: function(config) {
        if (Ext.isFunction(config)) {
            return {
                scope: this,
                handler: config
            };
        }
        if (Ext.isObject(config)) {
            return config;
        }
        return null;
    },

    getDisclosure: function() {
        var value = this._disclosure,
            onItemDisclosure = this.getOnItemDisclosure();

        if (onItemDisclosure && onItemDisclosure != value) {
            value = true;
            this.setDisclosure(value);
        }

        return value;
    },

    updateOnItemDisclosure: function(newOnItemDisclosure) {
        // If we have an onItemDisclosure configuration, force disclose config to true
        if (newOnItemDisclosure) {
            this.setDisclosure(true);
        }
    },

    handleItemDisclosure: function(e) {
        var me = this,
            item = e.getTarget().parentNode,
            index = item.getAttribute('itemIndex'),
            record = me.getStore().getAt(index),
            onItemDisclosure = me.getOnItemDisclosure();

        if (me.getPreventSelectionOnDisclose()) {
            e.stopEvent();
        }
        me.fireAction('disclose', [record, item, index, e], 'doDisclose');

        if (onItemDisclosure && onItemDisclosure.handler) {
            onItemDisclosure.handler.call(me, record, item, index);
        }
    },

    updateBaseCls: function(newBaseCls, oldBaseCls) {
        var me = this;
        me.callParent(arguments);
        me.itemClsShortCache = newBaseCls + '-item';

        me.headerClsShortCache = newBaseCls + '-header';
        me.headerClsCache = '.' + me.headerClsShortCache;

        me.labelClsShortCache = newBaseCls + '-item-label';
        me.labelClsCache = '.' + me.labelClsShortCache;

        me.disclosureClsShortCache = newBaseCls + '-disclosure';
        me.disclosureClsCache = '.' + me.disclosureClsShortCache;

        me.iconClsShortCache = newBaseCls + '-icon';
        me.iconClsCache = '.' + me.iconClsShortCache;
    },

    hiddenDisplayCache: Ext.baseCSSPrefix + 'hidden-display',

    doDisclose: Ext.emptyFn,

    updateListItem: function(record, item) {
        var extItem = Ext.get(item),
            innerItem = extItem.down(this.labelClsCache, true),
            index = this.getStore().indexOf(record),
            data = record.data,
            disclosure = data && data.hasOwnProperty('disclosure'),
            iconSrc = data && data.hasOwnProperty('iconSrc'),
            disclosureEl, iconEl;

        item.setAttribute('itemIndex', index);
        innerItem.innerHTML = this.getItemTpl().apply(data);

        if (this.getDisclosure() && disclosure) {
            disclosureEl = extItem.down(this.disclosureClsCache);
            disclosureEl[disclosure ? 'removeCls' : 'addCls'](this.hiddenDisplayCache);
        }

        if (this.getIcon()) {
            iconEl = extItem.down(this.iconClsCache, true);
            iconEl.style.backgroundImage = iconSrc ? 'url(' + iconSrc + ')' : '';
        }
    },

    getItemElementConfig: function(index, data) {
        var config = {
                cls: this.itemClsShortCache,
                itemIndex: index,
                children: [{
                    cls: this.labelClsShortCache,
                    html: this.getItemTpl().apply(data)
                }]
            },
            iconSrc;

        if (this.getIcon()) {
            iconSrc = data.iconSrc;
            config.children.push({
                cls: this.iconClsShortCache,
                style: 'background-image: ' + iconSrc ? 'url(' + iconSrc + ')' : ''
            });
        }

        if (this.getDisclosure()) {
            config.children.push({
                cls: this.disclosureClsShortCache + ((data.disclosure === false) ? this.hiddenDisplayCache : '')
            });
        }
        return config;
    },

    findGroupHeaderIndices: function() {
        if (!this.getGrouped()) {
            return;
        }
        var me = this,
            store = me.getStore(),
            groups = store.getGroups(),
            groupLn = groups.length,
            items = me.getViewItems(),
            existingHeaders = me.elementContainer.element.query(me.headerClsCache),
            existingHeadersLn = existingHeaders.length,
            newHeaderItems = [],
            i, firstGroupedRecord, index, item;

        // Remove headers
        for (i = 0; i < existingHeadersLn; i++) {
            Ext.removeNode(existingHeaders[i]);
        }

        // Add header
        for (i = 0; i < groupLn; i++) {
            firstGroupedRecord = groups[i].children[0];
            index = store.indexOf(firstGroupedRecord);
            item = items[index];
            me.doAddHeader(item, store.getGroupString(firstGroupedRecord));
            newHeaderItems.push(index);
        }

        return newHeaderItems;
    },

    doAddHeader: function(item, html) {
        Ext.get(item).insertFirst(Ext.Element.create({
            cls: this.headerClsShortCache,
            html: html
        }));
    }
}, function() {
    //TODO This is hacky, find a better way @Jacky
    var prototype = this.prototype;

    prototype.cachedConfigList = prototype.cachedConfigList.slice();
    Ext.Array.remove(prototype.cachedConfigList, 'baseCls');
});
