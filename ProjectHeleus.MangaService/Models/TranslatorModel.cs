namespace ProjectHeleus.MangaService.Models
{
    using Interfaces;

    public class TranslatorModel 
        : ITranslator
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}