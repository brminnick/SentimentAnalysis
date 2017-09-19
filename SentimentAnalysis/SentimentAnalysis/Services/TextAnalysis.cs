using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.ProjectOxford.Text.Core;
using Microsoft.ProjectOxford.Text.Sentiment;

namespace SentimentAnalysis
{
    static class TextAnalysis
    {
        #region Constant Fields
        const string _sentimentAPIKey = "b94d27788b514a33bc6e8029e16fd1d5";
        readonly static Lazy<SentimentClient> _sentimentClientHolder = new Lazy<SentimentClient>(() => new SentimentClient(_sentimentAPIKey));
        #endregion

        #region Properties
        static SentimentClient SentimentClient => _sentimentClientHolder.Value;
        #endregion

        #region Methods
        public static async Task<float?> GetSentiment(string text)
        {
            var sentimentDocument = new SentimentDocument { Id = "1", Text = text };
            var sentimentRequest = new SentimentRequest { Documents = new List<IDocument> { { sentimentDocument } } };

            var sentimentResults = await SentimentClient.GetSentimentAsync(sentimentRequest);
            var documentResult = sentimentResults.Documents.FirstOrDefault();

            return documentResult?.Score;
        }
        #endregion
    }
}
