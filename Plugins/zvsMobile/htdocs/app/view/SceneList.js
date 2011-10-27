Ext.define('Scene', {
    extend: 'Ext.data.Model',
    fields: [
			 { name: 'id', type: 'int' },
			 { name: 'name', type: 'string' },
			 { name: 'is_running', type: 'bool' },
             { name: 'cmd_count', type: 'string' }
		 ]
});

var SceneStore = Ext.create('Ext.data.Store', {
    model: 'Scene',
    proxy: {
        type: 'scripttag',
        url: 'http://10.1.0.56:9999/JSON/GetSceneList',
        extraParams: {
            u: Math.random()
        },
        reader: {
            type: 'json',
            root: 'scenes',
            idProperty: 'id',
            successProperty: 'success'
        },

        callbackParam: 'callback'
    },
    autoLoad: true
});


Ext.define('zvsMobile.view.SceneList', {
    extend: 'Ext.Panel',
    alias: 'widget.SceneList',

    constructor: function (config) {
        var self = this;
        Ext.apply(config || {}, {
            items: [
            {
                xtype: 'toolbar',
                docked: 'top',
                title: 'Scenes',
                items: [{
                    xtype: 'button',
                    iconMask: true,
                    iconCls: 'refresh',
                    handler: function () {
                        SceneStore.load();
                    }
                }]
            },
             {
                 xtype: 'list',
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
             }] , 
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



					
