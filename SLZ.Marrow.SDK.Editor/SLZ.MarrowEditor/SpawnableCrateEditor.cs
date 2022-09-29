using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    [CustomEditor(typeof(SpawnableCrate))]
    [CanEditMultipleObjects]
    public class SpawnableCrateEditor : GameObjectCrateEditor
    {
        protected override string AssetReferenceDisplayName { get { return "Spawnable Prefab"; } }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUIPackedAssets()
        {
            base.OnInspectorGUIPackedAssets();
        }
    }

    [CustomPreview(typeof(SpawnableCrate))]
    public class SpawnableCratePreview : CratePreview { }
}