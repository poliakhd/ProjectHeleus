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
            var mangaWeb = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync($"{Url}/{mangaId}");
            var mangaWebSource = mangaWeb.GetElementsByClassName("leftContent");

            var manga = new Manga();

            if (mangaWebSource.Any())
            {
                var mangaSource = mangaWebSource[0];

                #region Names

                var names = mangaSource.GetElementsByClassName("names");

                if (names.Any())
                {
                    var name = names[0].GetElementsByClassName("name");
                    if (name.Any())
                        manga.Name = name[0].TextContent;

                    var nameAlternative = names[0].GetElementsByClassName("eng-name");
                    if (nameAlternative.Any())
                        manga.AlternativeName = nameAlternative[0].TextContent;

                    var nameOriginal = names[0].GetElementsByClassName("original-name");
                    if (nameOriginal.Any())
                        manga.OriginalName = nameOriginal[0].TextContent;
                }

                #endregion

                #region Description

                var description = mangaSource.GetElementsByTagName("meta");
                var descriptionMeta = description.FirstOrDefault(x => x.GetAttribute("itemprop") == "description");

                if (descriptionMeta != null)
                {
                    manga.Description = descriptionMeta.GetAttribute("content");
                }

                #endregion

                #region Covers

                var covers = mangaSource.GetElementsByClassName("picture-fotorama");

                if (covers.Any())
                {
                    var coversList = covers[0].GetElementsByTagName("img");

                    if (coversList.Any())
                    {
                        var coversUriList = new List<Uri>();

                        foreach (var cover in coversList)
                            coversUriList.Add(new Uri(cover.GetAttribute("data-full")));

                        manga.Covers = coversUriList;
                    }
                }

                #endregion

                #region Rating

                var rating = mangaSource.GetElementsByClassName("user-rating");

                if (rating.Any())
                {
                    var ratings = rating[0].GetElementsByTagName("meta");

                    if (ratings.Any())
                    {
                        var ratingValue = ratings.FirstOrDefault(x => x.GetAttribute("itemprop") == "ratingValue");

                        if (ratingValue != null)
                            manga.Rating = float.Parse(ratingValue.GetAttribute("content"));
                    }
                }

                #endregion

                #region Information

                var info = mangaSource.GetElementsByClassName("subject-meta col-sm-7");

                if (info.Any())
                {
                    var lines = info[0].GetElementsByTagName("p");

                    #region Volume

                    var volumeLineBeforeIndex = lines[0].TextContent.IndexOf(":");
                    var volumeLineAfterIndex = lines[0].TextContent.IndexOf(",");

                    manga.Volumes = int.Parse(volumeLineAfterIndex > 0 ? lines[0].TextContent.Substring(volumeLineBeforeIndex + 1, volumeLineAfterIndex - volumeLineBeforeIndex - 1) : lines[0].TextContent.Substring(volumeLineBeforeIndex + 1));

                    #endregion

                    #region Views

                    var viewsIndex = lines[1].TextContent.IndexOf(":", StringComparison.Ordinal);

                    if (viewsIndex > 0)
                        manga.Views = int.Parse(lines[1].TextContent.Substring(viewsIndex + 1));

                    #endregion

                    foreach (var line in lines.Skip(2))
                    {
                        #region Genres

                        var genres = line.GetElementsByClassName("elem_genre");

                        if (genres.Any())
                        {
                            var genresList = new List<Genre>();

                            foreach (var genre in genres)
                            {
                                var genreLink = genre.GetElementsByTagName("a");

                                if (genreLink.Any())
                                {
                                    genresList.Add(new Genre()
                                    {
                                        Title = genreLink[0].TextContent,
                                        Url = genreLink[0].GetAttribute("href")
                                    });
                                }
                            }

                            manga.Genres = genresList;
                            continue;
                        }

                        #endregion

                        #region Authors

                        var authors = line.GetElementsByClassName("elem_author");

                        if (authors.Any())
                        {
                            var authorsList = new List<Author>();

                            foreach (var author in authors)
                            {
                                var authorLink = author.GetElementsByTagName("a");

                                if (authorLink.Any())
                                {
                                    authorsList.Add(new Author()
                                    {
                                        Name = authorLink[0].TextContent,
                                        Url = authorLink[0].GetAttribute("href")
                                    });
                                }
                            }

                            manga.Authors = authorsList;
                            continue;
                        }

                        #endregion

                        #region Translators

                        var translators = line.GetElementsByClassName("elem_translator");

                        if (translators.Any())
                        {
                            var translatorsList = new List<Translator>();

                            foreach (var translator in translators)
                            {
                                var translatorLink = translator.GetElementsByTagName("a");

                                if (translatorLink.Any())
                                {
                                    translatorsList.Add(new Translator()
                                    {
                                        Name = translatorLink[0].TextContent,
                                        Url = translatorLink[0].GetAttribute("href")
                                    });
                                }
                            }

                            manga.Translators = translatorsList;
                            continue;
                        }

                        #endregion

                        #region Published

                        var published = line.GetElementsByClassName("elem_year");

                        if (published.Any())
                        {
                            manga.Published = int.Parse(published[0].GetElementsByTagName("a")[0].TextContent);
                            continue;
                        }

                        #endregion
                    }
                }

                #endregion

                #region Chapters
                
                var chaptersTable = mangaSource.GetElementsByClassName("expandable chapters-link");
                var mangaChapters = new List<MangaChapter>();

                if (chaptersTable.Any())
                {
                    var chaptersBody = chaptersTable[0].GetElementsByTagName("tbody");

                    if (chaptersBody.Any())
                    {
                        var chaptersRows = chaptersBody[0].GetElementsByTagName("tr");

                        if (chaptersRows.Any())
                        {
                            foreach (var chaptersRow in chaptersRows)
                            {
                                var chapterColumn = chaptersRow.GetElementsByTagName("td");

                                if (chapterColumn.Any())
                                {
                                    var url  = chapterColumn[0].GetElementsByTagName("a")[0].GetAttribute("href");
                                    var name = Regex.Replace(chapterColumn[0].GetElementsByTagName("a")[0].TextContent, @"\s{2,}", " ").TrimStart().TrimEnd();
                                    var date = chapterColumn[1].TextContent.Replace("\n","").Replace(" ", "");

                                    mangaChapters.Add(new MangaChapter()
                                    {
                                        Name = name,
                                        Url = url,
                                        Date = date
                                    });
                                }
                            }
                        }
                    }
                }

                manga.Chapters = mangaChapters;

                #endregion
            }

            return manga;
        }
        public async Task<IEnumerable<string>> GetMangaChapterContent(string mangaId)
        {
            var images = new List<string>();

            var mangaWeb = await BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithJavaScript()).OpenAsync($"{Url}/{mangaId}");
            var mangaImages = mangaWeb.GetElementsByTagName("script").FirstOrDefault(x => x.TextContent.Contains("rm_h.init"));

            if (mangaImages != null)
            {
                var source = mangaImages.TextContent.Substring(mangaImages.TextContent.IndexOf("rm_h.init"));
                
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

            return images;
        }

        #endregion

        private async Task<IEnumerable<ListManga>> GetListContent(string sourceUrl, int page)
        {
            if (page > 0)
                sourceUrl = $"{sourceUrl}&offset={70 * page}&max=70";

            var source = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(sourceUrl);
            var list = source.GetElementsByClassName("tile col-sm-6");

            var mangas = new List<ListManga>();

            if (list.Any())
            {
                try
                {
                    foreach (var element in list)
                    {
                        var manga = new ListManga();

                        #region Title

                        var title = element.GetElementsByTagName("h3");

                        if (title.Any())
                            manga.Title = title[0].TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' ');

                        #endregion

                        #region TitleAlt

                        var titleAlt = element.GetElementsByTagName("h4");

                        if (titleAlt.Any())
                            manga.TitleAlt = titleAlt[0].TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' ');

                        #endregion

                        #region Url

                        var url = element.GetElementsByClassName("img");

                        if (url.Any())
                        {
                            url = url[0].GetElementsByTagName("a");

                            if (url.Any())
                                manga.Url = url[0].GetAttribute("href");
                        }

                        #endregion

                        #region Cover

                        var imageUrl = element.GetElementsByClassName("img");

                        if (imageUrl.Any())
                        {
                            imageUrl = imageUrl[0].GetElementsByTagName("a");

                            if (imageUrl.Any())
                            {
                                imageUrl = imageUrl[0].GetElementsByTagName("img");

                                if (imageUrl.Any())
                                {
                                    manga.Cover = imageUrl[0].GetAttribute("src");
                                }
                            }
                        }

                        #endregion

                        #region Rating

                        var rating = element.GetElementsByClassName("desc");

                        if (rating.Any())
                        {
                            rating = rating[0].GetElementsByClassName("rating");

                            if (rating.Any())
                            {
                                var ratingStatus = rating[0].GetAttribute("title").Split(' ');

                                manga.Rating = float.Parse(ratingStatus[0]);
                                manga.RatingLimit = float.Parse(ratingStatus[2]);
                            }
                        }

                        #endregion

                        #region TileInfo

                        var tileInfo = element.GetElementsByClassName("tile-info");

                        #region Views

                        if (tileInfo.Any())
                        {
                            var views = tileInfo[0].GetElementsByTagName("p");

                            if (views.Any())
                            {
                                manga.Views =
                                    int.Parse(views[0].TextContent.Substring(views[0].TextContent.LastIndexOf(' ')));
                            }
                        }

                        #endregion

                        #region Authors

                        if (tileInfo.Any())
                        {
                            var brIndex =
                                tileInfo[0].Children.Index(
                                    tileInfo[0].ChildNodes.FirstOrDefault(x => x is IHtmlBreakRowElement));

                            var authors = tileInfo[0].GetElementsByTagName("a");

                            if (authors.Any())
                            {
                                var authorsLocal = new List<Author>();

                                foreach (var author in authors.Take(brIndex))
                                    authorsLocal.Add(new Author()
                                    {
                                        Name = author.TextContent,
                                        Url = author.GetAttribute("href")
                                    });

                                manga.Authors = authorsLocal;
                            }
                        }

                        #endregion

                        #region Genres

                        if (tileInfo.Any())
                        {
                            var brIndex =
                                tileInfo[0].Children.Index(
                                    tileInfo[0].ChildNodes.FirstOrDefault(x => x is IHtmlBreakRowElement));

                            var genres = tileInfo[0].GetElementsByTagName("a");

                            if (genres.Any())
                            {
                                var genresLocal = new List<Genre>();

                                foreach (var genre in genres.Skip(brIndex))
                                    genresLocal.Add(new Genre()
                                    {
                                        Title = genre.TextContent,
                                        Url = genre.GetAttribute("href")
                                    });

                                manga.Genres = genresLocal;
                            }
                        }

                        #endregion

                        #endregion

                        mangas.Add(manga);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return mangas;
        }
    }
}