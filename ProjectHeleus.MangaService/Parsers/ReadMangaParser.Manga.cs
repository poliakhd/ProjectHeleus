namespace ProjectHeleus.MangaService.Parsers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    
    using AngleSharp.Dom;
    using AngleSharp.Parser.Html;

    using Microsoft.Extensions.Logging;

    using Extensions;
    using Enhancements;
    using Shared.Models;
    using Shared.Models.Interfaces;

    public partial class ReadMangaParser
    {
        #region Implementation of IMangaParser

        public async Task<IManga> GetCatalogMangaAsync(string url)
        {
            #region Build URL

            url = $"{Url}/{url}";

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
                            var htmlManga = htmlDocument.QuerySelector(".leftContent");

                            if (htmlManga == null)
                                return null;

                            GetInformation(parsedManga, htmlManga);

                            var information = htmlManga.QuerySelectorAll(".subject-meta.col-sm-7 p");

                            if (information.Any())
                            {
                                GetVolume(information, parsedManga);
                                GetViews(information, parsedManga);

                                foreach (var informationLine in information.Skip(2))
                                {
                                    if (GetGenres(informationLine, parsedManga))
                                        continue;

                                    if (GetAuthors(informationLine, parsedManga))
                                        continue;

                                    if (GetTranslators(informationLine, parsedManga))
                                        continue;

                                    GetPublishedYear(informationLine, parsedManga);
                                }
                            }

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
        public virtual async Task<IChapterImages> GetMangaChapterImagesAsync(string url)
        {
            #region Build URL

            url = $"{Url}/{url}";

            #endregion

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
                            var htmlChapterImages =
                            htmlDocument.QuerySelectorAll("script")
                                .FirstOrDefault(x => x.TextContent.Contains("rm_h.init"));
                            if (htmlChapterImages == null)
                                return null;

                            string formattedImagesSource;

                            #region Preparing images source

                            var jsImagesSource =
                                htmlChapterImages.TextContent.Substring(htmlChapterImages.TextContent.IndexOf("rm_h.init"));

                            var preFormattedImagesSource = jsImagesSource.Substring(jsImagesSource.IndexOf("(")).TrimEnd();

                            var startBracket = preFormattedImagesSource.IndexOf("[");
                            var endBracket = preFormattedImagesSource.LastIndexOf("]");

                            formattedImagesSource = preFormattedImagesSource.Substring(startBracket + 1,
                                endBracket - startBracket - 1);

                            #endregion

                            if (string.IsNullOrEmpty(formattedImagesSource))
                            {
                                _logger.LogInformation(
                                    $"Cannot retrive images for chapter: Url - {url}; jsImagesSource: {jsImagesSource}; preFormattedImagesSource: {preFormattedImagesSource};");
                                return null;
                            }

                            var localImagesHrefs = new List<string>();

                            foreach (Match match in new Regex(@"\[(.*?)\]").Matches(formattedImagesSource))
                            {
                                var image = match.Groups[1].Value;

                                if (string.IsNullOrEmpty(image))
                                    continue;

                                var imageAttributes = image.Split(',');
                                localImagesHrefs.Add(
                                    $@"{imageAttributes[1].Replace("\'", "")}{imageAttributes[0].Replace("\'", "")}{imageAttributes
                                        [2].Replace("\"", "")}");
                            }

                            return new ChapterImagesModel { Images = localImagesHrefs };
                        }
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get manga chapter content from: {url}");
                _logger.LogError(e.Message);

                throw new HttpRequestException(e.Message);
            }
        }

        #endregion

        private void GetInformation(MangaModel formattedManga, IElement mangaRaw)
        {
            formattedManga.Id = mangaRaw.QuerySelector("meta[itemprop=url]")?.GetAttribute("content").Replace(Url + @"/", "");
            formattedManga.Name = mangaRaw.QuerySelector(".names .name")?.TextContent;
            formattedManga.AlternateNames = new[]
            {
                mangaRaw.QuerySelector(".names .eng-name")?.TextContent,
                mangaRaw.QuerySelector(".names .original-name")?.TextContent
            }.Where(x => x != null);
            formattedManga.Description = mangaRaw.QuerySelector("meta[itemprop=description]")?.GetAttribute("content");
            formattedManga.Covers =
                mangaRaw.QuerySelectorAll(".picture-fotorama img")?.Select(x => x.GetAttribute("data-full"));
            formattedManga.Rating =
                float.Parse(mangaRaw.QuerySelector(".user-rating meta[itemprop=ratingValue]")?.GetAttribute("content"));
        }
        private void GetVolume(IHtmlCollection<IElement> information, MangaModel formattedManga)
        {
            var volumeLineBeforeIndex = information[0].TextContent.IndexOf(":", StringComparison.Ordinal);
            var volumeLineAfterIndex = information[0].TextContent.IndexOf(",", StringComparison.Ordinal);

            formattedManga.Volumes =
                volumeLineAfterIndex > 0
                    ? information[0].TextContent.Substring(volumeLineBeforeIndex + 1,
                        volumeLineAfterIndex - volumeLineBeforeIndex - 1)
                    : information[0].TextContent.Substring(volumeLineBeforeIndex + 1);
            formattedManga.Volumes = formattedManga.Volumes.Trim();
        }
        private void GetViews(IHtmlCollection<IElement> information, MangaModel formattedManga)
        {
            if (information.Length > 6)
            {
                var viewsIndex = information[2].TextContent.IndexOf(":", StringComparison.Ordinal);

                if (viewsIndex > 0)
                    formattedManga.Views = int.Parse(information[2].TextContent.Substring(viewsIndex + 1));
            }
            else
                formattedManga.Views = 0;
        }
        private bool GetGenres(IElement informationLine, MangaModel formattedManga)
        {
            if (informationLine.QuerySelector(".elem_genre") != null)
            {
                formattedManga.Genres =
                    informationLine.QuerySelectorAll(".elem_genre a")
                        .Select(x => new GenreModel() { Id = x.GetAttribute("href").Substring(x.GetAttribute("href").LastIndexOf('/') + 1), Title = x.TextContent, Url = $@"{Url}{x.GetAttribute("href")}" });
                return true;
            }
            return false;
        }
        private bool GetAuthors(IElement informationLine, MangaModel formattedManga)
        {
            if (informationLine.QuerySelector(".elem_author") != null)
            {
                formattedManga.Authors =
                    informationLine.QuerySelectorAll(".elem_author a")
                        .Select(x => new AuthorModel() { Id = x.GetAttribute("href")?.Substring(x.GetAttribute("href").LastIndexOf('/') + 1), Name = x.TextContent, Url = $@"{Url}{x.GetAttribute("href")}" });
                return true;
            }
            return false;
        }
        private bool GetTranslators(IElement informationLine, MangaModel formattedManga)
        {
            if (informationLine.QuerySelector(".elem_translator") != null)
            {
                formattedManga.Translators =
                    informationLine.QuerySelectorAll(".elem_translator a")
                        .Where(x => !string.IsNullOrEmpty(x.TextContent))
                        .Select(x => new TranslatorModel() { Id = x.GetAttribute("href").Substring(x.GetAttribute("href").LastIndexOf('/') + 1), Name = x.TextContent, Url = $@"{Url}{x.GetAttribute("href")}" });
                return true;
            }
            return false;
        }
        private void GetPublishedYear(IElement informationLine, MangaModel formattedManga)
        {
            if (informationLine.QuerySelector(".elem_year") != null)
                formattedManga.Published = int.Parse(informationLine.QuerySelector(".elem_year a")?.TextContent);
        }
        private void GetChapters(IElement mangaRaw, MangaModel formattedManga)
        {
            var formattedChapters = new List<ChapterModel>();
            var htmlChapters = mangaRaw.QuerySelectorAll(".expandable.chapters-link tbody tr");

            foreach (var chapter in htmlChapters)
            {
                var chapterInfo = chapter.QuerySelectorAll("td");

                if (chapterInfo.Any())
                {
                    formattedChapters.Add(new ChapterModel()
                    {
                        Id = chapterInfo[0].QuerySelector("a")?.GetAttribute("href").Substring(1),
                        Name = Regex.Replace(chapterInfo[0].QuerySelector("a")?.TextContent, @"\s{2,}", " ").TrimStart().TrimEnd(),
                        Url = $@"{Url}{chapterInfo[0].QuerySelector("a")?.GetAttribute("href")}",
                        Date = chapterInfo[1].TextContent.Replace("\n", "").Replace(" ", "")
                    });
                }
            }

            formattedManga.Chapters = formattedChapters;
        }
    }
}