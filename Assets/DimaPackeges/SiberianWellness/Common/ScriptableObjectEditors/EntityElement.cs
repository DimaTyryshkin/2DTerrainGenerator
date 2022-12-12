using UnityEngine;

namespace SiberianWellness.ScriptableObjectEditors
{
	/// <summary>
	/// Смотри описание <seealso cref="EntityRoot"/>
	/// </summary>
	public abstract class EntityElement : ScriptableObject
	{
		[HideInInspector]
		public EntityRoot root;
		
		public virtual void OnSceneGui()
		{
		}

		public void OnCreate(EntityRoot root)
		{
			this.root = root;
			root.elements.Add(this);
		}

		public virtual void Destroy()
		{
			root.elements.Remove(this);
			DestroyImmediate(this, true);
		}
	}
}