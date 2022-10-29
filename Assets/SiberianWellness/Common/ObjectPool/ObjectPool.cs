using System.Collections;
using System.Collections.Generic;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;

namespace SiberianWellness
{
    public static class MonoBehaviourPoolExtantion
    {
        public static void ReturnToPool(this GameObject thisGo)
        {
            thisGo.GetComponent<PooledObject>().ReturnToPool();
        } 
        
        public static void ReturnToPool(this GameObject thisGo, float time)
        {
            thisGo.GetComponent<PooledObject>().ReturnToPool(time);
        } 
    }

    public class ObjectPool : MonoBehaviour
    {
        [SerializeField,  IsntNull]
        GameObject prefab;
        
        [SerializeField]
        int initialSize;

        readonly Stack<GameObject> instances = new Stack<GameObject>();
 
        /// <summary>
        /// Initializes the object pool.
        /// </summary>
        public void Initialize()
        {
            for (var i = 0; i < initialSize; i++)
            {
                var obj = CreateInstance();
                obj.SetActive(false);
                instances.Push(obj);
            }
        }

        public void SetPrefab(GameObject prefab)
        {
            Assert.IsNull(this.prefab);
            this.prefab = prefab;
        }

        public bool IsPoolForPrefab(GameObject prefab)
        {
            return this.prefab == prefab;
        }

        /// <summary>
        /// Returns a new object from the pool.
        /// </summary>
        /// <returns>A new object from the pool.</returns>
        public GameObject GetObject()
        {
            var obj = instances.Count > 0 ? instances.Pop() : CreateInstance();
            obj.SetActive(true);
            return obj;
        }
 
        public void ReturnObject(PooledObject pooledObject, float time)
        {
            StartCoroutine(WaitAndReturnObject(pooledObject, time));
        }

        IEnumerator WaitAndReturnObject(PooledObject pooledObject, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            ReturnObject(pooledObject);
        }
        
        public void ReturnObject(PooledObject pooledObject)
        { 
            Assert.IsNotNull(pooledObject);
            Assert.IsTrue(pooledObject.pool == this);

            GameObject go = pooledObject.gameObject;
            go.SetActive(false);
            if (!instances.Contains(go))
            {
                instances.Push(go);
            }
        }
         
        /// <summary>
        /// Resets the object pool to its initial state.
        /// </summary>
        public void Reset()
        {
            var objectsToReturn = new List<GameObject>();
            foreach (var instance in transform.GetComponentsInChildren<PooledObject>())
            {
                if (instance.gameObject.activeSelf)
                {
                    objectsToReturn.Add(instance.gameObject);
                }
            }
            
            foreach (GameObject instance in objectsToReturn)
            {
                instance.ReturnToPool();
            }
        }

        /// <summary>
        /// Creates a new instance of the pooled object type.
        /// </summary>
        /// <returns>A new instance of the pooled object type.</returns>
        GameObject CreateInstance()
        {
            var obj = Instantiate(prefab);
            var pooledObject = obj.AddComponent<PooledObject>();
            pooledObject.pool = this;
            obj.transform.SetParent(transform);
            return obj;
        }

       
    }
}

