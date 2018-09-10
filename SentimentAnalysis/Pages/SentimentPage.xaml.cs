using Xamarin.Forms;

namespace SentimentAnalysis
{
    public partial class SentimentPage : ContentPage
    {
        #region Constant Fields
        readonly SentimentViewModel _viewModel;
        #endregion

        #region Constructors
        public SentimentPage()
        {
            InitializeComponent();

            _viewModel = new SentimentViewModel();
            BindingContext = _viewModel;
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.SentimentAnalyisFailed += HandleSentimentAnalyisFailed;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.SentimentAnalyisFailed -= HandleSentimentAnalyisFailed;
        }

        void HandleSentimentAnalyisFailed(object sender, string ErrorMessage) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", ErrorMessage, "OK"));
        #endregion
    }
}
