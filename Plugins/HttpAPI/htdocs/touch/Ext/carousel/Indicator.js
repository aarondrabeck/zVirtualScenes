/**
 * A private utility class used by Ext.Carousel to create indicators.
 * @private
 */
Ext.define('Ext.carousel.Indicator', {
    extend: 'Ext.Component',
    xtype : 'carouselindicator',
    alternateClassName: 'Ext.Carousel.Indicator',

    config: {
        // @inherit
        baseCls: Ext.baseCSSPrefix + 'carousel-indicator',
        direction: 'horizontal'
    },

    initialize: function() {
        var me = this;
        me.callParent(arguments);

        me.indicators = [];

        me.element.on({
            tap  : 'onTap',
            scope: this
        });
    },

    updateDirection: function(newDirection, oldDirection) {
        var baseCls = this.getBaseCls();

        this.element.replaceCls(baseCls + '-' + oldDirection, baseCls + '-' + newDirection);

        if (newDirection === 'horizontal') {
            this.setBottom(0);
            this.setRight(null);
        } else {
            this.setRight(0);
            this.setBottom(null);
        }
    },

    addIndicator: function() {
        this.indicators.push(this.element.createChild({
            tag: 'span'
        }));
    },

    removeIndicator: function() {
        if (this.indicators.length) {
            this.indicators.pop().remove();
        }
    },

    setActiveIndex: function(index) {
        var indicators = this.indicators;

        if (indicators.length && indicators[index]) {
            indicators[index].radioCls(this.getBaseCls() + '-active');
        }
    },

    // @private
    onTap: function(e, t) {
        var box = this.element.getPageBox(),
            centerX = box.left + (box.width / 2),
            centerY = box.top + (box.height / 2),
            direction = this.getDirection();

        if ((direction === 'horizontal' && e.pageX > centerX) || (direction === 'vertical' && e.pageY > centerY)) {
            this.fireEvent('next');
        } else {
            this.fireEvent('previous');
        }
    }
});