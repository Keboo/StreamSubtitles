namespace StreamSubtitles
{
    public interface ISpeechRecognizerFactory
    {
        ISpeechRecognizer GetRecognizer();
    }
}