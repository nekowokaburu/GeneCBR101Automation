using GeneRev4App.Views;
using System;
using System.Net.Sockets;
using Xamarin.Forms;


namespace XamarinForms.Client
{
    public partial class MainPage : ContentPage
	{
        public MainPage()
		{
			InitializeComponent();
		}

        private async void Connect_Clicked(object sender, EventArgs e)
        {
            try
            {
                TcpClient client = new TcpClient(); 
                    await client.ConnectAsync(IPAddress.Text, Convert.ToInt32(Port.Text));
                if (client.Connected)
                {
                    Connection.Instance.Client = client;
                    Application.Current.MainPage = new NavigationPage(new OperationsPage());
                }
                else
                {
                    await DisplayAlert("Error", "Connection unsuccessful!", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ""+ex.ToString(), "Ok");
            }
        }

        private async void About_Clicked(object sender, EventArgs e)
        {
            var aboutPage = new AboutPage();
            await Navigation.PushModalAsync(aboutPage);
        }
    }
}
