using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using SiberianWellness.NotNullValidation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace MicroMachines.Lap
{
	public class LapRacerStatus
	{
		public int place;
		public int completeLaps;
		public int lastTriggerIndex;
		public float distanceToNextTrigger;
		public Racer racer;
	}

	public class LapCounter : MonoBehaviour
	{
		[SerializeField, IsntNull] LapTrigger[] triggers;

		public IReadOnlyList<LapRacerStatus> SortedRacersStatus => sortedRacersStatus;
		public event UnityAction StateChanged;

		Dictionary<Racer, LapRacerStatus> racersToRacerStatusMap;
		List<LapRacerStatus> sortedRacersStatus;
		int lapCount;
		int currentPlace;

		//--methods

		void Update()
		{
			foreach (LapRacerStatus racersStatus in sortedRacersStatus)
			{
				racersStatus.distanceToNextTrigger = GetDistanceToNextTrigger(racersStatus);
			}

			sortedRacersStatus.Sort(Compare);
		}

		public void StartRace(Racer[] cars, int lapCount)
		{
			Assert.IsTrue(lapCount > 0);
			currentPlace = 1;

			this.lapCount = lapCount;
			racersToRacerStatusMap = cars
				.Select(x => new LapRacerStatus()
				{
					completeLaps = 0,
					lastTriggerIndex = -1,
					racer = x
				})
				.ToDictionary(x => x.racer);

			foreach (LapTrigger trigger in triggers)
			{
				trigger.TriggerEnter += OnLapTriggerEnter;
			}

			sortedRacersStatus = racersToRacerStatusMap.Values.ToList();
		}

		void OnLapTriggerEnter(LapTrigger lapTrigger, Racer racer)
		{
			if (racersToRacerStatusMap.TryGetValue(racer, out LapRacerStatus lapRacerStatus))
			{
				if (lapRacerStatus.place == 0)
				{
					int lastIndex = lapRacerStatus.lastTriggerIndex;
					int triggerIndex = lapTrigger.TriggerIndex;

					if (lastIndex == triggerIndex - 1)
						lapRacerStatus.lastTriggerIndex = triggerIndex;

					if (lastIndex == triggers.Length - 1 && triggerIndex == 0)
					{
						lapRacerStatus.lastTriggerIndex = 0;
						lapRacerStatus.completeLaps++;

						if (lapRacerStatus.completeLaps == lapCount)
						{
							lapRacerStatus.place = currentPlace;
							currentPlace++;
						}
					}

					StateChanged?.Invoke();
				}
			}
		}

		int Compare(LapRacerStatus a, LapRacerStatus b)
		{
			if (a.place > 0 || b.place > 0)
			{
				if (a.place == 0)
					return 1;
				
				if (b.place == 0)
					return -1;
				
				return a.place.CompareTo(b.place);
			}

			int aScore = a.completeLaps * triggers.Length + a.lastTriggerIndex;
			int bScore = b.completeLaps * triggers.Length + b.lastTriggerIndex;

			if (aScore != bScore)
			{
				return -aScore.CompareTo(bScore);
			}
			else
			{
				return -b.distanceToNextTrigger.CompareTo(a.distanceToNextTrigger); // Чем больше растсояние, тем меньшее место игрок займет в таблице
			}
		}

		LapTrigger GetNextTrigger(LapRacerStatus racerStatus)
		{
			int nextTriggerIndex = (racerStatus.lastTriggerIndex + 1) % triggers.Length;
			return triggers[nextTriggerIndex];
		}

		float GetDistanceToNextTrigger(LapRacerStatus racerStatus)
		{
			return Vector3.Distance(GetNextTrigger(racerStatus).transform.position, racerStatus.racer.transform.position);
		}

#if UNITY_EDITOR
		[Button()]
		void LoadTriggers()
		{
			Undo.RecordObject(this, "LoadTriggers");
			triggers = GetComponentsInChildren<LapTrigger>()
				.OrderBy(x => x.transform.GetSiblingIndex())
				.ToArray();
		}
#endif
	}
}