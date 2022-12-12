using UnityEngine;

namespace SiberianWellness.Common
{
	public static class RectExtension
	{ 
		public static Rect ScaleSize(this Rect rect, float scale)
		{
			return rect.ScaleSize(scale, rect.center);
		}
 
		public static Rect ScaleSize(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;

			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;

			result.x += pivotPoint.x;
			result.y += pivotPoint.y;

			return result;
		}
	}
}