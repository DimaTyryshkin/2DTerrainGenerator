using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace SiberianWellness.Common
{
	public class CoroutineResult<T>
	{
		public T result;
	}
	
	public class CoroutineResult<T1,T2>
	{
		public T1 result1;
		public T2 result2;
	}
	
	public class WaitForAll : CustomYieldInstruction
	{
		List<CustomYieldInstruction> allWaited = new List<CustomYieldInstruction>();

		public CustomYieldInstruction Add(IEnumerator coroutine, MonoBehaviour coroutineRouter)
		{
			var status = new ProcessStatus();
			coroutineRouter.StartCoroutine(CoroutineDecorator(coroutine, status));
			Add(status);
			
			return status;
		}
		
		public void Add(CustomYieldInstruction wait)
		{
			allWaited.Add(wait);
		}

		static IEnumerator CoroutineDecorator(IEnumerator coroutine, ProcessStatus status)
		{
			yield return coroutine;
			status.SetComplete();
		}

		public override bool keepWaiting => allWaited.Any(w => w.keepWaiting);
	}
 

	public class ProcessStatus : CustomYieldInstruction
	{
		bool isComplete;
		
		public void SetComplete()
		{
			isComplete = true;
		}
		
		public override bool keepWaiting => !isComplete;
	}
	 
	public class ProcessStatusWithTimeout : CustomYieldInstruction
	{
		readonly float timeOut;
		bool  complete;

		public bool IsTimeOut => Time.realtimeSinceStartup > timeOut;
		
		public ProcessStatusWithTimeout(float timeOut)
		{
			this.timeOut = Time.realtimeSinceStartup + timeOut;
		}

		public void SetComplete()
		{
			complete = true;
		}

		public override bool keepWaiting => !complete && !IsTimeOut;

	}
	
	public class MultiStepStatus : CustomYieldInstruction
	{
		public UnityAction completeCallback;
		int stepCounter;
		int totalStepCount;

		public string Name { get; }

		public MultiStepStatus(string name = null)
		{
			Name = name;
		}

		public void AddStep()
		{
			totalStepCount++;
			if(!string.IsNullOrEmpty(Name))
				Debug.Log($"{Name} total:{totalStepCount} complete:{stepCounter}");
		}
		
		public void EndStep()
		{
			stepCounter++;
			Assert.IsFalse(stepCounter > totalStepCount);
			if(!string.IsNullOrEmpty(Name))
				Debug.Log($"{Name} total:{totalStepCount} complete:{stepCounter}");
			
			if(!keepWaiting)
				completeCallback?.Invoke();
		}

		public override bool keepWaiting
		{
			get
			{
				bool waiting = stepCounter != totalStepCount;
				return waiting;
			}
		}
	}
	
	public class ProcessStatus<T> : CustomYieldInstruction
	{
		bool isComplete;
		T    result;


		public T Result
		{
			get { return result; }
			set
			{
				result     = value;
				isComplete = true;
			}
		}

		public override bool keepWaiting => !isComplete;
	}
	
	public static class CoroutineDecorators
	{
		public static IEnumerator WithCallBack(IEnumerator coroutine, UnityAction onComplete)
		{
			yield return coroutine;
			onComplete?.Invoke();
		}
	}
}