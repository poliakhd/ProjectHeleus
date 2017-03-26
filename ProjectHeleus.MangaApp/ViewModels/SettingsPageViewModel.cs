using Windows.UI;
using Caliburn.Micro;
using ProjectHeleus.MangaApp.Models.Messages;
using ColorHelper = Microsoft.Toolkit.Uwp.ColorHelper;

namespace ProjectHeleus.MangaApp.ViewModels
{
    public class SettingsPageViewModel
        : Screen
    {
        #region Private Members

        private readonly IEventAggregator _eventAggregator;
        private Color _color;

        #endregion

        public SettingsPageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Initialize();
            NotifyProperties();
        }

        private void Initialize()
        {
            _eventAggregator.Subscribe(this);
        }
        private void NotifyProperties()
        {
        }

        public bool IsDarkTheme { get; set; }

        public void ThemeChanged()
        {
            _eventAggregator.PublishOnUIThread(new UpdateAppTheme {Theme = IsDarkTheme ? AppTheme.Dark : AppTheme.Light});
        }
    }
}