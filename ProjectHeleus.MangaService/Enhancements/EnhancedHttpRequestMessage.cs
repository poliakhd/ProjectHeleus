namespace ProjectHeleus.MangaService.Enhancements
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class EnhancedHttpRequestMessage : HttpRequestMessage
    {
        public EnhancedHttpRequestMessage(HttpMethod method, string requestUri)
            : base(method, requestUri)
        {
            PrepareHttpRequest();
        }

        public EnhancedHttpRequestMessage(HttpMethod method, Uri requestUri)
            : base(method, requestUri)
        {
            PrepareHttpRequest();
        }

        private void PrepareHttpRequest()
        {
            //85.143.218.246:3128

            Headers.Clear();

            Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

            Headers.TryAddWithoutValidation("Content-Type", "text/html; charset=utf-8");
            Headers.TryAddWithoutValidation("Accept-Language", "ru-RU");
            Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246");
        }
    }
}