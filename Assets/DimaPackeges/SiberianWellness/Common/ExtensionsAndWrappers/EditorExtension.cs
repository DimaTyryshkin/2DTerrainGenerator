using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SiberianWellness.Common
{
	public static class EditorExtension
	{
#if UNITY_EDITOR
		/// <summary>
		/// Возвращает все ресурсы указанного типа.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="optionalPath"> Путь начиная с Assets "Assets/Prefab"</param> 
		public static T[] LoadAllAssetsOfType<T>(string optionalPath = "", bool includeSubFolder = false) where T : Object
		{
			//AssetDatabase.LoadAssetAtPath<> разве это не тоже самое, хотя этот метод надстройка над данным. Но что она делает??



			string[] GUIDs;
			if (optionalPath != "")
			{
				if (optionalPath.EndsWith("/"))
				{
					optionalPath = optionalPath.TrimEnd('/');
				}

				GUIDs = AssetDatabase.FindAssets("t:" + typeof(T).Name, new string[] {optionalPath});
			}
			else
			{
				includeSubFolder = true;
				GUIDs            = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			}

			List<T> objectList = new List<T>(GUIDs.Length); // new T[GUIDs.Length];

			foreach (var guid in GUIDs) 
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);

				if (includeSubFolder == false)
				{
					var dir = Path.GetDirectoryName(assetPath).Replace(@"\",@"/");
					
					if (dir != optionalPath) 
						continue;
				}

				T asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;

				if (asset != null) objectList.Add(asset);
			}

			return objectList.ToArray();
		}
#endif
	}
}