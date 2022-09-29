using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using SLZ.Marrow.Warehouse;
using System.Linq;

#if UNITY_EDITOR

namespace SLZ.MarrowEditor
{
    public class AssetWarehouseTagsPopupWindow : PopupWindowContent
    {
        TreeViewState treeViewState;
        AssetWarehouseTreeView treeViewAW;
        private Vector2 toolScrollPos;
        int buttonsPerRow = 4;
        int padding = 5;
        Vector2 buttonSize = new Vector2(100, 20);

        public AssetWarehouseTagsPopupWindow(AssetWarehouseTreeView treeViewAW)
        {
            this.treeViewAW = treeViewAW;
        }

        void OnEnable()
        {
            if (treeViewState == null)
                treeViewState = new TreeViewState();

            AssetWarehouse.OnReady(() =>
            {
                if (treeViewAW == null)
                {
                    treeViewAW = new AssetWarehouseTreeView(treeViewState);
                    treeViewAW.ExpandAll();
                }
                AssetWarehouse.Instance.OnChanged += RefreshTree;
            });
        }

        public override Vector2 GetWindowSize()
        {

            return new Vector2(((treeViewAW.uniqueTags.Count * buttonSize.x) / (treeViewAW.uniqueTags.Count / buttonsPerRow)) + (buttonSize.x / 4), ((treeViewAW.uniqueTags.Count / buttonsPerRow) * (buttonSize.y + padding) + (buttonSize.y + padding)));
        }

        public override void OnGUI(Rect rect)
        {

            padding = 5;
            GUIStyle style = new GUIStyle();
            style.padding = new RectOffset(padding, padding, padding, padding);

            toolScrollPos = EditorGUILayout.BeginScrollView(toolScrollPos, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope(style))
            {

                EditorGUI.BeginChangeCheck();

                if (treeViewAW != null)
                {

                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("Tags: ", GUILayout.Width(50));

                    if (GUILayout.Button(new GUIContent("All", "Select All Tags"), MarrowGUIStyles.DefaultButton))
                    {
                        foreach (var kvp in treeViewAW.uniqueTags.ToArray())
                        {
                            treeViewAW.uniqueTags[kvp.Key] = true;
                        }
                    }

                    if (GUILayout.Button(new GUIContent("None", "Deselect All Tags"), MarrowGUIStyles.DefaultButton))
                    {
                        foreach (var kvp in treeViewAW.uniqueTags.ToArray())
                        {
                            treeViewAW.uniqueTags[kvp.Key] = false;
                        }
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(new GUIContent("X", "Close Tags Window"), MarrowGUIStyles.DefaultButton))
                    {
                        editorWindow.Close();
                    }
                    EditorGUILayout.EndHorizontal();

                    int buttonCount = 0;

                    foreach (var kvp in treeViewAW.uniqueTags.ToArray())
                    {

                        if (buttonCount % buttonsPerRow == 0)
                        {
                            EditorGUILayout.BeginHorizontal();
                        }

                        treeViewAW.uniqueTags[kvp.Key] = GUILayout.Toggle(treeViewAW.uniqueTags[kvp.Key], new GUIContent(kvp.Key.ToString(), "Filter by Tag: " + kvp.Key.ToString()), MarrowGUIStyles.DefaultButton, GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y));

                        if (buttonCount % buttonsPerRow == buttonsPerRow - 1)
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                        buttonCount++;

                    }

                    if (buttonCount % buttonsPerRow != 0 && buttonCount % buttonsPerRow > 4)
                    {
                        EditorGUILayout.EndHorizontal();
                    }

                }

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyCrateFilters();
                }

            }

            EditorGUILayout.EndScrollView();
        }

        private void ApplyCrateFilters()
        {
            if (!treeViewAW.list)
            {
                RefreshTree();
                treeViewAW.CollapseAll();

                for (int t = 0; t < treeViewAW.orderedObjs.Count; t++)
                {
                    if (treeViewAW.orderedObjs[t] != null && treeViewAW.showAvatars && treeViewAW.orderedObjs[t].GetType() == typeof(AvatarCrate))
                    {
                        treeViewAW.FrameItem(t);
                    }
                    if (treeViewAW.orderedObjs[t] != null && treeViewAW.showLevels && treeViewAW.orderedObjs[t].GetType() == typeof(LevelCrate))
                    {
                        treeViewAW.FrameItem(t);
                    }
                    if (treeViewAW.orderedObjs[t] != null && treeViewAW.showSpawnables && treeViewAW.orderedObjs[t].GetType() == typeof(SpawnableCrate))
                    {
                        treeViewAW.FrameItem(t);
                    }
                }
            }
            else
            {
                RefreshTree();
                treeViewAW.ExpandAll();
            }
        }

        private void RefreshTree()
        {
            if (treeViewAW != null)
            {
                treeViewAW.Reload();
            }
        }
    }
}
#endif