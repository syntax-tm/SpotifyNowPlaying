using System;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using log4net;
using SpotifyNowPlaying.Config;

namespace SpotifyNowPlaying.ViewModels;

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
        Settings = SettingsHelper.Instance.CurrentSettings;
    }

    public static SettingsViewModel Create()
    {
        return ViewModelSource.Create(() => new SettingsViewModel());
    }

    public void Save()
    {
        try
        {
            SettingsHelper.Save(Settings);

            _backupSettings = Settings;
        }
        catch (Exception e)
        {
            var message = $"An error occurred attempting to save user settings. {e.Message}";
            log.Error(message, e);
        }
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
        var message = $"Are you sure you want to restore the default settings?";
        var response = MessageBox.Show(message, "Confirm Restore Default", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (response != MessageBoxResult.Yes) return;

        Settings = _backupSettings;
    }

    public void Import()
    {

    }

    public void Export()
    {

    }

    public void Exit()
    {
        CurrentWindow?.Close();
    }

}
