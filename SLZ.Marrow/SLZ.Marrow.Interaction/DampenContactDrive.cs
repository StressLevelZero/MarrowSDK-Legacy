using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    [System.Serializable]
    public struct DampenContactDrive
    {
        [SerializeField]
        public float positionDamper;
        [SerializeField]
        public float maximumForce;

        public DampenContactDrive(float positionDamper, float maximumForce)
        {
            this.positionDamper = positionDamper;
            this.maximumForce = maximumForce;
        }
    }
}