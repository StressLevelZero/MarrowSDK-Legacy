using UnityEngine;

namespace SLZ.Marrow.Interaction
{
#if UNITY_EDITOR
    public partial class MarrowBodyTracker : MonoBehaviour
    {
        public void Editor_Setup(MarrowBody body, MarrowEntity entity, Bounds bounds)
        {
            this.body = body;
            this.entity = entity;

            var cols = gameObject.GetComponents<Collider>();
            foreach (var col in cols)
            {
                DestroyImmediate(col);
            }

            trigger = gameObject.AddComponent<BoxCollider>();
            trigger.center = bounds.center;
            trigger.size = bounds.size;
            trigger.isTrigger = true;

            gameObject.layer = (int)MarrowLayers.Plug;
        }
    }
#endif
}