using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;


namespace MicroMachines
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField, IsntNull] Transform offsetMarker;
        [SerializeField, IsntNull] Camera thisCamera;
        [SerializeField, IsntNull] GameObject viewCenterMarker;
        [SerializeField, IsntNull] AnimationCurve speedToScaleCurve;
        [SerializeField] float scaleFactor = 1;
        [SerializeField] float moveFactor = 1;
        [SerializeField] float speedLerpFactor = 0.5f;
        [SerializeField] float mainMoveLerpFactor = 8;


        Vector3 offset;
        float startCameraSize;
        Speedometer carSpeedometer;
        Transform target;
        float speedFactor;

        public float SpeedFactor => speedFactor;

        public void Init()
        {
            speedFactor = 0;
            startCameraSize = thisCamera.orthographicSize;
            offset = offsetMarker.position - transform.position;
            offsetMarker.gameObject.SetActive(false);
            viewCenterMarker.SetActive(false);
        }

        public void SetTarget(Transform target, Transform carTransform)
        {
            Assert.IsNotNull(target);
            this.target = target;
            carSpeedometer = Speedometer.Get(carTransform);
        }

        void LateUpdate()
        {
            Vector3 resultOffset = offset;
            float newSpeedFactor = speedToScaleCurve.Evaluate(carSpeedometer.SpeedKm);
            speedFactor = Mathf.Lerp(speedFactor, newSpeedFactor, speedLerpFactor * Time.deltaTime);


            if (thisCamera.orthographic)
                thisCamera.orthographicSize = startCameraSize * (1 +  speedFactor  * scaleFactor);
            else
                resultOffset *= speedFactor;

            Vector3 velocity = carSpeedometer.Velocity;
            velocity.y = 0;
            resultOffset -=velocity * (speedFactor  * moveFactor);

            transform.position = Vector3.Lerp(transform.position, target.position - resultOffset, mainMoveLerpFactor * Time.deltaTime);
        }
    }
}