using System;
using Xamarin.UITest;

namespace SentimentAnalysis.UITest
{
    public static class AppInitializer
    {
        public static IApp StartApp(Platform platform) => platform switch
        {
            Platform.iOS => ConfigureApp.iOS.StartApp(),
            Platform.Android => ConfigureApp.Android.StartApp(),
            _ => throw new NotSupportedException(),
        };
    }
}
