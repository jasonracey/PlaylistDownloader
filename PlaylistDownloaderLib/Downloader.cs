using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistDownloaderLib
{
    public interface IDownloader
    {
        ProcessingStatus? ProcessingStatus { get; }

        Task DownloadFilesAsync(IEnumerable<Uri> uris);
    }

    public class Downloader : IDownloader
    {
        private readonly IDestinationPathBuilder _destinationPathBuilder;
        private readonly IHttpClientWrapper _httpClientWrapper;
        
        private readonly object _downloadCompletionLock = new object();

        public ProcessingStatus? ProcessingStatus { get; private set; }

        public Downloader(
            IDestinationPathBuilder destinationPathBuilder,
            IHttpClientWrapper httpClientWrapper)
        {
            _destinationPathBuilder = destinationPathBuilder ?? throw new ArgumentNullException(nameof(destinationPathBuilder));
            _httpClientWrapper = httpClientWrapper ?? throw new ArgumentNullException(nameof(httpClientWrapper));
            SetState(0, 0);
        }

        public async Task DownloadFilesAsync(IEnumerable<Uri> uris)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));

            var uriArray = uris.ToArray();

            SetState(0, uriArray.Length, "Downloading...");

            var attemptsCompleted = 0;
            await Task.WhenAll(uriArray.Select(async uri =>
            {
                var destinationPath = _destinationPathBuilder.CreateDestinationPath(uri);
                await _httpClientWrapper.DownloadFileAsync(uri, destinationPath).ConfigureAwait(false);

                lock (_downloadCompletionLock)
                {
                    attemptsCompleted++;
                    SetState(
                        countCompleted: attemptsCompleted, 
                        countTotal: uriArray.Length, 
                        message: $"Downloaded {attemptsCompleted}/{uriArray.Length}");
                }
            }));
        }
        
        private void SetState(int countCompleted, int countTotal, string? message = null)
        {
            ProcessingStatus = new ProcessingStatus(countCompleted, countTotal, message);
        }
    }
}