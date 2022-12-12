using System.Collections.Generic;
using UnityEngine;

namespace SiberianWellness.ScriptableObjectEditors
{
	/// <summary>
	/// Базовый клас для рутовых сереализуемых конфигов
	/// Содержит список вложенных объектов
	///
	/// Все ScriptableObject`ы , которые содержват в себе другие ScriptableObject должны наследоваться от этого класса.
	/// Это позволяет использовать уже реализованый функционал для правильного создания\уделаения объектов во время редактирования их в UnityEditor
	/// Кроме того это нужно  для работы <see cref="BugFixer"/>
	/// </summary>
	public class EntityRoot : ScriptableObject
	{
		[HideInInspector]
		public List<EntityElement> elements=new List<EntityElement>();
	}
}