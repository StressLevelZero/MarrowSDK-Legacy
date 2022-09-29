// UltEvents // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace UltEvents.Editor
{
    /// <summary>[Editor-Only]
    /// Manages the GUI state used when drawing events.
    /// </summary>
    internal sealed class DrawerState
    {
        /************************************************************************************************************************/

        /// <summary>The currently active state.</summary>
        public static readonly DrawerState Current = new DrawerState();

        /************************************************************************************************************************/

        /// <summary>The <see cref="SerializedProperty"/> for the event currently being drawn.</summary>
        public SerializedProperty EventProperty { get; private set; }

        /// <summary>The event currently being drawn.</summary>
        public UltEventBase Event { get; private set; }

        /************************************************************************************************************************/

        /// <summary>The <see cref="SerializedProperty"/> for the call currently being drawn.</summary>
        public SerializedProperty CallProperty { get; private set; }

        /// <summary>The <see cref="SerializedProperty"/> for the target of the call currently being drawn.</summary>
        public SerializedProperty TargetProperty { get; private set; }

        /// <summary>The <see cref="SerializedProperty"/> for the method name of the call currently being drawn.</summary>
        public SerializedProperty MethodNameProperty { get; private set; }

        /// <summary>The <see cref="SerializedProperty"/> for the persistent arguments array of the call currently being drawn.</summary>
        public SerializedProperty PersistentArgumentsProperty { get; private set; }

        /// <summary>The index of the call currently being drawn.</summary>
        public int callIndex = -1;

        /// <summary>The call currently being drawn.</summary>
        public PersistentCall call;

        /// <summary>The parameters of the call currently being drawn.</summary>
        public ParameterInfo[] callParameters;

        /// <summary>The index of the parameter currently being drawn.</summary>
        public int parameterIndex;

        /************************************************************************************************************************/

        /// <summary>If true, each call will be stored so that subsequent calls can link to their return value.</summary>
        public bool CachePreviousCalls { get; private set; }

        /// <summary>The calls of the current event that come before the current call currently being drawn.</summary>
        private readonly List<PersistentCall> PreviousCalls = new List<PersistentCall>();

        /// <summary>The methods targeted by the calls of the event currently being drawn.</summary>
        private readonly List<MethodBase> PersistentMethodCache = new List<MethodBase>();

        /************************************************************************************************************************/

        /// <summary>The parameter currently being drawn.</summary>
        public ParameterInfo CurrentParameter
        {
            get { return callParameters[parameterIndex]; }
        }

        /************************************************************************************************************************/

        /// <summary>Caches the event from the specified property and returns true as long as it is not null.</summary>
        public bool TryBeginEvent(SerializedProperty eventProperty)
        {
            Event = eventProperty.GetValue<UltEventBase>();
            if (Event == null)
                return false;

            EventProperty = eventProperty;
            return true;
        }

        /// <summary>Cancels out a call to <see cref="TryBeginEvent"/>.</summary>
        public void EndEvent()
        {
            EventProperty = null;
            Event = null;
        }

        /************************************************************************************************************************/

        /// <summary>Starts caching calls so that subsequent calls can link to earlier return values.</summary>
        public void BeginCache()
        {
            CacheLinkedArguments();
            CachePreviousCalls = true;
        }

        /// <summary>Cancels out a call to <see cref="EndCache"/>.</summary>
        public void EndCache()
        {
            CachePreviousCalls = false;
            PreviousCalls.Clear();
        }

        /************************************************************************************************************************/

        /// <summary>Caches the call from the specified property.</summary>
        public void BeginCall(SerializedProperty callProperty)
        {
            CallProperty = callProperty;

            TargetProperty = GetTargetProperty(callProperty);
            MethodNameProperty = GetMethodNameProperty(callProperty);
            PersistentArgumentsProperty = GetPersistentArgumentsProperty(callProperty);

            call = GetCall(callProperty);
        }

        /// <summary>Cancels out a call to <see cref="BeginCall"/>.</summary>
        public void EndCall()
        {
            if (CachePreviousCalls)
                PreviousCalls.Add(call);

            call = null;
        }

        /************************************************************************************************************************/

        /// <summary>Returns the property encapsulating the <see cref="PersistentCall.Target"/>.</summary>
        public static SerializedProperty GetTargetProperty(SerializedProperty callProperty)
        {
            return callProperty.FindPropertyRelative(Names.PersistentCall.Target);
        }

        /// <summary>Returns the property encapsulating the <see cref="PersistentCall.MethodName"/>.</summary>
        public static SerializedProperty GetMethodNameProperty(SerializedProperty callProperty)
        {
            return callProperty.FindPropertyRelative(Names.PersistentCall.MethodName);
        }

        /// <summary>Returns the property encapsulating the <see cref="PersistentCall.PersistentArguments"/>.</summary>
        public static SerializedProperty GetPersistentArgumentsProperty(SerializedProperty callProperty)
        {
            return callProperty.FindPropertyRelative(Names.PersistentCall.PersistentArguments);
        }

        /// <summary>Returns the call encapsulated by the specified property.</summary>
        public static PersistentCall GetCall(SerializedProperty callProperty)
        {
            return callProperty.GetValue<PersistentCall>();
        }

        /************************************************************************************************************************/
        #region Linked Argument Cache
        /************************************************************************************************************************/

        /// <summary>Stores all the persistent methods in the current event.</summary>
        public void CacheLinkedArguments()
        {
            PersistentMethodCache.Clear();

            if (Event == null || Event._PersistentCalls == null)
                return;

            for (int i = 0; i < Event._PersistentCalls.Count; i++)
            {
                var call = Event._PersistentCalls[i];
                PersistentMethodCache.Add(call != null ? call.GetMethodSafe() : null);
            }
        }

        /************************************************************************************************************************/

        /// <summary>Ensures that any linked parameters remain linked to the correct target index.</summary>
        public void UpdateLinkedArguments()
        {
            if (Event == null ||
                PersistentMethodCache.Count == 0)
                return;

            for (int i = 0; i < Event._PersistentCalls.Count; i++)
            {
                var call = Event._PersistentCalls[i];
                if (call == null)
                    continue;

                for (int j = 0; j < call._PersistentArguments.Length; j++)
                {
                    var argument = call._PersistentArguments[j];
                    if (argument == null || argument._Type != PersistentArgumentType.ReturnValue)
                        continue;

                    var linkedMethod = PersistentMethodCache[argument.ReturnedValueIndex];

                    if (argument.ReturnedValueIndex < Event._PersistentCalls.Count)
                    {
                        var linkedCall = Event._PersistentCalls[argument.ReturnedValueIndex];
                        if (linkedMethod == (linkedCall != null ? linkedCall.GetMethodSafe() : null))
                            continue;
                    }

                    var index = IndexOfMethod(linkedMethod);
                    if (index >= 0)
                        argument.ReturnedValueIndex = index;
                }
            }

            PersistentMethodCache.Clear();
        }

        /************************************************************************************************************************/

        /// <summary>Returns the index of the persistent call that targets the specified `method` or -1 if there is none.</summary>
        public int IndexOfMethod(MethodBase method)
        {
            for (int i = 0; i < Event._PersistentCalls.Count; i++)
            {
                var call = Event._PersistentCalls[i];
                if ((call != null ? call.GetMethodSafe() : null) == method)
                {
                    return i;
                }
            }

            return -1;
        }

        /************************************************************************************************************************/

        /// <summary>Returns the method cached from the persistent call at the specified `index`.</summary>
        public MethodBase GetLinkedMethod(int index)
        {
            if (index >= 0 && index < PersistentMethodCache.Count)
                return PersistentMethodCache[index];
            else
                return null;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Previous Call Cache
        /************************************************************************************************************************/

        /// <summary>Tries to get the details of the a parameter or return value of the specified `type`.</summary>
        public bool TryGetLinkable(Type type, out int linkIndex, out PersistentArgumentType linkType)
        {
            if (Event != null)
            {
                // Parameters.
                var parameterTypes = Event.ParameterTypes;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (type.IsAssignableFrom(parameterTypes[i]))
                    {
                        linkIndex = i;
                        linkType = PersistentArgumentType.Parameter;
                        return true;
                    }
                }

                // Return Values.
                for (int i = 0; i < PreviousCalls.Count; i++)
                {
                    var method = PreviousCalls[i].GetMethodSafe();
                    if (method == null)
                        continue;

                    if (type.IsAssignableFrom(method.GetReturnType()))
                    {
                        linkIndex = i;
                        linkType = PersistentArgumentType.ReturnValue;
                        return true;
                    }
                }
            }

            linkIndex = -1;
            linkType = PersistentArgumentType.None;
            return false;
        }

        /************************************************************************************************************************/

        /// <summary>Tries to get the details of the a parameter or return value of the current parameter type.</summary>
        public bool TryGetLinkable(out int linkIndex, out PersistentArgumentType linkType)
        {
            if (callParameters != null)
            {
                return TryGetLinkable(CurrentParameter.ParameterType, out linkIndex, out linkType);
            }
            else
            {
                linkIndex = -1;
                linkType = PersistentArgumentType.None;
                return false;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The number of persistent calls that came earlier in the current event.</summary>
        public int PreviousCallCount
        {
            get { return PreviousCalls.Count; }
        }

        /************************************************************************************************************************/

        /// <summary>Returns the persistent call at the specified index in the current event.</summary>
        public PersistentCall GetPreviousCall(int index)
        {
            if (index >= 0 && index < PreviousCalls.Count)
                return PreviousCalls[index];
            else
                return null;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>Copies the contents of the `other` state to overwrite this one.</summary>
        public void CopyFrom(DrawerState other)
        {
            EventProperty = other.EventProperty;
            Event = other.Event;

            CallProperty = other.CallProperty;
            TargetProperty = other.TargetProperty;
            MethodNameProperty = other.MethodNameProperty;
            PersistentArgumentsProperty = other.PersistentArgumentsProperty;

            callIndex = other.callIndex;
            call = other.call;
            callParameters = other.callParameters;
            parameterIndex = other.parameterIndex;

            PreviousCalls.Clear();
            PreviousCalls.AddRange(other.PreviousCalls);
        }

        /************************************************************************************************************************/

        /// <summary>Clears all the details of this state.</summary>
        public void Clear()
        {
            EventProperty = null;
            Event = null;

            CallProperty = null;
            TargetProperty = null;
            MethodNameProperty = null;
            PersistentArgumentsProperty = null;

            callIndex = -1;
            call = null;
            callParameters = null;
            parameterIndex = 0;

            PreviousCalls.Clear();
        }

        /************************************************************************************************************************/
    }
}

#endif
