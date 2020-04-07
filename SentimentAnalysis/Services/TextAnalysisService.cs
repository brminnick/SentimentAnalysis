using System;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;

namespace SentimentAnalysis
{
    static class TextAnalysisService
    {
        readonly static Lazy<TextAnalyticsClient> _textAnalyticsApiClientHolder = new Lazy<TextAnalyticsClient>(() =>
            new TextAnalyticsClient(new Uri(CognitiveServicesConstants.BaseUrl), new AzureKeyCredential(CognitiveServicesConstants.TextSentimentAPIKey)));

        static TextAnalyticsClient TextAnalyticsApiClient => _textAnalyticsApiClientHolder.Value;

        public static async Task<TextSentiment> GetSentiment(string text)
        {
            var response = await TextAnalyticsApiClient.AnalyzeSentimentAsync(text).ConfigureAwait(false);
            return response.Value.Sentiment;
        }
    }
}
