Ext.define('zvsMobile.view.SceneDetails', {
    extend: 'Ext.Panel',
    alias: 'widget.SceneDetails',
    constructor: function (config) {
        var self = this;
        Ext.apply(config || {}, {
                 loadScene: function (sceneId) {
                    self.sceneId = sceneId;
                    //Get Device Details			
                    console.log('AJAX: GetSceneDetails');
                    Ext.util.JSONP.request({
                        url: 'http://10.1.0.56:9999/JSON/GetSceneDetails',
                        callbackKey: 'callback',
                        params: {
                            u: Math.random(),
                            id: sceneId
                        },
                        callback: function (data) {
                            //Send data to panel TPL                            
                            self.items.items[1].setData(data);
                            console.log(data);
                        }
                    });
                },
	            items: [{
                            xtype: 'toolbar',
                            docked: 'top',
                            title: 'Scene Details',
                            items: [{
                                        xtype: 'button',
                                        iconMask: true,
                                        iconCls: 'refresh',
                                        handler: function () {
                                            Ext.msg.alert('test');
                                        }
                                 }]
                            },
                            {
                                        xtype: 'panel',
                                        tpl: new Ext.XTemplate(
							            '<tpl for="scene">', 					    
					                        '<h2>{name} ({cmd_count})</h2>',
                                            'Commands: <ol>',
                                            '<tpl for="cmds">',  
                                                '<li>{device} | {action}</li>',
                                             '</tpl></ol>', 
                                        '</tpl>')
                             }]
        });
        this.callOverridden([config]);
    },
    config:
	{
	    layout: 'fit',
	   
	}
});
