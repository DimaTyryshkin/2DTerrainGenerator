using UnityEngine;

namespace SiberianWellness.Common
{
	public static class BoundsExtension
	{ 
		/// <summary>
		/// Create Bounds that encompasses two bounds.
		/// </summary>
		public static Bounds Encompass(Bounds a, Bounds b)
		{
			Vector3 aMin = a.min;
			Vector3 aMax = a.max;

			Vector3 bMin = b.min;
			Vector3 bMax = b.max;

			Vector3 min = Vector3Extension.GetMinComponents(aMin, bMin);
			Vector3 max = Vector3Extension.GetMaxComponents(aMax, bMax);

			return FromMinMax(min, max);
		}
		
		public static Bounds FromMinMax(Vector3 min, Vector3 max)
		{
			Vector3 size = max - min;
			return new Bounds(min + size * 0.5f, size);
		}
		
		/// <summary>
		/// Creates and returns an enlarged copy of the specified Bounds. The copy is enlarged by the specified amounts.
		/// </summary> 
		public static Bounds Inflate(Bounds a, float value)
		{
			Vector3 extents = a.extents;

			extents.x += Mathf.Sign(extents.x) * value;
			extents.y += Mathf.Sign(extents.y) * value;
			extents.z += Mathf.Sign(extents.z) * value;
		
			return new Bounds(a.center, extents * 2);
		}
		
		public static Bounds ToFlatXZ(Bounds a)
		{
			Vector3 extents = a.extents;
			extents.y = 0; 
			return new Bounds(a.center, extents * 2);
		}
	}
}