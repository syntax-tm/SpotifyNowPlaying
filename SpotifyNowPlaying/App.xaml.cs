using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows;
using log4net;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyNowPlaying.Output;
using SpotifyNowPlaying.Views;

namespace SpotifyNowPlaying;

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
            
            // TODO: create an argument to dump this to a file instead
            var allPlaylists = await SpotifyClientHelper.Client.PaginateAll(await SpotifyClientHelper.Client.Playlists.CurrentUsers());
            var json = JsonConvert.SerializeObject(allPlaylists, Formatting.Indented);

            Clipboard.SetText(json);
        
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
    
    [SupportedOSPlatform(@"windows")]
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
