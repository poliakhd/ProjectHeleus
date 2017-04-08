using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Models
{
    public class GenreModel : IGenre
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}