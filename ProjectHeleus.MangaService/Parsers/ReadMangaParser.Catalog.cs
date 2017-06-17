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
        #region Implementation of ICatalogParser

        public async Task<IEnumerable<ISort>> GetCatalogSortings()
        {
            var sorts = new []
            {
                new SortModel {Id = SortType.Update.ToString().ToLower(), Title = "Дата обновления"},
                new SortModel {Id = SortType.Rating.ToString().ToLower(), Title = "Рейтинг"},
                new SortModel {Id = SortType.Popular.ToString().ToLower(), Title = "Популярность"},
                new SortModel {Id = SortType.New.ToString().ToLower(), Title = "Новинки"},
            };

            return await Task.FromResult(sorts);
        }
        public async Task<IEnumerable<IManga>> GetCatalogMangasAsync(SortType sortType, int page)
        {
            switch (sortType)
            {
                case SortType.New:
                    return await ParseCatalogAsync($"{Url}/list?sortType=created", page);
                case SortType.Popular:
                    return await ParseCatalogAsync($"{Url}/list?sortType=rate", page);
                case SortType.Rating:
                    return await ParseCatalogAsync($"{Url}/list?sortType=votes", page);
                case SortType.Update:
                    return await ParseCatalogAsync($"{Url}/list?sortType=updated", page);
                default:
                    return null;
            }
        }

        #endregion

        private async Task<IEnumerable<IManga>> ParseCatalogAsync(string url, int page)
        {
            #region Build URL

            if (page > 0)
                url = $"{url}&offset={70 * page}&max=70";

            #endregion

            var parsedMangas = new List<MangaPreviewModel>();

            try
            {
                using (var client = new HttpClient())
                using (var request = new EnhancedHttpRequestMessage(HttpMethod.Get, url))
                using (var responese = await client.SendAsync(request))
                {
                    if (responese.IsSuccessStatusCode)
                    {
                        using (var htmlDocument = new HtmlParser().Parse(responese.GetStringContent()))
                        {
                            var htmlMangas = htmlDocument.QuerySelectorAll(".tile.col-sm-6");

                            if (!htmlMangas.Any())
                            {
                                _logger.LogInformation($"There were no content for catalog: {url}");
                                return null;
                            }

                            foreach (var htmlManga in htmlMangas)
                            {
                                #region Header

                                var formattedManga = new MangaPreviewModel
                                {
                                    Id = htmlManga.QuerySelector(".img a")?.GetAttribute("href").Replace("/", ""),
                                    Title =
                                        htmlManga.QuerySelector("h4")?
                                            .TextContent.Replace("\n", "")
                                            .TrimStart(' ')
                                            .TrimEnd(' '),
                                    Url = $@"{Url}{htmlManga.QuerySelector(".img a")?.GetAttribute("href")}",
                                    Cover = htmlManga.QuerySelector(".img a img")?.GetAttribute("src")
                                };

                                #endregion

                                #region Rating

                                var htmlRatings =
                                    htmlManga.QuerySelector(".desc .rating")?.GetAttribute("title").Split(' ');

                                if (htmlRatings != null)
                                {
                                    formattedManga.Rating = float.Parse(htmlRatings[0]);
                                    formattedManga.RatingLimit = float.Parse(htmlRatings[2]);
                                }

                                #endregion

                                parsedMangas.Add(formattedManga);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get calatog content from: {url}");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }

            return parsedMangas;
        }
    }
}