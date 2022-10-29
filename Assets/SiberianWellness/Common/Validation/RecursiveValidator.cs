using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using SiberianWellness.Common;
using UnityEngine;

using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SiberianWellness.Validation
{
	/// <summary>
	/// Проходит по дереву объектов и находит проблемы при пимоши валидаторов.
	/// Используется для поиска проблем в ассетах как в редакторе, так и в рантайме.
	/// </summary>
	public class RecursiveValidator
	{
		public class SearchScope : IDisposable
		{
			bool printFullInfo;
			
			//Progress
			int current;
			int total;
			DateTime timeStart;

			public SearchScope(bool printFullInfo)
			{    
				ResetStats();
				timeStart = DateTime.Now;
				this.printFullInfo = printFullInfo;
				allScanedObject = new HashSet<object>();
				validationProblems  = new List<ValidationProblem>();

#if UNITY_EDITOR
				if(printFullInfo)
					EditorUtility.DisplayProgressBar("Валидация", "", 0.0f);
#endif
			}


			public void AddTotalObjectsCount(int add)
			{
				total += add;
			}

			public void AddProgress(int add)
			{
				current += add;
#if UNITY_EDITOR
				if(printFullInfo)
					EditorUtility.DisplayProgressBar("Валидация", "", current / (float) total);
#endif
			}

			void IDisposable.Dispose()
			{
#if UNITY_EDITOR
				EditorUtility.ClearProgressBar();
#endif

				searchScope = null;

				totalTime = DateTime.Now - timeStart;
				PrintStats(printFullInfo);

				ResetStats();
			}
		}

		static SearchScope             searchScope;
		static ValidationContext       validationContext;
		static List<ValidationProblem> validationProblems;
		static HashSet<System.Object>  allScanedObject;
		
		public static ValidationContext ValidationContext => validationContext;

		static AbstractValidator[] validators;
        
		static int      maxActualDepth      = 0;
		static int      totalDepthSum       = 0;
		static int      totalUnityObjects   = 0;
		static int      totalObject         = 0;
		static int      maxDepthInIteration = 0;
		static TimeSpan totalTime           = new TimeSpan();


		public static SearchScope StartSearch(AbstractValidator validator, bool printFullInfo)
		{
			return StartSearch(new[] {validator}, printFullInfo);
		}

		public static SearchScope StartSearch(AbstractValidator[] validatorsForSearchProblems, bool printFullInfo)
		{
			AssertWrapper.IsAllNotNull(validatorsForSearchProblems);

			if (searchScope != null)
				throw new InvalidOperationException("Last search should be finished before start new search");

			searchScope = new SearchScope(printFullInfo);
			validators  = validatorsForSearchProblems.ToArray();
			validationContext = new ValidationContext();
			return searchScope;
		}

		public static void AddValidationProblem(ValidationProblem problem)
		{
			validationProblems.Add(problem);
		}

		static void ResetStats()
		{
			validationContext  = null;
			validationProblems = null;
			allScanedObject    = null;
			validators         = null;

			totalObject       = 0;
			maxActualDepth    = 0;
			totalUnityObjects = 0;
			totalDepthSum     = 0;
			totalTime         = new TimeSpan();
		}

		static void PrintStats(bool fullInfo)
		{ 
			var allScenes = validationProblems 
				.GroupBy(p => p.sceneName);

			
			foreach (var problemOnScene in allScenes)
			{
				Debug.Log($"<b>{problemOnScene.Key}</b>");
				foreach (var problem in problemOnScene.OrderBy(entry => entry.header))
				{
					if (problem.type == ValidationProblem.Type.Error)
						Debug.LogError(problem, problem.root);
					else
						Debug.LogWarning(problem, problem.root);
				}
			}
			
			if (fullInfo)
			{
				string msg = "Stats:" + Environment.NewLine;
				msg += "TotalObjectCount = " + totalObject + Environment.NewLine;
				msg += "MaxActualDepth = " + maxActualDepth + Environment.NewLine;
				msg += "AverageActualDepth = " + ((float) totalDepthSum) / totalUnityObjects + Environment.NewLine;
				msg += "Elapsed time = " + Mathf.RoundToInt((float)totalTime.TotalSeconds) + " sec" + Environment.NewLine;

				Debug.Log(msg);
			}
		}

		public static void ValidateGo(GameObject sourceObject)
		{
			MonoBehaviour[] monoBehaviours = sourceObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour t in monoBehaviours)
			{
				try
				{
					ValidateObject(t, sourceObject);
				}

				catch (System.ArgumentNullException e)
				{
					//Эта ветка перехватывает случаи Missing(Mono Script)
					ValidationContext.AddProblem("Missing script", ValidationProblem.Type.Error, e.Message + " object=" + sourceObject.FullName(),sourceObject);
				}
			}
		}

		public static void ValidateObject(Object obj, Object root = null)
		{
			if(searchScope == null)
				throw new InvalidOperationException("search should start with a call " + nameof(StartSearch));
 
			if (!obj) 
			{
				throw new ArgumentNullException("MonoBehaviour is null. It likely references a script that's been deleted.");
			}
  
			totalUnityObjects++;
			maxDepthInIteration = 0;
			MakeRecursion(obj, root? root:obj,null,0);
			totalDepthSum += maxDepthInIteration; 
		}

		static bool NeedInCheck(Type type)
		{
			var assemblyName = type.Assembly.FullName;
			if (assemblyName.Contains("UnityEngine"))
				return false; //Отличное место для проверки Image.sprite

			if (assemblyName.Contains("mscorlib"))
				return false;

			return true;
		}

		static void GetFieldsIncludeInherited(Type type, List<FieldInfo> result)
		{
			var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

			result.AddRange(fields);
			if (NeedInCheck(type.BaseType))
				GetFieldsIncludeInherited(type.BaseType, result);
		}

		static void MakeRecursion(System.Object obj, Object root, FieldInfo field, int depth)
		{
			validationContext.currentRoot = root;
			validationContext.currentFieldInfo = field;

			if (IsNull(obj))
				return;
			
			var type = obj.GetType();
			
			if (type.IsClass)
			{
				if (allScanedObject.Contains(obj))
					return;

				allScanedObject.Add(obj);
			}
 
			if(!NeedInCheck(type))
				return;
			
			 
			totalObject++;  
			 
			if (depth > maxDepthInIteration)
				maxDepthInIteration = depth;

			if (depth > maxActualDepth)
				maxActualDepth = depth;

			foreach (var v in validators)
				v.FindProblemsInObject(obj, obj?.GetType(), root);

			if (obj is IValidated validated)
			{
				try
				{
					validated.Validate(validationContext);
				}
				catch (Exception e)
				{
					validationContext.AddProblem("Exception while validate", ValidationProblem.Type.Error, e.ToString(), root);
				}
			}

			List<FieldInfo> fields= new List<FieldInfo>();
			GetFieldsIncludeInherited(obj.GetType(), fields);
			 
			foreach (FieldInfo f in fields)
			{ 
				validationContext.currentFieldInfo = f;
				
#if UNITY_EDITOR
				//Поля не сереализуемые в резакторе скипаем если игра не запущена. Они будут заполнены только в рантайме.
				if (!f.IsPublic && EditorApplication.isPlaying == false)
				{
					SerializeField sf = f.GetCustomAttribute<SerializeField>();
					if (sf == null)
						continue;
				}
#endif

				object fieldObject = f.GetValue(obj);

				foreach (var v in validators)
					v.FindProblemsInField(fieldObject, fieldObject?.GetType(), f, root);
				
				if(NeedInRecursion(fieldObject))
					MakeRecursion(fieldObject, root, f, depth++);

				if (fieldObject is ICollection collection) //Нельзя ходить по IEnumareble, так как будем залазить в символы строки=(
				{
					foreach (var item in collection)
					{
						foreach (var v in validators)
							v.FindProblemsInField(item, item?.GetType(), f, root);

						if(NeedInRecursion(item))
							MakeRecursion(item, root, f, depth++);
					}
				}
			}
		}

		static bool NeedInRecursion(System.Object obj)
		{
			if (obj is ScriptableObject so)
			{
#if UNITY_EDITOR
				string path          = AssetDatabase.GetAssetPath(so);
				bool   existInAssets = !string.IsNullOrEmpty(path);

				if (existInAssets)
				{
					bool isMainAsset = AssetDatabase.LoadMainAssetAtPath(path) == so;
					return !isMainAsset;
				}
#else
				return true;
#endif
			}

			//не заходим никуда больлше, все остльные либо не нужны, либо будут проверены и так 
			if (obj is UnityEngine.Object)
				return false;

			return true;
		}

		static bool IsNull(object obj)
		{ 
			if (obj == null || obj.Equals(null))
			{
				return true;
			}
			else
			{  
				return false;
			}
		} 
	}
}