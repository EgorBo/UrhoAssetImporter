using Urho;

namespace UrhoSharp.Viewer.Core.Components
{
	public class WirePlane : Component
	{
		public override void OnAttachedToNode(Node node)
		{
			const int size = 25;
			const float scale = 0.5f;
			Color color = new Color(0.8f, 0.8f, 0.8f);

			CustomGeometry geom = node.CreateComponent<CustomGeometry>();
			geom.BeginGeometry(0, PrimitiveType.LineList);
			var material = new Material();
			material.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol, 1, 1);
			geom.SetMaterial(material);

			var halfSize = size / 2;
			for (int i = -halfSize; i <= halfSize; i++)
			{
				//x
				geom.DefineVertex(new Vector3(i, 0, -halfSize) * scale);
				geom.DefineColor(color);
				geom.DefineVertex(new Vector3(i, 0, halfSize) * scale);
				geom.DefineColor(color);

				//z
				geom.DefineVertex(new Vector3(-halfSize, 0, i) * scale);
				geom.DefineColor(color);
				geom.DefineVertex(new Vector3(halfSize, 0, i) * scale);
				geom.DefineColor(color);
			}

			geom.Commit();
			base.OnAttachedToNode(node);
		}
	}
}
