using System;
using log4net;
using SpotifyNowPlaying.Config;

namespace SpotifyNowPlaying
{
    public class SettingsHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SettingsHelper));

        private const string SETTINGS_FILE_NAME = @"user.config";

        private static readonly object syncLock = new();
        private static SettingsHelper _instance;
        
        protected SettingsHelper()
        {
            Init();
        }

        public static SettingsHelper Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (syncLock)
                {
                    _instance = new SettingsHelper();
                }
                return _instance;
            }
        }

        public UserSettings CurrentSettings { get; set; }

        public static void Save(UserSettings settings)
        {
            IsolatedStorageManager.SaveFile(SETTINGS_FILE_NAME, settings);

            Instance.CurrentSettings = settings;
        }

        private void Init()
        {
            try
            {
                var fileExists = IsolatedStorageManager.FileExists(SETTINGS_FILE_NAME);
                if (!fileExists)
                {
                    CurrentSettings = new UserSettings();
                    return;
                }

                CurrentSettings = IsolatedStorageManager.ReadFile<UserSettings>(SETTINGS_FILE_NAME);
            }
            catch (Exception e)
            {
                var message = $"An error occurred initializing the user settings. {e.Message}";
                log.Error(message, e);
                CurrentSettings = new UserSettings();
            }
        }

    }
}
