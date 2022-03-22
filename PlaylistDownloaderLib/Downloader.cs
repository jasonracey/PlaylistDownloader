using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistDownloaderLib
{
    public interface IDownloader
{
    int CompletedDownloadAttempts { get; }
    string State { get; }
    int TotalFiles { get; }

    Task<IEnumerable<DownloadResult>> DownloadFilesAsync(IEnumerable<Uri> uris);
}

    public class Downloader : IDownloader
    {
        private readonly IDestinationPathBuilder _destinationPathBuilder;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public string State { get; private set; }

        public int CompletedDownloadAttempts { get; private set; }

        public int TotalFiles { get; private set; }

        public Downloader(
            IDestinationPathBuilder destinationPathBuilder,
            IHttpClientWrapper httpClientWrapper)
        {
            _destinationPathBuilder =
                destinationPathBuilder ?? throw new ArgumentNullException(nameof(destinationPathBuilder));
            _httpClientWrapper = httpClientWrapper ?? throw new ArgumentNullException(nameof(httpClientWrapper));
            State = string.Empty;
        }

        public async Task<IEnumerable<DownloadResult>> DownloadFilesAsync(IEnumerable<Uri> uris)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));

            var uriArray = uris.ToArray();

            CompletedDownloadAttempts = 0;
            State = "Downloading...";
            TotalFiles = uriArray.Length;

            var results = new List<DownloadResult>();

            await Task.WhenAll(uriArray.Select(async uri =>
            {
                try
                {
                    await DownloadFileAsync(uri).ConfigureAwait(false);
                    results.Add(new DownloadResult(uri, DownloadResultType.Success,
                        DownloadResultType.Success.ToString()));
                }
                catch (Exception ex)
                {
                    results.Add(new DownloadResult(uri, DownloadResultType.Failure, ex.Message));
                }
                finally
                {
                    CompletedDownloadAttempts++;
                }
            }));

            return results;
        }

        private async Task DownloadFileAsync(Uri uri)
        {
            var destinationPath = _destinationPathBuilder.CreateDestinationPath(uri);
            await _httpClientWrapper.DownloadFileAsync(uri, destinationPath).ConfigureAwait(false);
        }
    }
}