using SiberianWellness.Common;
using UnityEngine;

namespace Terraria
{
    public class ManualCamera : MonoBehaviour
    {
        [SerializeField] Camera thisCamera;
        [SerializeField] float scrollSensitive;
        
        Vector2 startDragMousePoint;
        bool isDrag;
        
        void Update()
        {
            float scrollInput = Input.mouseScrollDelta.y;
            if (scrollInput != 0)
            {
                Vector2 beforeScrollPoint =  GetMousePoint();
                thisCamera.orthographicSize /= Mathf.Clamp(scrollInput * scrollSensitive, -1, 1) + 1;
                MoveMouseToPoint(beforeScrollPoint);
            }

            if (Input.GetMouseButtonDown(1))
            {
                startDragMousePoint = thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XY);
                isDrag = true;
            }

            if (isDrag)
            {
                MoveMouseToPoint(startDragMousePoint);
                
            }

            if (Input.GetMouseButtonUp(1))
                isDrag = false;
        }

        Vector2 GetMousePoint()
        {
           return  thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XY);
        }

        void MoveMouseToPoint(Vector2 oldPoint)
        {
            Vector2 actualPoint = thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XY);
            Vector2 delta = oldPoint - actualPoint;
            transform.position += (Vector3)delta;
        }
    }
}
