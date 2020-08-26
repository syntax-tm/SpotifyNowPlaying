using System;
using System.Runtime.InteropServices;
using System.Threading;
using log4net;
using SpotifyNowPlaying.Config;
using SpotifyNowPlaying.Output;

namespace SpotifyNowPlaying
{
    internal class Program
    {

        private const string TITLE_BASE = @"Spotify Currently Playing";

        protected static readonly ILog log = LogManager.GetLogger(nameof(Program));

        private static void Main()
        {
            // setup the handler for exit messages
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);
            new Program().Start();

            // keep program running until an exit request
            while (!_exitSystem)
            {
                Thread.Sleep(500);
            }
        }

        public void Start()
        {
            // configure the logging
            LoggingHelper.Configure();

            // load the config if it exists or create the default
            ConfigHelper.Load();
            
            // clear all of the outputs on startup and set default title
            OutputManager.Clear();
            Console.Title = TITLE_BASE;

            while (true)
            {
                try
                {
                    Console.Clear();

                    var isChanged = SpotifyHelper.GetCurrentlyPlaying();

                    // isChanged being null either spotify is not running or playback never
                    // started, so add an additional delay before continuing
                    if (isChanged == null)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }
                    // no changes
                    else if ((bool)!isChanged)
                    {
                        continue;
                    }

                    Console.Title = $"{TITLE_BASE} | {SpotifyHelper.CurrentTitle}";

                    OutputManager.Process();
                }
                
                catch (Exception e)
                {
                    var message = $"An error occurred attempting to load the currrently playing info from Spotify. {e.Message}";
                    log.Error(message, e);
                }
                finally
                {
                    Thread.Sleep(ConfigHelper.CurrentConfig.Delay);
                }
            }
        }

#region Windows CtrlType Event Handling (App Exit)

        // i hate regions, but wanted this code to be grouped together since
        // it's only applicable to application exit

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool _exitSystem;
        private static EventHandler _handler;
        private delegate bool EventHandler(CtrlType sig);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private static bool Handler(CtrlType sig)
        {
            // delete any created files before exiting since we're no longer tracking
            // the currently playing song
            OutputManager.Clear();

            _exitSystem = true;
            Environment.Exit(-1);

            return true;
        }

#endregion

        
    }
}
