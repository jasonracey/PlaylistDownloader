using System;

namespace PlaylistDownloaderLib
{
    public sealed class DownloadResult
    {
        public Uri DownloadUri { get; }

        public DownloadResultType DownloadResultType { get; }

        public string DownloadResultMessage { get; }

        public DownloadResult(
            Uri downloadUri,
            DownloadResultType downloadResultType,
            string downloadResultMessage)
        {
            if (string.IsNullOrWhiteSpace(downloadResultMessage))
                throw new ArgumentNullException(nameof(downloadResultMessage));
            
            DownloadUri = downloadUri ?? throw new ArgumentNullException(nameof(downloadUri));
            DownloadResultType = downloadResultType;
            DownloadResultMessage = downloadResultMessage;
        }
    }
}