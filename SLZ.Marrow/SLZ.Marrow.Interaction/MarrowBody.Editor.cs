using System;
using SLZ.Marrow.Utilities;
using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    public partial class MarrowBody
    {

#if UNITY_EDITOR



        [SerializeField, HideInInspector]
        private bool _isRigidbodyHidden = true;

        private void OnValidate()
        {
            if (!HasRigidbody) return;
            Editor_RefreshRigidbodyView();
        }

        private void Editor_RefreshRigidbodyView()
        {
            if (_isRigidbodyHidden)
                _rigidbody.hideFlags |= HideFlags.HideInInspector;
            else
                _rigidbody.hideFlags &= ~HideFlags.HideInInspector;
        }

        [ContextMenu("Toggle Rigidbody")]
        private void Editor_ToggleRigidbody()
        {
            _isRigidbodyHidden = !_isRigidbodyHidden;
            Editor_RefreshRigidbodyView();
        }

        public void Editor_ResetColliders()
        {
            _colliders = Array.Empty<Collider>();
        }

        public void Editor_AddTracker()
        {

            var len = transform.childCount;
            for (int i = len - 1; i >= 0; i--)
            {
                var childGo = transform.GetChild(i).gameObject;
                _tracker = childGo.GetComponent<MarrowBodyTracker>();

                if (_tracker != null)
                {
                    DestroyImmediate(childGo);
                }
            }

            var go = new GameObject("MarrowBodyTracker");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            _tracker = go.AddComponent<MarrowBodyTracker>();
            _tracker.Editor_Setup(this, Entity, Bounds);
        }

        public void Editor_CalculateBounds()
        {
            var initT = new SimpleTransform(transform);

            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _rigidbody.position = Vector3.zero;
            _rigidbody.rotation = Quaternion.identity;

            var bounds = new Bounds();

            for (int i = 0; i < _colliders.Length; i++)
            {
                var col = _colliders[i];
                if (i == 0) bounds = col.bounds;
                bounds.Encapsulate(col.bounds);
            }

            Bounds = bounds;

            initT.CopyTo(transform);
            _rigidbody.position = initT.position;
            _rigidbody.rotation = initT.rotation;
        }

        public void Editor_AddCollider(Collider col)
        {
            Array.Resize(ref _colliders, _colliders.Length + 1);
            _colliders[^1] = col;
        }

        public void Editor_SetRigidbody(Rigidbody rb, bool read)
        {
            _rigidbody = rb;
            HasRigidbody = _rigidbody != null;

            if (read)
            {
                _rigidbodySettings.Snapshot(rb);
            }
            else
            {
                _rigidbodySettings.Apply(rb);
            }
        }

        public void Editor_SetEntity(MarrowEntity marrowEntity)
        {
            Entity = marrowEntity;
            InitInEntityTransform = SimpleTransform.InverseTransform(Entity.transform, transform);
        }
#endif
    }
}