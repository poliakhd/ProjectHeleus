using Windows.UI;
using Caliburn.Micro;
using ColorHelper = Microsoft.Toolkit.Uwp.ColorHelper;

namespace ProjectHeleus.MangaApp.Core
{
    public enum AppTheme
    {
        Light,
        Dark
    }

    public class Settings : PropertyChangedBase
    {
        public string PaneBackgroundColor
        {
            get { return Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneBackgroundColor"] as string ?? ColorHelper.ToHex(Colors.Black); }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneBackgroundColor"] = value;
                NotifyOfPropertyChange();
            }
        }

        public string PaneForegroundColor
        {
            get { return Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneForegroundColor"] as string ?? ColorHelper.ToHex(Colors.Black); }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneForegroundColor"] = value;
                NotifyOfPropertyChange();
            }
        }
        public bool IsDarkThemeEnabled
        {
            get { return (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["IsDarkThemeEnabled"]; }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["IsDarkThemeEnabled"] = value;
                UpdateAppTheme(value ? AppTheme.Dark : AppTheme.Light);

                NotifyOfPropertyChange();
            }
        }

        public void UpdateAppTheme(AppTheme theme)
        {
            switch (theme)
            {
                case AppTheme.Light:
                    PaneBackgroundColor = ColorHelper.ToHex(Colors.White);
                    PaneForegroundColor = ColorHelper.ToHex(Colors.Black);
                    break;
                case AppTheme.Dark:
                    PaneBackgroundColor = ColorHelper.ToHex(Colors.Black);
                    PaneForegroundColor = ColorHelper.ToHex(Colors.White);
                    break;
            }
        }
    }
}