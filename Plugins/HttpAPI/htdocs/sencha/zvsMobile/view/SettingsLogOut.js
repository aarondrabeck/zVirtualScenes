Ext.require(['Ext.Panel', 'Ext.MessageBox'], function () {

    Ext.define('zvsMobile.view.SettingsLogOut', {
        extend: 'Ext.Panel',
        xtype: 'LogOut',

        constructor: function (config) {
            var self = this;
            Ext.apply(config || {}, {
                items: [{
                    xtype: 'fieldset',
                    style: 'padding:10px;',
                    items: [{
                        xtype: 'textfield',
                        tpl: new Ext.XTemplate(
                '<div class="x-form-label">',
                    '<span>Currently logged in.</span>',
                '</div>',
                '<div class="x-form-field-container" style="padding:0.6em;">{name}</div>')
                    }, {
                        xtype: 'button',
                        text: 'Logout',
                        width: '90%',
                        style: 'margin:10px auto;',
                        handler: function (b) {
                            Ext.Ajax.request({
                                url: '/API/logout',
                                method: 'POST',
                                params: {
                                    u: Math.random()
                                },
                                success: function (response, opts) {
                                    var result = JSON.parse(response.responseText);
                                    if (result.success) {
                                        self.fireEvent('loggedOut');
                                    }
                                    else {
                                        Ext.Msg.alert('Logout failed.', 'Please try again.');
                                    }
                                },
                                failure: function (result, request) {
                                    Ext.Msg.alert('Logout failed.', 'Please try again.');
                                }
                            });
                        }
                    }]
                }]
            });
            this.callParent([config]);
        },
        config:
        {
            scrollable: 'vertical',
            layout: 'fit'
        }
    });

});