using UnityEngine;
using SLZ.Data;
using System;
using System.Linq;

namespace SLZ.VRMK
{
    [System.Serializable]
    [HelpURL("https://github.com/StressLevelZero/MarrowSDK/wiki/Avatars")]
    [AddComponentMenu("MarrowSDK/Avatar")]
    public partial class Avatar : MonoBehaviour
    {
        public Animator animator;
        [HideInInspector]
        public bool requiredBonesOrRefsMissing = false;
        [Tooltip("Use this to override eyeCenter in case an avatar doesn't feature eye bones")]
        public Transform eyeCenterOverride;

        [Tooltip("Required field for all meshes that are not seperate head or hair meshes")]
        public SkinnedMeshRenderer[] bodyMeshes;
        [Tooltip("Optional field for models with seperate head mesh(es)")]
        public SkinnedMeshRenderer[] headMeshes;
        [Tooltip("Optional field for all meshes related to the hair")]
        public SkinnedMeshRenderer[] hairMeshes;

        [Range(0f, .16f)]
        public float eyeOffset = .02f;
        public SoftEllipse wristEllipse { get { return _wristEllipse; } set { _wristEllipse = value; } }
        public SoftEllipse forearmEllipse { get { return _forearmEllipse; } set { _forearmEllipse = value; } }
        public SoftEllipse elbowEllipse { get { return _elbowEllipse; } set { _elbowEllipse = value; } }
        public SoftEllipse upperarmEllipse { get { return _upperarmEllipse; } set { _upperarmEllipse = value; } }
        public SoftEllipse ankleEllipse { get { return _ankleEllipse; } set { _ankleEllipse = value; } }
        public SoftEllipse calfEllipse { get { return _calfEllipse; } set { _calfEllipse = value; } }
        public SoftEllipse kneeEllipse { get { return _kneeEllipse; } set { _kneeEllipse = value; } }
        public SoftEllipse thighUpperEllipse { get { return _thighUpperEllipse; } set { _thighUpperEllipse = value; } }
        public Vector3 t7Local { get { return _t7Local; } }
        public Vector3 l1Local { get { return _l1Local; } }
        public Vector3 l3Local { get { return _l3Local; } }
        public Vector3 sacrumLocal { get { return _sacrumLocal; } }

        public float HeadTop { get { return _headTop; } set { _headTop = value; } }
        public float ChinY { get { return _chinY; } set { _chinY = value; } }
        public float UnderbustY { get { return _underbustY; } set { _underbustY = value; } }
        public float WaistY { get { return _waistY; } set { _waistY = value; } }
        public float HighHipsY { get { return _highHipY; } set { _highHipY = value; } }
        public float CrotchBottom { get { return _crotchBottom; } set { _crotchBottom = value; } }
        public float ForeheadEllipseZ { get { return _headEllipseZ; } set { _headEllipseZ = value; } }
        public float ForeheadEllipseNegZ { get { return _headEllipseNegZ; } set { _headEllipseNegZ = value; } }
        public float JawEllipseZ { get { return _jawEllipseZ; } set { _jawEllipseZ = value; } }
        public float JawEllipseNegZ { get { return _jawEllipseNegZ; } set { _jawEllipseNegZ = value; } }
        public float NeckEllipseZ { get { return _neckEllipseZ; } set { _neckEllipseZ = value; } }
        public float NeckEllipseNegZ { get { return _neckEllipseNegZ; } set { _neckEllipseNegZ = value; } }
        public float SternumEllipseZ { get { return _sternumEllipseZ; } set { _sternumEllipseZ = value; } }
        public float SternumEllipseNegZ { get { return _sternumEllipseNegZ; } set { _sternumEllipseNegZ = value; } }
        public float ChestEllipseZ { get { return _chestEllipseZ; } set { _chestEllipseZ = value; } }
        public float ChestEllipseNegZ { get { return _chestEllipseNegZ; } set { _chestEllipseNegZ = value; } }
        public float WaistEllipseZ { get { return _waistEllipseZ; } set { _waistEllipseZ = value; } }
        public float WaistEllipseNegZ { get { return _waistEllipseNegZ; } set { _waistEllipseNegZ = value; } }
        public float HighHipsEllipseZ { get { return _highHipsEllipseZ; } set { _highHipsEllipseZ = value; } }
        public float HighHipsEllipseNegZ { get { return _highHipsEllipseNegZ; } set { _highHipsEllipseNegZ = value; } }
        public float HipsEllipseZ { get { return _hipsEllipseZ; } set { _hipsEllipseZ = value; } }
        public float HipsEllipseNegZ { get { return _hipsEllipseNegZ; } set { _hipsEllipseNegZ = value; } }
        public float ForeheadEllipseX { get { return _headEllipseX; } set { _headEllipseX = value; } }
        public float JawEllipseX { get { return _jawEllipseX; } set { _jawEllipseX = value; } }
        public float NeckEllipseX { get { return _neckEllipseX; } set { _neckEllipseX = value; } }
        public float ChestEllipseX { get { return _chestEllipseX; } set { _chestEllipseX = value; } }
        public float WaistEllipseX { get { return _waistEllipseX; } set { _waistEllipseX = value; } }
        public float HighHipsEllipseX { get { return _highHipsEllipseX; } set { _highHipsEllipseX = value; } }
        public float HipsEllipseX { get { return _hipsEllipseX; } set { _hipsEllipseX = value; } }



