using System.Reflection;
using log4net;

namespace SpotifyNowPlaying.Output;

public abstract class FileOutput
{
    protected readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

    public string FileName { get; set; }
    public abstract OutputFileType FileType { get; }
    public bool IsEnabled { get; set; }
}
