using UnityEngine.Assertions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SiberianWellness.NotNullValidation.Internal
{
#if UNITY_EDITOR
	[CreateAssetMenu]
	public class NotNullFieldValidator_Config : ScriptableObject
	{
		static NotNullFieldValidator_Config inst;

		public static NotNullFieldValidator_Config GetInst()
		{
			if (!inst)
			{
				inst = Resources.Load<NotNullFieldValidator_Config>("EditorTools/NotNullFieldValidator_Config");
				Assert.IsNotNull(inst);
			}

			return inst;
		}

		public string[] excludeAssetsPathFromLog;

		public bool ExcludeFromLog(Object root)
		{
			foreach (string explodePath in excludeAssetsPathFromLog)
			{
				string pathToRoot = AssetDatabase.GetAssetPath(root);
				if (pathToRoot.Length > 0 && pathToRoot.StartsWith(explodePath))
					return true;
			}

			return false;
		}
	}
#endif
}