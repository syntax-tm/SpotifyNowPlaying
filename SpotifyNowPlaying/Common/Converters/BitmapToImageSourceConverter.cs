using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpotifyNowPlaying.Common.Converters
{
    [ValueConversion(typeof(Bitmap), typeof(ImageSource))]
    public class BitmapToImageSourceConverter : IValueConverter
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Image bmp))
                return null;
            
            using var ms = new MemoryStream();

            bmp.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();

            return bitmapImage;

            //var cbmp = (Bitmap) bmp.Clone();
            //
            //var handle = cbmp.GetHbitmap();
            //try
            //{
            //    return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //}
            //finally { DeleteObject(handle); }      
        }
 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BitmapToImageSourceConverterExtension : MarkupExtension
    {
        public IValueConverter ItemConverter { get; set; }

        public BitmapToImageSourceConverterExtension()
        {

        }

        public BitmapToImageSourceConverterExtension(IValueConverter itemConverter)
        {
            ItemConverter = itemConverter;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new BitmapToImageSourceConverter();
        }
    }
}
