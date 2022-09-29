// UltEvents // Copyright 2021 Kybernetik //

using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltEvents
{
    /// <summary>
    /// Encapsulates a delegate so it can be serialized for <see cref="UltEventBase"/>.
    /// </summary>
    [Serializable]
    public sealed class PersistentCall
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        [SerializeField]
        private Object _Target;

        /// <summary>The object on which the persistent method is called.</summary>
        public Object Target
        {
            get { return _Target; }
        }

        /************************************************************************************************************************/

        [SerializeField]
        private string _MethodName;

        /// <summary>The name of the persistent method.</summary>
        public string MethodName
        {
            get { return _MethodName; }
        }

        /************************************************************************************************************************/

        [SerializeField]
        internal PersistentArgument[] _PersistentArguments = NoArguments;

        /// <summary>The arguments which are passed to the method when it is invoked.</summary>
        public PersistentArgument[] PersistentArguments
        {
            get { return _PersistentArguments; }
        }

        /************************************************************************************************************************/

        [NonSerialized]
        internal MethodBase _Method;

        /// <summary>The method which this call encapsulates.</summary>
        public MethodBase Method
        {
            get
            {
                if (_Method == null)
                {
                    Type declaringType;
                    string methodName;
                    GetMethodDetails(out declaringType, out methodName);
                    if (declaringType == null || string.IsNullOrEmpty(methodName))
                        return null;

                    var argumentCount = _PersistentArguments.Length;
                    var parameters = ArrayCache<Type>.GetTempArray(argumentCount);
                    for (int i = 0; i < argumentCount; i++)
                    {
                        parameters[i] = _PersistentArguments[i].SystemType;
                    }

                    if (methodName == "ctor")
                        _Method = declaringType.GetConstructor(UltEventUtils.AnyAccessBindings, null, parameters, null);
                    else
                        _Method = declaringType.GetMethod(methodName, UltEventUtils.AnyAccessBindings, null, parameters, null);
                }

                return _Method;
            }
        }

        internal MethodBase GetMethodSafe()
        {
            try { return Method; }
            catch { return null; }
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        // Always clear the cached method in the editor in case the fields have been directly modified by Inspector or Undo operations.
        void ISerializationCallbackReceiver.OnBeforeSerialize() { ClearCache(); }
        void ISerializationCallbackReceiver.OnAfterDeserialize() { ClearCache(); }

        private void ClearCache()
        {
            _Method = null;

            for (int i = 0; i < _PersistentArguments.Length; i++)
            {
                _PersistentArguments[i].ClearCache();
            }
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>Constructs a new <see cref="PersistentCall"/> with default values.</summary>
        public PersistentCall() { }

        /// <summary>Constructs a new <see cref="PersistentCall"/> to serialize the specified `method`.</summary>
        public PersistentCall(MethodInfo method, Object target)
        {
            SetMethod(method, target);
        }

        /// <summary>Constructs a new <see cref="PersistentCall"/> to serialize the specified `method`.</summary>
        public PersistentCall(Delegate method)
        {
            SetMethod(method);
        }

        /// <summary>Constructs a new <see cref="PersistentCall"/> to serialize the specified `method`.</summary>
        public PersistentCall(Action method)
        {
            SetMethod(method);
        }

        /************************************************************************************************************************/

        /// <summary>Sets the method which this call encapsulates.</summary>
        public void SetMethod(MethodBase method, Object target)
        {
            _Method = method;
            _Target = target;

            if (method != null)
            {
                if (method.IsStatic || method.IsConstructor)
                {
                    _MethodName = UltEventUtils.GetFullyQualifiedName(method);
                    _Target = null;
                }
                else _MethodName = method.Name;

                var parameters = method.GetParameters();

                if (_PersistentArguments == null || _PersistentArguments.Length != parameters.Length)
                {
                    _PersistentArguments = NewArgumentArray(parameters.Length);
                }

                for (int i = 0; i < _PersistentArguments.Length; i++)
                {
                    var parameter = parameters[i];
                    var persistentArgument = _PersistentArguments[i];

                    persistentArgument.SystemType = parameter.ParameterType;

                    switch (persistentArgument.Type)
                    {
                        case PersistentArgumentType.Parameter:
                        case PersistentArgumentType.ReturnValue:
                            break;
                        default:
                            if ((parameter.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault)
                            {
                                persistentArgument.Value = parameter.DefaultValue;
                            }
                            break;
                    }
                }
            }
            else
            {
                _MethodName = null;
                _PersistentArguments = NoArguments;
            }
        }

        /// <summary>Sets the delegate which this call encapsulates.</summary>
        public void SetMethod(Delegate method)
        {
            if (method.Target == null)
            {
                SetMethod(method.Method, null);
            }
            else
            {
                var target = method.Target as Object;
                if (target != null)
                    SetMethod(method.Method, target);
                else
                    throw new InvalidOperationException("SetMethod failed because action.Target is not a UnityEngine.Object.");
            }
        }

        /// <summary>Sets the delegate which this call encapsulates.</summary>
        public void SetMethod(Action method)
        {
            SetMethod((Delegate)method);
        }

        /************************************************************************************************************************/

        private static readonly PersistentArgument[] NoArguments = new PersistentArgument[0];

        private static PersistentArgument[] NewArgumentArray(int length)
        {
            if (length == 0)
            {
                return NoArguments;
            }
            else
            {
                var array = new PersistentArgument[length];

                for (int i = 0; i < length; i++)
                    array[i] = new PersistentArgument();

                return array;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Acquire a delegate based on the <see cref="Target"/> and <see cref="MethodName"/> and invoke it.
        /// </summary>
        public object Invoke()
        {
            if (Method == null)
            {
                Debug.LogWarning("Attempted to Invoke a PersistentCall which couldn't find it's method: " + MethodName);
                return null;
            }

            object[] parameters;
            if (_PersistentArguments != null && _PersistentArguments.Length > 0)
            {
                parameters = ArrayCache<object>.GetTempArray(_PersistentArguments.Length);
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = _PersistentArguments[i].Value;
                }
            }
            else parameters = null;

            UltEventBase.UpdateLinkedValueOffsets();

#if UNITY_EDITOR
            // Somehow Unity ends up getting a UnityEngine.Object which pretends to be null.
            // But only in the Editor. At runtime it properly deserialized the target as null.

            // When calling a static method it just gets ignored. But when calling a constructor with a target, it
            // attempts to apply it to the existing object, which won't work because it's the wrong type.

            if (_Method.IsConstructor)
                return _Method.Invoke(null, parameters);
#endif

            return _Method.Invoke(_Target, parameters);
        }

        /************************************************************************************************************************/

        /// <summary>Sets the value of the first persistent argument.</summary>
        public void SetArguments(object argument0)
        {
            PersistentArguments[0].Value = argument0;
        }

        /// <summary>Sets the value of the first and second persistent arguments.</summary>
        public void SetArguments(object argument0, object argument1)
        {
            PersistentArguments[0].Value = argument0;
            PersistentArguments[1].Value = argument1;
        }

        /// <summary>Sets the value of the first, second, and third persistent arguments.</summary>
        public void SetArguments(object argument0, object argument1, object argument2)
        {
            PersistentArguments[0].Value = argument0;
            PersistentArguments[1].Value = argument1;
            PersistentArguments[2].Value = argument2;
        }

        /// <summary>Sets the value of the first, second, third, and fourth persistent arguments.</summary>
        public void SetArguments(object argument0, object argument1, object argument2, object argument3)
        {
            PersistentArguments[0].Value = argument0;
            PersistentArguments[1].Value = argument1;
            PersistentArguments[2].Value = argument2;
            PersistentArguments[3].Value = argument3;
        }

        /************************************************************************************************************************/

        internal void GetMethodDetails(out Type declaringType, out string methodName)
        {
#if UNITY_EDITOR
            // If you think this looks retarded, that's because it is.

            // Sometimes Unity ends up with an old reference to an object where the reference thinks it has been
            // destroyed even though it hasn't and it still has a value Instance ID. So we just get a new reference.

            if (_Target == null && !ReferenceEquals(_Target, null))
                _Target = UnityEditor.EditorUtility.InstanceIDToObject(_Target.GetInstanceID());
#endif

            GetMethodDetails(_MethodName, _Target, out declaringType, out methodName);
        }

        internal static void GetMethodDetails(string serializedMethodName, Object target, out Type declaringType, out string methodName)
        {
            if (string.IsNullOrEmpty(serializedMethodName))
            {
                declaringType = null;
                methodName = null;
                return;
            }

            if (target == null)
            {
                var lastDot = serializedMethodName.LastIndexOf('.');
                if (lastDot < 0)
                {
                    declaringType = null;
                    methodName = serializedMethodName;
                }
                else
                {
                    declaringType = Type.GetType(serializedMethodName.Substring(0, lastDot));
                    lastDot++;
                    methodName = serializedMethodName.Substring(lastDot, serializedMethodName.Length - lastDot);
                }
            }
            else
            {
                declaringType = target.GetType();
                methodName = serializedMethodName;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns true if the specified `type` can be represented by a non-linked <see cref="PersistentArgument"/>.
        /// </summary>
        public static bool IsSupportedNative(Type type)
        {
            return
                type == typeof(bool) ||
                type == typeof(string) ||
                type == typeof(int) ||
                (type.IsEnum && Enum.GetUnderlyingType(type) == typeof(int)) ||
                type == typeof(float) ||
                type == typeof(Vector2) ||
                type == typeof(Vector3) ||
                type == typeof(Vector4) ||
                type == typeof(Quaternion) ||
                type == typeof(Color) ||
                type == typeof(Color32) ||
                type == typeof(Rect) ||
                type == typeof(Object) || type.IsSubclassOf(typeof(Object));
        }

        /// <summary>
        /// Returns true if the type of each of the `parameters` can be represented by a non-linked <see cref="PersistentArgument"/>.
        /// </summary>
        public static bool IsSupportedNative(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (!IsSupportedNative(parameters[i].ParameterType))
                    return false;
            }

            return true;
        }

        /************************************************************************************************************************/

        /// <summary>Copies the contents of the `target` call to this call.</summary>
        public void CopyFrom(PersistentCall target)
        {
            _Target = target._Target;
            _MethodName = target._MethodName;
            _Method = target._Method;

            _PersistentArguments = new PersistentArgument[target._PersistentArguments.Length];
            for (int i = 0; i < _PersistentArguments.Length; i++)
            {
                _PersistentArguments[i] = target._PersistentArguments[i].Clone();
            }
        }

        /************************************************************************************************************************/

        /// <summary>Returns a description of this call.</summary>
        public override string ToString()
        {
            var text = new StringBuilder();
            ToString(text);
            return text.ToString();
        }

        /// <summary>Appends a description of this call.</summary>
        public void ToString(StringBuilder text)
        {
            text.Append("PersistentCall: MethodName=");
            text.Append(_MethodName);
            text.Append(", Target=");
            text.Append(_Target != null ? _Target.ToString() : "null");
            text.Append(", PersistentArguments=");
            UltEventUtils.AppendDeepToString(text, _PersistentArguments.GetEnumerator(), "\n        ");
        }

        /************************************************************************************************************************/
    }
}
