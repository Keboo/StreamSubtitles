using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamSubtitles.Tests.Simulators
{
    public class SpeechRecognizer : ISpeechRecognizer
    {
        private IEnumerable<string>? Words { get; }
        private StringBuilder CurrentText { get; } = new StringBuilder();
        private CancellationTokenSource Cts { get; } = new CancellationTokenSource();

        public SpeechRecognizer(IEnumerable<string>? words)
        {
            Words = words;
        }

        public void Dispose()
        {
            Cts.Cancel();
        }

        public event EventHandler<string>? Recognizing;
        public event EventHandler<string>? Recognized;

        public Task StartRecognitionAsync()
        {
            SimulateText();
            return Task.CompletedTask;
        }

        private void SimulateText()
        {
            const int wordCount = 16;

            foreach (string word in Words ?? GetWords().Take(wordCount))
            {
                CurrentText.Append(word);
                CurrentText.Append(' ');
                Recognizing?.Invoke(this, CurrentText.ToString());
            }

            Recognized?.Invoke(this, CurrentText.ToString());
            CurrentText.Clear();
        }

        private static IEnumerable<string> GetWords()
        {
            yield return "Lorem";
            yield return "ipsum";
            yield return "dolor";
            yield return "sit";
            yield return "amet,";
            yield return "consectetur";
            yield return "adipiscing";
            yield return "elit.Praesent";
            yield return "auctor";
            yield return "lectus";
            yield return "velit,";
            yield return "pretium";
            yield return "placerat";
            yield return "justo";
            yield return "cursus";
            yield return "non.";
            yield return "Vestibulum";
            yield return "condimentum";
            yield return "eget";
            yield return "est";
            yield return "et";
            yield return "posuere.";
            yield return "Curabitur";
            yield return "et";
            yield return "massa";
            yield return "vel";
            yield return "eros";
            yield return "volutpat";
            yield return "lobortis";
            yield return "vel";
            yield return "vel";
            yield return "neque.";
            yield return "Ut";
            yield return "dolor";
            yield return "est,";
            yield return "aliquet";
            yield return "vitae";
            yield return "condimentum";
            yield return "non,";
            yield return "facilisis";
            yield return "eu";
            yield return "est.";
            yield return "Sed";
            yield return "eros";
            yield return "magna,";
            yield return "euismod";
            yield return "et";
            yield return "condimentum";
            yield return "et.";
        }
    }
}