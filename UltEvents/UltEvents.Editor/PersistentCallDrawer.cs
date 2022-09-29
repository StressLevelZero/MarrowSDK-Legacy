// UltEvents // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltEvents.Editor
{
    [CustomPropertyDrawer(typeof(PersistentCall), true)]
    internal sealed class PersistentCallDrawer : PropertyDrawer
    {
        /************************************************************************************************************************/

        public const float
            RowHeight = 16,
            Padding = 2,
            SuggestionButtonWidth = 16;

        public static readonly GUIStyle
            PopupButtonStyle,
            PopupLabelStyle;

        private static readonly GUIContent
            ArgumentLabel = new GUIContent(),
            MethodNameSuggestionLabel = new GUIContent("?", "Suggest a method name");

        public static readonly Color
            ErrorFieldColor = new Color(1, 0.65f, 0.65f);

        /************************************************************************************************************************/

        static PersistentCallDrawer()
        {
            PopupButtonStyle = new GUIStyle(EditorStyles.popup)
            {
                fixedHeight = RowHeight
            };

            PopupLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(4, 14, 0, 0)
            };
        }

        /************************************************************************************************************************/

        public override float GetPropertyHeight(SerializedProperty callProperty, GUIContent label)
        {
            if (callProperty.hasMultipleDifferentValues)
            {
                if (DrawerState.GetPersistentArgumentsProperty(callProperty).hasMultipleDifferentValues)
                    return EditorGUIUtility.singleLineHeight;

                if (DrawerState.GetMethodNameProperty(callProperty).hasMultipleDifferentValues)
                    return EditorGUIUtility.singleLineHeight;
            }

            if (DrawerState.GetCall(callProperty).GetMethodSafe() == null)
                return EditorGUIUtility.singleLineHeight;

            callProperty = DrawerState.GetPersistentArgumentsProperty(callProperty);

            return (EditorGUIUtility.singleLineHeight + Padding) * (1 + callProperty.arraySize) - Padding;
        }

        /************************************************************************************************************************/

        public override void OnGUI(Rect area, SerializedProperty callProperty, GUIContent label)
        {
            DrawerState.Current.BeginCall(callProperty);

            var propertyarea = area;

            // If we are in the reorderable list of an event, adjust the property area to cover the list bounds.
            if (DrawerState.Current.CachePreviousCalls)
            {
                propertyarea.xMin -= 20;
                propertyarea.yMin -= 4;
                propertyarea.width += 4;
            }

            label = EditorGUI.BeginProperty(propertyarea, label, callProperty);
            {
                const float Space = 2;

                var x = area.x;
                var xMax = area.xMax;

                area.height = RowHeight;

                // Target Field.
                area.xMax = EditorGUIUtility.labelWidth + 12;
                bool autoOpenMethodMenu;
                DoTargetFieldGUI(area,
                    DrawerState.Current.TargetProperty, DrawerState.Current.MethodNameProperty,
                    out autoOpenMethodMenu);

                EditorGUI.showMixedValue = DrawerState.Current.PersistentArgumentsProperty.hasMultipleDifferentValues || DrawerState.Current.MethodNameProperty.hasMultipleDifferentValues;

                var method = EditorGUI.showMixedValue ? null : DrawerState.Current.call.GetMethodSafe();

                // Method Name Dropdown.
                area.x += area.width + Space;
                area.xMax = xMax;
                DoMethodFieldGUI(area, method, autoOpenMethodMenu);

                // Persistent Arguments.
                if (method != null)
                {
                    area.x = x;
                    area.xMax = xMax;

                    DrawerState.Current.callParameters = method.GetParameters();
                    if (DrawerState.Current.callParameters.Length == DrawerState.Current.PersistentArgumentsProperty.arraySize)
                    {
                        var labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth -= area.x - 14;

                        for (int i = 0; i < DrawerState.Current.callParameters.Length; i++)
                        {
                            DrawerState.Current.parameterIndex = i;
                            area.y += area.height + Padding;

                            ArgumentLabel.text = DrawerState.Current.callParameters[i].Name;

                            var argumentProperty = DrawerState.Current.PersistentArgumentsProperty.GetArrayElementAtIndex(i);

                            if (argumentProperty.propertyPath != "")
                            {
                                EditorGUI.PropertyField(area, argumentProperty, ArgumentLabel);
                            }
                            else
                            {
                                if (GUI.Button(area, new GUIContent(
                                           "Reselect these objects to show arguments",
                                           "This is the result of a bug in the way Unity updates the SerializedProperty for an array after it is resized while multiple objects are selected")))
                                {
                                    var selection = Selection.objects;
                                    Selection.objects = new Object[0];
                                    EditorApplication.delayCall += () => Selection.objects = selection;
                                }

                                break;
                            }
                        }

                        EditorGUIUtility.labelWidth = labelWidth;
                    }
                    else
                    {
                        Debug.LogError("Method parameter count doesn't match serialized argument count " + DrawerState.Current.callParameters.Length
                            + " : " + DrawerState.Current.PersistentArgumentsProperty.arraySize);
                    }
                    DrawerState.Current.callParameters = null;
                }

                EditorGUI.showMixedValue = false;
            }
            EditorGUI.EndProperty();

            DrawerState.Current.EndCall();
        }

        /************************************************************************************************************************/
        #region Target Field
        /************************************************************************************************************************/

        private static void DoTargetFieldGUI(Rect area, SerializedProperty targetProperty, SerializedProperty methodNameProperty, out bool autoOpenMethodMenu)
        {
            autoOpenMethodMenu = false;

            // Type field for a static type.
            if (targetProperty.objectReferenceValue == null && !targetProperty.hasMultipleDifferentValues)
            {
                var methodName = methodNameProperty.stringValue;
                string typeName;

                var lastDot = methodName.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    typeName = methodName.Substring(0, lastDot);
                    lastDot++;
                    methodName = methodName.Substring(lastDot, methodName.Length - lastDot);
                }
                else typeName = "";

                var color = GUI.color;
                if (Type.GetType(typeName) == null)
                    GUI.color = ErrorFieldColor;

                const float
                    ObjectPickerButtonWidth = 35,
                    Padding = 2;

                area.width -= ObjectPickerButtonWidth + Padding;

                EditorGUI.BeginChangeCheck();
                typeName = ObjectPicker.DrawTypeField(area, typeName, GetAllTypes, 0, EditorStyles.miniButton);
                if (EditorGUI.EndChangeCheck())
                {
                    methodNameProperty.stringValue = typeName + "." + methodName;
                }

                HandleTargetFieldDragAndDrop(area, ref autoOpenMethodMenu);

                GUI.color = color;

                area.x += area.width + Padding;
                area.width = ObjectPickerButtonWidth;
            }

            // Object field for an object reference.
            DoTargetObjectFieldGUI(area, targetProperty, ref autoOpenMethodMenu);
        }

        /************************************************************************************************************************/

        private static void DoTargetObjectFieldGUI(Rect area, SerializedProperty targetProperty, ref bool autoOpenMethodMenu)
        {
            if (targetProperty.hasMultipleDifferentValues)
                EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            var oldTarget = targetProperty.objectReferenceValue;
            var target = EditorGUI.ObjectField(area, oldTarget, typeof(Object), true);
            if (EditorGUI.EndChangeCheck())
            {
                SetBestTarget(oldTarget, target, out autoOpenMethodMenu);
            }

            EditorGUI.showMixedValue = false;
        }

        /************************************************************************************************************************/

        private static List<Type> _AllTypes;

        private static List<Type> GetAllTypes()
        {
            if (_AllTypes == null)
            {
                // Gather all types in all currently loaded assemblies.
                _AllTypes = new List<Type>(4192);

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    var types = assemblies[i].GetTypes();
                    for (int j = 0; j < types.Length; j++)
                    {
                        var type = types[j];
                        if (!type.ContainsGenericParameters &&// No Generics (because the type picker field doesn't let you pick generic parameters).
                            !type.IsInterface &&// No Interfaces (because they can't have any static methods).
                            !type.IsDefined(typeof(ObsoleteAttribute), true) &&// No Obsoletes.
                            type.GetMethods(UltEventUtils.StaticBindings).Length > 0)// No types without any static methods.
                        {
                            // The type might still not have any valid methods, but at least we've narrowed down the list a lot.

                            _AllTypes.Add(type);
                        }
                    }
                }

                _AllTypes.Sort((a, b) => a.FullName.CompareTo(b.FullName));

                // We probably just allocated thousands of arrays with all those GetMethods calls, so call for a cleanup imediately.
                GC.Collect();
            }

            return _AllTypes;
        }

        /************************************************************************************************************************/

        private static void HandleTargetFieldDragAndDrop(Rect area, ref bool autoOpenMethodMenu)
        {
            // Drag and drop objects into the type field.
            switch (Event.current.type)
            {
                case EventType.Repaint:
                case EventType.DragUpdated:
                    {
                        if (!area.Contains(Event.current.mousePosition))
                            break;

                        var dragging = DragAndDrop.objectReferences;
                        if (dragging != null && dragging.Length == 1)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        }
                    }
                    break;

                case EventType.DragPerform:
                    {
                        if (!area.Contains(Event.current.mousePosition))
                            break;

                        var dragging = DragAndDrop.objectReferences;
                        if (dragging != null && dragging.Length == 1)
                        {
                            SetBestTarget(DrawerState.Current.TargetProperty.objectReferenceValue, dragging[0], out autoOpenMethodMenu);

                            DragAndDrop.AcceptDrag();
                            GUI.changed = true;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /************************************************************************************************************************/

        private static void SetBestTarget(Object oldTarget, Object newTarget, out bool autoOpenMethodMenu)
        {
            // It's more likely that the user intends to target a method on a Component than the GameObject itself so
            // if a GameObject was dropped in, try to select a component with the same type as the old target,
            // otherwise select it's first component after the Transform.
            var gameObject = newTarget as GameObject;
            if (!(oldTarget is GameObject) && !ReferenceEquals(gameObject, null))
            {
                var oldComponent = oldTarget as Component;
                if (!ReferenceEquals(oldComponent, null))
                {
                    newTarget = gameObject.GetComponent(oldComponent.GetType());
                    if (newTarget != null)
                        goto FoundTarget;
                }

                var components = gameObject.GetComponents<Component>();
                newTarget = components.Length > 1 ? components[1] : components[0];
            }

        FoundTarget:

            SetTarget(newTarget);

            autoOpenMethodMenu = BoolPref.AutoOpenMenu && newTarget != null && DrawerState.Current.call.GetMethodSafe() == null;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        private static void DoMethodFieldGUI(Rect area, MethodBase method, bool autoOpenMethodMenu)
        {
            EditorGUI.BeginProperty(area, null, DrawerState.Current.MethodNameProperty);
            {
                if (includeRemoveButton)
                    area.width -= RemoveButtonWidth;

                var color = GUI.color;

                string label;
                if (EditorGUI.showMixedValue)
                {
                    label = "Mixed Values";
                }
                else if (method != null)
                {
                    label = MethodSelectionMenu.GetMethodSignature(method, false);

                    DoGetSetToggleGUI(ref area, method);
                }
                else
                {
                    var methodName = DrawerState.Current.MethodNameProperty.stringValue;
                    Type declaringType;
                    PersistentCall.GetMethodDetails(methodName,
                        DrawerState.Current.TargetProperty.objectReferenceValue,
                        out declaringType, out label);
                    DoMethodNameSuggestionGUI(ref area, declaringType, methodName);
                    GUI.color = ErrorFieldColor;
                }

                if (autoOpenMethodMenu || (GUI.Button(area, GUIContent.none, PopupButtonStyle) && Event.current.button == 0))
                {
                    MethodSelectionMenu.ShowMenu(area);
                }

                GUI.color = color;

                PopupLabelStyle.fontStyle = DrawerState.Current.MethodNameProperty.prefabOverride ? FontStyle.Bold : FontStyle.Normal;
                GUI.Label(area, label, PopupLabelStyle);
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static float _GetSetWidth;

        private static float GetSetWidth
        {
            get
            {
                if (_GetSetWidth <= 0)
                {
                    float _, width;
                    ArgumentLabel.text = "Get";
                    GUI.skin.button.CalcMinMaxWidth(ArgumentLabel, out _, out width);

                    ArgumentLabel.text = "Set";
                    GUI.skin.button.CalcMinMaxWidth(ArgumentLabel, out _, out _GetSetWidth);

                    if (_GetSetWidth < width)
                        _GetSetWidth = width;
                }

                return _GetSetWidth;
            }
        }

        /************************************************************************************************************************/

        private static void DoGetSetToggleGUI(ref Rect area, MethodBase method)
        {
            // Check if the method name starts with "get_" or "set_".
            // Check the underscore first since it's hopefully the rarest so it can break out early.

            var name = method.Name;
            if (name.Length <= 4 || name[3] != '_' || name[2] != 't' || name[1] != 'e')
                return;

            var first = name[0];
            var isGet = first == 'g';
            var isSet = first == 's';
            if (!isGet && !isSet)
                return;

            var methodName = (isGet ? "set_" : "get_") + name.Substring(4, name.Length - 4);
            var oppositePropertyMethod = method.DeclaringType.GetMethod(methodName, UltEventUtils.AnyAccessBindings);
            if (oppositePropertyMethod == null ||
                (isGet && !MethodSelectionMenu.IsSupported(method.GetReturnType())))
                return;

            area.width -= GetSetWidth + Padding;

            var buttonArea = new Rect(
                area.x + area.width + Padding,
                area.y,
                GetSetWidth,
                area.height);

            if (GUI.Button(buttonArea, isGet ? "Get" : "Set"))
            {
                var cachedState = new DrawerState();
                cachedState.CopyFrom(DrawerState.Current);

                EditorApplication.delayCall += () =>
                {
                    DrawerState.Current.CopyFrom(cachedState);

                    SetMethod(oppositePropertyMethod);

                    DrawerState.Current.Clear();

                    InternalEditorUtility.RepaintAllViews();
                };
            }
        }

        /************************************************************************************************************************/

        private static void DoMethodNameSuggestionGUI(ref Rect area, Type declaringType, string methodName)
        {
            if (declaringType == null ||
                string.IsNullOrEmpty(methodName))
                return;

            var lastDot = methodName.LastIndexOf('.');
            if (lastDot >= 0)
            {
                lastDot++;
                if (lastDot >= methodName.Length)
                    return;

                methodName = methodName.Substring(lastDot);
            }

            var methods = declaringType.GetMethods(UltEventUtils.AnyAccessBindings);
            if (methods.Length == 0)
                return;

            area.width -= SuggestionButtonWidth + Padding;

            var buttonArea = new Rect(
                area.x + area.width + Padding,
                area.y,
                SuggestionButtonWidth,
                area.height);

            if (GUI.Button(buttonArea, MethodNameSuggestionLabel))
            {
                var cachedState = new DrawerState();
                cachedState.CopyFrom(DrawerState.Current);

                EditorApplication.delayCall += () =>
                {
                    DrawerState.Current.CopyFrom(cachedState);

                    var bestMethod = methods[0];
                    var bestDistance = UltEventUtils.CalculateLevenshteinDistance(methodName, bestMethod.Name);

                    var i = 1;
                    for (; i < methods.Length; i++)
                    {
                        var method = methods[i];
                        var distance = UltEventUtils.CalculateLevenshteinDistance(methodName, method.Name);

                        if (bestDistance > distance)
                        {
                            bestDistance = distance;
                            bestMethod = method;
                        }
                    }

                    SetMethod(bestMethod);

                    DrawerState.Current.Clear();

                    InternalEditorUtility.RepaintAllViews();
                };
            }
        }

        /************************************************************************************************************************/

        public static void SetTarget(Object target)
        {
            DrawerState.Current.TargetProperty.objectReferenceValue = target;
            DrawerState.Current.TargetProperty.serializedObject.ApplyModifiedProperties();

            if (target == null ||
                DrawerState.Current.call.GetMethodSafe() == null)
            {
                SetMethod(null);
            }
        }

        /************************************************************************************************************************/

        public static void SetMethod(MethodInfo methodInfo)
        {
            DrawerState.Current.CallProperty.ModifyValues<PersistentCall>((call) =>
            {
                if (call != null)
                    call.SetMethod(methodInfo, DrawerState.Current.TargetProperty.objectReferenceValue);
            }, "Set Method");
        }

        /************************************************************************************************************************/
        #region Remove Button
        /************************************************************************************************************************/

        public const float RemoveButtonWidth = 18;

        public static bool includeRemoveButton;

        /************************************************************************************************************************/

        public static bool DoRemoveButtonGUI(Rect rowArea)
        {
            includeRemoveButton = false;

            rowArea.xMin = rowArea.xMax - RemoveButtonWidth + 2;
            rowArea.height = EditorGUIUtility.singleLineHeight + 2;

            return GUI.Button(rowArea, ReorderableList.defaultBehaviours.iconToolbarMinus, ReorderableList.defaultBehaviours.preButton);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif