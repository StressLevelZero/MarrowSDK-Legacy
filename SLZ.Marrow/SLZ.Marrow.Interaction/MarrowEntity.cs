using SLZ.Marrow.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace SLZ.Marrow.Interaction
{
    [SelectionBase]
    public partial class MarrowEntity : MarrowBehaviour
    {

        [Header("Marrow Object")]
        [SerializeField] private MarrowBody[] _bodies;
        [SerializeField] private MarrowJoint[] _joints;
        [SerializeField] private Collider[] _staticColliders;


        [SerializeField] private MarrowBody _anchorBody;

        public UnityEvent OnFreeze;
        public UnityEvent OnUnfreeze;

        private void SnapshotPose()
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.MarrowEntity.SnapshotPose()");

            throw new System.NotImplementedException();
        }
        private void ApplyPose()
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.MarrowEntity.ApplyPose()");

            throw new System.NotImplementedException();
        }
        public void Teleport(Vector3 position, Quaternion rotation, bool doResetPose = false)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.MarrowEntity.Teleport(UnityEngine.Vector3, UnityEngine.Quaternion, bool)");

            throw new System.NotImplementedException();
        }
        public void Freeze()
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.MarrowEntity.Freeze()");

            throw new System.NotImplementedException();
        }
        public void Unfreeze()
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Interaction.MarrowEntity.Unfreeze()");

            throw new System.NotImplementedException();
        }
    }
}