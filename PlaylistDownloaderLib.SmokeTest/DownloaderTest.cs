using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PlaylistDownloaderLib.SmokeTest
{
    public class Tests
    {
        private static readonly Uri RemoteFileUri = new ("http://archive.org/download/gd1976-06-03.123608.sbd.miller.flac24/gd76-06-03s2t01.mp3");
        private static readonly string TestDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "gd1976-06-03.123608.sbd.miller.flac24");
        private static readonly string TestFilePath = Path.Combine(TestDirectoryPath, "gd76-06-03s2t01.mp3");
    
        private Downloader _downloader = null!;
    
        private static void RemoveTestDirectory()
        {
            if (Directory.Exists(TestDirectoryPath))
                Directory.Delete(TestDirectoryPath, true);
        }
    
        [SetUp]
        public void Setup()
        {
            RemoveTestDirectory();
        
            var directoryWrapper = new DirectoryWrapper();
            var fileWrapper = new FileWrapper();
            var destinationPathBuilder = new DestinationPathBuilder(directoryWrapper, fileWrapper);
            var httpClientWrapper = new HttpClientWrapper();
    
            _downloader = new Downloader(
                destinationPathBuilder,
                httpClientWrapper);
        }

        [TearDown]
        public void TearDown()
        {
            RemoveTestDirectory();
        }

        [Test]
        public async Task CanDownloadFiles()
        {
            // arrange
            Assert.IsFalse(File.Exists(TestFilePath));
            var uris = new List<Uri> { RemoteFileUri };
        
            // act
            await _downloader.DownloadFilesAsync(uris);
        
            // assert
            Assert.IsTrue(File.Exists(TestFilePath));
        }
    }
}