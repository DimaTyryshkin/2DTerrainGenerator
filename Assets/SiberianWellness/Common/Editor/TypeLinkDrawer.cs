using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor; 
using UnityEngine;

namespace SiberianWellness.Common.TypeLinking
{
	[CustomPropertyDrawer(typeof(TypeLink), true)]
	public class TypeLinkDrawer : PropertyDrawer
	{
		static Dictionary<Type, Type[]>   baseTypeToSubTypes       = new Dictionary<Type, Type[]>(); //baseTypeToAvailableType   
		static Dictionary<Type, string[]> typeNameForGuiCash   = new Dictionary<Type, string[]>();
		static Dictionary<Type, string[]> typeToFullName = new Dictionary<Type, string[]>();
		static Dictionary<string, Type>   baseTypeNameToType = new Dictionary<string, Type>();

		public static string CurrentBaseType { get; set; }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
 
			position   = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
  
			Type baseType = GetChashedBaseType(property);

			CurrentBaseType = null;
			
			if (baseType != null)
			{
				RenderPopup(property, baseType, position);
			}
			else
			{
				EditorGUI.LabelField(position, "Тип не задан");
			}

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}

		Type GetChashedBaseType(SerializedProperty property)
		{ 
			var baseTypeName = CurrentBaseType ?? property.FindPropertyRelative("baseType").stringValue;

			if (string.IsNullOrWhiteSpace(baseTypeName))
				return null;

			if (!baseTypeNameToType.TryGetValue(baseTypeName, out Type typeToRender))
			{
				var types = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(t => t.GetTypes());

				foreach (var type in types)
				{
					if (type.FullName == baseTypeName)
					{
						baseTypeNameToType.Add(baseTypeName, type);
						return type;
					}
				}
			}

			return typeToRender;
		}
		
		static void RenderPopup(SerializedProperty property, Type baseType, Rect popupRect)
		{
			if (!baseTypeToSubTypes.TryGetValue(baseType, out Type[] availableTypes))
			{
				var allTypes = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(t => t.GetTypes());

				availableTypes = allTypes
					.Where(t => t.IsSubclassOf(baseType))
					.ToArray();

				baseTypeToSubTypes.Add(baseType, availableTypes);

				var orderedTypes = availableTypes.OrderBy(t => t.Name).ToList();
				typeNameForGuiCash.Add(baseType, orderedTypes.Select(t => t.Name.Replace("_","/")).ToArray());
				typeToFullName.Add(baseType, orderedTypes.Select(t => t.FullName).ToArray());
			}

			string oldTypeName = property.FindPropertyRelative("typeName").stringValue;
			int    indexOld    = typeToFullName[baseType].IndexOf(t => t == oldTypeName);

			EditorGUI.BeginChangeCheck();
			int indexNew = EditorGUI.Popup(popupRect, indexOld, typeNameForGuiCash[baseType]);

			if (EditorGUI.EndChangeCheck())
			{
				string selectedTypeName = typeToFullName[baseType][indexNew];
				//Type selectedType = typeCash[nameSpace].First(t=>t.Name == selectedTypeName);
				property.FindPropertyRelative("typeName").stringValue =  selectedTypeName;
				//Тут надо писать полное имя типа а не это гавно

				Undo.RecordObject(property.serializedObject.targetObject, "ChangeKey");
			}
		} 
	}
}