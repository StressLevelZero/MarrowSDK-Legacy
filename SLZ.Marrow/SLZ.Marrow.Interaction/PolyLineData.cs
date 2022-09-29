using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    [System.Serializable]
    public class PolyLineData : ScriptableObject
    {
        [System.Serializable]
        public enum SegmentResolution
        {
            Millimeter = 0,
            Centimeter = 1,
            Decimeter = 2,
            Meter = 3,
        }

        [SerializeField]
        public SegmentResolution segmentResolution;
        [SerializeField]
        public PolyLineVert[] polyVerts;
        [SerializeField]
        public float totalDistance;

        public float SegmentDistance()
        {
            switch (segmentResolution)
            {
                case SegmentResolution.Millimeter:
                    return 0.001f;
                case SegmentResolution.Centimeter:
                    return 0.01f;
                case SegmentResolution.Decimeter:
                    return 0.1f;
                case SegmentResolution.Meter:
                    return 1f;
                default:
                    return 0.001f;
            }
        }
    }
}