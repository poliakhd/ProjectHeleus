using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Mangas;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class ReadMangaParcer 
        : IParser
    {
        #region Private Members

        private readonly string _newUrl;
        private readonly string _updateUrl;
        private readonly string _ratingUrl;

        #endregion

        public string Url { get; set; } = "http://readmanga.me";

        public ReadMangaParcer()
        {
            _newUrl = $"{Url}/list?sortType=created";
            _updateUrl = $"{Url}/list?sortType=updated";
            _ratingUrl = $"{Url}/list?sortType=votes";
        }

        #region Implementation of IParser

        public async Task<IEnumerable<ListManga>> GetUpdateContent(int page)
        {
            return await GetListContent(_updateUrl, page);
        }
        public async Task<IEnumerable<ListManga>> GetNewContent(int page)
        {
            return await GetListContent(_newUrl, page);
        }
        public async Task<IEnumerable<ListManga>> GetRatingContent(int page)
        {
            return await GetListContent(_ratingUrl, page);
        }

        public async Task<Manga> GetMangaContent(string mangaId)
        {
            var webSource = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync($"{Url}/{mangaId}");
            var mangaWeb = webSource.QuerySelector(".leftContent");

            var manga = new Manga();

            #region Manga

            if (mangaWeb != null)
            {
                manga.Name = mangaWeb.QuerySelector(".names .name")?.TextContent;
                manga.AlternativeName = mangaWeb.QuerySelector(".names .eng-name")?.TextContent;
                manga.OriginalName = mangaWeb.QuerySelector(".names .original-name")?.TextContent;
                manga.Description = mangaWeb.QuerySelector("meta[itemprop=description]")?.GetAttribute("content");
                manga.Covers = mangaWeb.QuerySelectorAll(".picture-fotorama img")?.Select(x => new Uri(x.GetAttribute("data-full")));
                manga.Rating = float.Parse(mangaWeb.QuerySelector(".user-rating meta[itemprop=ratingValue]")?.GetAttribute("content"));

                var information = mangaWeb.QuerySelectorAll(".subject-meta.col-sm-7 p");
                if (information.Any())
                {
                    #region Volume

                    var volumeLineBeforeIndex = information[0].TextContent.IndexOf(":", StringComparison.Ordinal);
                    var volumeLineAfterIndex = information[0].TextContent.IndexOf(",", StringComparison.Ordinal);

                    manga.Volumes = int.Parse(volumeLineAfterIndex > 0 ? information[0].TextContent.Substring(volumeLineBeforeIndex + 1, volumeLineAfterIndex - volumeLineBeforeIndex - 1) : information[0].TextContent.Substring(volumeLineBeforeIndex + 1));

                    #endregion

                    #region Views

                    var viewsIndex = information[1].TextContent.IndexOf(":", StringComparison.Ordinal);

                    if (viewsIndex > 0)
                        manga.Views = int.Parse(information[1].TextContent.Substring(viewsIndex + 1));

                    #endregion

                    foreach (var informationLine in information.Skip(2))
                    {
                        #region Genres

                        if (informationLine.QuerySelector(".elem_genre") != null)
                        {
                            manga.Genres = informationLine.QuerySelectorAll(".elem_genre a").Select(x => new Genre() { Title = x.TextContent, Url = x.GetAttribute("href") });
                            continue;
                        }

                        #endregion

                        #region Authors

                        if (informationLine.QuerySelector(".elem_author") != null)
                        {
                            manga.Authors = informationLine.QuerySelectorAll(".elem_author a").Select(x => new Author() { Name = x.TextContent, Url = x.GetAttribute("href") });
                            continue;
                        }

                        #endregion

                        #region Translators

                        if (informationLine.QuerySelector(".elem_translator") != null)
                        {
                            manga.Translators = informationLine.QuerySelectorAll(".elem_translator a").Where(x => !string.IsNullOrEmpty(x.TextContent)).Select(x => new Translator() { Name = x.TextContent, Url = x.GetAttribute("href") });
                            continue;
                        }

                        #endregion

                        #region Published

                        if (informationLine.QuerySelector(".elem_year") != null)
                            manga.Published = int.Parse(informationLine.QuerySelector(".elem_year a")?.TextContent);

                        #endregion
                    }
                }

                #region Chapters

                var webChapters = mangaWeb.QuerySelectorAll(".expandable.chapters-link tbody tr");

                var chapters = new List<MangaChapter>();
                foreach (var chapter in webChapters)
                {
                    var chapterInfo = chapter.QuerySelectorAll("td");

                    if (chapterInfo.Any())
                    {
                        chapters.Add(new MangaChapter()
                        {
                            Name = Regex.Replace(chapterInfo[0].QuerySelector("a")?.TextContent, @"\s{2,}", " ").TrimStart().TrimEnd(),
                            Url = chapterInfo[0].QuerySelector("a")?.GetAttribute("href"),
                            Date = chapterInfo[1].TextContent.Replace("\n", "").Replace(" ", "")
                        });
                    }
                }

                manga.Chapters = chapters;

                #endregion
            }

            #endregion

            return manga;
        }
        public async Task<IEnumerable<string>> GetMangaChapterContent(string manga)
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

        private async Task<IEnumerable<ListManga>> GetListContent(string url, int page)
        {
            if (page > 0)
                url = $"{url}&offset={70 * page}&max=70";

            var webSource = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(url);
            var mangas = webSource.QuerySelectorAll(".tile.col-sm-6");

            var mangasResult = new List<ListManga>();

            if (mangas.Any())
            {
                try
                {
                    foreach (var element in mangas)
                    {
                        #region Header

                        var manga = new ListManga
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

                        manga.Authors = authorsAndGenres.Take(authorsAndGenresSeparator).Select(x => new Author() {Name = x.TextContent, Url = x.GetAttribute("href")});
                        manga.Genres = authorsAndGenres.Skip(authorsAndGenresSeparator).Select(x=> new Genre() {Title = x.TextContent, Url = x.GetAttribute("href")});
                        
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
    }
}