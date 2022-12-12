using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SiberianWellness.Common.TypeLinking
{
	[Serializable]
	public struct TypeLink
	{
#if UNITY_EDITOR
		public string baseType; //используется в редакторе 
#endif

		[SerializeField]
		string typeName;

		Type cashedType { get; set; }

		public TypeLink(string baseType)
		{
#if UNITY_EDITOR
			this.baseType = baseType;
#endif
			typeName      = null;
			cashedType = null;
		}

		public string TypeName => typeName;

		public Type LinkOnType
		{
			get
			{
				if (string.IsNullOrWhiteSpace(typeName))
					return null;
				
				if (cashedType != null)
				{
					if (cashedType.FullName == typeName)
						return cashedType;
				}

				var a = Assembly.GetCallingAssembly(); 
				cashedType = a.GetType(typeName); 
			 
				return cashedType;
			} 
		}
	}
}