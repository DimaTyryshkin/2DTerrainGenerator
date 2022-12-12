using System;
using System.Collections;
using System.Collections.Generic;

namespace SiberianWellness.Common
{
	/// <summary>
	/// Замеятет внутренности StartCoroutine. Это позволяет запускать коротины где угодно, например, в редакторе.
	/// </summary>
	public class CustomCoroutineHandler:IEnumerator
	{
		Stack<IEnumerator> stack;

		public CustomCoroutineHandler(IEnumerator unityCoroutine)
		{
			stack = new Stack<IEnumerator>();
			stack.Push(unityCoroutine);
		}

		public bool MoveNext()
		{
			if (stack.Count == 0)
				return false;

			IEnumerator head = stack.Peek();
			if(head.MoveNext())
			{
				if(head.Current is IEnumerator newEnum)
					stack.Push(newEnum);
			}
			else
			{
				stack.Pop();
			}
			
			return true;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public object Current { get; }
	}
}