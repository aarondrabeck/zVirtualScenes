/**
 * @class Ext.carousel.Carousel
 * @extends Ext.Panel
 *
 * Carousels, like [tabs](#!/guide/tabs), are a great way to allow the user to swipe through multiple full-screen pages.
 * A Carousel shows only one of its pages at a time but allows you to swipe through with your finger.
 * 
 * Carousels can be oriented either horizontally or vertically and are easy to configure - they just work like any other
 * Container. Here's how to set up a simple horizontal Carousel:
 * 
 *     @example
 *     Ext.create('Ext.Carousel', {
 *         fullscreen: true,
 * 
 *         defaults: {
 *             styleHtmlContent: true
 *         },
 * 
 *         items: [
 *             {
 *                 html : 'Item 1',
 *                 style: 'background-color: #5E99CC'
 *             },
 *             {
 *                 html : 'Item 2',
 *                 style: 'background-color: #759E60'
 *             },
 *             {
 *                 html : 'Item 3'
 *             }
 *         ]
 *     });
 * 
 * We can also make Carousels orient themselves vertically:
 *
 *     @example preview
 *     Ext.create('Ext.Carousel', {
 *         fullscreen: true,
 *         direction: 'vertical',
 *     
 *         defaults: {
 *             styleHtmlContent: true
 *         },
 *     
 *         items: [
 *             {
 *                 html : 'Item 1',
 *                 style: 'background-color: #759E60'
 *             },
 *             {
 *                 html : 'Item 2',
 *                 style: 'background-color: #5E99CC'
 *             }
 *         ]
 *     });
 * 
 * ### Common Configurations
 * * {@link #ui} defines the style of the carousel
 * * {@link #direction} defines the direction of the carousel
 * * {@link #indicator} defines if the indicator show be shown
 *
 * ### Useful Methods
 * * {@link #next} moves to the next card
 * * {@link #previous} moves to the previous card
 * * {@link #setActiveItem} moves to the passed card
 *
 * ## Further Reading
 * 
 * For more information about Carousels see the [Carousel guide](#!/guide/carousel).
 */
