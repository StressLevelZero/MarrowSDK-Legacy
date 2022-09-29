// UltEvents // Copyright 2021 Kybernetik //

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltEvents
{
    /// <summary>The type identifier of a <see cref="PersistentArgument"/>.</summary>
    public enum PersistentArgumentType
    {
        /// <summary>Type not set.</summary>
        None,

        // Uses _Int 0 or 1 as bool.
        /// <summary><see cref="bool"/>.</summary>
        Bool,

        // Uses _String.
        /// <summary><see cref="string"/>.</summary>
        String,

        // Uses _Int.
        /// <summary><see cref="int"/>.</summary>
        Int,

        // Uses _Int for the value and _String for the assembly qualified name of the type.
        /// <summary>Any kind of <see cref="System.Enum"/>.</summary>
        Enum,

        // Uses _X.
        /// <summary><see cref="float"/>.</summary>
        Float,

        // Uses _X and _Y.
        /// <summary><see cref="UnityEngine.Vector2"/>.</summary>
        Vector2,

        // Uses _X, _Y, and _Z.
        /// <summary><see cref="UnityEngine.Vector3"/>.</summary>
        Vector3,

        // Uses _X, _Y, _Z, and _W.
        /// <summary><see cref="UnityEngine.Vector4"/>.</summary>
        Vector4,

        // Uses _X, _Y, and _Z to store the euler angles.
        /// <summary><see cref="UnityEngine.Quaternion"/>.</summary>
        Quaternion,

        // Uses _X, _Y, _Z, and _W as RGBA.
        /// <summary><see cref="UnityEngine.Color"/>.</summary>
        Color,

        // Uses _Int to hold the RGBA bytes.
        /// <summary><see cref="UnityEngine.Color32"/>.</summary>
        Color32,

        // Uses _X, _Y, _Z, and _W as X, Y, Width, Height.
        /// <summary><see cref="UnityEngine.Rect"/>.</summary>
        Rect,

        // Uses _Object for the value and _String for the assembly qualified name of the type.
        /// <summary><see cref="UnityEngine.Object"/>.</summary>
        Object,

        // Uses _Int for the index of the target parameter.
        // If the type is a simple PersistentArgumentType (not Object or Enum), it is casted to a float and stored in _X.
        // Otherwise the assembly qualified name of the type is stored in _String.
        /// <summary>The value of a parameter passed to the event.</summary>
        Parameter,

        // Uses _Int for the index of the target call.
        // If the type is a simple PersistentArgumentType (not Object or Enum), it is casted to a float and stored in _X.
        // Otherwise the assembly qualified name of the type is stored in _String.
        /// <summary>The return value by a previous <see cref="PersistentCall"/>.</summary>
        ReturnValue,
    }

    /// <summary>
    /// Encapsulates a variable so it can be serialized for <see cref="UltEventBase"/>.
    /// </summary>
    [Serializable]
    public sealed class PersistentArgument
    {
        /************************************************************************************************************************/
        #region Fields
        /************************************************************************************************************************/

        [SerializeField]
        internal PersistentArgumentType _Type;

        [SerializeField]
        internal int _Int;

        [SerializeField]
        internal string _String;

        [SerializeField]
        internal float _X, _Y, _Z, _W;

        [SerializeField]
        internal Object _Object;

        /************************************************************************************************************************/

        [NonSerialized]
        private Type _SystemType;

        [NonSerialized]
        internal bool _HasSystemType;

        [NonSerialized]
        private object _Value;

        /************************************************************************************************************************/

        /// <summary>Constructs a new <see cref="PersistentArgument"/> with default values.</summary>
        public PersistentArgument() { }

        /// <summary>Constructs a new <see cref="PersistentArgument"/> with the specified `type`.</summary>
        public PersistentArgument(Type type)
        {
            _Type = GetArgumentType(type, out _String, out _Int);
            _SystemType = type;
            _HasSystemType = true;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Properties
        /************************************************************************************************************************/

        /// <summary>The type identifier of this argument.</summary>
        public PersistentArgumentType Type
        {
            get { return _Type; }
            internal set
            {
                _Int = 0;
                _X = _Y = _Z = _W = 0;
                _String = "";
                _Object = null;
                _Type = value;
                _HasSystemType = false;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The <see cref="System.Type"/> of this argument.
        /// </summary>
        public Type SystemType
        {
            get
            {
#if !UNITY_EDITOR// Ignore cache in the editor since it complicates the inspector GUI code.
                if (!_HasSystemType)
#endif
                {
                    _SystemType = GetArgumentType(_Type, _X, _String);
                    _HasSystemType = true;
                }

                return _SystemType;
            }
            internal set
            {
                // Can't pass _String and _Int in directly because setting the Type clears them.
                string assemblyQualifiedName;
                int linkIndex;
                Type = GetArgumentType(value, out assemblyQualifiedName, out linkIndex);
                _String = assemblyQualifiedName;
                _Int = linkIndex;
                _HasSystemType = true;
                _SystemType = value;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="bool"/> value of this argument.</summary>
        public bool Bool
        {
            get
            {
                AssertType(PersistentArgumentType.Bool);
                return _Int != 0;
            }
            set
            {
                AssertType(PersistentArgumentType.Bool);
                _Int = value ? 1 : 0;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="string"/> value of this argument.</summary>
        public string String
        {
            get
            {
                AssertType(PersistentArgumentType.String);
                return _String;
            }
            set
            {
                AssertType(PersistentArgumentType.String);
                _String = value ?? "";
                _Value = value;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="int"/> value of this argument.</summary>
        public int Int
        {
            get
            {
                AssertType(PersistentArgumentType.Int);
                return _Int;
            }
            set
            {
                AssertType(PersistentArgumentType.Int);
                _Int = value;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="System.Enum"/> value of this argument.</summary>
        public object Enum
        {
            get
            {
                AssertType(PersistentArgumentType.Enum);
                return System.Enum.ToObject(SystemType, _Int);
            }
            set
            {
                AssertType(PersistentArgumentType.Enum);
                _Int = (int)value;
                _Value = value;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="float"/> value of this argument.</summary>
        public float Float
        {
            get
            {
                AssertType(PersistentArgumentType.Float);
                return _X;
            }
            set
            {
                AssertType(PersistentArgumentType.Float);
                _X = value;
                _Value = null;
            }
        }

        /// <summary>The <see cref="UnityEngine.Vector2"/> value of this argument.</summary>
        public Vector2 Vector2
        {
            get
            {
                AssertType(PersistentArgumentType.Vector2);
                return new Vector2(_X, _Y);
            }
            set
            {
                AssertType(PersistentArgumentType.Vector2);
                _X = value.x;
                _Y = value.y;
                _Value = null;
            }
        }

        /// <summary>The <see cref="UnityEngine.Vector3"/> value of this argument.</summary>
        public Vector3 Vector3
        {
            get
            {
                AssertType(PersistentArgumentType.Vector3);
                return new Vector3(_X, _Y, _Z);
            }
            set
            {
                AssertType(PersistentArgumentType.Vector3);
                _X = value.x;
                _Y = value.y;
                _Z = value.z;
                _Value = null;
            }
        }

        /// <summary>The <see cref="UnityEngine.Vector4"/> value of this argument.</summary>
        public Vector4 Vector4
        {
            get
            {
                AssertType(PersistentArgumentType.Vector4);
                return new Vector4(_X, _Y, _Z, _W);
            }
            set
            {
                AssertType(PersistentArgumentType.Vector4);
                _X = value.x;
                _Y = value.y;
                _Z = value.z;
                _W = value.w;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="UnityEngine.Quaternion"/> value of this argument.</summary>
        public Quaternion Quaternion
        {
            // It would be better to store the components of the Quaternion directly instead of the euler angles,
            // but floating point imprecision when converting between them to show the euler angles in the inspector
            // means that changes to any one axis will have a small effect on the other axes as well.

            // This could be handled like the [Euler] attribute, but that still leads to small inaccuracies in the
            // displayed values when they are deserialized so storing the euler angles directly is more user-friendly.

            get
            {
                AssertType(PersistentArgumentType.Quaternion);
                return Quaternion.Euler(_X, _Y, _Z);
            }
            set
            {
                AssertType(PersistentArgumentType.Quaternion);
                var euler = value.eulerAngles;
                _X = euler.x;
                _Y = euler.y;
                _Z = euler.z;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="UnityEngine.Color"/> value of this argument.</summary>
        public Color Color
        {
            get
            {
                AssertType(PersistentArgumentType.Color);
                return new Color(_X, _Y, _Z, _W);
            }
            set
            {
                AssertType(PersistentArgumentType.Color);
                _X = value.r;
                _Y = value.g;
                _Z = value.b;
                _W = value.a;
                _Value = null;
            }
        }

        /// <summary>The <see cref="UnityEngine.Color32"/> value of this argument.</summary>
        public Color32 Color32
        {
            get
            {
                AssertType(PersistentArgumentType.Color32);
                return new Color32((byte)(_Int), (byte)(_Int >> 8), (byte)(_Int >> 16), (byte)(_Int >> 24));
            }
            set
            {
                AssertType(PersistentArgumentType.Color32);
                _Int = value.r | (value.g << 8) | (value.b << 16) | (value.a << 24);
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="UnityEngine.Rect"/> value of this argument.</summary>
        public Rect Rect
        {
            get
            {
                AssertType(PersistentArgumentType.Rect);
                return new Rect(_X, _Y, _Z, _W);
            }
            set
            {
                AssertType(PersistentArgumentType.Rect);
                _X = value.x;
                _Y = value.y;
                _Z = value.width;
                _W = value.height;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="UnityEngine.Object"/> value of this argument.</summary>
        public Object Object
        {
            get
            {
                AssertType(PersistentArgumentType.Object);

                // Unity's fake nulls cause problems if the argument is a child type of UnityEngine.Object.
                // For example, when invoking a MonoBehaviour parameter a fake null Object would fail to convert to MonoBehaviour.
                // So we make sure to return actual null instead of any fake value.

                if (_Object == null)
                    return null;
                else
                    return _Object;
            }
            set
            {
                AssertType(PersistentArgumentType.Object);
                _Object = value;
                _String = value != null ? value.GetType().AssemblyQualifiedName : "";
                _Value = value;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The value of a parameter passed into the <see cref="PersistentCall"/> (see <see cref="ParameterIndex"/>.</summary>
        public object Parameter
        {
            get
            {
                AssertType(PersistentArgumentType.Parameter);
                return UltEventBase.GetParameterValue(_Int);
            }
        }

        /// <summary>The index of the parameter passed into the <see cref="PersistentCall"/>.</summary>
        public int ParameterIndex
        {
            get
            {
                AssertType(PersistentArgumentType.Parameter);
                return _Int;
            }
            set
            {
                AssertType(PersistentArgumentType.Parameter);
                _Int = value;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The value returned by a previous <see cref="PersistentCall"/> (see <see cref="ReturnedValueIndex"/>.</summary>
        public object ReturnedValue
        {
            get
            {
                AssertType(PersistentArgumentType.ReturnValue);
                return UltEventBase.GetReturnedValue(_Int);
            }
        }

        /// <summary>The index of the <see cref="PersistentCall"/> which returns the value for this argument.</summary>
        public int ReturnedValueIndex
        {
            get
            {
                AssertType(PersistentArgumentType.ReturnValue);
                return _Int;
            }
            set
            {
                AssertType(PersistentArgumentType.ReturnValue);
                _Int = value;
                _Value = null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The value of this argument.</summary>
        public object Value
        {
            get
            {
                if (_Value == null)
                {
                    switch (_Type)
                    {
                        case PersistentArgumentType.Bool: _Value = Bool; break;
                        case PersistentArgumentType.String: _Value = String; break;
                        case PersistentArgumentType.Int: _Value = Int; break;
                        case PersistentArgumentType.Enum: _Value = Enum; break;
                        case PersistentArgumentType.Float: _Value = Float; break;
                        case PersistentArgumentType.Vector2: _Value = Vector2; break;
                        case PersistentArgumentType.Vector3: _Value = Vector3; break;
                        case PersistentArgumentType.Vector4: _Value = Vector4; break;
                        case PersistentArgumentType.Quaternion: _Value = Quaternion; break;
                        case PersistentArgumentType.Color: _Value = Color; break;
                        case PersistentArgumentType.Color32: _Value = Color32; break;
                        case PersistentArgumentType.Rect: _Value = Rect; break;
                        case PersistentArgumentType.Object: _Value = Object; break;

                        // Don't cache parameters or returned values.
                        case PersistentArgumentType.Parameter: return Parameter;
                        case PersistentArgumentType.ReturnValue: return ReturnedValue;

                        default:
                            throw new InvalidOperationException(
                       "Invalid " + Names.PersistentArgument.Full.Type + ": " + _Type);
                    }
                }

                return _Value;
            }
            set
            {
                switch (_Type)
                {
                    case PersistentArgumentType.Bool: Bool = (bool)value; break;
                    case PersistentArgumentType.String: String = (string)value; break;
                    case PersistentArgumentType.Int: Int = (int)value; break;
                    case PersistentArgumentType.Enum: Enum = value; break;
                    case PersistentArgumentType.Float: Float = (float)value; break;
                    case PersistentArgumentType.Vector2: Vector2 = (Vector2)value; break;
                    case PersistentArgumentType.Vector3: Vector3 = (Vector3)value; break;
                    case PersistentArgumentType.Vector4: Vector4 = (Vector4)value; break;
                    case PersistentArgumentType.Quaternion: Quaternion = (Quaternion)value; break;
                    case PersistentArgumentType.Color: Color = (Color)value; break;
                    case PersistentArgumentType.Color32: Color32 = (Color32)value; break;
                    case PersistentArgumentType.Rect: Rect = (Rect)value; break;
                    case PersistentArgumentType.Object: Object = (Object)value; break;

                    // Don't cache parameters or returned values.
                    case PersistentArgumentType.Parameter: ParameterIndex = (int)value; return;
                    case PersistentArgumentType.ReturnValue: ReturnedValueIndex = (int)value; return;

                    default:
                        throw new InvalidOperationException(
                   "Invalid " + Names.PersistentArgument.Full.Type + ": " + _Type);
                }

                _Value = value;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Methods
        /************************************************************************************************************************/

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AssertType(PersistentArgumentType type)
        {
            if (_Type != type)
                throw new InvalidOperationException(Names.PersistentArgument.Full.Type + " is " + _Type + " but should be " + type);
        }

        /************************************************************************************************************************/

#if UNITY_EDITOR
        internal void ClearCache()
        {
            _Value = null;
        }
#endif

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the <see cref="System.Type"/> associated with the specified <see cref="PersistentArgumentType"/>.
        /// <para></para>
        /// If the `type` can be inherited (such as an Enum or Object), the `assemblyQualifiedName` will be used to get the type.
        /// </summary>
        public static Type GetArgumentType(PersistentArgumentType type, float secondaryType, string assemblyQualifiedName)
        {
            switch (type)
            {
                case PersistentArgumentType.Bool: return typeof(bool);
                case PersistentArgumentType.String: return typeof(string);
                case PersistentArgumentType.Int: return typeof(int);
                case PersistentArgumentType.Float: return typeof(float);
                case PersistentArgumentType.Vector2: return typeof(Vector2);
                case PersistentArgumentType.Vector3: return typeof(Vector3);
                case PersistentArgumentType.Vector4: return typeof(Vector4);
                case PersistentArgumentType.Quaternion: return typeof(Quaternion);
                case PersistentArgumentType.Color: return typeof(Color);
                case PersistentArgumentType.Color32: return typeof(Color32);
                case PersistentArgumentType.Rect: return typeof(Rect);

                case PersistentArgumentType.Enum:
                case PersistentArgumentType.Object:
                default:
                    if (!string.IsNullOrEmpty(assemblyQualifiedName))
                        return System.Type.GetType(assemblyQualifiedName);
                    else
                        return null;

                case PersistentArgumentType.Parameter:
                case PersistentArgumentType.ReturnValue:
                    if (!string.IsNullOrEmpty(assemblyQualifiedName))
                        return System.Type.GetType(assemblyQualifiedName);
                    else
                        return GetArgumentType((PersistentArgumentType)secondaryType, -1, null);

                case PersistentArgumentType.None:
                    return null;
            }
        }

        /// <summary>
        /// Returns the <see cref="PersistentArgumentType"/> associated with the specified <see cref="System.Type"/>.
        /// <para></para>
        /// If the `type` can be inherited (such as an Enum or Object), the `assemblyQualifiedName` will be assigned as well (otherwise null).
        /// </summary>
        public static PersistentArgumentType GetArgumentType(Type type, out string assemblyQualifiedName, out int linkIndex)
        {
            linkIndex = 0;
            assemblyQualifiedName = null;

            if (type == typeof(bool)) return PersistentArgumentType.Bool;
            else if (type == typeof(string)) return PersistentArgumentType.String;
            else if (type == typeof(int)) return PersistentArgumentType.Int;
            else if (type == typeof(float)) return PersistentArgumentType.Float;
            else if (type == typeof(Vector2)) return PersistentArgumentType.Vector2;
            else if (type == typeof(Vector3)) return PersistentArgumentType.Vector3;
            else if (type == typeof(Vector4)) return PersistentArgumentType.Vector4;
            else if (type == typeof(Quaternion)) return PersistentArgumentType.Quaternion;
            else if (type == typeof(Color)) return PersistentArgumentType.Color;
            else if (type == typeof(Color32)) return PersistentArgumentType.Color32;
            else if (type == typeof(Rect)) return PersistentArgumentType.Rect;
            else if (type.IsEnum)
            {
                if (System.Enum.GetUnderlyingType(type) == typeof(int))
                {
                    assemblyQualifiedName = type.AssemblyQualifiedName;
                    return PersistentArgumentType.Enum;
                }
                else return PersistentArgumentType.None;
            }
            else if (type == typeof(Object) || type.IsSubclassOf(typeof(Object)))
            {
                assemblyQualifiedName = type.AssemblyQualifiedName;
                return PersistentArgumentType.Object;
            }
            else
            {
                assemblyQualifiedName = type.AssemblyQualifiedName;

#if UNITY_EDITOR
                PersistentArgumentType linkType;
                if (Editor.DrawerState.Current.TryGetLinkable(type, out linkIndex, out linkType))
                    return linkType;
#endif

                return PersistentArgumentType.ReturnValue;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Creates an exact copy of this argument.</summary>
        public PersistentArgument Clone()
        {
#pragma warning disable IDE0017 // Simplify object initialization
            var clone = new PersistentArgument();
#pragma warning restore IDE0017 // Simplify object initialization

            clone._Type = _Type;
            clone._Int = _Int;
            clone._String = _String;
            clone._X = _X;
            clone._Y = _Y;
            clone._Z = _Z;
            clone._W = _W;
            clone._Object = _Object;
            clone._SystemType = _SystemType;
            clone._HasSystemType = _HasSystemType;
            clone._Value = _Value;

            return clone;
        }

        /************************************************************************************************************************/

        /// <summary>Returns a string which describes this argument.</summary>
        public override string ToString()
        {
            switch (_Type)
            {
                case PersistentArgumentType.None:
                    return Names.PersistentArgument.Class + ": Type=None";
                case PersistentArgumentType.Bool:
                case PersistentArgumentType.String:
                case PersistentArgumentType.Int:
                case PersistentArgumentType.Enum:
                case PersistentArgumentType.Float:
                case PersistentArgumentType.Vector2:
                case PersistentArgumentType.Vector3:
                case PersistentArgumentType.Vector4:
                case PersistentArgumentType.Quaternion:
                case PersistentArgumentType.Color:
                case PersistentArgumentType.Color32:
                case PersistentArgumentType.Rect:
                case PersistentArgumentType.Object:
                    return Names.PersistentArgument.Class + ": SystemType=" + SystemType + ", Value=" + Value;
                case PersistentArgumentType.Parameter:
                    return Names.PersistentArgument.Class + ": SystemType=" + SystemType + ", Value=Parameter" + ParameterIndex;
                case PersistentArgumentType.ReturnValue:
                    return Names.PersistentArgument.Class + ": SystemType=" + SystemType + ", Value=ReturnValue" + ReturnedValueIndex;
                default:
                    Debug.LogWarning("Unhandled " + Names.PersistentArgumentType);
                    return base.ToString();
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}
