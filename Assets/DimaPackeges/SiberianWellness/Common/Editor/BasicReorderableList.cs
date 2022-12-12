using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

using UnityEditorInternal;

namespace SiberianWellness.Common
{
	public static class BasicReorderableList
	{
		public static ReorderableList SetupReorderableList<T>(
			string          headerText,
			List<T>         elements,
			Func<T, string> elementLabel,
			Action<T>       selectElement,
			Action          createElement,
			Action<T>       onRemoveElement) where T:class
		{
			Assert.IsNotNull(elements);

			var list = new ReorderableList(elements, typeof(T), true, true, true, true)
			{
				drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, headerText); },
				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					var element = elements[index];
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), elementLabel(element));
				}
			};

			list.onSelectCallback = l =>
			{
				var selectedElement = elements[list.index];
				selectElement(selectedElement);
			};

			if (createElement != null)
			{
				list.onAddDropdownCallback = (buttonRect, l) => { createElement(); };
			}

			list.onRemoveCallback = l =>
			{
				if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this item?", "Yes", "No"))
				{
					return;
				}

				var element = elements[l.index];
				onRemoveElement?.Invoke(element); 
				ReorderableList.defaultBehaviours.DoRemoveButton(l);
			};

			return list;
		}
	}
}