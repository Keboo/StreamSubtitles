using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.CognitiveServices.Speech;

namespace StreamSubtitles
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Region { get; } = "westus";
        public string RecognitionLanguage { get; } = "en-US";

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public ObservableCollection<string> SubtitleLines { get; } = new ObservableCollection<string>();

        private string _DisplayedLine;
        public string DisplayedLine
        {
            get => _DisplayedLine;
            set => Set(ref _DisplayedLine, value);
        }

        private SpeechRecognizer Recognizer { get; set; }

        public MainWindowViewModel()
        {
            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);

            BindingOperations.EnableCollectionSynchronization(SubtitleLines, SubtitleLines);

            Task.Factory.StartNew(WriteSubtitles, TaskCreationOptions.LongRunning);
        }

        private void OnStop()
        {

        }

        private async void OnStart()
        {
            bool isChecked = true;

            var config = SpeechConfig.FromSubscription(Environment.GetEnvironmentVariable("StreamSubtitlesKey"), Region);
            config.SpeechRecognitionLanguage = RecognitionLanguage;
            var recognizer = Recognizer = new SpeechRecognizer(config);
            {
                if (isChecked)
                {
                    recognizer.Recognizing += RecognizingEventHandler;
                }

                //EventHandler<SpeechRecognitionCanceledEventArgs> canceledHandler = (sender, e) => CanceledEventHandler(e, recoType, source);
                //EventHandler<SessionEventArgs> sessionStartedHandler = (sender, e) => SessionStartedEventHandler(e, recoType);
                //EventHandler<SessionEventArgs> sessionStoppedHandler = (sender, e) => SessionStoppedEventHandler(e, recoType, source);
                //EventHandler<RecognitionEventArgs> speechStartDetectedHandler = (sender, e) => SpeechDetectedEventHandler(e, recoType, "start");
                //EventHandler<RecognitionEventArgs> speechEndDetectedHandler = (sender, e) => SpeechDetectedEventHandler(e, recoType, "end");

                recognizer.Recognized += RecognizedEventHandler;
                //recognizer.Canceled += canceledHandler;
                //recognizer.SessionStarted += sessionStartedHandler;
                //recognizer.SessionStopped += sessionStoppedHandler;
                //recognizer.SpeechStartDetected -= speechStartDetectedHandler;
                //recognizer.SpeechEndDetected -= speechEndDetectedHandler;

                //start,wait,stop recognition
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                //await source.Task.ConfigureAwait(false);


                //TODO Move into stop
                //await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                // unsubscribe from events
                //recognizer.Recognizing -= RecognizingEventHandler;
                //recognizer.Recognized -= RecognizedEventHandler;
                //recognizer.Canceled -= canceledHandler;
                //recognizer.SessionStarted -= sessionStartedHandler;
                //recognizer.SessionStopped -= sessionStoppedHandler;
                //recognizer.SpeechStartDetected -= speechStartDetectedHandler;
                //recognizer.SpeechEndDetected -= speechEndDetectedHandler;
            }
        }

        private void RecognizingEventHandler(object _, SpeechRecognitionEventArgs e)
        {
            Debug.WriteLine($"  => {e.Result.Text}");
            //var log = (rt == RecoType.Base) ? this.baseModelLogText : this.customModelLogText;
            //this.WriteLine(log, "Intermediate result: {0} ", e.Result.Text);
        }

        private void RecognizedEventHandler(object _, SpeechRecognitionEventArgs e)
        {
            //TODO: Handle e.Result.Reason?

            //TODO Split up text based on length

            foreach (string line in GetLines(e.Result.Text))
            {
                SubtitleLines.Add(line);
            }
        }

        private const int FontSize = 36;

        private static Typeface Typeface { get; } = new Typeface(
            new FontFamily("Arial"),
            FontStyles.Italic,
            FontWeights.Normal,
            FontStretches.Normal);

        private static IEnumerable<string> GetLines(string text, double maxWidth = 740)
        {
            string[] words = text.Split(' ');

            var sb = new StringBuilder();

            for (var index = 0; index < words.Length; index++)
            {
                var word = words[index];
                //TODO: Don't put a space at the beginning
                sb.Append(' ');
                sb.Append(word);

                double width = GetWidth(sb.ToString());
                if (width > maxWidth)
                {
                    sb.Remove(sb.Length - word.Length - 1, word.Length + 1);
                    yield return sb.ToString().Trim();
                    sb.Clear();
                    index--;
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString().Trim();
            }
            static double GetWidth(string @string)
            {
                FormattedText formattedText = new FormattedText(@string,
                    Thread.CurrentThread.CurrentCulture,
                    FlowDirection.LeftToRight,
                    Typeface,
                    FontSize,
                    Brushes.Black,
                    1);
                return formattedText.Width;
            }
        }

        private const string FilePath = "output.txt";

        private async Task WriteSubtitles()
        {
            while (true)
            {
                string line = "";
                lock (SubtitleLines)
                {
                    if (SubtitleLines.Count > 0)
                    {
                        line = SubtitleLines[0];
                        SubtitleLines.RemoveAt(0);
                    }
                }

                DisplayedLine = line;
                await File.WriteAllTextAsync(FilePath, line);

                await Task.Delay(TimeSpan.FromSeconds(2.5));
            }
        }
    }
}
