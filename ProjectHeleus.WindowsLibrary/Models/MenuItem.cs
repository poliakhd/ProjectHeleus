namespace ProjectHeleus.WindowsLibrary.Models
{
    using System;

    using Windows.UI.Xaml.Controls;

    public class MenuItem
    {
        public string Name { get; set; }
        public Type Page { get; set; }
        public Symbol Icon { get; set; }
    }
}