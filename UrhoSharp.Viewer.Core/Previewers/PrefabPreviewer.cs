using System;
using Urho;
using Urho.Gui;
using UrhoSharp.Viewer.Core.Components;
using UrhoSharp.Viewer.Core.Utils;

namespace UrhoSharp.Viewer.Core.Previewers
{
	public class PrefabPreviewer : AbstractPreviewer
	{
		Node prefabNode;
		float scale;
		Material selectedMaterial;
		StaticModel selectedModel;

		public PrefabPreviewer(UrhoScene urhoApp) : base(urhoApp)
		{
		}

		protected override void OnShow(Node node, Asset asset)
		{
			App.Input.MouseButtonUp += Input_MouseButtonUp;
			App.Input.KeyUp += Input_KeyUp;
			node.CreateComponent<WirePlane>();
			Refresh();
		}

		void Input_KeyUp(KeyUpEventArgs e)
		{
			switch (e.Key)
			{
				case Key.X:
					RotateAxis(e.Qualifiers > 0 ? -90 : 90, 0, 0);
					break;
				case Key.Y:
					RotateAxis(0, e.Qualifiers > 0 ? -90 : 90, 0);
					break;
				case Key.Z:
					RotateAxis(0, 0, e.Qualifiers > 0 ? -90 : 90);
					break;
			}
		}

		void RotateAxis(float x, float y, float z)
		{
			prefabNode.Rotate(new Quaternion(x, y, z), TransformSpace.Local);
		}

		protected override void OnStop()
		{
			App.Input.MouseButtonUp -= Input_MouseButtonUp;
			App.Input.KeyUp -= Input_KeyUp;
			base.OnStop();
		}

		void Input_MouseButtonUp(MouseButtonUpEventArgs e)
		{
			var cursorPos = App.UI.CursorPosition;
			var cameraRay = App.Camera.GetScreenRay((float)cursorPos.X / App.Graphics.Width, (float)cursorPos.Y / App.Graphics.Height);
			var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 10000, DrawableFlags.Geometry);
			if (result != null)
			{
				var geometry = result.Value.Drawable as StaticModel;
				if (geometry != null)
				{
					if (selectedModel != null && !selectedModel.IsDeleted)
					{
						selectedModel?.SetMaterial(0, selectedMaterial);
						selectedMaterial.ReleaseRef();
					}
					selectedMaterial = geometry.GetMaterial(0);
					selectedMaterial.AddRef();
					selectedModel = geometry;

					var mat = Material.FromColor(Color.Blue);
					mat.FillMode = FillMode.Wireframe;
					geometry.SetMaterial(mat);
					var specColorAnimation = new ValueAnimation();

					Color color = new Color(0.8f, 0.8f, 0.1f);
					Color fade = new Color(0.5f, 0.5f, 0.5f);

					specColorAnimation.SetKeyFrame(0.0f, fade);
					specColorAnimation.SetKeyFrame(0.5f, color);
					specColorAnimation.SetKeyFrame(1.0f, fade);
					mat.SetShaderParameterAnimation("MatDiffColor", specColorAnimation, WrapMode.Loop, 1.0f);

					Editor?.HighlightXmlForNode(result.Value.Node);
				}
			}
		}

		public void Refresh()
		{
			var file = ResourceCache.GetFile(Asset.RelativePathToAsset, true);
			if (file == null)
				throw new InvalidOperationException($"{Asset} not found");

			prefabNode?.Remove();
			prefabNode = Scene.InstantiateXml(file, Vector3.Zero, Quaternion.Identity, CreateMode.Replicated);
			prefabNode?.ChangeParent(Node);

			if (scale == 0 && prefabNode != null)
			{
				prefabNode?.SetScaleBasedOnBoundingBox(10);
				scale = prefabNode.Scale.X;
			}
			else
			{
				prefabNode?.SetScale(scale);
			}
		}

	}
}
