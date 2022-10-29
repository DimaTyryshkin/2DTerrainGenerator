using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SiberianWellness.Common
{
	public class TimeOutCoroutineDecorator
	{
		readonly IEnumerator enumerator;
		readonly UnityAction success;
		readonly float       timeOut;

		public TimeOutCoroutineDecorator(IEnumerator enumerator, float timeLive)
		{
			timeOut         = Time.realtimeSinceStartup + timeLive;
			this.enumerator = enumerator;
		}

		public TimeOutCoroutineDecorator(IEnumerator enumerator, float timeOut, UnityAction success)
			: this(enumerator, timeOut)
		{
			this.success = success;
		}

		public IEnumerator MyCoroutine()
		{
			while (Time.realtimeSinceStartup < timeOut)
			{
				if (enumerator.MoveNext() == false)
					break;

				object current = enumerator.Current;

				yield return current;
			}

			success?.Invoke();
		}
	}
}