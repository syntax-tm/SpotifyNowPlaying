namespace SpotifyNowPlaying.Output;

public class TextFileOutput : FileOutput
{
    public string FileTemplate { get; set; }
    public override OutputFileType FileType => OutputFileType.Text;
}
