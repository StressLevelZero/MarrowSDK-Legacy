// UltEvents // Copyright 2021 Kybernetik //

using System;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace UltEvents
{
    /// <summary>
    /// Allows you to expose the add and remove methods of an <see cref="UltEvent{T0, T1}"/> without exposing the rest of its
    /// members such as the ability to invoke it.
    /// </summary>
    public interface IUltEvent<T0, T1> : IUltEventBase
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Delegates registered here are invoked by <see cref="UltEvent{T0, T1}.Invoke"/> after all
        /// <see cref="UltEvent{T0, T1}.PersistentCalls"/>.
        /// </summary>
        event Action<T0, T1> DynamicCalls;

        /************************************************************************************************************************/
    }

    /// <summary>
    /// A serializable event with 2 parameters which can be viewed and configured in the inspector.
    /// <para></para>
    /// This is a more versatile and user friendly implementation than <see cref="UnityEvent{T0, T1}"/>.
    /// </summary>
    [Serializable]
    public class UltEvent<T0, T1> : UltEventBase, IUltEvent<T0, T1>
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        public override int ParameterCount { get { return 2; } }

        /************************************************************************************************************************/

        /// <summary>
        /// Delegates registered to this event are serialized as <see cref="PersistentCall"/>s and are invoked by
        /// <see cref="Invoke"/> before all <see cref="DynamicCalls"/>.
        /// </summary>
        public event Action<T0, T1> PersistentCalls
        {
            add
            {
                AddPersistentCall(value);
            }
            remove
            {
                RemovePersistentCall(value);
            }
        }

        /************************************************************************************************************************/

        private Action<T0, T1> _DynamicCalls;

        /// <summary>
        /// Delegates registered here are invoked by <see cref="Invoke"/> after all <see cref="PersistentCalls"/>.
        /// </summary>
        public event Action<T0, T1> DynamicCalls
        {
            add
            {
                _DynamicCalls += value;
                OnDynamicCallsChanged();
            }
            remove
            {
                _DynamicCalls -= value;
                OnDynamicCallsChanged();
            }
        }

        /// <summary>
        /// The non-serialized method and parameter details of this event.
        /// Delegates registered here are called by <see cref="Invoke"/> after all <see cref="PersistentCalls"/>.
        /// </summary>
        protected override Delegate DynamicCallsBase
        {
            get { return _DynamicCalls; }
            set { _DynamicCalls = value as Action<T0, T1>; }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Operators and Call Registration
        /************************************************************************************************************************/

        /// <summary>
        /// Ensures that `e` isn't null and adds `method` to its <see cref="PersistentCalls"/> (if in Edit Mode) or
        /// <see cref="DynamicCalls"/> (in Play Mode and at runtime).
        /// </summary>
        public static UltEvent<T0, T1> operator +(UltEvent<T0, T1> e, Action<T0, T1> method)
        {
            if (e == null)
                e = new UltEvent<T0, T1>();

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && method.Target is Object)
            {
                e.PersistentCalls += method;
                return e;
            }
#endif

            e.DynamicCalls += method;
            return e;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If `e` isn't null, this method removes `method` from its <see cref="PersistentCalls"/> (if in Edit Mode) or
        /// <see cref="DynamicCalls"/> (in Play Mode and at runtime).
        /// </summary>
        public static UltEvent<T0, T1> operator -(UltEvent<T0, T1> e, Action<T0, T1> method)
        {
            if (e == null)
                return null;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && method.Target is Object)
            {
                e.PersistentCalls -= method;
                return e;
            }
#endif

            e.DynamicCalls -= method;
            return e;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates a new <see cref="UltEventBase"/> and adds `method` to its <see cref="PersistentCalls"/> (if in edit
        /// mode), or <see cref="DynamicCalls"/> (in Play Mode and at runtime).
        /// </summary>
        public static implicit operator UltEvent<T0, T1>(Action<T0, T1> method)
        {
            if (method != null)
            {
                var e = new UltEvent<T0, T1>();
                e += method;
                return e;
            }
            else return null;
        }

        /************************************************************************************************************************/

        /// <summary>Ensures that `e` isn't null and adds `method` to its <see cref="DynamicCalls"/>.</summary>
        public static void AddDynamicCall(ref UltEvent<T0, T1> e, Action<T0, T1> method)
        {
            if (e == null)
                e = new UltEvent<T0, T1>();

            e.DynamicCalls += method;
        }

        /// <summary>If `e` isn't null, this method removes `method` from its <see cref="DynamicCalls"/>.</summary>
        public static void RemoveDynamicCall(ref UltEvent<T0, T1> e, Action<T0, T1> method)
        {
            if (e != null)
                e.DynamicCalls -= method;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only] The types of each of this event's parameters.</summary>
        public override Type[] ParameterTypes { get { return _ParameterTypes; } }
        private static Type[] _ParameterTypes = new Type[] { typeof(T0), typeof(T1) };
#endif

        /************************************************************************************************************************/

        /// <summary>
        /// Invokes all <see cref="PersistentCalls"/> then all <see cref="DynamicCalls"/>.
        /// <para></para>
        /// See also: <seealso cref="InvokeSafe"/> and <seealso cref="UltEventUtils.InvokeX{T0, T1}(UltEvent{T0, T1}, T0, T1)"/>.
        /// </summary>
        public virtual void Invoke(T0 parameter0, T1 parameter1)
        {
            CacheParameter(parameter0);
            CacheParameter(parameter1);
            InvokePersistentCalls();
            if (_DynamicCalls != null)
                _DynamicCalls(parameter0, parameter1);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Invokes all <see cref="PersistentCalls"/> then all <see cref="DynamicCalls"/> inside a try/catch block
        /// which logs any exceptions that are thrown.
        /// <para></para>
        /// See also: <seealso cref="Invoke"/> and <seealso cref="UltEventUtils.InvokeX{T0, T1}(UltEvent{T0, T1}, T0, T1)"/>.
        /// </summary>
        public virtual void InvokeSafe(T0 parameter0, T1 parameter1)
        {
            try
            {
                Invoke(parameter0, parameter1);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /************************************************************************************************************************/
    }
}
