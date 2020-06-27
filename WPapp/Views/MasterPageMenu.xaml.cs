using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPapp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WPapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPageMenu : ContentPage
    {
        public ListView ListView;
        Constants wpsite = new Constants();

        public MasterPageMenu()
        {
            InitializeComponent();

            AppNameDisplay.Text = wpsite.sitename;
            AppNameDisplay.FontSize = 24;
            AppNameDisplay.FontAttributes = FontAttributes.Bold;
            // Tap the AppNameDisplay 10 times to view unique app ID
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                DisplayAppID();
            };
            tapGestureRecognizer.NumberOfTapsRequired = 10;
            AppNameDisplay.GestureRecognizers.Add(tapGestureRecognizer);

            HeaderGrid.BackgroundColor = wpsite.accentcolour;
            FooterGrid.BackgroundColor = wpsite.accentcolour;

            AppCredits.Text = wpsite.sitename + " \n " + wpsite.siteurl;

            if (wpsite.showcredits)
            {
                AppCredits.Text = AppCredits.Text + "\n\n\n\n" + "App made with ♡ by Sid\nwww.sidsidsid.cf";
            }

            BindingContext = new AppMasterDetailPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        async void DisplayAppID()
        {
            await DisplayAlert("App ID", "\n" + wpsite.AppID, "Ok");
        }

        class AppMasterDetailPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<AppMasterDetailPageMenuItem> MenuItems { get; set; }

            public AppMasterDetailPageMasterViewModel()
            {
                Constants wpsite = new Constants();
                List<WordPressPCL.Models.Category> allCategories = new List<WordPressPCL.Models.Category>();
                try
                {
                    using (var client = new WebClient())
                    using (client.OpenRead("http://google.com/generate_204"))
                        allCategories = Constants.GetAllCategories();
                }
                catch
                {
                    Console.WriteLine("OFFLINE cannot load categories");
                }

                //Constants wpsite = new Constants();
                //var allCategories = Constants.GetAllCategories();

                //MenuItems = new ObservableCollection<AppMasterDetailPageMenuItem>(new[]
                //{
                //    new AppMasterDetailPageMenuItem { Id = 0, Title = "Home", TargetType = typeof(Home) },
                //    new AppMasterDetailPageMenuItem { Id = 1, Title = "Page 2" },
                //    new AppMasterDetailPageMenuItem { Id = 2, Title = "Page 3" },
                //    new AppMasterDetailPageMenuItem { Id = 3, Title = "Page 4" },
                //    new AppMasterDetailPageMenuItem { Id = 4, Title = "Page 5" },
                //});

                MenuItems = new ObservableCollection<AppMasterDetailPageMenuItem>();
                MenuItems.Add(new AppMasterDetailPageMenuItem { Id = 0, Title = "Home", TargetType = typeof(Home) });
                int id = 0;
                foreach (var category in allCategories)
                {
                    id++;
                    MenuItems.Add(new AppMasterDetailPageMenuItem { Id = category.Id, Title = category.Name, TargetType = typeof(Home) });
                }
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
