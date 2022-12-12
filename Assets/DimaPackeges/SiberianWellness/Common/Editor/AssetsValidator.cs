using System;
using System.Collections;
using SiberianWellness.Common;
using SiberianWellness.Validation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SiberianWellness.Validation
{
	/// <summary>
	/// Поиск проблем во всех файлах проекта. Только в редакторе
	/// </summary>
	public static class AssetsValidator
	{
		static readonly int sceneWeight = 300;//Проверку одной сцены счиатем как сразу много объектов
		
		public static void Add_AllScenes(RecursiveValidator.SearchScope scope)
		{
			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes; 
			scope.AddTotalObjectsCount(scenes.Length * sceneWeight);
		}

		public static IEnumerator PrintNullInAllScenes(RecursiveValidator.SearchScope scope)
		{  
			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes; 
 
			foreach (var scene in scenes)
			{ 
				EditorSceneManager.OpenScene(scene.path);
				//На всякий случай ждем немного 
				yield return null;
				yield return null;
				yield return null;
				yield return null;
				PrintNullInScene(scope); 
			}
		}


		public static void Add_AllScriptableObjects(RecursiveValidator.SearchScope scope)
		{
			string[] assetsGuid = AssetDatabase.FindAssets("t:ScriptableObject");
			scope.AddTotalObjectsCount(assetsGuid.Length);
		}

		public static void PrintNullInScriptableObject(RecursiveValidator.SearchScope scope) 
		{
			RecursiveValidator.ValidationContext.currentScene = "Assets";
			string[] assetsGuid = AssetDatabase.FindAssets("t:ScriptableObject");
 
			foreach (string guid in assetsGuid)
			{
				scope.AddProgress(1);
				string pathToObj = AssetDatabase.GUIDToAssetPath(guid);

				if (pathToObj.Contains("CandyMatch3Kit")) //TODO remove
				{
					continue;
				}

				ScriptableObject obj = (ScriptableObject) AssetDatabase.LoadAssetAtPath(pathToObj, typeof(ScriptableObject));

				PrintNullOnScriptableObject(obj, pathToObj);
			}
		}

		public static void Add_Prefabs(RecursiveValidator.SearchScope scope)
		{
			string[] assetsGuid = AssetDatabase.FindAssets("t:GameObject");
			scope.AddTotalObjectsCount(assetsGuid.Length);
			
		}
		public static void PrintNullInPrefabs(RecursiveValidator.SearchScope scope)
		{
			RecursiveValidator.ValidationContext.currentScene = "Assets";
			string[] assetsGuid = AssetDatabase.FindAssets("t:GameObject");
 
			foreach (string guid in assetsGuid)
			{
				scope.AddProgress(1);
				string pathToGameObject = AssetDatabase.GUIDToAssetPath(guid);

				if (pathToGameObject.Contains("CandyMatch3Kit")) //TODO remove
				{
					continue;
				}

				GameObject gameObject = (GameObject) AssetDatabase.LoadAssetAtPath(pathToGameObject, typeof(GameObject));

				PrintNullOnGameObject(gameObject, pathToGameObject, gameObject);
			}
		}

		public static void PrintNullInScene(RecursiveValidator.SearchScope scope)
		{
			var scene = SceneManager.GetSceneAt(0);
			RecursiveValidator.ValidationContext.currentScene = scene.name;
			scope.AddProgress(sceneWeight);
			GameObject[] rootSceneGameObjects = scene.GetRootGameObjects();

			foreach (GameObject rootGameObjectInScene in rootSceneGameObjects)
			{
				PrintNullOnGameObject(rootGameObjectInScene, "Scene " + SceneManager.GetActiveScene().name);
			}
		}

		static void PrintNullOnGameObject(GameObject gameObject, string pathToAsset, GameObject ownerForLog = null)
		{ 
			RecursiveValidator.ValidateGo(gameObject);
		  
			foreach (Transform child in gameObject.transform)
			{
				PrintNullOnGameObject(child.gameObject, pathToAsset, ownerForLog);
			}
		}

		static void PrintNullOnScriptableObject(ScriptableObject scriptableObject, string pathToAsset)
		{ 
			RecursiveValidator.ValidateObject(scriptableObject); 
		}
	}
}