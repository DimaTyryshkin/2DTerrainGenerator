using UnityEngine;

namespace SiberianWellness.Common
{
	public static class GUIExtension
	{
		public static void DrawSprite(Rect rect, Sprite sprite)
		{	
			if (Event.current.type == EventType.Repaint)
			{
				Rect c   = sprite.textureRect; 
				var  tex = sprite.texture;
				c.xMin /= tex.width;
				c.xMax /= tex.width;
				c.yMin /= tex.height;
				c.yMax /= tex.height;
				GUI.DrawTextureWithTexCoords(rect, tex, c);
				//ScaleMode.StretchToFill
			}
		} 
	}
	
	public static class GUILayoutExtension
	{
		public static void DrawSprite(Sprite sprite, float maxSize)
		{
			Rect  r = sprite.rect;
			float w = r.width;
			float h = r.height;

			if (w > maxSize)
			{
				float k = w / maxSize;
				w /= k;
				h /= k;
			}
			
			if (h > maxSize)
			{
				float k = h / maxSize;
				w /= k;
				h /= k;
			}


			var rect = GUILayoutUtility.GetRect(w, h,GUILayout.Width(w), GUILayout.Height(h));
			GUIExtension.DrawSprite(rect, sprite);
		} 
	}
}