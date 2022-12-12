using UnityEditor;
using UnityEngine;

namespace SiberianWellness.Common
{
	public static class DependencyFinder
	{
		public static void FindAllDependencies()
		{
			FindDependenciesOnScene();
			FindDependenciesInScriptableObject();
			FindDependenciesInPrefabs();
		}
		
		public static void FindDependenciesInScene()
		{
			FindDependenciesOnScene(); 
		}

		static void FindDependenciesOnScene()
		{
			//--на сцене
			var all = Object.FindObjectsOfType<MonoBehaviour>();

			foreach (var monoBehaviour in all)
			{
				if (monoBehaviour is IDependencyFinder dependencyFinder)
				{
					Debug.Log($"{monoBehaviour.gameObject.FullName()} <color=blue>{monoBehaviour.GetType().Name}</color>", monoBehaviour.gameObject);
					dependencyFinder.FindDependency();
				}
			}
		}

		static void FindDependenciesInScriptableObject()
		{ 
			//--в ассетах ScriptableObject 
			string[] assetsGuid = AssetDatabase.FindAssets("t:ScriptableObject");
			foreach (string guid in assetsGuid)
			{
				string           pathToObj = AssetDatabase.GUIDToAssetPath(guid);
				ScriptableObject obj       = (ScriptableObject) AssetDatabase.LoadAssetAtPath(pathToObj, typeof(ScriptableObject));
				if (obj is IDependencyFinder dependencyFinder)
				{
					Debug.Log($"{AssetDatabase.GUIDToAssetPath(guid)} <color=blue>{obj.GetType().Name}</color>", obj);
					dependencyFinder.FindDependency();
				}
			}
		}

		static void FindDependenciesInPrefabs()
		{ 
			//--в ассетах GameObject 
			string[] assetsGuid = AssetDatabase.FindAssets("t:GameObject");
			foreach (string guid in assetsGuid)
			{
				string     pathToObj = AssetDatabase.GUIDToAssetPath(guid);
				GameObject go        = (GameObject) AssetDatabase.LoadAssetAtPath(pathToObj, typeof(GameObject));

				MonoBehaviour[] monobehaviours = go.GetComponents<MonoBehaviour>();
				foreach (var mb in monobehaviours)
				{
					if (mb is IDependencyFinder dependencyFinder)
					{
						Debug.Log($"{AssetDatabase.GUIDToAssetPath(guid)} <color=blue>{mb.GetType().Name}</color>", go);
						dependencyFinder.FindDependency();
					}
				}
			}
		}
	}
}