Ext.define('zvsMobile.view.SceneList', {
        extend: 'Ext.Panel',
        xtype: 'SceneList',        
        constructor: function (config) {
            var self = this;
            Ext.apply(config || {}, {
                items: [{
                    xtype: 'toolbar',
                    docked: 'top',
                    title: 'Scenes'
                }, {
                    ui: 'round',
                    xtype: 'list',
                    plugins: [
                    { xclass: 'Ext.plugin.PullRefresh' }
                ],
                    itemTpl: new Ext.XTemplate(
						    '<div class="scene">',
						    '<div class="imageholder running_{is_running}"></div>',
						    '<h2>{name} ({cmd_count})</h2>',
						    '</div>'
					    ),
                    cls: 'SceneListItem',
                    store: SceneStore,
                    listeners: {
                        scope: this,
                        selectionchange: function (list, records) {
                            if (records[0] !== undefined) {
                                var SceneViewPort = self.parent;
                                var SceneDetails = SceneViewPort.items.items[1];
                                var sceneId = records[0].data.id;
                                SceneDetails.loadScene(sceneId);
                                SceneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'left' });
                                SceneViewPort.setActiveItem(SceneDetails);
                            }
                        }
                    }
                }],
                listeners: {
                    scope: this,
                    activate: function () {
                        self.items.items[1].deselectAll();
                    }
                }
            });
            this.callOverridden([config]);
        },
        config:
	{
	    layout: 'fit'
	}
    });