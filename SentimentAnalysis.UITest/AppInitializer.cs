using System;
using Xamarin.UITest;

namespace SentimentAnalysis.UITest
{
    public static class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            switch (platform)
            {
                case Platform.iOS:
                    return ConfigureApp.iOS.StartApp();
                case Platform.Android:
                    return ConfigureApp.Android.StartApp();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
