using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SpotifyNowPlaying.Config
{
    public class OutputConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileFormat")]
        public string FileFormat { get; set; }


        public OutputConfig()
        {

        }

        public OutputConfig([NotNull] string fileName, [NotNull] string fileFormat, bool enabled = true)
        {
            FileName = fileName;
            FileFormat = fileFormat;
            Enabled = enabled;
        }

        public OutputConfig([NotNull] string name, [NotNull] string fileName, [NotNull] string fileFormat, bool enabled = true)
        {
            Name = name;
            FileName = fileName;
            FileFormat = fileFormat;
            Enabled = enabled;
        }
    }
}
