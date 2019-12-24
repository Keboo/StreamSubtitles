using System.Linq;

namespace StreamSubtitles.Tests
{
    public static class Factory
    {
        public static ISpeechRecognizerFactory GetRecognizerFactory(params string[] words)
        {
            return new Simulators.SpeechRecognizerFactory
            {
                Words = words.Any() ? words : null
            };
        }

        public static ISpeechRecognizer GetRecognizer()
        {
            return GetRecognizerFactory().GetRecognizer();
        }
    }

}