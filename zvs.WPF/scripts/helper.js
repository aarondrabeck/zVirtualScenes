 
helper = function() {
	this.IsJavascriptCommand = function(cmd) {
		return (cmd.hasOwnProperty("Script"));
	}

	/*
	Usage:
	require("helper.js");
	var h = new helper();
	log(h.SceneDetails());
	return true;
	*/	
	this.SceneDetails = function() {
		var sceneDetails = "";
		if(HasScene) {
			for(var cmd in Scene.Commands) {
				if(!this.IsJavascriptCommand(cmd.Command)) {
					sceneDetails+=(cmd.Device.Name + ", Current Level: " + cmd.Device.CurrentLevelText + " (" + cmd.Device.CurrentLevelInt + ")") + "\n";
				}
			}		
		} else {
			sceneDetails = "No scene information available";
		}
		return sceneDetails;
	}
}
