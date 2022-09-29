using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using SLZ.Marrow.Utilities;

namespace SLZ.Marrow.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class SplineJoint : MonoBehaviour
    {

        [Header("Configuration")]
        [SerializeField]
        private ContactCount _contactCount = ContactCount.One;
        [SerializeField]
        private Vector3 _axis = Vector3.forward;
        [SerializeField]
        private Vector3 _secondaryAxis = Vector3.up;
        [SerializeField]
        private Vector2 _size = Vector2.zero;
        [SerializeField]
        private Vector3 _anchor = Vector3.zero;

        [Space(15)]
        [SerializeField]
        private float _linearXDriveSpeed = 0;
        [SerializeField]
        private DampenContactDrive _linearXDrive = new(0, float.MaxValue);

        [Space(15)]
        [SerializeField, Tooltip("Sets rotational twist motion on the configured axis")]
        private ConfigurableJointMotion _angularXMotion = ConfigurableJointMotion.Locked;
        [SerializeField, Range(-177, 177)]
        private float _angularXMinLimit = 0;
        [SerializeField, Range(-177, 177)]
        private float _angularXMaxLimit = 0;
        [SerializeField]
        private float _angularXDriveSpeed = 0;
        [SerializeField]
        private SpringContactDrive _angularXDrive = new(0, 0, float.MaxValue);

        [Space(15)]
        [SerializeField]
        private SplineJointMotion _linearYZMotion = SplineJointMotion.Locked;
        [SerializeField]
        private SoftSplineJointLimit _linearYZLimit = new(0, 0, 0);
        [SerializeField]
        private SpringContactDrive _linearYZDrive = new(0, 0, float.MaxValue);

        [Space(15)]
        [SerializeField, Tooltip("Sets rotational swing motion on the configured axis")]
        private ConfigurableJointMotion _angularYZMotion = ConfigurableJointMotion.Locked;
        [SerializeField, Range(0, 177)]
        private float _angularYZLimit = 0;
        [SerializeField]
        private SpringContactDrive _angularYZDrive = new(0, 0, float.MaxValue);

        [Header("References")]
        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private PolyLine polyLine;

        private Quaternion AnchorRotation =>
            SynthesizeRotation(_axis.normalized, _secondaryAxis.normalized);

        private Vector3 Size => new(_size.y, 0, _size.x);
        private Vector3 FrontContact => new(0.5f * _size.y, 0, 0);
        private Vector3 RightContact => new(0, 0, 0.5f * _size.x);

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }


        public void SetAngularXDriveSpeed(float speed)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetAngularXDriveSpeed(float)");

            throw new System.NotImplementedException();
        }
        public void SetAngularXDrivePositionSpring(float springForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetAngularXDrivePositionSpring(float)");

            throw new System.NotImplementedException();
        }
        public void SetAngularXDrivePositionDamper(float damperForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetAngularXDrivePositionDamper(float)");

            throw new System.NotImplementedException();
        }
        public void SetAngularXDriveMaxForce(float maxForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetAngularXDriveMaxForce(float)");

            throw new System.NotImplementedException();
        }


        public void SetYZDrivePositionSpring(float springForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetYZDrivePositionSpring(float)");

            throw new System.NotImplementedException();
        }
        public void SetYZDrivePositionDamper(float damperForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetYZDrivePositionDamper(float)");

            throw new System.NotImplementedException();
        }
        public void SetYZDriveMaxForce(float maxForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetYZDriveMaxForce(float)");

            throw new System.NotImplementedException();
        }


        public void SetLinearXDriveSpeed(float speed)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetLinearXDriveSpeed(float)");

            throw new System.NotImplementedException();
        }
        public void SetLinearXDrivePositionDamper(float damperForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetLinearXDrivePositionDamper(float)");

            throw new System.NotImplementedException();
        }
        public void SetLinearXDriveMaxForce(float maxForce)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.SplineJoint.SetLinearXDriveMaxForce(float)");

            throw new System.NotImplementedException();
        }

        private static quaternion SynthesizeRotation(float3 right, float3 up)
        {

            var z = math.cross(right, up);
            var y = math.cross(z, right);

            return quaternion.LookRotationSafe(z, y);
        }

        public SimpleTransform GetContactOriginInWorld()
        {
            return new SimpleTransform(
                transform.position + transform.rotation * _anchor,
                transform.rotation * AnchorRotation);
        }

        public void PlaceOnSpline()
        {
            switch (_contactCount)
            {
                case ContactCount.One:
                    {
                        var targetPos = _rigidbody.transform.TransformPoint(_anchor);
                        targetPos = polyLine.transform.InverseTransformPoint(targetPos);

                        SplineUtility.GetNearestPoint(polyLine.SplineContainer.Spline, targetPos, out var _, out var splineTime);
                        polyLine.SplineContainer.Evaluate(splineTime, out var pos, out var tan, out var up);

                        var direction = math.normalize(tan);
                        var rot = SynthesizeRotation(direction, up);
                        var orientation = SynthesizeRotation(_axis, _secondaryAxis);

                        pos += math.rotate(rot, math.rotate(math.inverse(orientation), -_anchor));
                        rot = math.mul(rot, math.inverse(orientation));

                        _rigidbody.transform.SetPositionAndRotation(pos, rot);

                        break;
                    }
                case ContactCount.Two:
                case ContactCount.Four:
                    {
                        var orientation = SynthesizeRotation(_axis, _secondaryAxis);

                        var backAnchor = _anchor + (Vector3)math.rotate(orientation, -FrontContact);
                        var backPoint = _rigidbody.transform.TransformPoint(backAnchor);
                        backPoint = polyLine.transform.InverseTransformPoint(backPoint);

                        SplineUtility.GetNearestPoint(polyLine.SplineContainer.Spline, backPoint, out var _, out var splineTime);
                        polyLine.SplineContainer.Evaluate(splineTime, out var bkPos, out _, out var bkUp);

                        SplineUtility.GetPointAtLinearDistance(polyLine.SplineContainer.Spline, splineTime, _size.y, out var frSplineTime);

                        polyLine.SplineContainer.Evaluate(frSplineTime, out var frPos, out _, out _);

                        var direction = math.normalize(frPos - bkPos);
                        var rot = SynthesizeRotation(direction, bkUp);

                        var pos = bkPos + math.rotate(rot, math.rotate(math.inverse(orientation), -backAnchor));
                        rot = math.mul(rot, math.inverse(orientation));

                        _rigidbody.transform.SetPositionAndRotation(pos, rot);

                        break;
                    }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {

            SimpleTransform origin = GetContactOriginInWorld();

            Gizmos.color = Color.red;


            Vector3 forward = FrontContact;
            Vector3 right = RightContact;

            Gizmos.matrix = Matrix4x4.TRS(origin.position, origin.rotation, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Vector3.zero, Vector3.right * 0.15f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(Vector3.zero, Vector3.up * 0.15f);

            switch (_contactCount)
            {
                case ContactCount.One:
                    Gizmos.color = Color.white;

                    Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.05f, 0, 0.05f));
                    Gizmos.DrawWireSphere(Vector3.zero, 0.01f);
                    break;
                case ContactCount.Two:
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(Vector3.zero, new Vector3(_size.y, 0, 0) + Vector3.forward * 0.05f);
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(Vector3.zero + forward, 0.01f);
                    Gizmos.DrawWireSphere(Vector3.zero - forward, 0.01f);
                    break;
                case ContactCount.Four:
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(Vector3.zero, Size);
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(Vector3.zero + right + forward, 0.01f);
                    Gizmos.DrawWireSphere(Vector3.zero + right - forward, 0.01f);
                    Gizmos.DrawWireSphere(Vector3.zero - right + forward, 0.01f);
                    Gizmos.DrawWireSphere(Vector3.zero - right - forward, 0.01f);
                    break;
            }
        }
#endif
    }
}


