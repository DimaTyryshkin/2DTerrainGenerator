using System;

namespace SiberianWellness.NotNullValidation
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class IsntNullAttribute : UnityEngine.PropertyAttribute
	{ 
		public bool AllowInAsset { get; set; }
		public bool Warring { get; set; }
	}
}