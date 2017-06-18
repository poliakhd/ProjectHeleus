namespace ProjectHeleus.MangaApp.Helpers.Converters
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
    using WinRTMultibinding.Foundation.Interfaces;

    public class ImageToScrollVisibilityConverter : DependencyObject, IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //if (int.TryParse(value.ToString(), out int height))
            //{
                
            //}

            //return true;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}