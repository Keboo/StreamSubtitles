using System;
using System.Threading.Tasks;

namespace StreamSubtitles
{
    public interface ISpeechRecognizer : IDisposable
    {
        event EventHandler<string> Recognizing;
        event EventHandler<string> Recognized;

        Task StartRecognitionAsync();
    }
}