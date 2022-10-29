using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SiberianWellness.InputSystem
{
    public class GuiHit : MonoBehaviour
    {
        [SerializeField]
        GraphicRaycaster[] graphicRaycasters;

        List<RaycastResult> objectUnderPointer = new List<RaycastResult>();
        bool                isGuiUnderPointer  = false;
        bool                valueCalculated    = true; //Пропускаем первый кадр

        public bool IsGuiUnderPointer
        {
            get
            {
                HitGraphic();
                return isGuiUnderPointer;
            }
        }

        void HitGraphic()
        {
            if (valueCalculated == false)
            {
                isGuiUnderPointer = false;
                valueCalculated   = true;

                PointerEventData pointEvent = new PointerEventData(null);
                pointEvent.position = Input.mousePosition;

                foreach (var raycaster in graphicRaycasters)
                {
                    objectUnderPointer.Clear();
                    raycaster.Raycast(pointEvent, objectUnderPointer);
                    isGuiUnderPointer = isGuiUnderPointer || objectUnderPointer.Count > 0;
                }
            }
        }

        void LateUpdate()
        {
            valueCalculated = false;
        }
    }
}