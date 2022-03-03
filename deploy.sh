#!/bin/bash

dotnet build -c Release PlaylistDownloader.sln
rm -rf ~/Files/Software/Tools/PlaylistDownloader
mkdir ~/Files/Software/Tools/PlaylistDownloader
cp -r ./PlaylistDownloaderConsole/bin/Release/net6.0/ ~/Files/Software/Tools/PlaylistDownloader

