using Windows.UI.Xaml.Controls;

namespace ProjectHeleus.MangaApp.Views
{
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
