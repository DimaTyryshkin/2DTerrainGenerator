using System;

namespace SiberianWellness.NotNullValidation
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class IsntNullAttribute : UnityEngine.PropertyAttribute
	{
		public IsntNullAttribute(bool isError =true)
		{
			if (!isError)
				warring = true;
		}

		public bool AllowInAsset { get; set; }
		public readonly bool warring;
	}
}