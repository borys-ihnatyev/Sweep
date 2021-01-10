using System;
using System.IO;
using NUnit.Framework;

namespace Sweep.Core.Processing.AutoFolder
{
    [TestFixture]
    public class MusicFolderLocationParserTest : FileUsageTestFixture
    {
        [Test]
        public void OnCreateThrow_FileNotFoundException()
        {
            Assert.That(() => {
                MusicFolderLocation.Parse(Guid.NewGuid().ToString());
            }, Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void OnCreate_Throw_FileNotFoundWhenDirectoryPathPassed()
        {

            Assert.That(() => {
                var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
            MusicFolderLocation.Parse(dir.FullName);
            }, Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void OnCreate_Ok_WhenPassedFilePathExists()
        {
            new MusicFolderLocation.Parser(CreateFile());
        }

        [Test]
        public void ReturnsValid_RootPath()
        {
            var musicFolder = new MusicFolder("test", true);
            var newFilePath = musicFolder.MoveFile(CreateFile()).NewFilePath;
            var folderLocation = MusicFolderLocation.Parse(newFilePath);

            Assert.True(folderLocation.RootPath.Equals(musicFolder.Path, StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ReturnsValid_InnerFolderNumber()
        {
            var musicFolder = new MusicFolder("test", true);
            var newFilePath = musicFolder.MoveFile(CreateFile()).NewFilePath;
            const int expectedInnerFolder = 0;
            var folderLocation = MusicFolderLocation.Parse(newFilePath);
            Assert.AreEqual(expectedInnerFolder, folderLocation.InnerFolder);
        }

        [Test]
        public void ReturnsValid_Year()
        {
            var musicFolder = new MusicFolder("test", true);
            var newFilePath = musicFolder.MoveFile(CreateFile()).NewFilePath;
            var expectedYear = DateTime.Now.Year;
            var folderLocation = MusicFolderLocation.Parse(newFilePath);
            Assert.AreEqual(expectedYear,folderLocation.Year);
        }
    }
}
