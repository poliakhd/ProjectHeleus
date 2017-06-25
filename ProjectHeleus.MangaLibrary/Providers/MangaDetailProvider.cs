namespace ProjectHeleus.MangaLibrary.Providers
{
    using System;
    using System.Threading.Tasks;

    using Windows.Web.Http;

    using Newtonsoft.Json;
    using Microsoft.Toolkit.Uwp;

    using Interfaces;
    using Shared.Models;

    public class MangaDetailProvider : IDetailProvider
    {
        #region Implementation of IBaseProvider

        public string Url { get; set; }

        #endregion

        #region Implementation of IDetailProvider

        public async Task<ProviderRespose<MangaModel>> GetMangaContent(CatalogModel catalog, MangaPreviewModel mangaPreview)
        {
            try
            {
                using (var request =
                    new HttpHelperRequest(
                        new Uri(
                            $"http://tenmanga.westeurope.cloudapp.azure.com/api/manga/{catalog.Id}/{mangaPreview.Id}"),
                        HttpMethod.Get))
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    if (response.Success)
                    {
                        var responseData = JsonConvert.DeserializeObject<MangaModel>(await response.GetTextResultAsync());
                        return new ProviderRespose<MangaModel>()
                        {
                            HasResponse = true,
                            Value = responseData,
                            HasError = false,
                            ErrorMessage = string.Empty,
                            ErrorType = ErrorType.None
                        };
                    }

                    throw new FailedResponseException() {StatusCode = response.StatusCode};
                }
            }
            catch (FailedResponseException frex)
            {
                return new ProviderRespose<MangaModel>()
                {
                    HasResponse = false,
                    Value = null,
                    HasError = true,
                    ErrorMessage = string.Empty,
                    ErrorType = ErrorType.ServiceError
                };
            }
            catch (Exception ex)
            {
                return new ProviderRespose<MangaModel>()
                {
                    HasResponse = false,
                    Value = null,
                    HasError = true,
                    ErrorMessage = ex.Message,
                    ErrorType = ErrorType.Unknown
                };
            }
        }
        public async Task<ProviderRespose<ChapterImagesModel>> GetMangaChapterContent(CatalogModel catalog, MangaModel manga, ChapterModel chapter)
        {
            try
            {
                using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/manga/{catalog.Id}/{chapter.Id}"), HttpMethod.Get))
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    if (response.Success)
                    {
                        var responseData = JsonConvert.DeserializeObject<ChapterImagesModel>(await response.GetTextResultAsync());
                        return new ProviderRespose<ChapterImagesModel>()
                        {
                            HasResponse = true,
                            Value = responseData,
                            HasError = false,
                            ErrorMessage = string.Empty,
                            ErrorType = ErrorType.None
                        };

                    }

                    throw new FailedResponseException() { StatusCode = response.StatusCode };
                }
            }
            catch (FailedResponseException frex)
            {
                return new ProviderRespose<ChapterImagesModel>()
                {
                    HasResponse = false,
                    Value = null,
                    HasError = true,
                    ErrorMessage = string.Empty,
                    ErrorType = ErrorType.ServiceError
                };
            }
            catch (Exception ex)
            {
                return new ProviderRespose<ChapterImagesModel>()
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
    }
}