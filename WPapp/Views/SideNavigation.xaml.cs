using System;
using System.Collections.Generic;
using WPapp.Models;

using Xamarin.Forms;

namespace WPapp.Views
{
    public partial class SideNavigation : ContentPage
    {
        Constants wpsite = new Constants();

        private Thickness logoFrameMargin = new Thickness(20, 60, 20, 50);
        private Thickness logoFramePadding = new Thickness(5, 10, 5, 10);
        private Thickness childFrameMargin = new Thickness(20, 0, 20, 10);
        private Thickness childFramePadding = new Thickness(10, 10, 10, 10);

        public SideNavigation()
        {
            InitializeComponent();
            Populate();
        }

        private void Populate()
        {
            SidebarContainer.Children.Clear();

            Label siteName = new Label();
            siteName.Text = wpsite.sitename;
            siteName.FontSize = 22;
            siteName.FontAttributes = FontAttributes.Bold;
            siteName.HorizontalTextAlignment = TextAlignment.Center;
            siteName.VerticalTextAlignment = TextAlignment.Center;

            Frame siteNameFrame = new Frame();
            siteNameFrame.Margin = logoFrameMargin;
            siteNameFrame.Padding = logoFramePadding;
            siteNameFrame.HeightRequest = 40;
            siteNameFrame.Content = siteName;

            Label homeLabel = new Label();
            homeLabel.FontSize = 16;
            homeLabel.FontAttributes = FontAttributes.None;
            homeLabel.Text = "Home";

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                // Navigation.PushAsync(new Home());
            };
            tapGestureRecognizer.NumberOfTapsRequired = 1; // single-tap

            Frame homePageFrame = new Frame();
            homePageFrame.Margin = childFrameMargin;
            homePageFrame.Padding = childFramePadding;
            homePageFrame.Content = homeLabel;
            homePageFrame.GestureRecognizers.Add(tapGestureRecognizer);

            SidebarContainer.Children.Add(siteNameFrame);
            SidebarContainer.Children.Add(homePageFrame);

            var allCategories = Constants.GetAllCategories();
            foreach (var category in allCategories)
            {
                Label categoryLabel = new Label();
                categoryLabel.FontSize = 16;
                categoryLabel.FontAttributes = FontAttributes.None;
                categoryLabel.Text = category.Name;

                Frame categoryFrame = new Frame();
                categoryFrame.Margin = childFrameMargin;
                categoryFrame.Padding = childFramePadding;
                categoryFrame.Content = categoryLabel;

                SidebarContainer.Children.Add(categoryFrame);
            }
        }
    }
}
