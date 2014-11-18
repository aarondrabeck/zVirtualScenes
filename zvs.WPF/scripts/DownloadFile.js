

//lets make sure our output directory exists
if (!System.IO.Directory.Exists("c:\\temp\\")) System.IO.Directory.CreateDirectory("c:\\temp\\");

//execute wget.exe to download google, find a windows version of wget.exe (gnuwin or whatever)
var p = shell("wget.exe", "--output-file=c:\\temp\\google.html  http://www.google.com");

//wait until the process has finished
while (!p.HasExited) {
    log('wait');
    //hey look, we can sleep the thread!
    System.Threading.Thread.Sleep(500);
}

//ok, the download is complete
log('done');

//we still have access to the Process object, how long did it take?
log(p.TotalProcessorTime);


//since out output our results to a file, lets read them in and push them to the log
log(System.IO.File.ReadAllText("c:\\temp\\google.html"));



