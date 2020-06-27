using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WordPressPCL;
using WordPressPCL.Utility;
using WPapp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WPapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Author : ContentPage
    {
        Constants wpsite = new Constants();
        int UserID;
        Uri ProfilePicture;
        string Description;
        string Email;
        string FirstName;
        string LastName;
        string Locale;
        string DisplayName;
        string Nickname;
        DateTime Registered;
        Uri AuthorUri;
        List<WordPressPCL.Models.Post> posts = new List<WordPressPCL.Models.Post>();
        bool cacheMedia;
        public Author(int UserID)
        {
            InitializeComponent();
            this.UserID = UserID;
            this.cacheMedia = wpsite.cacheMedia;
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
            Task.Run(() =>
            {
                var client = new WordPressClient(wpsite.resturl);
                var user_task = client.Users.GetByID(UserID);
                user_task.Wait();
                var result = user_task.Result;
                ProfilePicture = new Uri(result.AvatarUrls.Size96.ToString());
                Description = result.Description;
                Email = result.Email;
                FirstName = result.FirstName;
                LastName = result.LastName;
                Locale = result.Locale;
                DisplayName = result.Name;
                Nickname = result.NickName;
                // Nickname = result.UserName;
                Registered = result.RegisteredDate;
                AuthorUri = new Uri(result.Link);

                var queryBuilder = new PostsQueryBuilder();
                queryBuilder.Authors = new int[] { UserID };
                var posts_task = client.Posts.Query(queryBuilder);
                posts = posts_task.Result.ToList();
                posts_task.Wait();
            }).Wait();

            var metaFS = new FormattedString();
            
            if (cacheMedia)
            {
                AuthorProfilePic.Source = new UriImageSource { CachingEnabled = true, Uri = ProfilePicture };
            } else
            {
                AuthorProfilePic.Source = ProfilePicture;
            }

            if (!((string.IsNullOrEmpty(FirstName)) && (string.IsNullOrEmpty(FirstName))))
            {
                AuthorName.Text = FirstName + LastName;
            }
            else
            {
                AuthorName.Text = DisplayName;
            }

            // metaFS.Spans.Add(TitleName);
            metaFS.Spans.Add(new Span { Text = Description });

            AuthorMeta.FormattedText = metaFS;

            AuthorPostsTitle.Text = "Posts written by " + DisplayName + ":";


            foreach (var item in posts)
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

                StackLayout postDetails = new StackLayout();
                postDetails.Children.Add(postTitle);
                postDetails.Children.Add(titleSeparator);
                postDetails.Children.Add(postFeaturedImage);
                postDetails.Children.Add(imageSeparator);
                postDetails.Children.Add(publishDate);
                postDetails.Children.Add(imageSeparator);

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
            bool answer = await DisplayAlert("Connection Error!", "This could be due to this device being offline, or, " + wpsite.sitename + "'s servers are down", "Retry", "Quit");
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
