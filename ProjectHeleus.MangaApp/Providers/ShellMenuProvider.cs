namespace ProjectHeleus.MangaApp.Providers
{
    using System.Collections.Generic;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml.Controls;
    using SharedLibrary.Models;
    using SharedLibrary.Providers.Menu.Contracts;
    using ViewModels;

    public class ShellMenuProvider : IMenuProvider
    {
        private readonly ResourceLoader _resourceLoader = new ResourceLoader();

        #region Implementation of IMenuProvider

        public virtual IEnumerable<MenuItem> GetMainItems()
        {
            yield return new MenuItem { Name = _resourceLoader.GetString("Home"), Icon = Symbol.Home};
            yield return new MenuItem { Name = _resourceLoader.GetString("Catalogs"), Icon = Symbol.Library, Page = typeof(CatalogsPageViewModel)};
        }

        public IEnumerable<MenuItem> GetOptionItems()
        {
            yield return new MenuItem { Name = _resourceLoader.GetString("Settings"), Icon = Symbol.Setting, Page = typeof(SettingsPageViewModel)};
        }

        #endregion
    }
}