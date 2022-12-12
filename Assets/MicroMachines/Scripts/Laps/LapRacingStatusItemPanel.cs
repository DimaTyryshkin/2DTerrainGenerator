using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.UI;

namespace MicroMachines.Lap
{
	public class LapRacingStatusItemPanel : MonoBehaviour
	{
		[SerializeField, IsntNull] Text racerNameText;
		[SerializeField, IsntNull] Image racerColorIcon;


		public void Draw(LapRacingStatusPanel.LapPanelItem item)
		{
			if (item.isPlayer)
				racerNameText.color = item.racerColor;
			
			racerNameText.text = item.racerName;
			racerColorIcon.color = item.racerColor;
		}
	}
}