using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesModel;

namespace zVirtualScenes.Triggers
{
    public class Scripting
    {
        private Core Core;
        private const string _FriendlyName = "ADVANCEDSCRIPT";

        public Scripting(Core Core)
        {
            this.Core = Core;
        }

        public void RunScript(zvsLocalDBEntities context, device_value_triggers trigger)
        {
            Core.Logger.WriteToLog(Urgency.INFO, "Running Advanced Script - " + trigger.FriendlyName, _FriendlyName);

            // Step 1: Split up the lines of the script
            String[] splitScript = trigger.trigger_script.ToLower().Split('\n');
            List<bool> ifStatements = new List<bool>();
            bool waitingForOpen = false;

            // Step 2: Start iterating through all lines of the script
            foreach (String scriptLine in splitScript)
            {
                // Step 3: Detect what command the line is using (eg. RunScene, if, else)
                if (waitingForOpen)
                {
                    if (!scriptLine.StartsWith("{"))
                    {
                        Core.Logger.WriteToLog(Urgency.ERROR, "Script " + trigger.FriendlyName + " started 'if' statement without the { on the next line", _FriendlyName);
                        throw new Exception();
                    }

                    waitingForOpen = false;
                }
                else if (ifStatements.Count > 0 && ifStatements[ifStatements.Count - 1] == false)
                {
                    if (scriptLine.StartsWith("}"))
                    {
                        ifStatements.RemoveAt(ifStatements.Count - 1);
                    }
                }
                else if (scriptLine.StartsWith("runscene"))
                {
                    // Run this scene!
                    String scene_name = scriptLine.Substring(scriptLine.IndexOf(" ") + 1);
                    scene _scene = context.scenes.FirstOrDefault(s => s.friendly_name.ToLower() == scene_name);

                    // Make sure the user specified a correct scene name
                    if (_scene != null)
                    {
                        _scene.RunScene(context);
                        Core.Logger.WriteToLog(Urgency.INFO, "Script " + trigger.FriendlyName + " ran scene '" + scene_name + "'", _FriendlyName);
                    }
                    else
                    {
                        Core.Logger.WriteToLog(Urgency.ERROR, "Script " + trigger.FriendlyName + " specified a invalid scene name '" + scene_name + "'", _FriendlyName);
                        return;
                    }

                }
                else if (scriptLine.StartsWith("if"))
                {
                    // Calculate this if statement!
                    try
                    {
                        String parameters = scriptLine.Substring(scriptLine.IndexOf("(") + 1, scriptLine.LastIndexOf(")") - scriptLine.IndexOf("(") - 1);

                        // TODO: Add ability to have multiple comparisions - if (Dimmer1.Level == 20 && Dimmer2.Level == 30)

                        device_value_triggers.TRIGGER_OPERATORS operatorType;
                        String strOperator;
                        String[] strValues = new String[2];
                        int[] intValues = new int[2];

                        if (parameters.Contains("=="))
                        {
                            operatorType = device_value_triggers.TRIGGER_OPERATORS.EqualTo;
                            strOperator = "==";
                        }
                        else if (parameters.Contains("!="))
                        {
                            operatorType = device_value_triggers.TRIGGER_OPERATORS.NotEqualTo;
                            strOperator = "!=";
                        }
                        else if (parameters.Contains("<"))
                        {
                            operatorType = device_value_triggers.TRIGGER_OPERATORS.LessThan;
                            strOperator = "<";
                        }
                        else if (parameters.Contains(">"))
                        {
                            operatorType = device_value_triggers.TRIGGER_OPERATORS.GreaterThan;
                            strOperator = ">";
                        }
                        else
                        {
                            // ??
                            Core.Logger.WriteToLog(Urgency.ERROR, "Script " + trigger.FriendlyName + " unkown operator in '" + parameters + "'", _FriendlyName);
                            return;
                        }

                        strValues[0] = parameters.Substring(0, parameters.IndexOf(strOperator));
                        strValues[1] = parameters.Substring(parameters.IndexOf(strOperator) + strOperator.Length);

                        intValues[0] = GetValue(context, trigger.FriendlyName, strValues[0]);
                        intValues[1] = GetValue(context, trigger.FriendlyName, strValues[1]);

                        switch (operatorType)
                        {
                            case device_value_triggers.TRIGGER_OPERATORS.EqualTo:
                                if (intValues[0] == intValues[1])
                                    ifStatements.Add(true);
                                else
                                    ifStatements.Add(false);
                                break;
                            case device_value_triggers.TRIGGER_OPERATORS.NotEqualTo:
                                if (intValues[0] != intValues[1])
                                    ifStatements.Add(true);
                                else
                                    ifStatements.Add(false);
                                break;
                            case device_value_triggers.TRIGGER_OPERATORS.LessThan:
                                if (intValues[0] < intValues[1])
                                    ifStatements.Add(true);
                                else
                                    ifStatements.Add(false);
                                break;
                            case device_value_triggers.TRIGGER_OPERATORS.GreaterThan:
                                if (intValues[0] > intValues[1])
                                    ifStatements.Add(true);
                                else
                                    ifStatements.Add(false);
                                break;
                        }

                        waitingForOpen = true;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Core.Logger.WriteToLog(Urgency.ERROR, "Script " + trigger.FriendlyName + " - Unable to get parameters for if statement from line", _FriendlyName);
                        Core.Logger.WriteToLog(Urgency.ERROR, "'" + scriptLine + "'.", _FriendlyName);
                        Core.Logger.WriteToLog(Urgency.ERROR, "Did you include both ( and )?", _FriendlyName);
                        return;
                    }
                }
            }

            Core.Logger.WriteToLog(Urgency.INFO, "Finished Running Advanced Script - " + trigger.FriendlyName, _FriendlyName);
        }

        private int GetValue(zvsLocalDBEntities context, String triggerName, String strValue)
        {
            int intValue;

            bool isint = int.TryParse(strValue, out intValue);

            if (isint)
                return intValue;

            // Step 1: Get Device
            String device_name = strValue.Substring(0, strValue.IndexOf(".")).Trim();
            String value_name = strValue.Substring(strValue.IndexOf(".") + 1).Trim();
            device _device = context.devices.FirstOrDefault(d => d.friendly_name.ToLower() == device_name);

            if (_device != null)
            {
                // Step 2: Get device value
                device_values device_value = _device.device_values.FirstOrDefault(dv => dv.label_name.ToLower() == value_name);

                if (device_value != null)
                {
                    int.TryParse(device_value.value2, out intValue);
                    return intValue;
                }
                else
                {
                    Core.Logger.WriteToLog(Urgency.ERROR, "Script " + triggerName + " specified a invalid device value name name '" + strValue + "'", _FriendlyName);
                    throw new Exception();
                }
            }
            else
            {
                Core.Logger.WriteToLog(Urgency.ERROR, "Script " + triggerName + " specified a invalid device name '" + device_name + "'", _FriendlyName);
                throw new Exception();
            }
        }
    }
}
