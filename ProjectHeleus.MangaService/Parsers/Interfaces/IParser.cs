namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    public interface IParser 
        : ICatalogParser, IMangaParser, IGenreParser
    {
        string Url { get; set; }
    }
}