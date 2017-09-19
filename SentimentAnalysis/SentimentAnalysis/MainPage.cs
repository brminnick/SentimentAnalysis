using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SentimentAnalysis
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Task.Run(async () => await TextAnalysis.GetSentiment("Great Job!"));
        }
    }
}
