using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SiberianWellness.Common
{
    public static class AssetDatabaseWrapper
    {
#if UNITY_EDITOR
        public static void CreateFolder(string path)
        {
            AssetDatabase.DeleteAsset(path);

            var p = path.Split('/');

            string curPath = "Assets";

            for (int i = 1; i < p.Length; i++)
            {
                var nextPath = curPath + "/" + p[i];

                if (!Directory.Exists(nextPath))
                {
                    AssetDatabase.CreateFolder(curPath, p[i]);
                }

                curPath = nextPath;
            }
        }
        
        public static List<T> Load<T>(Object pathProvider, bool includeSubFolder = false)
            where T:Object
        {
            var fileName = AssetDatabase.GetAssetPath(pathProvider);
            var path     = Path.GetDirectoryName(fileName);

            return Load<T>(path, includeSubFolder);
        }
		
        public static List<T> Load<T>(string assetFolderPath, bool includeSubFolder = false)
            where T:Object
        {  
            List<T> result = new List<T>();

            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var allGo = EditorExtension.LoadAllAssetsOfType<GameObject>(assetFolderPath, includeSubFolder);
                foreach (var obj in allGo)
                {
                    if (obj is GameObject go)
                    {
                        var c = go.GetComponent<T>();
                        if (c)
                            result.Add(c);
                    }
                }
            }
            else
            {
                //GameObject, ScriptableObject, Sprite ...
                var allAssets = EditorExtension.LoadAllAssetsOfType<T>(assetFolderPath, includeSubFolder);
                foreach (var obj in allAssets)
                {
                    if (obj is T inst)
                    {
                        result.Add(inst);
                    }
                }
            }

            return result;
        }
#endif
    }
}