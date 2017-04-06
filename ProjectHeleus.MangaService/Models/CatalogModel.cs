using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Models
{
    public class CatalogModel : ICatalog
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }
}