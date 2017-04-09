using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;
using ProjectHeleus.MangaService.Parsers.Core;

namespace ProjectHeleus.MangaService.Parsers
{
    public class MangaFoxParser 
        : DefaultParser
    {
        private readonly ILogger<MangaFoxParser> _logger;

        #region Private Members

        private string _updateUrl;
        private string _ratingUrl;
        private string _popularUrl;

        #endregion

        public new string Url { get; set; } = "http://mangafox.me";

        public MangaFoxParser(ILogger<MangaFoxParser> logger)
        {
            _logger = logger;

            _updateUrl = $"{Url}/directory/?latest";
            _ratingUrl = $"{Url}/directory/?rating";
            _popularUrl = $"{Url}/directory/";
        }

        #region Overrides of IParser

        #region Get Catalogs Content

        public override async Task<IEnumerable<IManga>> GetUpdateContent(int page)
        {
            return await GetListContent(_updateUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetRatingContent(int page)
        {
            return await GetListContent(_ratingUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetPopularContent(int page)
        {
            return await GetListContent(_popularUrl, page);
        }

        private async Task<IEnumerable<MangaShortModel>> GetListContent(string url, int page)
        {
            #region Build URL

            if (page > 1)
                url = $"{url.Substring(0, url.LastIndexOf('/') + 1)}{page}.htm{url.Substring(url.LastIndexOf('/') + 1)}";

            #endregion

            var formattedMangas = new List<MangaShortModel>();

            try
            {
                using (var htmlDocument = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(url))
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

                        var formattedManga = new MangaShortModel
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

                        if (ratings != null)
                        {
                            formattedManga.Rating = float.Parse(ratings);
                            formattedManga.RatingLimit = 5;
                        }

                        #endregion

                        var info = htmlManga.QuerySelectorAll(".info");

                        #region Views

                        if (info.Length > 1)
                            if(int.TryParse(info[1].TextContent.Split(' ')[1].Replace(".", "").Replace(",", ""), out int views))
                                formattedManga.Views = views;

                        #endregion

                        #region Genres

                        formattedManga.Authors = null;
                        formattedManga.Genres =
                            info[0].TextContent.Split(',')
                                .Select(x => x.Replace(".", "").Trim())
                                .Where(y => !string.IsNullOrEmpty(y))
                                .Select(x => new GenreModel() {Title = x, Url = null});

                        #endregion

                        formattedMangas.Add(formattedManga);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get calatog content from: {url}");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }

            return formattedMangas;
        }

        #endregion

        #region Get Manga Content

        public override async Task<IManga> GetMangaContent(string url)
        {
            #region Build URL

            url = $"{Url}/manga/{url}";

            #endregion

            var formattedManga = new MangaModel();

            try
            {
                using (var htmlDocument = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(url))
                {
                    var htmlManga = htmlDocument.QuerySelector("#page > .left");

                    if (htmlManga == null)
                    {
                        _logger.LogInformation($"There were no content for manga: {url}");
                        return null;
                    }

                    formattedManga.Id = url.Substring(url.LastIndexOf('/') + 1);

                    GetInformation(htmlManga, formattedManga);
                    GetVolume(htmlManga, formattedManga);
                    GetViews(htmlManga, formattedManga);
                    GetGenres(htmlManga, formattedManga);
                    GetAuthors(htmlManga, formattedManga);
                    GetPublishedYear(htmlManga, formattedManga);
                    GetChapters(htmlManga, formattedManga);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get manga content from: {url}");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }

            return formattedManga;
        }
        public override async Task<IChapterContent> GetMangaChapterContent(string url)
        {
            #region Build URL

            var urlTemplate = $"{Url}/manga{url.Substring(0, url.LastIndexOf('/') + 1)}{{0}}.html";
            url = $"{Url}/manga{url}";

            #endregion

            var imagesLinks = new List<string>();

            try
            {
                IEnumerable<IElement> htmlImagesLinks;

                using (var htmlDocument = await BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithJavaScript()).OpenAsync(url))
                {
                    htmlImagesLinks = htmlDocument.QuerySelectorAll("#top_bar select option").Where(x => x.TextContent != "Comments" && x.TextContent != "1");
                    imagesLinks.Add(htmlDocument.QuerySelector(".read_img a img").GetAttribute("src"));
                }

                foreach (var htmlImageLink in htmlImagesLinks)
                {
                    using (var imageSource = await BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithJavaScript()).OpenAsync(string.Format(urlTemplate, htmlImageLink.TextContent)))
                    {
                        imagesLinks.Add(imageSource.QuerySelector(".read_img a img")?.GetAttribute("src"));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get manga chapter content from: {url}");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }

            return new ChapterContentModel() { Images = imagesLinks };
        }

        private void GetInformation(IElement htmlManga, MangaModel formattedManga)
        {
            formattedManga.Name = htmlManga.QuerySelector("#title h1")?.TextContent;
            formattedManga.AlternateNames = htmlManga.QuerySelector("#title h3")?.TextContent.Split(';');
            formattedManga.Description = htmlManga.QuerySelector(".summary")?.TextContent;
            formattedManga.Covers =
                htmlManga.QuerySelectorAll("#series_info .cover img")?.Select(x => new Uri(x.GetAttribute("src")));

            var textRating = htmlManga.QuerySelectorAll("#series_info > .data > span")?[2].TextContent;

            if (string.IsNullOrEmpty(textRating))
                return;

            var firstRatingIndex = textRating.IndexOf(' ') + 1;
            var lastRatinIndex = textRating.IndexOf('/') - 1;

            formattedManga.Rating =
                float.Parse(textRating.Substring(firstRatingIndex, lastRatinIndex - firstRatingIndex));
            formattedManga.RatingLimit = 5;
        }
        private void GetVolume(IElement htmlManga, MangaModel formattedManga)
        {
            var volumes = htmlManga.QuerySelectorAll(".volume");
            var lastVolume = volumes.FirstOrDefault()?.TextContent;

            if (!string.IsNullOrEmpty(lastVolume))
                formattedManga.Volumes = lastVolume.Contains("TBD") ? $"> {volumes[1].TextContent.Split(' ')[1]}" : $"{lastVolume.Split(' ')[1]}";
            else
                formattedManga.Volumes = "N/A";
        }
        private void GetViews(IElement htmlManga, MangaModel formattedManga)
        {
            var views = htmlManga.QuerySelectorAll("#series_info > .data > span")?[1].TextContent.TrimStart().Split(' ');

            formattedManga.Views = -1;

            if (views != null && views.Any())
                if (int.TryParse(views[3].Replace(",", ""), out int result))
                    formattedManga.Views = result;
        }
        private void GetGenres(IElement htmlManga, MangaModel formattedManga)
        {
            var titleInfo = htmlManga.QuerySelectorAll("#title table td");

            formattedManga.Genres =
                titleInfo[3].QuerySelectorAll("a")
                    .Select(
                        x =>
                            new GenreModel()
                            {
                                Id =
                                    x.GetAttribute("href")
                                        .Replace(Url, "")
                                        .TrimEnd('/')
                                        .Substring(
                                            x.GetAttribute("href").Replace(Url, "").TrimEnd('/').LastIndexOf('/') + 1),
                                Title = x.TextContent,
                                Url = x.GetAttribute("href")
                            });
        }
        private void GetAuthors(IElement htmlManga, MangaModel formattedManga)
        {
            var titleInfo = htmlManga.QuerySelectorAll("#title table td");

            formattedManga.Authors =
                titleInfo[1].QuerySelectorAll("a")
                    .Select(x => new AuthorModel()
                    {
                        Id =
                            x.GetAttribute("href")
                                .Replace(Url, "")
                                .TrimEnd('/')
                                .Substring(
                                    x.GetAttribute("href").Replace(Url, "").TrimEnd('/').LastIndexOf('/') + 1),
                        Name = x.TextContent,
                        Url = x.GetAttribute("href")
                    });
        }
        private void GetPublishedYear(IElement htmlManga, MangaModel formattedManga)
        {
            var titleInfo = htmlManga.QuerySelectorAll("#title table td");
            formattedManga.Published = int.Parse(titleInfo[0].QuerySelector("a").TextContent);
        }
        private void GetChapters(IElement htmlManga, MangaModel formattedManga)
        {
            var chaptersWeb = htmlManga.QuerySelectorAll(".tips");
            var datesWeb = htmlManga.QuerySelectorAll("#chapters .date");

            var chapters = new List<ChapterModel>();
            for (int i = 0; i < chaptersWeb.Length; i++)
            {
                chapters.Add(new ChapterModel()
                {
                    Id = chaptersWeb[i].GetAttribute("href").Replace(Url, "").Replace("/manga/", ""),
                    Name = chaptersWeb[i].TextContent,
                    Url = chaptersWeb[i].GetAttribute("href"),
                    Date = datesWeb[i].TextContent
                });
            }

            formattedManga.Chapters = chapters;
        }

        #endregion

        #endregion
    }
}