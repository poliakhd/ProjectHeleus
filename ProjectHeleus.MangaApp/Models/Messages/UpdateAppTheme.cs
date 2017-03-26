namespace ProjectHeleus.MangaApp.Models.Messages
{
    public enum AppTheme
    {
        Light,
        Dark
    }

    public class UpdateAppTheme
    {
        public AppTheme Theme { get; set; }
    }
}