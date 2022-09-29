using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    [CustomEditor(typeof(AvatarCrate))]
    [CanEditMultipleObjects]
    public class AvatarCrateEditor : SpawnableCrateEditor
    {
        protected override string AssetReferenceDisplayName { get { return "Avatar Prefab"; } }
    }

    [CustomPreview(typeof(AvatarCrate))]
    public class AvatarCratePreview : CratePreview { }
}