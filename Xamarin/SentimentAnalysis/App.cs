using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SentimentAnalysis
{
    public class App : Application
    {
        public App() => MainPage = new SentimentPage();
    }
}
