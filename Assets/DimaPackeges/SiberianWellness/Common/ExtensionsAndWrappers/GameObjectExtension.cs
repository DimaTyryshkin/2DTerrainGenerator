using UnityEngine;

namespace SiberianWellness.Common
{
	public static class GameObjectExtension
	{
		public static void SetEnable(this GameObject go, bool enable)
		{
			go.SetActive(enable);
		}
 
		public static string FullName(this GameObject go)
		{
			string    s = "";
			Transform t;
			t = go.transform;

			do
			{
				s = "/" + t.name + s;
				t = t.parent;
			} while (t);

			return s;
		}
	}
}