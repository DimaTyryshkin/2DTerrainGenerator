using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;

namespace MicroMachines
{
	public class SpeedometerPresenter : MonoBehaviour
	{
		[SerializeField, IsntNull] SpeedometerPanel speedometerPanel;
		Speedometer speedometer;

		public void SetTarget(Transform target)
		{
			Assert.IsNotNull(target);
			speedometer = Speedometer.Get(target);
		}
		 
		void Update()
		{
			speedometerPanel.Draw(speedometer.SpeedKm);
		}
	}
}