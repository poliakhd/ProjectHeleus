using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjectHeleus.MangaApp.Views
{
    using Windows.UI.Xaml.Media.Animation;
    using ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPageView : Page
    {
        public DetailPageViewModel ViewModel => (DetailPageViewModel)DataContext;


        public DetailPageView()
        {
            this.InitializeComponent();
        }

        private void Image_OnImageOpened(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            if (e.RemovedItems.Count <= 0) return;

            var newSelectedItem = View.ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]) as FlipViewItem;
            var previousSelectedItem = View.ItemContainerGenerator.ContainerFromItem(e.RemovedItems[0]) as FlipViewItem;

            if (newSelectedItem == null) return;
            if (previousSelectedItem == null) return;

            var duration = new Duration(TimeSpan.FromMilliseconds(500));

            var hideAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.4,
                AutoReverse = false,
                Duration = duration
            };

            var hideSb = new Storyboard();
            hideSb.Children.Add(hideAnimation);
            Storyboard.SetTargetProperty(hideSb, "Opacity");
            Storyboard.SetTarget(hideSb, previousSelectedItem);

            hideSb.Begin();

            var showAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                AutoReverse = false,
                Duration = duration
            };

            var showSb = new Storyboard();
            showSb.Children.Add(showAnimation);
            Storyboard.SetTargetProperty(showSb, "Opacity");
            Storyboard.SetTarget(showSb, newSelectedItem);

            showSb.Begin();
        }
    }
}
