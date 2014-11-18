using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace zvs.WPF
{
    public class ChromeWindow : Window
    {
        public ChromeWindow()
        {
            this.MouseMove += ResetCursor;
            this.Loaded += ResizeableWindow_Loaded;
        }

        // The constants we'll use to identify our custom system menu items
        public const Int32 _SettingsSysMenuID = 1000;
        public const Int32 _AboutSysMenuID = 1001;

        void ResizeableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var wih = new WindowInteropHelper(this);

            /// Get the Handle for the Forms System Menu
            var systemMenuHandle = NativeMethods.GetSystemMenu(wih.Handle, false);

            /// Create our new System Menu items just before the Close menu item
            // InsertMenu(systemMenuHandle, 5, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty); // <-- Add a menu separator
            //InsertMenu(systemMenuHandle, 6, MF_BYPOSITION, _SettingsSysMenuID, "Settings...");
            // InsertMenu(systemMenuHandle, 7, MF_BYPOSITION, _AboutSysMenuID, "About...");

            if (!EnableMaximize)
                NativeMethods.DeleteMenu(systemMenuHandle, NativeMethods.SC_MAXIMIZE, NativeMethods.MF_BYCOMMAND);

            if (!EnableMinimize)
                NativeMethods.DeleteMenu(systemMenuHandle, NativeMethods.SC_MINIMIZE, NativeMethods.MF_BYCOMMAND);

            if (!EnableRestore)
                NativeMethods.DeleteMenu(systemMenuHandle, NativeMethods.SC_RESTORE, NativeMethods.MF_BYCOMMAND);

            // Attach our WndProc handler to this Window
            var source = HwndSource.FromHwnd(wih.Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        protected void ResetCursor(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                var element = e.OriginalSource as FrameworkElement;

                //Hack - only reset cursors if the original source isn't a drag handle
                if (element != null && !element.Name.Contains("DragHandle"))
                    this.Cursor = Cursors.Arrow;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Check if a System Command has been executed for our custom system menu items
            if (msg == NativeMethods.WM_SYSCOMMAND)
            {
                // Execute the appropriate code for the System Menu item that was clicked
                switch (wParam.ToInt32())
                {
                    case _SettingsSysMenuID:
                        MessageBox.Show("\"Settings\" was clicked");
                        handled = true;
                        break;
                    case _AboutSysMenuID:
                        MessageBox.Show("\"About\" was clicked");
                        handled = true;
                        break;
                }
            }

            //Fix Maximizing a WindowStyle=None window does not obey screen docked appbar/toolbar reservations including the start menu.
            if (msg == 0x0024)
            {
                WmGetMinMaxInfo(hwnd, lParam);
                handled = false;
            }

            return IntPtr.Zero;
        }

        #region Dependency properties

        public bool ShowWindowTitle
        {
            get { return (bool)GetValue(ShowWindowTitleProperty); }
            set { SetValue(ShowWindowTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowWindowTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowWindowTitleProperty =
            DependencyProperty.Register("ShowWindowTitle", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));

        public bool ShowWindowIcon
        {
            get { return (bool)GetValue(ShowWindowIconProperty); }
            set { SetValue(ShowWindowIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowWindowIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowWindowIconProperty =
            DependencyProperty.Register("ShowWindowIcon", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));

        

        public bool EnableMinimize
        {
            get { return (bool)GetValue(EnableMinimizeProperty); }
            set { SetValue(EnableMinimizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableMinimize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableMinimizeProperty =
            DependencyProperty.Register("EnableMinimize", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));

        public bool EnableMaximize
        {
            get { return (bool)GetValue(EnableMaximizeProperty); }
            set { SetValue(EnableMaximizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableMaximize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableMaximizeProperty =
            DependencyProperty.Register("EnableMaximize", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));



        public bool EnableRestore
        {
            get { return (bool)GetValue(EnableRestoreProperty); }
            set { SetValue(EnableRestoreProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableRestore.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableRestoreProperty =
            DependencyProperty.Register("EnableRestore", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));
        #endregion

        #region Resize Window Helpers
        private Dictionary<ResizeDirection, Cursor> cursors = new Dictionary<ResizeDirection, Cursor> 
        {
            {ResizeDirection.Top, Cursors.SizeNS},
            {ResizeDirection.Bottom, Cursors.SizeNS},
            {ResizeDirection.Left, Cursors.SizeWE},
            {ResizeDirection.Right, Cursors.SizeWE},
            {ResizeDirection.TopLeft, Cursors.SizeNWSE},
            {ResizeDirection.BottomRight, Cursors.SizeNWSE},
            {ResizeDirection.TopRight, Cursors.SizeNESW},
            {ResizeDirection.BottomLeft, Cursors.SizeNESW} 
        };

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        protected void ResizeIfPressed(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            var direction = GetDirectionFromName(element.Name);

            if (MinHeight > 0 &&
                MaxHeight > 0 &&
                MaxHeight == MinHeight &&
                (direction == ResizeDirection.Bottom ||
                direction == ResizeDirection.BottomLeft ||
                direction == ResizeDirection.BottomRight ||
                direction == ResizeDirection.TopLeft ||
                direction == ResizeDirection.TopRight ||
                direction == ResizeDirection.Top))
                return;

            if (MinWidth > 0 &&
                MaxWidth > 0 &&
                MaxWidth == MinWidth &&
               (direction == ResizeDirection.Left ||
                direction == ResizeDirection.BottomLeft ||
                direction == ResizeDirection.BottomRight ||
                direction == ResizeDirection.TopLeft ||
                direction == ResizeDirection.TopRight ||
               direction == ResizeDirection.Right))
                return;

            this.Cursor = cursors[direction];

            if (e.LeftButton == MouseButtonState.Pressed)
                ResizeWindow(direction);
        }

        private static ResizeDirection GetDirectionFromName(string name)
        {
            //Hack - Assumes the drag handles are all named *DragHandle
            var enumName = name.Replace("DragHandle", "");
            return (ResizeDirection)Enum.Parse(typeof(ResizeDirection), enumName);
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            var wih = new WindowInteropHelper(this);
            NativeMethods.SendMessage(wih.Handle, NativeMethods.WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        #endregion

        #region Custom Window Title Bar Event Handlers

        public static readonly RoutedCommand onMinimizeCommand = new RoutedCommand();
        public static readonly RoutedCommand onRestoreCommand = new RoutedCommand();
        public static readonly RoutedCommand onMaximizeCommand = new RoutedCommand();
        public static readonly RoutedCommand onCloseCommand = new RoutedCommand();

        protected void onMinimizeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (EnableMinimize)
                SystemCommands.MinimizeWindow(this);
        }

        protected void onRestoreCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (EnableRestore)
                SystemCommands.RestoreWindow(this);

        }

        protected void onMaximizeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (EnableMaximize)
                SystemCommands.MaximizeWindow(this);
        }

        protected void onCloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
        
        protected void TitleBarIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                SystemCommands.CloseWindow(this);
            else
            {
                SystemCommands.ShowSystemMenu(this, this.PointToScreen(e.GetPosition(this)));
            }
        }

        protected void TitleBar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            SystemCommands.ShowSystemMenu(this, this.PointToScreen(e.GetPosition(this)));
        }

        protected void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState == System.Windows.WindowState.Maximized)
                {
                    if (EnableRestore)
                        SystemCommands.RestoreWindow(this);
                }
                else
                {
                    if (EnableMaximize)
                        SystemCommands.MaximizeWindow(this);
                }
            }
            else
            {
                DragWindow(sender, e);
            }
        }

        protected void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
            {
                WindowState = System.Windows.WindowState.Normal;

                //double pct = PointToScreen(e.GetPosition(this)).X / System.Windows.SystemParameters.PrimaryScreenWidth;
                Top = 0;
                var mousePoint = this.PointToScreen(Mouse.GetPosition(this));
                Debug.WriteLine(mousePoint.X);
                var p = RealPixelsToWpf(this, mousePoint);

                Left = p.X;
            }

            DragMove();
        }

        static Point RealPixelsToWpf(Window w, Point p)
        {
            var t = PresentationSource.FromVisual(w).CompositionTarget.TransformFromDevice;
            return t.Transform(p);
        }
        #endregion

        #region Fix Maximizing a WindowStyle=None window does not obey screen docked appbar/toolbar reservations including the start menu.

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            var mmi = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            var MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {
                var monitorInfo = new NativeMethods.MONITORINFO();
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }
        
        #endregion
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                var val = (bool)value;
                if (val)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
