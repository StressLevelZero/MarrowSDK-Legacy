namespace SLZ.Marrow.Data
{
    [System.Serializable]
    public struct SoftJointLimitSpringExt
    {
        public float spring;
        public float damper;

        public SoftJointLimitSpringExt(UnityEngine.SoftJointLimitSpring softJointLimitSpring)
        {
            spring = softJointLimitSpring.spring;
            damper = softJointLimitSpring.damper;
        }

        public UnityEngine.SoftJointLimitSpring ToUnitySoftJointLimitSpring()
        {
            return new UnityEngine.SoftJointLimitSpring()
            {
                spring = spring,
                damper = damper
            };
        }
    }
}