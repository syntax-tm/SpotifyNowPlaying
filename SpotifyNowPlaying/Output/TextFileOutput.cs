using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyNowPlaying.Output
{
    public class TextFileOutput : FileOutput
    {

        public string FileTemplate { get; set; }
        public override OutputFileType FileType => OutputFileType.Text;
        
    }
}
