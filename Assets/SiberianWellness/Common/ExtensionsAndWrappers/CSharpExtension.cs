using System; 
using System.Collections.Generic;
using System.Linq;

namespace SiberianWellness.Common
{
	public static class EnumerableExtension
	{
		public static string ToStringMultiline<T>(this IEnumerable<T> collection, string header = null, Func<T, string> func = null, int tabCount = 0)
		{
			if (func == null)
				return collection.ToStringMultilineWithIndex(header, (t, i) => t.ToString(), tabCount);
			else
				return collection.ToStringMultilineWithIndex(header, (t, i) => func(t), tabCount);
		}

		public static string ToStringMultilineWithIndex<T>(this IEnumerable<T> collection, string header = null, Func<T,int,string> func = null, int tabCount = 0 )
		{
			if (func == null)
				func = (obj, i) => $"[{i:00}] '{obj}'"; 

			string tabs = "";
			for (int i = 0; i < tabCount; i++)
				tabs += "	"
					;
			string s = "";

			if (header != null)
				s +=$"{tabs}{header} [{collection.Count()}]{Environment.NewLine}";

			string prefix = string.IsNullOrWhiteSpace(header) ? "" : "    ";

			int index = 0;
			foreach (var element in collection)
			{
				s += tabs + prefix + func(element,index) + Environment.NewLine;
				index++;
			}

			return s;
		}

		public static IEnumerable<T> CastIfCan<TSource,T>(this IEnumerable<TSource> collection)
		{
			foreach (var element in collection)
			{
				if (element is T value)
					yield return value;
			}
		}
	}
	
	public static class StringExtension
	{
		public static string TrimEnd(this string origin, int count)
		{
			return origin.Substring(0, origin.Length - count);
		}
		
		public static bool ContainAny(this string origin, IList<string> values)
		{
			foreach (var value in values)
			{
				if (origin.Contains(value))
					return true;
			} 
			
			return false;
		}
		 
		
		public static int FindSubstringIndex(this string origin, string subString)
		{
			for (int i = 0; i < origin.Length; i++)
			{
				for (int j = 0; j < subString.Length; j++)
				{
					if (origin[i + j] != subString[j])
						break;

					if (j == subString.Length - 1)
						return i;
				}
			}

			return -1;
		}
	}

	public static class DictionaryWrapper
	{
		public static T2 GetOrDefault<T1, T2>(this IReadOnlyDictionary<T1, T2> dic, T1 key, T2 defaultValue)
		{
			if (dic.TryGetValue(key, out T2 val))
				return val;
			else
				return defaultValue;
		}
		
		public static T2 GetOrCreate<T1, T2>(this Dictionary<T1, T2> dic, T1 key, Func<T2>  factory)
		{
			if (dic.TryGetValue(key, out T2 val))
				return val;
			else
			{
				 var defaultValue =factory();
				 dic.Add(key, defaultValue);
				 return defaultValue;
			}
		}
	}

	public static class ListExtension
	{		
		public static T Random<T>(this List<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static int RandomIndex<T>(this List<T> list)
		{
			return UnityEngine.Random.Range(0, list.Count);
		}
		
		public static T Last<T>(this List<T> list)
		{
			return list[list.Count - 1];
		}
		 
		/// <summary>
		/// Shuffles the specified list.
		/// </summary> 
		public static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k     = UnityEngine.Random.Range(0, n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}

	public static class IEnumerableExtension
	{

		public static T FirstOrDefault<T>(IEnumerable<T> collection, Predicate<T> predicate) where T : class
		{
			foreach (var element in collection)
			{
				if (predicate(element))
					return element;
			}

			return null;
		}
	}

	public static class ArrayExtension
	{
		public static T Random<T>(this T[] array)
		{
			return array[UnityEngine.Random.Range(0, array.Length)];
		}

		public static T Random<T>(this T[] array, Random rnd)
		{ 
			return array[rnd.Next(0, array.Length)];
		}

		public static int RandomIndex<T>(this T[] array)
		{
			return UnityEngine.Random.Range(0, array.Length);
		}
		
		public static T Last<T>(this T[] array)
		{
			return array[array.Length - 1];
		}

		public static int IndexOf<T>(this T[] array, T element)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(element))
					return i;
			}

			return -1;
		}

		public static int IndexOf<T>(this T[] array, Predicate<T> predicate)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (predicate(array[i]))
					return i;
			}

			return -1;
		}
	}
}