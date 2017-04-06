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
using ProjectHeleus.MangaService.Parsers.Contracts;
using ProjectHeleus.MangaService.Parsers.Core;

namespace ProjectHeleus.MangaService.Parsers
{
    public class ReadMangaParser 
        : DefaultParser
    {
        #region Private Members

        private readonly string _newUrl;
        private readonly string _updateUrl;
        private readonly string _ratingUrl;
        private readonly string _popularUrl;

        #endregion

        public new string Url { get; set; } = "http://readmanga.me";

        public ReadMangaParser()
        {
            _newUrl = $"{Url}/list?sortType=created";
            _updateUrl = $"{Url}/list?sortType=updated";
            _ratingUrl = $"{Url}/list?sortType=votes";
            _popularUrl = $"{Url}/list?sortType=rate";
        }

        #region Overrides of IParser

        public override async Task<IEnumerable<IManga>> GetUpdateContent(int page)
        {
            return await GetListContent(_updateUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetNewContent(int page)
        {
            return await GetListContent(_newUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetRatingContent(int page)
        {
            return await GetListContent(_ratingUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetPopularContent(int page)
        {
            return await GetListContent(_popularUrl, page);
        }

        public override async Task<IManga> GetMangaContent(string mangaId)
        {
            var web = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync($"{Url}/{mangaId}");
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

        public override async Task<IEnumerable<string>> GetMangaChapterContent(string manga)
        {
            var webSource = await BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithJavaScript()).OpenAsync($"{Url}/{manga}");
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

            return images;
        }

        #endregion

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
                            Title = element.QuerySelector("h3")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            TitleAlt = element.QuerySelector("h4")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            Url = element.QuerySelector(".img a")?.GetAttribute("href"),
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

                        manga.Authors = authorsAndGenres.Take(authorsAndGenresSeparator).Select(x => new AuthorModel() {Name = x.TextContent, Url = x.GetAttribute("href")});
                        manga.Genres = authorsAndGenres.Skip(authorsAndGenresSeparator).Select(x=> new GenreModel() {Title = x.TextContent, Url = x.GetAttribute("href")});
                        
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
            formattedManga.AlternativeName = mangaRaw.QuerySelector(".names .eng-name")?.TextContent;
            formattedManga.OriginalName = mangaRaw.QuerySelector(".names .original-name")?.TextContent;
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
                int.Parse(volumeLineAfterIndex > 0
                    ? information[0].TextContent.Substring(volumeLineBeforeIndex + 1,
                        volumeLineAfterIndex - volumeLineBeforeIndex - 1)
                    : information[0].TextContent.Substring(volumeLineBeforeIndex + 1));
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
                        .Select(x => new GenreModel() { Title = x.TextContent, Url = x.GetAttribute("href") });
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
                        .Select(x => new AuthorModel() { Name = x.TextContent, Url = x.GetAttribute("href") });
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
                        .Select(x => new TranslatorModel() { Name = x.TextContent, Url = x.GetAttribute("href") });
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
                        Name =
                            Regex.Replace(chapterInfo[0].QuerySelector("a")?.TextContent, @"\s{2,}", " ").TrimStart().TrimEnd(),
                        Url = chapterInfo[0].QuerySelector("a")?.GetAttribute("href"),
                        Date = chapterInfo[1].TextContent.Replace("\n", "").Replace(" ", "")
                    });
                }
            }

            formattedManga.Chapters = chapters;
        }
    }
}