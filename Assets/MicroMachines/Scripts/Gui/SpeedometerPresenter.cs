using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

namespace MicroMachines
{
	public class SpeedometerPresenter : MonoBehaviour
	{
		[SerializeField, IsntNull] SpeedometerPanel speedometerPanel;
		[SerializeField] SpeedometerPanel motorSpeedometerPanel;
		
		SimpleCarController simpleCarController;
		Transform target;
		Vector3 lastPosition;

		public void SetTarget(Transform target)
		{
			Assert.IsNotNull(target);
			
			lastPosition = target.position;
			this.target = target;
		}
		
		public void SetTarget(SimpleCarController simpleCarController)
		{
			Assert.IsNotNull(simpleCarController);
			this.simpleCarController = simpleCarController;
		}

		void Update()
		{
			//float speedMeterPerSecond = Mathf.Abs(car.ForwardSpeed);
			float speedMeterPerSecond = Vector3.Distance(lastPosition, target.position) / Time.deltaTime;
			
			float speedKmPerHour = speedMeterPerSecond * 3.6f;	
			speedometerPanel.Draw(speedKmPerHour);

			lastPosition = target.position;

			if (simpleCarController)
			{
				motorSpeedometerPanel.Draw(simpleCarController.MotorSpeedMeterPerSecond * 3.6f);
			}
		}
	}
}