namespace ProjectHeleus.MangaApp.Models
{
    using Windows.UI.Xaml.Media;

    public class ImageModel
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public string Url { get; set; }

        public ImageSource Image { get; set; }
    }
}