using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace SiberianWellness.Validation
{
	public abstract class AbstractValidator
	{
		public abstract void FindProblemsInField(object value, Type valueType, FieldInfo ownerFieldInfo, Object rootObject);

		public abstract void FindProblemsInObject(object value, Type valueType, Object rootObject);
	}
} 