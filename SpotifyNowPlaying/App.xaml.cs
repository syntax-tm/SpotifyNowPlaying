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
        
        protected override async void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            try
            {
                // configure the logging
                LoggingHelper.Configure();

                // configure the isolated storage
                IsolatedStorageManager.Init();

                await SpotifyClientHelper.Init();
            
                MainWindow = new MainWindow();
                MainWindow.Closed += WindowOnClosed;
                
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
            Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs args)
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
