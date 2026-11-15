using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;

namespace GTVApps
{
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "GTVhub-Unique-Instance-Key";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                AlreadyRunningWindow alreadyRunning = new AlreadyRunningWindow();
                alreadyRunning.ShowDialog();
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex != null)
            {
                // Only release if we actually own it
                try { _mutex.ReleaseMutex(); } catch { }
                _mutex.Dispose();
            }
            base.OnExit(e);
        }
    }
}