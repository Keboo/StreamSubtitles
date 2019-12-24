using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace StreamSubtitles
{
    public static class LineSplitter
    {
        private const int FontSize = 36;

        private static Typeface Typeface { get; } = new Typeface(
            new FontFamily("Arial"),
            FontStyles.Italic,
            FontWeights.Normal,
            FontStretches.Normal);

        public static IEnumerable<string> GetLines(string text, double maxWidth = 740)
        {
            string[] words = text.Split(' ');

            var sb = new StringBuilder();

            for (var index = 0; index < words.Length; index++)
            {
                var word = words[index];
                if (index > 0)
                {
                    sb.Append(' ');
                }
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
    }
}