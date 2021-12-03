using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using log4net;
using Newtonsoft.Json;
using SpotifyNowPlaying.Common;
using SpotifyNowPlaying.Config;

namespace SpotifyNowPlaying.ViewModels
{
    public class SettingsViewModel
    {
        private const string SETTINGS_FILE_NAME = "settings.json";

        private readonly ILog log = LogManager.GetLogger(typeof(SettingsViewModel));
        
        protected virtual ICurrentWindowService CurrentWindow { get { return null; } }

        private UserSettings _backupSettings;

        public virtual bool CanSave { get; set; }
        public virtual bool CanReset { get; set; }
        public virtual bool CanCancel { get; set; }
        public virtual UserSettings Settings { get; set; }

        protected SettingsViewModel()
        {

        }

        public static SettingsViewModel Create()
        {
            return ViewModelSource.Create(() => new SettingsViewModel());
        }

        public void Save()
        {
            IsolatedStorageManager.SaveFile(SETTINGS_FILE_NAME, Settings);

            _backupSettings = Settings;
        }

        public void Load()
        {
            var fileExists = IsolatedStorageManager.FileExists(SETTINGS_FILE_NAME);
            if (!fileExists)
            {
                Default();
                return;
            }
            
            Settings = IsolatedStorageManager.ReadFile<UserSettings>(SETTINGS_FILE_NAME);
        }

        public void Default()
        {

        }

        public void Import()
        {

        }

        public void Export()
        {

        }

        public void Exit()
        {

        }

    }
}
