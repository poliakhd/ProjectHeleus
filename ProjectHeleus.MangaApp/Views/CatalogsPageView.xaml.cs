namespace ProjectHeleus.MangaApp.Views
{
    using Windows.UI.Xaml.Controls;

    using ViewModels;

    public sealed partial class CatalogsPageView : Page
    {
        public CatalogsPageViewModel ViewModel => (CatalogsPageViewModel) DataContext;

        public CatalogsPageView()
        {
            this.InitializeComponent();
        }
    }
}
