using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;
using zVirtualScenesApplication;
using System.Windows.Forms;

namespace GlobalHotKeyPlugin
{
    [Export(typeof(Plugin))]
    public class GlobalHotKeyPlugin : Plugin
    {
        public volatile bool isActive;
        private KeyboardHook hook = new KeyboardHook();
        //Form backgroundForm = new Form();

        public GlobalHotKeyPlugin()
            : base("GlobalHotKeyPlugin")
        {
            PluginName = "Global HotKey Plugin";
        }

        public override void Initialize()
        {
            //TODO: Make this a scene property

            //API.Object.Properties.NewObjectProperty("GlbHotKey", "Hot Key to activate scene.", "", ParamType.LIST);
            
            //foreach (string option in Enum.GetNames(typeof(CustomHotKeys)))
            //{
            //    API.Object.Properties.NewObjectPropertyOption("GlbHotKey", option.Replace('_','+'));
            //}            
        }

        protected override bool StartPlugin()
        {          

            #region Register Global Hot Keys
            try
            {
                hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D0);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D1);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D2);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D3);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D4);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D5);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D6);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D7);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D8);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D9);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.A);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.B);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.C);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.E);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.F);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.G);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.H);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.I);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.J);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.K);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.L);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.M);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.N);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.O);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.P);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Q);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.R);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.S);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.T);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.U);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.V);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.W);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.X);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Y);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Z);
                API.WriteToLog(Urgency.INFO, "Registered global hotkeys.");
            }
            catch (Exception ex)
            {
                API.WriteToLog(Urgency.ERROR, "Failed to register global hotkeys. - " + ex.Message);
            }
            #endregion

            //TODO:
            API.WriteToLog(Urgency.INFO, string.Format("{0} plugin started. With {1} {2}", PluginName, 99 , "Errors."));

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            API.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");
            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        { }

        public override void ProcessCommand(QuedCommand cmd)
        { }

        public override void Repoll(string id)
        { }

        public override void ActivateGroup(string GroupName)
        { }

        public override void DeactivateGroup(string GroupName)
        { }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            string modifiers = e.Modifier.ToString().Replace(", ", "_");
            string KeysPresseed = modifiers + "_" + e.Key.ToString();

            if (IsReady)
            {

            }
            ////Learn Mode
            //if (formSceneProperties.isOpen)
            //    formSceneProperties.SetGlobalHotKey(KeysPresseed);
            ////Run Mode
            //else
            //{
            //    foreach (Scene thiscene in MasterScenes)
            //    {
            //        if (Enum.GetName(typeof(CustomHotKeys), thiscene.GlobalHotKey) == KeysPresseed)
            //        {
            //            SceneResult result = thiscene.Run(this);
            //            AddLogEntry((UrgencyLevel)result.ResultType, "Global HotKey Interface:  (" + KeysPresseed + ") " + result.Description);
            //        }
            //    }
            //}
        }

        public enum CustomHotKeys
        {
            None = 0,
            Alt_Control_Win_A = 1,
            Alt_Control_Win_B = 2,
            Alt_Control_Win_C = 3,
            Alt_Control_Win_D = 4,
            Alt_Control_Win_E = 5,
            Alt_Control_Win_F = 6,
            Alt_Control_Win_G = 7,
            Alt_Control_Win_H = 8,
            Alt_Control_Win_I = 9,
            Alt_Control_Win_J = 10,
            Alt_Control_Win_K = 11,
            Alt_Control_Win_L = 12,
            Alt_Control_Win_M = 13,
            Alt_Control_Win_N = 14,
            Alt_Control_Win_O = 15,
            Alt_Control_Win_P = 16,
            Alt_Control_Win_Q = 17,
            Alt_Control_Win_R = 18,
            Alt_Control_Win_S = 19,
            Alt_Control_Win_T = 20,
            Alt_Control_Win_U = 21,
            Alt_Control_Win_V = 22,
            Alt_Control_Win_W = 23,
            Alt_Control_Win_X = 24,
            Alt_Control_Win_Y = 25,
            Alt_Control_Win_Z = 26,
            Alt_Control_Win_D1 = 27,
            Alt_Control_Win_D2 = 28,
            Alt_Control_Win_D3 = 29,
            Alt_Control_Win_D4 = 30,
            Alt_Control_Win_D5 = 31,
            Alt_Control_Win_D6 = 32,
            Alt_Control_Win_D7 = 33,
            Alt_Control_Win_D8 = 34,
            Alt_Control_Win_D9 = 35,
            Alt_Control_Win_D0 = 36

        }
    }
}
