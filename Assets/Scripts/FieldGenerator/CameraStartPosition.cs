using FieldGenerator;
using UnityEngine;

namespace Terraria
{
    public class CameraStartPosition : MonoBehaviour
    {
        [SerializeField] Camera targetCamera;
        [SerializeField] Transform cameraOwnerTransform;
        [SerializeField] TerrainDrawer noiseDrawer;

        public void UpdatePosition()
        {
            float degToRad = Mathf.PI / 180f;
            cameraOwnerTransform.position = noiseDrawer.Center;
            float distance = (noiseDrawer.FieldSettings.Size.y / 2f) / (Mathf.Tan(degToRad * targetCamera.fieldOfView / 2f));
            targetCamera.transform.position = (Vector3)noiseDrawer.Center + Vector3.back * noiseDrawer.FieldSettings.Size.z / 2 - Vector3.forward * distance;
        }
    }
}