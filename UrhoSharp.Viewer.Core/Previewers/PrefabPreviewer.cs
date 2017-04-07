using System;
using Urho;
using UrhoSharp.Viewer.Core.Components;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core.Previewers
{
	public class PrefabPreviewer : AbstractPreviewer
	{
		public PrefabPreviewer(UrhoScene urhoApp) : base(urhoApp) { }

		protected override void OnShow(Node node, Asset asset)
		{
			node.CreateComponent<WirePlane>();
			var file = ResourceCache.GetFile(asset.RelativePathToAsset, true);
			if (file == null)
				throw new InvalidOperationException($"{asset} not found");

			var prefabNode = node.Scene.InstantiateXml(file, Vector3.Zero, Quaternion.Identity, CreateMode.Replicated);

			prefabNode.AddRef();//temp workaround, fixed in urhosharp via Node.ChangeParent
			prefabNode.Remove();
			node.AddChild(prefabNode);
			prefabNode.ReleaseRef();

			prefabNode.SetScaleBasedOnBoundingBox(10);
		}
	}
}
