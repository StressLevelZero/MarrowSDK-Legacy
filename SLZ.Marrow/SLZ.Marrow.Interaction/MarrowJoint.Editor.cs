using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    public partial class MarrowJoint
    {
#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private bool _isConfigurableJointHidden = true;

        private void OnValidate()
        {
            if (!HasConfigJoint) return;
            Editor_RefreshConfigurableJointView();
        }

        private void Editor_RefreshConfigurableJointView()
        {
            if (_isConfigurableJointHidden)
                _configurableJoint.hideFlags |= HideFlags.HideInInspector;
            else
                _configurableJoint.hideFlags &= ~HideFlags.HideInInspector;
        }


        [ContextMenu("Toggle ConfigurableJoint")]
        private void Editor_ToggleConfigurableJoint()
        {
            if ((_configurableJoint.hideFlags & HideFlags.HideInInspector) != 0)
                _configurableJoint.hideFlags &= ~HideFlags.HideInInspector;
            else
                _configurableJoint.hideFlags |= HideFlags.HideInInspector;
        }

        public void Editor_SetConfigurableJoint(ConfigurableJoint cj, bool read)
        {
            _configurableJoint = cj;
            HasConfigJoint = _configurableJoint != null;


            if (read)
            {
                _configurableJointSettings.Snapshot(cj);
                _configurableJointSettings.CalculateStartRotation(cj);
                if (cj.connectedBody != null)
                    _connectedMarrowBody = cj.connectedBody.gameObject.GetComponent<MarrowBody>();
            }


            else
            {
                _configurableJointSettings.Apply(cj);

                if (_connectedMarrowBody == null)
                    return;


                if (_connectedMarrowBody.gameObject == gameObject)
                {
                    _connectedMarrowBody = null;
                    return;
                }

                cj.connectedBody = _connectedMarrowBody.gameObject.GetComponent<Rigidbody>();
            }
        }

        public void Editor_SetObject(MarrowEntity marrowEntity)
        {
            _parent = marrowEntity;
        }
#endif
    }
}