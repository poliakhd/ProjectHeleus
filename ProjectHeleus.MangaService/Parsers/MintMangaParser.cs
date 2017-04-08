namespace ProjectHeleus.MangaService.Parsers
{
    public class MintMangaParser 
        : ReadMangaParser
    {
        public new string Url { get; set; } = "http://mintmanga.com";

        public MintMangaParser()
        {
            NewUrl = $"{Url}/list?sortType=created";
            UpdateUrl = $"{Url}/list?sortType=updated";
            RatingUrl = $"{Url}/list?sortType=votes";
            PopularUrl = $"{Url}/list?sortType=rate";
        }
    }
}