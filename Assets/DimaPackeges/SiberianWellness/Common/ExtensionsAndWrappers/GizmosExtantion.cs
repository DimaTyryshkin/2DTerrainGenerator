using UnityEngine;

namespace SiberianWellness.Common
{
    public static class GizmosExtension
    {
       static  Quaternion xyRight = Quaternion.Euler(0, 0, +20);
       static Quaternion xyLeft = Quaternion.Euler(0, 0, -20);
       
       static  Quaternion xzRight = Quaternion.Euler(0, +20, 0);
       static Quaternion xzLeft = Quaternion.Euler(0, -20, 0);

        public static void DrawArrowXY(Vector3 from, Vector3 to, float size = 0.25f)
        {
            DrawArrow(from, to, size, xyRight, xyLeft);
        }
        
        public static void DrawArrowXZ(Vector3 from, Vector3 to, float size = 0.25f)
        {
            DrawArrow(from, to, size, xzRight, xzLeft);
        }
        
        static void DrawArrow(Vector3 from, Vector3 to, float size , Quaternion right, Quaternion left)
        {
            Vector3 direction = to - from;
            Gizmos.DrawRay(from, direction);
 
            Vector3 arrow = direction * (-size);
            Gizmos.DrawRay(to, right * arrow);
            Gizmos.DrawRay(to, left * arrow);
        }
         
        public static void DrawRect(Rect rect)
        {
            Vector2 p1 = rect.min;
            Vector2 p2 = p1 + new Vector2(0, rect.height);
            Vector2 p3 = rect.max;
            Vector2 p4 = p1 + new Vector2(rect.width, 0);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p1, p4);
        }  
        
        public static void DrawBounds(Bounds bounds)
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}