using System;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;


namespace MicroMachines
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform offsetMarker;
        [SerializeField] GameObject viewCenterMarker; 
        [SerializeField] float k;

        Vector3 offset;
        Transform target;

        public void Init()
        {
            offset = offsetMarker.position - transform.position;
            offsetMarker.gameObject.SetActive(false);
            viewCenterMarker.SetActive(false);
        }

        public void SetTarget(Transform target)
        {
            Assert.IsNotNull(target);
            this.target = target;
        }

        void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position,  target.position - offset, k * Time.deltaTime);
        }
    }
}