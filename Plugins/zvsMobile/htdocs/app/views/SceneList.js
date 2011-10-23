Ext.regModel('Scene', {
    feilds: ['id', 'name', 'is_running']
});

zvsMobile.SceneStore = new Ext.data.Store({
        model: 'Scene',
        remoteFilter: true
});	
	
 zvsMobile.SceneList = Ext.extend(Ext.List, {
		AjaxLoad: function () {
			       zvsMobile.mainTabPanel.setLoading(true);				  
				   Ext.util.JSONP.request({
                    url: 'http://10.1.0.55:9999/JSON/GetSceneList',
                    callbackKey: 'callback',
                    params: {
                        u: Math.random()
                    },
                    callback: function (data) {
                        zvsMobile.SceneStore.loadData(data);                        
                        zvsMobile.mainTabPanel.setLoading(false);
						console.log('Scenes Reloaded');
                    }                    
                });				 
		},

        cls: 'SceneListItem',
        loadingText: 'Loading...',
        emptyText: '<div class="emptyText">Loading...</div>',
        itemTpl: new Ext.XTemplate(
		'<div class="scene">',
            '<div class="imageholder running_{is_running}"></div>',
			'<h2>{name}</h2>',			
		'</div>'),
        scroll: false
    });
 Ext.reg('SceneList',  zvsMobile.SceneList);