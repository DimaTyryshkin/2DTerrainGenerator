using System;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.LocationData
{
	public class FastSelectionWindow : EditorWindow
	{
		FastSelectionTool toolCache;


		[MenuItem("Tools/Быстрое Выделение")]
		public static void Init()
		{
			FastSelectionWindow window = GetWindow<FastSelectionWindow>();
			window.titleContent.text = "Быстрое выделение";
		}

		void OnGUI()
		{
			var tool = GetTool();

			foreach (var obj in tool.objects)
			{
				if (obj)
				{
					if (obj is SceneAsset scene)
					{
						Color c = GUI.color;
						GUI.color = Color.gray;
						if (GUILayout.Button(obj.name, GUILayout.ExpandWidth(true)))
						{
							var pathToScene = AssetDatabase.GetAssetPath(scene);
							if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
								EditorSceneManager.OpenScene(pathToScene);
						}

						GUI.color = c;
					}
					else
					{
						if (GUILayout.Button(obj.name, GUILayout.ExpandWidth(true)))
							//Selection.activeObject = obj;
							AssetDatabase.OpenAsset(obj);
					}
				}
			}

			GUILayout.Space(10);
			if (GUILayout.Button("Tool", GUILayout.ExpandWidth(true)))
				Selection.activeObject = tool;
		}

		FastSelectionTool GetTool()
		{
			if (toolCache)
				return toolCache;
			
			toolCache = LoadFromResource();

			if (!toolCache)
			{
				FastSelectionTool asset = CreateInstance<FastSelectionTool>();
				DirectoryInfo dir = new DirectoryInfo("Assets/Editor/Resources/DeveloperPersonalSettings");
					if(!dir.Exists)
						dir.Create();
					
				AssetDatabase.CreateAsset(asset, "Assets/Editor/Resources/DeveloperPersonalSettings/FastSelectionTool.asset");
				AssetDatabase.SaveAssets();

				toolCache = LoadFromResource();
			}
			
			return toolCache;
		}

		FastSelectionTool LoadFromResource()
		{
			return Resources.Load<FastSelectionTool>("DeveloperPersonalSettings/FastSelectionTool");
		}
	}
}