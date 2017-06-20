namespace ProjectHeleus.MangaLibrary.Providers.Interfaces
{
    public enum ErrorType
    {
        None,
        NetworkError,
        ServiceError,
        Unknown
    }

    public class ProviderRespose<T>
    {
        public bool HasError { get; set; }
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }

        public bool HasResponse { get; set; }
        public T Value { get; set; }
    }

    public interface IBaseProvider
    {
        string Url { get; set; }
    }
}