using System;
using System.Windows;
using log4net;
using SpotifyNowPlaying.Common;
using SpotifyNowPlaying.Output;
using SpotifyNowPlaying.Views;

namespace SpotifyNowPlaying
{
    public partial class App
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        
        public App()
        {
            // configure the logging
            LoggingHelper.Configure();

            // configure the isolated storage
            IsolatedStorageManager.Init();
        }
        
        private async void App_OnStartup(object sender, StartupEventArgs args)
        {
            try
            {
                await SpotifyClientHelper.Init();
            
                MainWindow = new MainWindow();
                MainWindow.Closed += WindowOnClosed;

                ShutdownMode = ShutdownMode.OnExplicitShutdown;

                MainWindow.Show();
            }
            catch (Exception e)
            {
                var message = $"An error occurred during application startup. {e.Message}";
                log.Fatal(message, e);
                Environment.Exit(AppExitCode.UnhandledException);
            }
        }

        private void WindowOnClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void App_OnExit(object sender, ExitEventArgs args)
        {
            try
            {
                OutputManager.Cleanup();
            }
            catch (Exception e)
            {
                log.Fatal($"An error occurred during application exit. {e.Message}", e);

                Environment.Exit(AppExitCode.UnhandledException);
            }
        }
    }
}
