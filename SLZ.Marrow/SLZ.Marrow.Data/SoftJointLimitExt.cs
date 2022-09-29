using System;

namespace SLZ.Marrow.Data
{
    [Serializable]
    public struct SoftJointLimitExt
    {
        public float limit;
        public float bounciness;
        public float contactDistance;

        public SoftJointLimitExt(UnityEngine.SoftJointLimit softJointLimit)
        {
            limit = softJointLimit.limit;
            bounciness = softJointLimit.bounciness;
            contactDistance = softJointLimit.contactDistance;
        }

        public UnityEngine.SoftJointLimit ToUnitySoftJointLimit()
        {
            return new UnityEngine.SoftJointLimit()
            {
                limit = limit,
                bounciness = bounciness,
                contactDistance = contactDistance
            };
        }
    }
}
