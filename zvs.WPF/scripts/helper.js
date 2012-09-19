//usage 
//require("helper.js");
////
//var h = new helper();

//log(h.appPath);
//log(h.hostDetails);
//log(h.userName);
//
//
//var details = h.deviceDetails("Master Bedroom");
//log(details);
helper = function() {
	this.IsJavascriptCommand = function(cmd) {
		return (cmd.hasOwnProperty("Script"));
	}
	
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
	this.deviceByName = function(name) {
		for(var dev in zvsContext.Devices) {
			if(dev.Name == name)  return dev;
		}
		return;
	}
	this.deviceDetails = function(deviceName) {
		var details = "";
		var dev = this.deviceByName(deviceName);
		if(typeof dev != 'undefined') {
			details += "Name: "+dev.Name+"\n";
			details += "NodeNumber: "+dev.NodeNumber+"\n";
			details += "LastHeardFrom: "+dev.LastHeardFrom+"\n";
			details += "CurrentLevelText: "+dev.CurrentLevelText+"\n";
			details += "CurrentLevelInt: "+dev.CurrentLevelInt+"\n";
			details += "Type: "+dev.Type.Name+"\n";						
		} else {			
			details = "No device by that name: " + deviceName;
		}
		return details;
	}
	this.sceneByName = function(name) {
		for(var s in zvsContext.Scenes) {
			if(s.Name == name)  return s;
		}
		return;	
	}	
	this.appPath = zvs.WPF.App.Path;	
	this.hostDetails = zvs.WPF.App.GetHostDetails;
	this.userName = System.Environment.UserName;
}