        public Transform wristLf;
        public Transform wristRt;
        public Transform neck2;
        public Transform scapulaLf;
        public Transform scapulaRt;
        public Transform carpalLf;
        public Transform carpalRt;

        public Transform twistUpperArmLf;
        public Transform twistUpperArmRt;


        public Transform twistForearmLf;
        public Transform twistForearmRt;
        public Transform twistUpperThighLf;
        public Transform twistUpperThighRt;






        [Tooltip("Crown of head Y from EyeHeight / eyeHeight. Default is .068")]
        [Range(.02f, .2f)]
        [SerializeField]
        private float _headTop = .068f;
        [Range(.02f, .2f)]
        [SerializeField]
        private float _chinY = .068f;
        [Range(Constants.UnderbustYMin, Constants.UnderbustYMax)]
        [SerializeField]
        protected float _underbustY = Constants.UnderbustYDefault;
        [Range(Constants.WaistYMin, Constants.WaistYMax)]
        [SerializeField]
        protected float _waistY = Constants.WaistYDefault;
        [Range(Constants.HighHipYMin, Constants.HighHipYMax)]
        [SerializeField]
        protected float _highHipY = Constants.HighHipYDefault;
        [Tooltip("Bottom of crotch from hipCenter / eyeHeight. Default is .04")]
        [Range(Constants.CrotchBottomMin, Constants.CrotchBottomMax)]
        [SerializeField]
        private float _crotchBottom = Constants.CrotchBottomDefault;


        [Tooltip("Forehead Width on X / AvatarHeight. Default is .044")]
        [Range(.02f, .2f)]
        [SerializeField]
        private float _headEllipseX = .044f;
        [Tooltip("Jaw Width on X / AvatarHeight. Default is .044")]
        [Range(.02f, .2f)]
        [SerializeField]
        private float _jawEllipseX = .044f;
        [Tooltip("Neck Width on X / AvatarHeight. Default is .04")]
        [Range(.018f, .08f)]
        [SerializeField]
        private float _neckEllipseX = .035f;
        [Tooltip("Chest Width on X / AvatarHeight. Default is .1")]
        [Range(.04f, .24f)]
        [SerializeField]
        private float _chestEllipseX = .1f;
        [Tooltip("Waist Width on X / AvatarHeight. Default is .09")]
        [Range(.04f, .24f)]
        [SerializeField]
        private float _waistEllipseX = .091f;
        [Tooltip("High Hips Width on X / AvatarHeight. Default is .09")]
        [Range(.04f, .24f)]
        [SerializeField]
        private float _highHipsEllipseX = .091f;
        [Tooltip("Hips Width on X / AvatarHeight. Default is .09")]
        [Range(.04f, .24f)]
        [SerializeField]
        private float _hipsEllipseX = .091f;


