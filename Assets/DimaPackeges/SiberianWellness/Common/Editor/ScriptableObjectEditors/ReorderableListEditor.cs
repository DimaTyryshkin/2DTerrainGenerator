using System;
using System.Collections.Generic;
using SiberianWellness.Common;
using SiberianWellness.ScriptableObjectEditors;
using SiberianWellness.ScriptableObjectEditors.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace SiberianWellness.ScriptableObjectEditors.Editor
{
	/// <summary>
	/// Рисует список объектов с возможностью по кнопочке "+" добавлять новые.
	/// Работает только с <see cref="EntityElement"/> объектами
	/// </summary>
	public class ReorderableListEditor<T>
		where  T: EntityElement
	{
		public ScriptableObjectEditorBase editor;
		Type                         currentType;

		ReorderableList list;
		T               selectedElement;
		List<T>         collection;

		public T Selected => selectedElement;

		public ReorderableListEditor(ScriptableObjectEditorBase editor, List<T> collection, Type[] typesToCreate , string header, Func<T,string> elementName)
		{
			int removedCount =collection.RemoveAll(e => !e);
			if (removedCount > 0)
				editor.Save();

			Assert.IsNotNull(collection);

			this.collection = collection;
			this.editor     = editor;

			list = BasicReorderableList.SetupReorderableList(
				header,
				collection,
				element =>element?elementName(element):"null",
				selected => selectedElement = selected,
				() =>
				{
					if (typesToCreate.Length == 1)
					{
						AddNewElement(typesToCreate[0]);
					}
					else
					{
						var menu = new GenericMenu();
						foreach (var t in typesToCreate)
							menu.AddItem(new GUIContent(t.Name), false, AddNewElement, t);

						menu.ShowAsContext();
					}
				},
				(removed) => 
				{
					removed.Destroy();
					editor.Save();
					selectedElement = null; 
				}
			);
		}

		void AddNewElement(object userdata)
		{
			var t = userdata as Type;
			var c =  editor.CreateSubAsset<T>(t,null);
			Assert.IsNotNull(c);
			 
			collection.Add(c);
			editor.Save();
		}

		public void OnGui()
		{   
			GUILayout.BeginVertical("box");
			{ 
				list.DoLayoutList();
  
				if (selectedElement)
				{
					OnGuiElementEditor(selectedElement);
					//InspectorEditor.drawScriptForNextEditor = false;
					//var e = UnityEditor.Editor.CreateEditor(selectedElement);
					//e.OnInspectorGUI();s
				}

			}
			GUILayout.EndVertical(); 
		}

		protected virtual void OnGuiElementEditor(T element)
		{
			var e = UnityEditor.Editor.CreateEditor(element);
			e.OnInspectorGUI();
		}
	}
}