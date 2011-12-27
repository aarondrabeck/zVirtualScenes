/**
 * Navigation  are most commonly used as dockedItems within an Ext.Panel.
 * They can be docked at the 'top' or 'bottom' of a Panel by specifying
 * the dock config.
 *
 * The {@link #defaultType} of Toolbar's is '{@link Ext.Button button}'.
 *
 * # Example:
 *
 *     var myNavBar = new Ext.NavigationBar({
 *         dock : 'top',
 *         title: 'Navigation',
 *         items: [
 *             {
 *                 text: 'My Button'
 *             }
 *         ]
 *     });
 *
 *     var myPanel = new Ext.Panel({
 *         dockedItems: [myNavBar],
 *         fullscreen : true,
 *         html       : 'Test Panel'
 *     });
 *
 */
Ext.define('Ext.NavigationBar', {
    extend: 'Ext.Container',

    xtype: 'navigationbar',

    requires: [
        'Ext.Button',
        'Ext.Title',
        'Ext.Spacer',
        'Ext.util.SizeMonitor'
    ],

    // private
    isToolbar: true,

    config: {
        // @inherit
        baseCls: Ext.baseCSSPrefix + 'toolbar',

        /**
         * @cfg {String} ui
         * Style options for Toolbar. Either 'light' or 'dark'.
         * @accessor
         */
        ui: 'dark',

        /**
         * @cfg {String} title
         * The title of the toolbar.
         * @accessor
         */
        title: null,

        /**
         * @cfg {String} defaultType
         * The default xtype to create.
         * @accessor
         */
        defaultType: 'button',

        layout: {
            type: 'hbox'
        },

        items: []
    },

    constructor: function() {
        this.refreshTitlePosition = Ext.Function.createThrottled(this.refreshTitlePosition, 50, this);

        this.callParent(arguments);
    },

    initialize: function() {
        this.on({
            painted: 'onPainted',
            erased: 'onErased'
        });
    },

    applyItems: function(items) {
        if (!this.initialized) {
            var SizeMonitor = Ext.util.SizeMonitor,
                leftBox, rightBox, spacer;

            this.leftBox = leftBox = this.add({
                xtype: 'container',
                style: 'position: relative',
                layout: {
                    type: 'hbox',
                    align: 'center'
                }
            });
            this.spacer = spacer = this.add({
                xtype: 'component',
                style: 'position: relative',
                flex: 1
            });
            this.rightBox = rightBox = this.add({
                xtype: 'container',
                style: 'position: relative',
                layout: {
                    type: 'hbox',
                    align: 'center'
                }
            });
            this.titleComponent = this.add({
                xtype: 'title',
                centered: true
            });

            this.sizeMonitors = {
                leftBox: new SizeMonitor({
                    element: leftBox.renderElement,
                    callback: this.refreshTitlePosition,
                    scope: this
                }),
                spacer: new SizeMonitor({
                    element: spacer.renderElement,
                    callback: this.refreshTitlePosition,
                    scope: this
                }),
                rightBox: new SizeMonitor({
                    element: rightBox.renderElement,
                    callback: this.refreshTitlePosition,
                    scope: this
                })
            };

            this.doAdd = this.doBoxAdd;
            this.doInsert = this.doBoxInsert;
        }

        this.callParent(arguments);
    },

    doBoxAdd: function(item) {
        if (item.config.align == 'right') {
            this.rightBox.add(item);
        }
        else {
            this.leftBox.add(item);
        }

        if (this.painted) {
            this.refreshTitlePosition();
        }
    },

    //TODO doBoxRemove
    doBoxInsert: function(index, item) {
        if (item.config.align == 'right') {
            this.rightBox.add(item);
        }
        else {
            this.leftBox.add(item);
        }
    },

    onPainted: function() {
        var sizeMonitors = this.sizeMonitors;

        this.painted = true;
        this.refreshTitlePosition();

        sizeMonitors.leftBox.refresh();
        sizeMonitors.spacer.refresh();
        sizeMonitors.rightBox.refresh();
    },

    onErased: function() {
        this.painted = false;
    },

    refreshTitlePosition: function() {
        var titleElement = this.titleComponent.renderElement;

//        if (!this.titleBox) {
            titleElement.setWidth(null);
            titleElement.setLeft(null);
//            this.titleBox = titleBox = titleElement.getPageBox();
//        }

        var spacerBox = this.spacer.renderElement.getPageBox(),
            titleBox = titleElement.getPageBox(),
            widthDiff = titleBox.width - spacerBox.width,
            titleLeft = titleBox.left,
            titleRight = titleBox.right,
            halfWidthDiff, leftDiff, rightDiff;

        if (widthDiff > 0) {
            titleElement.setWidth(spacerBox.width);
            halfWidthDiff = widthDiff / 2;
            titleLeft += halfWidthDiff;
            titleRight -= halfWidthDiff;
        }

        leftDiff = spacerBox.left - titleLeft;
        rightDiff = titleRight - spacerBox.right;

        if (leftDiff > 0) {
            titleElement.setLeft(leftDiff);
        }
        else if (rightDiff > 0) {
            titleElement.setLeft(-rightDiff);
        }

        titleElement.repaint();
    },

    // @private
    updateTitle: function(newTitle) {
        this.titleComponent.setTitle(newTitle);

        this.titleBox = null;

        if (this.painted) {
            this.refreshTitlePosition();
        }
    },

    destroy: function() {
        this.callParent();

        var sizeMonitors = this.sizeMonitors;

        sizeMonitors.leftBox.destroy();
        sizeMonitors.spacer.destroy();
        sizeMonitors.rightBox.destroy();
    }
});
