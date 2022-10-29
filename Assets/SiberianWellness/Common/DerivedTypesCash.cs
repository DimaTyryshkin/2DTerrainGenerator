using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiberianWellness.Common;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SiberianWellness.Common
{
	public class DerivedTypesCash<T>
	{
		const bool UseCash = true;
		
		Type[]   types;
		string[] names;
     
		public Type[] Types {
			get
			{
				if (types == null || !UseCash)
					types = GetDerivedTypes(typeof(T));

				return types;
			}
		}

		public string[] Names
		{
			get
			{
				if (names == null)
					names = Types.Select(t => t.Name).ToArray();

				return names;
			}
		}

		#if UNITY_EDITOR
		public T DrawPopup<T>(T current, Func<Type,T,T> onChanged)
			where T:Object
		{
			Assert.IsNotNull(onChanged);
			
			Type type = null;
			if(current)
				type = current.GetType();
			
			int currentIndex = Types.IndexOf(t => t == type);
           
			GUI.changed = false;
			int index = EditorGUILayout.Popup(currentIndex, Names);
			if (GUI.changed)
				return onChanged(Types[index], current) as T;
			else
				return current;
		}
		#endif

		static Type[] GetDerivedTypes(Type baseType)
		{
			List<Type> types      = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				try
				{
					var typesToAdd = assembly.GetTypes()
						.Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
						.ToArray();
                    
					types.AddRange(typesToAdd);
				}
				catch (ReflectionTypeLoadException)
				{
				}
			}

			return types.ToArray();
		}
	}
}