// UltEvents // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace UltEvents.Editor
{
    /// <summary>[Editor-Only]
    /// Manages the copying and pasting of events and persistent calls.
    /// </summary>
    public static class Clipboard
    {
        /************************************************************************************************************************/

        private static UltEventBase _Event;

        /// <summary>Indicates whether an event has been copied.</summary>
        public static bool HasEvent { get { return _Event != null; } }

        /************************************************************************************************************************/

        /// <summary>Stores the details of the specified event.</summary>
        public static void CopyEvent(UltEventBase e)
        {
            var eventType = e.GetType();

            if (_Event == null || _Event.GetType() != eventType)
                _Event = (UltEventBase)Activator.CreateInstance(eventType);

            _Event.CopyFrom(e);
        }

        /// <summary>Stores the details of the event contained in the specified property.</summary>
        public static void CopyEvent(Serialization.PropertyAccessor accessor, Object target)
        {
            var e = (UltEventBase)accessor.GetValue(target);
            CopyEvent(e);
        }

        /// <summary>Stores the details of the event contained in the specified property.</summary>
        public static void CopyEvent(SerializedProperty property)
        {
            var accessor = property.GetAccessor();
            if (accessor == null)
                return;

            CopyEvent(accessor, property.serializedObject.targetObject);
        }

        /************************************************************************************************************************/

        /// <summary>Overwrites the specified event with the previously copied details.</summary>
        public static void Paste(UltEventBase e)
        {
            e.CopyFrom(_Event);
        }

        /************************************************************************************************************************/

        private static PersistentCall _Call;

        /// <summary>Indicates whether a persistent call has been copied.</summary>
        public static bool HasCall { get { return _Call != null; } }

        /************************************************************************************************************************/

        /// <summary>Stores the details of the specified call.</summary>
        public static void CopyCall(PersistentCall call)
        {
            if (_Call == null)
                _Call = new PersistentCall();

            _Call.CopyFrom(call);
        }

        /// <summary>Stores the details of the call contained in the specified property.</summary>
        public static void CopyCall(Serialization.PropertyAccessor accessor, Object target)
        {
            var call = (PersistentCall)accessor.GetValue(target);
            CopyCall(call);
        }

        /// <summary>Stores the details of the call contained in the specified property.</summary>
        public static void CopyCall(SerializedProperty property)
        {
            var accessor = property.GetAccessor();
            if (accessor == null)
                return;

            CopyCall(accessor, property.serializedObject.targetObject);
        }

        /************************************************************************************************************************/

        /// <summary>Overwrites the specified call with the previously copied details.</summary>
        public static void PasteCall(PersistentCall call)
        {
            call.CopyFrom(_Call);
        }

        /// <summary>Overwrites the call contained in the specified property with the copied details.</summary>
        public static void PasteCall(Serialization.PropertyAccessor accessor, Object target)
        {
            var call = (PersistentCall)accessor.GetValue(target);
            PasteCall(call);
        }

        /// <summary>Overwrites the call contained in the specified property with the copied details.</summary>
        public static void PasteCall(SerializedProperty property)
        {
            property.ModifyValues<PersistentCall>((call) =>
            {
                PasteCall(call);
            }, "Paste PersistentCall");
        }

        /************************************************************************************************************************/
    }
}

#endif