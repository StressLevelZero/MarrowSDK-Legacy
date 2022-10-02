using Cysharp.Threading.Tasks;
using SLZ.Marrow;
using SLZ.Marrow.Warehouse;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace SLZ.MarrowEditor
{
    [Overlay(typeof(SceneView), id: ID_OVERLAY_AWSPAWNABLE, displayName: "AW Spawner")]
    [Icon("Assets/Resources/Icons/crate-ball.png")]

    public class AWSpawnerOverlayToolbar : Overlay
    {
        private const string ID_OVERLAY_AWSPAWNABLE = "aw-spawnable-overlay-toolbar";
        private const string COLLAPSED_TOOLTIP = "Select a Spawnable Crate from the Asset Warehouse and Drag it into the Scene";
        private const string EXPANDED_TOOLTIP = "Select a Spawnable Crate from the Asset Warehouse";
        private const string PREFAB_PATH = "Assets/Prefabs/Spawnable Placer (Template).prefab";
        private SpawnableCrate currentCrate = null;
        GameObject spawnablePrefab = null;

        public override VisualElement CreatePanelContent()
        {
            VisualElement rootVisualElement = new VisualElement();

            Label dragSpawnableLabel = new Label();
            Button initializeOverlayButton = new Button();

            initializeOverlayButton.text = "Initialize Overlay";
            initializeOverlayButton.clickable.clicked += InitializeOverlay;

            if (this.collapsed)
            {
                dragSpawnableLabel.style.paddingTop = 5;
                dragSpawnableLabel.style.paddingBottom = 5;
                if (currentCrate)
                {
                    dragSpawnableLabel.text = "Drag To Scene";
                }
                else
                {
                    dragSpawnableLabel.text = "Select Spawnable";
                }
                dragSpawnableLabel.tooltip = COLLAPSED_TOOLTIP;
            }
            else
            {
                dragSpawnableLabel.style.paddingTop = 5;
                dragSpawnableLabel.style.paddingBottom = 5;
                dragSpawnableLabel.text = "Select AW\nSpawnable";
                dragSpawnableLabel.tooltip = EXPANDED_TOOLTIP;
            }

            Texture2D spawnableIcon = Resources.Load<Texture2D>("Icons/crate-ball");

            Image dragSpawnableTexture = new Image();

            if (this.collapsed)
            {
                dragSpawnableTexture.image = spawnableIcon;
                dragSpawnableTexture.tooltip = COLLAPSED_TOOLTIP;
            }
            else
            {
                dragSpawnableTexture.image = spawnableIcon;
                dragSpawnableTexture.tooltip = EXPANDED_TOOLTIP;
            }


            dragSpawnableTexture.RegisterCallback<MouseDownEvent>(evt =>
            {
                DragAndDrop.PrepareStartDrag();


                if (spawnablePrefab != null)
                {
                    DragAndDrop.StartDrag("Dragging");
                    DragAndDrop.objectReferences = new Object[] { spawnablePrefab };
                }
            });

            dragSpawnableTexture.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;


                if (spawnablePrefab != null && spawnablePrefab.GetComponent<SpawnableCratePlacer>() && spawnablePrefab.name != "Spawnable Placer (" + currentCrate.name.ToString() + ")")
                {
                    spawnablePrefab.name = "Spawnable Placer (" + currentCrate.name.ToString() + ")";
                    spawnablePrefab.GetComponent<SpawnableCratePlacer>().spawnableCrateReference = new SpawnableCrateReference(currentCrate.Barcode);
                    spawnablePrefab.GetComponent<MeshFilter>().sharedMesh = currentCrate.PreviewMesh.EditorAsset;
                }
            });

            dragSpawnableLabel.RegisterCallback<MouseDownEvent>(evt =>
            {
                DragAndDrop.PrepareStartDrag();


                if (spawnablePrefab != null)
                {
                    DragAndDrop.StartDrag("Dragging");
                    DragAndDrop.objectReferences = new Object[] { spawnablePrefab };
                }
            });

            dragSpawnableLabel.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;


                if (spawnablePrefab != null && spawnablePrefab.GetComponent<SpawnableCratePlacer>() && spawnablePrefab.name != "Spawnable Placer (" + currentCrate.name.ToString() + ")")
                {
                    spawnablePrefab.name = "Spawnable Placer (" + currentCrate.name.ToString() + ")";
                    spawnablePrefab.GetComponent<SpawnableCratePlacer>().spawnableCrateReference = new SpawnableCrateReference(currentCrate.Barcode);
                    spawnablePrefab.GetComponent<MeshFilter>().sharedMesh = currentCrate.PreviewMesh.EditorAsset;
                    PrefabUtility.RevertObjectOverride(spawnablePrefab, InteractionMode.AutomatedAction);
                }
            });


            rootVisualElement.Add(dragSpawnableLabel);
            rootVisualElement.Add(dragSpawnableTexture);
            rootVisualElement.Add(initializeOverlayButton);

            if (this.collapsed && currentCrate != null)
            {
                SetOverlayContents(dragSpawnableLabel, dragSpawnableTexture);


                this.collapsed = false;
                this.collapsed = true;
            }

            CheckOverlayPrefabInit(dragSpawnableLabel, dragSpawnableTexture, initializeOverlayButton);


            Selection.selectionChanged += () =>
            {


                CheckOverlayPrefabInit(dragSpawnableLabel, dragSpawnableTexture, initializeOverlayButton);

                if (File.Exists(PREFAB_PATH) && spawnablePrefab == null)
                {

                    SetSpawnablePrefab();
                }


                if (dragSpawnableLabel != null && spawnablePrefab != null && Selection.instanceIDs.Length == 1)
                {

                    if (Selection.activeObject is SpawnableCrate)
                    {
                        SpawnableCrate crateObj = (SpawnableCrate)Selection.activeObject;
                        currentCrate = crateObj;

                        dragSpawnableLabel.tooltip = "Click here and drag into the Scene to create a Spawnable Crate Placer for [" + crateObj.name.ToString() + "]";
                        dragSpawnableTexture.tooltip = "Click here and drag into the Scene to create a Spawnable Crate Placer for [" + crateObj.name.ToString() + "]";

                        SetOverlayContents(dragSpawnableLabel, dragSpawnableTexture);
                    }
                }
            };

            return rootVisualElement;
        }

        private void CheckOverlayPrefabInit(Label dragSpawnableLabel, Image dragSpawnableTexture, Button initializeOverlayButton)
        {
            if (!File.Exists(PREFAB_PATH))
            {
                spawnablePrefab = null;
                dragSpawnableLabel.style.display = DisplayStyle.None;
                dragSpawnableTexture.style.display = DisplayStyle.None;
                initializeOverlayButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                initializeOverlayButton.style.display = DisplayStyle.None;
                dragSpawnableLabel.style.display = DisplayStyle.Flex;
                dragSpawnableTexture.style.display = DisplayStyle.Flex;
            }
        }

        private void InitializeOverlay()
        {
            GameObject createPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            createPrefab.name = "Spawnable Placer (Template)";
            var box = createPrefab.GetComponent<BoxCollider>();
            Object.DestroyImmediate(box);


            CreateSpawnablePrefab(createPrefab);

            if (this.collapsed)
            {
                this.collapsed = false;
                this.collapsed = true;
            }
            else
            {
                this.collapsed = true;
                this.collapsed = false;
            }
        }

        private void CreateSpawnablePrefab(GameObject createPrefab)
        {
            if (!Directory.Exists("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            if (File.Exists(PREFAB_PATH))
            {

                Debug.Log("File already exists, skipping creation");
            }
            else
            {
                bool prefabCreated = false;

                createPrefab.AddComponent<SpawnableCratePlacer>();
                createPrefab.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterial;

                PrefabUtility.SaveAsPrefabAsset(createPrefab, PREFAB_PATH, out prefabCreated);


                if (prefabCreated == true)
                {
                    Object.DestroyImmediate(createPrefab);

                }
            }
        }

        private void SetOverlayContents(Label dragSpawnableLabel, Image dragSpawnableTexture)
        {
            dragSpawnableLabel.text = "Drag to Scene\n" + currentCrate.name.ToString();
            SetSpawnableIcon(dragSpawnableTexture, currentCrate).Forget();

            dragSpawnableLabel.style.paddingBottom = 42;
            dragSpawnableTexture.style.paddingTop = 38;
            dragSpawnableTexture.StretchToParentSize();
        }

        private async UniTaskVoid SetSpawnableIcon(Image dragSpawnableTexture, Crate crate)
        {
            if (crate.MainAsset.EditorAsset != null)
            {
                await UniTask.WaitUntil(() => !AssetPreview.IsLoadingAssetPreview(crate.MainAsset.EditorAsset.GetInstanceID()));
                if (AssetPreview.GetAssetPreview(crate.MainAsset.EditorAsset) != null)
                {
                    dragSpawnableTexture.image = AssetPreview.GetAssetPreview(crate.MainAsset.EditorAsset);
                }
            }
        }

        private void SetSpawnablePrefab()
        {
            if (spawnablePrefab == null)
            {
                spawnablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH);

                if (spawnablePrefab.GetComponent<SpawnableCratePlacer>() == null) { spawnablePrefab.AddComponent<SpawnableCratePlacer>(); }
                if (spawnablePrefab.GetComponent<MeshRenderer>() == null) { spawnablePrefab.AddComponent<MeshRenderer>().material = MarrowSDK.VoidMaterial; }
                if (spawnablePrefab.GetComponent<MeshFilter>() == null) { spawnablePrefab.AddComponent<MeshFilter>(); }

            }
        }

    }

}
#endif