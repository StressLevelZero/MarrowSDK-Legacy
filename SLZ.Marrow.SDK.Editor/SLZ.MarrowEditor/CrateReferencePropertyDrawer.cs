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
    [CustomPropertyDrawer(typeof(CrateReference))]
    public class CrateReferencePropertyDrawer : PropertyDrawer
    {
        private GUIStyle buttonStyle = null;
        private Dictionary<UnityEngine.Object, CrateReference> targetReferences = new Dictionary<UnityEngine.Object, CrateReference>();
        private bool multiEdit = false;
        private bool targetReferencesSame = false;
        private Type crateRefType = null;
        private Type crateRefActualType = null;

        private bool isInArray;
        private bool isDragging;
        private bool isDropping;
        private bool isValidDrop;

        DisplayMode displayMode = DisplayMode.AUTO;
        enum DisplayMode
        {
            AUTO,
            BARCODE,
            CRATE_SELECTOR
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
                labelContent = new GUIContent(ObjectNames.NicifyVariableName((crateRefType == null ? "Marrow Asset" : crateRefType.Name) + pos.ToString()));
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
                CrateReference crateReference = property.GetActualObjectForSerializedProperty<CrateReference>(fieldInfo, ref labelText);

                if (crateReference == null)
                {
                    EditorGUI.LabelField(position, new GUIContent("------", "something broke blame k2"));
                }
                else
                {
                    SerializedProperty barcodeProp = property.FindPropertyRelative("_barcode").FindPropertyRelative("_id");

                    Crate editorCrate = crateReference.EditorCrate;
                    crateRefType = crateReference.GetType();
                    crateRefActualType = crateReference.CrateType;
                    bool crateExists = editorCrate != null;

                    switch (displayMode)
                    {
                        case DisplayMode.AUTO:
                            if (crateExists || !Barcode.IsValid(crateReference.Barcode))
                                DrawCrateReference(position, fullPosition, property);
                            else
                                DrawBarcode(position, fullPosition, barcodeProp);
                            break;
                        case DisplayMode.BARCODE:
                            DrawBarcode(position, fullPosition, barcodeProp);
                            break;
                        case DisplayMode.CRATE_SELECTOR:
                            DrawCrateReference(position, fullPosition, property);
                            break;
                    }

                }
            }
            else
            {
                var labelText = label.text;
                CrateReference crateReference = property.GetActualObjectForSerializedProperty<CrateReference>(fieldInfo, ref labelText);
                crateRefType = crateReference.GetType();

                SerializedProperty barcodeProp = property.FindPropertyRelative("_barcode").FindPropertyRelative("_id");

                foreach (var targetRef in targetReferences)
                {
                    var _ = targetRef.Value.EditorCrate;
                }

                switch (displayMode)
                {
                    case DisplayMode.AUTO:
                    case DisplayMode.CRATE_SELECTOR:
                        DrawCrateReference(position, fullPosition, property);
                        break;
                    case DisplayMode.BARCODE:
                        DrawBarcode(position, fullPosition, barcodeProp);
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
                    var crateRef = multiProp.GetActualObjectForSerializedProperty<CrateReference>(fieldInfo, ref labelText);
                    if (crateRef != null)
                    {
                        targetReferences.Add(targetObject, crateRef);
                        if (sameGUID == null)
                        {
                            sameGUID = crateRef.Barcode.ToString();
                        }
                        else if (same)
                        {
                            if (!sameGUID.Equals(crateRef.Barcode.ToString()))
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

        private void DrawCrateReference(Rect position, Rect fullPosition, SerializedProperty property)
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

            Crate crate;
            Type crateType = typeof(Crate);
            string barcode = Barcode.EMPTY;
            if (multiEdit && !targetReferencesSame)
            {
                crate = null;
            }
            else
            {
                CrateReference crateRef = targetReferences.First().Value;
                crate = crateRef.EditorCrate;
                crateType = crateRef.CrateType;
                barcode = crateRef.Barcode;
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
                        EditorGUI.HelpBox(dropBox, $"Insert {validDrops} {crateRefActualType?.Name}{extraS}", MessageType.Error);
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

            crate = (Crate)EditorGUI.ObjectField(fieldPos, GUIContent.none, crate, crateType, false);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeCrate(property, crate);
            }


            if (!string.IsNullOrEmpty(barcode))
            {
                EditorGUI.LabelField(fieldPos, new GUIContent("", barcode));
            }


            EditorGUI.LabelField(buttonPos, new GUIContent("", "Switch To Text Barcode"));
            if (GUI.Button(buttonPos, EditorGUIUtility.IconContent("InputField Icon"), buttonStyle))
            {
                displayMode = DisplayMode.BARCODE;
            }

        }

        private void DrawBarcode(Rect position, Rect fullPosition, SerializedProperty barcodeProp)
        {
            float buttonWidth = 20;
            Rect buttonPos = fullPosition;
            Rect fieldPos = position;

            buttonPos.width = buttonWidth;
            buttonPos.x = fullPosition.width + fullPosition.x - buttonWidth * 0.9f;

            fieldPos.width -= (buttonPos.width);

            EditorGUI.PropertyField(fieldPos, barcodeProp, GUIContent.none);
            EditorGUI.LabelField(fieldPos, new GUIContent("", barcodeProp.stringValue));

            EditorGUI.LabelField(buttonPos, new GUIContent("", "Switch To Crate Selector"));
            if (GUI.Button(buttonPos, EditorGUIUtility.IconContent("Selectable Icon"), buttonStyle))
            {
                displayMode = DisplayMode.CRATE_SELECTOR;
            }
        }

        private void ChangeCrate(SerializedProperty property, Crate crate)
        {
            bool anyChanged = false;
            foreach (var targetRef in targetReferences)
            {
                bool changed = SetCrate(property, targetRef.Key, targetRef.Value, crate);
                if (changed)
                {
                    anyChanged = true;
                }
            }

            if (anyChanged)
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();

                property.FindPropertyRelative("_editorCrateChanged").boolValue = false;
            }
        }

        private bool SetCrate(SerializedProperty property, Object targetObject, CrateReference crateReference, Crate crate)
        {
            Undo.RecordObject(targetObject, "Set Crate Reference");
            bool changed = crateReference.SetEditorCrate(crate);
            if (changed)
                SetDirty(targetObject);
            return changed;
        }

        static void SetDirty(Object obj)
        {
            UnityEngine.GUI.changed = true;

            EditorUtility.SetDirty(obj);
            var comp = obj as Component;
            if (comp != null && comp.gameObject != null && comp.gameObject.activeInHierarchy)
                EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);
        }

        private int ValidDropsCount()
        {
            int validDrops = 0;
            foreach (var dropObjs in DragAndDrop.objectReferences)
            {
                if (crateRefActualType != null && crateRefActualType.IsInstanceOfType(dropObjs))
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
                            if (dropObj != null && crateRefActualType != null && crateRefActualType.IsInstanceOfType(dropObj) && dropObj is Crate dropCrate)
                            {
                                parentProp.InsertArrayElementAtIndex(insertIndex);
                                parentProp.GetArrayElementAtIndex(insertIndex).FindPropertyRelative("_barcode").FindPropertyRelative("_id").stringValue = dropCrate.Barcode;
                                insertIndex++;
                            }
                        }
                        DragAndDrop.AcceptDrag();
                    }
                }
            }
        }

    }

    [CustomPropertyDrawer(typeof(GenericCrateReference))]
    public class GenericCrateReferencePropertyDrawer : CrateReferencePropertyDrawer { }


    [CustomPropertyDrawer(typeof(LevelCrateReference))]
    public class SceneCrateReferenceDrawer : CrateReferencePropertyDrawer { }


    [CustomPropertyDrawer(typeof(SpawnableCrateReference))]
    public class SpawnableCrateReferencePropertyDrawer : CrateReferencePropertyDrawer { }

    [CustomPropertyDrawer(typeof(AvatarCrateReference))]
    public class AvatarCrateReferencePropertyDrawer : CrateReferencePropertyDrawer { }

    [CustomPropertyDrawer(typeof(VFXCrateReference))]
    public class VFXCrateReferencePropertyDrawer : CrateReferencePropertyDrawer { }

}