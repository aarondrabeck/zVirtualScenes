using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace zVirtualScenesApplication
{
    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private formzVirtualScenes zVirtualScenesMain;

        public HttpProcessor(TcpClient s, HttpServer srv, formzVirtualScenes Form)
        {
            this.socket = s;
            this.srv = srv;
            this.zVirtualScenesMain = Form;
        }

        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        public void process()
        {
            inputStream = new BufferedStream(socket.GetStream());
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
            }
            catch (Exception e)
            {
               zVirtualScenesMain.LogThis(1,"HTTP Interface: Exception -" + e.ToString());
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            string userIP = socket.Client.RemoteEndPoint.ToString();
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');

            if (tokens.Length != 3)
                zVirtualScenesMain.LogThis(1,"HTTP Interface: ["+userIP+"] Sent invalid http request.");
            else
            {
                http_method = tokens[0].ToUpper();

                if (http_method == "GET")
                {
                    zVirtualScenesMain.LogThis(1,"HTTP Interface: ["+userIP+"] " + request);
                    http_url = tokens[1];

                    //http:/localhost:8085//zVirtualScene?cmd=RunScene&Scene=0 
                    //http:/localhost:8085/zVirtualScene?cmd=MultilevelPowerSwitch&node=2&level=0 
                    //http://localhost:8085/zVirtualScene?cmd=ThermoStat&node=7&HeatCoolMode=-1&FanMode=-1&EngeryMode=-1&HeatPoint=-1&CoolPoint=-1
                    //http://localhost:8085/zVirtualScene?cmd=ListDevices            
                    //http://localhost:8085/zVirtualScene?cmd=ListScenes
                    //http://localhost:8085/zVirtualScene?cmd=BinaryPowerSwitch&state=ON
                    //zVirtualScene?cmd=RepollDevices
                    string[] acceptedCMDs = new string[] { "/zVirtualScene?cmd=RunScene&Scene=" , 
                                                           "/zVirtualScene?cmd=MultilevelPowerSwitch&",
                                                           "/zVirtualScene?cmd=ThermoStat&", 
                                                           "/zVirtualScene?cmd=ListDevices",
                                                           "/zVirtualScene?cmd=ListScenes",                     
                                                           "/zVirtualScene?cmd=BinaryPowerSwitch&",
                                                           "/zVirtualScene?cmd=RepollDevices"};

                    #region RUN SCENE
                    if (http_url.Contains(acceptedCMDs[0]))  
                    {
                        int sceneID = 0;
                        try { sceneID = Convert.ToInt32(http_url.Remove(0, acceptedCMDs[0].Length)); }
                        catch  
                        { 
                            zVirtualScenesMain.LogThis(1,"HTTP Interface: ["+userIP+"] No such scene.");  //LOG
                            outputStream.WriteLine("ERR: No such scene.  ({0})", http_url); //FEEDBACK to USER
                            return; 
                        }
                        if (sceneID > 0)
                        {
                            foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                                if (scene.ID == sceneID)
                                {                                    
                                    SceneResult result = scene.Run(zVirtualScenesMain.ControlThinkController);
                                    zVirtualScenesMain.LogThis((int)result.ResultType, "HTTP Interface: [" + userIP + "] " + result.Description);
                                    outputStream.WriteLine("zVirtualScenes HTTP Interface: [" + userIP + "] " + result.Description + ". ({0})", http_url);
                                    return;
                                } 
                        }
                        zVirtualScenesMain.LogThis(1, "HTTP Interface: ["+userIP+"] Cannot find scene.");
                        outputStream.WriteLine("ERR: Cannot find scene. ({0})", http_url); 
                        return;
                    }
#endregion

                    #region Run MultilevelPowerSwitch
                    if (http_url.Contains(acceptedCMDs[1]))
                    {
                        int node = 0;
                        byte level = 0;
                        try 
                        {                             
                            string prams = http_url.Remove(0, acceptedCMDs[1].Length); //Strip CMD
                            string[] values = prams.Split('&'); //Get values

                            node = Convert.ToInt32(values[0].Remove(0, "node=".Length));
                            level = Convert.ToByte(values[1].Remove(0, "level=".Length)); 
                        }
                        catch
                        {
                            zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Error parsing command. ");  //LOG
                            outputStream.WriteLine("ERR: Error parsing command, check syntax. ({0})", http_url); //FEEDBACK to USER
                            return;
                        }

                        if (node > 0)
                        {
                            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                            {
                                if (device.NodeID == node && device.Type == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch )
                                {
                                    Action action = (Action)device;

                                    if (action != null)
                                    {
                                        //set Level
                                        if (level == 255)
                                            level = 99;
                                        action.Level = level;


                                        ActionResult result = action.Run(zVirtualScenesMain.ControlThinkController);
                                        zVirtualScenesMain.LogThis((int)result.ResultType, "HTTP Interface: [" + userIP + "] " + result.Description);
                                        outputStream.WriteLine("zVirtualScenes HTTP Interface: [" + userIP + "] " + result.Description + " ({0})", http_url);
                                        return;
                                    }
                                }
                            }
                        }
                        zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Cannot find device.");
                        outputStream.WriteLine("ERR: Cannot find device.", http_url);
                        return;
                    }
#endregion

                    #region RUN THERMO
                    if (http_url.Contains(acceptedCMDs[2]))
                    {
                        int node = 0;
                        int HeatCoolMode = -1;
                        int FanMode = -1;
                        int EngeryMode = -1;
                        int HeatPoint = -1;
                        int CoolPoint = -1; 

                        try
                        {
                            string prams = http_url.Remove(0, acceptedCMDs[2].Length); //Strip CMD
                            string[] values = prams.Split('&'); //Get values

                            node = Convert.ToInt32(values[0].Remove(0, "node=".Length));
                            HeatCoolMode = Convert.ToInt32(values[1].Remove(0, "HeatCoolMode=".Length));
                            FanMode = Convert.ToInt32(values[2].Remove(0, "FanMode=".Length));
                            EngeryMode = Convert.ToInt32(values[3].Remove(0, "EngeryMode=".Length));
                            HeatPoint = Convert.ToInt32(values[4].Remove(0, "HeatPoint=".Length));
                            CoolPoint = Convert.ToInt32(values[5].Remove(0, "CoolPoint=".Length));
                        }
                        catch (Exception e) 
                        {
                            zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Error parsing command - " + e);  //LOG
                            outputStream.WriteLine("ERR: Error parsing command, check syntax. ({0})", http_url); //FEEDBACK to USER
                            return;
                        }

                        //Make sure atleast one thermo action was chosen
                        if (HeatCoolMode == -1 && FanMode == -1 && EngeryMode == -1 && HeatPoint == -1 && CoolPoint == -1)
                        {
                            zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Please define at least one Temperature Mode.");  //LOG
                            outputStream.WriteLine("ERR: Please define at least one Temperature Mode. ({0})", http_url); //FEEDBACK to USER
                            return;
                        }                       

                        if (node > 0)
                        {
                            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                            {
                                if (device.NodeID == node && device.Type == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                                {
                                    Action action = (Action)device;
                                    action.HeatCoolMode = HeatCoolMode;
                                    action.FanMode = FanMode;
                                    action.EngeryMode = EngeryMode;
                                    action.HeatPoint = HeatPoint;
                                    action.CoolPoint = CoolPoint;


                                    ActionResult result = action.Run(zVirtualScenesMain.ControlThinkController);
                                    zVirtualScenesMain.LogThis((int)result.ResultType, "HTTP Interface: [" + userIP + "] " + result.Description);
                                    outputStream.WriteLine("HTTP Interface: [" + userIP + "] " + result.Description + ". ({0})", http_url);

                                    return;
                                }
                            }
                        }
                        zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Cannot find device.");
                        outputStream.WriteLine("ERR: Cannot find device.", http_url);
                        return;
                    } 
#endregion

                    #region Device Listing
                    if (http_url.Contains(acceptedCMDs[3]))
                    {
                        zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Sent a device listing.");                        

                        outputStream.WriteLine("LIST START");
                        foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)                        
                            outputStream.WriteLine(device.ToString());
                        
                        outputStream.WriteLine("LIST END");
                        return;
                    }
                    #endregion

                    #region scene Listing
                    if (http_url.Contains(acceptedCMDs[4]))
                    {
                        zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Requested a scene listing.");

                        outputStream.WriteLine("LIST START");
                        foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                            outputStream.WriteLine(scene.ToStringForHTTP());
                        
                        outputStream.WriteLine("LIST END");
                        return;
                    }
                    #endregion

                    #region Run BinaryPowerSwitch
                    if (http_url.Contains(acceptedCMDs[5]))
                    {
                        int node = 0;
                        string state = "";
                        try
                        {
                            string prams = http_url.Remove(0, acceptedCMDs[1].Length); //Strip CMD
                            string[] values = prams.Split('&'); //Get values

                            node = Convert.ToInt32(values[0].Remove(0, "node=".Length));
                            state = values[1].Remove(0, "state=".Length);
                        }
                        catch
                        {
                            zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Error parsing command. ");  //LOG
                            outputStream.WriteLine("ERR: Error parsing command, check syntax. ({0})", http_url); //FEEDBACK to USER
                            return;
                        }

                        if (node > 0)
                        {
                            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                            {
                                if (device.NodeID == node && device.Type == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
                                {
                                    Action action = (Action)device;

                                    if (action != null)
                                    {
                                        //set Level
                                        if (state == "ON")
                                            action.Level = 1; 
                                        else
                                            action.Level = 0;

                                        ActionResult result = action.Run(zVirtualScenesMain.ControlThinkController);
                                        zVirtualScenesMain.LogThis((int)result.ResultType, "HTTP Interface: [" + userIP + "] " + result.Description);
                                        outputStream.WriteLine("zVirtualScenes HTTP Interface: [" + userIP + "] " + result.Description + " ({0})", http_url);
                                        return;
                                    }
                                }
                            }
                        }
                        zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Cannot find device.");
                        outputStream.WriteLine("ERR: Cannot find device.", http_url);
                        return;
                    }
                    #endregion

                    #region Repoll Device
                    if (http_url.Contains(acceptedCMDs[6]))
                    {
                        zVirtualScenesMain.RepollDevices();
                        zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Repolled ZWave devices.");
                        outputStream.WriteLine("zVirtualScenes HTTP Interface: [" + userIP + "] Repolled ZWave devices. ({0})", http_url);                                    
                        return;
                    }
                    #endregion

                outputStream.WriteLine("ERR: Command not recognized. Ignored...", http_url);
                zVirtualScenesMain.LogThis(1, "HTTP Interface: [" + userIP + "] Command not recognized. Ignored...");
                }                   
            }
        }
    }
}
