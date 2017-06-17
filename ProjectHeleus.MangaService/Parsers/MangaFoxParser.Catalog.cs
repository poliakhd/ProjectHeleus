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
        #region Implementation of ICatalogParser

        public async Task<IEnumerable<ISort>> GetCatalogSortings()
        {
            var sorts = new []
            {
                new SortModel {Id = SortType.Update.ToString().ToLower(), Title = "Latest Chapters"},
                new SortModel {Id = SortType.Rating.ToString().ToLower(), Title = "Rating"},
                new SortModel {Id = SortType.Popular.ToString().ToLower(), Title = "Popularity"}
            };

            return await Task.FromResult(sorts);
        }
        public async Task<IEnumerable<IManga>> GetCatalogMangasAsync(SortType sortType, int page)
        {
            switch (sortType)
            {
                case SortType.Update:
                    return await ParseCatalogAsync($"{Url}/directory/?latest", page);
                case SortType.Rating:
                    return await ParseCatalogAsync($"{Url}/directory/?rating", page);
                case SortType.Popular:
                    return await ParseCatalogAsync($"{Url}/directory/", page);
                default:
                    return null;
            }
        }

        #endregion

        private async Task<IEnumerable<MangaPreviewModel>> ParseCatalogAsync(string url, int page, bool skipUrlBuilding = false)
        {
            #region Build URL

            if (!skipUrlBuilding && page > 0)
                url = $"{url.Substring(0, url.LastIndexOf('/') + 1)}{page}.htm{url.Substring(url.LastIndexOf('/') + 1)}";

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
                            var htmlMangas = htmlDocument.QuerySelectorAll("#mangalist .list li");

                            if (!htmlMangas.Any())
                            {
                                _logger.LogInformation($"There were no content for catalog: {url}");
                                return null;
                            }

                            foreach (var htmlManga in htmlMangas)
                            {
                                #region Header

                                var id = htmlManga.QuerySelector(".manga_text a")?.GetAttribute("href").TrimEnd('/');
                                id = id.Substring(id.LastIndexOf('/') + 1);

                                var parsedManga = new MangaPreviewModel
                                {
                                    Id = id,
                                    Title =
                                        htmlManga.QuerySelector(".manga_text a")?
                                            .TextContent.Replace("\n", "")
                                            .TrimStart(' ')
                                            .TrimEnd(' '),
                                    Url = htmlManga.QuerySelector(".manga_text a")?.GetAttribute("href").Replace(Url, ""),
                                    Cover = htmlManga.QuerySelector(".manga_img div img")?.GetAttribute("src")
                                };

                                #endregion

                                #region Rating

                                var ratings = htmlManga.QuerySelector(".rate")?.TextContent;

                                if (!string.IsNullOrEmpty(ratings))
                                {
                                    parsedManga.Rating = float.Parse(ratings);
                                    parsedManga.RatingLimit = 5;
                                }

                                #endregion

                                parsedMangas.Add(parsedManga);
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