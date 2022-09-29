using SLZ.Marrow.Warehouse;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using Cysharp.Threading.Tasks;
using System;
using Object = UnityEngine.Object;

namespace SLZ.MarrowEditor
{
    public class ScannableTreeViewItem : TreeViewItem
    {
        public string barcode = Barcode.EMPTY;
    }


    public class AssetWarehouseTreeView : TreeView
    {
        public List<Object> orderedObjs = new List<Object>();
        public string search = "";
        public bool list = false;
        public bool showAvatars = true;
        public bool showLevels = true;
        public bool showSpawnables = true;

        public Texture2D palletIcon;
        public Texture2D avatarIcon;
        public Texture2D levelIcon;
        public Texture2D spawnableIcon;

        public Dictionary<string, bool> uniqueTags = new Dictionary<string, bool>();
        public Dictionary<string, bool> uniqueAuthors = new Dictionary<string, bool>();


        public AssetWarehouseTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            AssetPreview.SetPreviewTextureCacheSize(10000);

            orderedObjs = new List<Object>();

            TreeViewItem root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            orderedObjs.Add(null);
            List<TreeViewItem> allItems = new List<TreeViewItem>();
            if (!list)
                allItems.Add(new TreeViewItem { id = 1, depth = 0, displayName = "Pallets" });


            orderedObjs.Add(null);
            List<Pallet> pallets = AssetWarehouse.Instance.GetPallets();
            pallets = pallets.OrderBy(x => x.Title).ToList();

            List<string> distinctTags = new List<string>();
            List<string> distinctAuthors = new List<string>();

            int id = 2;

            distinctAuthors = GetDistinctAuthorsFromPallets(pallets);


            for (var a = 0; a < distinctAuthors.Count; a++)
            {
                if (!uniqueAuthors.ContainsKey(distinctAuthors[a].ToString().ToLower()))
                {
                    uniqueAuthors.Add(distinctAuthors[a].ToString().ToLower(), true);
                }
            }


            distinctTags = GetDistinctTagsFromCrates(pallets);


            for (var u = 0; u < distinctTags.Count; u++)
            {
                if (!uniqueTags.ContainsKey(distinctTags[u].ToString().ToLower()))
                {
                    uniqueTags.Add(distinctTags[u].ToString().ToLower(), true);
                }
            }


