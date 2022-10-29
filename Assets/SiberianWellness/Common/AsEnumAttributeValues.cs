using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace SiberianWellness.Common
{
	public static class AsEnumAttributeValues
	{ 
		class TypeInfo
		{
			public string[] labels;
			public string[] values; 
		}
		
		static Dictionary<Type, TypeInfo> typeToTypeInfo = new Dictionary<Type, TypeInfo>();

		#if UNITY_EDITOR
		//В реадкторе грузи сразу все типы чтобы валидация сработала
		static AsEnumAttributeValues()
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				var assemblyName = assembly.FullName;
				if (assemblyName.Contains("UnityEngine"))
					continue;

				if (assemblyName.Contains("mscorlib"))
					continue;

				foreach (var type in assembly.GetTypes())
				{
					var attribute = type.GetCustomAttribute<AsEnumSourceAttribute>();

					if (attribute != null)
						GetTypeInfo(type);
				}
			}
		}
		#endif

		public static bool IsValidValue(Type sourceType, string value)
		{
			return GetTypeInfo(sourceType).values.Contains(value); 
		}

		public static string[] GetAvailableLabels(Type type)
		{
			return GetTypeInfo(type).labels;
		}
		
		public static string[] GetAvailableValues(Type type)
		{
			return GetTypeInfo(type).values;
		}

		static TypeInfo GetTypeInfo(Type type)
		{
			Assert.IsNotNull(type);
			
			if (typeToTypeInfo.TryGetValue(type, out TypeInfo typeInfo))
			{
				return typeInfo;
			}
			else
			{
				var newTypeInfo = LoadType(type);
				typeToTypeInfo.Add(type, newTypeInfo);
				return newTypeInfo;
			}
		}

		static TypeInfo LoadType(Type type)
		{ 
			List<KeyValuePair<string, string>> keyToValueList = new List<KeyValuePair<string, string>>(32);
			LoadType(type, "", keyToValueList);

#if UNITY_EDITOR
			//Сортируем для красоты в редакторе

			var comparer = StringComparer.InvariantCultureIgnoreCase;
			keyToValueList.Sort((p1, p2) => comparer.Compare(p1.Key, p2.Key));

			for (int i = 0; i < keyToValueList.Count; i++)
			{
				if (keyToValueList[i].Key == "None")
				{
					var temp = keyToValueList[i];
					keyToValueList.RemoveAt(i);
					keyToValueList.Insert(0, temp);
				}
			}
#endif

			var labels = new string[keyToValueList.Count];
			var values = new string[keyToValueList.Count];

			for (int i = 0; i < keyToValueList.Count; i++)
			{
				labels[i] = keyToValueList[i].Key;
				values[i] = keyToValueList[i].Value;
			}

#if UNITY_EDITOR
			int n = labels.Length;
			for (int i = 0; i < n - 1; i++)
			{
				for (int k = i + 1; k < n; k++)
				{
					if (labels[i] == labels[k])
						throw new Exception($"Список ключей '{type.Name}' содержит дублирующиеся ключи '{labels[k]}'");

					if (values[i] == values[k])
						throw new Exception($"Список значений '{type.Name}' содержит дублирующиеся начение '{values[k]}'");
				}
			}
#endif

			TypeInfo info = new TypeInfo
			{
				labels = labels,
				values = values
			};


			return info;
		}

		static void LoadType(Type type, string prefix, List<KeyValuePair<string,string>> keyToValueList)
		{
#if UNITY_EDITOR
			if (string.IsNullOrWhiteSpace(prefix))
			{
				var attribute = type.GetCustomAttribute<AsEnumSourceAttribute>();
				Assert.IsNotNull(attribute, $"Тип '{type}' должен быть помечен аттрибутом {nameof(AsEnumSourceAttribute)}");
			}
#endif
			
			Assert.IsNotNull(type);
			var          allFields           = type.GetFields(BindingFlags.Public | BindingFlags.Static);

			foreach (var f in allFields)
			{
				if (f.FieldType == typeof(string) && f.IsInitOnly)
				{
					string value = (string) f.GetValue(null);
					string label = prefix + f.Name.Replace("_","/");

					keyToValueList.Add(new KeyValuePair<string, string>(label,value));
				}
			}

			var nestedTypes = type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public);

			foreach (var t in nestedTypes)
			{
				LoadType(t, prefix + t.Name + "/", keyToValueList);
			}
		}
	}
}