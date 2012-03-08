Ext.define('zvsMobile.view.SettingsLogOut', {
        extend: 'Ext.Panel',
        xtype: 'LogOut',

        constructor: function (config) {
            var self = this;
            Ext.apply(config || {}, {
			    UpdateLogoutHTML: function() 
				{
				console.log(self.items.items[0]);
					self.items.items[0].updateHtml('<div class="logout_info"><p> Logged in to: ' + zvsMobile.app.BaseURL() + '</p></div>')
				},
                items: [{
                        xtype: 'panel',
                        html: ''
                    },{
                    xtype: 'fieldset',
                    style: 'padding:10px;',
                    items: [ {
                        xtype: 'button',
                        text: 'Logout',
                        width: '90%',
                        style: 'margin:10px auto;',
                        handler: function (b) {
                            Ext.Ajax.request({
                                url: zvsMobile.app.BaseURL() + '/logout',
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
            xtype: 'panel',
            layout: 'vbox',
            scrollable: 'vertical'
        }
    });