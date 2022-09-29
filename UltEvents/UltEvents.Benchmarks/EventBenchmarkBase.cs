// UltEvents // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UltEvents.Benchmarks
{
    /// <summary>[Editor-Only]
    /// A simple performance test which calls <see cref="Test"/> in <see cref="Awake"/> and logs the amount of time it
    /// takes.
    /// </summary>
    [AddComponentMenu("")]// Don't show in the Add Component menu. You need to drag this script onto a prefab manually.
    [HelpURL(UltEventUtils.APIDocumentationURL + "/Behchmarks/EventBenchmarkBase")]
    public abstract class EventBenchmarkBase : MonoBehaviour
    {
        /************************************************************************************************************************/

        private void Awake()
        {
            var start = EditorApplication.timeSinceStartup;

            Test();

            Debug.Log(EditorApplication.timeSinceStartup - start);
        }

        /************************************************************************************************************************/

        /// <summary>Called by <see cref="Awake"/>.</summary>
        protected abstract void Test();

        /************************************************************************************************************************/
    }
}

#endif