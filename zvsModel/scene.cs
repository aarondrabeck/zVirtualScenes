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
    
    public partial class scene : INotifyPropertyChanged
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
    
        public scene()
        {
            this.device_value_triggers = new ObservableCollection<device_value_triggers>();
            this.scene_commands = new ObservableCollection<scene_commands>();
            this.scene_property_value = new ObservableCollection<scene_property_value>();
            this.scheduled_tasks = new ObservableCollection<scheduled_tasks>();
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
    
    	private string _friendly_name;
        public string friendly_name {
    		get { 
    			return _friendly_name;
    		} 
    		set {
    			if (value != _friendly_name){
    				_friendly_name = value;
    			    NotifyPropertyChanged("friendly_name");
    			}
    		}
    	 }
    
    	private Nullable<int> _sort_order;
        public Nullable<int> sort_order {
    		get { 
    			return _sort_order;
    		} 
    		set {
    			if (value != _sort_order){
    				_sort_order = value;
    			    NotifyPropertyChanged("sort_order");
    			}
    		}
    	 }
    
    	private bool _is_running;
        public bool is_running {
    		get { 
    			return _is_running;
    		} 
    		set {
    			if (value != _is_running){
    				_is_running = value;
    			    NotifyPropertyChanged("is_running");
    			}
    		}
    	 }
    
    
    	private ObservableCollection<device_value_triggers> _device_value_triggers;
        public virtual ObservableCollection<device_value_triggers> device_value_triggers {
    		get { 
    			return _device_value_triggers;
    		} 
    		set {
    			if (value != _device_value_triggers){
    				_device_value_triggers = value;
    			    NotifyPropertyChanged("device_value_triggers");
    			}
    		}
    	 }
    
    	private ObservableCollection<scene_commands> _scene_commands;
        public virtual ObservableCollection<scene_commands> scene_commands {
    		get { 
    			return _scene_commands;
    		} 
    		set {
    			if (value != _scene_commands){
    				_scene_commands = value;
    			    NotifyPropertyChanged("scene_commands");
    			}
    		}
    	 }
    
    	private ObservableCollection<scene_property_value> _scene_property_value;
        public virtual ObservableCollection<scene_property_value> scene_property_value {
    		get { 
    			return _scene_property_value;
    		} 
    		set {
    			if (value != _scene_property_value){
    				_scene_property_value = value;
    			    NotifyPropertyChanged("scene_property_value");
    			}
    		}
    	 }
    
    	private ObservableCollection<scheduled_tasks> _scheduled_tasks;
        public virtual ObservableCollection<scheduled_tasks> scheduled_tasks {
    		get { 
    			return _scheduled_tasks;
    		} 
    		set {
    			if (value != _scheduled_tasks){
    				_scheduled_tasks = value;
    			    NotifyPropertyChanged("scheduled_tasks");
    			}
    		}
    	 }
    }
}
