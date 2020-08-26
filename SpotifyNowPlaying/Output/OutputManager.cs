using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using log4net;
using SpotifyNowPlaying.Config;
using SpotifyNowPlaying.Extensions;

namespace SpotifyNowPlaying.Output
{
    public static class OutputManager
    {

        private static readonly ILog log = LogManager.GetLogger(nameof(OutputManager));

        private static IDictionary<string, string> _staticTokens;

        public static void Clear()
        {
            var outputs = ConfigHelper.CurrentConfig.Outputs;
            var activeOutputs = ValidateOutputs(outputs);

            if (!activeOutputs.Any()) return;

            foreach (var output in activeOutputs)
            {
                var outputPath = Path.Combine(ConfigHelper.CurrentConfig.OutputFolder, output.FileName);

                File.WriteAllText(outputPath, string.Empty, Encoding.UTF8);
            }
        }

        public static void Process()
        {
            var outputs = ConfigHelper.CurrentConfig.Outputs;
            var activeOutputs = ValidateOutputs(outputs);

            if (!activeOutputs.Any()) return;

            foreach (var output in activeOutputs)
            {
                var outputPath = Path.Combine(ConfigHelper.CurrentConfig.OutputFolder, output.FileName);
                var formattedText = Format(output.FileFormat);

                File.WriteAllText(outputPath, formattedText, Encoding.UTF8);
            }
        }

        private static IDictionary<string, string> GetStaticTokens()
        {
            if (_staticTokens != null) return _staticTokens;

            _staticTokens = new Dictionary<string, string>
                    {
                        { "{newline}", Environment.NewLine },
                        { "{nl}", Environment.NewLine },
                        { "{tab}", "\t" },
                        { "{t}", "\t" },
                        { "{space}", " " },
                        { "{s}", " " },
                        { "{cr}", "\r" },
                        { "{lf}", "\n" },
                        { "{crlf}", "\r\n" },
                        { "{quote}", "'" },
                        { "{q}", "'" },
                        { "{dquote}", "\"" },
                        { "{dq}", "\"" },
                        { "{unicode}", "\0" },
                        { "{u}", "\0" },
                        { "{alert}", "\a" },
                        { "{a}", "\a" },
                        { "{form}", "\f" },
                        { "{f}", "\f" },
                        { "{vtab}", "\v" },
                        { "{bs}", "\\" }
                    };

            return _staticTokens;
        }

        private static IDictionary<string, string> GetDateTokens()
        {
            return new Dictionary<string, string>
                {
                    {"{longdate}", DateTime.Now.ToLongDateString()},
                    {"{longtime}", DateTime.Now.ToLongTimeString()},
                    {"{date}", DateTime.Now.ToShortDateString()},
                    {"{time}", DateTime.Now.ToShortTimeString()},
                    {"{dayofweek}", DateTime.Now.DayOfWeek.ToString()},
                    {"{hour}", DateTime.Now.Hour.ToString()},
                    {"{minutes}", DateTime.Now.Minute.ToString()},
                    {"{month}", DateTime.Now.Month.ToString()},
                    {"{year}", DateTime.Now.Year.ToString()}
                };
        }

        private static IDictionary<string, string> GetSpotifyTokens()
        {
            var currentlyPlaying = SpotifyHelper.CurrentlyPlaying;
            var previouslyPlaying = SpotifyHelper.PreviouslyPlaying;
            var spotifyTokens = new Dictionary<string, string>
                {
                    { "{artist}", currentlyPlaying?.Artist },
                    { "{song}", currentlyPlaying?.Song },
                    { "{previous_artist}", previouslyPlaying?.Artist },
                    { "{previous_song}", previouslyPlaying?.Artist }
                };

            return spotifyTokens;
        }

        private static IDictionary<string, string> GetTokens()
        {
            var staticTokens = GetStaticTokens();
            var dateTokens = GetDateTokens();
            var spotifyTokens = GetSpotifyTokens();

            return new Dictionary<string, string>
                     {
                         staticTokens,
                         dateTokens,
                         spotifyTokens
                     };
        }

        private static string Format(string stringFormat)
        {
            var tokens = GetTokens();
            var output = stringFormat;

            foreach (var token in tokens.Keys)
            {
                if (!output.Contains(token)) continue;
                var tokenValue = tokens[token] ?? string.Empty;
                output = output.Replace(token, tokenValue);
            }

            return output;
        }

        private static IList<OutputConfig> ValidateOutputs([NotNull] IList<OutputConfig> outputs)
        {
            if (!outputs.Any())
            {
                log.Warn("Current configuration contains no outputs.");
                return null;
            }

            var activeOutputs = outputs.Where(o => o.Enabled).ToList();
            if (!activeOutputs.Any())
            {
                log.Warn("All outputs are currently disabled.");
                return null;
            }

            return activeOutputs;
        }

    }
}
