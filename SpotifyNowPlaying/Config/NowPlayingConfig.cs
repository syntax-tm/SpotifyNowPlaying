using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyNowPlaying.Config
{
    public class NowPlayingConfig
    {
        [JsonProperty("delay")]
        public int Delay { get; set; } = 2000;

        [JsonProperty("outputFolder")]
        public string OutputFolder { get; set; }

        [JsonProperty("outputs")]
        public IList<OutputConfig> Outputs { get; set; }

        public NowPlayingConfig()
        {
            Outputs = new List<OutputConfig>();
        }

        public NowPlayingConfig(IList<OutputConfig> outputs)
        {
            Outputs = outputs;
        }
    }
}
