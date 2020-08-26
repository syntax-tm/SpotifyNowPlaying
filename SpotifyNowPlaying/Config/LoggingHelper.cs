using System.Diagnostics;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using static log4net.Appender.ColoredConsoleAppender;

namespace SpotifyNowPlaying.Config
{
    public static class LoggingHelper
    {
        public static void Configure()
        {
            var hierarchy = (Hierarchy) LogManager.GetRepository();

            var consoleOut = new ColoredConsoleAppender();            

            SetLayout(consoleOut);
            SetColors(consoleOut);
            SetFilter(consoleOut);

            consoleOut.ActivateOptions();

            BasicConfigurator.Configure(hierarchy, consoleOut);
        }

        private static void SetLayout(ColoredConsoleAppender appender)
        {
            appender.Layout = new PatternLayout("%message%newline");
        }

        private static void SetFilter(ColoredConsoleAppender appender)
        {
            var filter = new LevelMatchFilter();

            filter.AcceptOnMatch = true;
            filter.LevelToMatch = Debugger.IsAttached 
                ? Level.Debug
                : Level.Info;

            appender.AddFilter(filter);
        }

        private static void SetColors(ColoredConsoleAppender appender)
        {
            if (Debugger.IsAttached)
            {
                var debug = new LevelColors
                {
                    Level = Level.Debug,
                    ForeColor = Colors.Blue | Colors.HighIntensity
                };

                appender.AddMapping(debug);
            }

            var info = new LevelColors
            {
                Level = Level.Info,
                ForeColor = Colors.White | Colors.HighIntensity
            };

            var warn = new LevelColors
            {
                Level = Level.Warn,
                ForeColor = Colors.Yellow | Colors.HighIntensity
            };

            var err = new LevelColors
            {
                Level = Level.Error,
                ForeColor = Colors.Red | Colors.HighIntensity
            };

            var fatal = new LevelColors
            {
                Level = Level.Fatal,
                ForeColor = Colors.White | Colors.HighIntensity,
                BackColor = Colors.Red | Colors.HighIntensity
            };

            appender.AddMapping(info);
            appender.AddMapping(warn);
            appender.AddMapping(err);
            appender.AddMapping(fatal);
        }

    }
}