        [Tooltip("Forehead Z Fwd / AvatarHeight. Default is .06")]
        [Range(.02f, .2f)]
        [SerializeField]
        private float _headEllipseZ = .06f;
        [Tooltip("Jaw Z Fwd / AvatarHeight. Default is .06")]
        [Range(.02f, .2f)]
        [SerializeField]
        private float _jawEllipseZ = .06f;
        [Tooltip("Neck Z / AvatarHeight. Default is .04")]
        [Range(.018f, .08f)]
        [SerializeField]
        private float _neckEllipseZ = .04f;
        [Tooltip("Sternum Z / AvatarHeight. Default is .068")]
        [Range(.01f, .24f)]
        [SerializeField]
        private float _sternumEllipseZ = .052f;
        [Tooltip("Chest Width on Z / AvatarHeight. Default is .068")]
        [Range(.01f, .24f)]
        [SerializeField]
        private float _chestEllipseZ = .076f;
        [Tooltip("Waist Width on Z / AvatarHeight. Default is .068")]
        [Range(.01f, .24f)]
        [SerializeField]
        private float _waistEllipseZ = .075f;
        [Tooltip("High Hips Width on Z / AvatarHeight. Default is .068")]
        [Range(.01f, .24f)]
        [SerializeField]
        private float _highHipsEllipseZ = .07f;
        [Tooltip("Hips Width on Z / AvatarHeight. Default is .068")]
        [Range(.01f, .24f)]
        [SerializeField]
        private float _hipsEllipseZ = .07f;
        [Tooltip("Forehead Z Neg / AvatarHeight. Default is .06")]
        [Range(.02f, .2f)]
        [SerializeField]
        private float _headEllipseNegZ = .06f;
        [Tooltip("Jaw Z Neg / AvatarHeight. Default is .06")]
        [Range(.02f, .2f)]
        [SerializeField]
        private float _jawEllipseNegZ = .06f;
        [Tooltip("Neck Z / AvatarHeight. Default is .03")]
        [Range(.018f, .08f)]
        [SerializeField]
        private float _neckEllipseNegZ = .03f;
        [Tooltip("Sternum Neg Z / AvatarHeight. Default is .068")]
        [Range(.01f, .22f)]
        [SerializeField]
        private float _sternumEllipseNegZ = .052f;
        [Tooltip("Chest Width on Neg Z / AvatarHeight. Default is .068")]
        [Range(.01f, .22f)]
        [SerializeField]
        private float _chestEllipseNegZ = .07f;
        [Tooltip("Waist Width on Neg Z / AvatarHeight. Default is .06")]
        [Range(.01f, .22f)]
        [SerializeField]
        private float _waistEllipseNegZ = .06f;
        [Tooltip("High Hips Width on Neg Z / AvatarHeight. Default is .07")]
        [Range(.01f, .22f)]
        [SerializeField]
        private float _highHipsEllipseNegZ = .07f;
        [Tooltip("Hips Width on Neg Z / AvatarHeight. Default is .068")]
        [Range(.01f, .22f)]
        [SerializeField]
        private float _hipsEllipseNegZ = .08f;










