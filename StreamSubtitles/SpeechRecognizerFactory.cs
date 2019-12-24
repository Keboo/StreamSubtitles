using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace StreamSubtitles
{
    public class SpeechRecognizerFactory : ISpeechRecognizerFactory
    {
        private string Region { get; } = "westus";
        private string RecognitionLanguage { get; } = "en-US";

        public ISpeechRecognizer GetRecognizer()
        {
            SpeechConfig config = SpeechConfig.FromSubscription(Environment.GetEnvironmentVariable("StreamSubtitlesKey"), Region);
            config.SpeechRecognitionLanguage = RecognitionLanguage;
            return new SpeechRecognizerWrapper(new SpeechRecognizer(config));
        }

        private class SpeechRecognizerWrapper : ISpeechRecognizer
        {
            private readonly SpeechRecognizer _SpeechRecognizer;

            public event EventHandler<string> Recognizing;
            public event EventHandler<string> Recognized;

            public SpeechRecognizerWrapper(SpeechRecognizer speechRecognizer)
            {
                _SpeechRecognizer = speechRecognizer ?? throw new ArgumentNullException(nameof(speechRecognizer));
                _SpeechRecognizer.Recognizing += SpeechRecognizerOnRecognizing;
                _SpeechRecognizer.Recognized += SpeechRecognizerOnRecognized;
            }

            private void SpeechRecognizerOnRecognized(object? sender, SpeechRecognitionEventArgs e)
                => Recognized?.Invoke(this, e.Result.Text);

            private void SpeechRecognizerOnRecognizing(object? sender, SpeechRecognitionEventArgs e) 
                => Recognizing?.Invoke(this, e.Result.Text);

            public void Dispose()
            {
                _SpeechRecognizer.Dispose();
            }

            public async Task StartRecognitionAsync() 
                => await _SpeechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        }
    }
}