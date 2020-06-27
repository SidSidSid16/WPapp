using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using WPapp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WPapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Category : ContentPage
    {
        Constants wpsite = new Constants();
        int categoryID;
        string categoryName;
        string siteName;
        bool cacheMedia;

        public Category(int catID)
        {
            InitializeComponent();
            this.categoryID = catID;
            this.siteName = wpsite.sitename;
            this.cacheMedia = wpsite.cacheMedia;
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                {
                    this.categoryName = Constants.GetCategoryName(categoryID);
                    CategoryHeader.Title = categoryName;
                    Populate();
                }
            }
            catch
            {
                ConnectionOfflineAlert();
            }
        }

        private void Populate()
        {
            List<int> catIDs = new List<int>();
            catIDs.Add(categoryID);

            PostContainer.Children.Clear();

            CategoryTitle.Text = "In " + categoryName + ":"; 

            var categoryPosts = Constants.GetCategoryPosts(catIDs.ToArray());

            foreach (var item in categoryPosts)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) => {
                    await Navigation.PushAsync(new Post(item.Id));
                };
                tapGestureRecognizer.NumberOfTapsRequired = 1; // single-tap

                Frame featuredFrame = new Frame();
                featuredFrame.BorderColor = Color.Gray;
                featuredFrame.CornerRadius = 5;
                featuredFrame.Padding = 20;
                featuredFrame.Margin = new Thickness(0, 10, 0, 0);
                featuredFrame.GestureRecognizers.Add(tapGestureRecognizer);

                Label postTitle = new Label();
                // postTitle.FontSize = (double)NamedSize.Subtitle;
                postTitle.FontSize = 22;
                postTitle.FontAttributes = FontAttributes.Bold;
                var html = item.Title.Rendered;
                var htmlTitle = new HtmlDocument();
                htmlTitle.LoadHtml(html);
                postTitle.Text = WebUtility.HtmlDecode(htmlTitle.DocumentNode.InnerText);

                BoxView titleSeparator = new BoxView();
                titleSeparator.Color = Color.Gray;
                titleSeparator.HeightRequest = 1;
                titleSeparator.HorizontalOptions = LayoutOptions.Fill;

                Image postFeaturedImage = new Image();
                postFeaturedImage.Aspect = Aspect.AspectFill;
                
                if (cacheMedia)
                {
                    postFeaturedImage.Source = new UriImageSource { CachingEnabled = true, Uri = Constants.GetMediaUri(item.FeaturedMedia.Value) };
                } else
                {
                    postFeaturedImage.Source = ImageSource.FromUri(Constants.GetMediaUri(item.FeaturedMedia.Value));
                }

                BoxView imageSeparator = new BoxView();
                imageSeparator.Color = Color.Gray;
                imageSeparator.HeightRequest = 2;
                imageSeparator.HorizontalOptions = LayoutOptions.Fill;

                Label publishDate = new Label();
                // exerpt.FontSize = (double)NamedSize.Body;
                publishDate.FontSize = 12;
                DateTime publishedDateTime = item.DateGmt;
                TimeSpan difference = DateTime.Now.Subtract(publishedDateTime);
                if ((difference.TotalHours) > 24)
                {
                    if ((Math.Round(difference.TotalDays, 0) == 1))
                    {
                        publishDate.Text = Math.Round(difference.TotalDays, 0) + " day ago | " + publishedDateTime.ToString();
                    }
                    else
                    {
                        publishDate.Text = Math.Round(difference.TotalDays, 0) + " days ago | " + publishedDateTime.ToString();
                    }
                }
                else
                {
                    if ((Math.Round(difference.TotalHours, 0) == 1))
                    {
                        publishDate.Text = Math.Round(difference.TotalHours, 0) + " hour ago | " + publishedDateTime.ToString();
                    }
                    else
                    {
                        publishDate.Text = Math.Round(difference.TotalHours, 0) + " hours ago | " + publishedDateTime.ToString();
                    }
                }

                Label exerpt = new Label();
                exerpt.FontSize = 12;
                html = item.Excerpt.Rendered;
                var htmlExerpt = new HtmlDocument();
                htmlExerpt.LoadHtml(html);
                exerpt.Text = WebUtility.HtmlDecode(htmlExerpt.DocumentNode.InnerText);

                StackLayout postDetails = new StackLayout();
                postDetails.Children.Add(postTitle);
                postDetails.Children.Add(titleSeparator);
                postDetails.Children.Add(postFeaturedImage);
                postDetails.Children.Add(imageSeparator);
                postDetails.Children.Add(publishDate);
                postDetails.Children.Add(exerpt);

                featuredFrame.Content = postDetails;
                PostContainer.Children.Add(featuredFrame);

            }
        }

        void refreshButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Populate();
        }


        async void ConnectionOfflineAlert()
        {
            bool answer = await DisplayAlert("Connection Error!", "This could be due to this device being offline, or, " + siteName + "'s servers are down", "Retry", "Quit");
            if (answer)
            {
                (Application.Current).MainPage = new AppMasterDetailPage();
            }
            else
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
