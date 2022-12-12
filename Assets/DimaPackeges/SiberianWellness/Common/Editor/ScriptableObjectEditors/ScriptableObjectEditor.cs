using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace SiberianWellness.ScriptableObjectEditors.Editor
{
	/// <summary>
	/// Смотри <see cref="ScriptableObjectEditorWindow{AssetT,EditorT,WindowT}"/>
	/// Базовый класс для всех кастомных редакторов которые работают c <see cref="ScriptableObject"/>
	/// Во первых тут вынесена логика которая не зависит от рисовки окошка в котором рендеристя редактор.
	/// Окошки у всех редакторов одинаковые и вынесены в класс <see cref="ScriptableObjectEditorWindow{AssetT,EditorT,WindowT}"/>, а уникальная логика самого редактора пишется в наследниках этого класса.
	/// Во вторых тут запилен фугкционал который правильно создает/удаялет вложенные ассеты. Это когда  ScriptableObject содержит в себе другие ScriptableObject
	/// </summary> 
	public abstract class ScriptableObjectEditor<TAsset>:ScriptableObjectEditorBase
		where TAsset :EntityRoot
	{
		public TAsset asset;
		
		public virtual void Validate()
		{
			
		}
		
		
		public virtual void OnSceneGui()
		{
			
		}
		
		public virtual void OnGui()
		{
			
		}
		
		public override T CreateSubAsset<T>(Type type, T oldAsset)   
		{
			if (oldAsset)
				oldAsset.Destroy();

			var newAsset = ScriptableObject.CreateInstance(type) as  EntityElement;
			Assert.IsNotNull(newAsset);
			newAsset.OnCreate(asset);
			
			AssetDatabase.AddObjectToAsset(newAsset, asset); 
			EditorUtility.SetDirty(newAsset);
			EditorUtility.SetDirty(asset);
             
			return newAsset as T;
		}
		
		protected Object CreateSubAsset2(Type type, Object oldAsset)   
		{
			if (oldAsset)
				(oldAsset as EntityElement).Destroy();

			var newAsset = ScriptableObject.CreateInstance(type) as  EntityElement;
			Assert.IsNotNull(newAsset);
			newAsset.OnCreate(asset);
			
			AssetDatabase.AddObjectToAsset(newAsset, asset); 
			EditorUtility.SetDirty(newAsset);
             
			return newAsset;
		}

		public override void Save()
		{
			EditorUtility.SetDirty(asset);
			AssetDatabase.SaveAssets();
		}
	}
}