Ext.define('Ext.carousel.Carousel', {
    extend: 'Ext.dataview.ComponentView',
    xtype: 'carousel',

    alternateClassName: 'Ext.Carousel',

    requires: ['Ext.carousel.Indicator', 'Ext.util.SizeMonitor'],

    config: {
        /**
         * @cfg {String} baseCls
         * The base CSS class to apply to the Carousel's element
         * @accessor
         */
        baseCls: Ext.baseCSSPrefix + 'carousel',

        /**
         * @cfg {String} itemCls
         * The CSS class to apply each card in the carousel
         * @accessor
         */
        itemCls: Ext.baseCSSPrefix + 'carousel-item',

        /**
         * @cfg {String} ui
         * Style options for Carousel. Default is 'dark'. 'light' is also available.
         * @accessor
         */
        ui: 'dark',

        /**
         * @cfg {Boolean} indicator
         * Provides an indicator while toggling between child items to let the user
         * know where they are in the card stack.
         * @accessor
         */
        indicator: true,

        /**
         * @cfg {String} direction
         * The direction of the Carousel. Default is 'horizontal'. 'vertical' also available.
         * @accessor
         */
        direction: 'horizontal',

        scrollable: false
    },

    // @private
    initialize: function() {
        this.callParent();

        this.element.on({
            drag     : 'onDrag',
            dragstart: 'onDragStart',
            dragend  : 'onDragEnd',

            scope: this
        });

        this.on({
            painted         : 'onPainted',
            activeitemchange: 'onActiveItemChange',

            scope: this
        });
    },

    // @private
    updateCardSize: function() {
        this.currentSize = this.element.getSize();
        this.currentScroll = {
            x: 0,
            y: 0
        };

        var cards = this.getInnerItems(),
            ln = cards.length,
            i, card;

        for (i = 0; i < ln; i++) {
            card = cards[i];
            if (this.isIndexInRange(i)) {
                this.updateCardPosition(card);
            }
        }
    },

    // @private
    onPainted: function() {
        this.updateCardSize();

        if (!this.sizeMonitor) {
            // @TODO: this should be working but somehow its not
            this.sizeMonitor = new Ext.util.SizeMonitor({
                element: this.element,
                callback: this.updateCardSize,
                scope: this
            });
        } else {
            this.sizeMonitor.refresh();
        }
    },

    // @private
    onAdd: function(item, index) {
        if (!item.isInnerItem()) {
            return this.callParent(arguments);
        }

        item.element.addCls(this.getItemCls());
        item.hide();

        this.callParent(arguments);

        var isCardInRange = this.isCardInRange(item),
            activeItem = this.getActiveItem(),
            indicator = this.getIndicator();

        if (isCardInRange) {
            if (this.isPainted() && activeItem !== item) {
                this.updateCardPosition(item);
            }

            item.show();
        }

        if (indicator) {
            indicator.addIndicator();
        }
    },

    // @private
    onRemove: function(item, index) {
        if (item.isInnerItem()) {
            if (!this.isCardInRange(item)) {
                item.show();
            }
            item.element.removeCls(this.getItemCls());
        }

        this.callParent(arguments);

        var indicator = this.getIndicator();
        if (indicator) {
            indicator.addIndicator();
        }
    },

    // @private
    updateCardPosition: function(card, index, activeIndex) {
        card.element.dom.style.webkitTransform = this.getCardTransform(this.getCardOffset(card, index, activeIndex));
    },

    // @private
    getCardTransform: function(offset) {
        if (this.getDirection() === 'horizontal') {
            return 'translate3d(' + offset + 'px, 0px, 0px)';
        } else {
            return 'translate3d(0px, ' + offset + 'px, 0px)';
        }
    },

    /**
     * Returns the amount of pixels from the current drag to a card.
     * @private
     */
    getCardOffset: function(card, index, activeIndex) {
        var cardOffset = this.getCardIndexOffset(card, index, activeIndex),
            currentSize = this.currentSize,
            currentScroll = this.currentScroll;

        return this.getDirection() === 'horizontal' ?
            (cardOffset * currentSize.width) + currentScroll.x :
            (cardOffset * currentSize.height) + currentScroll.y;
    },

    /**
     * Returns the difference between the index of the active card and the passed card.
     * @private
     */
    getCardIndexOffset: function(card, index, activeIndex) {
        if (index === undefined) {
            index = this.getInnerItems().indexOf(card);
        }
        if (activeIndex === undefined) {
            activeIndex = this.getActiveIndex();
        }
        return index - activeIndex;
    },

    /**
     * Returns true if the passed card is within 2 cards from the active card.
     * @private
     */
    isCardInRange: function(card) {
        return Math.abs(this.getCardIndexOffset(card)) <= 1;
    },

    // @private
    isIndexInRange: function(index, activeIndex) {
        if (activeIndex === undefined) {
            activeIndex = this.getActiveIndex();
        }
        return Math.abs(index - activeIndex) <= 1;
    },

    /**
     * Returns the index of the currently active card.
     * @return {Number} The index of the currently active card.
     */
    getActiveIndex: function() {
        return this.getInnerItems().indexOf(this.getActiveItem());
    },

    // @private
    updateItemCls: function(newItemCls, oldItemCls) {
        if (oldItemCls) {
            // @TODO: update all the child item cls's
        }
    },

    // @private
    updateDirection: function(direction) {
        // @TODO: implement logic to update the direction of the carousel
    },

    // @private
    applyIndicator: function(indicator) {
        return Ext.factory(indicator ? {direction: this.getDirection()} : null, Ext.carousel.Indicator, this.getIndicator());
    },

    // @private
    updateIndicator: function(indicator) {
        if (indicator) {
            this.add(indicator);

            var activeIndex = this.getActiveIndex();

            if (activeIndex !== -1) {
                indicator.setActiveIndex(activeIndex);
            }

            indicator.setUi(this.getUi());

            indicator.on({
                next: 'next',
                previous: 'previous',
                scope: this
            });
        }
    },

    // @private
    onDrag: function(e, t) {
        var activeIndex = this.getActiveIndex(),
            cards = this.getInnerItems(),
            ln = cards.length,
            deltaX = e.deltaX,
            deltaY = e.deltaY,
            i, card;

        this.currentScroll = {
            x: deltaX,
            y: deltaY
        };

        // Slow the drag down in the bounce

        // If this is a horizontal carousel
        if (this.getDirection() == 'horizontal') {
            if (
                // And we are on the first card and dragging left
                (activeIndex == 0 && deltaX > 0) ||
                // Or on the last card and dragging right
                (activeIndex == cards.length - 1 && deltaX < 0)
            ) {
                // Then slow the drag down
                this.currentScroll.x = deltaX / 2;
            }
        }
        // If this is a vertical carousel
        else {
            if (
                // And we are on the first card and dragging up
                (activeIndex == 0 && deltaY > 0) ||
                // Or on the last card and dragging down
                (activeIndex == cards.length - 1 && deltaY < 0)
            ) {
                // Then slow the drag down
                this.currentScroll.y = deltaY / 2;
            }
        }

        // This will update all the cards to their correct position based on the current drag
        for (i = 0; i < ln; i++) {
            if (this.isIndexInRange(i, activeIndex)) {
                this.updateCardPosition(cards[i]);
            }
        }
    },

    // @private
    onDragStart: function(e) {
        e.stopPropagation();
    },

    // @private
    onDragEnd: function(e, t) {
        var cards = this.getInnerItems(),
            activeIndex = this.getActiveIndex(),
            previousDelta, deltaOffset;

        if (this.getDirection() === 'horizontal') {
            deltaOffset = e.deltaX;
            previousDelta = e.previousDeltaX;
        } else {
            deltaOffset = e.deltaY;
            previousDelta = e.previousDeltaY;
        }

        // We have gone to the right
        if (deltaOffset < 0 && Math.abs(deltaOffset) > 3 && previousDelta <= 0 && cards[activeIndex+1]) {
            this.next();
        }
        // We have gone to the left
        else if (deltaOffset > 0 && Math.abs(deltaOffset) > 3 && previousDelta >= 0 && cards[activeIndex-1]) {
            this.previous();
        } else {
            // drag back to current active card
            this.onActiveItemChange(this, this.getActiveItem());
        }
    },

    // @private
    onActiveItemChange: function(carousel, activeItem) {
        var cards = this.getInnerItems(),
            ln = cards.length,
            activeIndex = cards.indexOf(activeItem),
            indicator = this.getIndicator(),
            i, card;

        this.currentScroll = {
            x: 0,
            y: 0
        };

        for (i = 0; i < ln; i++) {
            card = cards[i];
            if (this.isIndexInRange(i, activeIndex)) {
                this.updateCardPosition(card, i, activeIndex);
                card.show();
            } else {
                card.hide();
            }
        }

        if (indicator) {
            indicator.setActiveIndex(activeIndex);
        }
    },

    /**
     * Switches to the next card
     * @return {Ext.carousel.Carousel} this
     */
    next: function() {
        var next = this.getInnerItems()[this.getActiveIndex()+1];
        if (next) {
            this.setActiveItem(next);
        }
        return this;
    },

    /**
     * Switches to the previous card
     * @return {Ext.carousel.Carousel} this
     */
    previous: function() {
        var prev = this.getInnerItems()[this.getActiveIndex()-1];
        if (prev) {
            this.setActiveItem(prev);
        }
        return this;
    },

    // @private
    destroy: function() {
        if (this.sizeMonitor) {
            this.sizeMonitor.destroy();
        }
        this.callParent();
    }
}, function() {
    //<deprecated product=touch since=2.0>

    /**
     * Switches to the previous card
     * @member Ext.carousel.Carousel
     * @method prev
     * @deprecated
     */
    Ext.deprecateClassMethod(this, 'prev', 'previous');

    /**
     * Returns true if the Carousel indicators are arranged horizontally
     * @member Ext.carousel.Carousel
     * @method isHorizontal
     * @deprecated
     */
    Ext.deprecateClassMethod(this, 'isHorizontal', function() {
        return this.getDirection() == 'horizontal';
    }, 'isHorizontal is deprecated, please use this.getDirection()');

    /**
     * Returns true if the Carousel indicators are arranged vertically
     * @member Ext.carousel.Carousel
     * @method isVertical
     * @deprecated
     */
    Ext.deprecateClassMethod(this, 'isVertical', function() {
        return this.getDirection() == 'vertical';
    }, 'isVertical is deprecated, please use this.getDirection()');

    //</deprecated>
});