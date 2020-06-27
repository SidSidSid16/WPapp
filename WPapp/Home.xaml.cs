using System;
using System.Net;
using WPapp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace WPapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        Constants wpsite = new Constants();
        bool cacheMedia;
        string siteName;
        int restOfHomeNum;

        public Home()
        {
            InitializeComponent();
            HomeHeader.Title = "Home";

            this.cacheMedia = wpsite.cacheMedia;
            this.siteName = wpsite.sitename;
            this.restOfHomeNum = wpsite.RestOfHomeNum;

            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    Populate();
            }
            catch
            {
                ConnectionOfflineAlert();
            }
        }


        private void Populate()
        {
            FeaturedContainer.Children.Clear();

            var featuredPosts = Constants.GetFeaturedPosts();

            foreach (var item in featuredPosts)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) =>
                {
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
                postTitle.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
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
                publishDate.FontSize = Device.GetNamedSize(NamedSize.Caption, typeof(Label));
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
                exerpt.FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label));
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
                FeaturedContainer.Children.Add(featuredFrame);
            }

            BoxView featuredSeparator = new BoxView();
            featuredSeparator.Color = Color.Gray;
            featuredSeparator.HeightRequest = 5;
            featuredSeparator.HorizontalOptions = LayoutOptions.Fill;
            featuredSeparator.Margin = new Thickness(0, 20, 0, 0);
            FeaturedContainer.Children.Add(featuredSeparator);

            List<int> categoryList = wpsite.restOfHomeCats;
            List<int> tagList = wpsite.restOfHomeTags;

            if (categoryList == null)
            {
                int columnNum = 0;
                int rowNum = 0;
                foreach (var tag in tagList)
                {
                    string tagName = "Latest in " + Constants.GetTagName(tag) + ":";
                    Label name = new Label();
                    name.Text = tagName;
                    name.FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label));
                    name.FontAttributes = FontAttributes.Bold;
                    name.Margin = new Thickness(0, 20, 10, 0);
                    FeaturedContainer.Children.Add(name);

                    List<int> tagsTemp = new List<int>();
                    tagsTemp.Add(tag);
                    featuredPosts = Constants.GetTagPosts(tagsTemp.ToArray(), restOfHomeNum);

                    Grid postGrid = new Grid();
                    var column = new ColumnDefinition();
                    column.Width = new GridLength(5, GridUnitType.Star);
                    postGrid.ColumnDefinitions.Add(column);
                    postGrid.ColumnDefinitions.Add(column);
                    double division = featuredPosts.Count / 2;
                    double rowCount = Math.Ceiling(division);
                    for (int i = 0; i < rowCount; i++)
                    {
                        var row = new RowDefinition();
                        row.Height = GridLength.Auto;
                        postGrid.RowDefinitions.Add(row);
                    }



                    foreach (var item in featuredPosts)
                    {
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += async (s, e) =>
                        {
                            await Navigation.PushAsync(new Post(item.Id));
                        };
                        tapGestureRecognizer.NumberOfTapsRequired = 1; // single-tap

                        Frame featuredFrame = new Frame();
                        featuredFrame.BorderColor = Color.Gray;
                        featuredFrame.CornerRadius = 5;
                        featuredFrame.Padding = 20;
                        // featuredFrame.Margin = new Thickness(0, 10, 0, 0);
                        featuredFrame.GestureRecognizers.Add(tapGestureRecognizer);

                        Label postTitle = new Label();
                        // postTitle.FontSize = (double)NamedSize.Subtitle;
                        postTitle.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
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
                        }
                        else
                        {
                            postFeaturedImage.Source = ImageSource.FromUri(Constants.GetMediaUri(item.FeaturedMedia.Value));
                        }

                        BoxView imageSeparator = new BoxView();
                        imageSeparator.Color = Color.Gray;
                        imageSeparator.HeightRequest = 2;
                        imageSeparator.HorizontalOptions = LayoutOptions.Fill;

                        Label publishDate = new Label();
                        // exerpt.FontSize = (double)NamedSize.Body;
                        publishDate.FontSize = Device.GetNamedSize(NamedSize.Caption, typeof(Label));
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

                        StackLayout postDetails = new StackLayout();
                        postDetails.Children.Add(postTitle);
                        postDetails.Children.Add(titleSeparator);
                        postDetails.Children.Add(postFeaturedImage);
                        postDetails.Children.Add(imageSeparator);
                        postDetails.Children.Add(publishDate);

                        featuredFrame.Content = postDetails;

                        postGrid.Children.Add(featuredFrame, columnNum, rowNum);
                        columnNum++;
                        if (columnNum == 2)
                        {
                            rowNum++;
                            columnNum = 0;
                        }
                    }
                    FeaturedContainer.Children.Add(postGrid);
                    columnNum = 0;
                    rowNum = 0;
                }
            } else
            {
            int columnNum = 0;
            int rowNum = 0;
            foreach (var category in categoryList)
            {
                string catName = "Latest in " + Constants.GetCategoryName(category) + ":";
                Label name = new Label();
                name.Text = catName;
                name.FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label));
                name.FontAttributes = FontAttributes.Bold;
                name.Margin = new Thickness(0, 20, 10, 0);
                FeaturedContainer.Children.Add(name);

                List<int> catsTemp = new List<int>();
                catsTemp.Add(category);
                featuredPosts = Constants.GetCategoryPosts(catsTemp.ToArray(), restOfHomeNum);

                Grid postGrid = new Grid();
                var column = new ColumnDefinition();
                column.Width = new GridLength(5, GridUnitType.Star);
                postGrid.ColumnDefinitions.Add(column);
                postGrid.ColumnDefinitions.Add(column);
                double division = featuredPosts.Count / 2;
                double rowCount = Math.Ceiling(division);
                for (int i = 0; i < rowCount; i++)
                {
                    var row = new RowDefinition();
                    row.Height = GridLength.Auto;
                    postGrid.RowDefinitions.Add(row);
                }

                foreach (var item in featuredPosts)
                {
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        await Navigation.PushAsync(new Post(item.Id));
                    };
                    tapGestureRecognizer.NumberOfTapsRequired = 1; // single-tap

                    Frame featuredFrame = new Frame();
                    featuredFrame.BorderColor = Color.Gray;
                    featuredFrame.CornerRadius = 5;
                    featuredFrame.Padding = 20;
                    // featuredFrame.Margin = new Thickness(0, 10, 0, 0);
                    featuredFrame.GestureRecognizers.Add(tapGestureRecognizer);

                    Label postTitle = new Label();
                    // postTitle.FontSize = (double)NamedSize.Subtitle;
                    postTitle.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
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
                    }
                    else
                    {
                        postFeaturedImage.Source = ImageSource.FromUri(Constants.GetMediaUri(item.FeaturedMedia.Value));
                    }

                    BoxView imageSeparator = new BoxView();
                    imageSeparator.Color = Color.Gray;
                    imageSeparator.HeightRequest = 2;
                    imageSeparator.HorizontalOptions = LayoutOptions.Fill;

                    Label publishDate = new Label();
                    // exerpt.FontSize = (double)NamedSize.Body;
                    publishDate.FontSize = Device.GetNamedSize(NamedSize.Caption, typeof(Label));
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


                    StackLayout postDetails = new StackLayout();
                    postDetails.Margin = new Thickness(0);
                    postDetails.Padding = new Thickness(0); 
                    postDetails.Children.Add(postTitle);
                    postDetails.Children.Add(titleSeparator);
                    postDetails.Children.Add(postFeaturedImage);
                    postDetails.Children.Add(imageSeparator);
                    postDetails.Children.Add(publishDate);

                    featuredFrame.Content = postDetails;

                    if ((featuredPosts.First() == item) && !(featuredPosts.Count() % 2 == 0))
                    {
                        postGrid.Children.Add(featuredFrame, columnNum, rowNum);
                        Grid.SetColumnSpan(featuredFrame, 2);
                        columnNum++;
                    }
                    else
                    {
                        postGrid.Children.Add(featuredFrame, columnNum, rowNum);
                        Grid.SetColumnSpan(featuredFrame, 1);
                    }

                        
                    columnNum++;
                    if (columnNum == 2)
                    {
                        rowNum++;
                        columnNum = 0;
                    }
                }
                FeaturedContainer.Children.Add(postGrid);
                columnNum = 0;
                rowNum = 0;
            }
            }
        }

        async void ConnectionOfflineAlert()
        {
            bool answer = await DisplayAlert("Connection Error!", "This could be due to this device being offline, or, " + siteName + "'s servers are down", "Retry", "Quit");
            if (answer)
            {
                (Application.Current).MainPage = new AppMasterDetailPage();
            } else
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        void refreshButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Populate();
        }
        
    }
}
