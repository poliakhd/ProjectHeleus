namespace ProjectHeleus.MangaApp
{
    using System;
    using System.Collections.Generic;

    using Windows.UI.Core;
    using Windows.ApplicationModel.Activation;

    using Caliburn.Micro;

    using Providers;
    using ViewModels;
    using MangaLibrary.Providers;
    using MangaLibrary.Core.Collections;
    using MangaLibrary.Providers.Interfaces;
    using WindowsLibrary.Providers.Menu.Contracts;

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
//#if DEBUG
//                .Instance(CurrentAppSimulator.LicenseInformation)
//                .Singleton<IInAppPurchaseProvider, SimulatorInAppPurchaseProviderProvider>()
//#else
//                .Instance(CurrentApp.LicenseInformation)
//                .Singleton<IInAppPurchaseProvider, InAppPurchaseProviderProvider>()
//#endif
                .Singleton<IMenuProvider, ShellMenuProvider>()
                .Singleton<ICatalogsProvider, MangaCatalogsProvider>()
                .Singleton<ShellPageViewModel>()
                .Singleton<SettingsPageViewModel>()
                .PerRequest<CatalogsPageViewModel>()
                .PerRequest<MangaCollection>();

            ConfigureNavigation();
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
