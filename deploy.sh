#!/bin/bash

dotnet build -c Release PlaylistDownloader.sln
rm -rf ../../Tools/PlaylistDownloader
mkdir ../../Tools/PlaylistDownloader
cp -r ./PlaylistDownloaderConsole/bin/Release/net6.0/ ../../Tools/PlaylistDownloader

