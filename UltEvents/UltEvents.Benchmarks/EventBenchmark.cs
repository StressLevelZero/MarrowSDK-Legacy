// UltEvents // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UltEvents.Benchmarks
{
    /// <summary>[Editor-Only]
    /// A simple performance test that loads and instantiates a prefab to test how long it takes.
    /// </summary>
    [AddComponentMenu("")]// Don't show in the Add Component menu. You need to drag this script onto a prefab manually.
    [HelpURL(UltEventUtils.APIDocumentationURL + "/Behchmarks/EventBenchmark")]
    public sealed class EventBenchmark : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private string _PrefabPath;

        /************************************************************************************************************************/

        private void Update()
        {
            // Wait a bit to avoid mixing this performance test in with the engine startup processes.
            if (Time.timeSinceLevelLoad < 1)
                return;

            // Sleep to make this frame show up easily in the Unity Profiler.
            System.Threading.Thread.Sleep(100);

            var start = EditorApplication.timeSinceStartup;

            // Include the costs of loading and instantiating the prefab as well as the actual event invocation.
            var prefab = Resources.Load<GameObject>(_PrefabPath);
            Instantiate(prefab);

            Debug.Log(EditorApplication.timeSinceStartup - start);

            enabled = false;
        }

        /************************************************************************************************************************/
    }
}

#endif