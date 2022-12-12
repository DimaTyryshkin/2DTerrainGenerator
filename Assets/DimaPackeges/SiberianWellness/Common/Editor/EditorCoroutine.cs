using System.Collections;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SiberianWellness.Common
{
    public class EditorCoroutine
    {
        /// <summary>
        /// Use only "yield return null" to skip frame in editor
        /// </summary> 
        /// <param name="routineContainer">corotine still live while <paramref name="routineContainer"/> still exist </param>
        /// <returns></returns>
        public static EditorCoroutine Start(IEnumerator routine, UnityEngine.Object routineContainer = null)
        { 
            EditorCoroutine coroutine = new EditorCoroutine(routine, routineContainer);
            coroutine.Start();
            return coroutine;
        }

        CustomCoroutineHandler routineHandler;
        Object      routineContainer;
        bool        routineContainerRequired; 

        EditorCoroutine(IEnumerator routine, Object routineContainer = null)
        {
            this.routineContainer    = routineContainer;
            routineContainerRequired = routineContainer;
            
            routineHandler = new CustomCoroutineHandler(routine); 
        }

        void Start()
        {
            EditorApplication.update += Update;
        }

        public void Stop()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (!routineContainerRequired || routineContainer)
            {
                if (!routineHandler.MoveNext())
                {
                    Stop();
                }
            }
            else
            {
                Stop();
            }
        }
    }
}