namespace ProjectHeleus.MangaService.Models
{
    using Interfaces;

    public class CatalogModel 
        : ICatalog
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }
}