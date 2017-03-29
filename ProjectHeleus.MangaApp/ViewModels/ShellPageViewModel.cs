using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using ProjectHeleus.SharedLibrary.Extenstions;
using ProjectHeleus.SharedLibrary.Models;
using ProjectHeleus.SharedLibrary.Providers.Menu.Contracts;

namespace ProjectHeleus.MangaApp.ViewModels
{
    public class ShellPageViewModel
        : Screen
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

        private void MainMenuItemClick(object sender, ItemClickEventArgs eventArgs)
        {
            var menuItem = eventArgs.ClickedItem as MenuItem;

            if (menuItem.Page == typeof(CatalogsPageViewModel))
                _navigation.For<CatalogsPageViewModel>().Navigate();
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
    }
}