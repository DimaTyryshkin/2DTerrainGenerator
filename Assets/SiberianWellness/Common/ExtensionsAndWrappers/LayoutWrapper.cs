using UnityEngine;
using UnityEngine.UI;

public static class LayoutWrapper
{
	public static void RefreshLayoutGroups(GameObject root)
	{
		foreach (var layoutGroup in root.GetComponentsInChildren<LayoutGroup>())
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
		}
	}
}
