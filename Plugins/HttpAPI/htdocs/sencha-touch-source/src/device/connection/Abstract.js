/**
 * @private
 */
Ext.define('Ext.device.connection.Abstract', {
    extend: 'Ext.Evented',

    config: {
        online: false,
        type: null
    },

    /**
     * @property {String} UNKNOWN
     */
    UNKNOWN: 'Unknown connection',

    /**
     * @property {String} ETHERNET
     */
    ETHERNET: 'Ethernet connection',

    /**
     * @property {String} WIFI
     */
    WIFI: 'WiFi connection',

    /**
     * @property {String} CELL_2G
     */
    CELL_2G: 'Cell 2G connection',

    /**
     * @property {String} CELL_3G
     */
    CELL_3G: 'Cell 3G connection',

    /**
     * @property {String} CELL_4G
     */
    CELL_4G: 'Cell 4G connection',

    /**
     * @property {String} NONE
     */
    NONE: 'No network connection',

    /**
     * True if the device is currently online
     * @return {Boolean} online
     */
    isOnline: function() {
        return this.getOnline();
    }

    /**
     * @method getType
     * Returns the current connection type.
     * @return {String} type
     */
});
