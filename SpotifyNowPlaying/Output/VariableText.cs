using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace SpotifyNowPlaying.Output;

public class VariableText
{
    private string _format;

    public string Token { get; init; }
    public string Format
    {
        get => _format;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(Format));

            _format = value;
            Formats = _format.Split(OutputFormatter.TOKEN_FORMAT_SEPARATOR);
        }
    }
    public IList<string> Formats { get; private set; }

    public string GetJsonPath()
    {
        var sb = new StringBuilder();

        if (!Token.StartsWith(@"$."))
        {
            sb.Append(@"$.");
        }

        sb.Append(Token);

        return sb.ToString();
    }

    public string GetFormattedValue([NotNull] string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

        if (!Formats.Any()) return value;

        var formatters = OutputFormatter.GetFormatters();
        foreach (var format in Formats)
        {
            var isValid = formatters.ContainsKey(format);
            if (isValid)
            {
                value = formatters[format](value);
            }
        }

        return value;
    }
}
