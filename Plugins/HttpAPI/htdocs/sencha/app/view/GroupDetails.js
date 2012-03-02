Ext.define('zvsMobile.view.GroupDetails', {
    extend: 'Ext.Panel',
    xtype: 'GroupDetails',
    constructor: function (config) {
        var self = this;
        var groupId = 0;
        var RepollTimer;

        Ext.apply(config || {}, {
            items: [{
                xtype: 'panel',
                id: 'groupStatusTPL',
                tpl: new Ext.XTemplate(
                        '<div class="group_info">',
                            '<div class="head">',
							    '<div class="image"></div>',
							    '<tpl for="group">',
					                '<h1>{name}</h1>',
                                    '</tpl>',
                             '</div>',
                        '</div>')
            }, {
                xtype: 'button',
                text: 'Turn On',
                ui: 'confirm',
                margin: '25 5 5 5',
                handler: function () {
                    console.log('AJAX: ActivateGroup' + self.groupId);
                    Ext.Ajax.request({
                        url: zvsMobile.app.BaseURL() + '/commands/',
                        method: 'POST',
                        params: {
                            u: Math.random(),
                            name: 'GROUP_ON',
                            arg: self.groupId
                        },
                        success: function (response, opts) {
                            var result = JSON.parse(response.responseText);
                            if (result.success) {
                                Ext.getCmp('GroupActiveResult').setHtml('All device in group Turned On.');
                            }
                            else {
                                Ext.Msg.alert('Group', 'Communication Error!');
                            }
                        }
                    });
                }
            }, {
                xtype: 'button',
                text: 'Turn Off',
                ui: 'confirm',
                margin: '5 5 25 5',
                handler: function () {
                    console.log('AJAX: DeactivateGroup' + self.groupId);
                    Ext.Ajax.request({
                        url: zvsMobile.app.BaseURL() + '/commands/',
                        method: 'POST',
                        params: {
                            u: Math.random(),
                            name: 'GROUP_OFF',
                            arg: self.groupId
                        },
                        success: function (response, opts) {
                            var result = JSON.parse(response.responseText);
                            if (result.success) {
                                Ext.getCmp('GroupActiveResult').setHtml('All device in group Turned Off.');
                            }
                            else {
                                Ext.Msg.alert('Group', 'Communication Error!');
                            }
                        }
                    });

                }
            }, {
                xtype: 'panel',
                id: 'GroupActiveResult',
                cls: 'result',
                html: ''
            }, {
                xtype: 'panel',
                id: 'groupDetailsTPL',
                tpl: new Ext.XTemplate(
                     	'<tpl for="group">',
                            '<div class="group_overview">',
                                '<table class="info">',
                                '<thead>',
                                    '<tr>',
                                        '<th scope="col" abbr="Device">Device</th>',
                                        '<th scope="col" abbr="Action">Type</th>',
                                    '</tr>',
                                '</thead>',
                                '<tbody>',
                                '<tpl for="devices">',
                                        '<tr>',
                                            '<td>{name}</td>',
                                            '<td>{type}</td>',
                                            '</tr>',
                                        '</tpl>',
                                '</tbody>',
                                '</table>',
                            '</div>',
                        '</tpl>'
                    )

            }]
        });
        this.callOverridden([config]);
    },
    config: {
        layout: 'vbox',
        scrollable: 'vertical'
    },    
    loadGroup: function (groupID) {
        var self = this;
        self.groupId = groupID;
        Ext.getCmp('GroupActiveResult').setHtml('');
        //Get Device Details			
        console.log('AJAX: GetgroupDetails');

        Ext.data.JsonP.request({
            url: zvsMobile.app.BaseURL() + '/group/' + self.groupId,
            callbackKey: 'callback',
            params: {
                u: Math.random()
            },
            success: function (result) {
                //Send data to panel TPL              
                Ext.getCmp('groupDetailsTPL').setData(result);
                Ext.getCmp('groupStatusTPL').setData(result);
            }
        });
    }
});