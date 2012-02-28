Ext.define('zvsMobile.view.tablet.SceneTabletViewPort', {
    extend: 'Ext.Panel',
    xtype: 'SceneTabletViewPort',
    requires: ['zvsMobile.view.SceneList', 'zvsMobile.view.SceneDetails'],
    config: {
        layout: 'hbox',
        items: [{
            xtype: 'SceneList',
            id: 'SceneList',
            flex: 1,
            listeners: {
                scope: this,
                selectionchange: function (list, records) {
                    if (records[0] !== undefined) {
                        var SceneDetails = Ext.getCmp('SceneDetails');                   
                        SceneDetails.loadScene(records[0].data.id);

                        var sceneDetailsPane = Ext.getCmp('sceneDetailsPane');
                        sceneDetailsPane.getLayout().setAnimation({ type: 'slide', direction: 'up' });
                        sceneDetailsPane.setActiveItem(SceneDetails);
                    }
                }
            }
        }, {
            flex: 2,
            id: 'sceneDetailsPane',
            layout: {
                type: 'card',
                animation: {
                    type: 'slide',
                    direction: 'left'
                }
            },
            items: [{
                cls: 'emptyDetail',
                html: "Select a scene to see more details."
            }, {
                xtype: 'SceneDetails',
                id: 'SceneDetails'
            }, {
                xtype: 'toolbar',
                docked: 'top',
                title: 'Scene Details'

            }]
        }
		]
    }
});