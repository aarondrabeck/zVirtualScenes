using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;
using zVirtualScenesApplication;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;

namespace GlobalHotKeyPlugin
{
    [Export(typeof(zvsPlugin))]
    public class GlobalHotKeyPlugin : zvsPlugin
    {
        public volatile bool isActive;
        private KeyboardHook hook = new KeyboardHook();

        public GlobalHotKeyPlugin()
            : base("GLOBALHOTKEYS")
        {
            PluginName = "Global HotKeys";
        }

        public override void Initialize()
        {
            zvsAPI.Scenes.Properties.New("Global Hotkey", "Hotkey that will activate this scene.", "None", Data_Types.LIST);
            
            foreach (string option in Enum.GetNames(typeof(CustomHotKeys)))
            {
                zvsAPI.Scenes.Properties.NewPropertyOption("Global Hotkey", option.Replace('_', '+'));
            }            
        }

        protected override bool StartPlugin()
        {
            int success = 0; 
            int errors = 0; 
            #region Register Global Hot Keys
            try
            {
                hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D0)) { success++; } else { errors++; }; 
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D1)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D2)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D3)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D4)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D5)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D6)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D7)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D8)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D9)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.A)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.B)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.C)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.E)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.F)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.G)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.H)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.I)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.J)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.K)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.L)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.M)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.N)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.O)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.P)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Q)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.R)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.S)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.T)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.U)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.V)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.W)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.X)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Y)) { success++; } else { errors++; };
                if (hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Z)) { success++; } else { errors++; };
            }
            catch (Exception ex)
            {
                zvsAPI.WriteToLog(Urgency.ERROR, "Failed to register global hotkeys. - " + ex.Message);
            }
            #endregion

            zvsAPI.WriteToLog(Urgency.INFO, string.Format("{0} plugin started. (Registered {1} hotkeys with {2} errors.)", PluginName, success , errors));

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            zvsAPI.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");
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

            foreach (Scene scene in zvsAPI.Scenes.GetScenes())
            {
                string sceneHotKey = zvsAPI.Scenes.Properties.GetScenePropertyValue(scene.id, "Global Hotkey");

                if (!string.IsNullOrEmpty(sceneHotKey))
                {
                    if (sceneHotKey.Replace("+", "_").Equals(KeysPresseed))
                    {
                        string result = scene.RunScene();
                        zvsAPI.WriteToLog(Urgency.INFO, "Global HotKey (" + KeysPresseed + "): " + result);
                    }
                }  
            }            
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
