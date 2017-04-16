namespace ProjectHeleus.MangaApp.Core
{
    using System.Collections.Generic;
    using Caliburn.Micro;

    public static class Extensions
    {
        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> collection)
        {
            return new BindableCollection<T>(collection);
        }
    }
}