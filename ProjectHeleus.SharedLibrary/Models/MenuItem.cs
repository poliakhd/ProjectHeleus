using System;
using Windows.UI.Xaml.Controls;

namespace ProjectHeleus.SharedLibrary.Models
{
    public class MenuItem
    {
        public string Name { get; set; }
        public Type Page { get; set; }
        public Symbol Icon { get; set; }
    }
}