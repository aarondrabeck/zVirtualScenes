Ext.define('zvsMobile.view.SettingsLogIn', {
    extend: 'Ext.Panel',
    requires: ['Ext.field.Password', 'Ext.form.FieldSet'],
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
                            id: 'APIURL_textfield',
                            name: 'apiURL',
                            value: zvsMobile.app.BaseURL(),
                            label: 'HTTP API URL'
                        }, {
                            xtype: 'passwordfield',
                            id: 'loginPanel_password',
                            name: 'password',
                            label: 'Password',
                            listeners: {
                                keyup: function (t, e) {
                                    if (e.browserEvent.keyCode == 13) {
                                        var submitButton = Ext.getCmp('submitButton');
                                        submitButton._handler.call(submitButton.scope, submitButton, Ext.EventObject());
                                    }
                                }
                            }
                        }, {
                            xtype: 'button',
                            id: 'saveButton',
                            text: 'Save',
                            width: '90%',
                            style: 'margin:10px auto;',
                            handler: function (b) {
                                var password = Ext.getCmp('loginPanel_password');
                                var passwordtxt = password.getValue();
                                var APIURL_textfield = Ext.getCmp('APIURL_textfield');
                                var enteredURL = APIURL_textfield.getValue();

                                //Save to store/localstorage
                                appSettingsStore = Ext.getStore('appSettingsStore');
                                var BaseURLRecord = appSettingsStore.findRecord('SettingName', 'BaseURL');
                                if (BaseURLRecord != null) {
                                    if (enteredURL == '') {
                                        appSettingsStore.remove(BaseURLRecord);
                                        appSettingsStore.sync();
                                    }
                                    else {
                                        BaseURLRecord.set('Value', enteredURL);
                                        appSettingsStore.sync();
                                    }
                                }
                                else {
                                    if (enteredURL != '') {
                                        appSettingsStore.add({ SettingName: 'BaseURL', Value: enteredURL });
                                        appSettingsStore.sync();
                                    }
                                }

                                var Password = appSettingsStore.findRecord('SettingName', 'Password');
                                if (Password != null) {
                                    if (passwordtxt == '') {
                                        appSettingsStore.remove(Password);
                                        appSettingsStore.sync();
                                    }
                                    else {
                                        Password.set('Value', passwordtxt);
                                        appSettingsStore.sync();
                                    }
                                }
                                else {
                                    if (passwordtxt != '') {
                                        appSettingsStore.add({ SettingName: 'Password', Value: passwordtxt });
                                        appSettingsStore.sync();
                                    }
                                }
                            }
                        }, {
                            xtype: 'button',
                            id: 'submitButton',
                            text: 'Login',
                            width: '90%',
                            style: 'margin:10px auto;',
                            handler: function (b) {

                                appSettingsStore = Ext.getStore('appSettingsStore');

                                var BaseURLRecord = appSettingsStore.findRecord('SettingName', 'BaseURL');
                                if (BaseURLRecord === null) {
                                    Ext.Msg.alert('A URL must be saved.', 'Please try again.');
                                    return;
                                }

                                var Password = appSettingsStore.findRecord('SettingName', 'Password');
                                if (Password === null) {
                                    Ext.Msg.alert('A password must be saved.', 'Please try again.');
                                    return;
                                }

                                Ext.Ajax.request({
                                    url: zvsMobile.app.BaseURL() + '/login',
                                    method: 'POST',
                                    params: {
                                        u: Math.random(),
                                        password: Password.get('Value')
                                    },
                                    success: function (response, opts) {
                                        var result = JSON.parse(response.responseText);
                                        if (result.success) {

                                            var token = result.zvstoken;
                                            appSettingsStore = Ext.getStore('appSettingsStore');
                                            var tokenRecord = appSettingsStore.findRecord('SettingName', 'zvstoken');
                                            if (tokenRecord != null) {
                                                tokenRecord.set('Value', token);
                                                appSettingsStore.sync();
                                            }
                                            else {
                                                appSettingsStore.add({ SettingName: 'zvstoken', Value: token });
                                                appSettingsStore.sync();
                                            }

                                            //Sets the tokens in the stores
                                            zvsMobile.app.SetStoreProxys();

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