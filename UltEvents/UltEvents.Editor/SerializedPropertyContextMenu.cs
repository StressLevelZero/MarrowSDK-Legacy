// UltEvents // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UltEvents.Editor
{
    internal static class SerializedPropertyContextMenu
    {
        /************************************************************************************************************************/

        [InitializeOnLoadMethod]
        private static void OnPropertyContextMenu()
        {
            EditorApplication.contextualPropertyMenu += (menu, property) =>
            {
                if (property.propertyType != SerializedPropertyType.Generic)
                    return;

                var accessor = property.GetAccessor();
                if (accessor == null)
                    return;

                var type = accessor.GetFieldElementType(property);
                if (typeof(UltEventBase).IsAssignableFrom(type))
                {
                    AddEventFunctions(menu, property, accessor);
                    BoolPref.AddDisplayOptions(menu);
                }
                else if (type == typeof(PersistentCall))
                {
                    AddCallClipboardItems(menu, property, accessor);
                    BoolPref.AddDisplayOptions(menu);
                }
            };
        }

        /************************************************************************************************************************/

        public static void AddEventFunctions(GenericMenu menu, SerializedProperty property,
            Serialization.PropertyAccessor accessor)
        {
            property = property.Copy();

            var type = accessor.GetFieldElementType(property);
            if (type == typeof(UltEvent))
            {
                menu.AddItem(new GUIContent("Invoke Event"), false, () =>
                {
                    var events = property.GetValues<UltEvent>();
                    for (int i = 0; i < events.Length; i++)
                    {
                        var e = events[i] as UltEvent;
                        if (e != null)
                            e.Invoke();
                    }
                    property.OnPropertyChanged();
                });
            }

            AddEventClipboardItems(menu, property, accessor);

            menu.AddItem(new GUIContent("Clear Event"), false, () =>
            {
                property.ModifyValues<UltEventBase>((e) =>
                {
                    if (e != null)
                        e.Clear();
                }, "Clear Event");
            });

            menu.AddItem(new GUIContent("Log Description"), false, () =>
            {
                var targets = property.serializedObject.targetObjects;
                var events = property.GetValues<UltEventBase>();

                for (int i = 0; i < events.Length; i++)
                    Debug.Log(events[i], targets[i]);
            });
        }

        /************************************************************************************************************************/

        private static void AddEventClipboardItems(GenericMenu menu, SerializedProperty property,
            Serialization.PropertyAccessor accessor)
        {
            // Copy Event.
            menu.AddItem(new GUIContent("Copy Event"), false, () =>
            {
                Clipboard.CopyEvent(property);
            });

            // Paste Event.
            AddMenuItem(menu, "Paste Event (Overwrite)", Clipboard.HasEvent, () =>
            {
                property.ModifyValues<UltEventBase>((e) =>
                {
                    Clipboard.Paste(e);
                }, "Paste Event");
            });

            // Paste Listener.
            AddMenuItem(menu, "Paste Listener (New) %#V", Clipboard.HasCall, () =>
            {
                property.ModifyValues<UltEventBase>((e) =>
               {
                   var call = new PersistentCall();
                   Clipboard.PasteCall(call);

                   if (e._PersistentCalls == null)
                       e._PersistentCalls = new List<PersistentCall>();
                   e._PersistentCalls.Add(call);
               }, "Paste PersistentCall");
            });
        }

        /************************************************************************************************************************/

        private static void AddCallClipboardItems(GenericMenu menu, SerializedProperty property,
            Serialization.PropertyAccessor accessor)
        {
            menu.AddItem(new GUIContent("Copy Listener %C"), false, () =>
            {
                Clipboard.CopyCall(property);
            });

            AddMenuItem(menu, "Paste Listener (Overwrite) %V", Clipboard.HasCall, () => Clipboard.PasteCall(property));
        }

        /************************************************************************************************************************/

        public static void AddMenuItem(GenericMenu menu, string label, bool enabled, GenericMenu.MenuFunction function)
        {
            if (enabled)
            {
                menu.AddItem(new GUIContent(label), false, function);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(label));
            }
        }

        /************************************************************************************************************************/

        public static void AddPropertyModifierFunction(GenericMenu menu, SerializedProperty property, string label, Action function)
        {
            menu.AddItem(new GUIContent(label), false, () =>
            {
                function();
                property.serializedObject.ApplyModifiedProperties();
            });
        }

        public static void AddPropertyModifierFunction(GenericMenu menu, SerializedProperty property, string label,
            Action<SerializedProperty> function)
        {
            menu.AddItem(new GUIContent(label), false, () =>
            {
                ForEachTarget(property, function);
            });
        }

        /************************************************************************************************************************/

        private static void ForEachTarget(SerializedProperty property, Action<SerializedProperty> function)
        {
            var targets = property.serializedObject.targetObjects;

            if (targets.Length == 1)
            {
                function(property);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                var path = property.propertyPath;
                for (int i = 0; i < targets.Length; i++)
                {
                    using (var serializedObject = new SerializedObject(targets[i]))
                    {
                        property = serializedObject.FindProperty(path);
                        function(property);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        /************************************************************************************************************************/
    }
}

#endif