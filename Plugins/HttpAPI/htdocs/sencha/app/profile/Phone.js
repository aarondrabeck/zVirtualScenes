Ext.define('zvsMobile.profile.Phone', {
    extend: 'Ext.app.Profile',

    config: {
        views: ['Main',
                'DevicePhoneViewPort',
                'ScenePhoneViewPort',
                'GroupPhoneViewPort']
    },

    isActive: function () {
        // return true;
        return Ext.os.is.Phone;
    },



    launch: function () {

        zvsMobile.tabPanel = Ext.create('zvsMobile.view.phone.Main');

        if (zvsMobile.app.BaseURL() != '') {

            zvsMobile.app.SetStoreProxys();
            //see if they are logged in 
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
                            zvsMobile.app.SetStoreProxys();
                            var settings = zvsMobile.tabPanel.items.items[4];
                            settings.items.items[1].fireEvent('loggedIn');
                        }
                        else {
                            zvsMobile.app.fireEvent('ShowLoginScreen');
                        }
                    }
                    else {
                        zvsMobile.app.fireEvent('ShowLoginScreen');
                    }
                },
                failure: function (result, request) {
                    zvsMobile.app.fireEvent('ShowLoginScreen');
                }
            });
        }
        else {
            zvsMobile.app.fireEvent('ShowLoginScreen');
        }

    }
});