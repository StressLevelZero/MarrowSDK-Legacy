using System;
using UnityEditor;
using UnityEngine;

namespace SLZ.Marrow.Interaction
{
    public partial class MarrowEntity
    {
#if UNITY_EDITOR
        public void Editor_Bake()
        {

            var rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rigidbodies)
            {

                if (rb.gameObject.TryGetComponent<MarrowBody>(out var mb))
                    continue;

                rb.hideFlags |= HideFlags.HideInInspector;

                mb = rb.gameObject.AddComponent<MarrowBody>();
                mb.Editor_SetRigidbody(rb, true);
            }


            var marrowBodies = gameObject.GetComponentsInChildren<MarrowBody>();
            foreach (var mb in marrowBodies)
            {
                if (!mb.gameObject.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb = mb.gameObject.AddComponent<Rigidbody>();
                    rb.hideFlags |= HideFlags.HideInInspector;
                }

                mb.Editor_SetRigidbody(rb, false);
                mb.Editor_ResetColliders();

                EditorUtility.SetDirty(mb);
                AssetDatabase.SaveAssetIfDirty(mb);

                EditorUtility.SetDirty(rb);
                AssetDatabase.SaveAssetIfDirty(rb);
            }

            _staticColliders = Array.Empty<Collider>();

            var colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {

                if (col.isTrigger)
                {
                    continue;
                }


                if (col.attachedRigidbody == null)
                {
                    Editor_AddCollider(col);
                    continue;
                }

                if (!col.attachedRigidbody.gameObject.TryGetComponent<MarrowBody>(out var mb))
                    continue;

                mb.Editor_AddCollider(col);

                EditorUtility.SetDirty(mb);
                AssetDatabase.SaveAssetIfDirty(mb);
            }


            var configJoints = gameObject.GetComponentsInChildren<ConfigurableJoint>();
            foreach (var cj in configJoints)
            {

                bool hasMarrowJoint = false;
                foreach (var curMj in cj.gameObject.GetComponents<MarrowJoint>())
                {
                    hasMarrowJoint = curMj.TryGetConfigurableJoint(out var otherCj) && cj == otherCj;
                    if (hasMarrowJoint) break;
                }


                if (hasMarrowJoint)
                    continue;

                cj.hideFlags |= HideFlags.HideInInspector;

                var mj = cj.gameObject.AddComponent<MarrowJoint>();
                mj.Editor_SetConfigurableJoint(cj, true);

                EditorUtility.SetDirty(mj);
                AssetDatabase.SaveAssetIfDirty(mj);

                EditorUtility.SetDirty(cj);
                AssetDatabase.SaveAssetIfDirty(cj);
            }



            var marrowJoints = gameObject.GetComponentsInChildren<MarrowJoint>();
            foreach (var mj in marrowJoints)
            {


                bool hasConfigJoint = false;
                foreach (var curCj in mj.gameObject.GetComponents<ConfigurableJoint>())
                {
                    hasConfigJoint = mj.TryGetConfigurableJoint(out var otherCj) && curCj == otherCj;
                    if (hasConfigJoint) break;
                }


                if (hasConfigJoint)
                    continue;

                var cj = mj.gameObject.AddComponent<ConfigurableJoint>();
                cj.hideFlags |= HideFlags.HideInInspector;

                mj.Editor_SetConfigurableJoint(cj, false);

                EditorUtility.SetDirty(mj);
                AssetDatabase.SaveAssetIfDirty(mj);

                EditorUtility.SetDirty(cj);
                AssetDatabase.SaveAssetIfDirty(cj);
            }


            _bodies = GetComponentsInChildren<MarrowBody>();
            foreach (var mb in _bodies)
            {
                mb.Editor_SetEntity(this);
                mb.Editor_CalculateBounds();
                mb.Editor_AddTracker();


                if (_anchorBody == null)
                {
                    _anchorBody = mb;
                    continue;
                }

                if (!_anchorBody.TryGetRigidbody(out var aBody) || !mb.TryGetRigidbody(out var bBody))
                    continue;

                if (aBody.mass < bBody.mass)
                    _anchorBody = mb;
            }

            _joints = GetComponentsInChildren<MarrowJoint>();
            foreach (var joint in _joints)
            {
                joint.Editor_SetObject(this);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Editor_AddCollider(Collider col)
        {
            Array.Resize(ref _staticColliders, _staticColliders.Length + 1);
            _staticColliders[^1] = col;
        }
#endif
    }
}