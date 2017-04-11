using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit.Highlighting;
using Urho;
using UrhoSharp.Viewer.Core;
using UrhoSharp.Viewer.Core.Previewers;
using UrhoSharp.Viewer.Wpf.Properties;
using Color = System.Drawing.Color;
using Panel = System.Windows.Forms.Panel;

namespace UrhoSharp.Viewer.Wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged, IEditor
	{
		PreviewerApplication previewer;
		Panel urhoSurface;
		string workingDirectory;
		Asset currentAsset;
		List<SolutionItem> solutionItems;
		UrhoScene scene;

		public List<SolutionItem> SolutionItems
		{
			get { return solutionItems; }
			set { SetField(ref solutionItems, value); }
		}

		public string WorkingDirectory
		{
			get { return workingDirectory; }
			set { SetField(ref workingDirectory, value); }
		}

		public MainWindow()
		{
			InitializeComponent();
			previewer = new PreviewerApplication(new AssetsResolver { AssetsImporterFormats = true, AssetsImporterRareFormats = true, Images = true });
			previewer.SurfaceRecreationRequested += RecreateSurface;
		}

		void BuildTree(string folder)
		{
			SolutionItems = folder == null ? null : new SolutionItem(true, folder).Children;
			DataContext = this;
		}

		IntPtr RecreateSurface()
		{
			urhoSurface?.Dispose();
			urhoSurface = new Panel { Dock = DockStyle.Fill, BackColor = Color.Gray };
			WindowsFormsHost.Child = urhoSurface;
			WindowsFormsHost.Focus();
			urhoSurface.Focus();
			return urhoSurface.Handle;
		}

		async void SolutionExplorerTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var item = e.NewValue as SolutionItem;
			if (item != null && !item.IsFolder)
			{
				try
				{
					currentAsset = null;
					WorkingDirectory = string.Empty;
					LoadingStatus.Text = "Loading...";
					LoadingPanel.Visibility = Visibility.Visible;
					scene = await previewer.Show(item.Path, this, (int)WindowsFormsHost.Width, (int)WindowsFormsHost.Height);
					currentAsset = scene?.CurrentAsset;
					if (currentAsset != null)
					{
						var xmlBasedAssets = new [] {
							AssetsType.AnimationSet2D, AssetsType.Material,
							AssetsType.Particle2D, AssetsType.Particle3D, AssetsType.Prefab,
							AssetsType.RenderPath, AssetsType.Scene, AssetsType.UI };

						if (xmlBasedAssets.Contains(currentAsset.Type))
						{
							var raw = File.ReadAllText(currentAsset.FullPathToAsset);
							RawEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
							RawEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
							RawEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
							RawEditor.Text = raw;
							RawEditor.IsReadOnly = false;
						}
						else
						{
							RawEditor.IsReadOnly = true;
							RawEditor.Text = "";
						}
						WorkingDirectory = currentAsset.RootDirectory;
					}
					LoadingPanel.Visibility = Visibility.Collapsed;
				}
				catch (OperationCanceledException)
				{
				}
				catch (InvalidOperationException exc)
				{
					LoadingStatus.Text = exc.Message;
				}
				catch (NotSupportedException)
				{
					LoadingStatus.Text = "Not supported.";
				}
			}
		}

		void OnBrowseClick(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog { Description = "Solution folder" };
			dialog.ShowDialog();
			RootFolderPath.Text = dialog.SelectedPath;
		}

		void OnRootFolderChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(RootFolderPath.Text) || !DirectoryExists(RootFolderPath.Text))
				BuildTree(null);
			else
			{
				BuildTree(RootFolderPath.Text);
				Settings.Default.RootDir = RootFolderPath.Text;
				Settings.Default.Save();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			field = value;
			NotifyPropertyChanged(propertyName);
			return true;
		}

		void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		static bool DirectoryExists(string path)
		{
			try
			{
				return Directory.Exists(path);
			}
			catch
			{
				return false;
			}
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(WorkingDirectory) && Directory.Exists(WorkingDirectory))
			{
				Process.Start(WorkingDirectory);
			}
		}

		private void RawEditor_TextChanged(object sender, EventArgs e)
		{
			File.WriteAllText(currentAsset.FullPathToAsset, RawEditor.Text);
			var previewer = scene?.CurrentPreviewer as PrefabPreviewer;
			previewer?.Refresh();
		}

		public void HighlightXmlForNode(Node node)
		{
			try
			{
				var text = $"name=\"Name\" value=\"{node.Name}\"";
				var index = RawEditor.Text.IndexOf(text);
				RawEditor.Select(index - 11, text.Length + 14);

				var lines = RawEditor.Text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].Contains(text))
					{
						RawEditor.ScrollToLine(i);
						break;
					}
				}
			}
			catch { }
		}
	}
}
