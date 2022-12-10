using UnityEngine;

namespace SiberianWellness.Common
{
	public static class QuaternionExtension
	{
		public static Quaternion FromToRotation(this Quaternion from, Quaternion to)
		{
			return Quaternion.Inverse(from) * to; // Любая перестановка все ломает, причем коварно невсегда.
		}
		
		public static Quaternion FromToRotation(this Quaternion from, Quaternion to, float t)
		{
			Quaternion q = FromToRotation(from, to);
			q = Quaternion.Lerp(Quaternion.identity, q, t);
			return q;
		}
	}
}