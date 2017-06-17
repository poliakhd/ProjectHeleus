namespace ProjectHeleus.MangaService.Parsers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using AngleSharp.Parser.Html;
    using Enhancements;
    using Extensions;
    using Microsoft.Extensions.Logging;

    using Shared.Types;
    using Shared.Models;
    using Shared.Models.Interfaces;

    public partial class MangaFoxParser 
    {
        #region Implementation of IGenreParser

        public async Task<IEnumerable<IGenre>> GetCatalogGenresAsync()
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new EnhancedHttpRequestMessage(HttpMethod.Get, $"{Url}/directory/"))
                using (var responese = await client.SendAsync(request))
                {
                    if (responese.IsSuccessStatusCode)
                    {
                        using (var htmlDocument = new HtmlParser().Parse(responese.GetStringContent()))
                        {
                            var htmlGenres = htmlDocument.QuerySelectorAll("#genre_filter li > a");
                            if (htmlGenres == null)
                            {
                                _logger.LogWarning($"Genres were not found at {Url}/directory/");
                                return null;
                            }

                            return htmlGenres.Select(x => new GenreModel()
                            {
                                Id = x.GetAttribute("href")?.TrimEnd('/').Substring(x.GetAttribute("href").TrimEnd('/').LastIndexOf("/") + 1),
                                Title = x.TextContent,
                                Url = $"{Url}{x.GetAttribute("href")}"
                            });
                        }
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get genres content from: {Url}/directory/");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }
        }
        public async Task<IEnumerable<IManga>> GetGenreMangasAsync(SortType sortType, string url, int page)
        {
            return await ParseCatalogAsync(BuildGenreUrl(sortType, url, page), page, true);
        }

        #endregion

        private string BuildGenreUrl(SortType sortType, string url, int page)
        {
            var formattedUrl = $"{Url}/directory/{url}/{{0}}";

            if (page > 0)
            {
                formattedUrl = formattedUrl.Replace("{0}", "");
                formattedUrl = $"{formattedUrl}{page}.html{{0}}";
            }

            switch (sortType)
            {
                case SortType.Popular:
                    formattedUrl = string.Format(formattedUrl, "");
                    break;
                case SortType.Rating:
                    formattedUrl = string.Format(formattedUrl, "?rating");
                    break;
                case SortType.Update:
                    formattedUrl = string.Format(formattedUrl, "?latest");
                    break;
            }

            return formattedUrl;
        }
    }
}