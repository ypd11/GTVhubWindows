using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace GTVApps
{
    /// <summary>
    /// Interaction logic for NoInternetWindow.xaml
    /// </summary>
    public partial class NoInternetWindow : Window
    {
        public NoInternetWindow()
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


        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            if (IsInternetAvailable())
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close(); // Internet is back! Continue to MainWindow
            }
            else
            {
                ShakeWindow();
            }
        }

        private void ShakeWindow()
        {
            // If Left is NaN (common during startup), default to 0 or use current screen position
            double startLeft = double.IsNaN(this.Left) ? (SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2) : this.Left;

            DoubleAnimation shakeAnimation = new DoubleAnimation();
            shakeAnimation.From = startLeft;
            shakeAnimation.To = startLeft + 10;
            shakeAnimation.Duration = TimeSpan.FromMilliseconds(50);
            shakeAnimation.AutoReverse = true;
            shakeAnimation.RepeatBehavior = new RepeatBehavior(3);

            this.BeginAnimation(Window.LeftProperty, shakeAnimation);
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
