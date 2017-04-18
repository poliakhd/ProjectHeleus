using Windows.UI;
using Windows.UI.Xaml;
using Caliburn.Micro;

namespace ProjectHeleus.MangaApp.ViewModels
{
    using Core;
    using MangaLibrary.Core;

    public class SettingsPageViewModel
        : Screen
    {
        #region Private Members

        private readonly IEventAggregator _eventAggregator;
        private Settings _settings;

        #endregion

        public SettingsPageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _settings = Application.Current.Resources["Settings"] as Settings;

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
    }
}