using NUnit.Framework;

namespace Sweep.Core.Marking.Representation
{

    [TestFixture]
    public class TrackNameModelParserTest
    {
        [Test]
        [TestCase("Artist   -   Song_name_(my_remix)")]
        [TestCase("Artist_-_Song_name_(my_remix")]
        [TestCase("Artist-_Song_NAME_(my_remiX")]
        [TestCase("Artist-Song_name_(my_remix")]
        public void ParsesCorrectlyVarietyOfStyles(string fileName) {
            var model = TrackNameModel.Parser.Parse(fileName);
            Assert.AreEqual("Artist - Song name (My remix)", model.FullName);
        }
    }
}
