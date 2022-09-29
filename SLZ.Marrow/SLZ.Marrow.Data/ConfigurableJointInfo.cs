using System;
using SLZ.Marrow.Utilities;
using UnityEngine;

namespace SLZ.Marrow.Data
{
    [Serializable]
    public class ConfigurableJointInfo
    {
        public Rigidbody connectedBody;
        public Vector3 axis;
        public Vector3 secondaryAxis;
        public Vector3 anchor;
        public Vector3 connectedAnchor;
        public bool autoConfigureConnectedAnchor;
        public float breakForce;
        public float breakTorque;
        public bool enableCollision;
        public bool enablePreprocessing;
        public float massScale;
        public float connectedMassScale;
        public Quaternion startRotation;
        public float projectionAngle;
        public float projectionDistance;
        public JointProjectionModeExt projectionModeExt;
        public JointDriveExt slerpDriveExt;
        public JointDriveExt angularYZDriveExt;
        public JointDriveExt angularXDriveExt;
        public RotationDriveMode rotationDriveMode;
        public Vector3 targetAngularVelocity;
        public Quaternion targetRotation;
        public JointDriveExt zDriveExt;
        public JointDriveExt yDriveExt;
        public JointDriveExt xDriveExt;
        public Vector3 targetVelocity;
        public Vector3 targetPosition;
        public SoftJointLimitExt angularZLimitExt;
        public SoftJointLimitExt angularYLimitExt;
        public SoftJointLimitExt highAngularXLimitExt;
        public SoftJointLimitExt lowAngularXLimitExt;
        public SoftJointLimitExt linearLimitExt;
        public SoftJointLimitSpringExt angularYZLimitSpringExt;
        public SoftJointLimitSpringExt angularXLimitSpringExt;
        public SoftJointLimitSpringExt linearLimitSpringExt;
        public ConfigurableJointMotion angularZMotion;
        public ConfigurableJointMotion angularYMotion;
        public ConfigurableJointMotion angularXMotion;
        public ConfigurableJointMotion zMotion;
        public ConfigurableJointMotion yMotion;
        public ConfigurableJointMotion xMotion;
        public bool configuredInWorldSpace;
        public bool swapBodies;

        public ConfigurableJointInfo()
        {
            connectedBody = null;
            anchor = Vector3.zero;
            axis = Vector3.right;
            autoConfigureConnectedAnchor = true;
            connectedAnchor = Vector3.zero;
            secondaryAxis = Vector3.up;

            zMotion = ConfigurableJointMotion.Free;
            yMotion = ConfigurableJointMotion.Free;
            xMotion = ConfigurableJointMotion.Free;

            angularZMotion = ConfigurableJointMotion.Free;
            angularYMotion = ConfigurableJointMotion.Free;
            angularXMotion = ConfigurableJointMotion.Free;

            linearLimitSpringExt.spring = 0;
            linearLimitSpringExt.damper = 0;

            linearLimitExt.limit = 0;
            linearLimitExt.bounciness = 0;
            linearLimitExt.contactDistance = 0;

            angularXLimitSpringExt.spring = 0;
            angularXLimitSpringExt.damper = 0;

            lowAngularXLimitExt.limit = 0;
            lowAngularXLimitExt.bounciness = 0;
            lowAngularXLimitExt.contactDistance = 0;

            highAngularXLimitExt.limit = 0;
            highAngularXLimitExt.bounciness = 0;
            highAngularXLimitExt.contactDistance = 0;

            angularYZLimitSpringExt.spring = 0;
            angularYZLimitSpringExt.damper = 0;

            angularYLimitExt.limit = 0;
            angularYLimitExt.bounciness = 0;
            angularYLimitExt.contactDistance = 0;

            angularZLimitExt.limit = 0;
            angularZLimitExt.bounciness = 0;
            angularZLimitExt.contactDistance = 0;

            targetPosition = Vector3.zero;
            targetVelocity = Vector3.zero;

            xDriveExt.positionSpring = 0;
            xDriveExt.positionDamper = 0;
            xDriveExt.maximumForce = float.MaxValue;

            yDriveExt.positionSpring = 0;
            yDriveExt.positionDamper = 0;
            yDriveExt.maximumForce = float.MaxValue;

            zDriveExt.positionSpring = 0;
            zDriveExt.positionDamper = 0;
            zDriveExt.maximumForce = float.MaxValue;

            targetRotation = Quaternion.identity;

            targetAngularVelocity = Vector3.zero;

            rotationDriveMode = RotationDriveMode.XYAndZ;

            angularXDriveExt.positionSpring = 0;
            angularXDriveExt.positionDamper = 0;
            angularXDriveExt.maximumForce = float.MaxValue;

            angularYZDriveExt.positionSpring = 0;
            angularYZDriveExt.positionDamper = 0;
            angularYZDriveExt.maximumForce = float.MaxValue;

            slerpDriveExt.positionSpring = 0;
            slerpDriveExt.positionDamper = 0;
            slerpDriveExt.maximumForce = float.MaxValue;

            projectionModeExt = JointProjectionModeExt.None;
            projectionDistance = 0.1f;
            projectionAngle = 180f;
            configuredInWorldSpace = false;
            swapBodies = false;
            breakForce = float.MaxValue;
            breakTorque = float.MaxValue;
            enableCollision = false;
            enablePreprocessing = true;
            massScale = 1;
            connectedMassScale = 1;
        }

