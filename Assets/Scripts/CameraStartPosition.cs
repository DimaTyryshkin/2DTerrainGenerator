using FieldGenerator;
using UnityEngine;

namespace Terraria
{
    public class CameraStartPosition : MonoBehaviour
    {
        [SerializeField] Camera thisCamera;
        [SerializeField] NoiseDrawer noiseDrawer;


        void Start()
        {
            transform.position = (Vector3)noiseDrawer.Center + Vector3.back * 10;
            thisCamera.orthographicSize = noiseDrawer.FieldSettings.Size.magnitude / 4f;
        }
    }
}