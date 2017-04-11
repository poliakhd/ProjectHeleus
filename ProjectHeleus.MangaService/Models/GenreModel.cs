namespace ProjectHeleus.MangaService.Models
{
    using Interfaces;

    public class GenreModel 
        : IGenre
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}