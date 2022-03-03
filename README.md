# PlaylistDownloader
Tool for downloading files from playlists.

### Prerequisites (macOS only)

1. [Download and install latest version of .NET](https://dotnet.microsoft.com/en-us/download/dotnet)
2. Clone this repository
3. Build and deploy:
```
$ ./deploy.sh
```

### Usage
1. Navigate to ~/Files/Software/Tools/PlaylistDownloader
2. Run the executable:
```
$ ./PlaylistDownloaderConsole <path-to-playlist-file>
```
The playlist can be any text file listing urls one per line:
```
url1
url2
url3
```
