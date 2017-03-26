using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Caliburn.Micro;
using ProjectHeleus.MangaApp.Models.Messages;
using ProjectHeleus.SharedLibrary.Extenstions;
using ProjectHeleus.SharedLibrary.Models;
using ProjectHeleus.SharedLibrary.Providers.Menu.Contracts;
using ColorHelper = Microsoft.Toolkit.Uwp.ColorHelper;

namespace ProjectHeleus.MangaApp.ViewModels
{
    public class ShellPageViewModel
        : Screen, IHandle<UpdateAppTheme>
    {
        #region Private Members

        private readonly IEventAggregator _eventAggregator;
        private readonly WinRTContainer _container;
        private readonly IMenuProvider _menuProvider;
        
        private INavigationService _navigation;

        private bool _paneOpen;

        #endregion

        #region Menu

        public BindableCollection<MenuItem> MainMenu
        {
            get { return _menuProvider.GetMainItems().ToBindableCollection(); }
        }
        public BindableCollection<MenuItem> OptionsMenu
        {
            get { return _menuProvider.GetOptionItems().ToBindableCollection(); }
        }

        public bool PaneOpen
        {
            get { return _paneOpen; }
            set
            {
                _paneOpen = value;
                NotifyOfPropertyChange();
            }
        }
        private void UpdatePane()
        {
            PaneOpen = PaneOpen && !PaneOpen;
        }
        public string PaneBackgroundColor
        {
            get { return Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneBackgroundColor"] as string ?? ColorHelper.ToHex(Colors.Black); }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneBackgroundColor"] = value;
                NotifyOfPropertyChange();
            }
        }

        private void MainMenuItemClick(object sender, ItemClickEventArgs eventArgs)
        {
            var menuItem = eventArgs.ClickedItem as MenuItem;

            //if (menuItem.Page == typeof(HardwareArticlesViewModel))
            //    _navigation.For<HardwareArticlesViewModel>().Navigate();
            //if (menuItem.Page == typeof(SoftwareArticlesViewModel))
            //    _navigation.For<SoftwareArticlesViewModel>().Navigate();

            UpdatePane();
        }
        private void OptionMenuItemClick(object sender, ItemClickEventArgs eventArgs)
        {
            var menuItem = eventArgs.ClickedItem as MenuItem;

            if (menuItem.Page == typeof(SettingsPageViewModel))
                _navigation.For<SettingsPageViewModel>().Navigate();

            UpdatePane();
        }

        #endregion

        #region Navigation

        public void SetupNavigation(Frame frame)
        {
            _navigation = _container.RegisterNavigationService(frame);
            _navigation.BackRequested += (sender, args) =>
            {
                if (_navigation.CanGoBack)
                    _navigation.GoBack();
                else
                    TryClose();
            };
        }

        #endregion

        public ShellPageViewModel(WinRTContainer container, IEventAggregator eventAggregator, IMenuProvider menuProvider)
        {
            _eventAggregator = eventAggregator;
            _menuProvider = menuProvider;
            _container = container;

            Initialize();
            NotifyProperties();
        }

        private void Initialize()
        {
            _eventAggregator.Subscribe(this);
        }
        private void NotifyProperties()
        {
            NotifyOfPropertyChange(nameof(MainMenu));
            NotifyOfPropertyChange(nameof(OptionsMenu));
        }

        #region Implementation of IHandle<UpdateAppTheme>

        public void Handle(UpdateAppTheme message)
        {
            UpdateAppTheme(message.Theme);
        }
        private void UpdateAppTheme(AppTheme theme)
        {
            switch (theme)
            {
                case AppTheme.Light:
                    PaneBackgroundColor = ColorHelper.ToHex(Colors.White);
                    break;
                case AppTheme.Dark:
                    PaneBackgroundColor = ColorHelper.ToHex(Colors.Black);
                    break;
            }
        }

        #endregion
    }
}