using UnityEngine;

namespace SiberianWellness
{
    public class AutoKillPooled : MonoBehaviour
    {
        public float time = 2.0f;

        PooledObject pooledObject;
        float accTime;
        
        void OnEnable()
        {
            accTime = 0.0f;
        }
        
        void Start()
        {
            pooledObject = GetComponent<PooledObject>();
        }
        
        void Update()
        {
            accTime += Time.deltaTime;
            if (accTime >= time)
            {
                pooledObject.ReturnToPool();
            }
        }
    }
}