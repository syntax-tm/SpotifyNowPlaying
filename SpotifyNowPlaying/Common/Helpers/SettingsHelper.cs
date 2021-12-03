using System.IO;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using SpotifyNowPlaying.ViewModels;

namespace SpotifyNowPlaying.Common
{
    public static class SettingsHelper
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(SettingsHelper));

        private const string DEFAULT_SETTINGS_FILE_NAME = @"user.config";

        private static SettingsViewModel _settings;
        public static SettingsViewModel Settings => _settings;

        public static void Init()
        {
            var assemblyLocation = Assembly.GetEntryAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(assemblyLocation);

            var configPath = Path.Combine(assemblyDir, DEFAULT_SETTINGS_FILE_NAME);

            var configExists = File.Exists(configPath);
            if (configExists)
            {
                var configJson = File.ReadAllText(configPath);
                
                _settings = SettingsViewModel.Create();

                JsonConvert.PopulateObject(configJson, _settings);
            }

        }

    }
}
