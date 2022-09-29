using SLZ.Marrow.Utilities;
using UnityEngine;
using SLZ.Marrow.Data;

namespace SLZ.Marrow.Interaction
{
    [DisallowMultipleComponent]
    public partial class MarrowBody : MarrowBehaviour
    {

        [SerializeField] private Collider[] _colliders;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private RigidbodyInfo _rigidbodySettings = new();
        [SerializeField] private MarrowBodyTracker _tracker;

        [field: SerializeField] public SimpleTransform InitInEntityTransform { get; private set; } = SimpleTransform.Identity;
        [field: SerializeField] public bool HasRigidbody { get; private set; }
        [field: SerializeField] public MarrowEntity Entity { get; private set; }
        [field: SerializeField] public Bounds Bounds { get; private set; }

        public bool TryGetRigidbody(out Rigidbody rigidbody)
        {
            rigidbody = _rigidbody;
            return HasRigidbody;
        }
    }
}