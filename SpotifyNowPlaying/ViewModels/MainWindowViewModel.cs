using System;
using System.ComponentModel;
using System.Threading;
using DevExpress.Mvvm.POCO;
using log4net;
using SpotifyAPI.Web;
using SpotifyNowPlaying.Output;

namespace SpotifyNowPlaying.ViewModels;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class MainWindowViewModel
{
    private readonly ILog log = LogManager.GetLogger(typeof(MainWindowViewModel));

    private readonly BackgroundWorker _backgroundWorker;

    public virtual int Interval { get; set; }
    public virtual int StoppedInterval { get; set; }
    public virtual bool IsRunning { get; set; }
    public virtual SpotifyPlaybackState State { get; protected set; }

    protected MainWindowViewModel()
    {
        Interval = 2000;
        StoppedInterval = 5000;

        _backgroundWorker = new BackgroundWorker
        {
            WorkerSupportsCancellation = true
        };
        
        _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
        _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;

        _backgroundWorker.RunWorkerAsync();
    }

    public static MainWindowViewModel Create()
    {
        return ViewModelSource.Create(() => new MainWindowViewModel());
    }
    
    private async void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
    {
        IsRunning = true;

        while (!_backgroundWorker.CancellationPending)
        {
            try
            {
                var request = new PlayerCurrentlyPlayingRequest();
                var currentlyPlaying = await SpotifyClientHelper.Client.Player.GetCurrentlyPlaying(request);

                var queue = await SpotifyClientHelper.Client.Player.GetQueue();
                

                var state = await SpotifyPlaybackState.Create(currentlyPlaying);

                if (State == null || !State.Equals(state))
                {
                    State = state;
                    OutputManager.Process(State);
                }

                var sleepTime = State?.IsStopped ?? true
                    ? StoppedInterval
                    : Interval;
                
                Thread.Sleep(TimeSpan.FromMilliseconds(sleepTime));
            }
            catch (APIException ae)
            {
                var exType = ae.GetType().Name;
                log.Error($"An {exType} error occurred getting the current playback. {ae.Message}", ae);
                e.Cancel = true;
                _backgroundWorker.CancelAsync();
            }
            catch (Exception ex)
            {
                log.Error($"An error occurred getting the current playback. {ex.Message}", ex);
                e.Cancel = true;
                _backgroundWorker.CancelAsync();
            }
        }
    }
    
    private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        IsRunning = false;
    }

}
