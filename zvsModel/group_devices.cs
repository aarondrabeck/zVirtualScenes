//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace zVirtualScenesModel
{
    
    public partial class group_devices : INotifyPropertyChanged
    {
    	public event PropertyChangedEventHandler PropertyChanged;
         protected void NotifyPropertyChanged(string name)
            {
                onBeforePropertyChanged(name);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
                onAfterPropertyChanged(name);
            }
         partial void onBeforePropertyChanged(string name);
         partial void onAfterPropertyChanged(string name);
    
    
    	private int _device_id;
        public int device_id {
    		get { 
    			return _device_id;
    		} 
    		set {
    			if (value != _device_id){
    				_device_id = value;
    			    NotifyPropertyChanged("device_id");
    			}
    		}
    	 }
    
    	private int _id;
        public int id {
    		get { 
    			return _id;
    		} 
    		set {
    			if (value != _id){
    				_id = value;
    			    NotifyPropertyChanged("id");
    			}
    		}
    	 }
    
    	private int _group_id;
        public int group_id {
    		get { 
    			return _group_id;
    		} 
    		set {
    			if (value != _group_id){
    				_group_id = value;
    			    NotifyPropertyChanged("group_id");
    			}
    		}
    	 }
    
    
    	private device _device;
        public virtual device device {
    		get { 
    			return _device;
    		} 
    		set {
    			if (value != _device){
    				_device = value;
    			    NotifyPropertyChanged("device");
    			}
    		}
    	 }
    
    	private group _group;
        public virtual group group {
    		get { 
    			return _group;
    		} 
    		set {
    			if (value != _group){
    				_group = value;
    			    NotifyPropertyChanged("group");
    			}
    		}
    	 }
    }
}
