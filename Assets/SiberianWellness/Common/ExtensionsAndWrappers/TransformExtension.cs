using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace SiberianWellness.Common
{
    public static class TransformExtension
    {
        public static Rect GetWorldRect(this RectTransform t)
        {
            return t.TransformRect(t.rect);
        }

        /// <summary>
        ///   Transforms rect from local space to world space.
        /// </summary>
        public static Rect TransformRect(this Transform t, Rect rect)
        {
            Vector2 start = t.TransformPoint(rect.position);
            Vector2 end   = t.TransformPoint(rect.max);

            Vector2 size = end - start;

            return new Rect(start, size);
        }

        public static string PrintChildren(this Transform transform)
        {
            string s = "";
            int    n = 0;
            foreach (Transform t in transform)
            {
                s += $"({n}) {t.gameObject.name}" + Environment.NewLine;
                n++;
            }

            return s;
        }

        public static T AddComponentAsChild<T>(this Transform parent, string name, bool localScaleToOne = true)
        {
            GameObject newGo = parent.AttachChild(new GameObject(name, typeof(T)));
            return newGo.GetComponent<T>();
        }

        public static List<T> GetComponentsInFirstChild<T>(this Transform t)
            where T : Component
        {
            List<T> result = new List<T>();

            for (int i = 0; i < t.childCount; i++)
            {
                var component = t.GetChild(i).GetComponent<T>();
                if (component)
                    result.Add(component);
            }

            return result;
        }

        public static GameObject InstantiateAsChild(this Transform parent, string name, GameObject childPrefab, bool localScaleToOne = true)
        {
            var go = parent.InstantiateAsChild(childPrefab, localScaleToOne);
            go.name = name;
            return go;
        }

        public static T InstantiateAsChild<T>(this Transform parent, T childPrefab, bool localScaleToOne = true) where T : MonoBehaviour
        {
            var childGo = GameObject.Instantiate(childPrefab.gameObject);
            parent.AttachChild(childGo.gameObject, localScaleToOne);
            return childGo.GetComponent<T>();
        }

        public static GameObject InstantiateAsChild(this Transform parent, GameObject childPrefab, bool localScaleToOne = true)
        {
            var childGo = GameObject.Instantiate(childPrefab);
            return parent.AttachChild(childGo, localScaleToOne);
        }

        public static GameObject InstantiateAsChildAndSaveLocalTransform(this Transform parent, GameObject childPrefab)
        {
            var       childGo = Object.Instantiate(childPrefab);
            Transform child   = childGo.transform;
            child.SetParent(parent);

            child.localScale    = childPrefab.transform.localScale;
            child.localRotation = childPrefab.transform.localRotation;
            child.localPosition = childPrefab.transform.localPosition;
            return childGo;
        }

        public static GameObject AttachChild(this Transform parent, GameObject childGo, bool localScaleToOne = true)
        {
            Transform child = childGo.transform;
            child.SetParent(parent);
            if (localScaleToOne)
                child.localScale = Vector3.one;

            child.localPosition = Vector3.zero;

            return childGo;
        }

        public static void DestroyChilds(this Transform transform)
        {
            int n = 100000;
            while (transform.childCount > 0)
            {
                n--;
                if (n == 0)
                {
                    Debug.LogError("Infinity loop");
                    break;
                }
                
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Undo.DestroyObjectImmediate(transform.GetChild(0).gameObject);
                    continue;
                }
#endif
                Object.DestroyImmediate(transform.GetChild(0).gameObject);
               
            }
        }

        public static void ForAllHierarchy(this Transform transform, UnityAction<Transform> action)
        {
            action(transform);

            for (int n = 0; n < transform.childCount; n++)
            {
                transform.GetChild(n).ForAllHierarchy(action);
            }
        }

        [CanBeNull]
        public static Transform Find(this Transform transform, Predicate<Transform> preficate)
        {
            for (int n = 0; n < transform.childCount; n++)
            {
                var child = transform.GetChild(n);

                if (preficate(child))
                {
                    return child;
                }
                else
                {
                    var result = child.Find(preficate);
                    if (result)
                        return result;
                }
            } 

            return null;
        }

        [CanBeNull]
        public static T Find<T>(this Transform transform, Predicate<T> preficate) where T : MonoBehaviour
        {
            for (int n = 0; n < transform.childCount; n++)
            {
                var child = transform.GetChild(n);

                var c = child.GetComponent<T>();
                if (c && preficate(c))
                    return c;
            }

            return null;
        }   
    }

    public static class TransformSetterWrapper
    {
        public static void SetPos(this Transform t, Vector3 pos)
        {
            t.position = pos;
        }

        public static void SetLocalPos(this Transform t, Vector3 pos)
        {
            t.localPosition = pos;
        }

        public static void SetLocalScale(this Transform t, Vector3 scale)
        {
            t.localScale = scale;
        }
    }
}