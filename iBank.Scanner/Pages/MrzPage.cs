using System.Threading.Tasks;

using Xamarin.Forms;

namespace iBank.Scanner
{
    public abstract class MrzPage : ContentPage
    {
        protected MrzPage()
        {
            NavigationPage.SetHasBackButton(this, true);
        }

        public abstract Task RaiseMrzDetected(object sender, MrzRecognizedEventArgs e);
    }
}