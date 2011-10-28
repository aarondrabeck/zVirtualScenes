/**
 * Wraps a Google Map in an Ext.Component.
 *
 * http://code.google.com/apis/maps/documentation/v3/introduction.html
 *
 * To use this component you must include an additional JavaScript file from Google:
 *
 *     <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=true">&lt/script>
 *
 * {@img ../guildes/map/screenshot.png}
 *
 *     var panel = new Ext.Panel({
 *         fullscreen: true,
 *         items: [{
 *             xtype: 'map',
 *             useCurrentLocation: true
 *         }]
 *     });
 *
 */
Ext.define('Ext.Map', {
    extend: 'Ext.Component',
    xtype : 'map',
    require: ['Ext.util.GeoLocation'],

    config: {
        /**
         * @event maprender
         * @param {Ext.Map} this
         * @param {google.maps.Map} map The rendered google.map.Map instance
         */

        /**
         * @event centerchange
         * @param {Ext.Map} this
         * @param {google.maps.Map} map The rendered google.map.Map instance
         * @param {google.maps.LatLng} center The current LatLng center of the map
         */

        /**
         * @event typechange
         * @param {Ext.Map} this
         * @param {google.maps.Map} map The rendered google.map.Map instance
         * @param {Number} mapType The current display type of the map
         */

        /**
         * @event zoomchange
         * @param {Ext.Map} this
         * @param {google.maps.Map} map The rendered google.map.Map instance
         * @param {Number} zoomLevel The current zoom level of the map
         */

        /**
         * @cfg {String} baseCls
         * The base CSS class to apply to the Maps's element
         * @accessor
         */
        baseCls: Ext.baseCSSPrefix + 'map',

        /**
         * @cfg {Boolean} useCurrentLocation
         * Pass in true to center the map based on the geolocation coordinates.
         * @accessor
         */
        useCurrentLocation: false,

        /**
         * @cfg {google.maps.Map} map
         * The wrapped map.
         * @accessor
         */
        map: null,

        /**
         * @cfg {Ext.util.GeoLocation} geo
         * @accessor
         */
        geo: null,

        /**
         * @cfg {Boolean} maskMap
         * Masks the map (Defaults to false)
         * @accessor
         */
        maskMap: false,

        /**
         * @cfg {String} maskMapCls
         * CSS class to add to the map when maskMap is set to true.
         * @accessor
         */
        maskMapCls: Ext.baseCSSPrefix + 'mask-map',

        /**
         * @cfg {Object} mapOptions
         * MapOptions as specified by the Google Documentation:
         * http://code.google.com/apis/maps/documentation/v3/reference.html
         * @accessor
         */
        mapOptions: {}
    },

    constructor: function() {
        this.callParent(arguments);
        this.element.setVisibilityMode(Ext.Element.OFFSETS);

        if (!(window.google || {}).maps) {
            this.setHtml('Google Maps API is required');
        }
        else if (this.useCurrentLocation) {
            this.geo = this.geo || Ext.create('Ext.util.GeoLocation', {autoLoad: false});
            this.geo.on({
                locationupdate : this.onGeoUpdate,
                locationerror : this.onGeoError,
                scope : this
            });
        }

        if (this.geo) {
            this.on({
                painted: this.onUpdate,
                scope: this,
                single: true
            });
            this.geo.updateLocation();
        }

        if (this.getMaskMap()) {
            this.element.mask(null, this.getMaskMapCls());
        }
        this.renderMap();
    },

    // @private
    renderMap: function() {
        var me = this,
            gm = (window.google || {}).maps,
            renderElement = me.renderElement,
            mapOptions = me.getMapOptions(),
            event;

        if (gm) {
            if (Ext.is.iPad) {
                Ext.merge(mapOptions, {
                    navigationControlOptions: {
                        style: gm.NavigationControlStyle.ZOOM_PAN
                    }
                });
            }

            Ext.merge(mapOptions, {
                center: new gm.LatLng(37.381592, -122.135672), // Palo Alto
                zoom: 12,
                mapTypeId: gm.MapTypeId.ROADMAP
            });

            if (me.maskMap && !me.mask) {
                renderElement.mask(null, this.maskMapCls);
                me.mask = true;
            }

            if (renderElement && renderElement.dom && renderElement.dom.firstChild) {
                Ext.fly(renderElement.dom.firstChild).remove();
            }

            if (me.map) {
                gm.event.clearInstanceListeners(me.map);
            }

            me.map = new gm.Map(renderElement.dom, mapOptions);

            event = gm.event;
            //Track zoomLevel and mapType changes
            event.addListener(me.map, 'zoom_changed', Ext.bind(me.onZoomChange, me));
            event.addListener(me.map, 'maptypeid_changed', Ext.bind(me.onTypeChange, me));
            event.addListener(me.map, 'center_changed', Ext.bind(me.onCenterChange, me));

            me.fireEvent('maprender', me, me.map);
        }

    },

    // @private
    onGeoUpdate: function(coords) {
        var center;
        if (coords) {
            center = this.getMapOptions().center = new google.maps.LatLng(coords.latitude, coords.longitude);
        }

        this.update(center);
    },

    // @private
    onGeoError: Ext.emptyFn,

    // @private
    onUpdate: function(map, e, options) {
        this.update((options || {}).data);
    },


    /**
     * Moves the map center to the designated coordinates hash of the form:
     *
     *     { latitude: 37.381592, longitude: -122.135672 }
     *
     * or a google.maps.LatLng object representing to the target location.
     *
     * @param {Object/google.maps.LatLng} coordinates Object representing the desired Latitude and
     * longitude upon which to center the map.
     */
    update: function(coordinates) {
        var me = this,
            gm = (window.google || {}).maps;

        if (gm) {
            coordinates = coordinates || me.getCoords() || new gm.LatLng(37.381592, -122.135672);

            if (coordinates && !(coordinates instanceof gm.LatLng) && 'longitude' in coordinates) {
                coordinates = new gm.LatLng(coordinates.latitude, coordinates.longitude);
            }

            if (!me.getHidden()) {
                if (!me.map) {
                    me.renderMap();
                }
                if (me.map && coordinates instanceof gm.LatLng) {
                    me.map.panTo(coordinates);
                }
            }
            else {
                me.on('painted', me.onUpdate, me, {single: true, data: coordinates});
            }
        }
    },

    // @private
    onZoomChange : function() {
        var mapOptions = this.getMapOptions(),
            map = this.getMap();
        mapOptions.zoom = (map && map.getZoom) ? map.getZoom() : mapOptions.zoom || 10;

        this.fireEvent('zoomchange', this, map, mapOptions.zoom);
    },

    // @private
    onTypeChange : function() {
        var mapOptions = this.getMapOptions(),
            map = this.getMap();
        mapOptions.mapTypeId = (map && map.getMapTypeId) ? map.getMapTypeId() : mapOptions.mapTypeId;

        this.fireEvent('typechange', this, map, mapOptions.mapTypeId);
    },

    // @private
    onCenterChange: function() {
        var mapOptions = this.getMapOptions(),
            map = this.getMap();
        mapOptions.center = (map && map.getCenter) ? map.getCenter() : mapOptions.center;

        this.fireEvent('centerchange', this, map, mapOptions.center);

    },

    // @private
    onDestroy: function() {
        Ext.destroy(this.geo);
        if (this.maskMap && this.mask) {
            this.el.unmask();
        }
        if (this.map && (window.google || {}).maps) {
            google.maps.event.clearInstanceListeners(this.map);
        }
        this.callParent();
    }
}, function() {
    /**
     * @deprecated 2.0.0
     * Returns the state of the Map. This has been deprecated. Please use {@link #getMapOptions} instead/
     * @return {Object} mapOptions
     */
    Ext.deprecateClassMethod(this, 'getState', 'getMapOptions');
});
