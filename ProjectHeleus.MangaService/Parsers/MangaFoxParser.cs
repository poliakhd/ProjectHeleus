namespace ProjectHeleus.MangaService.Parsers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using AngleSharp;
    using AngleSharp.Dom;
    using Microsoft.Extensions.Logging;

    using Interfaces;
    using Shared.Types;
    using Shared.Models;
    using Shared.Models.Interfaces;

    public class MangaFoxParser 
        : IParser
    {
        #region Private Members

        private readonly ILogger<MangaFoxParser> _logger;

        #endregion

        public MangaFoxParser(ILogger<MangaFoxParser> logger)
        {
            _logger = logger;
        }

        #region Implementation of IParser

        public string Url { get; set; } = "http://mangafox.me";

        #endregion

        #region Implementation of ICatalogParser

        public async Task<IEnumerable<ISort>> GetCatalogSorts()
        {
            var sorts = new List<SortModel>
            {
                new SortModel() {Id = SortType.Update.ToString().ToLower(), Title = "Latest Chapters"},
                new SortModel() {Id = SortType.Rating.ToString().ToLower(), Title = "Rating"},
                new SortModel() {Id = SortType.Popular.ToString().ToLower(), Title = "Popularity"}
            };

            return await Task.FromResult(sorts);
        }
        public async Task<IEnumerable<IManga>> GetAllFromCatalogAsync(SortType sortType, int page)
        {
            try
            {
                switch (sortType)
                {
                    case SortType.Update:
                        return await GetCatalogContentAsync($"{Url}/directory/?latest", page);
                    case SortType.Rating:
                        return await GetCatalogContentAsync($"{Url}/directory/?rating", page);
                    case SortType.Popular:
                        return await GetCatalogContentAsync($"{Url}/directory/", page);
                    default:
                        throw new NotSupportedException();
                }
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private async Task<IEnumerable<MangaPreviewModel>> GetCatalogContentAsync(string url, int page)
        {
            #region Build URL

            if (page > 1)
                url = $"{url.Substring(0, url.LastIndexOf('/') + 1)}{page}.htm{url.Substring(url.LastIndexOf('/') + 1)}";

            #endregion

            var formattedMangas = new List<MangaPreviewModel>();

            try
            {
                using (
                    var htmlDocument =
                        await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(url))
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

                        var formattedManga = new MangaPreviewModel
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

        #region Implementation of IMangaParser

        public async Task<IManga> GetMangaAsync(string url)
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
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            return formattedManga;
        }
        public async Task<IChapterImages> GetMangaChapterAsync(string url)
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
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            return new ChapterImagesModel() { Images = imagesLinks };
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

        #region Implementation of IGenreParser

        public async Task<IEnumerable<IGenre>> GetAllGenresAsync()
        {
            try
            {
                using (var htmlDocument = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync($"{Url}/directory/"))
                {
                    return
                        htmlDocument.QuerySelectorAll("#genre_filter li > a")?
                            .Select(
                                x =>
                                    new GenreModel()
                                    {
                                        Id =
                                            x.GetAttribute("href")?
                                                .TrimEnd('/')
                                                .Substring(x.GetAttribute("href").TrimEnd('/').LastIndexOf("/") + 1),
                                        Title = x.TextContent,
                                        Url = $"{Url}{x.GetAttribute("href")}"
                                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get genres content from: {Url}/directory/");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        public async Task<IEnumerable<IManga>> GetAllFromGenreGenreAsync(SortType sortType, string url, int page)
        {
            try
            {
                return await GetCatalogContentAsync(GetGenreContentUrl(sortType, url, page), 0);
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private string GetGenreContentUrl(SortType sortType, string url, int page)
        {
            string formattedUrl = $"{Url}/directory/{url}/";

            if (page > 1)
                formattedUrl = $"{formattedUrl}{page}.html{{0}}";

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

        #endregion
    }
}