using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;
using ProjectHeleus.MangaService.Parsers.Core;

namespace ProjectHeleus.MangaService.Parsers
{
    public class ReadMangaParser 
        : DefaultParser
    {
        #region Protected Members

        protected string NewUrl;
        protected string UpdateUrl;
        protected string RatingUrl;
        protected string PopularUrl;

        #endregion

        public new string Url { get; set; } = "http://readmanga.me";

        public ReadMangaParser()
        {
            NewUrl = $"{Url}/list?sortType=created";
            UpdateUrl = $"{Url}/list?sortType=updated";
            RatingUrl = $"{Url}/list?sortType=votes";
            PopularUrl = $"{Url}/list?sortType=rate";
        }

        #region Overrides of IParser

        public override async Task<IEnumerable<IManga>> GetUpdateContent(int page)
        {
            return await GetListContent(UpdateUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetNewContent(int page)
        {
            return await GetListContent(NewUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetRatingContent(int page)
        {
            return await GetListContent(RatingUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetPopularContent(int page)
        {
            return await GetListContent(PopularUrl, page);
        }

        public override async Task<IManga> GetMangaContent(string manga)
        {
            var web = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync($"{Url}/{manga}");
            var mangaRaw = web.QuerySelector(".leftContent");

            if (mangaRaw == null)
                return null;

            var formattedManga = new MangaModel();

            GetInformation(formattedManga, mangaRaw);

            var information = mangaRaw.QuerySelectorAll(".subject-meta.col-sm-7 p");
            if (information.Any())
            {
                GetVolume(information, formattedManga);
                GetViews(information, formattedManga);

                foreach (var informationLine in information.Skip(2))
                {
                    if (GetGenres(informationLine, formattedManga))
                        continue;

                    if (GetAuthors(informationLine, formattedManga))
                        continue;

                    if (GetTranslators(informationLine, formattedManga))
                        continue;

                    GetPublishedYear(informationLine, formattedManga);
                }
            }


            GetChapters(mangaRaw, formattedManga);

            return formattedManga;
        }

        public override async Task<IChapterContent> GetMangaChapterContent(string manga)
        {
            using (var webSource = await BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithJavaScript()).OpenAsync($"{Url}/{manga}"))
            {
                var mangaImagesWeb = webSource.QuerySelectorAll("script").FirstOrDefault(x => x.TextContent.Contains("rm_h.init"));

                var images = new List<string>();

                #region MangaImages

                if (mangaImagesWeb != null)
                {
                    var source = mangaImagesWeb.TextContent.Substring(mangaImagesWeb.TextContent.IndexOf("rm_h.init"));
                    var formattedSource = source.Substring(source.IndexOf("(")).TrimEnd();

                    var startBracket = formattedSource.IndexOf("[");
                    var endBracket = formattedSource.LastIndexOf("]");

                    var formattedSourceToRegex = formattedSource.Substring(startBracket + 1, endBracket - startBracket - 1);
                    
                   
                    foreach (Match match in new Regex(@"\[(.*?)\]").Matches(formattedSourceToRegex))
                    {
                        var image = match.Groups[1].Value;

                        if (!string.IsNullOrEmpty(image))
                        {
                            var imageAttributes = image.Split(',');

                            images.Add(
                                $@"{imageAttributes[1].Replace("\'", "")}{imageAttributes[0].Replace("\'", "")}{imageAttributes
                                    [2].Replace("\"", "")}"
                            );
                        }
                    }
                }

                #endregion

                return new ChapterContentModel() {Images = images};
            }
        }

        #endregion

        #region Internal Methods

        private async Task<IEnumerable<IManga>> GetListContent(string url, int page)
        {
            if (page > 0)
                url = $"{url}&offset={70 * page}&max=70";

            var webSource = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(url);
            var mangas = webSource.QuerySelectorAll(".tile.col-sm-6");

            var mangasResult = new List<MangaShortModel>();

            if (mangas.Any())
            {
                try
                {
                    foreach (var element in mangas)
                    {
                        #region Header

                        var manga = new MangaShortModel
                        {
                            Id = element.QuerySelector(".img a")?.GetAttribute("href").Replace("/", ""),
                            Title = element.QuerySelector("h3")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            TitleAlt = element.QuerySelector("h4")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            Url = $@"{Url}{element.QuerySelector(".img a")?.GetAttribute("href")}",
                            Cover = element.QuerySelector(".img a img")?.GetAttribute("src")
                        };

                        #endregion

                        #region Rating

                        var ratings = element.QuerySelector(".desc .rating")?.GetAttribute("title").Split(' ');

                        if (ratings != null)
                        {
                            manga.Rating = float.Parse(ratings[0]);
                            manga.RatingLimit = float.Parse(ratings[2]);
                        }

                        #endregion

                        #region Views

                        var views = element.QuerySelector(".tile-info p");
                        if (views != null)
                            manga.Views = int.Parse(views?.TextContent.Substring(views.TextContent.LastIndexOf(' ')));

                        #endregion

                        #region AuthorsAndGenres

                        var tileInfo = element.QuerySelector(".tile-info");
                        
                        var authorsAndGenres = tileInfo.QuerySelectorAll("a");
                        var authorsAndGenresSeparator = tileInfo.Children.Index(tileInfo.ChildNodes.FirstOrDefault(x => x is IHtmlBreakRowElement));

                        manga.Authors = authorsAndGenres.Take(authorsAndGenresSeparator).Select(x => new AuthorModel() { Id = x.GetAttribute("href").Substring(x.GetAttribute("href").LastIndexOf('/') + 1), Name = x.TextContent, Url = $@"{Url}{ x.GetAttribute("href")}"});
                        manga.Genres = authorsAndGenres.Skip(authorsAndGenresSeparator).Select(x=> new GenreModel() { Id = x.GetAttribute("href").Substring(x.GetAttribute("href").LastIndexOf('/') + 1), Title = x.TextContent, Url = $@"{Url}{ x.GetAttribute("href")}"});
                        
                        #endregion

                        mangasResult.Add(manga);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return mangasResult;
        }

        private void GetInformation(MangaModel formattedManga, IElement mangaRaw)
        {
            formattedManga.Name = mangaRaw.QuerySelector(".names .name")?.TextContent;
            formattedManga.AlternateNames = new[]
            {
                mangaRaw.QuerySelector(".names .eng-name")?.TextContent,
                mangaRaw.QuerySelector(".names .original-name")?.TextContent
            };
            formattedManga.Description = mangaRaw.QuerySelector("meta[itemprop=description]")?.GetAttribute("content");
            formattedManga.Covers =
                mangaRaw.QuerySelectorAll(".picture-fotorama img")?.Select(x => new Uri(x.GetAttribute("data-full")));
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
        }
        private void GetViews(IHtmlCollection<IElement> information, MangaModel formattedManga)
        {
            var viewsIndex = information[1].TextContent.IndexOf(":", StringComparison.Ordinal);

            if (viewsIndex > 0)
                formattedManga.Views = int.Parse(information[1].TextContent.Substring(viewsIndex + 1));
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
            var webChapters = mangaRaw.QuerySelectorAll(".expandable.chapters-link tbody tr");

            var chapters = new List<ChapterModel>();
            foreach (var chapter in webChapters)
            {
                var chapterInfo = chapter.QuerySelectorAll("td");

                if (chapterInfo.Any())
                {
                    chapters.Add(new ChapterModel()
                    {
                        Id = chapterInfo[0].QuerySelector("a")?.GetAttribute("href").Substring(1),
                        Name =
                            Regex.Replace(chapterInfo[0].QuerySelector("a")?.TextContent, @"\s{2,}", " ").TrimStart().TrimEnd(),
                        Url = $@"{Url}{chapterInfo[0].QuerySelector("a")?.GetAttribute("href")}",
                        Date = chapterInfo[1].TextContent.Replace("\n", "").Replace(" ", "")
                    });
                }
            }

            formattedManga.Chapters = chapters;
        }

        #endregion
    }
}