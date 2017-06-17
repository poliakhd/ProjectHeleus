namespace ProjectHeleus.MangaService.Parsers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using AngleSharp.Parser.Html;

    using Microsoft.Extensions.Logging;

    using Extensions;
    using Enhancements;
    using Shared.Types;
    using Shared.Models;
    using Shared.Models.Interfaces;

    public partial class ReadMangaParser
    {
        #region Implementation of IGenreParser

        public async Task<IEnumerable<IGenre>> GetCatalogGenresAsync()
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new EnhancedHttpRequestMessage(HttpMethod.Get, $"{Url}/list/genres/sort_name"))
                using (var responese = await client.SendAsync(request))
                {
                    if (responese.IsSuccessStatusCode)
                    {
                        using (var htmlDocument = new HtmlParser().Parse(responese.GetStringContent()))
                        {
                            return
                                htmlDocument.QuerySelectorAll(".table.table-hover tbody tr td a")?
                                    .Select(
                                        x =>
                                            new GenreModel()
                                            {
                                                Id =
                                                    x.GetAttribute("href")?
                                                        .TrimEnd('/')
                                                        .Substring(
                                                            x.GetAttribute("href").TrimEnd('/').LastIndexOf("/") + 1),
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
                _logger.LogError($"Cannot get genres content from: {Url}/list/genres/sort_name");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }
        }
        public async Task<IEnumerable<IManga>> GetGenreMangasAsync(SortType sortType, string url, int page)
        {
            return await ParseCatalogAsync(GetGenreContentUrl(sortType, url, page), 0);
        }

        #endregion

        private string GetGenreContentUrl(SortType sortType, string url, int page)
        {
            string formattedUrl = $"{Url}/list/genre/{url}{{0}}{{1}}"; ;

            switch (sortType)
            {
                case SortType.New:
                    formattedUrl = string.Format(formattedUrl, "?sortType=created", page > 0 ? $"{url}&offset={70 * page}&max=70" : "");
                    break;
                case SortType.Popular:
                    formattedUrl = string.Format(formattedUrl, "?sortType=rate", page > 0 ? $"{url}&offset={70 * page}&max=70" : "");
                    break;
                case SortType.Rating:
                    formattedUrl = string.Format(formattedUrl, "?sortType=votes", page > 0 ? $"{url}&offset={70 * page}&max=70" : "");
                    break;
                case SortType.Update:
                    formattedUrl = string.Format(formattedUrl, "?sortType=updated", page > 0 ? $"{url}&offset={70 * page}&max=70" : "");
                    break;
            }

            return formattedUrl;
        }
    }
}