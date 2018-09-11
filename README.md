# Sentiment Analysis

| Happy Sentiment      | Sad Sentiment |
|---------------------------|---------------------------
| ![Happy Sentiment](https://user-images.githubusercontent.com/13558917/45384332-930a2a80-b5c2-11e8-93a3-120a5f574cfb.gif)|  ![Sad Sentiment](https://user-images.githubusercontent.com/13558917/45384333-93a2c100-b5c2-11e8-81d4-39cbe973164c.gif)|

Microsoft's [Cognitive Services](https://azure.microsoft.com/services/cognitive-services/?WT.mc_id=none-XamarinBlog-bramin) team have created the [Sentiment Analysis API](https://westus.dev.cognitive.microsoft.com/docs/services/TextAnalytics.V2.0/operations/56f30ceeeda5650db055a3c9/?WT.mc_id=none-XamarinBlog-bramin) that uses machine learning to determine the sentiment of uploaded text. And the best part is, we don't need to be machine learning experts to use it.

I just submit the text as a POST Request:

```json
{
  "documents": [
    {
      "language": "en",
      "id": "251c99d7-1f89-426a-a3ad-c6fa1b34f020",
      "text": "I hope you find time to actually get your reports done today."
    }
  ]
}
```

And the API returns back its sentiment score:

```json
{
"sentiment": {
  "documents": [
    {
      "id": "251c99d7-1f89-426a-a3ad-c6fa1b34f020",
      "score": 0.776355504989624
    }
  ]
}
```

The sentiment score ranges between 0 and 1.

Scores close to 0 indicate negative sentiment, while scores close to 1 indicate positive sentiment.

## Learn More

Visit the Microsoft Docs to learn more about Cognitive Services:

- [Cognitive Services](https://azure.microsoft.com/services/cognitive-services/?WT.mc_id=none-XamarinBlog-bramin)
- [Text Analytics](https://azure.microsoft.com/services/cognitive-services/text-analytics/?WT.mc_id=none-XamarinBlog-bramin)
- [Sentiment Analysis API](https://westus.dev.cognitive.microsoft.com/docs/services/TextAnalytics.V2.0/operations/56f30ceeeda5650db055a3c9/?WT.mc_id=none-XamarinBlog-bramin)
