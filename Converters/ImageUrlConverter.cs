using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GdzApp.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        private static BitmapImage _placeholder;

        static ImageUrlConverter()
        {
            _placeholder = new BitmapImage();
            _placeholder.BeginInit();
            _placeholder.UriSource = new Uri("https://via.placeholder.com/400x300/2EA8E6/FFFFFF?text=No+Image");
            _placeholder.EndInit();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var s = value as string;
                if (string.IsNullOrEmpty(s)) return _placeholder;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(s, UriKind.Absolute);

                // УБИРАЕМ принудительное изменение размера
                // bmp.DecodePixelWidth = 140; // УДАЛИТЬ ЭТУ СТРОКУ!

                // Вместо этого настраиваем кэширование
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                bmp.EndInit();
                return bmp;
            }
            catch
            {
                return _placeholder;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}