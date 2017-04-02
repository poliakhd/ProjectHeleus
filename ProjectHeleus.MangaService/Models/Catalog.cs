using System.ComponentModel.DataAnnotations;

namespace ProjectHeleus.MangaService.Models
{
    public class Catalog
    {
        [Key]
        public string Title { get; set; }
        public string Url { get; set; }
    }
}