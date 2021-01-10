using System.Collections.Generic;
using Sweep.Core.Marking;
using Sweep.Core.Marking.Representation;
using NUnit.Framework;

namespace Sweep.Core
{
    [TestFixture]
    public class KeyTest
    {
        private static IEnumerable<string> CreateTestKeyStrings(KeyNotation keyNotation)
        {
            const int notesCount = 12;
            const string hashMark = "#";

            for (var i = 0; i < notesCount; i++)
            {
                yield return hashMark + new Key((Note) i, Tone.Moll).ToString(keyNotation).ToLower();
                yield return hashMark + new Key((Note) i, Tone.Dur).ToString(keyNotation).ToLower();
            }
        }

        [Test]
        [TestCase(KeyNotation.Is_M)]
        [TestCase(KeyNotation.IsMinMaj)]
        [TestCase(KeyNotation.IsMinorMajor)]
        [TestCase(KeyNotation.IsMollDur)]
        [TestCase(KeyNotation.IsStrip_M)]
        [TestCase(KeyNotation.IsStripMinMaj)]
        [TestCase(KeyNotation.IsStripMinorMajor)]
        [TestCase(KeyNotation.IsStripMollDur)]
        [TestCase(KeyNotation.Sharp_M)]
        [TestCase(KeyNotation.Sharp_StripM)]
        [TestCase(KeyNotation.SharpMajorMinor)]
        [TestCase(KeyNotation.SharpMinMaj)]
        [TestCase(KeyNotation.SharpMollDur)]
        [TestCase(KeyNotation.SharpStripMajorMinor)]
        [TestCase(KeyNotation.SharpStripMinMaj)]
        [TestCase(KeyNotation.SharpStripMollDur)]
        public void TestGetKeyHashEntry(KeyNotation keyNotation)
        {
            var testKeyStrings = CreateTestKeyStrings(keyNotation);
            foreach (var keyString in testKeyStrings)
            {
                var keyEntry = HashTag.Parser.First(keyString);

                var keyHashTag = keyEntry.HashTag as KeyHashTag;
                Assert.NotNull(keyHashTag, "Not Founded key in \"{0}\"", keyString);

                var expectedKeyString = "#" + keyHashTag.Key.ToString(keyNotation);

                Assert.AreEqual(expectedKeyString, keyString);
            }
        }

        [Test]
        public void TestParseInvalidHashTagModel()
        {
            const string invalidHashTagModelString = "sddskl sdlksl ds";
            var model = HashTagModel.Parser.All(invalidHashTagModelString);
            Assert.AreEqual(0, model.Count);
        }

        [Test]
        public void TestValidHashTagModelWithoutKeyHashTags()
        {
            const string invalidHashTagModelString = "#sddskl #sdlksl #ds";
            var model = HashTagModel.Parser.All(invalidHashTagModelString);
            Assert.AreEqual(3, model.Count);
        }

        [Test]
        public void TestKeyEqualsOperator()
        {
            var amoll1 = new Key(Note.A, Tone.Moll);
            var amoll2 = new Key(Note.A, Tone.Moll);

            Assert.True(amoll1 == amoll2);
        }

        [Test]
        public void TestKeyNotEqualsOperator()
        {
            var amoll = new Key(Note.A, Tone.Moll);
            var adur = new Key(Note.A, Tone.Dur);

            Assert.True(adur != amoll);
        }

        [Test]
        public void TestKeyEqualsOverride()
        {
            var amoll1 = new Key(Note.A, Tone.Moll);
            var amoll2 = new Key(Note.A, Tone.Moll);

            Assert.AreEqual(amoll1, amoll2);
            Assert.AreNotSame(amoll1, amoll2);
        }

        [Test]
        public void TestKeyHashCodeOverride()
        {
            var amoll1 = new Key(Note.A, Tone.Moll);
            var amoll2 = new Key(Note.A, Tone.Moll);

            Assert.AreEqual(amoll1.GetHashCode(), amoll2.GetHashCode());
        }
    }
}
