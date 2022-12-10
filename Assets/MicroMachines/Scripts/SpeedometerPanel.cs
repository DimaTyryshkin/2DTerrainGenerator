using NaughtyAttributes;
using SiberianWellness.NotNullValidation;
using UnityEngine;


namespace MicroMachines
{
	public class SpeedometerPanel : MonoBehaviour
	{
		[SerializeField, IsntNull] RectTransform line; 
		[SerializeField] float maxValue; 
		[SerializeField]
		float minAngle, maxAngle;
 
		public void Draw(float value)
		{
			float k = Mathf.Clamp01(value / maxValue);
			line.eulerAngles = new Vector3(0, 0, Mathf.Lerp(minAngle, maxAngle, k));
		}

		
		#if UNITY_EDITOR
		[SerializeField] float TestDrawValue;

		[Button()] 
		void TestDraw()
		{
			Draw(TestDrawValue);
		}
		#endif
	}
}