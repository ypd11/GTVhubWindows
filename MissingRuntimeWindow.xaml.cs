using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GTVApps
{
    /// <summary>
    /// Interaction logic for MissingRuntimeWindow.xaml
    /// </summary>
    public partial class MissingRuntimeWindow : Window
    {
        public MissingRuntimeWindow()
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
            // This uses the Windows 11/10 DWM API to color the title bar
            var helper = new WindowInteropHelper(this);
            int attribute = 20; // DWMWA_USE_IMMERSIVE_DARK_MODE
            int useImmersiveDarkMode = 1; // 1 = True
            DwmSetWindowAttribute(helper.Handle, attribute, ref useImmersiveDarkMode, sizeof(int));
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            // Official Microsoft download link for the Evergreen Bootstrapper
            string url = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
