// UltEvents // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

#if UNITY_EDITOR

using UnityEngine;

namespace UltEvents.Benchmarks
{
    /// <summary>[Editor-Only]
    /// A simple performance test which invokes a <see cref="UltEvent"/>.
    /// </summary>
    [AddComponentMenu("")]// Don't show in the Add Component menu. You need to drag this script onto a prefab manually.
    [HelpURL(UltEventUtils.APIDocumentationURL + "/Behchmarks/UltEventBenchmark")]
    public sealed class UltEventBenchmark : EventBenchmarkBase
    {
        /************************************************************************************************************************/

        [SerializeField]
        private UltEvent _Event;

        /************************************************************************************************************************/

        /// <summary>Called by <see cref="Awake"/>.</summary>
        protected override void Test()
        {
            _Event.Invoke();
        }

        /************************************************************************************************************************/
    }
}

#endif