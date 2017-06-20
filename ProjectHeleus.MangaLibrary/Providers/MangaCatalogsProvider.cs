namespace ProjectHeleus.MangaLibrary.Providers
{
    using System;
    using System.Threading.Tasks;

    using Windows.Web.Http;

    using Caliburn.Micro;
    using Newtonsoft.Json;
    using Microsoft.Toolkit.Uwp;

    using Interfaces;
    using Shared.Models;


    public class MangaCatalogsProvider
        : ICatalogsProvider
    {
        #region Implementation of IBaseProvider

        public string Url { get; set; }

        #endregion

        #region Implementation of ICatalogsProvider

        public async Task<ProviderRespose<BindableCollection<CatalogModel>>> GetAllCatalogs()
        {
            try
            {
                using (var request = new HttpHelperRequest(new Uri("http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs"), HttpMethod.Get))
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    var responseData = JsonConvert.DeserializeObject<BindableCollection<CatalogModel>>(await response.GetTextResultAsync());

                    return new ProviderRespose<BindableCollection<CatalogModel>>()
                    {
                        HasResponse = true,
                        Value = responseData,
                        HasError = false,
                        ErrorMessage = string.Empty,
                        ErrorType = ErrorType.None
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProviderRespose<BindableCollection<CatalogModel>>()
                {
                    HasResponse = false,
                    Value = null,
                    HasError = true,
                    ErrorMessage = ex.Message,
                    ErrorType = ErrorType.Unknown
                };
            }
        }

        public Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog)
        {
            return GetCatalogContent(catalog, null, null, 0);
        }
        public Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, int page)
        {
            return GetCatalogContent(catalog, null, null, page);
        }
        public Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, SortModel sort, int page)
        {
            return GetCatalogContent(catalog, null, sort, page);
        }
        public Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, GenreModel genre, int page)
        {
            return GetCatalogContent(catalog, genre, null, page);
        }
        public async Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, GenreModel genre, SortModel sort, int page)
        {
            try
            {
                using (var request = new HttpHelperRequest(new Uri(BuildUrl(catalog, genre, sort, page)), HttpMethod.Get))
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    var responseData = JsonConvert.DeserializeObject<BindableCollection<MangaPreviewModel>>(await response.GetTextResultAsync());

                    return new ProviderRespose<BindableCollection<MangaPreviewModel>>()
                    {
                        HasResponse = true,
                        Value = responseData,
                        HasError = false,
                        ErrorMessage = string.Empty,
                        ErrorType = ErrorType.None
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProviderRespose<BindableCollection<MangaPreviewModel>>()
                {
                    HasResponse = false,
                    Value = null,
                    HasError = true,
                    ErrorMessage = ex.Message,
                    ErrorType = ErrorType.Unknown
                };
            }
        }


        public async Task<ProviderRespose<BindableCollection<SortModel>>> GetCatalogSorts(CatalogModel catalog)
        {
            try
            {
                using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs/{catalog.Id}/"), HttpMethod.Get))
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    var responseData = JsonConvert.DeserializeObject<BindableCollection<SortModel>>(await response.GetTextResultAsync());

                    return new ProviderRespose<BindableCollection<SortModel>>()
                    {
                        HasResponse = true,
                        Value = responseData,
                        HasError = false,
                        ErrorMessage = string.Empty,
                        ErrorType = ErrorType.None
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProviderRespose<BindableCollection<SortModel>>()
                {
                    HasResponse = false,
                    Value = null,
                    HasError = true,
                    ErrorMessage = ex.Message,
                    ErrorType = ErrorType.Unknown
                };
            }
        }
        public async Task<ProviderRespose<BindableCollection<GenreModel>>> GetCatalogGenres(CatalogModel catalog)
        {
            try
            {
                using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/genres/{catalog.Id}/"), HttpMethod.Get))
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    var responseData = JsonConvert.DeserializeObject<BindableCollection<GenreModel>>(await response.GetTextResultAsync());
                    return new ProviderRespose<BindableCollection<GenreModel>>()
                    {
                        HasResponse = true,
                        Value = responseData,
                        HasError = false,
                        ErrorMessage = string.Empty,
                        ErrorType = ErrorType.None
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProviderRespose<BindableCollection<GenreModel>>()
                {
                    HasResponse = false,
                    Value = null,
                    HasError = true,
                    ErrorMessage = ex.Message,
                    ErrorType = ErrorType.Unknown
                };
            }
        }

        #endregion

        private string BuildUrl(CatalogModel catalog, GenreModel genre, SortModel sort, int page)
        {
            if ((genre is null) && (sort is null))
                return $"http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs/{catalog.Id}/{page}";

            if ((sort != null) && (genre is null))
                return $"http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs/{catalog.Id}/{sort.Id}/{page}";

            if ((genre != null) && (sort is null))
                return $"http://tenmanga.westeurope.cloudapp.azure.com/api/genres/{catalog.Id}/{genre.Id}/{page}";

            return $"http://tenmanga.westeurope.cloudapp.azure.com/api/genres/{catalog.Id}/{genre.Id}/{sort.Id}/{page}";
        }
    }
}