using UnityEngine;

namespace SiberianWellness.Common
{
	public static class OperationSystemClipboard
	{
		public static void WriteText(string text)
		{
			GUIUtility.systemCopyBuffer = text;
		}

		public static string ReadText()
		{
			return GUIUtility.systemCopyBuffer;
		}
	}
}