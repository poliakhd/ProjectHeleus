namespace ProjectHeleus.MangaService.Extensions
{
    using System.Net.Http;

    public static class HttpExtensions
    {
        public static string GetStringContent(this HttpResponseMessage httpResponse)
        {
            return httpResponse.Content.ReadAsStringAsync().Result;
        }
    }
}