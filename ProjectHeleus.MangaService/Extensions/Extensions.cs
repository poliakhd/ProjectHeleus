using System;
using Microsoft.AspNetCore.Mvc;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Parsers;
using ProjectHeleus.MangaService.Parsers.Contracts;
using StructureMap;

namespace ProjectHeleus.MangaService.Extensions
{
    public static class Extensions
    {
        public static IParser GetParser(this IContainer container, CatalogType catalogType)
        {
            IParser parser = null;
            
            switch (catalogType)
            {
                case CatalogType.MangaFox:
                    parser = container.GetInstance<IParser>(nameof(MangaFoxParser));
                    break;
                case CatalogType.ReadManga:
                    parser = container.GetInstance<IParser>(nameof(ReadMangaParser));
                    break;
                case CatalogType.MintManga:
                    parser = container.GetInstance<IParser>(nameof(MintMangaParser));
                    break;
            }

            return parser;
        }

        public static CatalogType GetCatalogType(this string catalog)
        {
            switch (catalog)
            {
                case "readmanga.me":
                    return CatalogType.ReadManga;
                case "mangafox.me":
                    return CatalogType.MangaFox;
                case "mintmanga.com":
                    return CatalogType.MintManga;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}