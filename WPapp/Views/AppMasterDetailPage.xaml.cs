using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WPapp.Models;
using WPapp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WPapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppMasterDetailPage : MasterDetailPage
    {
        public AppMasterDetailPage()
        {

            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as AppMasterDetailPageMenuItem;
            if (item == null)
                return;

            //var page = (Page)Activator.CreateInstance(item.TargetType);
            // page.Title = item.Title;

            if (!(item.Title.ToString() == "Home"))
                Detail = new NavigationPage(new Category(item.Id));
            else
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);
                Detail = new NavigationPage(page);
            }
                

            //Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }


        async void ConnectionErrorMessage()
        {
            Constants wpsite = new Constants();
            bool answer = await DisplayAlert("Errror: Cannot Connect To Server", "Either your internet connection is down, or, " + wpsite.sitename + "'s servers are offline.", "Yes", "No");
            Console.WriteLine("Answer: " + answer);
        }
    }
}
