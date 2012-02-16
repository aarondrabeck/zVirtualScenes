Ext.require(['Ext.Panel', 'zvsMobile.view.SettingsLogIn', 'zvsMobile.view.SettingsLogOut'], function () {
    Ext.define('zvsMobile.view.SettingsViewPort', {
        extend: 'Ext.Panel',
        xtype: 'SettingsViewPort',

        constructor: function (config) {
            var self = this;
            Ext.apply(config || {}, {
                items: [
                    {
                        xtype: 'toolbar',
                        docked: 'top',
                        title: 'Settings',
                        items: []
                    },
                    {
                        xtype: 'LogIn',
                        listeners: {
                            loggedIn: function () {

                                //activate the logout screen and enable all the tabs...
                                var logoutPanel = self.items.items[2];
                                self.setActiveItem(logoutPanel);
                                zvsMobile.tabPanel.getTabBar().getComponent(0).setDisabled(false);
                                zvsMobile.tabPanel.getTabBar().getComponent(1).setDisabled(false);
                                zvsMobile.tabPanel.getTabBar().getComponent(2).setDisabled(false);

                                //Change view to the device list
                                var devices = zvsMobile.tabPanel.items.items[0];
                                zvsMobile.tabPanel.getLayout().setAnimation({ type: 'slide', direction: 'right' });
                                zvsMobile.tabPanel.setActiveItem(devices);

                                //Get data
                                DeviceStore.load();
                                SceneStore.load();
                                GroupsStore.load();   
                            }
                        }
                    },
                    {
                        xtype: 'LogOut',
                        listeners: {
                            loggedOut: function () {                                
                                //activate the login screen and disable all the tabs...
                                var logInPanel = self.items.items[1];
                                self.setActiveItem(logInPanel);
                                zvsMobile.tabPanel.getTabBar().getComponent(0).setDisabled(true);
                                zvsMobile.tabPanel.getTabBar().getComponent(1).setDisabled(true);
                                zvsMobile.tabPanel.getTabBar().getComponent(2).setDisabled(true);
                            }
                        }
                    }
                    ],
                listeners: {
                    activate: function () {
                       
                        Ext.Ajax.request({
                            url: '/API/login',
                            method: 'GET',
                            params: {
                                u: Math.random()
                            },
                            success: function (response, opts) {
                                var result = JSON.parse(response.responseText);
                                if (result.success && result.isLoggedIn) {
                                    var logoutPanel = self.items.items[2];
                                    self.setActiveItem(logoutPanel);
                                }
                                else {
                                    self.items.items[2].fireEvent('loggedOut');
                                }
                            },
                            failure: function (result, request) {
                                    self.items.items[2].fireEvent('loggedOut');                                
                            }
                        });
                    }
                }
            });
            this.callParent([config]);
        },
        config:
            {
                layout: 'card'
            }
    });

});