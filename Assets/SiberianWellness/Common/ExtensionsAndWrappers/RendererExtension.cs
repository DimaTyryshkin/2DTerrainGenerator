using UnityEngine;

namespace SiberianWellness.Common
{
	public static class RendererExtension
	{ 
		public static Bounds GetTotalRendererBounds(this Transform t)
		{
			bool   rendererFound = false;
			Bounds totalBounds   = new Bounds();

			var allRenderersOnChild = t.GetComponentsInChildren<Renderer>();

			foreach (var r in allRenderersOnChild)
			{
				if (!rendererFound)
				{
					rendererFound = true;
					totalBounds   = r.bounds;
				}
				else
				{
					totalBounds.Encapsulate(r.bounds);
				}
			}

			if (rendererFound)
				return totalBounds;
			else
				return new Bounds(t.position, Vector3.zero);
		}
	}
}