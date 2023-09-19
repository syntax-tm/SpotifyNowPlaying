using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace SpotifyNowPlaying
{
    public static class LoggingHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoggingHelper));

        public static void Configure()
        {
            var hierarchy = (Hierarchy) LogManager.GetRepository();
            var logger = hierarchy.LoggerFactory.CreateLogger(hierarchy, $"{nameof(SpotifyNowPlaying)}Repository");
            logger.Hierarchy = hierarchy;

            var debugOut = new FormattedDebugAppender
            {
                Layout = GetLayout()
            };
            
            debugOut.ActivateOptions();
            
            hierarchy.Root.AddAppender(debugOut);

            var level = Debugger.IsAttached
                ? Level.Debug
                : Level.Info;

            hierarchy.Threshold = level;
            logger.Level = level;

            BasicConfigurator.Configure(hierarchy);

            log.Debug("Logging configured.");
        }

        private static ILayout GetLayout()
        {
            var layout = new PatternLayout("%message%newline");
            layout.ActivateOptions();
            return layout;
        }
        
        [SuppressMessage("ReSharper", "ConditionalInvocation")]
        private class FormattedDebugAppender : DebugAppender
        {
            private static bool _autoFlushSet;

            protected override void Append(LoggingEvent loggingEvent)
            {
                var message = RenderLoggingEvent(loggingEvent);
                if (string.IsNullOrWhiteSpace(message)) return;
                
                Debug.Write(message);

                if (!ImmediateFlush) return;
                if (!_autoFlushSet)
                {
                    Debug.AutoFlush = true;
                    _autoFlushSet = true;
                }
                
                Debug.Flush();
            }
        }
    }
}
