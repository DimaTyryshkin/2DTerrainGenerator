using UnityEngine;
using UnityEditor;

namespace SiberianWellness.Common
{ 
	// IngredientDrawer
	[CustomPropertyDrawer(typeof(RangeMinMaxInt))]
	public class RangeMinMaxIntDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			var minRectText = new Rect(position.x, position.y, 40, position.height);
			var minRect = new Rect(position.x + 45, position.y, 40, position.height);
			var maxRectText = new Rect(position.x + 90, position.y, 40, position.height);
			var maxRect = new Rect(position.x + +135, position.y, 40, position.height);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels  
			EditorGUI.LabelField(minRectText, "min");
			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), GUIContent.none);

			EditorGUI.LabelField(maxRectText, "max");
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
