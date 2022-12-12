using System;
using System.Linq;
using SiberianWellness.Common;
using UnityEditor;
using UnityEngine;

namespace SiberianWellness.Common
{
	[CustomPropertyDrawer(typeof(AsEnumAttribute))]
	public class AsEnumPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				var      a      = attribute as AsEnumAttribute;
				string[] labels = AsEnumAttributeValues.GetAvailableLabels(a.ValuesSource);
				string[] values = AsEnumAttributeValues.GetAvailableValues(a.ValuesSource);


				string curValue = property.stringValue;
				int    index    = values.IndexOf(curValue);

				EditorGUI.BeginChangeCheck();
				EditorGUI.BeginProperty(position, label, property);
				{
					if (index == -1)
					{
						position  = EditorGUI.PrefixLabel(position, label);
						GUI.color = Color.red;
 
						if (GUI.Button(position, curValue))
							index = 0;

						GUI.color = Color.white;

					}
					else
					{
						index = EditorGUI.Popup(position, property.displayName, index, labels);
					}
				}
				EditorGUI.EndProperty();

				if (EditorGUI.EndChangeCheck())
				{
					if (index > -1)
						property.stringValue = values[index];
				}
			}
			else
			{
				EditorGUILayout.HelpBox($"'{property.type}' is not supported by '{nameof(AsEnumAttribute)}'. Use 'string' instead", MessageType.Error);
			}
		} 
	}
}