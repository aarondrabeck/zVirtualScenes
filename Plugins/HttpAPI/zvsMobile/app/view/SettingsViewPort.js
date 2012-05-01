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
                                var logoutPanel = Ext.getCmp('SettingsViewPort').items.items[2];  
								logoutPanel.items.items[0].updateHtml('<div class="logout_info"><p> Logged in to: ' + zvsMobile.app.BaseURL() + '</p></div>');
                               
							   Ext.getCmp('SettingsViewPort').setActiveItem(logoutPanel);
                                zvsMobile.tabPanel.getTabBar().getComponent(0).setDisabled(false);
                                zvsMobile.tabPanel.getTabBar().getComponent(1).setDisabled(false);
                                zvsMobile.tabPanel.getTabBar().getComponent(2).setDisabled(false);

                                //Get data
                                DeviceStore.load();
                                SceneStore.load();
                                GroupStore.load();


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
                                var logInPanel = Ext.getCmp('SettingsViewPort').items.items[1];
                                Ext.getCmp('SettingsViewPort').setActiveItem(logInPanel);
                                zvsMobile.tabPanel.getTabBar().getComponent(0).setDisabled(true);
                                zvsMobile.tabPanel.getTabBar().getComponent(1).setDisabled(true);
                                zvsMobile.tabPanel.getTabBar().getComponent(2).setDisabled(true);

                                zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap', zvsMobile.tabPanel.getTabBar().getComponent(3));
                            }
                        }
                    }
                    ],
            listeners: {
                activate: function () {
                    if (zvsMobile.app.BaseURL() != '') {
                        Ext.Ajax.request({
                            url: zvsMobile.app.BaseURL() + '/login',
                            method: 'GET',
                            params: {
                                u: Math.random()
                            },
                            success: function (response, opts) {

                                if (response.responseText != '') {

                                    var result = JSON.parse(response.responseText);
                                    if (result.success && result.isLoggedIn) {
                                        var logoutPanel = Ext.getCmp('SettingsViewPort').items.items[2];
                                        Ext.getCmp('SettingsViewPort').setActiveItem(logoutPanel);
                                    }
                                    else {
                                        Ext.getCmp('SettingsViewPort').items.items[2].fireEvent('loggedOut');
                                    }
                                }
                                else {
                                    Ext.getCmp('SettingsViewPort').items.items[2].fireEvent('loggedOut');
                                }
                            },
                            failure: function (result, request) {
                                Ext.getCmp('SettingsViewPort').items.items[2].fireEvent('loggedOut');
                            }
                        });
                    }
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
