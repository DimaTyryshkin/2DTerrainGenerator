using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiberianWellness.Common
{
	public static class SceneExtension
	{
		public static T GetComponentInChildren<T>(this Scene scene, bool includeInactive)
			where T : Component
		{
			foreach (var go in scene.GetRootGameObjects())
			{
				if (go.activeSelf || includeInactive)
				{
					T c = go.GetComponentInChildren<T>(includeInactive);
					if (c)
						return c;
				}
			}

			return null;
		}

		public static T[] GetComponentsInChildren<T>(this Scene scene, bool includeInactive)
			where T:Component
		{
			List<T> list = new List<T>(128);

			foreach (var go in scene.GetRootGameObjects())
			{
				if (go.activeSelf || includeInactive)
				{
					T[] c = go.GetComponentsInChildren<T>(includeInactive);
					list.AddRange(c);
				}
			}

			return list.ToArray();
		}
	}
}