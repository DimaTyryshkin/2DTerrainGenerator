using System;

namespace SiberianWellness.ScriptableObjectEditors.Editor
{
	public abstract class ScriptableObjectEditorBase
	{
		public abstract T CreateSubAsset<T>(Type type, T oldAsset)
			where T : EntityElement;

		public abstract void Save();
	}
}