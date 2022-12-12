using UnityEngine;

namespace SiberianWellness.Common
{
	public enum Plaine
	{
		YZ = 0,
		XZ = 1,
		XY = 2
	}

	public static class CameraExtension
	{
		public static Vector3 ScreenPointToWorldPointOnPlane(this Camera camera, Vector3 screenPoint, Plaine onPlane)
		{
			var r = camera.ScreenPointToRay(screenPoint);
            
			Vector3 dir = r.direction.normalized;
			int     i   = (int) onPlane;
            
			float t = -r.origin[i] / dir[i];
			return r.GetPoint(t);
		}
	}

	
}