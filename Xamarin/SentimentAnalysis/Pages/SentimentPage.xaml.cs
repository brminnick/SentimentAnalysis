using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SentimentAnalysis
{
    public partial class SentimentPage : ContentPage
    {
        public SentimentPage()
        {
            InitializeComponent();

            var viewModel = new SentimentViewModel();
            viewModel.SentimentAnalyisFailed += HandleSentimentAnalyisFailed;

            BindingContext = viewModel;

            On<iOS>().SetUseSafeArea(true);
        }

        void HandleSentimentAnalyisFailed(object sender, string ErrorMessage) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", ErrorMessage, "OK"));
    }
}
