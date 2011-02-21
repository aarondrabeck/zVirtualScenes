using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Security.Cryptography;

namespace zVirtualScenesApplication
{
    
    
    public class GlobalFunctions
    {
        public RichTextBox log = new RichTextBox();

       

        ///// <summary>
        ///// SEND HTTP 
        ///// </summary>
        ///// <param name="URL">URL as string</param>
        ///// <returns>HTML PAGE</returns>
        //public string HTTPSend(string URL)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
        //    request.Method = "GET";
        //    request.Timeout = 30000;

        //    string str = string.Empty;
        //    try
        //    {
        //        WebResponse response = request.GetResponse();
        //        str = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default).ReadToEnd();
        //        response.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        zVirtualScenes.Invoke(new LogThisDelegate(this.LogThis), new object[] { 2, "Exception occured: " + e.Message });                
        //    }
        //    return str;
        //}


        /// <summary>
        /// Sets Property and if value changed call the eventHaldler event to notify binded ListBoxs of change. 
        /// </summary>
        /// <param name="URL">URL as string</param>
        public static void Set<T>(object owner, string propName, ref T oldValue, T newValue, PropertyChangedEventHandler eventHandler)
        {
            // make sure the property name really exists
            if (owner.GetType().GetProperty(propName) == null)
            {
                throw new ArgumentException("No property named" +  propName + " on " + owner.GetType().FullName);
            }        
                // we only raise an event if the value has changed
            if (!Equals(oldValue, newValue))
            {
                oldValue = newValue;
                if (eventHandler != null)
                {
                eventHandler(owner, new PropertyChangedEventArgs(propName));
                }
            }
        }

        


    }
}