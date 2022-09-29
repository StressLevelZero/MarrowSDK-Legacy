// UltEvents // Copyright 2021 Kybernetik //

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UltEvents
{
    /// <summary>Various utility methods used by <see cref="UltEvents"/>.</summary>
    public static class UltEventUtils
    {
        /************************************************************************************************************************/

        /// <summary>The sub-menu which all <see cref="UltEvents"/> components are listed in.</summary>
        public const string ComponentMenuPrefix = "UltEvents/";

        /// <summary>The address of the online documentation.</summary>
        public const string DocumentationURL = "https://kybernetik.com.au/ultevents";

        /// <summary>The address of the API documentation.</summary>
        public const string APIDocumentationURL = DocumentationURL + "/api/UltEvents";

        /************************************************************************************************************************/
        #region Event Extensions
        /************************************************************************************************************************/

        /// <summary>
        /// Calls e.Invoke if it isn't null.
        /// <para></para>
        /// See also: <seealso cref="UltEvent.Invoke"/> and <seealso cref="UltEvent.InvokeSafe"/>.
        /// </summary>
        public static void InvokeX(this UltEvent e)
        {
            if (e != null)
                e.Invoke();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls e.Invoke if it isn't null.
        /// <para></para>
        /// See also: <seealso cref="UltEvent{T0}.Invoke"/> and <seealso cref="UltEvent{T0}.InvokeSafe"/>.
        /// </summary>
        public static void InvokeX<T0>(this UltEvent<T0> e, T0 parameter0)
        {
            if (e != null)
                e.Invoke(parameter0);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls e.Invoke if it isn't null.
        /// <para></para>
        /// See also: <seealso cref="UltEvent{T0, T1}.Invoke"/> and <seealso cref="UltEvent{T0, T1}.InvokeSafe"/>.
        /// </summary>
        public static void InvokeX<T0, T1>(this UltEvent<T0, T1> e, T0 parameter0, T1 parameter1)
        {
            if (e != null)
                e.Invoke(parameter0, parameter1);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls e.Invoke if it isn't null.
        /// <para></para>
        /// See also: <seealso cref="UltEvent{T0, T1, T2}.Invoke"/> and <seealso cref="UltEvent{T0, T1, T2}.InvokeSafe"/>.
        /// </summary>
        public static void InvokeX<T0, T1, T2>(this UltEvent<T0, T1, T2> e, T0 parameter0, T1 parameter1, T2 parameter2)
        {
            if (e != null)
                e.Invoke(parameter0, parameter1, parameter2);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls e.Invoke if it isn't null.
        /// <para></para>
        /// See also: <seealso cref="UltEvent{T0, T1, T2, T3}.Invoke"/> and <seealso cref="UltEvent{T0, T1, T2, T3}.InvokeSafe"/>.
        /// </summary>
        public static void InvokeX<T0, T1, T2, T3>(this UltEvent<T0, T1, T2, T3> e, T0 parameter0, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (e != null)
                e.Invoke(parameter0, parameter1, parameter2, parameter3);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Type Names
        /************************************************************************************************************************/

        private static readonly Dictionary<Type, string>
            TypeNames = new Dictionary<Type, string>
            {
                { typeof(object), "object" },
                { typeof(void), "void" },
                { typeof(bool), "bool" },
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(short), "short" },
                { typeof(int), "int" },
                { typeof(long), "long" },
                { typeof(ushort), "ushort" },
                { typeof(uint), "uint" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
            };

        private static readonly Dictionary<Type, string>
            FullTypeNames = new Dictionary<Type, string>(TypeNames);

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the name of a `type` as it would appear in C# code.
        /// <para></para>
        /// For example, typeof(List&lt;float&gt;).FullName would give you:
        /// System.Collections.Generic.List`1[[System.Single, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
        /// <para></para>
        /// This method would instead return System.Collections.Generic.List&lt;float&gt; if `fullName` is true, or
        /// just List&lt;float&gt; if it is false.
        /// <para></para>
        /// Note that all returned values are stored in a dictionary to speed up repeated use.
        /// </summary>
        public static string GetNameCS(this Type type, bool fullName = true)
        {
            if (type == null)
                return "";

            // Check if we have already got the name for that type.
            var names = fullName ? FullTypeNames : TypeNames;
            string name;
            if (names.TryGetValue(type, out name))
                return name;

            var text = new StringBuilder();

            if (type.IsArray)// Array = TypeName[].
            {
                text.Append(type.GetElementType().GetNameCS(fullName));

                text.Append('[');
                var dimensions = type.GetArrayRank();
                while (dimensions-- > 1)
                    text.Append(",");
                text.Append(']');

                goto Return;
            }

            if (type.IsPointer)// Pointer = TypeName*.
            {
                text.Append(type.GetElementType().GetNameCS(fullName));
                text.Append('*');

                goto Return;
            }

            if (type.IsGenericParameter)// Generic Parameter = TypeName (for unspecified generic parameters).
            {
                text.Append(type.Name);
                goto Return;
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)// Nullable = TypeName?.
            {
                text.Append(underlyingType.GetNameCS(fullName));
                text.Append('?');

                goto Return;
            }

            // Other Type = Namespace.NestedTypes.TypeName<GenericArguments>.

            if (fullName && type.Namespace != null)// Namespace.
            {
                text.Append(type.Namespace);
                text.Append('.');
            }

            var genericArguments = 0;

            if (type.DeclaringType != null)// Account for Nested Types.
            {
                // Count the nesting level.
                var nesting = 1;
                var declaringType = type.DeclaringType;
                while (declaringType.DeclaringType != null)
                {
                    declaringType = declaringType.DeclaringType;
                    nesting++;
                }

                // Append the name of each outer type, starting from the outside.
                while (nesting-- > 0)
                {
                    // Walk out to the current nesting level.
                    // This avoids the need to make a list of types in the nest or to insert type names instead of appending them.
                    declaringType = type;
                    for (int i = nesting; i >= 0; i--)
                        declaringType = declaringType.DeclaringType;

                    // Nested Type Name.
                    genericArguments = AppendNameAndGenericArguments(text, declaringType, fullName, genericArguments);
                    text.Append('.');
                }
            }

            // Type Name.
            AppendNameAndGenericArguments(text, type, fullName, genericArguments);

        Return:// Remember and return the name.
            name = text.ToString();
            names.Add(type, name);
            return name;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Appends the generic arguments of `type` (after skipping the specified number).
        /// </summary>
        public static int AppendNameAndGenericArguments(StringBuilder text, Type type, bool fullName = true, int skipGenericArguments = 0)
        {
            text.Append(type.Name);

            if (type.IsGenericType)
            {
                var backQuote = type.Name.IndexOf('`');
                if (backQuote >= 0)
                {
                    text.Length -= type.Name.Length - backQuote;

                    var genericArguments = type.GetGenericArguments();
                    if (skipGenericArguments < genericArguments.Length)
                    {
                        text.Append('<');

                        var firstArgument = genericArguments[skipGenericArguments];
                        skipGenericArguments++;

                        if (firstArgument.IsGenericParameter)
                        {
                            while (skipGenericArguments < genericArguments.Length)
                            {
                                text.Append(',');
                                skipGenericArguments++;
                            }
                        }
                        else
                        {
                            text.Append(firstArgument.GetNameCS(fullName));

                            while (skipGenericArguments < genericArguments.Length)
                            {
                                text.Append(", ");
                                text.Append(genericArguments[skipGenericArguments].GetNameCS(fullName));
                                skipGenericArguments++;
                            }
                        }

                        text.Append('>');
                    }
                }
            }

            return skipGenericArguments;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Member Names
        /************************************************************************************************************************/

        /// <summary>
        /// Returns the full name of a `member` as it would appear in C# code.
        /// <para></para>
        /// For example, passing this method info in as its own parameter would return "<see cref="UltEventUtils"/>.GetNameCS".
        /// <para></para>
        /// Note that when `member` is a <see cref="Type"/>, this method calls <see cref="GetNameCS(Type, bool)"/> instead.
        /// </summary>
        public static string GetNameCS(this MemberInfo member, bool fullName = true)
        {
            if (member == null)
                return "null";

            var type = member as Type;
            if (type != null)
                return type.GetNameCS(fullName);

            var text = new StringBuilder();

            if (member.DeclaringType != null)
            {
                text.Append(member.DeclaringType.GetNameCS(fullName));
                text.Append('.');
            }

            text.Append(member.Name);

            return text.ToString();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Appends the full name of a `member` as it would appear in C# code.
        /// <para></para>
        /// For example, passing this method info in as its own parameter would append "<see cref="UltEventUtils"/>.AppendName".
        /// <para></para>
        /// Note that when `member` is a <see cref="Type"/>, this method calls <see cref="GetNameCS(Type, bool)"/> instead.
        /// </summary>
        public static StringBuilder AppendNameCS(this StringBuilder text, MemberInfo member, bool fullName = true)
        {
            if (member == null)
            {
                text.Append("null");
                return text;
            }

            var type = member as Type;
            if (type != null)
            {
                text.Append(type.GetNameCS(fullName));
                return text;
            }

            if (member.DeclaringType != null)
            {
                text.Append(member.DeclaringType.GetNameCS(fullName));
                text.Append('.');
            }

            text.Append(member.Name);

            return text;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Deep to String
        /************************************************************************************************************************/

        /// <summary>Returns a string containing the value of each element in `collection`.</summary>
        public static string DeepToString(this IEnumerable collection, string separator)
        {
            if (collection == null)
                return "null";
            else
                return collection.GetEnumerator().DeepToString(separator);
        }

        /// <summary>Returns a string containing the value of each element in `collection` (each on a new line).</summary>
        public static string DeepToString(this IEnumerable collection)
        {
            return collection.DeepToString(Environment.NewLine);
        }

        /************************************************************************************************************************/

        /// <summary>Each element returned by `enumerator` is appended to `text`.</summary>
        public static void AppendDeepToString(StringBuilder text, IEnumerator enumerator, string separator)
        {
            text.Append("[]");
            var countIndex = text.Length - 1;
            var count = 0;

            if (enumerator != null)
            {
                while (enumerator.MoveNext())
                {
                    text.Append(separator);
                    text.Append('[');
                    text.Append(count);
                    text.Append("] = ");
                    text.Append(enumerator.Current);

                    count++;
                }
            }

            text.Insert(countIndex, count);
        }

        /// <summary>Returns a string containing the value of each element in `enumerator`.</summary>
        public static string DeepToString(this IEnumerator enumerator, string separator)
        {
            var text = new StringBuilder();
            AppendDeepToString(text, enumerator, separator);
            return text.ToString();
        }

        /// <summary>Returns a string containing the value of each element in `enumerator` (each on a new line).</summary>
        public static string DeepToString(this IEnumerator enumerator)
        {
            return enumerator.DeepToString(Environment.NewLine);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>Commonly used <see cref="BindingFlags"/>.</summary>
        public const BindingFlags
            AnyAccessBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
            InstanceBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            StaticBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the <see cref="MemberInfo.DeclaringType"/> for constructors or <see cref="MethodInfo.ReturnType"/>
        /// for regular methods.
        /// </summary>
        public static Type GetReturnType(this MethodBase method)
        {
            return method.IsConstructor ? method.DeclaringType : (method as MethodInfo).ReturnType;
        }

        /************************************************************************************************************************/

        /// <summary>Returns "AssemblyQualifiedName.MethodName".</summary>
        public static string GetFullyQualifiedName(MethodBase method)
        {
            return method.DeclaringType.AssemblyQualifiedName + "." + method.Name;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calculate the number of removals, inserts, and replacements needed to turn `a` into `b`.
        /// </summary>
        public static int CalculateLevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a))
            {
                return string.IsNullOrEmpty(b) ? 0 : b.Length;
            }

            if (string.IsNullOrEmpty(b))
                return a.Length;

            var n = a.Length;
            var m = b.Length;
            var d = new int[n + 1, m + 1];

            // initialise the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
                // Execution is contained in the For statement.
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
                // Execution is contained in the For statement.
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    var cost = (b[j - 1] == a[i - 1]) ? 0 : 1;

                    d[i, j] = Mathf.Min(
                        Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Sorts `list`, maintaining the order of any elements with an identical comparison
        /// (unlike the standard <see cref="List{T}.Sort(Comparison{T})"/> method).
        /// </summary>
        public static void StableInsertionSort<T>(IList<T> list, Comparison<T> comparison)
        {
            var count = list.Count;
            for (int j = 1; j < count; j++)
            {
                var key = list[j];

                var i = j - 1;
                for (; i >= 0 && comparison(list[i], key) > 0; i--)
                {
                    list[i + 1] = list[i];
                }
                list[i + 1] = key;
            }
        }

        /// <summary>
        /// Sorts `list`, maintaining the order of any elements with an identical comparison
        /// (unlike the standard <see cref="List{T}.Sort()"/> method).
        /// </summary>
        public static void StableInsertionSort<T>(IList<T> list) where T : IComparable<T>
        {
            StableInsertionSort(list, (a, b) => a.CompareTo(b));
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Translates a zero based index to a placement name: 0 = "1st", 1 = "2nd", etc.
        /// </summary>
        public static string GetPlacementName(int index)
        {
            switch (index)
            {
                case 0: return "1st";
                case 1: return "2nd";
                case 2: return "3rd";
                default: return index + "th";
            }
        }

        /************************************************************************************************************************/
    }
}
