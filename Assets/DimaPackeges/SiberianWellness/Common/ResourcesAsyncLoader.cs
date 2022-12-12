using System; 
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace SiberianWellness.Common
{
	/// <summary>
	/// Обертка над асинхронной загрузкой ресурсов. Сделана исключительно чтобы избежать ошибок при использовании ResourceRequest.
	/// Unity крашится если попытаться создать инстанс недогруженного объекта.
	/// Позволяет сэмитировать долгую загрузку и проверить игру в таком режиме.
	/// </summary> 
	public class ResourcesAsyncLoader<T> : CustomYieldInstruction where T : Object
	{
		readonly ResourceRequest request;
		readonly string          path;
		readonly bool            throwFileNotFound;

#if UNITY_EDITOR
		//Эмитация задержки при загрузке. Для тестирования.
		readonly float loadingDelay = 0;
		readonly float loadEndTime;
		bool           LoadingBlockedByDelay => Time.realtimeSinceStartup < loadEndTime;
#endif

		public T LoadedAsset
		{
			get
			{
#if UNITY_EDITOR
				if (!request.isDone || LoadingBlockedByDelay)
#else
				if (!request.isDone)
#endif
					throw new InvalidOperationException($"Cant asses to result while resource '{path}' is loading");

				if (!request.asset)
				{
					if (throwFileNotFound)
						throw new Exception($"Cant find resource '{path}'");
					else
						return null;
				}

				var result = request.asset as T;

				if (!result)
					throw new Exception($"Cant convert resource to target type. " +
					                    $"asset.name='{request.asset.name}' " +
					                    $"asset.type='{request.asset.GetType().Name}' " +
					                    $"targetType='{typeof(T).FullName}'");

				return result;
			}
		}

		public ResourcesAsyncLoader(string path, bool throwFileNotFound = true)
		{
			Assert.IsFalse(string.IsNullOrWhiteSpace(path));
			this.throwFileNotFound = true;

			this.path = path;
			request   = Resources.LoadAsync<T>(path);

#if UNITY_EDITOR
			loadEndTime = Time.realtimeSinceStartup + loadingDelay;
#endif
		}

		public override bool keepWaiting
		{
			get
			{
#if UNITY_EDITOR
				if (LoadingBlockedByDelay)
					return true;
#endif
				return !request.isDone;
			}
		}
	}
}