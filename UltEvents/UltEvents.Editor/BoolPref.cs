// UltEvents // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace UltEvents.Editor
{
    /// <summary>[Editor-Only]
    /// A simple wrapper around <see cref="EditorPrefs"/> to get and set a bool.
    /// <para></para>
    /// If you are interested in a more comprehensive pref wrapper that supports more types, you should check out
    /// <see href="https://kybernetik.com.au/inspector-gadgets">Inspector Gadgets</see>.
    /// </summary>
    public sealed class BoolPref
    {
        /************************************************************************************************************************/

        /// <summary>The text that will be displayed for this item in a context menu.</summary>
        public readonly string Label;

        /// <summary>The identifier with which this pref will be saved.</summary>
        public readonly string Key;

        /// <summary>The starting value to use for this pref if none was previously saved.</summary>
        public readonly bool DefaultValue;

        /// <summary>Called when the value is changed.</summary>
        public readonly Action OnChanged;

        /************************************************************************************************************************/

        private bool _HasValue;
        private bool _Value;

        /// <summary>The current value of this pref.</summary>
        public bool Value
        {
            get
            {
                if (!_HasValue)
                {
                    _HasValue = true;
                    _Value = EditorPrefs.GetBool(Key, DefaultValue);
                }

                return _Value;
            }
            set
            {
                _Value = value;
                EditorPrefs.SetBool(Key, value);
                _HasValue = true;
            }
        }

        /// <summary>Returns the current value of the `pref`.</summary>
        public static implicit operator bool(BoolPref pref)
        {
            return pref.Value;
        }

        /************************************************************************************************************************/

        /// <summary>Constructs a new <see cref="BoolPref"/>.</summary>
        public BoolPref(string label, bool defaultValue, Action onChanged = null)
        {
            Label = label;
            Key = Names.Namespace + "." + label;
            DefaultValue = defaultValue;
            OnChanged = onChanged;
            _Value = EditorPrefs.GetBool(Key, defaultValue);
        }

        /************************************************************************************************************************/

        /// <summary>Adds a menu item to toggle this pref.</summary>
        public void AddToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Display Options ->/" + Label), _Value, () =>
            {
                _Value = !_Value;
                EditorPrefs.SetBool(Key, _Value);
                if (OnChanged != null)
                    OnChanged();
            });
        }

        /************************************************************************************************************************/

        /// <summary>Various settings.</summary>
        public static readonly BoolPref
            UseIndentation = new BoolPref("Use Indentation", true),
            AutoOpenMenu = new BoolPref("Auto Open Menu", true),
            AutoHideFooter = new BoolPref("Auto Hide Footer", true),
            ShowNonPublicMethods = new BoolPref("Show Non-Public Methods", true),
            GroupNonPublicMethods = new BoolPref("Group Non-Public Methods", true),
            ShowStaticMethods = new BoolPref("Show Static Methods", true),
            ShowFullTypeNames = new BoolPref("Use Full Type Names", false),
            GroupMethodOverloads = new BoolPref("Sub-Menu for Method Overloads", true),
            SubMenuForEachBaseType = new BoolPref("Base Types ->/Individual Sub-Menus", true, MethodSelectionMenu.ClearMemberCache),
            SubMenuForBaseTypes = new BoolPref("Base Types ->/Group Sub-Menu", true),
            SubMenuForRootBaseType = new BoolPref("Base Types ->/Group Root Type", false);

        /************************************************************************************************************************/

        /// <summary>Adds menu items to toggle all prefs.</summary>
        public static void AddDisplayOptions(GenericMenu menu)
        {
            UseIndentation.AddToMenu(menu);
            AutoOpenMenu.AddToMenu(menu);
            AutoHideFooter.AddToMenu(menu);
            ShowNonPublicMethods.AddToMenu(menu);
            GroupNonPublicMethods.AddToMenu(menu);
            ShowStaticMethods.AddToMenu(menu);
            ShowFullTypeNames.AddToMenu(menu);
            GroupMethodOverloads.AddToMenu(menu);
            SubMenuForEachBaseType.AddToMenu(menu);
            SubMenuForBaseTypes.AddToMenu(menu);
            SubMenuForRootBaseType.AddToMenu(menu);
        }

        /************************************************************************************************************************/
    }
}

#endif
