using System;
using System.Net;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

using Xamarin.Forms;

using SentimentAnalysis.Shared;

namespace SentimentAnalysis
{
    public class SentimentViewModel : INotifyPropertyChanged
    {
        #region Fields
        string _emojiLabelText = string.Empty;
        string _userInputEntryText = string.Empty;
        bool _isInternetConnectionActive;
        ICommand _submitButtonCommand;
        Color _backgroundColor = ColorConstants.DefaultBackgroundColor;
        #endregion

        #region Events
        public event EventHandler<string> SentimentAnalyisFailed;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public bool IsInternetConnectionInactive => !IsInternetConnectionActive;

        public ICommand SubmitButtonCommand => _submitButtonCommand ??
            (_submitButtonCommand = new Command(async () => await ExecuteSubmitButtonCommand(UserInputEntryText).ConfigureAwait(false)));

        public bool IsInternetConnectionActive
        {
            get => _isInternetConnectionActive;
            set => SetProperty(ref _isInternetConnectionActive, value, () => OnPropertyChanged(nameof(IsInternetConnectionInactive)));
        }

        public string EmojiLabelText
        {
            get => _emojiLabelText;
            set => SetProperty(ref _emojiLabelText, value);
        }

        public string UserInputEntryText
        {
            get => _userInputEntryText;
            set => SetProperty(ref _userInputEntryText, value);
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
        #endregion

        #region Methods
        async Task ExecuteSubmitButtonCommand(string userInputEntryText)
        {
            SetIsBusy(true);

            try
            {
                var result = await TextAnalysisService.GetSentiment(userInputEntryText).ConfigureAwait(false);
                if (result is null)
                {
                    OnSentimentAnalyisFailed("No Results Returned");
                }
                else
                {
                    SetBackgroundColor((double)result);
                    SetEmoji((double)result);
                }
            }
            catch (ErrorResponseException e) when (e.Response.StatusCode.Equals(HttpStatusCode.Unauthorized))
            {
                OnSentimentAnalyisFailed("Invalid API Key");
            }
            catch (Microsoft.Rest.ValidationException)
            {
                OnSentimentAnalyisFailed("API Key Cannot Be Null");
            }
            catch (AggregateException e) when (e.InnerExceptions.Select(x => x.Message).Any(x => x.Contains("Missing input documents")))
            {
                OnSentimentAnalyisFailed("No Text Submitted");
            }
            catch (Exception e)
            {
                OnSentimentAnalyisFailed(e.Message);
            }
            finally
            {
                SetIsBusy(false);
            }
        }

        void SetEmoji(double result)
        {
            switch (result)
            {
                case double number when (number < 0.4):
                    EmojiLabelText = EmojiConstants.SadFaceEmoji;
                    break;
                case double number when (number >= 0.4 && number <= 0.6):
                    EmojiLabelText = EmojiConstants.NeutralFaceEmoji;
                    break;
                case double number when (number > 0.6):
                    EmojiLabelText = EmojiConstants.HappyFaceEmoji;
                    break;
            }
        }

        void SetBackgroundColor(double result)
        {
            switch (result)
            {
                case double number when (number <= 0.1):
                    BackgroundColor = ColorConstants.EmotionColor1;
                    break;
                case double number when (number > 0.1 && number <= 0.2):
                    BackgroundColor = ColorConstants.EmotionColor2;
                    break;
                case double number when (number > 0.2 && number <= 0.3):
                    BackgroundColor = ColorConstants.EmotionColor3;
                    break;
                case double number when (number > 0.3 && number <= 0.4):
                    BackgroundColor = ColorConstants.EmotionColor4;
                    break;
                case double number when (number > 0.4 && number <= 0.6):
                    BackgroundColor = ColorConstants.EmotionColor5;
                    break;
                case double number when (number > 0.6 && number <= 0.7):
                    BackgroundColor = ColorConstants.EmotionColor6;
                    break;
                case double number when (number > 0.7 && number <= 0.8):
                    BackgroundColor = ColorConstants.EmotionColor7;
                    break;
                case double number when (number > 0.8 && number <= 0.9):
                    BackgroundColor = ColorConstants.EmotionColor8;
                    break;
                case double number when (number > 0.9):
                    BackgroundColor = ColorConstants.EmotionColor9;
                    break;
            }
        }

        void SetIsBusy(bool isBusy)
        {
            switch (isBusy)
            {
                case true:
                    BackgroundColor = ColorConstants.DefaultBackgroundColor;
                    EmojiLabelText = string.Empty;
                    break;
            }

            IsInternetConnectionActive = isBusy;
        }

        void SetProperty<T>(ref T backingStore, T value, Action onChanged = null, [CallerMemberName] string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        void OnSentimentAnalyisFailed(string errorMessage) => SentimentAnalyisFailed?.Invoke(this, errorMessage);
        #endregion
    }
}
