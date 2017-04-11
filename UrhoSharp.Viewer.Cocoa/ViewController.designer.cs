// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UrhoSharp.Viewer.Cocoa
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField PathLabel { get; set; }

		[Outlet]
		AppKit.NSView UrhoSurfacePlaceholder { get; set; }

		[Action ("OpenFileButton:")]
		partial void OpenFileButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (PathLabel != null) {
				PathLabel.Dispose ();
				PathLabel = null;
			}

			if (UrhoSurfacePlaceholder != null) {
				UrhoSurfacePlaceholder.Dispose ();
				UrhoSurfacePlaceholder = null;
			}
		}
	}
}
