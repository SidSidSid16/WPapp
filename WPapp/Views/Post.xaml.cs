using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WordPressPCL;
using WPapp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WPapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Post : ContentPage
    {
        Constants wpsite = new Constants();
        int PostID;
        List<int> categoryIDs = new List<int>();
        int AuthorID = 0;
        DateTime publishDateTime;
        string title;
        Uri featuredImageUri;
        string content;
        bool showAuthorPage;
        bool cacheMedia;
        Uri postUri;

        public Post(int postID)
        {
            InitializeComponent();
            this.PostID = postID;
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
                var post_task = client.Posts.GetByID(PostID);
                post_task.Wait();
                var thisPost = post_task.Result;
                categoryIDs = thisPost.Categories.ToList();
                AuthorID = thisPost.Author;
                publishDateTime = thisPost.DateGmt;
                title = thisPost.Title.Rendered;
                featuredImageUri = Constants.GetMediaUri(thisPost.FeaturedMedia.Value);
                content = thisPost.Content.Rendered;
                showAuthorPage = wpsite.showAuthorPage;
                postUri = new Uri(thisPost.Link);
            }).Wait();

            if (showAuthorPage)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) => {
                    await Navigation.PushAsync(new Author(AuthorID));
                };
                tapGestureRecognizer.NumberOfTapsRequired = 1; // single-tap
                AuthorFrame.GestureRecognizers.Add(tapGestureRecognizer);
            }

            var htmlTitle = new HtmlDocument();
            htmlTitle.LoadHtml(title);
            Title.Text = WebUtility.HtmlDecode(htmlTitle.DocumentNode.InnerText);
            
            if (cacheMedia)
            {
                AuthorProfilePic.Source = new UriImageSource { CachingEnabled = true, Uri = Constants.GetUserAvatar(AuthorID) };
            } else
            {
                AuthorProfilePic.Source = Constants.GetUserAvatar(AuthorID);
            }

            var categoryNameList = new System.Text.StringBuilder();
            int i = 0;
            foreach (int categoryID in categoryIDs)
            {
                if ((categoryIDs.Count() > 1) && (i == 0))
                {
                    categoryNameList.Append(Constants.GetCategoryName(categoryID));
                    categoryNameList.Append(", ");
                }
                else
                {
                    categoryNameList.Append(Constants.GetCategoryName(categoryID));
                }
                i++;
            }
            
            if (cacheMedia)
            {
                FeaturedImage.Source = new UriImageSource { CachingEnabled = true, Uri = featuredImageUri };
            }else
            {
                FeaturedImage.Source = featuredImageUri;
            }

            DateTime publishedDateTime = publishDateTime;
            TimeSpan difference = DateTime.Now.Subtract(publishedDateTime);
            string publishedAgo;
            if ((difference.TotalHours) > 24)
            {
                if ((Math.Round(difference.TotalDays, 0) == 1))
                {
                    publishedAgo = Math.Round(difference.TotalDays, 0) + " day ago on " + publishedDateTime.ToShortDateString() + " at " + publishedDateTime.ToShortTimeString() + " GMT";
                }
                else
                {
                    publishedAgo = Math.Round(difference.TotalDays, 0) + " days ago, on " + publishedDateTime.ToShortDateString() + " at " + publishedDateTime.ToShortTimeString() + " GMT";
                }
            }
            else
            {
                if ((Math.Round(difference.TotalHours, 0) == 1))
                {
                    publishedAgo = Math.Round(difference.TotalHours, 0) + " hour ago, on " + publishedDateTime.ToShortDateString() + " at " + publishedDateTime.ToShortTimeString() + " GMT";
                }
                else
                {
                    publishedAgo = Math.Round(difference.TotalHours, 0) + " hours ago, on " + publishedDateTime.ToShortDateString() + " at " + publishedDateTime.ToShortTimeString() + " GMT";
                }
            }

            PostMeta.Text = "Written by " + Constants.GetUserDisplayName(AuthorID) + "\nPublished " + publishedAgo + " and categorized in " + categoryNameList.ToString();

            HtmlDocument htmlContent = new HtmlDocument();
            htmlContent.LoadHtml(content);
            HtmlNode[] nodes = htmlContent.DocumentNode.Descendants().ToArray();

            foreach (HtmlNode item in nodes)
            {
                // figures
                if (item.Name.ToString() == "figure")
                {
                    HtmlNode[] figureNodes = item.Descendants().ToArray();

                    foreach (HtmlNode figure in figureNodes)
                    {
                        if (figure.Name.ToString() == "img")
                        {
                            Uri imageSource = new Uri(figure.GetAttributeValue("src", "").ToString());

                            Int32.TryParse(figure.GetAttributeValue("data-attachment-id", "").ToString(), out int mediaID);

                            string caption = null;
                            double aspectRatio = 0;
                            Task.Run(() =>
                            {
                                var client = new WordPressClient(wpsite.resturl);
                                var caption_task = client.Media.GetByID(mediaID);
                                caption_task.Wait();
                                caption = caption_task.Result.Caption.Rendered;
                                double height = caption_task.Result.MediaDetails.Height;
                                double width = caption_task.Result.MediaDetails.Width;
                                aspectRatio = height / width;
                                var htmlCaption = new HtmlDocument();
                                htmlCaption.LoadHtml(caption);
                                caption = WebUtility.HtmlDecode(htmlCaption.DocumentNode.InnerText);
                            }).Wait();

                            Image image = new Image();
                            if (cacheMedia)
                            {
                                image.Source = new UriImageSource { CachingEnabled = true, Uri = imageSource };
                            } else
                            {
                                image.Source = imageSource;
                            }
                            image.Aspect = Aspect.AspectFit;
                            double maxWidth = Application.Current.MainPage.Width - 20;
                            double imageHeight = aspectRatio * maxWidth;
                            image.HeightRequest = imageHeight;
                            image.IsAnimationPlaying = true;

                            Label imageCaption = new Label();
                            imageCaption.Text = caption;
                            imageCaption.HorizontalTextAlignment = TextAlignment.Center;
                            imageCaption.FontSize = 12;
                            imageCaption.Margin = new Thickness(0, 0, 0, 10);

                            PostContainer.Children.Add(image);
                            PostContainer.Children.Add(imageCaption);
                        }
                    }
                }

                // paragraph
                if ((item.Name.ToString() == "p"))
                {
                    Label paragraph = new Label();
                    var text = new FormattedString();
                    paragraph.FontSize = 18;
                    paragraph.Margin = new Thickness(0, 10, 0, 0);
                    HtmlNode[] paragraphNodes = item.Descendants().ToArray();

                    foreach (HtmlNode node in paragraphNodes)
                    {
                        if (node.Name.ToString() == "#text")
                        {
                            Span textSpan = new Span();
                            textSpan.Text = WebUtility.HtmlDecode(node.InnerText.ToString());
                            textSpan.FontSize = 18;
                            // text.Spans.Add(textSpan);

                            if (node.ParentNode.Name.ToString() == "a")
                            {
                                Span linktext = new Span();
                                linktext.Text = WebUtility.HtmlDecode(node.InnerText.ToString());
                                linktext.FontSize = 18;
                                linktext.TextColor = wpsite.accentcolour;
                                linktext.TextDecorations = TextDecorations.Underline;
                                text.Spans.Add(linktext);
                            } else
                            {
                                if (node.ParentNode.ParentNode.Name.ToString() == "blockquote")
                                {
                                    Label quotetext = new Label();
                                    quotetext.Text = WebUtility.HtmlDecode(node.InnerText.ToString());
                                    quotetext.Margin = new Thickness(0, 15, 0, 0);
                                    quotetext.FontSize = 20;
                                    quotetext.TextColor = wpsite.accentcolour;
                                    quotetext.TextDecorations = TextDecorations.None;
                                    quotetext.FontAttributes = FontAttributes.Italic;
                                    //text.Spans.Add(quotetext);
                                    PostContainer.Children.Add(quotetext);
                                    HtmlNode[] quoteNodes = node.ParentNode.ParentNode.Descendants().ToArray();
                                    foreach (HtmlNode quoteNode in quoteNodes)
                                    {
                                        if (quoteNode.Name.ToString() == "cite")
                                        {
                                            Label quotecite = new Label();
                                            quotecite.Text = WebUtility.HtmlDecode(quoteNode.InnerText.ToString());
                                            quotecite.Margin = new Thickness(0, 0, 0, 5);
                                            quotecite.FontSize = 18;
                                            quotecite.TextColor = Color.Black;
                                            quotecite.TextDecorations = TextDecorations.None;
                                            quotecite.FontAttributes = FontAttributes.Bold;
                                            // quotetext.FontAttributes = FontAttributes.Italic;
                                            quotecite.HorizontalTextAlignment = TextAlignment.End;
                                            PostContainer.Children.Add(quotecite);
                                        }
                                    }
                                } else
                                {
                                    text.Spans.Add(textSpan);
                                }
                            }
                        }

                        paragraph.FormattedText = text;
                        PostContainer.Children.Add(paragraph);
                    }
                }

                // header
                string headingTag = item.Name.ToString().ToLower();
                if ( headingTag[0] == 'h')
                {
                    Label heading = new Label();
                    heading.Text = WebUtility.HtmlDecode(item.InnerText.ToString());
                    heading.Margin = new Thickness(0, 25, 0, 10);
                    double.TryParse(headingTag[1].ToString(), out double headingNum);
                    double fontSize = 36 - (4 * headingNum);
                    heading.FontSize = fontSize;
                    heading.FontAttributes = FontAttributes.Bold;
                    PostContainer.Children.Add(heading);
                }
            }
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


        public async Task sharePost_task()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = postUri.ToString(),
                Title = "Share Web Link"
            });
        }

        public void sharePost_Clicked(System.Object sender, System.EventArgs e)
        {
            sharePost_task();
        }
    }
}
