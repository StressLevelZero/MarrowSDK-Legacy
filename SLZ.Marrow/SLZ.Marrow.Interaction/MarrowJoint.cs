using SLZ.Marrow.Data;
using SLZ.Marrow.Utilities;
using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    public partial class MarrowJoint : MarrowBehaviour
    {

        [SerializeField] private ConfigurableJoint _configurableJoint;
        [SerializeField] private MarrowBody _connectedMarrowBody;
        [SerializeField] private ConfigurableJointInfo _configurableJointSettings = new();
        [SerializeField] private MarrowEntity _parent;

        [field: SerializeField]
        public bool HasConfigJoint { get; private set; }

        public bool TryGetConfigurableJoint(out ConfigurableJoint joint)
        {
            joint = _configurableJoint;
            return HasConfigJoint;
        }
    }
}