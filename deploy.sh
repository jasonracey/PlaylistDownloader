#!/bin/bash

# deploy console
echo "Deploying Console..."
rm -Rf ../../Tools/PlaylistDownloader
mkdir ../../Tools/PlaylistDownloader
cp -R ./PlaylistDownloaderConsole/bin/Release/netstandard2.1/ ../../Tools/PlaylistDownloader

# deploy app
echo "Deploying UI..."
rm -Rf /Applications/Playlist\ Downloader.app
cp -Rf ./PlaylistDownloaderUI/bin/Release/Playlist\ Downloader.app /Applications/Playlist\ Downloader.app

echo "Done!"
