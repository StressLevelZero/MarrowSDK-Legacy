using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.GUI;
using UnityEditor.SceneManagement;
using SLZ.Marrow.Warehouse;
using Object = UnityEngine.Object;

namespace SLZ.MarrowEditor
{
    [CustomPropertyDrawer(typeof(MarrowAsset))]
    public class MarrowAssetPropertyDrawer : PropertyDrawer
    {
        private GUIStyle buttonStyle = null;
        private Dictionary<UnityEngine.Object, MarrowAsset> targetReferences = new Dictionary<UnityEngine.Object, MarrowAsset>();
        private bool multiEdit = false;
        private bool targetReferencesSame = false;
        private Type marrowAssetType = null;
        private Type marrowAssetActualType = null;

        private bool isInArray;
        private bool isDragging;
        private bool isDropping;
        private bool isValidDrop;

        DisplayMode displayMode = DisplayMode.AUTO;
        enum DisplayMode
        {
            AUTO,
            GUID,
            OBJECT_SELECTOR
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect fullPosition = position;


            EditorGUI.BeginProperty(position, label, property);

            SetTargetReferences(property, label);
            isInArray = property.TryFindParentArrayProperty(out _);

            isDropping = Event.current.type == EventType.DragPerform && fullPosition.Contains(Event.current.mousePosition);
            isDragging = DragAndDrop.objectReferences.Length > 0;
            if (isDragging)
                isValidDrop = ValidDropsCount() > 0;
            else
                isValidDrop = false;

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.padding = new RectOffset(1, 1, 1, 1);
            }

