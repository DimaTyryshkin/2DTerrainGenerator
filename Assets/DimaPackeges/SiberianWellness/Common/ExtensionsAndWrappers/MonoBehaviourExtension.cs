using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SiberianWellness.Common
{
	public static class MonoBehaviourExtension
	{
		public static Coroutine StartCoroutine(this MonoBehaviour mb, IEnumerator enumerator, UnityAction completeCallBack)
		{
			return mb.StartCoroutine(Decorator(enumerator, completeCallBack));
		}

		static IEnumerator Decorator(IEnumerator enumerator, UnityAction completeCallBack)
		{
			yield return enumerator;
			completeCallBack();
		}
	}
}