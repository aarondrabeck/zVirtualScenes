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
                            value: (zvsMobile.app.BaseURL() === '' && window.location.origin != undefined ? window.location.origin + "/API/" : zvsMobile.app.BaseURL()),
                            label: 'HTTP API URL'
                        }, {
                            xtype: 'passwordfield',
                            id: 'loginPanel_password',
                            name: 'password',
                            label: 'Password',
                            listeners: { keyup: function (t, e) {
                                if (e.browserEvent.keyCode == 13) {
                                    var submitButton = Ext.getCmp('submitButton');
                                    submitButton._handler.call(submitButton.scope, submitButton, Ext.EventObject());
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
                                var password = Ext.getCmp('loginPanel_password');
                                if (password.getValue() == '') {
                                    password.focus();
                                    return false;
                                }

                                var APIURL_textfield = Ext.getCmp('APIURL_textfield');
                                var enteredURL = APIURL_textfield.getValue();
                                if (enteredURL == '') {
                                    APIURL_textfield.focus();
                                    return false;
                                }

                                //update local var and stores

                                //Ext.getStore('DeviceStore')._proxy._url = enteredURL + '/Devices';
                                // Ext.getStore('GroupStore')._proxy._url = enteredURL + '/Groups';
                                //Ext.getStore('SceneStore')._proxy._url = enteredURL + '/Scenes';

                                //Save to store/localstorage
                                appSettingsStore = Ext.getStore('appSettingsStore');
                                var BaseURLRecord = appSettingsStore.findRecord('SettingName', 'BaseURL');
                                if (BaseURLRecord != null) {
                                    BaseURLRecord.set('Value', enteredURL);
                                    appSettingsStore.sync();
                                }
                                else {
                                    appSettingsStore.add({ SettingName: 'BaseURL', Value: enteredURL });
                                    appSettingsStore.sync();
                                }

                                zvsMobile.app.SetStoreProxys();

                                Ext.Ajax.request({
                                    url: zvsMobile.app.BaseURL() + '/login',
                                    method: 'POST',
                                    params: {
                                        u: Math.random(),
                                        password: password.getValue()
                                    },
                                    success: function (response, opts) {
                                        console.log(response);
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