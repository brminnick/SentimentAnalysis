using System;
using NUnit.Framework;

using Xamarin.UITest;

namespace SentimentAnalysis.UITest
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    abstract class BaseTest
    {
        readonly Platform _platform;

        IApp? _app;
        SentimentPage? _sentimentPage;

        protected BaseTest(Platform platform) => _platform = platform;

        protected IApp App => _app ?? throw new NullReferenceException();
        protected SentimentPage SentimentPage => _sentimentPage ?? throw new NullReferenceException();

        [SetUp]
        public virtual void TestSetup()
        {
            _app = AppInitializer.StartApp(_platform);
            _sentimentPage = new SentimentPage(App);

            App.Screenshot("App Launched");
        }

        [TearDown]
        public virtual void TestTearDown()
        {
        }

        [Test]
        [Ignore("REPL Used for Testing")]
        public void ReplTest() => App.Repl();
    }
}
