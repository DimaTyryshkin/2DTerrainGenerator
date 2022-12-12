using System.Collections.Generic;
using UnityEngine.Assertions;

namespace SiberianWellness.Common
{
	public static class AssertWrapper
	{
		public static void IsAllNotNull<T>(T[] array, string msg = null) where T : class
		{
			Assert.IsNotNull(array, msg);

			foreach (var e in array)
			{
				Assert.IsNotNull(e, msg);
			}
		}

		public static void IsAllNotNull<T>(List<T> array, string msg = null) where T : class
		{
			Assert.IsNotNull(array, msg);

			foreach (var e in array)
			{
				Assert.IsNotNull(e, msg);
			}
		}

		public static void IsAllNotNull<T>(ICollection<T> array, string msg = null) where T : class
		{
			Assert.IsNotNull(array, msg);

			foreach (var e in array)
			{
				Assert.IsNotNull(e, msg);
			}
		}
	}
}