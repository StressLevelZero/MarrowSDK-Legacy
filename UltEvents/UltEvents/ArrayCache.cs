// UltEvents // Copyright 2021 Kybernetik //

using System;
using UnityEngine;

namespace UltEvents
{
    /// <summary>
    /// Stores arrays of various sizes so they can be reused without garbage collection.
    /// </summary>
    public static class ArrayCache<T>
    {
        /************************************************************************************************************************/

        [ThreadStatic]
        private static T[][] _Arrays;

        /************************************************************************************************************************/

        /// <summary>
        /// Get a cached array of the specified size for temporary use. The array must be used and discarded
        /// immediately as it may be reused by anything else that calls this method with the same `length`.
        /// </summary>
        public static T[] GetTempArray(int length)
        {
            if (_Arrays == null || _Arrays.Length <= length + 1)
            {
                var newSize = length < 16 ? 16 : Mathf.NextPowerOfTwo(length + 1);
                Array.Resize(ref _Arrays, newSize);
            }

            var array = _Arrays[length];
            if (array == null)
            {
                array = new T[length];
                _Arrays[length] = array;
            }

            return array;
        }

        /************************************************************************************************************************/
    }
}
