using System.Collections;
using System.Linq;
using SiberianWellness.Common;
using SiberianWellness.NotNullValidation;
using UnityEngine;


namespace MicroMachines.TrafficLight
{
	public class TrafficLightManager : MonoBehaviour
	{
		[SerializeField, IsntNull] TrafficLightView trafficLightView;
		[SerializeField] float colorDelay = 1;
		[SerializeField] float enableAiDelay;

		IEnumerator Start()
		{
			BaseCarInput[] racers = FindObjectsOfType<BaseCarInput>()
				.Where(x => x.enabled)
				.ToArray();

			SetInputEnable(racers, false);


			trafficLightView.SetColor(TrafficLightView.Color.Red);
			yield return new WaitForSeconds(colorDelay);

			trafficLightView.SetColor(TrafficLightView.Color.Yellow);
			yield return new WaitForSeconds(colorDelay);

			trafficLightView.SetColor(TrafficLightView.Color.Green);

			var players = racers
				.Where(r => r is CarInput)
				.ToArray();

			SetInputEnable(players, true);

			var ai = racers.Except(players).ToArray();
			yield return SetInputEnableWithRandomDelay(ai, true);
		}

		void SetInputEnable(BaseCarInput[] racers, bool isEnable)
		{
			foreach (BaseCarInput racer in racers)
			{
				racer.InputEnable = isEnable;
			}
		}

		IEnumerator SetInputEnableWithRandomDelay(BaseCarInput[] racers, bool isEnable)
		{
			racers.Shuffle();
			foreach (BaseCarInput racer in racers)
			{
				racer.InputEnable = isEnable;
				yield return new WaitForSeconds(Random.Range(0, enableAiDelay));
			}
		}
	}
}