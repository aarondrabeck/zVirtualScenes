using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Security.Cryptography;
using BrightIdeasSoftware;
using System.Drawing;
using System.Drawing.Drawing2D;

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

        public static string ExtractNumbers(string expr)
        {
            return string.Join(null, System.Text.RegularExpressions.Regex.Split(expr, "[^\\d]"));
        }

    }
    public class CustomeLevelRenderer : BarRenderer
    {
        private void Draw3DBorder(Graphics g, Rectangle r)
        {
            int PenWidth = (int)Pens.White.Width;

            g.DrawLine(Pens.DarkGray,
                new Point(r.Left, r.Top),
                new Point(r.Width - PenWidth, r.Top));
            g.DrawLine(Pens.DarkGray,
                new Point(r.Left, r.Top),
                new Point(r.Left, r.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(r.Left, r.Height - PenWidth),
                new Point(r.Width - PenWidth, r.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(r.Width - PenWidth, r.Top),
                new Point(r.Width - PenWidth, r.Height - PenWidth));
        } 

        public override void Render(Graphics g, Rectangle r)
        {
            string LevelSuffix = ""; 
            int level = 0;
            try
            {
                if (this.GetText().Contains("L"))
                {
                    string strippedLevel = this.GetText().Replace("L", "");
                    level = Convert.ToInt32(strippedLevel);
                    if (level > 98)
                        level = 100;
                    LevelSuffix = "%";

                    double width = r.Width;
                    width = width / 100 * level;
                    r.Width = (int)width;

                    if (r.Width == 0)
                        r.Width = 1;

                    Draw3DBorder(g,r);
                    g.FillRectangle(Brushes.LightYellow, r);
                }
                else if (this.GetText().Contains("T"))
                {
                    string strippedLevel = this.GetText().Replace("T", "");
                    level = Convert.ToInt32(strippedLevel);                    
                    LevelSuffix = "F";

                    double width = r.Width;
                    width = width / 100 * level;
                    r.Width = (int)width;

                    if (r.Width == 0)
                        r.Width = 1;

                    g.FillRectangle(Brushes.LightSkyBlue, r);
                }
                
            }
            catch
            {
                level = 1;
                g.FillRectangle(Brushes.Blue, r);
                
                LevelSuffix = "???";
            }

            
            
            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            switch (this.Column.TextAlign)
            {
                case HorizontalAlignment.Center: fmt.Alignment = StringAlignment.Center; break;
                case HorizontalAlignment.Left: fmt.Alignment = StringAlignment.Near; break;
                case HorizontalAlignment.Right: fmt.Alignment = StringAlignment.Far; break;
            }
            g.DrawString(level + LevelSuffix, this.Font, this.TextBrush, r, fmt);
        }
    }
   
    
        
    
}