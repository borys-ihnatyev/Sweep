using System;
using System.IO;
using Sweep.Core.Marking.Representation;
using NUnit.Framework;

namespace Sweep.Core.Processing.AutoFolder
{
    [TestFixture]
    public class HashTagFolderCollectionTest
    {
        private const string fixturesFolder = "/Users/borys/Workspace/MMK platform transition/Sweep/Core.Test/fixtures";

        [Test]
        public void ThrowsException_OnAddFolderWithSinglePatternThatExists() {
            Assert.That(
                () => {
                    new HashTagFolderCollection {
                        {fixturesFolder + @"/music/pop/", "#pop"},
                        {fixturesFolder + @"/music/deep/", "#pop"},
                    };
                },
                Throws.TypeOf<HashTagFolderCollection.PatternAlreadyExistsException>()
            );
        }


        [Test]
        public void ThrowsException_OnAddFolderWithMultiplePatterns_WhichSomeExists()
        {
            Assert.That(
                () => {
                     new HashTagFolderCollection {
                        {fixturesFolder + @"/music/house/", "#pop #house"},
                        {fixturesFolder + @"/music/house/", "#house"},
                        {fixturesFolder + @"/music/pop/", "#pop #house"},
                        {fixturesFolder + @"/music/pop/", "#pop"}
                    };
                },
                Throws.TypeOf<HashTagFolderCollection.PatternAlreadyExistsException>()
            );
        }

        [Test]
        public void ThrowsException_OnAddFolderWithSinglePatternThatExists_Advenced()
        {
               Assert.That(
                () => {
                     new HashTagFolderCollection
            {
                {fixturesFolder + @"/music/pop/", "#pop #house"},
                {fixturesFolder + @"/music/deep/", "#house #pop"}
            };
                },
                Throws.TypeOf<HashTagFolderCollection.PatternAlreadyExistsException>()
            );

        }

        [Test]
        public void NotThrowsException_OnAddFolderWithSinglePatternThatHasSameHashTag()
        {
            var collection = new HashTagFolderCollection
            {
                {fixturesFolder + @"/music/pop/", "#pop #house"}
            };
            collection.Add(fixturesFolder + @"/music/deep/", "#pop");
        }

        [Test]
        public void GetMatchPath_WhenExists_1()
        {
            var popFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/pop/");
            var houseFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/house/");
            var mainFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/deep/");
            var specFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/spec/");

            var collection = new HashTagFolderCollection
            {
                {popFolderPath, "#pop"},
                {houseFolderPath, "#house"},
                {houseFolderPath, "#house #pop"},
                {mainFolderPath, "#deep"},
                {mainFolderPath, "#deep #house"},
                {mainFolderPath, "#deep #house #pop"},
                {mainFolderPath, "#newdisco"},
                {specFolderPath, "#spec"},
                {specFolderPath, "#trap"},
                {specFolderPath, "#rnb"},
                {specFolderPath, "#pop #spec"},
                {specFolderPath, "#house #spec"},
                {specFolderPath, "#house #spec #pop"},
            };

            Assert.AreEqual(houseFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#pop #house #entrophy")));
            Assert.AreEqual(popFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#entrophy #pop")));
            Assert.AreEqual(houseFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#house #entrophy")));
            Assert.AreEqual(specFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#spec #entrophy")));
            Assert.AreEqual(specFolderPath,
                collection.GetMatchPath(HashTagModel.Parser.All("#spec #pop #house #entrophy")));
            Assert.AreEqual(specFolderPath,
                collection.GetMatchPath(HashTagModel.Parser.All("#spec #pop #house #deep #entrophy")));
            Assert.AreEqual(mainFolderPath,
                collection.GetMatchPath(HashTagModel.Parser.All("#pop #house #deep #entrophy")));
        }

        [Test]
        public void MustMoveToMorePrioritySubsetofModel()
        {
            const string popPath = fixturesFolder + @"/music/pop";
            const string housePath = fixturesFolder + @"/music/house";
            const string mainPath = fixturesFolder + @"/music/deep";

            var collection = new HashTagFolderCollection
            {
                {housePath, "#house #deep"},
                {popPath, "#pop"},
                {housePath, "#house", 1},
                {mainPath, "#deep", 2},
            };

            const string expectedPath = mainPath;
            var actualPath = collection.GetMatchPath(HashTagModel.Parser.All("#pop #house #deep"));

            Assert.AreEqual(expectedPath, actualPath);
        }

        private static HashTagFolderCollection MakeTestCollection()
        {
            var popFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/pop/");
            var houseFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/house/");
            var mainFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/deep/");
            var specFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/spec/");
            var mixesFolderPath = PathExtension.Normalize(fixturesFolder + @"/music/mixes/");

            return new HashTagFolderCollection
            {
                {popFolderPath, "#pop"},
                {popFolderPath, "#rus"},
                {popFolderPath, "#pop #rus"},
                {popFolderPath, "#ukr"},
                {popFolderPath, "#pop #ukr"},
                {houseFolderPath, "#house"},
                {houseFolderPath, "#house #pop"},
                {mainFolderPath, "#deep"},
                {mainFolderPath, "#deep #house"},
                {mainFolderPath, "#deep #house #pop"},
                {mainFolderPath, "#newdisco"},
                {specFolderPath, "#spec"},
                {specFolderPath, "#trap"},
                {specFolderPath, "#rnb"},
                {specFolderPath, "#pop #spec"},
                {specFolderPath, "#house #spec"},
                {specFolderPath, "#house #spec #pop"},
                {mixesFolderPath, "#mix"},
                {mixesFolderPath, "#mix #pop"},
                {mixesFolderPath, "#mix #deep"},
                {mixesFolderPath, "#mix #house"},
                {mixesFolderPath, "#mix #deep #house"},
                {mixesFolderPath, "#mix #deep #house #pop"},
                {mixesFolderPath, "#mix #newdisco"},
            };
        }
    }
}
