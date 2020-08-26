using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using Newtonsoft.Json;

namespace SpotifyNowPlaying.Config
{
    public static class ConfigHelper
    {

        private static readonly ILog log = LogManager.GetLogger(nameof(ConfigHelper));

        private const string _appConfigFile = "config.json";

        public static NowPlayingConfig CurrentConfig { get; set; }
        
        public static void Load()
        {
            try
            {
                var exists = File.Exists(_appConfigFile);
                if (!exists)
                {
                    log.Debug($"Config file '{_appConfigFile}' not found. Creating default...");
                    SaveDefault();
                }

                var configJson = File.ReadAllText(_appConfigFile);
                var config = JsonConvert.DeserializeObject<NowPlayingConfig>(configJson);

                CurrentConfig = config;

                // make sure the output path is valid and create the directory if it does not exist
                DirectoryHelper.EnsureDirectoryExists(CurrentConfig.OutputFolder);
            }
            catch (Exception e)
            {
                var message = $"An error occurred atttempting to create the config file. {e.Message}";

                log.Error(message, e);

                throw new ConfigurationErrorsException(message, e);
            }
        }

        public static void Save()
        {
            if (CurrentConfig == null) throw new ArgumentNullException(nameof(CurrentConfig));

            var configJson = JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented);

            File.WriteAllText(_appConfigFile, configJson, Encoding.UTF8);
        }

        public static void SaveDefault()
        {
            var exists = File.Exists(_appConfigFile);
            if (exists)
            {
                log.Debug($"Default config not created because file '{_appConfigFile}' already exists.");
                return;
            }

            var defaultOutputs = new List<OutputConfig>
                    {
                        new OutputConfig(@"Artist Output", @"artist.txt", @"{artist}"),
                        new OutputConfig(@"Song Output", @"song.txt", @"{song}"),
                        new OutputConfig(@"Current Song Output", @"currentSong.txt", @"{song}{newline}by {artist}")
                    };

            var defaultConfig = new NowPlayingConfig(defaultOutputs)
                    {
                        OutputFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                    };

            CurrentConfig = defaultConfig;

            Save();
        }

    }
}
