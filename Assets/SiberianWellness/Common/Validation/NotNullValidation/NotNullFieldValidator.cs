using System;
using System.Reflection;
using SiberianWellness.Validation;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SiberianWellness.NotNullValidation.Internal
{
    public class NotNullFieldValidator : AbstractValidator
    {
        public NotNullFieldValidator()
        {
            
        }
        
        static bool IsSimilarToNull(object obj)
        {
            if (obj == null || obj.Equals(null))
            {
                return true;
            }
            else
            {
                if (obj is string s)
                    return string.IsNullOrWhiteSpace(s);

                return false;
            }
        }

        public override void FindProblemsInField(object value, Type valueType, FieldInfo ownerFieldInfo, Object rootObject)
        {
#if UNITY_EDITOR
            if (IsSimilarToNull(value))
            {
                if(NotNullFieldValidator_Config.GetInst().ExcludeFromLog(rootObject))
                    return;
                
                var attribute = ownerFieldInfo.GetCustomAttribute<IsntNullAttribute>();

                if (attribute != null)
                {

                    if (attribute.AllowInAsset)
                    {
                        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(rootObject)))
                            return;
                    }

                    ValidationProblem.Type type = attribute.warring ?
                        ValidationProblem.Type.Warning :
                        ValidationProblem.Type.Error;

                    RecursiveValidator.ValidationContext.AddProblem(nameof(IsntNullAttribute), type);
                }
            }
#endif
        }

        public override void FindProblemsInObject(object value, Type valueType, Object rootObject)
        {
#if UNITY_EDITOR
            if(NotNullFieldValidator_Config.GetInst().ExcludeFromLog(rootObject))
                return;
            
            if (value is ScriptableObject so)
            {
                MonoScript s = MonoScript.FromScriptableObject(so);
                if (!s)
                    RecursiveValidator.ValidationContext.AddProblem("Missing script", ValidationProblem.Type.Error, overrideRoot: so);
            }

            if (value is MonoBehaviour mb)
            {
                MonoScript s = MonoScript.FromMonoBehaviour(mb);
                if (!s)
                    RecursiveValidator.ValidationContext.AddProblem("Missing script", ValidationProblem.Type.Error, overrideRoot: mb);
            }
#endif
        }
    }
}