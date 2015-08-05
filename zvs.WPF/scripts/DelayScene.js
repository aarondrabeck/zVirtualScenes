
//Useage:
//var s = new sceneHelper();

//s.delay("SCENE NAME", number_of_seconds_do_delay, true if you want to run it without waiting, false if you want to wait
//s.delay("a", 1000, false);
//s.delay("a", 1000, true);
var sceneHelper = function () {

    this.delay = function (scene, waitDuration, async) {
        if(typeof scene == 'undefined') return;
        if(typeof waitDuration == 'undefined') waitDuration = 1000;
        if(typeof async == 'undefined') async = false;
        var cmd = "RunScene('" + scene + "')";
        if(typeof scene == "number") {
            cmd = "RunScene(" + scene + ")";
        }
        if(async) {
            Delay(cmd, waitDuration);
        } else {
            System.Threading.Thread.Sleep(waitDuration);
            eval(cmd);
        }
		
    }
}

return;