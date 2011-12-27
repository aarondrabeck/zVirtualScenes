/**
 * Panel is a container that has specific functionality and structural components that make it the perfect building
 * block for application-oriented user interfaces.
 *
 * Panels are, by virtue of their inheritance from {@link Ext.Container}, capable of being configured with a {@link
 * Ext.Container#layout layout}, and containing child Components.
 *
 * When either specifying child {@link Ext.Container#items items} of a Panel, or dynamically {@link Ext.Container#add
 * adding} Components to a Panel, remember to consider how you wish the Panel to arrange those child elements, and
 * whether those child elements need to be sized using one of Ext's built-in `**{@link Ext.Container#layout layout}**`
 * schemes.
 *
 * # Example:
 *
 *     @example preview
 *     var panel = Ext.create('Ext.Panel', {
 *         fullscreen: true,
 *
 *         items: [
 *             {
 *                 dock : 'top',
 *                 xtype: 'toolbar',
 *                 title: 'Standard Titlebar'
 *             },
 *             {
 *                 dock : 'top',
 *                 xtype: 'toolbar',
 *                 ui   : 'light',
 *                 items: [
 *                     {
 *                         text: 'Test Button'
 *                     }
 *                 ]
 *             }
 *         ],
 *
 *         html: 'Testing'
 *     });
 *
 */
Ext.define('Ext.Panel', {
    isPanel: true,
    extend : 'Ext.Container',
    xtype  : 'panel',
    alternateClassName: 'Ext.lib.Panel',
    config: {
        baseCls: Ext.baseCSSPrefix + 'panel'
    }
});
