using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace iBank.Scanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Verification_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(new MrzDatabaseVerifierPage());

        private async void Button_View_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(new MrzViewerPage());

        private async void Button_Document_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(new VerifyInputPage());
    }
}