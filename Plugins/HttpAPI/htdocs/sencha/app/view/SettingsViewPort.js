Ext.define('zvsMobile.view.SettingsViewPort', {
        extend: 'Ext.Panel',
        xtype: 'SettingsViewPort',
        requires: ['zvsMobile.view.SettingsLogIn',
                   'zvsMobile.view.SettingsLogOut'],
        initialize: function () {
            this.callParent(arguments);
            this.getEventDispatcher().addListener('element', '#SettingsViewPort', 'swipe', this.onTouchPadEvent, this);
        },
        onTouchPadEvent: function (e, target, options, eventController) {
            if (e.direction === 'right' && e.distance > 50 && zvsMobile.tabPanel.getTabBar().getComponent(2)._disabled != true) {
                zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(2));
            }
        },

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

                                //Get data
                                DeviceStore.load();
                                SceneStore.load();
                                GroupsStore.load();


                                //Change view to the device list
                                zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(0));
//                                var task = Ext.create('Ext.util.DelayedTask', function () {
//                                    zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(0));
//                                    console.log('setDevicePaneAcgtives');
//                                });

//                                task.delay(500);

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
