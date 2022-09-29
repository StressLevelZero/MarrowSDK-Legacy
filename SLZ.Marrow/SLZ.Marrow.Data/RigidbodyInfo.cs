using System;
using UnityEngine;

namespace SLZ.Marrow.Data
{
    [Serializable]
    public class RigidbodyInfo
    {
        [SerializeField] private float _mass = 1f;
        [SerializeField] private float _drag = 0f;
        [SerializeField] private float _angularDrag = 0f;
        [SerializeField] private bool _useGravity = true;
        [SerializeField] private bool _isKinematic = false;
        [SerializeField] private RigidbodyInterpolation _interpolate;
        [SerializeField, EnumFlags] private CollisionDetectionMode _collisionDetection;
        [SerializeField] private RigidbodyConstraints _constraints;
        [SerializeField] private Vector3 _centerOfMass;
        [SerializeField] private float _maxAngularVelocity;
        [SerializeField] private Vector3 _inertiaTensor;

        [SerializeField] private Vector3 _initalVelocity;
        [SerializeField] private Vector3 _initialAngularVelocity;

        public void Snapshot(Rigidbody rb)
        {
            _mass = rb.mass;
            _drag = rb.drag;
            _angularDrag = rb.angularDrag;
            _useGravity = rb.useGravity;
            _isKinematic = rb.isKinematic;
            _interpolate = rb.interpolation;
            _collisionDetection = rb.collisionDetectionMode;
            _constraints = rb.constraints;
            _centerOfMass = rb.centerOfMass;
            _maxAngularVelocity = rb.maxAngularVelocity;
            _inertiaTensor = rb.inertiaTensor;
            _initalVelocity = rb.velocity;
            _initialAngularVelocity = rb.angularVelocity;
        }

        public void Apply(Rigidbody rb)
        {
            rb.mass = _mass;
            rb.drag = _drag;
            rb.angularDrag = _angularDrag;
            rb.useGravity = _useGravity;
            rb.isKinematic = _isKinematic;
            rb.interpolation = _interpolate;
            rb.collisionDetectionMode = _collisionDetection;
            rb.constraints = _constraints;
            rb.centerOfMass = _centerOfMass;
            rb.maxAngularVelocity = _maxAngularVelocity;
            rb.inertiaTensor = _inertiaTensor;
            rb.velocity = _initalVelocity;
            rb.angularVelocity = _initialAngularVelocity;
        }

    }
}