            GUIContent labelContent;
            try
            {
                var path = property.propertyPath;
                int pos = int.Parse(path.Split('[').LastOrDefault().TrimEnd(']'));
                labelContent = new GUIContent(ObjectNames.NicifyVariableName((marrowAssetType == null ? "Marrow Asset" : marrowAssetType.Name) + pos.ToString()));
            }
            catch
            {
                labelContent = label;
            }


            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), labelContent);


            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (!multiEdit)
            {
                var labelText = label.text;
                MarrowAsset assetReference = property.GetActualObjectForSerializedProperty<MarrowAsset>(fieldInfo, ref labelText);

                if (assetReference == null)
                {
                    EditorGUI.LabelField(position, new GUIContent("------", "something broke blame k2"));
                }
                else
                {
                    SerializedProperty guidProp = property.FindPropertyRelative("_assetGUID");

                    var editorAsset = assetReference.EditorAsset;
                    marrowAssetType = assetReference.GetType();
                    marrowAssetActualType = assetReference.AssetType;
                    bool assetExists = editorAsset != null;

                    switch (displayMode)
                    {
                        case DisplayMode.AUTO:
                            if (assetExists || !Barcode.IsValid(assetReference.AssetGUID))
                                DrawMarrowAssetReference(position, fullPosition, property);
                            else
                                DrawGUID(position, fullPosition, guidProp);
                            break;
                        case DisplayMode.GUID:
                            DrawGUID(position, fullPosition, guidProp);
                            break;
                        case DisplayMode.OBJECT_SELECTOR:
                            DrawMarrowAssetReference(position, fullPosition, property);
                            break;
                    }

                }
            }
            else
            {
                var labelText = label.text;
                MarrowAsset assetReference = property.GetActualObjectForSerializedProperty<MarrowAsset>(fieldInfo, ref labelText);
                marrowAssetType = assetReference.GetType();

                SerializedProperty guidProp = property.FindPropertyRelative("_assetGUID");

                foreach (var targetRef in targetReferences)
                {
                    var _ = targetRef.Value.EditorAsset;
                }

                switch (displayMode)
                {
                    case DisplayMode.AUTO:
                    case DisplayMode.OBJECT_SELECTOR:
                        DrawMarrowAssetReference(position, fullPosition, property);
                        break;
                    case DisplayMode.GUID:
                        DrawGUID(position, fullPosition, guidProp);
                        break;
                }
            }


            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        private void SetTargetReferences(SerializedProperty property, GUIContent label)
        {
            targetReferences.Clear();
            bool same = true;
            string sameGUID = null;
            if (property.serializedObject.targetObjects.Length > 1)
            {
                multiEdit = true;
            }
            foreach (var targetObject in property.serializedObject.targetObjects)
            {
                var serializedObjectMulti = new SerializedObject(targetObject);
                SerializedProperty multiProp = serializedObjectMulti.FindProperty(property.propertyPath);
                if (multiProp != null)
                {
                    var labelText = label.text;
                    var assetRef = multiProp.GetActualObjectForSerializedProperty<MarrowAsset>(fieldInfo, ref labelText);
                    if (assetRef != null)
                    {
                        targetReferences.Add(targetObject, assetRef);
                        if (sameGUID == null)
                        {
                            sameGUID = assetRef.AssetGUID.ToString();
                        }
                        else if (same)
                        {
                            if (!sameGUID.Equals(assetRef.AssetGUID.ToString()))
                            {
                                same = false;
                            }
                        }
                    }
                    else
                    {
                        same = false;
                    }
                }
                else
                {
                    same = false;
                }
            }
            targetReferencesSame = same;
        }

        private void DrawMarrowAssetReference(Rect position, Rect fullPosition, SerializedProperty property)
        {
            EditorGUI.BeginChangeCheck();

            float buttonWidth = 20;
            Rect buttonPos = fullPosition;
            Rect fieldPos = position;
            Rect dropBox = fieldPos;

            buttonPos.width = buttonWidth;
            buttonPos.x = fullPosition.width + fullPosition.x - buttonWidth * 0.9f;

            fieldPos.width -= (buttonPos.width);

            if (isDragging && isInArray && isValidDrop)
            {
                fieldPos.width /= 2f;
                dropBox = fieldPos;
                dropBox.x += fieldPos.width;
            }

            Object obj;
            Type objType = typeof(Object);
            string barcode = Barcode.EMPTY;
            if (multiEdit && !targetReferencesSame)
            {
                obj = null;
            }
            else
            {
                MarrowAsset crateRef = targetReferences.First().Value;
                obj = crateRef.EditorAsset;
                objType = crateRef.AssetType;
                barcode = crateRef.AssetGUID;
            }

            if (isDragging && isInArray && isValidDrop)
            {
                bool isDragInBox = dropBox.Contains(Event.current.mousePosition);
                if (isDragInBox)
                {
                    int validDrops = ValidDropsCount();
                    if (validDrops > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                        string extraS = validDrops > 1 ? "s" : "";
                        EditorGUI.HelpBox(dropBox, $"Insert {validDrops} {marrowAssetActualType?.Name}{extraS}", MessageType.Error);
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        EditorGUI.HelpBox(dropBox, $"Invalid items", MessageType.Error);
                    }
                }
                else
                {
                    EditorGUI.HelpBox(dropBox, $"DROP to insert {DragAndDrop.objectReferences.Length} items", MessageType.Warning);
                }
                if (isDropping && dropBox.Contains(Event.current.mousePosition))
                {
                    HandleDrop(property);
                }
            }

            obj = (Object)EditorGUI.ObjectField(fieldPos, GUIContent.none, obj, objType, false);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeObject(property, obj);
            }


            if (!string.IsNullOrEmpty(barcode))
            {
                EditorGUI.LabelField(fieldPos, new GUIContent("", barcode));
            }

            EditorGUI.LabelField(buttonPos, new GUIContent("", "Switch To Text GUID"));
            if (GUI.Button(buttonPos, EditorGUIUtility.IconContent("InputField Icon"), buttonStyle))
            {
                displayMode = DisplayMode.GUID;
            }

        }

        private void DrawGUID(Rect position, Rect fullPosition, SerializedProperty guidProp)
        {
            float buttonWidth = 20;
            Rect buttonPos = fullPosition;
            Rect fieldPos = position;

            buttonPos.width = buttonWidth;
            buttonPos.x = fullPosition.width + fullPosition.x - buttonWidth * 0.9f;

            fieldPos.width -= buttonPos.width;

            EditorGUI.PropertyField(fieldPos, guidProp, GUIContent.none);
            EditorGUI.LabelField(fieldPos, new GUIContent("", guidProp.stringValue));

            EditorGUI.LabelField(buttonPos, new GUIContent("", "Switch To Asset Selector"));
            if (GUI.Button(buttonPos, EditorGUIUtility.IconContent("Selectable Icon"), buttonStyle))
            {
                displayMode = DisplayMode.OBJECT_SELECTOR;
            }
        }

        private void ChangeObject(SerializedProperty property, Object obj)
        {
            bool anyChanged = false;
            foreach (var targetRef in targetReferences)
            {
                bool changed = SetObject(property, targetRef.Key, targetRef.Value, obj);
                if (changed)
                {
                    anyChanged = true;
                }
            }

            if (anyChanged)
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();

                property.FindPropertyRelative("_editorAssetChanged").boolValue = false;
            }
        }

        private bool SetObject(SerializedProperty property, Object targetObject, MarrowAsset marrowAssetReference, Object obj)
        {
            Undo.RecordObject(targetObject, "Set MarrowAssetReference");
            bool changed = marrowAssetReference.SetEditorAsset(obj);
            if (changed)
                SetDirty(targetObject);
            return changed;
        }

        static void SetDirty(Object obj)
        {
            UnityEngine.GUI.changed = true;

            EditorUtility.SetDirty(obj);
            var comp = obj as Component;
            if (comp != null && comp.gameObject != null && comp.gameObject.activeInHierarchy && !Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);
        }

        private int ValidDropsCount()
        {
            int validDrops = 0;
            foreach (var dropObjs in DragAndDrop.objectReferences)
            {
                if (marrowAssetActualType != null && marrowAssetActualType.IsInstanceOfType(dropObjs))
                    validDrops++;
            }
            return validDrops;
        }

        private void HandleDrop(SerializedProperty property)
        {
            if (isDropping)
            {
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    if (property.TryFindParentArrayProperty(out var parentProp))
                    {
                        int insertIndex = parentProp.arraySize;
                        try
                        {
                            insertIndex = int.Parse(property.propertyPath.Split('[').LastOrDefault().TrimEnd(']')) + 1;
                        }
                        catch { }

                        foreach (var dropObj in DragAndDrop.objectReferences)
                        {
                            if (dropObj != null && marrowAssetActualType != null && marrowAssetActualType.IsInstanceOfType(dropObj))
                            {
                                parentProp.InsertArrayElementAtIndex(insertIndex);
                                var guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(dropObj)).ToString();
                                parentProp.GetArrayElementAtIndex(insertIndex).FindPropertyRelative("_assetGUID").stringValue = guid;
                                insertIndex++;
                            }
                        }
                        DragAndDrop.AcceptDrag();
                    }
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(MarrowAssetT<>))]
    public class MarrowAssetTPropertyDrawer : MarrowAssetPropertyDrawer { }

    [CustomPropertyDrawer(typeof(MarrowGameObject))]
    public class MarrowGameObjectPropertyDrawer : MarrowAssetPropertyDrawer { }

    [CustomPropertyDrawer(typeof(MarrowMesh))]
    public class MarrowMeshPropertyDrawer : MarrowAssetPropertyDrawer { }

    [CustomPropertyDrawer(typeof(MarrowTexture))]
    public class MarrowTexturePropertyDrawer : MarrowAssetPropertyDrawer { }

    [CustomPropertyDrawer(typeof(MarrowTexture2D))]
    public class MarrowTexture2DPropertyDrawer : MarrowAssetPropertyDrawer { }

    [CustomPropertyDrawer(typeof(MarrowScene))]
    public class MarrowScenePropertyDrawer : MarrowAssetPropertyDrawer { }

}
