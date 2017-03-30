using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectHeleus.MangaApp.Models
{
    public class Catalog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}