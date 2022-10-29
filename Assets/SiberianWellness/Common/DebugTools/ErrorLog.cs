using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace SiberianWellness.DebugTools
{
	public class ErrorLog
	{
		
		public struct ErrorEntity
		{
			public string condition;
			public string stacktrace;
		}

		static ErrorLog inst;
		
		List<ErrorEntity> errors = new List<ErrorEntity>();
 
		public event UnityAction NewError;

		public IReadOnlyList<ErrorEntity> Errors => errors;
		
		public ErrorLog()
		{
			inst = this;
			Application.logMessageReceived += OnLogMessageReceived;
		}

		[Conditional("QA")]
		[Conditional("DEV")]
		public static void Log(string condition, string stacktrace)
		{
			inst?.LogInternal(condition, stacktrace);
		}

		void OnLogMessageReceived(string condition, string stacktrace, LogType type)
		{
			if (type == LogType.Assert || type == LogType.Error || type == LogType.Exception)
				Log(condition, stacktrace);
		}
		
		void LogInternal(string condition, string stacktrace)
		{
			ErrorEntity entity = new ErrorEntity()
			{
				condition  = condition,
				stacktrace = stacktrace
			};
				
			errors.Add(entity);

			NewError?.Invoke();
		}

		public void Clear()
		{
			errors.Clear();
		}
	}
}