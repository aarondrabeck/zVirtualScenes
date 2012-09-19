//usage 
//require("helper.js");
////
//var h = new helper();

//log(h.appPath);
//log(h.hostDetails);
//log(h.userName);
//log(h.applicationName);
//log(h.applicationVersion);
//log(h.appPath);
//log(h.applicationNameAndVersion);
//log(h.appDataPath);
//log(h.DBNamePlusFullPath);
//
//
//var details = h.deviceDetails("Master Bedroom");
//log(details);
//
//Get a specific device
//var d = h.deviceByName("Bar Lights");
//
//output some specific property
//log(d.Name);
//
//get device value by name or specific device
//log(h.getDeviceValue("Bar Lights", "Manufacturer Name"));
//log(h.getDeviceValue(d, "Manufacturer Name"));


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
	this.deviceDetails = function(device) {
		var details = "";
		var name = device
		if(typeof name == 'string') {
			device = this.deviceByName(device);					
		} else {
			name = device.Name;
		}
		if(typeof device != 'undefined') {
			details += "Name: "+device.Name+"\n";
			details += "NodeNumber: "+device.NodeNumber+"\n";
			details += "LastHeardFrom: "+device.LastHeardFrom+"\n";
			details += "CurrentLevelText: "+device.CurrentLevelText+"\n";
			details += "CurrentLevelInt: "+device.CurrentLevelInt+"\n";
			details += "Type: "+device.Type.Name+"\n";						
		} else {			
			details = "No device by that name: " + name;
		}
		return details;
	}
	this.sceneByName = function(name) {
		for(var s in zvsContext.Scenes) {
			if(s.Name == name)  return s;
		}
		return;	
	}	

	this.getDeviceValue = function(device, valueName) {
		if(typeof device == 'string') device = this.deviceByName(device);
		for(var v in device.Values) {
			if(v.Name == valueName){
				return v.Value;
			}
		}
		return "";
	}
	
	
	this.appPath = zvs.WPF.App.Path;	
	this.hostDetails = zvs.WPF.App.GetHostDetails;
	this.userName = System.Environment.UserName;
	
	this.applicationName = zvs.Processor.Utils.ApplicationName;
	this.applicationVersion = zvs.Processor.Utils.ApplicationVersion;
	this.applicationNameAndVersion = zvs.Processor.Utils.ApplicationNameAndVersion;
	this.appDataPath = zvs.Processor.Utils.AppDataPath;
	this.DBNamePlusFullPath = zvs.Processor.Utils.DBNamePlusFullPath;


	
}
