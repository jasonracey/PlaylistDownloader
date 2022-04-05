using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppKit;
using Foundation;
using PlaylistDownloaderLib;

namespace PlaylistDownloaderUI
{
    public partial class ViewController : NSViewController
    {
        private const int OpenFile = 1;
        
        private static readonly string[] PlaylistFileTypes = { "m3u" };
        
        private static readonly string DownloadsPath = Path.Combine(
            Environment.SpecialFolder.UserProfile.ToString(), 
            "Downloads");
        
        private static readonly  NSOpenPanel OpenPanel = new NSOpenPanel
        {
            AllowedFileTypes = PlaylistFileTypes,
            AllowsMultipleSelection = false,
            CanChooseDirectories = false,
            CanCreateDirectories = false,
            Directory = DownloadsPath,
            ReleasedWhenClosed = false,
            ShowsHiddenFiles = false,
            ShowsResizeIndicator = true
        };
        
        private static readonly Downloader Downloader = new Downloader(
            new DestinationPathBuilder(
                new DirectoryWrapper(), 
                new FileWrapper()), 
            new HttpClientWrapper());
        
        private IEnumerable<Uri> _uris = Enumerable.Empty<Uri>();
        
        private NSTimer _timer;
        
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Progress.Indeterminate = false;
            Status.StringValue = string.Empty;
            
            SetIdleState();
        }

        partial void ClearClicked(NSButton sender)
        {
            SetIdleState();
        }
        
        async partial void DownloadClicked(NSButton sender)
        {
            SetBusyState();
            StartTimer();

            try
            {
                await Downloader.DownloadFilesAsync(_uris);
                SetIdleState();
            }
            catch (Exception e)
            {
                SetErrorState($"Error: {e.Message}");
            }
            finally
            {
                StopTimer();
            }
        }

        partial void AddClicked(NSButton sender)
        {
            var result = OpenPanel.RunModal();
            if (result != OpenFile) return;

            _uris = File
                .ReadAllLines(OpenPanel.Url.Path)
                .Select(line => new Uri(line));

            var builder = new StringBuilder();
            foreach (var uri in _uris)
            {
                builder.AppendLine(uri.OriginalString);
            }
            TextField.StringValue = builder.ToString();

            SetAddedState();
        }
        
        private static double GetPercentCompleted()
        {
            var completed = Downloader?.ProcessingStatus?.CountCompleted ?? 0.0D;
            var total = Downloader?.ProcessingStatus?.CountTotal ?? 0.0D ;
            return total == 0.0D 
                ? 0.0D 
                : 100 * completed / total;
        }

        private static string GetStatusMessage()
        {
            return Downloader?.ProcessingStatus?.Message ?? string.Empty;
        }

        private void SetAddedState()
        {
            TextField.Editable = false;
            Progress.DoubleValue = 0.0D;
            Progress.Hidden = true;
            Status.StringValue = string.Empty;
            Status.TextColor = NSColor.White;
            Clear.Enabled = true;
            Download.Enabled = true;
            Add.Enabled = false;
        }
        
        private void SetBusyState()
        {
            TextField.Editable = false;
            Progress.DoubleValue = 0.0D;
            Progress.Hidden = false;
            Status.StringValue = string.Empty;
            Status.TextColor = NSColor.White;
            Clear.Enabled = false;
            Download.Enabled = false;
            Add.Enabled = false;
        }

        private void SetErrorState(string message)
        {
            TextField.StringValue = string.Empty;
            TextField.Editable = false;
            Progress.DoubleValue = 0.0D;
            Progress.Hidden = true;
            Status.StringValue = message;
            Status.TextColor = NSColor.Red;
            Clear.Enabled = false;
            Download.Enabled = false;
            Add.Enabled = true;
        }

        private void SetIdleState()
        {
            TextField.StringValue = string.Empty;
            TextField.Editable = false;
            Progress.DoubleValue = 0.0D;
            Progress.Hidden = true;
            Status.StringValue = string.Empty;
            Status.TextColor = NSColor.White;
            Clear.Enabled = false;
            Download.Enabled = false;
            Add.Enabled = true;
        }
        
        private void StartTimer()
        {
            StopTimer();

            const double seconds = 0.25D;
            
            _timer = NSTimer.CreateRepeatingScheduledTimer(seconds, _ => {
                Progress.DoubleValue = GetPercentCompleted();
                Status.StringValue = GetStatusMessage();
            });
        }

        private void StopTimer()
        {
            if (_timer == null) return;
            _timer.Invalidate();
            _timer.Dispose();
            _timer = null;
        }
    }
}