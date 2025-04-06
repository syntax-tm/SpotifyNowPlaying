using System;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SpotifyNowPlaying.Converters;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
[ValueConversion(typeof(string), typeof(FontFamily))]
public class StringToFontFamilyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string fontName)
        {
            throw new InvalidCastException($"{nameof(value)} must by type {typeof(string)}.");
        }

        var fonts = new InstalledFontCollection();
        var font = fonts.Families.FirstOrDefault(f => f.Name.EqualsIgnoreCase(fontName));

        return font;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not FontFamily fontFamily)
        {
            throw new InvalidCastException($"{nameof(value)} must by type {typeof(FontFamily)}.");
        }

        return fontFamily.Source;
    }
}

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class StringToFontFamilyConverterExtension : MarkupExtension
{
    public IValueConverter ItemConverter { get; set; }

    public StringToFontFamilyConverterExtension()
    {

    }

    public StringToFontFamilyConverterExtension(IValueConverter itemConverter)
    {
        ItemConverter = itemConverter;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new StringToFontFamilyConverter();
    }
}
