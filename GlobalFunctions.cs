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