// UltEvents // Copyright 2021 Kybernetik //

using UnityEngine;

namespace UltEvents
{
    /// <summary>
    /// Holds <see cref="UltEvent"/>s which are called by various <see cref="MonoBehaviour"/> update events:
    /// <see cref="Update"/>, <see cref="LateUpdate"/>, and <see cref="FixedUpdate"/>.
    /// </summary>
    [AddComponentMenu(UltEventUtils.ComponentMenuPrefix + "Update Events")]
    [HelpURL(UltEventUtils.APIDocumentationURL + "/UpdateEvents")]
    [DisallowMultipleComponent]
    public class UpdateEvents : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private UltEvent _UpdateEvent;

        /// <summary>Invoked by <see cref="Update"/>.</summary>
        public UltEvent UpdateEvent
        {
            get
            {
                if (_UpdateEvent == null)
                    _UpdateEvent = new UltEvent();
                return _UpdateEvent;
            }
            set { _UpdateEvent = value; }
        }

        /// <summary>Invokes <see cref="UpdateEvent"/>.</summary>
        public virtual void Update()
        {
            if (_UpdateEvent != null)
                _UpdateEvent.Invoke();
        }

        /************************************************************************************************************************/

        [SerializeField]
        private UltEvent _LateUpdateEvent;

        /// <summary>Invoked by <see cref="LateUpdate"/>.</summary>
        public UltEvent LateUpdateEvent
        {
            get
            {
                if (_LateUpdateEvent == null)
                    _LateUpdateEvent = new UltEvent();
                return _LateUpdateEvent;
            }
            set { _LateUpdateEvent = value; }
        }

        /// <summary>Invokes <see cref="LateUpdateEvent"/>.</summary>
        public virtual void LateUpdate()
        {
            if (_LateUpdateEvent != null)
                _LateUpdateEvent.Invoke();
        }

        /************************************************************************************************************************/

        [SerializeField]
        private UltEvent _FixedUpdateEvent;

        /// <summary>Invoked by <see cref="FixedUpdate"/>.</summary>
        public UltEvent FixedUpdateEvent
        {
            get
            {
                if (_FixedUpdateEvent == null)
                    _FixedUpdateEvent = new UltEvent();
                return _FixedUpdateEvent;
            }
            set { _FixedUpdateEvent = value; }
        }

        /// <summary>Invokes <see cref="FixedUpdateEvent"/>.</summary>
        public virtual void FixedUpdate()
        {
            if (_FixedUpdateEvent != null)
                _FixedUpdateEvent.Invoke();
        }

        /************************************************************************************************************************/
    }
}