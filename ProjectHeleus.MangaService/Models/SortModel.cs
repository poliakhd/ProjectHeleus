namespace ProjectHeleus.MangaService.Models
{
    using Interfaces;

    public class SortModel : ISort
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}