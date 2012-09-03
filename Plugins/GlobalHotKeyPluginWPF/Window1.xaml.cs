using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GlobalHotKeyPlugin
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }
        // Note that powers of 2 are used; each value has only a single bit set
        private const int MOD_ALT = 0x1;     // If bit 0 is set, Alt is pressed
        private const int MOD_CONTROL = 0x2; // If bit 1 is set, Ctrl is pressed
        private const int MOD_SHIFT = 0x4;   // If bit 2 is set, Shift is pressed 
        private const int MOD_WIN = 0x8;     // If bit 3 is set, Win is pressed

        // If we wanted to represent a combination of keys:
        private const int altAndControl = MOD_ALT | MOD_CONTROL; // == 3
        private const int controlAndShift = MOD_CONTROL | MOD_SHIFT; // == 6


        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint key = (uint)Key.D1;

            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, (uint)altAndControl, key))
            {
                Console.WriteLine("FAILED");
            }
            
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            Console.WriteLine("HOTKEY");
        }
    }
}
