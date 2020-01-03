using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace StreamSubtitles
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ISpeechRecognizerFactory _SpeechRecognizerFactory;

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public ObservableCollection<string> SubtitleLines { get; } = new ObservableCollection<string>();

        private string _DisplayedLine = "";
        public string DisplayedLine
        {
            get => _DisplayedLine;
            set => Set(ref _DisplayedLine, value);
        }

        private string? _PendingText;
        public string? PendingText
        {
            get => _PendingText;
            set => Set(ref _PendingText, value);
        }

        private ISpeechRecognizer? _Recognizer;
        public ISpeechRecognizer? Recognizer
        {
            get => _Recognizer;
            set
            {
                ISpeechRecognizer? originalValue = _Recognizer;
                if (Set(ref _Recognizer, value))
                {
                    originalValue?.Dispose();
                }
            }
        }

        public MainWindowViewModel(ISpeechRecognizerFactory speechRecognizerFactory)
        {
            _SpeechRecognizerFactory = speechRecognizerFactory ?? throw new ArgumentNullException(nameof(speechRecognizerFactory));
            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);

            BindingOperations.EnableCollectionSynchronization(SubtitleLines, SubtitleLines);

            Task.Factory.StartNew(WriteSubtitles, TaskCreationOptions.LongRunning);
        }

        private void OnStop()
        {
            if (Recognizer is { } recognizer)
            {
                recognizer.Recognizing -= RecognizingEventHandler;
                recognizer.Recognized -= RecognizedEventHandler;
            }
            Recognizer = null;
            PendingText = null;
        }

        private async void OnStart()
        {
            ISpeechRecognizer recognizer = Recognizer = _SpeechRecognizerFactory.GetRecognizer();

            recognizer.Recognizing += RecognizingEventHandler;
            recognizer.Recognized += RecognizedEventHandler;

            await recognizer.StartRecognitionAsync().ConfigureAwait(false);
        }

        private void RecognizingEventHandler(object? _, string? text)
        {
            PendingText = text;
        }

        private void RecognizedEventHandler(object? _, string text)
        {
            //TODO: Handle e.Result.Reason?
            PendingText = null;
            foreach (string line in LineSplitter.GetLines(text))
            {
                lock (SubtitleLines)
                {
                    SubtitleLines.Add(line);
                }
            }
        }

        private const string FilePath = "output.txt";

        private async Task WriteSubtitles()
        {
            while (true)
            {
                string line = "";
                while (string.IsNullOrWhiteSpace(DisplayedLine) && !SubtitleLines.Any())
                {
                    await Task.Delay(100);
                }
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