            for (int p = 0; p < pallets.Count; p++)
            {

                List<Crate> avatars = new List<Crate>();
                List<Crate> levels = new List<Crate>();
                List<Crate> spawnables = new List<Crate>();

                for (int c = 0; c < pallets[p].Crates.Count; c++)
                {

                    if (pallets[p].Crates[c].GetType() == typeof(AvatarCrate))
                    {
                        avatarIcon = AssetPreview.GetMiniThumbnail(pallets[p].Crates[c]);
                    }
                    if (pallets[p].Crates[c].GetType() == typeof(LevelCrate))
                    {
                        levelIcon = AssetPreview.GetMiniThumbnail(pallets[p].Crates[c]);
                    }
                    if (pallets[p].Crates[c].GetType() == typeof(SpawnableCrate))
                    {
                        spawnableIcon = AssetPreview.GetMiniThumbnail(pallets[p].Crates[c]);
                    }


                    if (!Search(pallets[p].Crates[c].Barcode, "barcode") && !Search(pallets[p].Crates[c].Title, "title"))
                        continue;


                    if (uniqueAuthors != null && uniqueAuthors.Count > 0 && uniqueAuthors.Keys.Contains(pallets[p].Author.ToLower().ToString()) && uniqueAuthors[pallets[p].Author.ToLower().ToString()] == true)
                    {
                        if (pallets[p].Crates[c].Tags == null || pallets[p].Crates[c].Tags.Count == 0)
                        {

                            if (showAvatars && pallets[p].Crates[c].GetType() == typeof(AvatarCrate))
                            {
                                avatars.Add(pallets[p].Crates[c]);
                            }

                            if (showLevels && pallets[p].Crates[c].GetType() == typeof(LevelCrate))
                            {
                                levels.Add(pallets[p].Crates[c]);
                            }

                            if (showSpawnables && pallets[p].Crates[c].GetType() == typeof(SpawnableCrate))
                            {
                                spawnables.Add(pallets[p].Crates[c]);
                            }

                        }
                        else
                        {
                            for (int t = 0; t < pallets[p].Crates[c].Tags.Count; t++)
                            {
                                if (pallets[p].Crates[c].Tags.Count > 0 && uniqueTags.Keys.Contains(pallets[p].Crates[c].Tags[t].ToLower().ToString()) && uniqueTags[pallets[p].Crates[c].Tags[t].ToLower().ToString()] == true)
                                {

                                    if (showAvatars && pallets[p].Crates[c].GetType() == typeof(AvatarCrate) && !avatars.Contains(pallets[p].Crates[c]))
                                    {
                                        avatars.Add(pallets[p].Crates[c]);
                                    }

                                    if (showLevels && pallets[p].Crates[c].GetType() == typeof(LevelCrate) && !levels.Contains(pallets[p].Crates[c]))
                                    {
                                        levels.Add(pallets[p].Crates[c]);
                                    }

                                    if (showSpawnables && pallets[p].Crates[c].GetType() == typeof(SpawnableCrate) && !spawnables.Contains(pallets[p].Crates[c]))
                                    {
                                        spawnables.Add(pallets[p].Crates[c]);
                                    }
                                }

                            }

                        }
                    }
                }

                if (avatars.Count == 0 && levels.Count == 0 && spawnables.Count == 0 && !Search(pallets[p].Title, "title") && !Search(pallets[p].Barcode, "barcode"))
                    continue;


                int idCache = id;
                if (palletIcon == null)
                {
                    palletIcon = (Texture2D)AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(pallets[p]));
                }
                var palletTreeItem = new ScannableTreeViewItem { id = id, depth = (list ? 0 : 1), displayName = pallets[p].Title, barcode = pallets[p].Barcode };
                palletTreeItem.icon = (Texture2D)palletIcon;
                allItems.Add(palletTreeItem);
                orderedObjs.Add(pallets[p]);
                id++;

                avatars = avatars.OrderBy(crate => crate.Title).ToList();
                levels = levels.OrderBy(crate => crate.Title).ToList();
                spawnables = spawnables.OrderBy(crate => crate.Title).ToList();


                SetupCrateType(avatars, allItems, ref id, typeof(AvatarCrate));
                SetupCrateType(levels, allItems, ref id, typeof(LevelCrate));
                SetupCrateType(spawnables, allItems, ref id, typeof(SpawnableCrate));
            }

            SetupParentsAndChildrenFromDepths(root, allItems);

