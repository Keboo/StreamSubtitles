using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StreamSubtitles.Tests
{
    [TestClass]
    public class LineSplitterTests
    {
        [DataTestMethod]
        [DataRow("Lorem ipsum dolor sit amet, consectetur")]
        [DataRow("Lorem ipsum dolor sit amet,")]
        [DataRow("Lorem ipsum dolor sit")]
        [DataRow("Lorem ipsum dolor")]
        [DataRow("Lorem ipsum")]
        [DataRow("Lorem")]
        public void GetLines_ReturnsSingleLine(string text)
        {
            // Arrange

            // Act
            string line = LineSplitter.GetLines(text).Single();

            // Assert
            Assert.AreEqual(text, line);
        }

        [TestMethod]
        public void GetLines_SplitsIntoMultipleLines()
        {
            // Arrange
            string text =
                @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent auctor lectus velit, pretium placerat justo cursus non. Vestibulum condimentum eget est et posuere. Curabitur et massa vel eros volutpat lobortis vel vel neque. Ut dolor est, aliquet vitae condimentum non, facilisis eu est. Sed eros magna, euismod et condimentum et.";

            // Act
            string[] lines = LineSplitter.GetLines(text).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Lorem ipsum dolor sit amet, consectetur",
                "adipiscing elit. Praesent auctor lectus velit,",
                "pretium placerat justo cursus non. Vestibulum",
                "condimentum eget est et posuere. Curabitur",
                "et massa vel eros volutpat lobortis vel vel",
                "neque. Ut dolor est, aliquet vitae",
                "condimentum non, facilisis eu est. Sed eros",
                "magna, euismod et condimentum et.",
            }, lines);
        }
    }
}