        public void Apply(ConfigurableJoint joint)
        {
            joint.axis = axis;
            joint.secondaryAxis = secondaryAxis;

            joint.anchor = anchor;
            joint.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
            joint.connectedAnchor = connectedAnchor;
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;
            joint.enableCollision = enableCollision;
            joint.enablePreprocessing = enablePreprocessing;
            joint.massScale = massScale;
            joint.connectedMassScale = connectedMassScale;

            joint.projectionAngle = projectionAngle;
            joint.projectionDistance = projectionDistance;
            joint.projectionMode = (UnityEngine.JointProjectionMode)projectionModeExt;
            joint.slerpDrive = slerpDriveExt.ToUnityJointDrive();
            joint.angularYZDrive = angularYZDriveExt.ToUnityJointDrive();
            joint.angularXDrive = angularXDriveExt.ToUnityJointDrive();
            joint.rotationDriveMode = rotationDriveMode;
            joint.targetAngularVelocity = targetAngularVelocity;
            joint.targetRotation = targetRotation;
            joint.zDrive = zDriveExt.ToUnityJointDrive();
            joint.yDrive = yDriveExt.ToUnityJointDrive();
            joint.xDrive = xDriveExt.ToUnityJointDrive();
            joint.targetVelocity = targetVelocity;
            joint.targetPosition = targetPosition;
            joint.angularZLimit = angularZLimitExt.ToUnitySoftJointLimit();
            joint.angularYLimit = angularYLimitExt.ToUnitySoftJointLimit();
            joint.highAngularXLimit = highAngularXLimitExt.ToUnitySoftJointLimit();
            joint.lowAngularXLimit = lowAngularXLimitExt.ToUnitySoftJointLimit();
            joint.linearLimit = linearLimitExt.ToUnitySoftJointLimit();
            joint.angularYZLimitSpring = angularYZLimitSpringExt.ToUnitySoftJointLimitSpring();
            joint.angularXLimitSpring = angularXLimitSpringExt.ToUnitySoftJointLimitSpring();
            joint.linearLimitSpring = linearLimitSpringExt.ToUnitySoftJointLimitSpring();
            joint.angularZMotion = angularZMotion;
            joint.angularYMotion = angularYMotion;
            joint.angularXMotion = angularXMotion;
            joint.zMotion = zMotion;
            joint.yMotion = yMotion;
            joint.xMotion = xMotion;
            joint.configuredInWorldSpace = configuredInWorldSpace;
            joint.swapBodies = swapBodies;

            var actorACurRot = joint.transform.rotation;
            var actorAT = joint.transform;
            var actorBT = (connectedBody == null) ? SimpleTransform.Identity : new SimpleTransform(connectedBody.transform);
            var actorStartRotation = (swapBodies) ? startRotation : Quaternion.Inverse(startRotation);

            actorAT.rotation = actorBT.rotation * actorStartRotation;

            joint.connectedBody = connectedBody;

            actorAT.rotation = actorACurRot;
        }

        public void Snapshot(ConfigurableJoint joint)
        {
            axis = joint.axis;
            secondaryAxis = joint.secondaryAxis;

            anchor = joint.anchor;
            connectedAnchor = joint.connectedAnchor;
            autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            breakForce = joint.breakForce;
            breakTorque = joint.breakTorque;
            enableCollision = joint.enableCollision;
            enablePreprocessing = joint.enablePreprocessing;
            massScale = joint.massScale;
            connectedMassScale = joint.connectedMassScale;

            projectionAngle = joint.projectionAngle;
            projectionDistance = joint.projectionDistance;
            projectionModeExt = (JointProjectionModeExt)joint.projectionMode;
            slerpDriveExt = new JointDriveExt(joint.slerpDrive);
            angularYZDriveExt = new JointDriveExt(joint.angularYZDrive);
            angularXDriveExt = new JointDriveExt(joint.angularXDrive);
            rotationDriveMode = joint.rotationDriveMode;
            targetAngularVelocity = joint.targetAngularVelocity;
            targetRotation = joint.targetRotation;
            zDriveExt = new JointDriveExt(joint.zDrive);
            yDriveExt = new JointDriveExt(joint.yDrive);
            xDriveExt = new JointDriveExt(joint.xDrive);
            targetVelocity = joint.targetVelocity;
            targetPosition = joint.targetPosition;
            angularZLimitExt = new SoftJointLimitExt(joint.angularZLimit);
            angularYLimitExt = new SoftJointLimitExt(joint.angularYLimit);
            highAngularXLimitExt = new SoftJointLimitExt(joint.highAngularXLimit);
            lowAngularXLimitExt = new SoftJointLimitExt(joint.lowAngularXLimit);
            linearLimitExt = new SoftJointLimitExt(joint.linearLimit);
            angularYZLimitSpringExt = new SoftJointLimitSpringExt(joint.angularYZLimitSpring);
            angularXLimitSpringExt = new SoftJointLimitSpringExt(joint.angularXLimitSpring);
            linearLimitSpringExt = new SoftJointLimitSpringExt(joint.linearLimitSpring);
            angularZMotion = joint.angularZMotion;
            angularYMotion = joint.angularYMotion;
            angularXMotion = joint.angularXMotion;
            zMotion = joint.zMotion;
            yMotion = joint.yMotion;
            xMotion = joint.xMotion;
            configuredInWorldSpace = joint.configuredInWorldSpace;
            swapBodies = joint.swapBodies;

            connectedBody = joint.connectedBody;
        }

        public void CalculateStartRotation(ConfigurableJoint joint)
        {
            var actorARot = joint.transform.rotation;
            var actorBRot = (connectedBody != null) ? connectedBody.transform.rotation : Quaternion.identity;

            if (swapBodies) (actorARot, actorBRot) = (actorBRot, actorARot);

            startRotation = Quaternion.Inverse(actorARot) * actorBRot;
        }

    }
}

