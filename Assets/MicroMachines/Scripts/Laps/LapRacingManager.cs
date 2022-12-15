using System;
using System.Linq;
using MicroMachines.CarAi;
using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines.Lap
{
	public class LapRacingManager : MonoBehaviour
	{
		[SerializeField] int lapCount;
		[SerializeField, IsntNull] LapCounter lapCounter;
		[SerializeField, IsntNull] LapRacingStatusPanel lapRacingStatusPanel;

		LapRacerStatus playerStatus;


		void Start()
		{
			lapCounter.StateChanged += LapCounter_OnStateChanged;

			Racer[] allCars = FindObjectsOfType<Racer>();
			lapCounter.StartRace(allCars, lapCount);

			foreach (LapRacerStatus racer in lapCounter.SortedRacersStatus)
			{
				var input = racer.racer.GetComponent<CarInput>();
				if (input && input.enabled)
				{
					playerStatus = racer;
					break;
				}
			}

			lapRacingStatusPanel.gameObject.SetActive(true);
			Draw();
		}

		void Update()
		{
			Draw();
		}

		void LapCounter_OnStateChanged()
		{
			foreach (var racer in lapCounter.SortedRacersStatus)
			{
				if (racer.completeLaps == lapCount)
				{
					var carInput = racer.racer.GetComponent<CarInput>();
					var aiCarInput = racer.racer.GetComponent<NavigationFieldAiCarInput>();

					if (carInput)
						carInput.DisableEngine = true;

					if (aiCarInput)
						aiCarInput.InputEnable = false;
				}
			}
		}

		void Draw()
		{
			lapRacingStatusPanel.ClearList();
			foreach (var racerStatus in lapCounter.SortedRacersStatus)
			{
				lapRacingStatusPanel.AddItem(new LapRacingStatusPanel.LapPanelItem()
				{
					racerColor = racerStatus.racer.racerColor,
					racerName = racerStatus.racer.racerName,
					isPlayer = racerStatus == playerStatus

				});
			}

			lapRacingStatusPanel.UpdateLayout();

			LapRacerStatus status = playerStatus;
			if (status == null)
				status = lapCounter.SortedRacersStatus[0];

			lapRacingStatusPanel.DrawLaps(Mathf.Min(status.completeLaps + 1, lapCount), lapCount);
		}
	}
}