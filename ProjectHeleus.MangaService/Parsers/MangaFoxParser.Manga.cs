namespace ProjectHeleus.MangaService.Parsers
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    
    using AngleSharp.Dom;
    using AngleSharp.Parser.Html;

    using Microsoft.Extensions.Logging;

    using Extensions;
    using Enhancements;
    using Shared.Models;
    using Shared.Models.Interfaces;

    public partial class MangaFoxParser
    {
        #region Implementation of IMangaParser

        public async Task<IManga> GetCatalogMangaAsync(string url)
        {
            #region Build URL

            url = $"{Url}/manga/{url}";

            #endregion

            var parsedManga = new MangaModel();

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
                            var htmlManga = htmlDocument.QuerySelector("#page > .left");

                            if (htmlManga == null)
                            {
                                _logger.LogInformation($"There were no content for manga: {url}");
                                return null;
                            }

                            parsedManga.Id = url.Substring(url.LastIndexOf('/') + 1);

                            GetInformation(htmlManga, parsedManga);
                            GetVolume(htmlManga, parsedManga);
                            GetViews(htmlManga, parsedManga);
                            GetGenres(htmlManga, parsedManga);
                            GetAuthors(htmlManga, parsedManga);
                            GetPublishedYear(htmlManga, parsedManga);
                            GetChapters(htmlManga, parsedManga);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get manga content from: {url}");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }

            return parsedManga;
        }
        public async Task<IChapterImages> GetMangaChapterImagesAsync(string url)
        {
            #region Build URL

            var urlTemplate = $"{Url}/manga{url.Substring(0, url.LastIndexOf('/') + 1)}{{0}}.html";
            url = $"{Url}/manga{url}";

            #endregion

            var images = new Dictionary<string, string>();

            try
            {
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    Proxy = new WebProxy("85.143.218.246:3128", false)
                };

                using (var client = new HttpClient(handler))
                using (var request = new EnhancedHttpRequestMessage(HttpMethod.Get, url))
                using (var responese = await client.SendAsync(request))
                {
                    if (responese.IsSuccessStatusCode)
                    {
                        using (var htmlDocument = new HtmlParser().Parse(responese.GetStringContent()))
                        {
                            var htmlImages =
                                htmlDocument.QuerySelectorAll("#top_bar select option")
                                    .Where(x => x.TextContent != "Comments");

                            _logger.LogError(htmlDocument.Body.TextContent);

                            if (!htmlImages.Any())
                                return null;

                            foreach (var imageLink in htmlImages)
                                images.Add(imageLink.TextContent, null);

                            do
                            {
                                foreach (var htmlImage in htmlImages)
                                {
                                    if (!string.IsNullOrEmpty(images[htmlImage.TextContent]))
                                        continue;

                                    using (
                                        var imageRequest = new HttpRequestMessage(HttpMethod.Get,
                                            string.Format(urlTemplate, htmlImage.TextContent)))
                                    using (var imageResponse = await client.SendAsync(imageRequest))
                                    {
                                        if (responese.IsSuccessStatusCode)
                                        {
                                            using (
                                                var imageSource =
                                                    new HtmlParser().Parse(imageResponse.GetStringContent()))
                                            {
                                                images[htmlImage.TextContent] =
                                                    imageSource.QuerySelector(".read_img a img")?.GetAttribute("src");
                                            }
                                        }
                                    }
                                }

                            } while (images.Values.Any(x => x is null));
                        }
                    }
                    else
                    {
                        _logger.LogTrace(responese.StatusCode.ToString());
                        _logger.LogTrace(responese.GetStringContent());
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get manga chapter content from: {url}");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }

            return new ChapterImagesModel() { Images = images.Values };
        }

        #endregion

        private void GetInformation(IElement htmlManga, MangaModel formattedManga)
        {
            formattedManga.Name = htmlManga.QuerySelector("#title h1")?.TextContent;
            formattedManga.AlternateNames = htmlManga.QuerySelector("#title h3")?.TextContent.Split(';');
            formattedManga.Description = htmlManga.QuerySelector(".summary")?.TextContent;
            formattedManga.Covers = htmlManga.QuerySelectorAll("#series_info .cover img")?.Select(x => x.GetAttribute("src"));

            var textRating = htmlManga.QuerySelectorAll("#series_info > .data > span")?[2].TextContent;

            if (string.IsNullOrEmpty(textRating))
                return;

            var firstRatingIndex = textRating.IndexOf(' ') + 1;
            var lastRatinIndex = textRating.IndexOf('/') - 1;

            formattedManga.Rating = float.Parse(textRating.Substring(firstRatingIndex, lastRatinIndex - firstRatingIndex));
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
    }
}