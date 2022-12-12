using UnityEngine.UI;

namespace SiberianWellness.Common
{
	public static class GraphicExtension
	{
		public static void SetAlpha(this Graphic g, float alpha)
		{
			var color = g.color;
			color.a = alpha;
			g.color = color;
		}
	}
}