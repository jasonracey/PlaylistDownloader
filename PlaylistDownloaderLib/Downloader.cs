using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlaylistDownloaderLib
{
    public interface IDownloader
    {
        ProcessingStatus? ProcessingStatus { get; }

        Task<IEnumerable<DownloadResult>> DownloadFilesAsync(IEnumerable<Uri> uris);
    }

    public class Downloader : IDownloader
    {
        private readonly IDestinationPathBuilder _destinationPathBuilder;
        private readonly IHttpClientWrapper _httpClientWrapper;
        
        private readonly object _resultsLock = new object();
        private readonly object _statusLock = new object();

        public ProcessingStatus? ProcessingStatus { get; private set; }

        public Downloader(
            IDestinationPathBuilder destinationPathBuilder,
            IHttpClientWrapper httpClientWrapper)
        {
            _destinationPathBuilder = destinationPathBuilder ?? throw new ArgumentNullException(nameof(destinationPathBuilder));
            _httpClientWrapper = httpClientWrapper ?? throw new ArgumentNullException(nameof(httpClientWrapper));
            SetState(0, 0);
        }

        public async Task<IEnumerable<DownloadResult>> DownloadFilesAsync(IEnumerable<Uri> uris)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));

            var uriArray = uris.ToArray();

            SetState(0, uriArray.Length, "Downloading...");

            var results = new List<DownloadResult>();
            
            var attemptsCompleted = 0;
            await Task.WhenAll(uriArray.Select(async uri =>
            {
                try
                {
                    await DownloadFileAsync(uri).ConfigureAwait(false);
                    lock (_resultsLock)
                    {
                        results.Add(new DownloadResult(uri, DownloadResultType.Success,
                            DownloadResultType.Success.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    lock (_resultsLock)
                    {
                        results.Add(new DownloadResult(uri, DownloadResultType.Failure, ex.Message));
                    }
                }
                finally
                {
                    Interlocked.Increment(ref attemptsCompleted);
                    
                    SetState(
                        countCompleted: attemptsCompleted, 
                        countTotal: uriArray.Length, 
                        message: $"Downloaded {attemptsCompleted}/{uriArray.Length}");
                }
            }));

            return results;
        }

        private async Task DownloadFileAsync(Uri uri)
        {
            var destinationPath = _destinationPathBuilder.CreateDestinationPath(uri);
            await _httpClientWrapper.DownloadFileAsync(uri, destinationPath).ConfigureAwait(false);
        }
        
        private void SetState(int countCompleted, int countTotal, string? message = null)
        {
            lock (_statusLock)
            {
                ProcessingStatus = new ProcessingStatus(countCompleted, countTotal, message);
            }
        }
    }
}