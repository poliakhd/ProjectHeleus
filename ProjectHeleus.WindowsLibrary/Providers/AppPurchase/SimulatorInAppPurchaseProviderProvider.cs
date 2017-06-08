namespace ProjectHeleus.WindowsLibrary.Providers.AppPurchase
{
    using System;

    using Windows.ApplicationModel.Store;

    using Contracts;

    public class SimulatorInAppPurchaseProviderProvider : IInAppPurchaseProvider
    {
        #region Implementation of IAppPurchaseProvider

        public async void Buy(string name)
        {
            if (!CurrentAppSimulator.LicenseInformation.ProductLicenses[name].IsActive)
            {
                try
                {
                    await CurrentAppSimulator.RequestProductPurchaseAsync(name);
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