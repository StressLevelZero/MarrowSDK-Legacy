using System;

namespace SLZ.Marrow.Data
{
    [Serializable]
    public struct JointDriveExt
    {
        public float positionSpring;
        public float positionDamper;
        public float maximumForce;

        public JointDriveExt(UnityEngine.JointDrive drive)
        {
            positionSpring = drive.positionSpring;
            positionDamper = drive.positionDamper;
            maximumForce = drive.maximumForce;
        }

        public UnityEngine.JointDrive ToUnityJointDrive()
        {
            return new UnityEngine.JointDrive()
            {
                positionSpring = positionSpring,
                positionDamper = positionDamper,
                maximumForce = maximumForce
            };
        }
    }
}