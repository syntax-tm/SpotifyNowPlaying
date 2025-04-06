using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
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
            
            var client = SpotifyClientHelper.Client;

            // TODO: create an argument to dump this to a file instead
            var allPlaylists = await SpotifyClientHelper.Client.PaginateAll(await SpotifyClientHelper.Client.Playlists.CurrentUsers());

            var playlists = new Dictionary<string, object>();

            foreach (var playlist in allPlaylists)
            {
                var image = await SpotifyClientHelper.Client.Playlists.GetCovers(playlist.Id);
                
                var firstImage = image.OrderByDescending(i => i?.Height ?? 0 * i?.Width ?? 0).FirstOrDefault();

                playlists[playlist.Id] = new
                {
                    id = playlist.Id,
                    name = playlist.Name,
                    description = playlist.Description,
                    cover_photo = firstImage.Url,
                    href = playlist.Href,
                    is_collaborative = playlist.Collaborative,
                    is_private = !playlist.Public,
                    image = firstImage
                };
            }
            var json = JsonConvert.SerializeObject(playlists.Values, Formatting.Indented);

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var downloadsPath = Path.Join(userProfile, "Downloads");

            File.WriteAllText(Path.Join(downloadsPath, @"playlists.json"), json, new UTF8Encoding(false));

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
