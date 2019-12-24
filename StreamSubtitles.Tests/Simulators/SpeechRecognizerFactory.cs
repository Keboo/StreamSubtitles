using System.Collections.Generic;

namespace StreamSubtitles.Tests.Simulators
{
    public class SpeechRecognizerFactory : ISpeechRecognizerFactory
    {
        public IEnumerable<string>? Words { get; set; }

        public ISpeechRecognizer GetRecognizer() => new SpeechRecognizer(Words);
    }
}