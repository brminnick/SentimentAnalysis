using System.Linq;

using Xamarin.UITest;

using SentimentAnalysis.Shared;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace SentimentAnalysis.UITest
{
    class SentimentPage : BasePage
    {
        readonly Query _submitButton, _enterTextBox, _activityIndicator, _emojiResultsLabel;

        public SentimentPage(IApp app) : base(app, PageTitles.SentimentPage)
        {
            _submitButton = x => x.Marked(AutomationIdConstants.SubmitButton);
            _enterTextBox = x => x.Marked(AutomationIdConstants.EnterTextBox);
            _activityIndicator = x => x.Marked(AutomationIdConstants.ActivityIndicator);
            _emojiResultsLabel = x => x.Marked(AutomationIdConstants.EmojiResultsLabel);
        }

        public string EmojiResultsText => App.Query(_emojiResultsLabel).First().Text;

        public void TapSubmitButton()
        {
            App.Tap(_submitButton);
            App.Screenshot("Submit Button Tapped");
        }

        public void EnterText(string text)
        {
            App.EnterText(_enterTextBox, text);
            App.DismissKeyboard();

            App.Screenshot($"Entered Text: {text}");
        }

        public void WaitForActivityIndicator()
        {
            App.WaitForElement(_activityIndicator);
            App.Screenshot("Activity Indicator Appeared");
        }

        public void WaitForNoActivityIndicator()
        {
            App.WaitForNoElement(_activityIndicator);
            App.Screenshot("Activity Indicator Disappeared");
        }
    }
}
