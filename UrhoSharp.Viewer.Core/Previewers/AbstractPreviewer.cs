using Urho;
using Urho.Resources;

namespace UrhoSharp.Viewer.Core.Previewers
{
	public abstract class AbstractPreviewer
	{
		protected UrhoScene App { get; }

		protected ResourceCache ResourceCache => App.ResourceCache;

		protected AbstractPreviewer(UrhoScene urhoApp)
		{
			App = urhoApp;
		}

		protected abstract void OnShow(Node node, Asset asset);

		protected virtual void OnStop() {}

		public virtual bool RotateRootNode => true;

		public void Show(Node node, Asset asset)
		{
			OnShow(node, asset);
			App.Update += OnUpdate;
		}

		public void Stop()
		{
			App.Update -= OnUpdate;
			OnStop();
		}

		// TODO: will be replaced with Material.FromColor with the next UrhoSharp nuget
		protected Material CreateDefaultMaterial()
		{
			var material = ResourceCache.GetMaterial("Materials/DefaulPreviewerMaterial.xml").Clone(string.Empty);
			//material.SetShaderParameter("MatSpecColor", Color.Green);
			return material;
		}

		protected virtual void OnUpdate(UpdateEventArgs e) { }
	}
}