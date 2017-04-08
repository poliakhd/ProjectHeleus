using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Models
{
    public class TranslatorModel : ITranslator
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}