using System;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using SiberianWellness.NotNullValidation;
using UnityEditor;
using UnityEngine;

namespace MicroMachines.CarAi
{
	public class CarPathRecorder : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField, IsntNull] CarRigidbody carRigidbody;
		[SerializeField, IsntNull] CarEffects carEffects;

		[SerializeField] bool isRecord;

		static readonly int recorFrequncy = 10;
		CarPathRecord record;
		float timeStartRecord;
		float recordPeriod;
		float timeNextRecord;


		void Start()
		{
			timeStartRecord = Time.time;
			record = new CarPathRecord();
			recordPeriod = 1f / recorFrequncy;
			timeNextRecord = 0;
		}

		void Update()
		{
			if (!isRecord)
			{
				enabled = false;
				return;
			}

			if (Time.time < timeNextRecord)
				return;

			timeNextRecord = Time.time + recordPeriod;

			CarPathPoint point = new CarPathPoint
			{
				time = Time.time - timeStartRecord,
				pos = carEffects.transform.position,
				rot = carEffects.transform.rotation,
				wheelsPos = carEffects.wheelsView.Select(x => x.wheelView.position).ToArray(),
				wheelsRots = carEffects.wheelsView.Select(x => x.wheelView.rotation).ToArray()
			};

			record.points.Add(point);
		}

		[Button()]
		void SaveRecord()
		{
			string json = JsonUtility.ToJson(record, true);
			string filePath = EditorUtility.SaveFilePanelInProject("Save record", "record.json", "json", "Save record");

			string log = $"records count ={record.points.Count}" + Environment.NewLine;
			log += filePath;
			Debug.Log(log);
			File.WriteAllText(filePath, json);
		}
#endif
	}
}