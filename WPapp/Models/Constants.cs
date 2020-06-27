using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WordPressPCL;
using WordPressPCL.Utility;
using Xamarin.Forms.Xaml;

namespace WPapp.Models
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class Constants
    {
        public string AppID;
        public string sitename;
        public string siteurl;
        public string resturl;
        public string posturl;
        public string caturl;
        public string featdispl;
        public int featdisplnum;
        public bool storeOffline;
        public bool cacheMedia;
        public bool showcredits;
        public bool showAuthorPage;
        public List<int> restOfHomeCats = new List<int>();
        public List<int> restOfHomeTags = new List<int>();
        public int RestOfHomeNum;
        public Color accentcolour;

        public Constants()
        {
            // Website Name - This will be displayed in multiple locations around the app
            string sitename = "Discover Tech";

            // URL of the WordPress website
            string siteurl = "https://www.discover-tech.tk/";

            // Homepage Carousel Setup - Which posts
            // latest      - Show the latest posts  
            // t-[tag_id] - Show the latest posts which have this tag (replace tag_id with the ID of the tag, append "t-" to the name)
            //              You can use more than one tag_id, just separate them with a comma eg: t-110,111,112,113
            // c-[cat_id] - Show the latest posts which are in this category (replace cat_id with the ID of the category, append "c-" to the name)
            //              You can use more than one cat_id, just separate them with a comma eg: c-110,111,112,113
            string featdispl = "t-100";

            // Homepage Carousel Setup - How many posts
            // How many posts would you like the carousel to display
            // 4 [Default]
            int featdisplnum = 5;

            // Posts Under Featured Posts: What to display
            // latest      - Show the latest posts  
            // t-[tag_id] - Show the latest posts which have this tag (replace tag_id with the ID of the tag, append "t-" to the name)
            //              You can use more than one tag_id, just separate them with a comma eg: t-110,111,112,113
            // c-[cat_id] - Show the latest posts which are in this category (replace cat_id with the ID of the category, append "c-" to the name)
            //              You can use more than one cat_id, just separate them with a comma eg: c-110,111,112,113
            string restofhome = "c-4,3,6,5";

            // Posts Under Featured Posts: How many to display
            // Enter the number of posts you would like to display for each category/tag/latest
            int restofhomeNum = 5;

            // Accent Colour
            // Enter the hex code of your desired accent colour
            // Default: r=3, g=169, b=244
            int r = 3;
            int g = 169;
            int b = 244;

            // NOT IMPLEMENTED
            // Actively store content for offline reading?
            bool storeOffline = false;

            // Show Author Page?
            // When the author details frame is pressed inside of the post page, an author page will be shown
            // Set to true to show this page
            bool showauthorpage = true;

            // Cache Images & Media?
            // Caching images will result in a performance increase as assets can be loaded quicker
            // Wordpress server will also experience reduced loads as this app will not query for images very frequently
            bool cachemedia = true;

            // Show App Credits?
            // Show some love and set this to true :-)
            bool showcredits = true;


            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // No need to edit anything below this line
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            AppID = "sidh01";

            this.sitename = sitename;
            this.siteurl = siteurl;
            resturl = siteurl + "wp-json/";
            posturl = siteurl + "wp-json/wp/v2/posts";
            caturl = siteurl + "wp-json/wp/v2/categories";
            this.featdispl = featdispl;
            this.featdisplnum = featdisplnum;
            this.accentcolour = Color.FromArgb(r, g, b);
            this.storeOffline = storeOffline;
            this.cacheMedia = cachemedia;
            this.showAuthorPage = showauthorpage;
            this.showcredits = showcredits;
            this.RestOfHomeNum = restofhomeNum;

            var homeDisplAffix = new System.Text.StringBuilder();
            homeDisplAffix.Append(restofhome[0]);
            homeDisplAffix.Append(restofhome[1]);
            if ((homeDisplAffix.ToString()) == "t-")
            {
                var homeDispl = new System.Text.StringBuilder();
                homeDispl.Append(restofhome);
                string tags = homeDispl.Remove(0, 2).ToString();
                Console.WriteLine(tags);
                List<string> tagListString = tags.Split(',').ToList();
                restOfHomeTags = tagListString.Select(int.Parse).ToList();
                restOfHomeCats = null;
            }
            else
            {
                if ((homeDisplAffix.ToString()) == "c-")
                {
                    var homeDispl = new System.Text.StringBuilder();
                    homeDispl.Append(restofhome);
                    string cats = homeDispl.Remove(0, 2).ToString();
                    Console.WriteLine(cats);
                    List<string> catListString = cats.Split(',').ToList();
                    restOfHomeCats = catListString.Select(int.Parse).ToList();
                    restOfHomeTags = null;
                }
            }
        }

        public static List<WordPressPCL.Models.Post> GetAllPosts()
        {
            List<WordPressPCL.Models.Post> posts = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var post_task = client.Posts.GetAll();
                post_task.Wait();
                posts = post_task.Result.ToList();
            }).Wait();
            return posts;
        }

        public static List<WordPressPCL.Models.Post> GetCategoryPosts(int[] categoryIDs)
        {
            List<WordPressPCL.Models.Post> posts = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var queryBuilder = new PostsQueryBuilder();
                queryBuilder.Categories = categoryIDs;
                var post_task = client.Posts.Query(queryBuilder);
                post_task.Wait();
                posts = post_task.Result.ToList();
            }).Wait();
            return posts;
        }

        public static List<WordPressPCL.Models.Post> GetCategoryPosts(int[] categoryIDs, int numOfPosts)
        {
            List<WordPressPCL.Models.Post> posts = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var queryBuilder = new PostsQueryBuilder();
                queryBuilder.Page = 1;
                queryBuilder.PerPage = numOfPosts;
                queryBuilder.Categories = categoryIDs;
                var post_task = client.Posts.Query(queryBuilder);
                post_task.Wait();
                posts = post_task.Result.ToList();
            }).Wait();
            return posts;
        }

        public static List<WordPressPCL.Models.Post> GetTagPosts(int[] tagIDs)
        {
            List<WordPressPCL.Models.Post> posts = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var queryBuilder = new PostsQueryBuilder();
                queryBuilder.Tags = tagIDs;
                var post_task = client.Posts.Query(queryBuilder);
                post_task.Wait();
                posts = post_task.Result.ToList();
            }).Wait();
            return posts;
        }

        public static List<WordPressPCL.Models.Post> GetTagPosts(int[] tagIDs, int numOfPosts)
        {
            List<WordPressPCL.Models.Post> posts = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var queryBuilder = new PostsQueryBuilder();
                queryBuilder.Page = 1;
                queryBuilder.PerPage = numOfPosts;
                queryBuilder.Tags = tagIDs;
                var post_task = client.Posts.Query(queryBuilder);
                post_task.Wait();
                posts = post_task.Result.ToList();
            }).Wait();
            return posts;
        }

        public static List<WordPressPCL.Models.Post> GetFeaturedPosts()
        {
            List<WordPressPCL.Models.Post> posts = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var queryBuilder = new PostsQueryBuilder();
                queryBuilder.PerPage = wpsite.featdisplnum;
                queryBuilder.Page = 1;
                var featDisplAffix = new System.Text.StringBuilder();
                featDisplAffix.Append(wpsite.featdispl[0]);
                featDisplAffix.Append(wpsite.featdispl[1]);
                if ((featDisplAffix.ToString()) == "t-")
                {
                    var featDispl = new System.Text.StringBuilder();
                    featDispl.Append(wpsite.featdispl);
                    string tags = featDispl.Remove(0, 2).ToString();
                    List<string> tagListString = tags.Split(',').ToList();
                    List<int> tagList = tagListString.Select(int.Parse).ToList();
                    queryBuilder.Tags = tagList.ToArray();
                }
                else
                {
                    if ((featDisplAffix.ToString()) == "c-")
                    {
                        var featDispl = new System.Text.StringBuilder();
                        featDispl.Append(wpsite.featdispl);
                        string cats = featDispl.Remove(0, 2).ToString();
                        List<string> catListString = cats.Split(',').ToList();
                        List<int> catList = catListString.Select(int.Parse).ToList();
                        queryBuilder.Categories = catList.ToArray();
                    }
                }
                var post_task = client.Posts.Query(queryBuilder);
                post_task.Wait();
                posts = post_task.Result.ToList();
            }).Wait();
            return posts;
        }



        public static List<WordPressPCL.Models.Category> GetAllCategories()
        {
            List<WordPressPCL.Models.Category> categories = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var category_task = client.Categories.GetAll();
                category_task.Wait();
                categories = category_task.Result.ToList();
            }).Wait();
            return categories;
        }

        public static string GetCategoryName(int categoryID)
        {
            string categoryName = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var category_task = client.Categories.GetByID(categoryID);
                category_task.Wait();
                categoryName = category_task.Result.Name.ToString();
            }).Wait();
            return categoryName;
        }



        public static string GetTagName(int tagID)
        {
            string tagName = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var tag_task = client.Tags.GetByID(tagID);
                tag_task.Wait();
                tagName = tag_task.Result.Name.ToString();
            }).Wait();
            return tagName;
        }



        public static Uri GetMediaUri(int MediaID)
        {
            Uri mediaUri = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var mediaUri_task = client.Media.GetByID(MediaID);
                mediaUri_task.Wait();
                mediaUri = new Uri(mediaUri_task.Result.SourceUrl);
            }).Wait();
            return mediaUri;
        }



        public static Uri GetUserAvatar(int UserID)
        {
            Uri userAvatar = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var userAvatar_task = client.Users.GetByID(UserID);
                userAvatar_task.Wait();
                userAvatar = new Uri(userAvatar_task.Result.AvatarUrls.Size96.ToString());
            }).Wait();
            return userAvatar;
        }

        public static String GetUserDisplayName(int UserID)
        {
            String userDisplayName = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var userDisplayName_task = client.Users.GetByID(UserID);
                userDisplayName_task.Wait();
                userDisplayName = userDisplayName_task.Result.Name.ToString();
            }).Wait();
            return userDisplayName;
        }



        public static double GetMediaAspectRatio(int mediaID)
        {
            double aspectRatio = 0;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var aspectRatio_task = client.Media.GetByID(mediaID);
                aspectRatio_task.Wait();
                double height = aspectRatio_task.Result.MediaDetails.Height;
                double width = aspectRatio_task.Result.MediaDetails.Width;
                aspectRatio = height/width;
            }).Wait();
            return aspectRatio;
        }

        public static string GetMediaCaption(int mediaID)
        {
            string caption = null;

            Task.Run(() =>
            {
                Constants wpsite = new Constants();
                var client = new WordPressClient(wpsite.resturl);
                var caption_task = client.Media.GetByID(mediaID);
                caption_task.Wait();
                caption = caption_task.Result.Caption.Rendered;
                var htmlCaption = new HtmlDocument();
                htmlCaption.LoadHtml(caption);
                caption = WebUtility.HtmlDecode(htmlCaption.DocumentNode.InnerText);
            }).Wait();
            return caption;
        }
    }
}
