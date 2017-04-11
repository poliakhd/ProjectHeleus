namespace ProjectHeleus.MangaService.Models
{
    using Interfaces;

    public class AuthorModel
        : IAuthor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}