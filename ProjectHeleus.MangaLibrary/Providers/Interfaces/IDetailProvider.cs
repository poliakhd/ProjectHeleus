namespace ProjectHeleus.MangaLibrary.Providers.Interfaces
{
    using System.Threading.Tasks;
    using Shared.Models;

    public interface IDetailProvider : IBaseProvider
    {
        Task<ProviderRespose<MangaModel>> GetMangaContent(CatalogModel catalog, MangaPreviewModel mangaPreview);

        Task<ProviderRespose<ChapterImagesModel>> GetMangaChapterContent(CatalogModel catalog, MangaModel manga, ChapterModel chapter);
    }
}