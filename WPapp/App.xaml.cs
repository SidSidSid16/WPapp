using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WPapp.Views;
using WPapp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;

namespace WPapp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // MainPage = new Home();
            // MainPage = new NavigationPage(new Home());

            MainPage = new AppMasterDetailPage();


        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
