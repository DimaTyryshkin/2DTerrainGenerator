using SiberianWellness.Common;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace SiberianWellness.InputSystem
{
	public interface ITouchInputHandler
	{
	}

	public interface IClickHandler : ITouchInputHandler
	{
		void Click(EventData eventData);
	}

	public interface IDoubleClickHandler : ITouchInputHandler
	{
		void DoubleClick();
	}

	public interface IPointerUpDownHandler : ITouchInputHandler
	{
		void PointerUp();
		void PointerDown();
	}

	public interface IDragHandler : ITouchInputHandler
	{
		void BeginDrag();
	}

	public struct EventData
	{
		public Vector3 startPoint;
	}

	public class TouchInput : MonoBehaviour
	{
		public static TouchInput instance;
		
		enum State
		{
			NoTouch = 0,
			OneTouch,
			TwoTouch,
			MultiTouch,
		}
 
		[SerializeField, IsntNull]
		GuiHit guiHit;
		
		[SerializeField, IsntNull] 
		GameObject pointerImage;

		[SerializeField] 
		bool showClick;
		
		[SerializeField] 
		bool logClickHandlers;
		
		Camera rayCastCamera;

		float defaultPixelToUnitX;
		float defaultPixelToUnitY;
		float pixelToUnitX;
		float pixelToUnitY;
		float clickDelay = 0.4f;
		float minimalSwapDistance = 30;//пикселей
		float clickTime;
		float doubleClickTime;

		bool    guiUnderPointerOnLastClick;
		bool    pressed;
		bool    isDrag;
		Vector3 startDragPoint;
		Vector3 startDragScreenPoint;
		Vector3 clickPoint;

		ITouchInputHandler[] targets;
		ITouchInputHandler[] doubleClickTargets;
		
		//doubleTouch
		/// <summary>
		/// Начальное расстояние между двумя тапами
		/// </summary>
		float lastDoubleTouchDelta;

		State touchState = State.NoTouch;

		public event UnityAction<Vector3> Drag;
		public event UnityAction<float>   Scroll;

		public event UnityAction<GameObject> ClickOnGameObject;
		public event UnityAction FingerTouchEnded;

		public ITouchInputHandler[] Targets => targets;

		public bool InputAvailable { get; set; } = true;
  
        GameObject go;

        public void Init(Camera rayCastCamera)
        { 
	        Assert.IsTrue(instance==null);
	        instance = this;
	        
	        this.rayCastCamera = rayCastCamera;
	        
	        pixelToUnitX = defaultPixelToUnitX = GetPixelPerUnit(new Vector3(100, 0));
	        pixelToUnitY = defaultPixelToUnitY = GetPixelPerUnit(new Vector3(0, 100));
        }

        float GetPixelPerUnit(Vector3 screenPoint2)
        {
	        Vector3 screenPoint1 = new Vector3(0, 0); 
	        var     p1           = rayCastCamera.ScreenPointToWorldPointOnPlane(screenPoint1, Plaine.XZ);
	        var     p2           = rayCastCamera.ScreenPointToWorldPointOnPlane(screenPoint2, Plaine.XZ);

	        return (p2 - p1).magnitude / (screenPoint2 - screenPoint1).magnitude;
        }

        public void ChangePixelPerUnit(float multiply)
        {
	        pixelToUnitX = defaultPixelToUnitX * multiply;
	        pixelToUnitY = defaultPixelToUnitY * multiply;
        }

        void LateUpdate()
		{ 
			if(!instance)
				return;
 
			var count = Input.touchCount;
#if UNITY_EDITOR
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
				count = 1;

			if (Input.GetMouseButtonUp(0))
				count = 1;
			
			float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
			if (mouseWheel != 0 && InputAvailable)
				Scroll?.Invoke(mouseWheel * 500);
#endif
			if (count == 1 && (touchState == State.NoTouch || touchState == State.OneTouch))
			{
				touchState = State.OneTouch;
				Vector3    pointPos   = Vector3.zero;
				TouchPhase touchPhase = TouchPhase.Moved;

#if UNITY_EDITOR
				//PC
				pointPos = Input.mousePosition;

				if (Input.GetMouseButtonDown(0))
					touchPhase = TouchPhase.Began;

				if (Input.GetMouseButtonUp(0))
					touchPhase = TouchPhase.Ended;
#else
				//Mobile
				var touch = Input.touches[0];
				pointPos = touch.position;
				touchPhase = touch.phase;
#endif
				ProcessOneTouch(pointPos, touchPhase);
			}

#if !UNITY_EDITOR
			if (count != 1 && touchState == State.OneTouch)
			{
				//Появился второй тач порсле первого. Либо первый тач как-то странно прервался.
				if (pressed)
					ProcessOneTouch(Vector3.zero, TouchPhase.Canceled);
			}
#endif

			if (count == 2 && (touchState == State.NoTouch || touchState == State.OneTouch || touchState == State.TwoTouch))
			{
				touchState = State.TwoTouch;
				var t0 = Input.touches[0];
				var t1 = Input.touches[1];

				float delta = (t0.position - t1.position).magnitude;
				if (lastDoubleTouchDelta == 0)
					lastDoubleTouchDelta = delta;

				if (InputAvailable)
				{
					Scroll?.Invoke(delta - lastDoubleTouchDelta);
					lastDoubleTouchDelta = delta;	
				}
			}
			else
			{
				lastDoubleTouchDelta = 0;
			}

			if (count == 0)
				touchState = State.NoTouch;

			if (count > 2)
				touchState = State.MultiTouch;
		}

		void ProcessOneTouch(Vector3 screenPointPos, TouchPhase touchPhase)
		{
			if (touchPhase == TouchPhase.Began)
			{
				isDrag                     = false;
				guiUnderPointerOnLastClick = guiHit.IsGuiUnderPointer;

				if (!guiUnderPointerOnLastClick)
				{
					clickPoint = screenPointPos;
					var ray = rayCastCamera.ScreenPointToRay(screenPointPos);

					RaycastHit hitInfo;
 
					if (Physics.Raycast(ray, out hitInfo,100, LayerMask.GetMask("Default")))
					{
						if (hitInfo.rigidbody)
							go = hitInfo.rigidbody.gameObject;
						else
							go = hitInfo.collider.gameObject;

						if (logClickHandlers)
							Debug.Log($"Began Click on go '{go.FullName()}'");

						targets = go.GetComponents<ITouchInputHandler>();
						if (InputAvailable && targets != null)
						{
							foreach (var target in targets)
							{
								if (target is IPointerUpDownHandler handler)
									handler.PointerDown();
							}
						}
					}

					startDragPoint = RayToWorldPoint(ray);

					clickTime    = Time.unscaledTime + clickDelay;
					pressed = true;
				}
			}

			if (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled)
			{
				if (!isDrag && touchPhase == TouchPhase.Ended)
				{
					// double click
					if (Time.unscaledTime > doubleClickTime)
					{
						if (Time.unscaledTime < clickTime)
						{
							//Это был первый клик   
							doubleClickTime = Time.unscaledTime + clickDelay;
							if (InputAvailable) //TODO вынести выше?
							{
								if (logClickHandlers)
									Debug.Log($"Ended Click on go '{(go?go.FullName():"")}'");

								if (targets != null)
								{
									foreach (var target in targets)
									{
										if (target is IClickHandler clickHandler)
										{
											if (logClickHandlers)
												Debug.Log($"Ended Click2 on go '{(target as MonoBehaviour).gameObject.FullName()}'");

											if (showClick)
											{
												Debug.Log(startDragPoint);
												pointerImage.transform.position = startDragPoint;
											}

											clickHandler.Click(new EventData() {startPoint = startDragPoint});
										}
									}
								}

								ClickOnGameObject?.Invoke(go);
							}
						}
					}
					else if (doubleClickTargets != null)
					{
						//второй клик
						if (targets == doubleClickTargets)
						{
							if (InputAvailable)
							{
								foreach (var doubleClickTarget in doubleClickTargets)
								{
									if (doubleClickTarget is IDoubleClickHandler doubleClickHandler)
										doubleClickHandler.DoubleClick();

									doubleClickTargets = null;
								}
							}

							doubleClickTime = 0;
						}
					}

					if (targets != null)
						doubleClickTime = Time.unscaledTime + clickDelay;
				}

				if (InputAvailable)
				{
					FingerTouchEnded?.Invoke();

					if (targets != null)
					{
						foreach (var target in targets)
						{
							if (target is IPointerUpDownHandler pointerUpDownHandler)
								pointerUpDownHandler.PointerUp();
						}
					}
				}

				doubleClickTargets = targets;

				pressed                    = false;
				isDrag                     = false;
				guiUnderPointerOnLastClick = false;

				targets = null;
			}

			if (pressed)
			{	
				if (!isDrag)
				{
					if (Vector3.Distance(screenPointPos, clickPoint) > minimalSwapDistance)
					{
						if (InputAvailable && targets!=null)
						{
							foreach (var target in targets)
							{
								if (target is IDragHandler dragHandler)
									dragHandler.BeginDrag();
							}
						}

						isDrag = true;

						//Заново пересчитываем точку начала свайпа, чтобы движение было без дергания
						var dragDir = (screenPointPos - clickPoint).normalized;
						//Но не больше некоторого колличества пикселей, иначе может быть значительное смешение пальца по дисплею, без соответствующей реации от игры
						startDragScreenPoint = clickPoint + dragDir * minimalSwapDistance; 
					}
				}

				if (isDrag)
				{
					Vector3 delta = startDragScreenPoint - screenPointPos;

					Vector3 resultDelta = new Vector3(
						delta.x * pixelToUnitX,
						delta.y * pixelToUnitY
					);
					
					if (InputAvailable)
						Drag?.Invoke(resultDelta);

					startDragScreenPoint = screenPointPos;
				}
			}
        }

		Vector3 RayToWorldPoint(Ray r)
		{
			Vector3 dir = r.direction.normalized;
			float   t   = -r.origin.y / dir.y;
			return r.GetPoint(t);
		}
	}
}