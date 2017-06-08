namespace ProjectHeleus.Shared.Models
{
    using Interfaces;

    public class CatalogModel 
        : ICatalog
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
    }
}