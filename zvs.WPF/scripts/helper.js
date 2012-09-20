////usage 
////import the library


//require("helper.js");

////create an instance of our helper class
//var h = new helper();

////now you have access to many application specific variables
//log(h.appPath);
//log(h.hostDetails);
//log(h.userName);
//log(h.applicationName);
//log(h.applicationVersion);
//log(h.appPath);
//log(h.applicationNameAndVersion);
//log(h.appDataPath);
//log(h.DBNamePlusFullPath);


////you also have acess to devices
//var details = h.deviceDetails("Master Bedroom");
//log(details);

////Get a specific device
//var d = h.deviceByName("Bar Lights");

////output some specific property of a device
//log(d.Name);

////get device value by name or specific device
//log(h.getDeviceValue("Bar Lights", "Manufacturer Name"));
//log(h.getDeviceValue(d, "Manufacturer Name"));



////get all of the plugins, iterate over and show properties
//var plugins = h.getPlugins();
//for(var p in plugins) {
//	log(p.Name + ":" + p.UniqueIdentifier + ":" + p.isEnabled + ":" + p.Description);
//}


//grab the noaa plugin, output specific plugin properties (lat, long, sunrise, sunset only available in builds later than 3.8.5)
//var noaa = h.getPlugin("NOAA");
//log(noaa.isDark());
//log(noaa.Sunrise);
//log(noaa.Sunset);
//log(noaa.Lat);
//log(noaa.Long);


helper = function() {	
	this.appPath = zvs.WPF.App.Path;	
	this.hostDetails = zvs.WPF.App.GetHostDetails;
	this.userName = System.Environment.UserName;
	
	this.applicationName = zvs.Processor.Utils.ApplicationName;
	this.applicationVersion = zvs.Processor.Utils.ApplicationVersion;
	this.applicationNameAndVersion = zvs.Processor.Utils.ApplicationNameAndVersion;
	this.appDataPath = zvs.Processor.Utils.AppDataPath;
	this.DBNamePlusFullPath = zvs.Processor.Utils.DBNamePlusFullPath;
	this.app = System.Windows.Application.Current;
	this.core = this.app.zvsCore;
	this.pluginManager = this.core.pluginManager;
	this.triggerManager = this.core.triggerManager;
	this.scheduledTaskManager = this.core.scheduledTaskManager;

	this.getPlugins = function() {
		if(typeof this.pluginManager !='undefined') return this.pluginManager.GetPlugins();
		return;
	}
	this.getPlugin = function(uniqueIdentifier) {
		if(typeof this.pluginManager !='undefined') return this.pluginManager.GetPlugin(uniqueIdentifier);
		return;
	}

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
			details += "Values: "+this.deviceValues(device)+"\n";		
			
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
	this.deviceValues = function(device) {
		var details = "";
		if(typeof device == 'string') device = this.deviceByName(device);
		for (var value in device.Values)
		{	
			details+= value.Name + " = " + value.Value + "\n";
		}
		return details;
	}
	this.builtinCommands = function() {
		return zvsContext.BuiltinCommands;
	}

	this.builtinCommand = function(name) {
		var c = this.builtinCommands();
		for (var cmd in c) {
			if(cmd.Name == name || cmd.UniqueIdentifier == name) return cmd;
		}
		return;
	}
	this.runBuiltinCommand = function(cmd, arg) {
		if(typeof cmd == 'string') {
			cmd = this.builtinCommand(cmd);
		}
		if(typeof cmd !='undefined') {
			cmd.Run(zvsContext, arg);
		}
	}
	this.repollAll = function () {
	    return this.runBuiltinCommand("REPOLL_ALL");
	}
	this.repollDevice = function (dev) {
	    if (typeof dev != 'number') dev = dev.DeviceId;
	    return this.runBuiltinCommand("REPOLL_ME", dev);
	}
	this.groupOn = function (group) {
	    return this.runBuiltinCommand("GROUP_ON", group);
	}
	this.groupOff = function (group) {
	    return this.runBuiltinCommand("GROUP_OFF", group);
	}
	this.timeDelayScene = function (delay) {
	    return this.runBuiltinCommand("TIMEDELAY", delay);
	}


}
