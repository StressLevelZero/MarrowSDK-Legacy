using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    [System.Serializable]
    public struct SpringContactDrive
    {
        [SerializeField]
        public float positionSpring;
        [SerializeField]
        public float positionDamper;
        [SerializeField]
        public float maximumForce;

        public SpringContactDrive(float positionSpring, float positionDamper, float maximumForce)
        {
            this.positionSpring = positionSpring;
            this.positionDamper = positionDamper;
            this.maximumForce = maximumForce;
        }
    }
}