using UnityEngine;

namespace SiberianWellness.Common
{
	public static class SpriteRendererExtension
	{
		public static void SetAlpha(this SpriteRenderer r, float a)
		{
			Color c = r.color;
			c.a     = a;
			r.color = c;
		}
	}
}