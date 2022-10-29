using System.Collections; 
using UnityEngine;

namespace SiberianWellness.InputSystem
{
	public class TouchInputTest : MonoBehaviour,IClickHandler, IDragHandler, IDoubleClickHandler, IPointerUpDownHandler
	{ 
		public IEnumerator Start()
		{
			yield return null;
			yield return null;
			TouchInput.instance.ClickOnGameObject+= InstanceOnClickOnGameObject;
		}

		void InstanceOnClickOnGameObject(GameObject arg0)
		{
			if (arg0)
				Debug.Log("Click on Go. Name=" + arg0.name);
			else
				Debug.Log("Click in milk");
		}

		public void DoubleClick()
		{
			Debug.Log("DoubleClick");
		} 

		public void PointerUp()
		{
			Debug.Log("PointerUp");
		}

		public void PointerDown()
		{
			Debug.Log("PointerDown");
		}

		public void BeginDrag()
		{
			Debug.Log("BeginDrag");
		}

		public void Click(EventData eventData)
		{
			Debug.Log("Click");
		}
	}
}