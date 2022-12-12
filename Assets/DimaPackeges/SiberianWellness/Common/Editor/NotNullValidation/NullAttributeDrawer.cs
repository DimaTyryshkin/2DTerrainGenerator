using System.Reflection;
using UnityEditor;
using UnityEngine; 

namespace SiberianWellness.NotNullValidation.Internal
{ 
    /// <summary>
    /// Drawerer for fields with NotNullAttribute assigned.
    /// </summary>
    [CustomPropertyDrawer(typeof(IsntNullAttribute))]
    public class NotNullAttributeDrawer : PropertyDrawer
    {   
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)  
        {  
            EditorGUI.BeginProperty(position, label, property);         
            BuildObjectField(position, property, label); 
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        private bool IsNotWiredUp(SerializedProperty property)
        { 
            if (this.IsPropertyNotNullInSceneAndPrefab(property))
            {
                return false;
            }
            else
            {
                if (property.propertyType == SerializedPropertyType.String)
                {
                    return string.IsNullOrWhiteSpace(property.stringValue);
                }
                else
                {
                    return property.objectReferenceValue == null;
                }
            }
        }

        private bool IsPropertyNotNullInSceneAndPrefab(SerializedProperty property)
        {
            IsntNullAttribute myAttribute = this.fieldInfo.GetCustomAttribute<IsntNullAttribute>();
            //bool isPrefabAllowedNull = myAttribute.IgnorePrefab;
            //return EditorUtility.IsPersistent(property.serializedObject.targetObject) && isPrefabAllowedNull;
            return EditorUtility.IsPersistent(property.serializedObject.targetObject) && false;
        }
  
        private void BuildObjectField(Rect drawArea, SerializedProperty property, GUIContent label)
        {    
            if (property.propertyType != SerializedPropertyType.ObjectReference && 
                property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(drawArea, property, label, true);
                //EditorGUI.ObjectField(drawArea, property, label);  
                return;
            }

            if (this.IsPropertyNotNullInSceneAndPrefab(property))
            {
                // Render Object Field for NotNull (InScene) attributes on Prefabs.
                EditorGUI.BeginDisabledGroup(true); 
                EditorGUI.ObjectField(drawArea, property, label);  
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (IsNotWiredUp(property))
                    GUI.color = Color.yellow;

                if (property.propertyType == SerializedPropertyType.String)
                    EditorGUI.PropertyField(drawArea, property, label, true);
                else
                    EditorGUI.ObjectField(drawArea, property, label);
 
                GUI.color = Color.white;
            }
        } 
    }
}