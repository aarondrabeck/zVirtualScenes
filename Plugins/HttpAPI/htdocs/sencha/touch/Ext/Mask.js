Ext.define('Ext.Mask', {
    extend: 'Ext.Button',
    xtype: 'mask',

    config: {
        baseCls: 'x-mask',
        pressedCls: 'x-mask-pressed',
        hidden: true,
        top: 0,
        left: 0,
        right: 0,
        bottom: 0
    }
});