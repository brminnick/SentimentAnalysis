using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Azure.AI.TextAnalytics;
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
        IAsyncCommand? _submitButtonCommand;
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

        public IAsyncCommand SubmitButtonCommand => _submitButtonCommand ??= new AsyncCommand(() => ExecuteSubmitButtonCommand(UserInputEntryText), _ => !IsInternetConnectionActive);

        public bool IsInternetConnectionActive
        {
            get => _isInternetConnectionActive;
            set => SetProperty(ref _isInternetConnectionActive, value, () => Device.BeginInvokeOnMainThread(SubmitButtonCommand.RaiseCanExecuteChanged));
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
                var sentiment = await TextAnalysisService.GetSentiment(userInputEntryText).ConfigureAwait(false);

                SetBackgroundColor(sentiment);
                SetEmoji(sentiment);
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

        void SetEmoji(in TextSentiment sentiment)
        {
            EmojiLabelText = sentiment switch
            {
                TextSentiment.Negative => EmojiConstants.SadFaceEmoji,
                TextSentiment.Mixed => EmojiConstants.NeutralFaceEmoji,
                TextSentiment.Neutral => EmojiConstants.NeutralFaceEmoji,
                TextSentiment.Positive => EmojiConstants.HappyFaceEmoji,
                _ => throw new NotSupportedException()
            };
        }

        void SetBackgroundColor(in TextSentiment sentiment)
        {
            BackgroundColor = sentiment switch
            {
                TextSentiment.Negative => ColorConstants.NegativeSentiment,
                TextSentiment.Mixed => ColorConstants.NeutralSentiment,
                TextSentiment.Neutral => ColorConstants.NeutralSentiment,
                TextSentiment.Positive => ColorConstants.PositiveSentiment,
                _ => throw new NotSupportedException()
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
