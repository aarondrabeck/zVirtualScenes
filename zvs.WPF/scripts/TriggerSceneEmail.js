require("settings.js");
require("gmail.js");

var g = new gmail(settings);
var body = "Hello,\n\nThis event was just triggered:\n\n";
if(HasTrigger) {
	log("Has a trigger attached, lets load up the body");
	body+="ScheduledTask: " + Trigger.Name + "\n";
}
if(HasScene) {
	log("Has a scene attached, lets load up the body");
	body+="Scene: " + Scene.Name + "\n";
}
log("Send it out");
g.Send("TO.ADDRESS@gmail.com", "TO NAME", "zVirtual Scenes - Event", body);