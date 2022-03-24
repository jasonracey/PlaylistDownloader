// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PlaylistDownloaderUI
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton Add { get; set; }

		[Outlet]
		AppKit.NSButton Clear { get; set; }

		[Outlet]
		AppKit.NSButton Download { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator Progress { get; set; }

		[Outlet]
		AppKit.NSTextField Status { get; set; }

		[Outlet]
		AppKit.NSTextField TextField { get; set; }

		[Action ("AddClicked:")]
		partial void AddClicked (AppKit.NSButton sender);

		[Action ("ClearClicked:")]
		partial void ClearClicked (AppKit.NSButton sender);

		[Action ("DownloadClicked:")]
		partial void DownloadClicked (AppKit.NSButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (Add != null) {
				Add.Dispose ();
				Add = null;
			}

			if (Clear != null) {
				Clear.Dispose ();
				Clear = null;
			}

			if (Download != null) {
				Download.Dispose ();
				Download = null;
			}

			if (Progress != null) {
				Progress.Dispose ();
				Progress = null;
			}

			if (Status != null) {
				Status.Dispose ();
				Status = null;
			}

			if (TextField != null) {
				TextField.Dispose ();
				TextField = null;
			}

		}
	}
}
