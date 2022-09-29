// UltEvents // Copyright 2021 Kybernetik //
// Copied from Kybernetik.Core.

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace UltEvents.Editor
{
    /// <summary>[Editor-Only] Allows you to draw GUI fields which can be used to pick an object from a list.</summary>
    public static class ObjectPicker
    {
        /************************************************************************************************************************/
        #region Main Drawing Methods
        /************************************************************************************************************************/

        /// <summary>Draws a field which lets you pick an object from a list and returns the selected object.</summary>
        public static T Draw<T>(Rect area, T selected, Func<List<T>> getOptions, int suggestions, Func<T, GUIContent> getLabel, Func<T> getDragAndDrop,
            GUIStyle style)
        {
            var id = CheckCommand(ref selected);

            if (GUI.Button(area, getLabel(selected), style))
                ObjectPickerWindow.Show(id, selected, getOptions(), suggestions, getLabel);

            CheckDragAndDrop(area, ref selected, getOptions, getDragAndDrop);

            return selected;
        }

        /// <summary>Draws a field which lets you pick an object from a list and returns the selected object.</summary>
        public static T Draw<T>(Rect area, T selected, Func<List<T>> getOptions, int suggestions, Func<T, GUIContent> getLabel, Func<T> getDragAndDrop)
        {
            return Draw(area, selected, getOptions, suggestions, getLabel, getDragAndDrop, InternalGUI.TypeButtonStyle);
        }

        /************************************************************************************************************************/

        /// <summary>Draws a field (using GUILayout) which lets you pick an object from a list and returns the selected object.</summary>
        public static T DrawLayout<T>(T selected, Func<List<T>> getOptions, int suggestions, Func<T, GUIContent> getLabel, Func<T> getDragAndDrop,
            GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            var id = CheckCommand(ref selected);

            if (GUILayout.Button(getLabel(selected), style, layoutOptions))
                ObjectPickerWindow.Show(id, selected, getOptions(), suggestions, getLabel);

            CheckDragAndDrop(GUILayoutUtility.GetLastRect(), ref selected, getOptions, getDragAndDrop);

            return selected;
        }

        /// <summary>Draws a field (using GUILayout) which lets you pick an object from a list and returns the selected object.</summary>
        public static T DrawLayout<T>(T selected, Func<List<T>> getOptions, int suggestions, Func<T, GUIContent> getLabel, Func<T> getDragAndDrop,
            params GUILayoutOption[] layoutOptions)
        {
            return DrawLayout(selected, getOptions, suggestions, getLabel, getDragAndDrop, InternalGUI.TypeButtonStyle, layoutOptions);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws a field (as an inspector field using GUILayout) which lets you pick an object from a list and returns
        /// the selected object.
        /// </summary>
        public static T DrawEditorLayout<T>(GUIContent label, T selected, Func<List<T>> getOptions, int suggestions,
            Func<T, GUIContent> getLabel, Func<T> getDragAndDrop, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                selected = DrawLayout(selected, getOptions, suggestions, getLabel, getDragAndDrop, style, layoutOptions);
            }
            GUILayout.EndHorizontal();

            return selected;
        }

        /// <summary>
        /// Draws a field (as an inspector field using GUILayout) which lets you pick an object from a list and returns
        /// the selected object.
        /// </summary>
        public static T DrawEditorLayout<T>(GUIContent label, T selected, Func<List<T>> getOptions, int suggestions,
            Func<T, GUIContent> getLabel, Func<T> getDragAndDrop, params GUILayoutOption[] options)
        {
            return DrawEditorLayout(label, selected, getOptions, suggestions, getLabel, getDragAndDrop, InternalGUI.TypeButtonStyle, options);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Type Field
        /************************************************************************************************************************/

        /// <summary>Draws a field which lets you pick a <see cref="Type"></see> from a list and returns the selected type.</summary>
        public static Type DrawTypeField(Rect area, Type selected, Func<List<Type>> getOptions, int suggestions, GUIStyle style)
        {
            return Draw(area, selected, getOptions, suggestions,
                getLabel: (type) => new GUIContent(type != null ? type.GetNameCS() : "null"),
                getDragAndDrop: () => DragAndDrop.objectReferences[0].GetType(),
                style: style);
        }

        /************************************************************************************************************************/

        /// <summary>Draws a field which lets you pick an asset <see cref="Type"></see> from a list and returns the selected type.</summary>
        public static Type DrawAssetTypeField(Rect area, Type selected, Func<List<Type>> getOptions, int suggestions, GUIStyle style)
        {
            return Draw(area, selected, getOptions, suggestions,
                getLabel: (type) => new GUIContent(type != null ? type.GetNameCS() : "null", AssetPreview.GetMiniTypeThumbnail(type)),
                getDragAndDrop: () => DragAndDrop.objectReferences[0].GetType(),
                style: style);
        }

        /************************************************************************************************************************/

        /// <summary>Draws a field which lets you pick a <see cref="Type"></see> from a list and returns the selected <see cref="Type.AssemblyQualifiedName"/>.</summary>
        public static string DrawTypeField(Rect area, string selectedTypeName, Func<List<Type>> getOptions, int suggestions, GUIStyle style)
        {
            var selected = Type.GetType(selectedTypeName);

            selected = Draw(area, selected, getOptions, suggestions,
               getLabel: (type) => new GUIContent(type != null ? type.GetNameCS() : "No Type Selected"),
               getDragAndDrop: () => DragAndDrop.objectReferences[0].GetType(),
               style: style);

            return selected != null ? selected.AssemblyQualifiedName : null;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Utils
        /************************************************************************************************************************/

        /// <summary>
        /// Removes any duplicates of the first few elements in `options` (from 0 to `suggestions`) from anywhere later
        /// in the list.
        /// </summary>
        public static void RemoveDuplicateSuggestions<T>(List<T> options, int suggestions) where T : class
        {
            for (int i = options.Count - 1; i >= suggestions; i--)
            {
                var obj = options[i];
                for (int j = 0; j < suggestions; j++)
                {
                    if (obj == options[j])
                    {
                        options.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        /************************************************************************************************************************/

        private static int CheckCommand<T>(ref T selected)
        {
            var id = GUIUtility.GetControlID(FocusType.Passive);
            ObjectPickerWindow.TryGetPickedObject(id, ref selected);
            return id;
        }

        /************************************************************************************************************************/

        private static void CheckDragAndDrop<T>(Rect area, ref T selected, Func<List<T>> getOptions, Func<T> getDragAndDrop)
        {
            var currentEvent = Event.current;
            if (DragAndDrop.objectReferences.Length == 1 && area.Contains(currentEvent.mousePosition))
            {
                var drop = getDragAndDrop();

                // If the dragged object is a valid type, continue.
                if (!getOptions().Contains(drop))
                    return;

                if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.MouseDrag)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
                else if (currentEvent.type == EventType.DragPerform)
                {
                    selected = drop;
                    DragAndDrop.AcceptDrag();
                    GUI.changed = true;
                }
            }
        }

        /************************************************************************************************************************/

        private static class InternalGUI
        {
            public static readonly GUIStyle
                TypeButtonStyle;

            static InternalGUI()
            {
                TypeButtonStyle = new GUIStyle(EditorStyles.miniButton)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/

    internal sealed class ObjectPickerWindow : EditorWindow
    {
        /************************************************************************************************************************/

        private static int _FieldID;
        private static bool _HasPickedObject;
        private static object _PickedObject;

        private readonly List<GUIContent>
            Labels = new List<GUIContent>(),
            SearchedLabels = new List<GUIContent>();
        private readonly List<object>
            SearchedObjects = new List<object>();

        [NonSerialized]
        private object _SelectedObject;

        [NonSerialized]
        private IList _Objects;

        [NonSerialized]
        private int _Suggestions;

        [NonSerialized]
        private int _LabelWidthCalculationProgress;

        [NonSerialized]
        private float _MaxLabelWidth;

        [NonSerialized]
        private string _SearchText = "";

        [NonSerialized]
        private Vector2 _ScrollPosition;

        /************************************************************************************************************************/

        private bool HasSearchText
        {
            get { return !string.IsNullOrEmpty(_SearchText); }
        }

        /************************************************************************************************************************/

        public static void Show<T>(int fieldID, T selected, List<T> objects, int suggestions, Func<T, GUIContent> getLabel)
        {
            if (objects == null || objects.Count == 0)
            {
                Debug.LogError("'objects' list is null or empty.");
                return;
            }

            _FieldID = fieldID;
            _HasPickedObject = false;

            var window = CreateInstance<ObjectPickerWindow>();
            window.titleContent = new GUIContent("Pick a " + typeof(T).GetNameCS());
            window.minSize = new Vector2(112, 100);

            if (window.Labels.Capacity < objects.Count)
                window.Labels.Capacity = objects.Count;

            for (int i = 0; i < objects.Count; i++)
                window.Labels.Add(getLabel(objects[i]));

            //Debug.LogTemp("Showing Object Picker Window: " + window._Labels.DeepToString());

            window._SelectedObject = selected;
            window._Objects = objects;
            window._Suggestions = suggestions;

            // Auto-Scroll to the selected object.
            if (selected != null)
            {
                object sel = selected;

                for (int i = 0; i < window._Objects.Count; i++)
                {
                    if (sel == window._Objects[i])
                    {
                        window._ScrollPosition = new Vector2(0, i * InternalGUI.LabelHeight);
                        break;
                    }
                }
            }

            //window._Objects.LogErrorIfModified("the '" + nameof(objects) + "' list passed into " + Reflection.GetCallingMethod(2).GetNameCS());

            window.ShowAuxWindow();
        }

        /************************************************************************************************************************/

        public static void TryGetPickedObject<T>(int fieldID, ref T picked)
        {
            if (_HasPickedObject && _FieldID == fieldID)
            {
                picked = (T)_PickedObject;
                _PickedObject = null;
                _HasPickedObject = false;
                GUI.changed = true;
            }
        }

        /************************************************************************************************************************/

        private void PickAndClose()
        {
            _PickedObject = _SelectedObject;
            _HasPickedObject = true;
            Close();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        /************************************************************************************************************************/

        private void OnGUI()
        {
            switch (Event.current.type)
            {
                case EventType.MouseMove:
                case EventType.Layout:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                case EventType.Ignore:
                case EventType.Used:
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                case EventType.ContextClick:
                    return;

                default:
                    break;
            }

            if (CheckInput())
            {
                Event.current.Use();
                return;
            }

            UpdateLabelWidthCalculation();

            var area = new Rect(0, 0, position.width, position.height);

            DrawSearchBar(ref area);

            area.yMax = position.height;

            var viewRect = CalculateViewRect(area.height);

            // Selection List.
            _ScrollPosition = GUI.BeginScrollView(area, _ScrollPosition, viewRect);
            {
                // Figure out how many fields are actually visible.
                int firstVisibleField, lastVisibleField;
                DetermineVisibleRange(out firstVisibleField, out lastVisibleField);

                if (HasSearchText)// Active Search.
                {
                    DrawSearchedOptions(viewRect, firstVisibleField, lastVisibleField);
                }
                else// No Search.
                {
                    DrawAllOptions(viewRect, firstVisibleField, lastVisibleField);
                }
            }
            GUI.EndScrollView(true);
        }

        /************************************************************************************************************************/

        private void UpdateLabelWidthCalculation()
        {
            if (_LabelWidthCalculationProgress < Labels.Count)
            {
                var calculationCount = 0;
                do
                {
                    var label = Labels[_LabelWidthCalculationProgress];

                    var width = InternalGUI.ButtonStyle.CalcSize(label).x;
                    if (_MaxLabelWidth < width)
                        _MaxLabelWidth = width;
                }
                while (++_LabelWidthCalculationProgress < Labels.Count && calculationCount++ < 100);

                Repaint();
            }
        }

        /************************************************************************************************************************/

        private bool CheckInput()
        {
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.KeyUp)
            {
                switch (currentEvent.keyCode)
                {
                    case KeyCode.Return:
                        PickAndClose();
                        return true;

                    case KeyCode.Escape:
                        Close();
                        return true;

                    case KeyCode.UpArrow:
                        OffsetSelectedIndex(-1);
                        return true;

                    case KeyCode.DownArrow:
                        OffsetSelectedIndex(1);
                        return true;

                    default:
                        break;
                }
            }

            return false;
        }

        /************************************************************************************************************************/

        private void DrawSearchBar(ref Rect area)
        {
            area.height = InternalGUI.SearchBarHeight;
            GUI.BeginGroup(area, EditorStyles.toolbar);
            {
                area.x += 2;
                area.y += 2;
                area.width -= InternalGUI.SearchBarEndStyle.fixedWidth + 4;

                GUI.SetNextControlName("SearchFilter");
                EditorGUI.BeginChangeCheck();
                var searchText = GUI.TextField(area, _SearchText, InternalGUI.SearchBarStyle);
                if (EditorGUI.EndChangeCheck())
                    OnSearchTextChanged(searchText);
                EditorGUI.FocusTextInControl("SearchFilter");

                area.x = area.xMax;
                area.width = InternalGUI.SearchBarEndStyle.fixedWidth;
                if (HasSearchText)
                {
                    if (GUI.Button(area, "", InternalGUI.SearchBarCancelStyle))
                    {
                        _SearchText = "";
                    }
                }
                else GUI.Box(area, "", InternalGUI.SearchBarEndStyle);
            }
            GUI.EndGroup();

            area.x = 0;
            area.width = position.width;
            area.y += area.height;
        }

        /************************************************************************************************************************/

        private void OnSearchTextChanged(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                SearchedLabels.Clear();
                SearchedObjects.Clear();
            }
            // If the search text starts the same as before, it will include only a subset of the previous options.
            // So we can just remove objects from the previous search list instead of checking the full list again.
            else if (SearchedLabels.Count > 0 && text.StartsWith(_SearchText))
            {
                for (int i = SearchedLabels.Count - 1; i >= 0; i--)
                {
                    if (!IsVisibleInSearch(text, SearchedLabels[i].text))
                    {
                        SearchedLabels.RemoveAt(i);
                        SearchedObjects.RemoveAt(i);
                    }
                }
            }
            // Otherwise clear the search list and re-gather any visible objects from the full list.
            else
            {
                SearchedLabels.Clear();
                SearchedObjects.Clear();

                for (int i = 0; i < Labels.Count; i++)
                {
                    var label = Labels[i];
                    if (IsVisibleInSearch(text, label.text))
                    {
                        SearchedLabels.Add(label);
                        SearchedObjects.Add(_Objects[i]);
                    }
                }
            }

            _SearchText = text;

            if (!SearchedObjects.Contains(_SelectedObject))
                _SelectedObject = SearchedObjects.Count > 0 ? SearchedObjects[0] : null;
        }

        private static bool IsVisibleInSearch(string search, string text)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(text, search, CompareOptions.IgnoreCase) >= 0;
        }

        /************************************************************************************************************************/

        private Rect CalculateViewRect(float height)
        {
            var area = new Rect();

            if (HasSearchText)
            {
                area.height = InternalGUI.LabelHeight * SearchedLabels.Count;
            }
            else
            {
                area.height = InternalGUI.LabelHeight * Labels.Count;

                if (_Suggestions > 0)
                    area.height += InternalGUI.HeaddingStyle.fixedHeight * 2;
            }

            if (_MaxLabelWidth < position.width)
            {
                area.width = position.width;

                if (area.height > height)
                    area.width -= 16;
            }
            else area.width = _MaxLabelWidth;

            return area;
        }

        /************************************************************************************************************************/

        private void DetermineVisibleRange(out int firstVisibleField, out int lastVisibleField)
        {
            var top = _ScrollPosition.y;
            var bottom = top + position.height - InternalGUI.SearchBarHeight;
            if (_Suggestions > 0)
            {
                top -= InternalGUI.HeaddingStyle.fixedHeight * 2;
                bottom += InternalGUI.HeaddingStyle.fixedHeight;
            }

            firstVisibleField = Mathf.Max(0, (int)(top / InternalGUI.LabelHeight));
            lastVisibleField = Mathf.Min(Labels.Count, Mathf.CeilToInt(bottom / InternalGUI.LabelHeight));
        }

        /************************************************************************************************************************/

        private void DrawAllOptions(Rect area, int firstVisibleField, int lastVisibleField)
        {
            if (_Suggestions == 0 || _Suggestions >= Labels.Count)
            {
                area.y = firstVisibleField * InternalGUI.LabelHeight;
                DrawRange(ref area, Labels, _Objects, firstVisibleField, lastVisibleField);
            }
            else
            {
                area.height = InternalGUI.HeaddingStyle.fixedHeight;
                GUI.Label(area, "Suggestions", InternalGUI.HeaddingStyle);

                area.y = area.yMax + firstVisibleField * InternalGUI.LabelHeight;
                DrawRange(ref area, Labels, _Objects, firstVisibleField, Mathf.Min(lastVisibleField, _Suggestions));

                area.height = InternalGUI.HeaddingStyle.fixedHeight;
                GUI.Label(area, "Other Options", InternalGUI.HeaddingStyle);
                area.y = area.yMax;

                DrawRange(ref area, Labels, _Objects, Mathf.Max(_Suggestions, firstVisibleField), lastVisibleField);
            }
        }

        /************************************************************************************************************************/

        private void DrawSearchedOptions(Rect area, int firstVisibleField, int lastVisibleField)
        {
            area.y = firstVisibleField * InternalGUI.LabelHeight;
            DrawRange(ref area, SearchedLabels, SearchedObjects, firstVisibleField, lastVisibleField);
        }

        /************************************************************************************************************************/

        private void DrawRange(ref Rect area, List<GUIContent> labels, IList objects, int start, int end)
        {
            area.height = InternalGUI.LabelHeight;

            if (end > labels.Count)
                end = labels.Count;

            for (; start < end; start++)
            {
                DrawOption(area, labels, objects, start);
                area.y = area.yMax;
            }
        }

        /************************************************************************************************************************/

        private void DrawOption(Rect area, List<GUIContent> labels, IList objects, int index)
        {
            var obj = objects[index];
            var wasOn = obj == _SelectedObject;
            var isOn = GUI.Toggle(area, wasOn, labels[index], wasOn ? InternalGUI.SelectedButtonStyle : InternalGUI.ButtonStyle);
            if (isOn != wasOn)
            {
                if (wasOn)
                {
                    PickAndClose();
                }
                else if (isOn)
                {
                    _SelectedObject = obj;
                }
            }
        }

        /************************************************************************************************************************/

        private void Update()
        {
            if (focusedWindow != this)
                Close();
        }

        /************************************************************************************************************************/

        private void OffsetSelectedIndex(int offset)
        {
            var objects = HasSearchText ? SearchedObjects : _Objects;

            if (objects.Count == 0)
                return;

            var index = objects.IndexOf(_SelectedObject);
            if (index >= 0)
                _SelectedObject = objects[Mathf.Clamp(index + offset, 0, objects.Count)];
            else
                _SelectedObject = objects[0];
        }

        /************************************************************************************************************************/

        private static class InternalGUI
        {
            public static readonly GUIStyle
                SearchBarStyle,
                SearchBarEndStyle,
                SearchBarCancelStyle,
                HeaddingStyle,
                ButtonStyle,
                SelectedButtonStyle;

            public static float SearchBarHeight
            {
                get { return EditorStyles.toolbar.fixedHeight; }
            }

            public static float LabelHeight
            {
                get { return ButtonStyle.fixedHeight; }
            }

            static InternalGUI()
            {
                SearchBarStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
                SearchBarEndStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty");
                SearchBarCancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");

                HeaddingStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    fixedHeight = 22
                };

                ButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 12
                };

                SelectedButtonStyle = new GUIStyle(ButtonStyle)
                {
                    fontStyle = FontStyle.Bold
                };
            }
        }

        /************************************************************************************************************************/
    }
}

#endif
