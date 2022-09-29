// UltEvents // Copyright 2021 Kybernetik //

using System;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace UltEvents
{
    /// <summary>
    /// Allows you to expose the add and remove methods of an <see cref="UltEvent"/> without exposing the rest of its
    /// members such as the ability to invoke it.
    /// </summary>
    public interface IUltEvent : IUltEventBase
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Delegates registered here are invoked by <see cref="UltEvent.Invoke"/> after all
        /// <see cref="UltEvent.PersistentCalls"/>.
        /// </summary>
        event Action DynamicCalls;

        /************************************************************************************************************************/
    }

    /// <summary>
    /// A serializable event with no parameters which can be viewed and configured in the inspector.
    /// <para></para>
    /// This is a more versatile and user friendly implementation than <see cref="UnityEvent"/>.
    /// </summary>
    [Serializable]
    public sealed class UltEvent : UltEventBase, IUltEvent
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        public override int ParameterCount { get { return 0; } }

        /************************************************************************************************************************/

        /// <summary>
        /// Delegates registered to this event are serialized as <see cref="PersistentCall"/>s and are invoked by
        /// <see cref="Invoke"/> before all <see cref="DynamicCalls"/>.
        /// </summary>
        public event Action PersistentCalls
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

        private Action _DynamicCalls;

        /// <summary>
        /// Delegates registered here are invoked by <see cref="Invoke"/> after all <see cref="PersistentCalls"/>.
        /// </summary>
        public event Action DynamicCalls
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
            set { _DynamicCalls = value as Action; }
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
        public static UltEvent operator +(UltEvent e, Action method)
        {
            if (e == null)
                e = new UltEvent();

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
        public static UltEvent operator -(UltEvent e, Action method)
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
        public static implicit operator UltEvent(Action method)
        {
            if (method != null)
            {
                var e = new UltEvent();
                e += method;
                return e;
            }
            else return null;
        }

        /************************************************************************************************************************/

        /// <summary>Ensures that `e` isn't null and adds `method` to its <see cref="DynamicCalls"/>.</summary>
        public static void AddDynamicCall(ref UltEvent e, Action method)
        {
            if (e == null)
                e = new UltEvent();

            e.DynamicCalls += method;
        }

        /// <summary>If `e` isn't null, this method removes `method` from its <see cref="DynamicCalls"/>.</summary>
        public static void RemoveDynamicCall(ref UltEvent e, Action method)
        {
            if (e != null)
                e.DynamicCalls -= method;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only] The types of each of this event's parameters.</summary>
        public override Type[] ParameterTypes { get { return Type.EmptyTypes; } }
#endif

        /************************************************************************************************************************/

        /// <summary>
        /// Invokes all <see cref="PersistentCalls"/> then all <see cref="DynamicCalls"/>.
        /// <para></para>
        /// See also: <seealso cref="InvokeSafe"/> and <seealso cref="UltEventUtils.InvokeX(UltEvent)"/>.
        /// </summary>
        public void Invoke()
        {
            InvokePersistentCalls();
            if (_DynamicCalls != null)
                _DynamicCalls();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Invokes all <see cref="PersistentCalls"/> then all <see cref="DynamicCalls"/> inside a try/catch block
        /// which logs any exceptions that are thrown.
        /// <para></para>
        /// See also: <seealso cref="Invoke"/> and <seealso cref="UltEventUtils.InvokeX(UltEvent)"/>.
        /// </summary>
        public void InvokeSafe()
        {
            try
            {
                Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /************************************************************************************************************************/
    }
}
