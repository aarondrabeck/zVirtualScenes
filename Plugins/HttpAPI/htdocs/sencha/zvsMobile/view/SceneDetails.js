Ext.require(['Ext.Panel', 'Ext.util.JSONP', 'Ext.MessageBox'], function () {
    Ext.define('zvsMobile.view.SceneDetails', {
        extend: 'Ext.Panel',
        xtype: 'SceneDetails',
        constructor: function (config) {
            var self = this;
            self.RepollTimer;
            Ext.apply(config || {}, {
                layout: {
                    type: 'vbox',
                    align: 'strech'
                },
                items: [{
                    xtype: 'toolbar',
                    docked: 'top',
                    title: 'Scene Details',
                    items: [{
                        xtype: 'button',
                        iconMask: true,
                        ui: 'back',
                        text: 'Back',
                        handler: function () {
                            var SceneViewPort = self.parent;
                            SceneViewPort.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                            SceneViewPort.setActiveItem(SceneViewPort.items.items[0]);
                        }
                    }]
                }, {
                    xtype: 'panel',
                    tpl: new Ext.XTemplate(
                        '<div class="scene_info">',
                            '<div class="head">',
							    '<div class="image s_img_{scene.is_running}"></div>',
							    '<tpl for="scene">',
					                '<h1>{name}</h1>',
                                    '<div class="scene_overview"><strong>Running:</strong>',
                                    '<tpl if="is_running">Yes<tpl else>No</tpl></div>',
                            '</div>',
                        '</tpl>',
                    '</div>'
                )
                }, {
                    xtype: 'button',
                    text: 'Activate',
                    ui: 'confirm',
                    margin: '25 5',
                    handler: function () {
                        var scene = self.items.items[1].getData();
                        console.log('AJAX: ActivateScene');

                        Ext.Ajax.request({
                            url: '/API/scene/' + scene.scene.id,
                            method: 'POST',
                            params: {
                                is_running: true
                            },
                            success: function (response, opts) {
                                var result = JSON.parse(response.responseText);
                                if (result.success) {
                                    self.delayedReload();
                                    Ext.Msg.alert('Scene Activation', result.desc);
                                }
                                else {
                                    Ext.Msg.alert('Scene Activation', 'Communication Error!');
                                }
                            }
                        });


                    }
                }, {
                    xtype: 'panel',
                    tpl: new Ext.XTemplate(
                     	'<tpl for="scene">',
                         '<tpl if="cmd_count &gt; 0">',
                            '<div class="scene_overview">',
                                '<table class="info">',
                                '<thead>',
                                    '<tr>',
                                        '<th></th>',
                                        '<th scope="col" abbr="Device">Device / Cmd</th>',
                                        '<th scope="col" abbr="Action">Action</th>',
                                    '</tr>',
                                '</thead>',
                                '<tbody>',
                                '<tpl for="cmds">',
                                        '<tr>',
                                            '<th scope="row">{order}</th>',
                                            '<td>{device}</td>',
                                            '<td>{action}</td>',
                                            '</tr>',
                                        '</tpl>',
                                '</tbody>',
                                '</table>',
                            '</div>',
                        '</tpl>',
                        '</tpl>'
                    )

                }]
            });
            this.callOverridden([config]);
        },
        config:
	{
	    layout: 'fit',
	    scrollable: 'vertical'
	},
        delayedReload: function () {
            var self = this;
            if (self.RepollTimer) {
                clearInterval(self.RepollTimer);
            }

            self.RepollTimer = setTimeout(function () {
                self.loadScene(self.sceneId);
                SceneStore.load();
            }, 500);
        }, loadScene: function (sceneId) {
            var self = this;
            self.sceneId = sceneId;
            //Get Device Details			
            console.log('AJAX: GetSceneDetails');
            Ext.data.JsonP.request({
                url: '/API/scene/' + sceneId,
                callbackKey: 'callback',
                params: {
                    u: Math.random()
                },
                success: function (result) {
                    //Send data to panel TPL                            
                    self.items.items[1].setData(result);
                    self.items.items[3].setData(result);
                }
            });
        }
    });
});
