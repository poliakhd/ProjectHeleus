using System.Collections.Generic;
using Caliburn.Micro;

namespace ProjectHeleus.SharedLibrary.Extenstions
{
    public static class Extensions
    {
        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> source)
        {
            return new BindableCollection<T>(source);
        }
    }
}