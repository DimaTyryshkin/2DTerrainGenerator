using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines.TrafficLight
{
	public class TrafficLightView : MonoBehaviour
	{
		public enum Color
		{
			Red = 1,
			Yellow = 2,
			Green = 3, 
		}

		[SerializeField, IsntNull] GameObject[] red;
		[SerializeField, IsntNull] GameObject[] yellow;
		[SerializeField, IsntNull] GameObject[] green;

		public void SetColor(Color color)
		{
			DisableAll();

			if (color == Color.Red)
				SetEnable(red, true);

			if (color == Color.Yellow)
				SetEnable(yellow, true);

			if (color == Color.Green)
				SetEnable(green, true);
		}

		void DisableAll()
		{
			SetEnable(red, false);
			SetEnable(yellow, false);
			SetEnable(green, false);
		}

		void SetEnable(GameObject[] objects, bool isEnable)
		{
			foreach (var obj in objects)
				obj.SetActive(isEnable);
		}
	}
}