using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace PlaylistDownloaderLib.UnitTest
{
    [TestFixture]
    public class DownloaderTest
    {
        private Mock<IDestinationPathBuilder> _mockDestinationPathBuilder = null!;
        private Mock<IHttpClientWrapper> _mockHttpClientWrapper = null!;

        private Downloader _downloader = null!;

        [SetUp]
        public void TestInitialize()
        {
            _mockDestinationPathBuilder = new Mock<IDestinationPathBuilder>();
            _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            _downloader = new Downloader(_mockDestinationPathBuilder.Object, _mockHttpClientWrapper.Object);
        }

        [Test]
        public void WhenConstructorArgNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new Downloader(null!, _mockHttpClientWrapper.Object);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new Downloader(_mockDestinationPathBuilder.Object, null!);
            });
            Assert.IsNotNull(new Downloader(_mockDestinationPathBuilder.Object, _mockHttpClientWrapper.Object));
        }

        [Test]
        public void WhenUrisNull_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _downloader.DownloadFilesAsync(null!));
        }

        [Test]
        public async Task WhenUrisEmpty_Skipped()
        {
            // act
            await _downloader.DownloadFilesAsync(new List<Uri>());

            // assert
            Assert.AreEqual("Downloading...", _downloader.ProcessingStatus?.Message);
            Assert.AreEqual(0, _downloader.ProcessingStatus?.CountTotal);
            Assert.AreEqual(0, _downloader.ProcessingStatus?.CountCompleted);
        }

        [Test]
        public async Task WhenDownloadSucceeds_UpdatesCount_AndReturnsSuccessResult()
        {
            // arrange
            var uris = new List<Uri>
            {
                new("https://www.contoso.com/path/file1"),
                new("https://www.contoso.com/path/file2"),
                new("https://www.contoso.com/path/file3"),
            };

            // act
            await _downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual("Downloaded 3/3", _downloader.ProcessingStatus?.Message);
            Assert.AreEqual(uris.Count, _downloader.ProcessingStatus?.CountTotal);
            Assert.AreEqual(uris.Count, _downloader.ProcessingStatus?.CountCompleted);
        }

        [Test]
        public async Task WhenDownloadStarts_ResetsCount()
        {
            // arrange
            var uris = new List<Uri>
            {
                new("https://www.contoso.com/path/file1"),
                new("https://www.contoso.com/path/file2"),
                new("https://www.contoso.com/path/file3"),
            };

            // act
            await _downloader.DownloadFilesAsync(uris);
            await _downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual(uris.Count, _downloader.ProcessingStatus?.CountCompleted);
        }

        [Test]
        public async Task WhenDownloadFails_UpdatesCount_AndReturnsFailureResult()
        {
            //arrange
            const string mockMessage = "mock error message";
            var uris = new List<Uri>
            {
                new("https://www.contoso.com/path/file1"),
            };
            _mockHttpClientWrapper
                .Setup(m => m.DownloadFileAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(mockMessage));

            // act
            await _downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual("Downloaded 1/1", _downloader.ProcessingStatus?.Message);
            Assert.AreEqual(uris.Count, _downloader.ProcessingStatus?.CountTotal);
            Assert.AreEqual(uris.Count, _downloader.ProcessingStatus?.CountCompleted);
        }
    }
}