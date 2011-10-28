/**
 * This class acts as a factory for environment-specific viewport implementations.
 * Default implementation goes to {@link Ext.viewport.Default}
 */
Ext.define('Ext.viewport.Viewport', {
    requires: [
        'Ext.viewport.Ios',
        'Ext.viewport.Android'
    ],

    constructor: function(config) {
        var osName = Ext.os.name,
            viewportName, viewport;

        switch (osName) {
            case 'Android':
                viewportName = 'Android';
                break;

            case 'iOS':
                viewportName = 'Ios';
                break;

            default:
                viewportName = 'Default';
        }

        viewport = Ext.create('Ext.viewport.' + viewportName, config);

        return viewport;
    }
});
