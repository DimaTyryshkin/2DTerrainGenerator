using System;
using System.Collections;
using UnityEngine.Events;

namespace SiberianWellness.Common
{
	/// <summary>
	/// Обертка над коротинами позволяющая перехватывать асинхронные ошибки так же подменять асинхронный контектс(вызывать коротины без юнити)
	/// </summary>
	public class CatchExceptionCoroutineDecorator
	{
		CatchExceptionCoroutineDecorator parent;
		bool                             noExceptionCatch;
		IEnumerator                      enumerator;
		UnityAction<Exception>           exception;
		UnityAction                      success;

		public CatchExceptionCoroutineDecorator(IEnumerator enumerator)
		{
			noExceptionCatch = true;
			this.enumerator  = enumerator;
		}

		public CatchExceptionCoroutineDecorator(IEnumerator enumerator, UnityAction<Exception> exception)
			: this(enumerator)
		{
			this.exception = exception;
		}
        
		public CatchExceptionCoroutineDecorator(IEnumerator enumerator, UnityAction success)
			: this(enumerator)
		{
			this.success = success;
		}

		public CatchExceptionCoroutineDecorator(IEnumerator enumerator, UnityAction success, UnityAction<Exception> exception)
			: this(enumerator)
		{
			this.exception = exception;
			this.success   = success;
		}
 
		public IEnumerator MyCoroutine()
		{
			while (noExceptionCatch)
			{
				object current = null;
				try
				{
					if (enumerator.MoveNext() == false)
					{
						break;
					}

					current = enumerator.Current;
				}
				catch (Exception e)
				{
					OnException(e); 
					yield break;
				}

				if (current is IEnumerator newEnumerator)
				{
					CatchExceptionCoroutineDecorator newWrapper = new CatchExceptionCoroutineDecorator(newEnumerator, success, exception)
					{
						parent = this
					};
                 
					yield return newWrapper.MyCoroutine();
				}
				else
				{
					yield return current;
				}
			}

			if(noExceptionCatch)
				success?.Invoke();
		}

		void OnException(Exception e)
		{
			noExceptionCatch = false;
			
			if (parent != null)
			{
				parent.OnException(e);
			}
			else
			{
				exception?.Invoke(e);
			}
		}
	}
}