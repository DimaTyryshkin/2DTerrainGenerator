using System;
using System.Reflection;
using SiberianWellness.Common;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SiberianWellness.Validation
{

    /// <summary>
    /// Not null violation represents data for objects that do not have their required (NotNull) fields  
    /// assigned.
    /// </summary>
    public class ValidationProblem
    {
        public enum Type
        {
            Error   = 0,
            Warning = 1,
        }

        public Object root;
        public FieldInfo fieldInfo;
        public string header;
        public string msg;
        public string sceneName;
        public Type type;

        string logMsg;
        
        
        
        string FullName
        {
            get
            { 
                if (root is MonoBehaviour mb)
                    return mb.gameObject.FullName();

                if (root is GameObject go)
                    return go.FullName();

                return root.name;
            }
        }

        public void CacheToString()
        {
            logMsg = ConvertToString();
        }
        
        string ConvertToString()
        {
            string log = $"[<b>{header}</b>: <b>FullName=</b>{FullName}";

            if (fieldInfo != null)
                log += $" <b>Field=</b>{fieldInfo.Name}]";
            else
                log += "]";
 

            if (msg != null)
            {
                log += Environment.NewLine + msg ;
            }

#if UNITY_EDITOR
            var pathToRoot = AssetDatabase.GetAssetPath(root);
            if (!string.IsNullOrWhiteSpace(pathToRoot))
                log += Environment.NewLine + $"<b>Path=</b>{pathToRoot}";
            
            if (!string.IsNullOrWhiteSpace(sceneName))
                log += Environment.NewLine + $"<b>Scene=</b>{sceneName}";
#endif
            log += Environment.NewLine; 
            return log;
        }

        public override string ToString()
        {
            return logMsg;
        }
    }
}
