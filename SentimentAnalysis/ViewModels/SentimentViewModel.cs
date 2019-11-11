using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using SentimentAnalysis.Shared;
using Xamarin.Forms;

namespace SentimentAnalysis
{
    public class SentimentViewModel : INotifyPropertyChanged
    {
        readonly WeakEventManager<string> _sentimentAnalysisFailedEventManager = new WeakEventManager<string>();
        readonly WeakEventManager _propertyChangedEventManager = new WeakEventManager();

        string _emojiLabelText = string.Empty;
        string _userInputEntryText = string.Empty;
        bool _isInternetConnectionActive;
        ICommand? _submitButtonCommand;
        Color _backgroundColor = ColorConstants.DefaultBackgroundColor;

        public event EventHandler<string> SentimentAnalyisFailed
        {
            add => _sentimentAnalysisFailedEventManager.AddEventHandler(value);
            remove => _sentimentAnalysisFailedEventManager.RemoveEventHandler(value);
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => _propertyChangedEventManager.AddEventHandler(value);
            remove => _propertyChangedEventManager.RemoveEventHandler(value);
        }

        public bool IsInternetConnectionInactive => !IsInternetConnectionActive;

        public ICommand SubmitButtonCommand => _submitButtonCommand ??= new AsyncCommand(() => ExecuteSubmitButtonCommand(UserInputEntryText));

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
            catch (ErrorResponseException e) when (e.Response.StatusCode is HttpStatusCode.Unauthorized)
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

        void SetEmoji(in double result)
        {
            EmojiLabelText = result switch
            {
                double number when (number < 0.4) => EmojiConstants.SadFaceEmoji,
                double number when (number >= 0.4 && number <= 0.6) => EmojiConstants.NeutralFaceEmoji,
                double number when (number > 0.6) => EmojiConstants.HappyFaceEmoji,
                _ => throw new ArgumentOutOfRangeException(nameof(result))
            };
        }

        void SetBackgroundColor(in double result)
        {
            BackgroundColor = result switch
            {
                double number when (number <= 0.1) => ColorConstants.EmotionColor1,
                double number when (number > 0.1 && number <= 0.2) => ColorConstants.EmotionColor2,
                double number when (number > 0.2 && number <= 0.3) => ColorConstants.EmotionColor3,
                double number when (number > 0.3 && number <= 0.4) => ColorConstants.EmotionColor4,
                double number when (number > 0.4 && number <= 0.6) => ColorConstants.EmotionColor5,
                double number when (number > 0.6 && number <= 0.7) => ColorConstants.EmotionColor6,
                double number when (number > 0.7 && number <= 0.8) => ColorConstants.EmotionColor7,
                double number when (number > 0.8 && number <= 0.9) => ColorConstants.EmotionColor8,
                double number when (number > 0.9) => ColorConstants.EmotionColor9,
                _ => throw new ArgumentOutOfRangeException(nameof(result))
            };
        }

        void SetIsBusy(in bool isBusy)
        {
            if (isBusy)
            {
                BackgroundColor = ColorConstants.DefaultBackgroundColor;
                EmojiLabelText = string.Empty;
            }

            IsInternetConnectionActive = isBusy;
        }

        void SetProperty<T>(ref T backingStore, in T value, in Action? onChanged = null, [CallerMemberName] in string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        void OnPropertyChanged([CallerMemberName] in string propertyName = "") =>
            _propertyChangedEventManager.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(INotifyPropertyChanged.PropertyChanged));

        void OnSentimentAnalyisFailed(string errorMessage) => _sentimentAnalysisFailedEventManager.HandleEvent(this, errorMessage, nameof(SentimentAnalyisFailed));
    }
}
