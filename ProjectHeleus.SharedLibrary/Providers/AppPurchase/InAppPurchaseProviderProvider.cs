using System;
using Windows.ApplicationModel.Store;
using ProjectHeleus.SharedLibrary.Providers.AppPurchase.Contracts;

namespace ProjectHeleus.SharedLibrary.Providers.AppPurchase
{
    public class InAppPurchaseProviderProvider : IInAppPurchaseProvider
    {
        #region Implementation of IAppPurchaseProvider

        public async void Buy(string name)
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses[name].IsActive)
            {
                try
                {
                    await CurrentApp.RequestProductPurchaseAsync(name);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        #endregion 
    }
}