using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinForms.Client;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("Pacifico.ttf", Alias = "FontPacifico")]
namespace GeneRev4App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
