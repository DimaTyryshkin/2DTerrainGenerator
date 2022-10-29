using System.Collections; 
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SiberianWellness.Common
{
	public static class Fade
	{ 
		public static IEnumerator FromToPosition(Transform obj, Vector3 objPos, Vector3 targetObj, float duration)
		{
			void SetPos(float t) => obj.position =  Vector3.Lerp(objPos, targetObj, t);
			yield return FromTo(0, 1, duration, SetPos);
		}
		
		public static IEnumerator ColorFromTo(Graphic graphic, Color from, Color to, float duration)
		{
			void SetColor(float t) => graphic.color = Color.Lerp(from, to, t);
			yield return FromTo(0, 1, duration, SetColor);
		}
		
		public static IEnumerator AlphaFromTo(Graphic targetObj, float from, float to, float duration, UnityAction callback)
		{
			yield return AlphaFromTo(targetObj, from, to, duration);
			callback.Invoke();
		}
		
		public static IEnumerator AlphaFromTo(Graphic graphic, float from, float to, float duration)
		{
			Assert.IsTrue(to<= 1 && to>=0);
			Assert.IsTrue(from<= 1 && from>=0);
			
			Color color = graphic.color;
			void SetAlpha(float t)
			{
				color.a = t;
				graphic.color = color;
			}
			
			yield return FromTo(from, to, duration, SetAlpha);
		}

		public static IEnumerator RectScaleFromTo(Transform transform, Vector2 from, Vector2 to, float duration)
		{
			void SetSize(float t) => transform.SetLocalScale(Vector2.Lerp(from, to, t));
			yield return FromTo(0, 1, duration, SetSize);
		}
		
		public static IEnumerator RectScaleFromTo(RectTransform rect, Vector2 from, Vector2 to, float duration)
		{
			void SetSize(float t)
			{
				var size = Vector2.Lerp(from, to, t);
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}

			yield return FromTo(0, 1, duration, SetSize);
		}

		public static IEnumerator FromTo(float from, float to, float duration, UnityAction<float> setter)
		{
			if (Mathf.Approximately(duration,0))
			{
				setter(to);
				yield break;
			}

			float timeStart = Time.time;
			float t         = 0;

			setter(from);
			while (true)
			{
				yield return null;
			
				t = (Time.time - timeStart) / duration;
				float value = Mathf.Lerp(from, to, t);
				setter(value);

				if (t >= 1)
					break;
			}
		}
	}
}