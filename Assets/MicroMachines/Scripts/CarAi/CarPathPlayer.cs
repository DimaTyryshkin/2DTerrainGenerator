using SiberianWellness.Common;
using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines.CarAi
{
	public class CarPathPlayer : MonoBehaviour
	{
		[SerializeField, IsntNull] CarEffects carEffects;
		[SerializeField, IsntNull] TextAsset[] path;

		CarPathRecord record;
		int nextPointIndex;
		float timeStartPlaying;

		void Start()
		{
			Play();
		}

		void Play()
		{
			record = JsonUtility.FromJson<CarPathRecord>(path.Random().text);
			nextPointIndex = 1;
			ApplyPoint(record.points[0]);
			timeStartPlaying = Time.time;
		}

		void Update()
		{
			float time = Time.time - timeStartPlaying;

			while (time > record.points[nextPointIndex].time)
			{
				nextPointIndex++;
				if (nextPointIndex > record.points.Count - 1)
				{
					enabled = false;
					return;
				}
			}

			var lastPoint = record.points[nextPointIndex - 1];
			var nextPoint = record.points[nextPointIndex];
			float t = (time - lastPoint.time) / (nextPoint.time - lastPoint.time);
			LerpAndApplyPoint(lastPoint, nextPoint, t);
		}


		void ApplyPoint(CarPathPoint point)
		{
			carEffects.transform.position = point.pos;
			carEffects.transform.rotation = point.rot;

			for (int i = 0; i < point.wheelsPos.Length; i++)
			{
				var t = carEffects.wheelsView[i].wheelView.transform;
				t.position = point.wheelsPos[i];
				t.rotation = point.wheelsRots[i];
			}
		}

		void LerpAndApplyPoint(CarPathPoint p1, CarPathPoint p2, float t)
		{
			carEffects.transform.position = Vector3.Lerp(p1.pos, p2.pos, t);
			carEffects.transform.rotation = Quaternion.Lerp(p1.rot, p2.rot, t);

			for (int i = 0; i < p1.wheelsPos.Length; i++)
			{
				var wheelTransform = carEffects.wheelsView[i].wheelView.transform;
				wheelTransform.position = Vector3.Lerp(p1.wheelsPos[i], p2.wheelsPos[i], t);
				wheelTransform.rotation = Quaternion.Lerp(p1.wheelsRots[i], p2.wheelsRots[i], t);
			}
		}
	}
}