﻿using System;

using AppKit;
using Foundation;
using Urho;
using UrhoSharp.Viewer.Core;
using System.Threading.Tasks;
using CoreGraphics;

namespace UrhoSharp.Viewer.Cocoa
{
	public partial class ViewController : NSViewController
	{
		PreviewerApplication previewer;
		UrhoSurface urhoSurface;

		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			urhoSurface = new UrhoSurface();
			urhoSurface.Frame = UrhoSurfacePlaceholder.Frame;
			urhoSurface.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
			UrhoSurfacePlaceholder.AddSubview(urhoSurface);

			previewer = new PreviewerApplication(new AssetsResolver { AssetsImporterFormats = true, AssetsImporterRareFormats = true, Images = true });
			previewer.SurfaceRecreationRequested += OnSurfaceRequested;
		}

		partial void OpenFileButton(Foundation.NSObject sender)
		{
			var dlg = NSOpenPanel.OpenPanel;
			dlg.CanChooseFiles = true;
			dlg.CanChooseDirectories = false;
			dlg.AllowsMultipleSelection = false;
			dlg.Title = "Choose any format Urho3D and Assetimp support";

			if (dlg.RunModal() == 1)
			{
				Run(dlg.Filename);
			}
		}

		async void Run(string file)
		{
			await Task.Delay(1000);
			previewer.Show(file);
		}

		IntPtr OnSurfaceRequested()
		{
			return urhoSurface.Handle;
		}

		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}
			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}



	public class UrhoSurface : NSView
	{
		public override async void ViewDidMoveToWindow()
		{
			base.ViewDidMoveToWindow();
			PostsFrameChangedNotifications = true;
			PostsBoundsChangedNotifications = true;
		}

		public override async void SetFrameSize(CoreGraphics.CGSize newSize)
		{
			base.SetFrameSize(newSize);
			if (Application.HasCurrent)
				NSOpenGLContext.CurrentContext?.Update();
		}
	}
}