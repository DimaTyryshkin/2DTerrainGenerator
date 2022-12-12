using SiberianWellness.Common;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.UI;

namespace MicroMachines.Lap
{
	public class LapRacingStatusPanel : MonoBehaviour
	{
		[SerializeField, IsntNull] Transform itemsRoot;
		[SerializeField, IsntNull] Text currentLaps;
		[SerializeField, IsntNull] Text maxLaps;
		[SerializeField, IsntNull] LapRacingStatusItemPanel itemTemplate;
		
		public struct LapPanelItem
		{
			public string racerName;
			public bool isPlayer;
			public Color racerColor;
		}

		public void ClearList()
		{
			itemsRoot.DestroyChildren();
		}

		public void AddItem(LapPanelItem item)
		{
			LapRacingStatusItemPanel itemPanel = itemsRoot.InstantiateAsChild(itemTemplate);
			itemPanel.gameObject.SetActive(true);
			itemPanel.Draw(item);
		}

		public void UpdateLayout()
		{
			LayoutWrapper.RefreshLayoutGroups(itemsRoot);
		}

		public void DrawLaps(int currentLaps, int maxLaps)
		{
			this.currentLaps.text = currentLaps.ToString();
			this.maxLaps.text = maxLaps.ToString();
		}
	}
}