using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;
using Windows.UI;
using Windows.UI.Core;
using Caliburn.Micro;
using ProjectHeleus.MangaApp.Providers;
using ProjectHeleus.MangaApp.ViewModels;
using ProjectHeleus.SharedLibrary.Providers.AppPurchase;
using ProjectHeleus.SharedLibrary.Providers.AppPurchase.Contracts;
using ProjectHeleus.SharedLibrary.Providers.Menu.Contracts;
using ColorHelper = Microsoft.Toolkit.Uwp.ColorHelper;

namespace ProjectHeleus.MangaApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        #region Private Members

        private WinRTContainer _container;

        #endregion

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container
#if DEBUG
                .Instance(CurrentAppSimulator.LicenseInformation)
                .Singleton<IInAppPurchaseProvider, SimulatorInAppPurchaseProviderProvider>()
#else
                .Instance(CurrentApp.LicenseInformation)
                .Singleton<IInAppPurchaseProvider, InAppPurchaseProviderProvider>()
#endif
                .Singleton<IMenuProvider, MenuProvider>()
                .Singleton<ShellPageViewModel>()
                .PerRequest<SettingsPageViewModel>();

            ConfigureSettings();
            ConfigureNavigation();
        }

        private void ConfigureSettings()
        {
            if(Windows.Storage.ApplicationData.Current.LocalSettings.Values["IsDarkThemeEnabled"] is null)
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["IsDarkThemeEnabled"] = false;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneBackgroundColor"] is null)
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneBackgroundColor"] = ColorHelper.ToHex(Colors.White);

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneForegroundColor"] is null)
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["PaneForegroundColor"] = ColorHelper.ToHex(Colors.Black);
        }

        private static void ConfigureNavigation()
        {
            var navigation = SystemNavigationManager.GetForCurrentView();
            navigation.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            navigation.BackRequested += (o, e) =>
            {
                e.Handled = true;

                var n = IoC.Get<INavigationService>();
                if (n.CanGoBack)
                    n.GoBack();
            };
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            DisplayRootViewFor<ShellPageViewModel>();
        }

        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            base.OnSuspending(sender, e);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
