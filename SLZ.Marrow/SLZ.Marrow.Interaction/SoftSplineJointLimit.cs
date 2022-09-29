using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    [System.Serializable]
    public struct SoftSplineJointLimit
    {
        [SerializeField]
        public float limit;
        [SerializeField]
        public float bounciness;
        [SerializeField]
        public float contactDistance;

        public SoftSplineJointLimit(float limit, float bounciness, float contactDistance)
        {
            this.limit = limit;
            this.bounciness = bounciness;
            this.contactDistance = contactDistance;
        }
    }
}