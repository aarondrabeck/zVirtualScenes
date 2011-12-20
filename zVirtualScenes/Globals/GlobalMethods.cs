using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesAPI;
using System.Windows.Forms;
using System.Drawing;
using zVirtualScenesCommon;

namespace zVirtualScenesApplication.Globals
{
    public static class GlobalMethods
    {
        public static int DrawDynamicUserInputBoxes(Panel p, Data_Types paramType, int top, int left, string UniqueName, string CommandText, List<string> options, string value, object tag )
        {
            NumericUpDown Numeric = new NumericUpDown();
            switch ((Data_Types)paramType)
                {
                    case Data_Types.NONE:
                        return left;
                    case Data_Types.BOOL:
                        CheckBox cb = new CheckBox();
                        cb.Name = UniqueName;
                        cb.Text = CommandText;
                        cb.Top = top + 3;
                        cb.Left = left + 3;
                        cb.Tag = tag;
                        p.Controls.Add(cb);
                        cb.AutoSize = true;

                        if (!String.IsNullOrEmpty(value))
                        {
                            bool bval = true;
                            bool.TryParse(value, out bval);
                            cb.Checked = bval;
                        }

                        left += cb.Width + 5;
                        break;
                    case Data_Types.DECIMAL:
                        Numeric.Name = UniqueName;
                        Numeric.Top = top;
                        Numeric.Width = 175;
                        Numeric.Left = left;
                        Numeric.Tag = tag;
                        Numeric.DecimalPlaces = 11; 
                        Numeric.Maximum = Decimal.MaxValue;
                        Numeric.Minimum = Decimal.MinValue;
                        p.Controls.Add(Numeric);
                        left += Numeric.Width + 5;

                        if (!String.IsNullOrEmpty(value))
                        {
                            Decimal dec = 0;
                            Decimal.TryParse(value, out dec);
                            Numeric.Value = dec;
                        }

                        break;
                    case Data_Types.BYTE:
                        Numeric.Name = UniqueName;
                        Numeric.Top = top;
                        Numeric.Left = left;
                        Numeric.Tag = tag;
                        Numeric.Maximum = Byte.MaxValue;
                        Numeric.Minimum = Byte.MinValue;
                        Numeric.Width = 50;
                        p.Controls.Add(Numeric);
                        left += Numeric.Width + 5;

                        if (!String.IsNullOrEmpty(value))
                        {
                            Byte b = 0;
                            Byte.TryParse(value, out b);
                            Numeric.Value = b;
                        }

                        break;
                    case Data_Types.INTEGER:
                        Numeric.Name = UniqueName;
                        Numeric.Top = top;
                        Numeric.Left = left;
                        Numeric.Tag = tag;
                        Numeric.Width = 75;
                        Numeric.Maximum = Int64.MaxValue;
                        Numeric.Minimum = Int64.MinValue;
                        p.Controls.Add(Numeric);
                        left += Numeric.Width + 5;

                        if (!String.IsNullOrEmpty(value))
                        {
                            Int32 i = 0;
                            Int32.TryParse(value, out i);
                            Numeric.Value = i;
                        }

                        break;
                    case Data_Types.SHORT:
                        Numeric.Name = UniqueName;
                        Numeric.Top = top;
                        Numeric.Left = left;
                        Numeric.Tag = tag;
                        Numeric.Width = 75;
                        Numeric.Maximum = short.MaxValue;
                        Numeric.Minimum = short.MinValue;
                        p.Controls.Add(Numeric);
                        left += Numeric.Width + 5;

                        if (!String.IsNullOrEmpty(value))
                        {
                            short i = 0;
                            short.TryParse(value, out i);
                            Numeric.Value = i;
                        }

                        break;
                    case Data_Types.STRING:                        
                        TextBox tbx = new TextBox();
                        tbx.Name = UniqueName;
                        tbx.Top = top;
                        tbx.Left = left;
                        tbx.Tag = tag; 
                        p.Controls.Add(tbx);
                        tbx.Width = 400;
                        left += tbx.Width + 5;

                        if (!String.IsNullOrEmpty(value))
                            tbx.Text = value;

                        break;

                    case Data_Types.LIST:
                        ComboBox cmbo = new ComboBox();

                        foreach(string option in options)
                            cmbo.Items.Add(option);

                        cmbo.Name = UniqueName;
                        cmbo.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbo.Top = top;
                        cmbo.Left = left;
                        cmbo.Tag = tag; 
                        p.Controls.Add(cmbo);
                        left += cmbo.Width + 5;

                        if (!String.IsNullOrEmpty(value))
                            cmbo.SelectedIndex = cmbo.Items.IndexOf(value);
                        break;
                    case Data_Types.COMPORT:
                        Numeric.Name = UniqueName;
                        Numeric.Top = top;
                        Numeric.Left = left;
                        Numeric.Tag = tag;
                        Numeric.Width = 75;
                        Numeric.Maximum = 99;
                        Numeric.Minimum = 0;
                        p.Controls.Add(Numeric);
                        left += Numeric.Width + 5;

                        if (!String.IsNullOrEmpty(value))
                        {
                            Int32 i = 0;
                            Int32.TryParse(value, out i);
                            Numeric.Value = i;
                        }

                        break;
                }
            return left;
        }

        
        
    }
}
