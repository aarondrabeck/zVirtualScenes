Ext.define('Scene', {
 extend: 'Ext.data.Model',
 fields: [
			 {name: 'id', type: 'int'},
			 {name: 'name',  type: 'string'},
			 {name: 'is_running',       type: 'bool'}
		 ] 
 });

 var SceneStore = Ext.create('Ext.data.Store', {
	 model: 'Scene',
	 proxy: {
			type: 'scripttag',
			url : 'http://10.1.0.56:9999/JSON/GetSceneList',
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
     initialize: function () {
         var self = this;
         this.add({
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
         });
         this.callParent();
     },
     config:
	{
	    layout: 'fit',
	    items: [{
	        xtype: 'list',
	        itemTpl: new Ext.XTemplate(
						'<div class="scene">',
						'<div class="imageholder running_{is_running}"></div>',
						'<h2>{name}</h2>',
						'</div>'
					),
	        cls: 'SceneListItem',
	        store: SceneStore,
	        listeners: {
	            scope: this,
	            selectionchange: function (list, records) {
	                if (records[0] !== undefined) {
	                    var sceneId = records[0].data.id;
	                    Ext.Msg.confirm('Acitvate Scene', 'Are you sure you want to activate &quot;' + records[0].data.name + '&quot;?',
								function (choice) {
								    if (choice === 'yes') {
								        console.log('AJAX: ActivateScene');
								        Ext.util.JSONP.request({
								            url: 'http://10.1.0.56:9999/JSON/ActivateScene',
								            callbackKey: 'callback',
								            params: {
								                u: Math.random(),
								                id: sceneId
								            },
								            callback: function (data) {
								                if (data.success) {
								                    Ext.Msg.alert('Scene Activation', data.desc);
								                }
								                else {
								                    Ext.Msg.alert('Scene Activation', 'Communication Error!');
								                }
								            }
								        });
								    }
								    list.deselectAll();
								}
								);
	                }
	            }
	        }
	    }]
	}
 });


			
					
