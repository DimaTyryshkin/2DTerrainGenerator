using System.Collections.Generic;
using SiberianWellness;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace SiberianWellness
{
	public class DynamicObjectPool : MonoBehaviour
	{
		Dictionary<GameObject, ObjectPool> prefabToPool = new Dictionary<GameObject, ObjectPool>();

		public T GetInstance<T>(T prefab) where T : Component
		{
			GameObject go = GetInstance(prefab.gameObject);
			return go.GetComponent<T>();
		}

		public GameObject GetInstance(GameObject prefab)
		{
			Assert.IsNotNull(prefab);
#if DEV && UNITY_EDITOR
			Assert.IsFalse(string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(prefab)), $"Объект '{prefab.name}' должен быть ассетом");
#endif

			if (prefabToPool.TryGetValue(prefab, out ObjectPool pool))
			{
				return pool.GetObject();
			}
			else
			{
				var poolGo  = new GameObject(prefab.name + "_ObjectPool");
				poolGo.transform.SetParent(transform);
				var newPool = poolGo.AddComponent<ObjectPool>();
				
				newPool.SetPrefab(prefab);
				prefabToPool.Add(prefab, newPool);
		
				return newPool.GetObject();
			}
		} 
	}
}