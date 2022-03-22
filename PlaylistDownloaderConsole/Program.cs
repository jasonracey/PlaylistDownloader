using System;
using System.IO;
using System.Linq;
using PlaylistDownloaderLib;

namespace PlaylistDownloaderConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var directoryWrapper = new DirectoryWrapper();
            var fileWrapper = new FileWrapper();
            var destinationPathBuilder = new DestinationPathBuilder(directoryWrapper, fileWrapper);
            var httpClientWrapper = new HttpClientWrapper();
            
            Console.WriteLine();
            Console.WriteLine("Playlist Downloader");
            Console.WriteLine("by https://github.com/jasonracey");
            Console.WriteLine();

            var input = args[0];

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please specify a file path.");
            }
            else if (!File.Exists(input))
            {
                Console.WriteLine("File not found.");
            }
            else
            {
                var downloader = new Downloader(
                    destinationPathBuilder,
                    httpClientWrapper);
    
                var uris = File
                    .ReadAllLines(input)
                    .Select(TryCreateUri)
                    .NotNull();
    
                Console.WriteLine("Downloading files...");
    
                downloader.DownloadFilesAsync(uris);
    
                Console.WriteLine();
                Console.WriteLine("Thanks for using Playlist Downloader.");
                Console.WriteLine();
            }
        }

        private static Uri? TryCreateUri(string path) 
        {
            return Uri.TryCreate(path, UriKind.Absolute, out var uri) 
                ? uri 
                : null; 
        }
    }
}