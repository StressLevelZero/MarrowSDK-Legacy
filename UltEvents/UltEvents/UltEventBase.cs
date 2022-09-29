// UltEvents // Copyright 2021 Kybernetik //

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace UltEvents
{
    /// <summary>
    /// Allows you to expose the add and remove methods of an <see cref="UltEvent"/> without exposing the rest of its
    /// members such as the ability to invoke it.
    /// </summary>
    public interface IUltEventBase
    {
        /************************************************************************************************************************/

        /// <summary>Adds the specified 'method to the persistent call list.</summary>
        PersistentCall AddPersistentCall(Delegate method);

        /// <summary>Removes the specified 'method from the persistent call list.</summary>
        void RemovePersistentCall(Delegate method);

        /************************************************************************************************************************/
    }

    /// <summary>
    /// A serializable event which can be viewed and configured in the inspector.
    /// <para></para>
    /// This is a more versatile and user friendly implementation than <see cref="UnityEvent"/>.
    /// </summary>
    [Serializable]
    public abstract class UltEventBase : IUltEventBase
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        public abstract int ParameterCount { get; }

        /************************************************************************************************************************/

        [SerializeField]
        internal List<PersistentCall> _PersistentCalls;

        /// <summary>
        /// The serialized method and parameter details of this event.
        /// </summary>
        public List<PersistentCall> PersistentCallsList
        {
            get { return _PersistentCalls; }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The non-serialized method and parameter details of this event.
        /// </summary>
        protected abstract Delegate DynamicCallsBase { get; set; }

        /// <summary>
        /// Clears the cached invocation list of <see cref="DynamicCallsBase"/>.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        protected void OnDynamicCallsChanged()
        {
#if UNITY_EDITOR
            _DynamicCallInvocationList = null;
#endif
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        internal bool HasAnyDynamicCalls()
        {
            return DynamicCallsBase != null;
        }

        /************************************************************************************************************************/

        private Delegate[] _DynamicCallInvocationList;

        internal Delegate[] GetDynamicCallInvocationList()
        {
            if (_DynamicCallInvocationList == null && DynamicCallsBase != null)
                _DynamicCallInvocationList = DynamicCallsBase.GetInvocationList();

            return _DynamicCallInvocationList;
        }

        internal int GetDynamicCallInvocationListCount()
        {
            if (DynamicCallsBase == null)
                return 0;
            else
                return GetDynamicCallInvocationList().Length;
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Operators and Call Registration
        /************************************************************************************************************************/

        /// <summary>Ensures that `e` isn't null and adds `method` to its <see cref="PersistentCallsList"/>.</summary>
        public static PersistentCall AddPersistentCall<T>(ref T e, Delegate method) where T : UltEventBase, new()
        {
            if (e == null)
                e = new T();

            return e.AddPersistentCall(method);
        }

        /// <summary>Ensures that `e` isn't null and adds `method` to its <see cref="PersistentCallsList"/>.</summary>
        public static PersistentCall AddPersistentCall<T>(ref T e, Action method) where T : UltEventBase, new()
        {
            if (e == null)
                e = new T();

            return e.AddPersistentCall(method);
        }

        /************************************************************************************************************************/

        /// <summary>If `e` isn't null, this method removes `method` from its <see cref="PersistentCallsList"/>.</summary>
        public static void RemovePersistentCall(ref UltEventBase e, Delegate method)
        {
            if (e != null)
                e.RemovePersistentCall(method);
        }

        /// <summary>If `e` isn't null, this method removes `method` from its <see cref="PersistentCallsList"/>.</summary>
        public static void RemovePersistentCall(ref UltEventBase e, Action method)
        {
            if (e != null)
                e.RemovePersistentCall(method);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Add the specified 'method to the persistent call list.
        /// </summary>
        public PersistentCall AddPersistentCall(Delegate method)
        {
            if (_PersistentCalls == null)
                _PersistentCalls = new List<PersistentCall>(4);

            var call = new PersistentCall(method);
            _PersistentCalls.Add(call);
            return call;
        }

        /// <summary>
        /// Remove the specified 'method from the persistent call list.
        /// </summary>
        public void RemovePersistentCall(Delegate method)
        {
            if (_PersistentCalls == null)
                return;

            for (int i = 0; i < _PersistentCalls.Count; i++)
            {
                var call = _PersistentCalls[i];
                if (call.GetMethodSafe() == method.Method && ReferenceEquals(call.Target, method.Target))
                {
                    _PersistentCalls.RemoveAt(i);
                    return;
                }
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>
        /// Invokes all <see cref="PersistentCalls"/> then all <see cref="DynamicCalls"/>.
        /// </summary>
        public void DynamicInvoke(params object[] parameters)
        {
            // A larger array would actually work fine, but it probably still means something is wrong.
            if (parameters.Length != ParameterCount)
                throw new ArgumentException("Invalid parameter count " +
                     parameters.Length + " should be " + ParameterCount);

            CacheParameters(parameters);
            InvokePersistentCalls();
            var dynamicCalls = DynamicCallsBase;
            if (dynamicCalls != null)
                dynamicCalls.DynamicInvoke(parameters);
        }

        /************************************************************************************************************************/

        /// <summary>Invokes all <see cref="PersistentCall"/>s registered to this event.</summary>
        protected void InvokePersistentCalls()
        {
            var originalParameterOffset = _ParameterOffset;
            var originalReturnValueOffset = _ReturnValueOffset;

            try
            {
                if (_PersistentCalls != null)
                {
                    for (int i = 0; i < _PersistentCalls.Count; i++)
                    {
                        var result = _PersistentCalls[i].Invoke();
                        LinkedValueCache.Add(result);
                        _ParameterOffset = originalParameterOffset;
                        _ReturnValueOffset = originalReturnValueOffset;
                    }
                }
            }
            finally
            {
                LinkedValueCache.RemoveRange(originalParameterOffset, LinkedValueCache.Count - originalParameterOffset);
                _ParameterOffset = _ReturnValueOffset = originalParameterOffset;
            }
        }

        /************************************************************************************************************************/
        #region Linked Value Cache (Parameters and Returned Values)
        /************************************************************************************************************************/

        private static readonly List<object> LinkedValueCache = new List<object>();
        private static int _ParameterOffset, _ReturnValueOffset;

        /************************************************************************************************************************/

        internal static void UpdateLinkedValueOffsets()
        {
            _ParameterOffset = _ReturnValueOffset = LinkedValueCache.Count;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Stores the `parameter` so it can be accessed by <see cref="PersistentCall"/>s.
        /// </summary>
        protected static void CacheParameter(object parameter)
        {
            LinkedValueCache.Add(parameter);
            _ReturnValueOffset = LinkedValueCache.Count;
        }

        /// <summary>
        /// Stores the `parameters` so they can be accessed by <see cref="PersistentCall"/>s.
        /// </summary>
        protected static void CacheParameters(object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
                LinkedValueCache.Add(parameters[i]);

            _ReturnValueOffset = LinkedValueCache.Count;
        }

        /************************************************************************************************************************/

        internal static int ReturnedValueCount
        {
            get { return LinkedValueCache.Count - _ReturnValueOffset; }
        }

        /************************************************************************************************************************/

        internal static object GetParameterValue(int index)
        {
            return LinkedValueCache[_ParameterOffset + index];
        }

        internal static object GetReturnedValue(int index)
        {
            return LinkedValueCache[_ReturnValueOffset + index];
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Parameter Display
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>The type of each of this event's parameters.</summary>
        public abstract Type[] ParameterTypes { get; }

        /************************************************************************************************************************/

        private static readonly Dictionary<Type, ParameterInfo[]>
            EventTypeToParameters = new Dictionary<Type, ParameterInfo[]>();

        internal ParameterInfo[] Parameters
        {
            get
            {
                var type = GetType();

                ParameterInfo[] parameters;
                if (!EventTypeToParameters.TryGetValue(type, out parameters))
                {
                    var invokeMethod = type.GetMethod("Invoke", ParameterTypes);
                    if (invokeMethod == null || invokeMethod.DeclaringType == typeof(UltEvent) ||
                        invokeMethod.DeclaringType.Name.StartsWith(Names.UltEvent.Class + "`"))
                    {
                        parameters = null;
                    }
                    else
                    {
                        parameters = invokeMethod.GetParameters();
                    }

                    EventTypeToParameters.Add(type, parameters);
                }

                return parameters;
            }
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<Type, string>
            EventTypeToParameterString = new Dictionary<Type, string>();

        internal string ParameterString
        {
            get
            {
                var type = GetType();

                string parameters;
                if (!EventTypeToParameterString.TryGetValue(type, out parameters))
                {
                    if (ParameterTypes.Length == 0)
                    {
                        parameters = " ()";
                    }
                    else
                    {
                        var invokeMethodParameters = Parameters;

                        var text = new StringBuilder();

                        text.Append(" (");
                        for (int i = 0; i < ParameterTypes.Length; i++)
                        {
                            if (i > 0)
                                text.Append(", ");

                            text.Append(ParameterTypes[i].GetNameCS(false));

                            if (invokeMethodParameters != null)
                            {
                                text.Append(" ");
                                text.Append(invokeMethodParameters[i].Name);
                            }
                        }
                        text.Append(")");

                        parameters = text.ToString();
                    }

                    EventTypeToParameterString.Add(type, parameters);
                }

                return parameters;
            }
        }

        /************************************************************************************************************************/
#endif
        #endregion
        /************************************************************************************************************************/

        /// <summary>
        /// Clears all <see cref="PersistentCallsList"/> and <see cref="DynamicCallsBase"/> registered to this event.
        /// </summary>
        public void Clear()
        {
            if (_PersistentCalls != null)
                _PersistentCalls.Clear();

            DynamicCallsBase = null;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns true if this event has any <see cref="PersistentCallsList"/> or <see cref="DynamicCallsBase"/> registered.
        /// </summary>
        public bool HasCalls
        {
            get
            {
                return (_PersistentCalls != null && _PersistentCalls.Count > 0) || DynamicCallsBase != null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Copies the contents of this the `target` event to this event.</summary>
        public virtual void CopyFrom(UltEventBase target)
        {
            if (target._PersistentCalls == null)
            {
                _PersistentCalls = null;
            }
            else
            {
                if (_PersistentCalls == null)
                    _PersistentCalls = new List<PersistentCall>();
                else
                    _PersistentCalls.Clear();

                for (int i = 0; i < target._PersistentCalls.Count; i++)
                {
                    var call = new PersistentCall();
                    call.CopyFrom(target._PersistentCalls[i]);
                    _PersistentCalls.Add(call);
                }
            }

            DynamicCallsBase = target.DynamicCallsBase;

#if UNITY_EDITOR
            _DynamicCallInvocationList = target._DynamicCallInvocationList;
#endif
        }

        /************************************************************************************************************************/

        /// <summary>Returns a description of this event.</summary>
        public override string ToString()
        {
            var text = new StringBuilder();
            ToString(text);
            return text.ToString();
        }

        /// <summary>Appends a description of this event.</summary>
        public void ToString(StringBuilder text)
        {
            text.Append(GetType().GetNameCS());

            text.Append(": PersistentCalls=");
            UltEventUtils.AppendDeepToString(text, _PersistentCalls.GetEnumerator(), "\n    ");

            text.Append("\n    DynamicCalls=");
#if UNITY_EDITOR
            var invocationList = GetDynamicCallInvocationList();
#else
            var invocationList = DynamicCallsBase != null ? DynamicCallsBase.GetInvocationList() : null;
#endif
            var enumerator = invocationList != null ? invocationList.GetEnumerator() : null;
            UltEventUtils.AppendDeepToString(text, enumerator, "\n    ");
        }

        /************************************************************************************************************************/
    }
}
