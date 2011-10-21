using System.ComponentModel.Composition;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using zVirtualScenesAPI;
using System.Data;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace zvsMobile
{
    [Export(typeof(Plugin))]
    public class zvsMobilePlugin : Plugin
    {
        public volatile bool isActive;

        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);

        public zvsMobilePlugin()
            : base("ZVSMOBILE",
               "zvsMobile Plugin",
                "This plug-in acts as a HTTP server to send respond to JSON AJAX requests."
                ) { }       

        public override void Initialize()
        {
            DefineOrUpdateSetting(new plugin_settings
            {
                name = "PORT",
                friendly_name = "HTTP Port",
                value = "9999",
                value_data_type = (int)Data_Types.INTEGER,
                description = "The port that HTTP will listen for commands on."
            });           
        }

        protected override bool StartPlugin()
        {
            try
            {
                if (!httplistener.IsListening)
                {
                    int port = 9999;
                    int.TryParse(GetSettingValue("PORT"), out port);
                    httplistener.Prefixes.Add("http://*:" + port + "/");
                    httplistener.Start();

                    ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(HttpListen));

                    WriteToLog(Urgency.INFO, string.Format("{0} plugin started on port {1}.", this.Friendly_Name, port));
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error while starting. " + ex.Message);
            }

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin ended.");

            try
            {
                if (httplistener != null && httplistener.IsListening)
                {
                    httplistener.Stop();
                    WriteToLog(Urgency.INFO, string.Format("{0} plugin shutdown."));
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error while shuting down. " + ex.Message);
            }

            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
        }
        public override bool ProcessDeviceCommand(device_command_que cmd)
        {
            return true;
        }
        public override bool ProcessDeviceTypeCommand(device_type_command_que cmd)
        {
            return true;
        }
        public override bool Repoll(device device)
        {
            return true;
        }
        public override bool ActivateGroup(long groupID)
        {
            return true;
        }
        public override bool DeactivateGroup(long groupID)
        {
            return true;
        }     

        private void HttpListen(object state)
        {
            try
            {
                while (httplistener.IsListening)
                {
                    httplistener.BeginGetContext(new AsyncCallback(HttpListenerCallback), httplistener);
                    listenForNextRequest.WaitOne();
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, ex.Message);
            }
        }

        public void HttpListenerCallback(IAsyncResult result)
        {
            try
            {
                HttpListener listener = (HttpListener)result.AsyncState;
                HttpListenerContext context = null;

                if (listener == null) return;

                try
                {
                    context = listener.EndGetContext(result);
                }
                catch
                {
                    return;
                }
                finally
                {
                    listenForNextRequest.Set();
                }

                if (context == null)
                    return;

                // Obtain a response object
                using (System.Net.HttpListenerResponse response = context.Response)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    response.ContentType = "application/javascript;charset=utf-8";

                    string data = string.Empty;
       
                    if (context.Request.RawUrl.Contains("/JSON/GetDeviceList"))
                    {
                        List<object> devices = new List<object>();
                        foreach (device d in zvsEntityControl.zvsContext.devices)
                        {
                            var device = new {id = d.id,
                                     name = d.friendly_name,
                                     on_off = d.GetLevelMeter() > 0 ? "ON" : "OFF",
                                     level = d.GetLevelMeter(),
                                     level_txt = d.GetLevelText(),
                                     type = d.device_types.name,
                            };

                            devices.Add(device);                                 
                        }
                        data = context.Request.QueryString["callback"] + "(" + js.Serialize(devices) + ");";
                    }         

                    if (context.Request.RawUrl.Contains("/JSON/GetDeviceDetails"))
                    {
                        //if (!string.IsNullOrEmpty(context.Request.QueryString["id"]))
                        //{
                            long id = 0;
                            long.TryParse(context.Request.QueryString["id"],out id);
                            if (id > 0)
                            {
                                device d = zvsEntityControl.zvsContext.devices.SingleOrDefault(o => o.id == id);

                                if (d != null)
                                {
                                    var details = new
                                    {
                                        id = d.id,
                                        name = d.friendly_name,
                                        on_off = d.GetLevelMeter() > 0 ? "ON" : "OFF",
                                        level = d.GetLevelMeter(),
                                        level_txt = d.GetLevelText(),
                                        type = d.device_types.name,
                                        type_txt = d.device_types.friendly_name,
                                        last_heard_from = d.last_heard_from.HasValue ? d.last_heard_from.Value.ToString() : "",
                                        groups = d.GetGroups
                                    };
                                    data = context.Request.QueryString["callback"] + "(" + js.Serialize(details) + ");";
                                }
                            }
                        //}
                    }



                    response.StatusCode = (int)HttpStatusCode.OK;
                    MemoryStream stream = new MemoryStream();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data + Environment.NewLine);
                    stream.Write(buffer, 0, buffer.Length);

                    byte[] bytes = stream.ToArray();
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                }

            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, ex.Message + ex.InnerException + "END");
            }
        }     

       
    }
}