using System.Reflection;
using UnityEngine;

namespace SiberianWellness.Validation
{
	public class ValidationContext
	{
		public Object    currentRoot;
		public FieldInfo currentFieldInfo;
		public string    currentScene;
		
		public void AddProblem(string header, ValidationProblem.Type type, string msg = null, Object overrideRoot = null)
		{
			ValidationProblem problem = new ValidationProblem();
			problem.type      = type;
			problem.root      = currentRoot;
			problem.fieldInfo = currentFieldInfo;
			problem.sceneName = currentScene;

			problem.header = header;
			problem.msg    = msg;

			if (overrideRoot)
				problem.root = overrideRoot;
			
			problem.CacheToString();
			RecursiveValidator.AddValidationProblem(problem);
		}
	}
}