using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR

#endif

namespace SLZ.Marrow.Interaction
{
    [RequireComponent(typeof(SplineContainer))]
    public class PolyLine : MonoBehaviour
    {

        [SerializeField]
        public PolyLineData lineData;

        [SerializeField]
        private SplineContainer _splineContainer;
        public SplineContainer SplineContainer => _splineContainer;


        public static int Mod(int i, int max)
        {
            int r = i % max;
            return r < 0 ? r + max : r;
        }

        private void Reset()
        {
            _splineContainer = GetComponent<SplineContainer>();
        }
    }
}