using System.Collections.Generic;

namespace ProjectHeleus.MangaService.Models.Mangas
{
    public class ListManga
    {
        public string Title { get; set; }
        public string TitleAlt { get; set; }
        public string Url { get; set; }
        public string Cover { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public float RatingLimit { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }
}