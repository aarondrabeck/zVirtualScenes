Ext.define('zvsMobile.view.SettingsLogIn', {
    extend: 'Ext.Panel',
    xtype: 'LogIn',

    constructor: function (config) {
        var self = this;
        Ext.apply(config || {}, {
            items: [
                    {
                        xtype: 'fieldset',
                        style: 'padding:10px;',
                        items: [{
                            xtype: 'textfield',
                            name: 'apiURL',
                            value: zvsMobile.app.APIURL,
                            label: 'HTTP API URL'
                        }, {
                            xtype: 'passwordfield',
                            id: 'loginPanel_password',
                            name: 'password',
                            label: 'Password',
                            listeners: { keyup: function (t, e) {
                                if (e.browserEvent.keyCode == 13) {
                                    var submitButton = self.items.items[0].items.items[1];
                                    submitButton._handler();
                                }
                            }
                            }
                        }, {
                            xtype: 'button',
                            text: 'Login',
                            width: '90%',
                            style: 'margin:10px auto;',
                            handler: function (b) {
                                var password = Ext.getCmp('loginPanel_password');
                                if (password.getValue() == '') {
                                    password.focus();
                                    return false;
                                }

                                Ext.Ajax.request({
                                    url: zvsMobile.app.APIURL + '/login',
                                    method: 'POST',
                                    params: {
                                        u: Math.random(),
                                        password: password.getValue()
                                    },
                                    success: function (response, opts) {
                                        var result = JSON.parse(response.responseText);
                                        if (result.success) {
                                            password.blur();
                                            password.setValue('');
                                            self.fireEvent('loggedIn');
                                        }
                                        else {
                                            Ext.Msg.alert('Invalid Credentials.', 'Please try again.');
                                        }
                                    },
                                    failure: function (result, request) {
                                        Ext.Msg.alert('Communication Error.', 'Please try again.');
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