        [SerializeField]
        private SoftEllipse _thighUpperEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);
        [SerializeField]
        private SoftEllipse _kneeEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);
        [SerializeField]
        private SoftEllipse _calfEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);
        [SerializeField]
        private SoftEllipse _ankleEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);
        [SerializeField]
        private SoftEllipse _upperarmEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);
        [SerializeField]
        private SoftEllipse _elbowEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);
        [SerializeField]
        private SoftEllipse _forearmEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);
        [SerializeField]
        private SoftEllipse _wristEllipse = new SoftEllipse(.03f, 0f, .03f, 0f);





        public SoftBulge bulgeBreast;
        public SoftBulge bulgeUpperBack;
        public SoftBulge bulgeAbdomen;
        public SoftBulge bulgeLowerBack;
        public SoftBulge bulgeGroin;
        public SoftBulge bulgeButt;


        public AudioVarianceData footstepsWalk;
        public AudioVarianceData footstepsJog;
        public AudioVarianceData highFallOntoFeet;




        public AudioVarianceData smallEffort;
        public AudioVarianceData bigEffort;
        public AudioVarianceData smallPain;
        public AudioVarianceData bigPain;
        public AudioVarianceData dying;
        public AudioVarianceData dead;
        public AudioVarianceData recovery;
        protected Vector3 _t7Local;
        protected Vector3 _l1Local;
        protected Vector3 _l3Local;
        protected Vector3 _sacrumLocal;
        protected float _t7OffsetZ;
        protected float _l1OffsetZ;
        protected float _l3OffsetZ;
        protected float _t7Y;
        protected float _l1Y;
        protected float _l3Y;


        public class Constants
        {
            public const float UnderbustYMin = 0.3f;
            public const float UnderbustYMax = 0.4f;
            public const float UnderbustYDefault = 0.35f;

            public const float WaistYMin = 0.5f;
            public const float WaistYMax = 0.7f;
            public const float WaistYDefault = 0.62f;

            public const float HighHipYMin = 0.75f;
            public const float HighHipYMax = 0.85f;
            public const float HighHipYDefault = 0.81f;

            public const float CrotchBottomMin = 0.02f;
            public const float CrotchBottomMax = 0.2f;
            public const float CrotchBottomDefault = 0.04f;
        }

        [System.Serializable]
        public class ArtOffsets
        {
        }

        [System.Serializable]
        public struct HandSchematic
        {
        }
        public static void GuessLimbAxis(Vector3 limbDown, Quaternion boneRotInRoot, out Vector3 fwd, out Vector3 up)
        {
            Vector3 limbDownInBone = Quaternion.Inverse(boneRotInRoot) * limbDown;
            if (Mathf.Abs(limbDownInBone.x) > Mathf.Abs(limbDownInBone.y))
            {
                if (Mathf.Abs(limbDownInBone.x) > Mathf.Abs(limbDownInBone.z))
                { fwd = Vector3.right * Mathf.Sign(limbDownInBone.x); }
                else
                { fwd = Vector3.forward * Mathf.Sign(limbDownInBone.z); }
            }
            else
            {
                if (Mathf.Abs(limbDownInBone.y) > Mathf.Abs(limbDownInBone.z))
                { fwd = Vector3.up * Mathf.Sign(limbDownInBone.y); }
                else
                { fwd = Vector3.forward * Mathf.Sign(limbDownInBone.z); }
            }
            float dotX = Vector3.Dot(boneRotInRoot * Vector3.right, Vector3.forward);
            float dotY = Vector3.Dot(boneRotInRoot * Vector3.up, Vector3.forward);
            float dotZ = Vector3.Dot(boneRotInRoot * Vector3.forward, Vector3.forward);
            if (Mathf.Abs(dotX) > Mathf.Abs(dotY))
            {
                if (Mathf.Abs(dotX) > Mathf.Abs(dotZ))
                { up = Vector3.right * Mathf.Sign(dotX); }
                else
                { up = Vector3.forward * Mathf.Sign(dotZ); }
            }
            else
            {
                if (Mathf.Abs(dotY) > Mathf.Abs(dotZ))
                { up = Vector3.up * Mathf.Sign(dotY); }
                else
                { up = Vector3.forward * Mathf.Sign(dotZ); }
            }
        }
        private void Reset()
        {
            animator = GetComponent<Animator>();

            if (animator != null)
            {
                var handLeft = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                if (handLeft != null)
                {
                    wristLf = handLeft;
                }
                var handRight = animator.GetBoneTransform(HumanBodyBones.RightHand);
                if (handRight != null)
                {
                    wristRt = handRight;
                }

                var skinnedMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (skinnedMeshes.Length == 1)
                    bodyMeshes = skinnedMeshes.ToArray();
                if (skinnedMeshes.Length > 0)
                {
                    bodyMeshes = skinnedMeshes.Where(skinnedMesh => !skinnedMesh.name.Contains("head", StringComparison.OrdinalIgnoreCase)).ToArray();
                }
            }
        }
    }
}
