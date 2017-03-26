using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using ProjectHeleus.MangaApp.ViewModels;
using ProjectHeleus.SharedLibrary.Models;
using ProjectHeleus.SharedLibrary.Providers.Menu.Contracts;

namespace ProjectHeleus.MangaApp.Providers
{
    public class MenuProvider : IMenuProvider
    {
        private readonly ResourceLoader _resourceLoader = new ResourceLoader();

        #region Implementation of IMenuProvider

        public IEnumerable<MenuItem> GetMainItems()
        {
            yield return new MenuItem { Name = _resourceLoader.GetString("Home"), Icon = Symbol.Home};
            yield return new MenuItem { Name = _resourceLoader.GetString("Library"), Icon = Symbol.Library};
        }

        public IEnumerable<MenuItem> GetOptionItems()
        {
            yield return new MenuItem { Name = _resourceLoader.GetString("Settings"), Icon = Symbol.Setting, Page = typeof(SettingsPageViewModel)};
        }

        #endregion
    }
}