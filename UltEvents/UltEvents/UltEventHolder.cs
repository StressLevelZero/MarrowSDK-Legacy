// UltEvents // Copyright 2021 Kybernetik //

using UnityEngine;

namespace UltEvents
{
    /// <summary>
    /// A component which encapsulates a single <see cref="UltEvent"/>.
    /// </summary>
    [AddComponentMenu(UltEventUtils.ComponentMenuPrefix + "Ult Event Holder")]
    [HelpURL(UltEventUtils.APIDocumentationURL + "/UltEventHolder")]
    public class UltEventHolder : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private UltEvent _Event;

        /// <summary>The encapsulated event.</summary>
        public UltEvent Event
        {
            get
            {
                if (_Event == null)
                    _Event = new UltEvent();
                return _Event;
            }
            set { _Event = value; }
        }

        /************************************************************************************************************************/

        /// <summary>Calls Event.Invoke().</summary>
        public virtual void Invoke()
        {
            if (_Event != null)
                _Event.Invoke();
        }

        /************************************************************************************************************************/
    }
}