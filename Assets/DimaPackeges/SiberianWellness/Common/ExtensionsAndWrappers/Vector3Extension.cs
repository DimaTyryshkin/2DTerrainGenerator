using UnityEngine;

namespace SiberianWellness.Common
{
	public static class Vector3Extension
	{
		public static Vector3 GetMinComponents(Vector3 a, Vector3 b)
		{
			float xMin = Mathf.Min(a.x, b.x);
			float yMin = Mathf.Min(a.y, b.y);
			float zMin = Mathf.Min(a.z, b.z);

			return new Vector3(xMin, yMin, zMin);
		}
		
		public static Vector3 GetMaxComponents(Vector3 a, Vector3 b)
		{
			float xMin = Mathf.Max(a.x, b.x);
			float yMin = Mathf.Max(a.y, b.y);
			float zMin = Mathf.Max(a.z, b.z);

			return new Vector3(xMin, yMin, zMin);
		}
	}
}