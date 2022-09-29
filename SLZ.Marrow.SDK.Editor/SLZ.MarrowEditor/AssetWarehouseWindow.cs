using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Cysharp.Threading.Tasks;
using SLZ.Marrow;
using SLZ.Marrow.Warehouse;
using System.Linq;

namespace SLZ.MarrowEditor
{
    public class AssetWarehouseWindow : EditorWindow
    {
        [SerializeField] TreeViewState treeViewState;
        AssetWarehouseTreeView treeViewAW;

        private string searchString = "";
        private Rect warehouseRect;
        private Rect searchBarRect;
        private Vector2 warehouseScrollPos;
        private Rect tagsButtonRect;









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
                    this.Repaint();
                }
                AssetWarehouse.Instance.OnChanged += RefreshTree;
            });
        }

        private void OnDisable()
        {
            AssetWarehouse.Instance.OnChanged -= RefreshTree;
        }

        void OnGUI()
        {
            int padding = 5;
            GUIStyle style = new GUIStyle();
            style.padding = new RectOffset(padding, padding, padding, padding);

            using (new GUILayout.VerticalScope(style))
            {

                if (AssetWarehouse.Instance == null)
                {
                    EditorGUILayout.HelpBox("AssetWarehouse is NULL!", MessageType.Error);
                }
                else
                {
                    if (AssetWarehouse.Instance.GetPallets().Count == 0)
                    {
                        GUILayout.Label(new GUIContent("WARNING! No Pallet Detected! Please create one", "Pallets are the root of all Bonelab mods."), EditorStyles.wordWrappedLabel);

                        GUILayout.Space(5);

                        if (treeViewAW != null)
                        {
                            if (GUILayout.Button(new GUIContent(" Create Pallet", treeViewAW.palletIcon, "Create a Pallet"), MarrowGUIStyles.DefaultButton))
                            {
                                EditorApplication.ExecuteMenuItem("Assets/New Pallet");
                            }
                        }
                    }

                    GUILayout.Space(5);

                    using (new GUILayout.HorizontalScope())
                    {
                        if (treeViewAW != null)
                        {
                            GUIContent treeListTextOff = new GUIContent("Tree View", "Toggle between Tree and List View");
                            GUIContent treeListTextOn = new GUIContent("List View", "Toggle between Tree and List View");

                            EditorGUI.BeginChangeCheck();
                            treeViewAW.list = GUILayout.Toggle(treeViewAW.list, treeViewAW.list ? treeListTextOff : treeListTextOn, MarrowGUIStyles.DefaultButton);

                            if (EditorGUI.EndChangeCheck())
                            {
                                UpdateSearch();
                                treeViewAW.ExpandAll();
                            }
                        }

                        if (GUILayout.Button(new GUIContent("Refresh", "Reload the AssetWarehouse"), MarrowGUIStyles.DefaultButton))
                        {
                            AssetWarehouse.Instance.Clear();
                            RefreshTree();

                            AssetWarehouse.Instance.InitAsync().ContinueWith(RefreshTree).Forget();
                        }

                        if (GUILayout.Button(new GUIContent("Docs", "Open the Asset Warehouse Documentation in the default web browser."), MarrowGUIStyles.DefaultButton))
                        {
                            Application.OpenURL("https://github.com/StressLevelZero/BoneMods-Internal/blob/Greased-Scotsman-docs-update/Docs/PalletsAndCrates.md");
                        }


                        GUILayout.FlexibleSpace();

                        bool disablePackButtons = false;

                        if (AssetWarehouse.Instance.GetPallets().Count > 1)
                        {
                            disablePackButtons = true;
                        }

                        foreach (Pallet pal in AssetWarehouse.Instance.GetPallets())
                        {
                            if (treeViewAW != null)
                            {
                                if (!pal.Internal)
                                {
                                    if (disablePackButtons == false)
                                    {
                                        if (GUILayout.Button(new GUIContent(" Pack " + pal.Title, treeViewAW.palletIcon, "Build the pallet into a mod"), MarrowGUIStyles.DefaultButton))
                                        {
                                            bool success = PalletPackerEditor.PackPallet(pal);
                                            if (success)
                                            {
                                                ModBuilder.OpenContainingBuiltModFolder(pal);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        GUI.enabled = false;
                                        if (GUILayout.Button(new GUIContent(" WARNING! Multiple Pallets Detected! Pack Disabled", treeViewAW.palletIcon, "Building the Pallet into a mod is disabled, multiple Pallets detected."), MarrowGUIStyles.DefaultButton))
                                        {

                                        }
                                        GUI.enabled = true;
                                    }
                                }

                            }
                        }

                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        if (treeViewAW != null)
                        {
                            using (new GUILayout.VerticalScope())
                            {
                                GUILayout.Space(10);

                                using (new GUILayout.HorizontalScope())
                                {

                                    EditorGUI.BeginChangeCheck();
                                    searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.MaxWidth(position.width - 60));
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        UpdateSearch();
                                        treeViewAW.ExpandAll();
                                    }

                                    string searchTooltip = "";
                                    searchTooltip = "Search for a Crate by its title or search for a full barcode";
                                    EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", searchTooltip));


                                    if (GUILayout.Button(new GUIContent("Clear", "Clear the Search Bar"), MarrowGUIStyles.DefaultButton))
                                    {
                                        searchString = "";
                                        UpdateSearch();
                                        treeViewAW.ExpandAll();
                                    }
                                }

                                searchBarRect = GUILayoutUtility.GetLastRect();
                                warehouseRect = new Rect(0, 0, position.width, treeViewAW.totalHeight);

                                warehouseScrollPos = EditorGUILayout.BeginScrollView(warehouseScrollPos, false, false);

                                treeViewAW.OnGUI(warehouseRect);
                                GUILayout.Space(treeViewAW.totalHeight);

                                EditorGUILayout.EndScrollView();
                            }
                        }
                    }
                }

                using (new GUILayout.VerticalScope())
                {


                    GUILayout.FlexibleSpace();

                    GUILayout.Label("Asset Warehouse Toolbox", EditorStyles.boldLabel);

                    GUILayout.Space(5);

                    EditorGUI.BeginChangeCheck();

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Filters: ", GUILayout.Width(50));

                        if (GUILayout.Button(new GUIContent("All", "Expand the AssetWarehouse Tree to view all items"), MarrowGUIStyles.DefaultButton))
                        {
                            if (treeViewAW != null)
                            {
                                searchString = "";
                                UpdateSearch();

                                EnableAllFilters();

                                RefreshTree();
                                treeViewAW.ExpandAll();
                            }
                        }

                        if (treeViewAW != null)
                        {
                            treeViewAW.showAvatars = GUILayout.Toggle(treeViewAW.showAvatars, new GUIContent(treeViewAW.avatarIcon, "Show Avatars"), MarrowGUIStyles.DefaultButton);
                            treeViewAW.showLevels = GUILayout.Toggle(treeViewAW.showLevels, new GUIContent(treeViewAW.levelIcon, "Show Levels"), MarrowGUIStyles.DefaultButton);
                            treeViewAW.showSpawnables = GUILayout.Toggle(treeViewAW.showSpawnables, new GUIContent(treeViewAW.spawnableIcon, "Show Spawnables"), MarrowGUIStyles.DefaultButton);
                        }

                    }


                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Authors: ", GUILayout.Width(50));

                        if (treeViewAW != null)
                        {
                            foreach (var kvp in treeViewAW.uniqueAuthors.ToArray())
                            {
                                treeViewAW.uniqueAuthors[kvp.Key] = GUILayout.Toggle(treeViewAW.uniqueAuthors[kvp.Key], new GUIContent(kvp.Key.ToString(), "Filter by Author: " + kvp.Key.ToString()), MarrowGUIStyles.DefaultButton);
                            }
                        }

                    }


                    using (new GUILayout.HorizontalScope())
                    {
                        if (treeViewAW != null)
                        {
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


                            if (GUILayout.Button(new GUIContent("Choose Tags", "Open the Tag Selector"), MarrowGUIStyles.DefaultButton))
                            {
                                PopupWindow.Show(tagsButtonRect, new AssetWarehouseTagsPopupWindow(treeViewAW));
                            }
                            if (Event.current.type == EventType.Repaint) tagsButtonRect = GUILayoutUtility.GetLastRect();

                        }

                    }


                    if (EditorGUI.EndChangeCheck())
                    {
                        ApplyCrateFilters();
                    }


                    using (new GUILayout.HorizontalScope())
                    {
                        if (EditorPrefs.GetBool("UnlockEditingScannables", false))
                        {
                            if (GUILayout.Button("Init", MarrowGUIStyles.DefaultButton))
                            {
                                AssetWarehouse.Instance.InitAsync().ContinueWith(RefreshTree).Forget();
                            }

                            if (GUILayout.Button("Clear", MarrowGUIStyles.DefaultButton))
                            {
                                AssetWarehouse.Instance.Clear();
                                RefreshTree();
                            }
                        }

                    }

                }

            }
        }

        private void EnableAllFilters()
        {
            treeViewAW.showAvatars = true;
            treeViewAW.showLevels = true;
            treeViewAW.showSpawnables = true;

            foreach (var kvp in treeViewAW.uniqueTags.ToArray())
            {
                treeViewAW.uniqueTags[kvp.Key] = true;
            }
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

        private void UpdateSearch()
        {
            if (treeViewAW.list)
            {
                treeViewAW.searchString = searchString;
                treeViewAW.search = null;
            }
            else
            {
                treeViewAW.search = searchString;
                treeViewAW.searchString = null;
            }

            RefreshTree();

            if (!string.IsNullOrEmpty(searchString))
                treeViewAW.ExpandAll();
            else
                treeViewAW.CollapseAll();
        }

        private void RefreshTree()
        {
            if (treeViewAW != null)
            {
                treeViewAW.Reload();
            }
        }

        [MenuItem("Stress Level Zero/Void Tools/Asset Warehouse (" + MarrowSDK.SDK_VERSION + ")", false, 10)]
        static void ShowWindow()
        {
            var window = GetWindow<AssetWarehouseWindow>();
            window.titleContent = new GUIContent("Asset Warehouse");
            window.Show();
        }
    }
}