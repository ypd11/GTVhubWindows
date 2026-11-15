using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GTVApps
{
    public partial class IncompatibleWindow : Window
    {
        public IncompatibleWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ApplyDarkTheme();
        }

        private void ApplyDarkTheme()
        {
            var helper = new WindowInteropHelper(this);
            int attribute = 20; // DWMWA_USE_IMMERSIVE_DARK_MODE
            int useImmersiveDarkMode = 1;
            DwmSetWindowAttribute(helper.Handle, attribute, ref useImmersiveDarkMode, sizeof(int));
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}