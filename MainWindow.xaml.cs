using System;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices; // Added for Taskbar/Fullscreen logic
using System.Windows.Interop;         // Added for WindowInteropHelper
using System.Net.NetworkInformation;

namespace GTVApps
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (!IsSystemCompatible())
            {
                IncompatibleWindow errorWin = new IncompatibleWindow();
                errorWin.Show();
                this.Close();
                return;
            }

            InitializeComponent();
            SafeInitialize();
        }

        private void SafeInitialize()
        {
            //if (!IsInternetAvailable())
            //{
            //    NoInternetWindow noNet = new NoInternetWindow();
            //    noNet.ShowDialog();
            //    this.Close();
            //}

            //InitializeWebView();
            try
            {
                if (!IsInternetAvailable())
                {
                    NoInternetWindow noNet = new NoInternetWindow();
                    noNet.ShowDialog();
                    this.Close();
                }

                InitializeWebView();
            }
            catch (Exception)
            {
                // This catches the error if the DLLs or Runtime are totally missing
                new MissingRuntimeWindow().Show();
                this.Close();
            }
        }

        private bool IsInternetAvailable()
        {
            try
            {
                using (Ping myPing = new Ping())
                {
                    string host = "8.8.8.8";
                    byte[] buffer = new byte[32];
                    int timeout = 2000; // 2 seconds
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);

                    return (reply != null && reply.Status == IPStatus.Success);
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsSystemCompatible()
        {
            // Windows 10 and 11 both report Major version 10.
            // Windows 7 is 6.1, Windows 8.1 is 6.3.
            return Environment.OSVersion.Version.Major >= 10;
        }

        async void InitializeWebView()
        {
            try
            {
                string userDataFolder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "GTVhub"
                );

                var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await GTVPlayer.EnsureCoreWebView2Async(environment);

                GTVPlayer.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0";
                GTVPlayer.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
                GTVPlayer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                GTVPlayer.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;
                GTVPlayer.NavigationCompleted += GTVPlayer_NavigationCompleted;
                GTVPlayer.Source=new Uri("https://gtvhub.pages.dev/");
            }
            catch (Exception e)
            {
                MissingRuntimeWindow runtimeWin = new MissingRuntimeWindow();
                runtimeWin.Show();
                MessageBox.Show(e.Message);
                this.Close();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ApplyDarkTheme();
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object sender, object e)
        {
            if (GTVPlayer.CoreWebView2.ContainsFullScreenElement)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
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

        private void GTVPlayer_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // Hide the loading text once the site is visible
                LoadingText.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoadingText.Text = "Connection Error. Please check your internet.";
            }
        }

        private void CoreWebView2_PermissionRequested(object sender, CoreWebView2PermissionRequestedEventArgs e)
        {
            // Check if the website is asking for Geolocation
            if (e.PermissionKind == CoreWebView2PermissionKind.Geolocation)
            {
                // This line automatically allows the site to see the location
                e.State = CoreWebView2PermissionState.Allow;

                // Mark as handled so the default browser prompt doesn't appear
                e.Handled = true;
            }
        }
    }
}