            return root;
        }

        private List<string> GetDistinctAuthorsFromPallets(List<Pallet> pallets)
        {
            List<string> distinctAuthors = new List<string>();



            for (int p = 0; p < pallets.Count; p++)
            {
                if (!distinctAuthors.Contains(pallets[p].Author))
                {
                    distinctAuthors.Add(pallets[p].Author);
                }
            }

            return distinctAuthors;
        }

        private List<string> GetDistinctTagsFromCrates(List<Pallet> pallets)
        {
            List<string> distinctTags = new List<string>();



            for (int p = 0; p < pallets.Count; p++)
            {
                for (int c = 0; c < pallets[p].Crates.Count; c++)
                {
                    for (int t = 0; t < pallets[p].Crates[c].Tags.Count; t++)
                    {
                        if (!distinctTags.Contains(pallets[p].Crates[c].Tags[t]))
                        {
                            distinctTags.Add(pallets[p].Crates[c].Tags[t]);
                        }
                    }
                }
            }

            return distinctTags;
        }

        private bool Search(string palletName, string searchType)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (searchType == "barcode" && palletName.ToLower() != search.ToLower())
                {
                    return false;
                }

                string[] searchSplit = search.Split(' ');
                foreach (string s in searchSplit)
                {

                    if (searchType == "title" && !palletName.ToLower().Contains(s.ToLower()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);

            var selectedObject = orderedObjs[id];
            if (selectedObject != null && AssetDatabase.CanOpenAssetInEditor(selectedObject.GetInstanceID()))
            {
                AssetDatabase.OpenAsset(selectedObject);
            }
        }


        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            var selectedObjects = selectedIds.Select(id => orderedObjs[id]).ToArray();
            if (selectedObjects.Length > 0)
            {
                Selection.objects = selectedObjects;
            }
        }

        private void SetupCrateType(List<Crate> crates, List<TreeViewItem> items, ref int id, System.Type crateType)
        {

            bool subListSetup = false;
            Texture2D crateIcon = null;
            for (int c = 0; c < crates.Count; c++)
            {
                if (crateIcon == null)
                {
                    crateIcon = AssetPreview.GetMiniThumbnail(crates[c]);
                }
                if (!subListSetup && !list)
                {

                    var crateTypeTreeItem = new TreeViewItem { id = id, depth = (list ? 0 : 2), displayName = Crate.GetCrateName(crateType) };
                    crateTypeTreeItem.icon = crateIcon;
                    items.Add(crateTypeTreeItem);
                    orderedObjs.Add(null);
                    id++;
                    subListSetup = true;
                }


                var crateTreeItem = new ScannableTreeViewItem { id = id, depth = (list ? 0 : 3), displayName = crates[c].Title, barcode = crates[c].Barcode };
                crateTreeItem.icon = crateIcon;



                SetCrateIcon(crateTreeItem, crates[c]).Forget();

                items.Add(crateTreeItem);
                orderedObjs.Add(crates[c]);
                id++;
            }
        }

        private async UniTaskVoid SetCrateIcon(TreeViewItem treeItem, Crate crate)
        {
            if (crate.MainAsset.EditorAsset != null)
            {
                await UniTask.WaitUntil(() => !AssetPreview.IsLoadingAssetPreview(crate.MainAsset.EditorAsset.GetInstanceID()));
                if (AssetPreview.GetAssetPreview(crate.MainAsset.EditorAsset) != null)
                    treeItem.icon = AssetPreview.GetAssetPreview(crate.MainAsset.EditorAsset);
            }
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return AssetWarehouse.ready;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            base.SetupDragAndDrop(args);
            if (AssetWarehouse.ready)
            {
                var dragItems = FindRows(args.draggedItemIDs);
                List<string> crateBarcodes = new List<string>();
                List<string> palletBarcodes = new List<string>();
                List<Object> allScannables = new List<Object>();
                foreach (var dragItem in dragItems)
                {
                    if (dragItem is ScannableTreeViewItem scannableItem)
                    {
                        if (AssetWarehouse.Instance.TryGetScannable(scannableItem.barcode, out var scannable))
                        {
                            allScannables.Add(scannable);
                            if (scannable is Pallet pallet)
                            {
                                palletBarcodes.Add(pallet.Barcode);
                            }
                            else if (scannable is Crate crate)
                            {
                                crateBarcodes.Add(crate.Barcode);
                            }
                        }
                    }
                }

                DragAndDrop.PrepareStartDrag();
                if (crateBarcodes.Count > 0)
                    DragAndDrop.SetGenericData("_cratesBarcodes", crateBarcodes);
                if (palletBarcodes.Count > 0)
                    DragAndDrop.SetGenericData("_palletBarcodes", palletBarcodes);
                DragAndDrop.objectReferences = allScannables.ToArray();
                DragAndDrop.StartDrag("Draggin scannables");
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            }
        }
    }
}