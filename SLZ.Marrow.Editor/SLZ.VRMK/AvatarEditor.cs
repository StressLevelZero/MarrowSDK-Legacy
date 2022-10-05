using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SLZ.MarrowEditor;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace SLZ.VRMK
{
    [CustomEditor(typeof(Avatar))]
    public class AvatarEditor : Editor
    {
        public bool _hardTorsoHandles;
        private bool _softTorsoHandles;
        private bool _lastHardTorsoHandles;
        private bool _lastSoftTorsoHandles;
        private int activeHandleLabel = 0;
        private string activeHandleLabelName = string.Empty;
        private Vector3 activeHandleLabelPosition = Vector3.zero;
        private Matrix4x4 activeHandleLabelMatrix = Matrix4x4.identity;
        private float snapValue = 0.001f;
        private GUIStyle boldCenteredStyle;

        private bool helpText = false;
        private bool modelImportText = false;
        private bool avatarScriptText = false;
        private bool fineTuningText = false;
        private bool avatarCrateText = false;



        private int padding = 5;

        private static AvatarEditor activeEditor = null;
        private Avatar avatar;


        private SerializedProperty animatorProp;
        private SerializedProperty eyeCenterOverrideProp;
        private SerializedProperty bodyMeshesProp;
        private SerializedProperty headMeshesProp;
        private SerializedProperty hairMeshesProp;
        private SerializedProperty eyeOffsetProp;

        private SerializedProperty wristLeftProp;
        private SerializedProperty wristRightProp;
        private SerializedProperty neck2Prop;
        private SerializedProperty scapulaLeftProp;
        private SerializedProperty scapulaRightProp;
        private SerializedProperty carpalLeftProp;
        private SerializedProperty carpalRightProp;

        private SerializedProperty twistUpperArmLeftProp;
        private SerializedProperty twistUpperArmRightProp;
        private SerializedProperty twistForearmLeftProp;
        private SerializedProperty twistForearmRightProp;
        private SerializedProperty twistUpperThighLeftProp;
        private SerializedProperty twistUpperThighRightProp;

        private SerializedProperty headTopProp;
        private SerializedProperty chinYProp;
        private SerializedProperty underbustYProp;
        private SerializedProperty waistYProp;
        private SerializedProperty highHipY;
        private SerializedProperty crotchBottomY;

        private SerializedProperty headEllipseXProp;
        private SerializedProperty jawEllipseXProp;
        private SerializedProperty neckEllipseXProp;
        private SerializedProperty chestEllipseXProp;
        private SerializedProperty waistEllipseXProp;
        private SerializedProperty highHipsEllipseXProp;
        private SerializedProperty hipsEllipseXProp;

        private SerializedProperty headEllipseZProp;
        private SerializedProperty jawEllipseZProp;
        private SerializedProperty neckEllipseZProp;
        private SerializedProperty sternumEllipseZProp;
        private SerializedProperty chestEllipseZProp;
        private SerializedProperty waistEllipseZProp;
        private SerializedProperty highHipsEllipseZProp;
        private SerializedProperty hipsEllipseZProp;
        private SerializedProperty headEllipseNegZProp;
        private SerializedProperty jawEllipseNegZProp;
        private SerializedProperty neckEllipseNegZProp;
        private SerializedProperty sternumEllipseNegZProp;
        private SerializedProperty chestEllipseNegZProp;
        private SerializedProperty waistEllipseNegZProp;
        private SerializedProperty highHipsEllipseNegZProp;
        private SerializedProperty hipsEllipseNegZProp;

        private SerializedProperty thighUpperEllipseProp;
        private SerializedProperty kneeEllipseProp;
        private SerializedProperty calfEllipseProp;
        private SerializedProperty ankleEllipseProp;
        private SerializedProperty upperarmEllipseProp;
        private SerializedProperty elbowEllipseProp;
        private SerializedProperty forearmEllipseProp;
        private SerializedProperty wristEllipseProp;

        private SerializedProperty bulgeBreastProp;
        private SerializedProperty bulgeUpperBackProp;
        private SerializedProperty bulgeAbdomenProp;
        private SerializedProperty bulgeLowerBackProp;
        private SerializedProperty bulgeGroinProp;
        private SerializedProperty bulgeButtProp;

        private SerializedProperty footstepWalkProp;
        private SerializedProperty footstepJogProp;
        private SerializedProperty highFallOnFeetProp;

        private SerializedProperty bigEffortProp;
        private SerializedProperty smallPainProp;
        private SerializedProperty bigPainProp;
        private SerializedProperty dyingProp;
        private SerializedProperty deadProp;
        private SerializedProperty recoveryProp;


        private static GUIStyle toggleButtonStyle = null;
        private static GUIStyle toggleButtonActiveStyle = null;

        public static GUIContent modifyBodyIcon = null;
        public static GUIContent modifySoftBodyIcon = null;
        public static GUIContent addAudioIcon = null;
        public static GUIContent eyeOverrideIcon = null;

        private bool meshesFoldout = true;
        private bool optionalBonesFoldout = false;
        private bool advancedFoldout = false;
        private bool twistFoldout = false;
        private bool softBulgeFoldout = false;
        private bool torsoFoldout = false;
        private bool limbFoldout = false;
        private bool headFoldout = false;
        private bool audioFoldout = true;

        private bool modifyEyeOverride = false;


        public virtual void OnEnable()
        {
            avatar = (Avatar)target;


            animatorProp = serializedObject.FindProperty(nameof(Avatar.animator));
            eyeCenterOverrideProp = serializedObject.FindProperty(nameof(Avatar.eyeCenterOverride));
            bodyMeshesProp = serializedObject.FindProperty(nameof(Avatar.bodyMeshes));
            headMeshesProp = serializedObject.FindProperty(nameof(Avatar.headMeshes));
            hairMeshesProp = serializedObject.FindProperty(nameof(Avatar.hairMeshes));
            eyeOffsetProp = serializedObject.FindProperty(nameof(Avatar.eyeOffset));

            wristLeftProp = serializedObject.FindProperty(nameof(Avatar.wristLf));
            wristRightProp = serializedObject.FindProperty(nameof(Avatar.wristRt));
            neck2Prop = serializedObject.FindProperty(nameof(Avatar.neck2));
            scapulaLeftProp = serializedObject.FindProperty(nameof(Avatar.scapulaLf));
            scapulaRightProp = serializedObject.FindProperty(nameof(Avatar.scapulaRt));
            carpalLeftProp = serializedObject.FindProperty(nameof(Avatar.carpalLf));
            carpalRightProp = serializedObject.FindProperty(nameof(Avatar.carpalRt));

            twistUpperArmLeftProp = serializedObject.FindProperty(nameof(Avatar.twistUpperArmLf));
            twistUpperArmRightProp = serializedObject.FindProperty(nameof(Avatar.twistUpperArmRt));
            twistForearmLeftProp = serializedObject.FindProperty(nameof(Avatar.twistForearmLf));
            twistForearmRightProp = serializedObject.FindProperty(nameof(Avatar.twistForearmRt));
            twistUpperThighLeftProp = serializedObject.FindProperty(nameof(Avatar.twistUpperThighLf));
            twistUpperThighRightProp = serializedObject.FindProperty(nameof(Avatar.twistUpperThighRt));

            headTopProp = serializedObject.FindProperty("_headTop");
            chinYProp = serializedObject.FindProperty("_chinY");
            underbustYProp = serializedObject.FindProperty("_underbustY");
            waistYProp = serializedObject.FindProperty("_waistY");
            highHipY = serializedObject.FindProperty("_highHipY");
            crotchBottomY = serializedObject.FindProperty("_crotchBottom");

            headEllipseXProp = serializedObject.FindProperty("_headEllipseX");
            jawEllipseXProp = serializedObject.FindProperty("_jawEllipseX");
            neckEllipseXProp = serializedObject.FindProperty("_neckEllipseX");
            chestEllipseXProp = serializedObject.FindProperty("_chestEllipseX");
            waistEllipseXProp = serializedObject.FindProperty("_waistEllipseX");
            highHipsEllipseXProp = serializedObject.FindProperty("_highHipsEllipseX");
            hipsEllipseXProp = serializedObject.FindProperty("_hipsEllipseX");

            headEllipseZProp = serializedObject.FindProperty("_headEllipseZ");
            jawEllipseZProp = serializedObject.FindProperty("_jawEllipseZ");
            neckEllipseZProp = serializedObject.FindProperty("_neckEllipseZ");
            sternumEllipseZProp = serializedObject.FindProperty("_sternumEllipseZ");
            chestEllipseZProp = serializedObject.FindProperty("_chestEllipseZ");
            waistEllipseZProp = serializedObject.FindProperty("_waistEllipseZ");
            highHipsEllipseZProp = serializedObject.FindProperty("_highHipsEllipseZ");
            hipsEllipseZProp = serializedObject.FindProperty("_hipsEllipseZ");
            headEllipseNegZProp = serializedObject.FindProperty("_headEllipseNegZ");
            jawEllipseNegZProp = serializedObject.FindProperty("_jawEllipseNegZ");
            neckEllipseNegZProp = serializedObject.FindProperty("_neckEllipseNegZ");
            sternumEllipseNegZProp = serializedObject.FindProperty("_sternumEllipseNegZ");
            chestEllipseNegZProp = serializedObject.FindProperty("_chestEllipseNegZ");
            waistEllipseNegZProp = serializedObject.FindProperty("_waistEllipseNegZ");
            highHipsEllipseNegZProp = serializedObject.FindProperty("_highHipsEllipseNegZ");
            hipsEllipseNegZProp = serializedObject.FindProperty("_highHipsEllipseNegZ");

            thighUpperEllipseProp = serializedObject.FindProperty("_thighUpperEllipse");
            kneeEllipseProp = serializedObject.FindProperty("_kneeEllipse");
            calfEllipseProp = serializedObject.FindProperty("_calfEllipse");
            ankleEllipseProp = serializedObject.FindProperty("_ankleEllipse");
            upperarmEllipseProp = serializedObject.FindProperty("_upperarmEllipse");
            elbowEllipseProp = serializedObject.FindProperty("_elbowEllipse");
            forearmEllipseProp = serializedObject.FindProperty("_forearmEllipse");
            wristEllipseProp = serializedObject.FindProperty("_wristEllipse");

            bulgeBreastProp = serializedObject.FindProperty(nameof(Avatar.bulgeBreast));
            bulgeUpperBackProp = serializedObject.FindProperty(nameof(Avatar.bulgeUpperBack));
            bulgeAbdomenProp = serializedObject.FindProperty(nameof(Avatar.bulgeAbdomen));
            bulgeLowerBackProp = serializedObject.FindProperty(nameof(Avatar.bulgeLowerBack));
            bulgeGroinProp = serializedObject.FindProperty(nameof(Avatar.bulgeGroin));
            bulgeButtProp = serializedObject.FindProperty(nameof(Avatar.bulgeButt));

            footstepWalkProp = serializedObject.FindProperty(nameof(Avatar.footstepsWalk));
            footstepJogProp = serializedObject.FindProperty(nameof(Avatar.footstepsJog));
            highFallOnFeetProp = serializedObject.FindProperty(nameof(Avatar.highFallOntoFeet));

            bigEffortProp = serializedObject.FindProperty(nameof(Avatar.bigEffort));
            smallPainProp = serializedObject.FindProperty(nameof(Avatar.smallPain));
            bigPainProp = serializedObject.FindProperty(nameof(Avatar.bigPain));
            dyingProp = serializedObject.FindProperty(nameof(Avatar.dying));
            deadProp = serializedObject.FindProperty(nameof(Avatar.dead));
            recoveryProp = serializedObject.FindProperty(nameof(Avatar.recovery));

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (toggleButtonStyle == null || toggleButtonActiveStyle == null)
            {
                toggleButtonStyle = new GUIStyle(MarrowGUIStyles.DefaultButton);
                toggleButtonActiveStyle = new GUIStyle(MarrowGUIStyles.DefaultButton);
                toggleButtonActiveStyle.normal.background = toggleButtonActiveStyle.active.background;
            }

            if (modifyBodyIcon == null || modifySoftBodyIcon == null || addAudioIcon == null || eyeOverrideIcon == null)
            {
                modifyBodyIcon = EditorGUIUtility.IconContent("d_AvatarMask On Icon");
                modifyBodyIcon.tooltip = "Show/Hide the Body Editor Handles";

                modifySoftBodyIcon = EditorGUIUtility.IconContent("d_AvatarMask Icon");
                modifySoftBodyIcon.tooltip = "Show/Hide the Soft Body Editor Handles";

                addAudioIcon = EditorGUIUtility.IconContent("d_Toolbar Plus@2x");
                addAudioIcon.tooltip = "Create New AudioDataVariance";

                eyeOverrideIcon = EditorGUIUtility.IconContent("d_animationvisibilitytoggleon@2x");
                eyeOverrideIcon.tooltip = "Override the eye bones, center of the eye";
            }

            using (new EditorGUILayout.VerticalScope())
            {
                helpText = EditorGUILayout.Foldout(helpText, "Usage Info");
                if (helpText)
                {
                    EditorGUILayout.LabelField("Avatars alter the player's appearance, size, strength and speed.\n"
                        + "\n"
                        + "Once the Avatar becomes a Prefab (see steps below), it is recommended to <i>directly edit the Prefab</i> when making changes or be sure to <i>Apply Overrides</i> made to the Avatar GameObject as changes are made.\n"
                        + "\n"
                        + "Avatars reference the Prefab in the Project window <i>not</i> the GameObject in the Scene.", MarrowGUIStyles.DefaultRichText);
                    EditorGUILayout.Space(padding);

                    using (new EditorGUI.IndentLevelScope())
                    {

                        modelImportText = EditorGUILayout.Foldout(modelImportText, "Importing A Model");
                        if (modelImportText)
                        {
                            EditorGUILayout.Space(padding);
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Select the model file (usually .FBX) from the Project window. In the Rig tab in the Inspector, set the <i>Animation Type</i> from <b>Generic</b> to <b>Humanoid</b>.\n"
                                                           + "\n"
                                                           + "Ensure the <i>Avatar Definition</i> is set to <b>Create From This Model</b> and Skin Weights are set to Standard, then click Apply.\n"
                                                           + "\n"
                                                           + "If Unity is unable to automatically map the bones, click the Configure button on the Rig tab and manually map the bones to their appropriate fields.\n"
                                                           + "\n"
                                                           + "If no Materials are visible on the Avatar, click the <i>Materials</i> tab in the Inspector and choose <b>Use External Materials (Legacy)</b> from the <i>Location</i> dropdown menu. This will automatically extract textures and materials into the folder with the .fbx and assign them to the model.\n"
                                                           + "\n"
                                                           + "Create a GameObject out of the FBX by dragging the FBX from the Project Window into the Scene view and reset the position of the GameObject's Transform to ensure it is at the origin (0,0,0).\n"
                                                           + "\n"
                                                           + "Turn this GameObject into its own Prefab by dragging it back into the Project Window. If prompted between creating a <i>Variant</i> or an <i>Original Prefab</i>, select <i>Original Prefab</i>.\n"
                                                           + "\n"
                                                           + "Open the Prefab by selecting it in the hierarchy and then clicking the <i>Open Prefab</i> button at the top of the Inspector.\n", MarrowGUIStyles.DefaultRichText);
                            }
                        }

                        avatarScriptText = EditorGUILayout.Foldout(avatarScriptText, "Completing the Avatar Script");
                        if (avatarScriptText)
                        {
                            EditorGUILayout.Space(padding);
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Add an Avatar script to the root of the Avatar Prefab.\n"
                                                           + "\n"
                                                           + "If the model does not have Eye Bones, add an Empty GameObject at the root of the Prefab for an Eye Center Override, reference this GameObject in the Eye Center Override field, and position it between the model's eyes, near the surface of the head mesh.\n"
                                                           + "\n"
                                                           + "Add at least one element to the Body Meshes list and select the model's Skinned Mesh Renderer. If the model has seperate Head or Hair meshes, add those to the appropriate list.\n"
                                                           + "\n"
                                                           + "Only the Wrist bones are required by the Avatar Script for basic functionality. If the model does not have specific Wrist bones, Hand bones usually work as a substitute.  However, proper wrist bones and completing the Avatar script as fully as possible is recommended for greater in-game fidelity.\n", MarrowGUIStyles.DefaultRichText);
                            }
                        }

                        fineTuningText = EditorGUILayout.Foldout(fineTuningText, "Fine-Tuning the Avatar");
                        if (fineTuningText)
                        {
                            EditorGUILayout.Space(padding);
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Once all required bones and Eye Center Override (if needed) are in place, a set of gizmos, appearing as draggable elipses, dots and handles will be displayed around the model.\n"
                                                           + "\n"
                                                           + "Click <b>HardTorso Handles</b> and adjust the gizmos by clicking and dragging the dots until they roughly line up with the countours of the avatar's mesh. Getting these gizmos to closely fit the model will avoid visual clipping issues in-game.\n"
                                                           + "\n"
                                                           + "The <b>Modify Soft Bits</b> handles can be used to adjust the preview soft-body system for breasts.  This system will eventually include the abdomen, groin, upper/lower back and butt.\n", MarrowGUIStyles.DefaultRichText);
                            }
                        }

                        avatarCrateText = EditorGUILayout.Foldout(avatarCrateText, "Creating an Avatar Crate for use in a Mod");
                        if (avatarCrateText)
                        {
                            EditorGUILayout.Space(padding);
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Select the Pallet in the Asset Warehouse.  If the AW windiow is not visible, select the Stress Level Zero -> Void Tools -> Asset Warehouse menu option.\n"
                                                           + "\n"
                                                           + "Click <i>Add Crate</i> in the Pallet's Inspector.\n"
                                                           + "\n"
                                                           + "Next, select the <i>Avatar Crate type</i> from the dropdown menu, choose the Avatar prefab as the <i>Asset Reference</i> and provide a <i>Crate Title</i>.\n"
                                                           + "\n"
                                                           + "Congratulations!  Your custom Avatar should now be ready to Pack into a mod.", MarrowGUIStyles.DefaultRichText);
                            }
                        }

                    }

                    EditorGUILayout.Space(padding);

                    if (GUILayout.Button(new GUIContent("MarrowSDK Documentation - Avatars", "Open the MarrowSDK Documentation in the default web browser."), MarrowGUIStyles.DefaultButton))
                    {
                        Application.OpenURL("https://github.com/StressLevelZero/MarrowSDK/wiki/Avatars");
                    }



                }

                EditorGUILayout.Space(padding);

                if (avatar.gameObject.scene.IsValid())
                {

                    RequiredAvatarBonesExist();
                }


                RequiredWristBoneFieldsFilled();

                RequiredBodyMeshesFilled();

                EditorGUILayout.PropertyField(animatorProp, new GUIContent("Avatar Animator", "Required Animator for the Avatar"));

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(new GUIContent("Edit Body", "Show/Hide the Body Editor Handles"));
                    _hardTorsoHandles = GUILayout.Toggle(_hardTorsoHandles, modifyBodyIcon, MarrowGUIStyles.SkinnyIconButton, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                    GUILayout.Label(new GUIContent("Edit Soft Body", "Show/Hide the Soft Body Editor Handles"));
                    _softTorsoHandles = GUILayout.Toggle(_softTorsoHandles, modifySoftBodyIcon, MarrowGUIStyles.SkinnyIconButton);
                    GUILayout.FlexibleSpace();
                }

                if (_hardTorsoHandles != _lastHardTorsoHandles)
                {
                    SceneView.RepaintAll();
                    _lastHardTorsoHandles = _hardTorsoHandles;
                    if (_hardTorsoHandles)
                    {
                        if (activeEditor != null)
                        {
                            activeEditor._hardTorsoHandles = false;
                        }

                        activeEditor = this;
                    }
                    else
                    {
                        activeEditor = null;
                    }
                }

                if (_softTorsoHandles != _lastSoftTorsoHandles)
                {
                    SceneView.RepaintAll();
                    _lastSoftTorsoHandles = _softTorsoHandles;
                }
























                EditorGUILayout.PropertyField(eyeCenterOverrideProp, new GUIContent("Eye Center Override", "Overrides the eye bone for the position of the eyes, centered"));
                EditorGUILayout.PropertyField(eyeOffsetProp, new GUIContent("Eye Offset", "Z Offset for the eyes, use this to push the face mesh out of the way of the players view"));


                meshesFoldout = EditorGUILayout.Foldout(meshesFoldout, "Meshes", MarrowGUIStyles.BoldFoldout);
                if (meshesFoldout)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(bodyMeshesProp);
                        EditorGUILayout.PropertyField(headMeshesProp);
                        EditorGUILayout.PropertyField(hairMeshesProp);
                    }
                }

                EditorGUILayout.Space();

                audioFoldout = EditorGUILayout.Foldout(audioFoldout, "Sound Effects", MarrowGUIStyles.BoldFoldout);
                if (audioFoldout)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {

                        SoundEffectField(footstepWalkProp, "Footstep Walk", "Light walking Sound, falls back to a default step sound if empty");
                        SoundEffectField(footstepJogProp, "Footstep Jog", "Fast Sprinting Sound, falls back to a default step sound if empty");
                        SoundEffectField(highFallOnFeetProp, "High Fall On Feet", "Landing on your feet from a high distance");

                        SoundEffectField(bigEffortProp, "Jump Effort", "Jumping Up Sound");
                        SoundEffectField(smallPainProp, "Small Pain", "Pain Sound from Small Damage");
                        SoundEffectField(bigPainProp, "Big Pain", "Pain Sound from Large Damage");
                        SoundEffectField(dyingProp, "Dying", "Sound played when dying");
                        SoundEffectField(deadProp, "Dead", "Sound played when fully dead");
                        SoundEffectField(recoveryProp, "Recovery", "Sound played when recovering from dying");
                    }
                }

                EditorGUILayout.Space();

                advancedFoldout = EditorGUILayout.Foldout(advancedFoldout, "Advanced Settings", MarrowGUIStyles.BoldFoldout);
                if (advancedFoldout)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        optionalBonesFoldout = EditorGUILayout.Foldout(optionalBonesFoldout, "Marrow Bones", MarrowGUIStyles.BoldFoldout);
                        if (optionalBonesFoldout)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.PropertyField(wristLeftProp, new GUIContent("Wrist Left", "Required Wrist Bone, use hand if no wrist bone"));
                                EditorGUILayout.PropertyField(wristRightProp, new GUIContent("Wrist Right", "Required Wrist Bone, use hand if no wrist bone"));
                                EditorGUILayout.PropertyField(neck2Prop, new GUIContent("Neck 2", "Optional Neck 2 Bone"));
                                EditorGUILayout.PropertyField(scapulaLeftProp, new GUIContent("Scapula Left", "Optional Chest Scapula Bone"));
                                EditorGUILayout.PropertyField(scapulaRightProp, new GUIContent("Scapula Right", "Optional Chest Scapula Bone"));
                                EditorGUILayout.PropertyField(carpalLeftProp, new GUIContent("Carpal Left", "Optional Wrist Carpal Bone"));
                                EditorGUILayout.PropertyField(carpalRightProp, new GUIContent("Carpal Right", "Optional Wrist Carpal Bone"));
                            }
                        }

                        twistFoldout = EditorGUILayout.Foldout(twistFoldout, "Twist Bones", MarrowGUIStyles.BoldFoldout);
                        if (twistFoldout)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.PropertyField(twistUpperArmLeftProp, new GUIContent("Upperarm Left Twist", "Optional Upperarm Left Twist Bone"));
                                EditorGUILayout.PropertyField(twistUpperArmRightProp, new GUIContent("Upperarm Right Twist", "Optional Upperarm Right Twist Bone"));
                                EditorGUILayout.PropertyField(twistForearmLeftProp, new GUIContent("Forearm Left Twist", "Optional Forearm Left Twist Bone"));
                                EditorGUILayout.PropertyField(twistForearmRightProp, new GUIContent("Forearm Right Twist", "Optional Forearm Left Twist Bone"));
                                EditorGUILayout.PropertyField(twistUpperThighLeftProp, new GUIContent("Upper Thigh Left Twist", "Optional Upper Thigh Left Twist Bone"));
                                EditorGUILayout.PropertyField(twistUpperThighRightProp, new GUIContent("Upper Thigh Right Twist", "Optional Upper Thigh Left Twist Bone"));
                            }
                        }

                        headFoldout = EditorGUILayout.Foldout(headFoldout, "Head", MarrowGUIStyles.BoldFoldout);
                        if (headFoldout)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Head", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(headTopProp);
                                    EditorGUILayout.PropertyField(headEllipseXProp);
                                    EditorGUILayout.PropertyField(headEllipseZProp);
                                }

                                EditorGUILayout.LabelField("Jaw", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(jawEllipseXProp);
                                    EditorGUILayout.PropertyField(jawEllipseZProp);
                                    EditorGUILayout.PropertyField(chinYProp);
                                }

                                EditorGUILayout.LabelField("Neck", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(neckEllipseXProp);
                                    EditorGUILayout.PropertyField(neckEllipseZProp);
                                }
                            }
                        }

                        torsoFoldout = EditorGUILayout.Foldout(torsoFoldout, "Torso", MarrowGUIStyles.BoldFoldout);
                        if (torsoFoldout)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Hips", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(hipsEllipseXProp);
                                    EditorGUILayout.PropertyField(hipsEllipseZProp);
                                }

                                EditorGUILayout.LabelField("High Hips", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(highHipY);
                                    EditorGUILayout.PropertyField(highHipsEllipseXProp);
                                    EditorGUILayout.PropertyField(highHipsEllipseZProp);
                                    EditorGUILayout.PropertyField(highHipsEllipseNegZProp);
                                }

                                EditorGUILayout.LabelField("Waist", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(waistYProp);
                                    EditorGUILayout.PropertyField(waistEllipseXProp);
                                    EditorGUILayout.PropertyField(waistEllipseZProp);
                                    EditorGUILayout.PropertyField(waistEllipseNegZProp);
                                }

                                EditorGUILayout.LabelField("Underbust", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(underbustYProp);
                                    EditorGUILayout.PropertyField(chestEllipseXProp);
                                    EditorGUILayout.PropertyField(chestEllipseZProp);
                                    EditorGUILayout.PropertyField(chestEllipseNegZProp);
                                }
                            }
                        }

                        limbFoldout = EditorGUILayout.Foldout(limbFoldout, "Limbs", MarrowGUIStyles.BoldFoldout);
                        if (limbFoldout)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Arms", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(upperarmEllipseProp);
                                    EditorGUILayout.PropertyField(elbowEllipseProp);
                                    EditorGUILayout.PropertyField(forearmEllipseProp);
                                    EditorGUILayout.PropertyField(wristEllipseProp);
                                }

                                EditorGUILayout.LabelField("Legs", EditorStyles.boldLabel);
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.PropertyField(thighUpperEllipseProp);
                                    EditorGUILayout.PropertyField(kneeEllipseProp);
                                    EditorGUILayout.PropertyField(calfEllipseProp);
                                    EditorGUILayout.PropertyField(ankleEllipseProp);
                                }
                            }
                        }

                        softBulgeFoldout = EditorGUILayout.Foldout(softBulgeFoldout, "Soft Bulges", MarrowGUIStyles.BoldFoldout);
                        if (softBulgeFoldout)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.PropertyField(bulgeBreastProp, new GUIContent("Breast Soft Body"));
                                EditorGUILayout.PropertyField(bulgeButtProp, new GUIContent("Butt Soft Body"));
                            }
                        }
                    }
                }

            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SoundEffectField(SerializedProperty prop, string name, string tooltip)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(prop, new GUIContent(name, tooltip));




            }
        }

        private void RequiredAvatarBonesExist()
        {
            Avatar avatar = (Avatar)target;

            if (avatar.animator == null)
            {
                EditorGUILayout.HelpBox("The Avatar must have an Animator.", MessageType.Error);
                avatar.requiredBonesOrRefsMissing = true;

            }
            else
            {
                avatar.requiredBonesOrRefsMissing = false;

                var animator = avatar.animator;

                if (animator.GetBoneTransform(HumanBodyBones.LeftEye) == null && avatar.eyeCenterOverride == null)
                {
                    EditorGUILayout.HelpBox("Avatars require Eye Bones or an Empty GameObject at the root of the Prefab to act as an Eye Center Override, which should be positioned between the eyes and near the surface of the head mesh.", MessageType.Warning);
                }

                List<HumanBodyBones> optionalBones = new List<HumanBodyBones>();

                optionalBones.Add(HumanBodyBones.LeftEye);
                optionalBones.Add(HumanBodyBones.RightEye);
                optionalBones.Add(HumanBodyBones.Jaw);

                optionalBones.Add(HumanBodyBones.UpperChest);

                optionalBones.Add(HumanBodyBones.LeftMiddleProximal);
                optionalBones.Add(HumanBodyBones.LeftMiddleIntermediate);
                optionalBones.Add(HumanBodyBones.LeftMiddleDistal);

                optionalBones.Add(HumanBodyBones.RightMiddleProximal);
                optionalBones.Add(HumanBodyBones.RightMiddleIntermediate);
                optionalBones.Add(HumanBodyBones.RightMiddleDistal);

                optionalBones.Add(HumanBodyBones.LeftLittleProximal);
                optionalBones.Add(HumanBodyBones.LeftLittleIntermediate);
                optionalBones.Add(HumanBodyBones.LeftLittleDistal);

                optionalBones.Add(HumanBodyBones.RightLittleProximal);
                optionalBones.Add(HumanBodyBones.RightLittleIntermediate);
                optionalBones.Add(HumanBodyBones.RightLittleDistal);

                List<String> missingRequiredBoneNames = new List<String>();

                foreach (HumanBodyBones reqBone in Enum.GetValues(typeof(HumanBodyBones)))
                {

                    if (!optionalBones.Contains(reqBone))
                    {
                        if (reqBone != HumanBodyBones.LastBone && animator.GetBoneTransform(reqBone) == null)
                        {
                            missingRequiredBoneNames.Add(reqBone.ToString());

                        }

                    }
                }

                if (missingRequiredBoneNames.Count > 0)
                {
                    String boneErrorString = "\nAvatars require the following missing bones: \n\n";

                    foreach (String boneName in missingRequiredBoneNames)
                    {
                        boneErrorString += boneName + "\n";
                    }

                    EditorGUILayout.HelpBox(boneErrorString, MessageType.Error);
                    avatar.requiredBonesOrRefsMissing = true;
                }
                else
                {
                    avatar.requiredBonesOrRefsMissing = false;
                }
            }
        }

        private void RequiredWristBoneFieldsFilled()
        {
            if (avatar.wristRt == null || avatar.wristLf == null)
            {
                EditorGUILayout.HelpBox("Complete the Wrist Lf and Wrist Rt fields below.  Other Avatar Script Bone fields are optional but recommended.  If the Avatar Rig does not have specific Wrist bones, the Hand bones should suffice. ", MessageType.Error);
                avatar.requiredBonesOrRefsMissing = true;
            }
            else
            {
                avatar.requiredBonesOrRefsMissing = false;
            }
        }

        private void RequiredBodyMeshesFilled()
        {
            if (avatar.bodyMeshes == null || avatar.bodyMeshes.Length == 0)
            {
                EditorGUILayout.HelpBox("At a minimum, add at least one element to the Body Meshes list and select the model's Skinned Mesh Renderer. If the model has seperate Head or Hair meshes, add those to the appropriate list.", MessageType.Warning);
                avatar.requiredBonesOrRefsMissing = true;
            }
            else if (avatar.bodyMeshes.Length > 0 && avatar.bodyMeshes[0] == null)
            {
                EditorGUILayout.HelpBox("At a minimum, add at least one element to the Body Meshes list and select the model's Skinned Mesh Renderer. If the model has seperate Head or Hair meshes, add those to the appropriate list.", MessageType.Warning);
                avatar.requiredBonesOrRefsMissing = true;
            }
            else
            {
                avatar.requiredBonesOrRefsMissing = false;
            }
        }

        private void OnSceneGUI()
        {
            Avatar avatar = (Avatar)target;

            if (avatar.animator == null)
            {
                return;
            }

            if (avatar.animator.GetBoneTransform(HumanBodyBones.LeftEye) == null && avatar.eyeCenterOverride == null)
            {
                return;
            }

            if (avatar.requiredBonesOrRefsMissing)
            {
                return;
            }

            if (avatar.wristLf == null || avatar.wristRt == null)
            {
                return;
            }

            var animator = avatar.animator;

            if (boldCenteredStyle == null)
            {
                boldCenteredStyle = new GUIStyle(EditorStyles.boldLabel);
                boldCenteredStyle.alignment = TextAnchor.MiddleCenter;
            }

            Quaternion worldToAnimator = Quaternion.Inverse(animator.transform.rotation);
            Vector3 eyeInRoot = ((avatar.eyeCenterOverride != null) ? avatar.eyeCenterOverride.position : animator.GetBoneTransform(HumanBodyBones.LeftEye).position) - animator.transform.position;
            float eyeCenterHeight = (worldToAnimator * eyeInRoot).y;


            Handles.matrix = Matrix4x4.TRS(avatar.transform.position, avatar.transform.rotation, Vector3.one);

            Vector3 neckLocal = animator.transform.InverseTransformDirection(animator.GetBoneTransform(HumanBodyBones.Neck).position - animator.transform.position);
            Vector3 headLocal = animator.transform.InverseTransformDirection(animator.GetBoneTransform(HumanBodyBones.Head).position - animator.transform.position);
            Vector3 shoulderLfLocal = animator.transform.InverseTransformDirection(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position - animator.transform.position);
            Vector3 shoulderRtLocal = animator.transform.InverseTransformDirection(animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position - animator.transform.position);
            Vector3 sternumLocal = Vector3.LerpUnclamped(shoulderLfLocal, shoulderRtLocal, .5f);
            float sternumEllipseX = (shoulderRtLocal.x - sternumLocal.x) / eyeCenterHeight;

            Vector3 hipLfLocal = animator.transform.InverseTransformDirection(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position - animator.transform.position);
            Vector3 hipRtLocal = animator.transform.InverseTransformDirection(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position - animator.transform.position);
            Vector3 hipsLocal = Vector3.LerpUnclamped(hipLfLocal, hipRtLocal, .5f);
            float hipEllipseX = (hipRtLocal.x - hipsLocal.x) / eyeCenterHeight;

            Transform rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            Transform rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            Transform rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            Vector3 rightUpperLegLocal = animator.transform.InverseTransformDirection(rightUpperLeg.position - animator.transform.position);
            Vector3 femurRt = rightLowerLeg.position - rightUpperLeg.position;
            Avatar.GuessLimbAxis(worldToAnimator * femurRt, worldToAnimator * rightUpperLeg.rotation, out Vector3 fwdLegRt, out Vector3 upLegRt);
            float femurRtLength = femurRt.magnitude / eyeCenterHeight;
            float upperHighThighY = Mathf.LerpUnclamped(avatar.CrotchBottom / femurRtLength, 1f, .1f);
            Vector3 rightThighLocal = Vector3.Lerp(rightUpperLeg.position, rightLowerLeg.position, upperHighThighY);
            Vector3 rightCalfLfLocal = Vector3.Lerp(rightLowerLeg.position, rightFoot.position, 0.3f);

            Vector3 femurRtFwd = rightUpperLeg.rotation * fwdLegRt;
            Vector3 femurRtUp = rightUpperLeg.rotation * upLegRt;
            Vector3 femurRtRt = Vector3.Cross(femurRtUp, femurRtFwd);
            Vector3 rightLegRt = Vector3.Cross(fwdLegRt, upLegRt);

            Transform rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Transform rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            Vector3 humerousRt = rightLowerArm.position - rightUpperArm.position;
            float humerousRtLength = humerousRt.magnitude / eyeCenterHeight;
            float upperArmHighY = Mathf.LerpUnclamped(.03f / humerousRtLength, 1f, .1f);
            Avatar.GuessLimbAxis(worldToAnimator * humerousRt, worldToAnimator * rightUpperArm.rotation, out Vector3 fwdArmRt, out Vector3 upArmRt);
            Vector3 rightUpperArmLocal = Vector3.Lerp(rightUpperArm.position, rightLowerArm.position, upperArmHighY);
            Vector3 rightElbowLocal = Vector3.Lerp(rightLowerArm.position, avatar.wristRt.position, 0.18f);

            Vector3 waistLocal = Vector3.Lerp(sternumLocal, hipsLocal, avatar.WaistY);
            float waistZOffset = (avatar.WaistEllipseZ - avatar.WaistEllipseNegZ) * .4f;
            waistLocal = new Vector3(0f, 0f, waistZOffset * eyeCenterHeight) + waistLocal;
            Vector3 underbustLocal = Vector3.Lerp(sternumLocal, hipsLocal, avatar.UnderbustY);
            float underbustZOffset = (avatar.ChestEllipseZ - avatar.ChestEllipseNegZ) * .2f;
            underbustLocal = new Vector3(0f, 0f, underbustZOffset * eyeCenterHeight) + underbustLocal;
            Vector3 highHipsLocal = Vector3.Lerp(sternumLocal, hipsLocal, avatar.HighHipsY);
            float highHipsZOffset = (avatar.HighHipsEllipseZ - avatar.HighHipsEllipseNegZ) * .4f;
            highHipsLocal = new Vector3(0f, 0f, highHipsZOffset * eyeCenterHeight) + highHipsLocal;

            Vector3 foreheadLocal = (avatar.HeadTop * eyeCenterHeight * .4f + eyeCenterHeight - headLocal.y) * Vector3.up + headLocal;
            Vector3 jawLocal = (avatar.ChinY * eyeCenterHeight * -.6f + eyeCenterHeight - headLocal.y) * Vector3.up + headLocal;
            Vector3 chinLocal = (avatar.ChinY * eyeCenterHeight * -1f + eyeCenterHeight - headLocal.y) * Vector3.up + headLocal;






























            if (_hardTorsoHandles)
            {
                float handleSize = 0.01f * eyeCenterHeight;

                if (activeEditor == this && activeHandleLabel > 0)
                {
                    var oldMatrix = Handles.matrix;
                    Handles.matrix = activeHandleLabelMatrix;
                    Handles.color = Color.white;

                    var inverseRot = Quaternion.Inverse(Quaternion.LookRotation(Handles.matrix.GetColumn(2), Handles.matrix.GetColumn(1)));

                    Handles.Label(activeHandleLabelPosition + inverseRot * SceneView.lastActiveSceneView.rotation * (Vector3.up * handleSize * 1.5f), activeHandleLabelName, boldCenteredStyle);

                    Handles.matrix = oldMatrix;
                }

                Handles.color = Color.cyan;
                EditorGUI.BeginChangeCheck();

                Vector3 headX = DrawAvatarHandle("Head", foreheadLocal + new Vector3(avatar.ForeheadEllipseX * eyeCenterHeight, 0f, 0f), Vector3.right, handleSize);
                float newHeadX = Mathf.Clamp((headX.x - headLocal.x) / eyeCenterHeight, .02f, .2f);

                Vector3 jawX = DrawAvatarHandle("Jaw", jawLocal + new Vector3(avatar.JawEllipseX * eyeCenterHeight, 0f, 0f), Vector3.right, handleSize);
                float newJawX = Mathf.Clamp((jawX.x - headLocal.x) / eyeCenterHeight, .02f, .2f);

                Vector3 neckX = DrawAvatarHandle("Neck", neckLocal + new Vector3(avatar.NeckEllipseX * eyeCenterHeight, 0f, 0f), Vector3.right, handleSize);
                float newNeckX = Mathf.Clamp((neckX.x - neckLocal.x) / eyeCenterHeight, .018f, .08f);

                Vector3 chestX = DrawAvatarHandle("Chest", underbustLocal + new Vector3(avatar.ChestEllipseX * eyeCenterHeight, 0f, 0f), Vector3.right, handleSize);
                float newChestX = Mathf.Clamp((chestX.x - underbustLocal.x) / eyeCenterHeight, .04f, .24f);

                Vector3 waistX = DrawAvatarHandle("Waist", waistLocal + new Vector3(avatar.WaistEllipseX * eyeCenterHeight, 0f, 0f), Vector3.right, handleSize);
                float newWaistX = Mathf.Clamp((waistX.x - waistLocal.x) / eyeCenterHeight, .04f, .24f);

                Vector3 highhipX = DrawAvatarHandle("High Hip", highHipsLocal + new Vector3(avatar.HighHipsEllipseX * eyeCenterHeight, 0f, 0f), Vector3.right, handleSize);
                float newHighHipsX = Mathf.Clamp((highhipX.x - highHipsLocal.x) / eyeCenterHeight, .04f, .24f);

                Vector3 hipX = DrawAvatarHandle("Hip", hipsLocal + new Vector3(avatar.HipsEllipseX * eyeCenterHeight, 0f, 0f), Vector3.right, handleSize);
                float newHipsX = Mathf.Clamp((hipX.x - hipsLocal.x) / eyeCenterHeight, hipEllipseX + .01f, .24f);


                Handles.color = Color.white;
                Vector3 headZ = DrawAvatarHandle("Head", foreheadLocal + new Vector3(0f, 0f, avatar.ForeheadEllipseZ * eyeCenterHeight), Vector3.forward, handleSize);
                float newHeadZ = Mathf.Clamp((headZ.z - headLocal.z) / eyeCenterHeight, .02f, .2f);

                Vector3 jawZ = DrawAvatarHandle("Jaw", jawLocal + new Vector3(0f, 0f, avatar.JawEllipseZ * eyeCenterHeight), Vector3.forward, handleSize);
                float newJawZ = Mathf.Clamp((jawZ.z - headLocal.z) / eyeCenterHeight, .02f, .2f);

                Vector3 neckZ = DrawAvatarHandle("Neck", neckLocal + new Vector3(0f, 0f, avatar.NeckEllipseZ * eyeCenterHeight), Vector3.forward, handleSize);
                float newNeckZ = Mathf.Clamp((neckZ.z - neckLocal.z) / eyeCenterHeight, .018f, .08f);

                Vector3 sternumZ = DrawAvatarHandle("Sternum", sternumLocal + new Vector3(0f, 0f, avatar.SternumEllipseZ * eyeCenterHeight), Vector3.forward, handleSize);
                float newSternumZ = Mathf.Clamp((sternumZ.z - sternumLocal.z) / eyeCenterHeight, .01f, .24f);

                Vector3 chestZ = DrawAvatarHandle("Chest", underbustLocal + new Vector3(0f, 0f, (avatar.ChestEllipseZ - underbustZOffset) * eyeCenterHeight), Vector3.forward, handleSize);
                float newChestZ = Mathf.Clamp((chestZ.z - underbustLocal.z + underbustZOffset * eyeCenterHeight) / eyeCenterHeight, .01f, .24f);

                Vector3 waistZ = DrawAvatarHandle("Waist", waistLocal + new Vector3(0f, 0f, (avatar.WaistEllipseZ - waistZOffset) * eyeCenterHeight), Vector3.forward, handleSize);
                float newWaistZ = Mathf.Clamp((waistZ.z - waistLocal.z + waistZOffset * eyeCenterHeight) / eyeCenterHeight, .01f, .24f);

                Vector3 highhipZ = DrawAvatarHandle("High Hip", highHipsLocal + new Vector3(0f, 0f, (avatar.HighHipsEllipseZ - highHipsZOffset) * eyeCenterHeight), Vector3.forward, handleSize);
                float newHighHipsZ = Mathf.Clamp((highhipZ.z - highHipsLocal.z + highHipsZOffset * eyeCenterHeight) / eyeCenterHeight, .01f, .24f);

                Vector3 hipZ = DrawAvatarHandle("Hip", hipsLocal + new Vector3(0f, 0f, avatar.HipsEllipseZ * eyeCenterHeight), Vector3.forward, handleSize);
                float newHipsZ = Mathf.Clamp((hipZ.z - hipsLocal.z) / eyeCenterHeight, .01f, .24f);


                Vector3 headNegZ = DrawAvatarHandle("Head", foreheadLocal - new Vector3(0f, 0f, avatar.ForeheadEllipseNegZ * eyeCenterHeight), Vector3.back, handleSize);
                float newHeadNegZ = Mathf.Clamp(-(headNegZ.z - headLocal.z) / eyeCenterHeight, .02f, .2f);

                Vector3 jawNegZ = DrawAvatarHandle("Jaw", jawLocal - new Vector3(0f, 0f, avatar.JawEllipseNegZ * eyeCenterHeight), Vector3.back, handleSize);
                float newJawNegZ = Mathf.Clamp(-(jawNegZ.z - headLocal.z) / eyeCenterHeight, .02f, .2f);

                Vector3 neckNegZ = DrawAvatarHandle("Neck", neckLocal - new Vector3(0f, 0f, avatar.NeckEllipseNegZ * eyeCenterHeight), Vector3.back, handleSize);
                float newNeckNegZ = Mathf.Clamp(-(neckNegZ.z - neckLocal.z) / eyeCenterHeight, .018f, .08f);

                Vector3 sternumNegZ = DrawAvatarHandle("Sternum", sternumLocal - new Vector3(0f, 0f, avatar.SternumEllipseNegZ * eyeCenterHeight), Vector3.back, handleSize);
                float newSternumNegZ = Mathf.Clamp(-(sternumNegZ.z - sternumLocal.z) / eyeCenterHeight, .01f, .24f);

                Vector3 chestNegZ = DrawAvatarHandle("Chest", underbustLocal - new Vector3(0f, 0f, (avatar.ChestEllipseNegZ + underbustZOffset) * eyeCenterHeight), Vector3.back, handleSize);
                float newChestNegZ = Mathf.Clamp(-(chestNegZ.z - underbustLocal.z + underbustZOffset * eyeCenterHeight) / eyeCenterHeight, .01f, .24f);

                Vector3 waistNegZ = DrawAvatarHandle("Waist", waistLocal - new Vector3(0f, 0f, (avatar.WaistEllipseNegZ + waistZOffset) * eyeCenterHeight), Vector3.back, handleSize);
                float newWaistNegZ = Mathf.Clamp(-(waistNegZ.z - waistLocal.z + waistZOffset * eyeCenterHeight) / eyeCenterHeight, .01f, .24f);

                Vector3 highhipNegZ = DrawAvatarHandle("High Hip", highHipsLocal - new Vector3(0f, 0f, (avatar.HighHipsEllipseNegZ + highHipsZOffset) * eyeCenterHeight), Vector3.back, handleSize);
                float newHighHipsNegZ = Mathf.Clamp(-(highhipNegZ.z - highHipsLocal.z + highHipsZOffset * eyeCenterHeight) / eyeCenterHeight, .01f, .24f);

                Vector3 hipNegZ = DrawAvatarHandle("Hip", hipsLocal - new Vector3(0f, 0f, avatar.HipsEllipseNegZ * eyeCenterHeight), Vector3.back, handleSize);
                float newHipsNegZ = Mathf.Clamp(-(hipNegZ.z - hipsLocal.z) / eyeCenterHeight, .01f, .24f);



                bool bustChanged = false;
                float newBustY;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var bustY = Handles.Slider(underbustLocal, Vector3.down, handleSize, Handles.CubeHandleCap, snapValue);
                    Handles.Label(underbustLocal - new Vector3(handleSize * 2f, 0f, 0f), "Underbust");

                    Vector3 ab = hipsLocal - sternumLocal;
                    Vector3 av = bustY - sternumLocal;
                    newBustY = Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
                    newBustY = Mathf.Clamp(newBustY, Avatar.Constants.UnderbustYMin, Avatar.Constants.UnderbustYMax);
                    bustChanged = check.changed;
                }

                bool waistChanged = false;
                float newWaistY;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var waistY = Handles.Slider(waistLocal, Vector3.down, handleSize, Handles.CubeHandleCap, snapValue);
                    Handles.Label(waistLocal - new Vector3(handleSize * 1.5f, 0f, 0), "Waist");

                    Vector3 ab = hipsLocal - sternumLocal;
                    Vector3 av = waistY - sternumLocal;
                    newWaistY = Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
                    newWaistY = Mathf.Clamp(newWaistY, Avatar.Constants.WaistYMin, Avatar.Constants.WaistYMax);

                    waistChanged = check.changed;
                }

                bool hipChanged = false;
                float newHipY;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var hipY = Handles.Slider(highHipsLocal, Vector3.down, handleSize, Handles.CubeHandleCap, snapValue);
                    Handles.Label(highHipsLocal - new Vector3(handleSize * 1.5f, 0f, 0), "High Hip");

                    Vector3 ab = hipsLocal - sternumLocal;
                    Vector3 av = hipY - sternumLocal;
                    newHipY = Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
                    newHipY = Mathf.Clamp(newHipY, Avatar.Constants.HighHipYMin, Avatar.Constants.HighHipYMax);
                    hipChanged = check.changed;
                }

                bool crotchChanged = false;
                float newCrotchY;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var crotchY = Handles.Slider(hipsLocal - new Vector3(0f, avatar.CrotchBottom * eyeCenterHeight, 0f), Vector3.down, handleSize, Handles.CubeHandleCap, snapValue);
                    Handles.Label(hipsLocal - new Vector3(0f, avatar.CrotchBottom * eyeCenterHeight, 0f) - new Vector3(handleSize * 1.5f, 0f, 0), "Crotch Bottom");
                    newCrotchY = (hipsLocal.y - crotchY.y) / eyeCenterHeight;
                    newCrotchY = Mathf.Clamp(newCrotchY, Avatar.Constants.CrotchBottomMin, Avatar.Constants.CrotchBottomMax);
                    crotchChanged = check.changed;
                }

                var chinY = Handles.Slider(chinLocal + Vector3.forward * avatar.JawEllipseZ * eyeCenterHeight, Vector3.forward, handleSize, Handles.SphereHandleCap, snapValue);
                float newChinY = (chinY.y - chinLocal.y);


                float newHeadTop = avatar.HeadTop;
                Handles.Label(foreheadLocal + Vector3.up * avatar.HeadTop * 0.6f * eyeCenterHeight + Vector3.up * handleSize * 2f, "Head Top", boldCenteredStyle);
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var headTop = DrawAvatarHandle("Head Top", foreheadLocal + Vector3.up * avatar.HeadTop * 0.6f * eyeCenterHeight, Vector3.up, Mathf.Max(avatar.ForeheadEllipseX, avatar.ForeheadEllipseZ) / 2f, Handles.RectangleHandleCap);
                    if (check.changed)
                        newHeadTop = (headTop.y - foreheadLocal.y) / (0.6f * eyeCenterHeight);
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var headTop = DrawAvatarHandle("Head Top", foreheadLocal + Vector3.up * avatar.HeadTop * 0.6f * eyeCenterHeight, Vector3.up, handleSize, Handles.CubeHandleCap);
                    if (check.changed)
                        newHeadTop = (headTop.y - foreheadLocal.y) / (0.6f * eyeCenterHeight);
                }


                Color altColor = new Color(1f, 0.5f, 1f);
                var thighEllipse = DrawLimbHandle("Thigh", rightThighLocal, rightUpperLeg.rotation, avatar.thighUpperEllipse, eyeCenterHeight, fwdLegRt, upLegRt, snapValue, altColor);
                var kneeEllipse = DrawLimbHandle("Knee", rightLowerLeg.position, rightLowerLeg.rotation, avatar.kneeEllipse, eyeCenterHeight, fwdLegRt, upLegRt, snapValue, altColor);
                var calfEllipse = DrawLimbHandle("Calf", rightCalfLfLocal, rightLowerLeg.rotation, avatar.calfEllipse, eyeCenterHeight, fwdLegRt, upLegRt, snapValue, altColor);
                var ankleEllipse = DrawLimbHandle("Ankle", rightFoot.position, rightFoot.rotation, avatar.ankleEllipse, eyeCenterHeight, fwdLegRt, upLegRt, snapValue, altColor);



                altColor = new Color(0.6f, 0.2f, 0.6f);
                var upperarmEllipse = DrawLimbHandle("Upperarm", rightUpperArmLocal, rightUpperArm.rotation, avatar.upperarmEllipse, eyeCenterHeight, fwdArmRt, upArmRt, snapValue, altColor);
                var elbowEllipse = DrawLimbHandle("Elbow", rightLowerArm.position, rightLowerArm.rotation, avatar.elbowEllipse, eyeCenterHeight, fwdArmRt, upArmRt, snapValue, altColor);
                var forearmEllipse = DrawLimbHandle("Forearm", rightElbowLocal, rightLowerArm.rotation, avatar.forearmEllipse, eyeCenterHeight, fwdArmRt, upArmRt, snapValue, altColor);
                var wristEllipse = DrawLimbHandle("Wrist", avatar.wristRt.position, rightLowerArm.rotation, avatar.wristEllipse, eyeCenterHeight, fwdArmRt, upArmRt, snapValue, altColor);

                if (EditorGUI.EndChangeCheck())
                {
                    activeHandleLabel += 10;
                    Undo.RecordObject(target, "Modify Avatar Body");
                    avatar.ForeheadEllipseX = newHeadX;
                    avatar.JawEllipseX = newJawX;
                    avatar.NeckEllipseX = newNeckX;
                    avatar.ChestEllipseX = newChestX;
                    avatar.WaistEllipseX = newWaistX;
                    avatar.HighHipsEllipseX = newHighHipsX;
                    avatar.HipsEllipseX = newHipsX;

                    avatar.ForeheadEllipseZ = newHeadZ;
                    avatar.JawEllipseZ = newJawZ;
                    avatar.NeckEllipseZ = newNeckZ;
                    avatar.SternumEllipseZ = newSternumZ;
                    avatar.ChestEllipseZ = newChestZ;
                    avatar.WaistEllipseZ = newWaistZ;
                    avatar.HighHipsEllipseZ = newHighHipsZ;
                    avatar.HipsEllipseZ = newHipsZ;

                    avatar.ForeheadEllipseNegZ = newHeadNegZ;
                    avatar.JawEllipseNegZ = newJawNegZ;
                    avatar.NeckEllipseNegZ = newNeckNegZ;
                    avatar.SternumEllipseNegZ = newSternumNegZ;
                    avatar.ChestEllipseNegZ = newChestNegZ;
                    avatar.WaistEllipseNegZ = newWaistNegZ;
                    avatar.HighHipsEllipseNegZ = newHighHipsNegZ;
                    avatar.HipsEllipseNegZ = newHipsNegZ;

                    if (bustChanged)
                        avatar.UnderbustY = newBustY;
                    if (waistChanged)
                        avatar.WaistY = newWaistY;
                    if (hipChanged)
                        avatar.HighHipsY = newHipY;
                    if (crotchChanged)
                        avatar.CrotchBottom = newCrotchY;

                    avatar.HeadTop = newHeadTop;

                    avatar.thighUpperEllipse = thighEllipse;
                    avatar.kneeEllipse = kneeEllipse;
                    avatar.calfEllipse = calfEllipse;
                    avatar.ankleEllipse = ankleEllipse;

                    avatar.upperarmEllipse = upperarmEllipse;
                    avatar.elbowEllipse = elbowEllipse;
                    avatar.forearmEllipse = forearmEllipse;
                    avatar.wristEllipse = wristEllipse;
                }
                else
                {
                    activeHandleLabel = (int)Mathf.Clamp(activeHandleLabel - 1, 0f, 50f);
                    if (activeHandleLabel <= 0)
                    {
                        activeHandleLabelName = "";
                    }
                }
            }
            if (_softTorsoHandles)
            {
                float t1HeightPerc = neckLocal.y / eyeCenterHeight;
                Vector3 sternumOffset = worldToAnimator * (Vector3.LerpUnclamped(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position, animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position, .5f) - animator.GetBoneTransform(HumanBodyBones.Neck).position);
                Vector3 sternumOffsetPerc = sternumOffset / eyeCenterHeight;
                Vector3 hipOffset = worldToAnimator * (Vector3.LerpUnclamped(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position, animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position, .5f) - animator.GetBoneTransform(HumanBodyBones.Hips).position);
                Vector3 hipOffsetPercent = hipOffset / eyeCenterHeight;

                var spineZ = avatar.GenerateSpineCurve(eyeCenterHeight, avatar.t7Local, avatar.l1Local, avatar.l3Local, avatar.sacrumLocal, sternumOffsetPerc, hipOffsetPercent, t1HeightPerc);



                var ellipseZ = avatar.GenerateTorsoZCurve(sternumOffsetPerc, t1HeightPerc, spineZ);
                var ellipseNegZ = avatar.GenerateTorsoNegZCurve(sternumOffsetPerc, t1HeightPerc, spineZ);
                var ellipseX = avatar.GenerateTorsoXCurve(sternumEllipseX, hipEllipseX, sternumOffsetPerc, t1HeightPerc);

                EditorGUI.BeginChangeCheck();

                bool breastDirty = BulgeHandles(avatar.bulgeBreast, avatar, 0f, avatar.UnderbustY, sternumLocal, hipsLocal, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, out Avatar.SoftBulge outBreast);
                bool abdomenDirty = BulgeHandles(avatar.bulgeAbdomen, avatar, avatar.UnderbustY, avatar.HighHipsY, sternumLocal, hipsLocal, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, out Avatar.SoftBulge outAbdomen);
                bool groinDirty = BulgeHandles(avatar.bulgeGroin, avatar, avatar.HighHipsY, 1.99f, sternumLocal, hipsLocal, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, out Avatar.SoftBulge outGroin);

                if (EditorGUI.EndChangeCheck())
                {

                    Undo.RecordObject(target, "Change Soft Torso");
                    if (breastDirty) avatar.bulgeBreast.Copy(outBreast);
                    if (abdomenDirty) avatar.bulgeAbdomen.Copy(outAbdomen);
                    if (groinDirty) avatar.bulgeGroin.Copy(outGroin);
                }
            }

        }

        Vector3 DrawAvatarHandle(string name, Vector3 position, Vector3 direction, float handleSize, Handles.CapFunction capFunction = null)
        {
            if (capFunction == null)
                capFunction = Handles.SphereHandleCap;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Vector3 sliderValue = Handles.Slider(position, direction, handleSize, capFunction, snapValue);
                if (check.changed)
                {
                    activeHandleLabelName = name;
                    activeHandleLabelPosition = position;
                    activeHandleLabelMatrix = Handles.matrix;
                }
                return sliderValue;
            }
        }

        private Avatar.SoftEllipse DrawLimbHandle(string name, Vector3 localLimbPosition, Quaternion rigLimbRotation, Avatar.SoftEllipse limbEllipse, float eyeCenterHeight, Vector3 fwdLimbRotation, Vector3 upLimbRotation, float snapValue, Color altColor)
        {
            var oldMatrix = Handles.matrix;
            Avatar.SoftEllipse newEllipse = new Avatar.SoftEllipse();
            newEllipse.XRadius = limbEllipse.XRadius;
            newEllipse.XBias = limbEllipse.XBias;
            newEllipse.ZRadius = limbEllipse.ZRadius;
            newEllipse.ZBias = limbEllipse.ZBias;
            Handles.matrix = Matrix4x4.TRS(localLimbPosition, rigLimbRotation * Quaternion.LookRotation(fwdLimbRotation, upLimbRotation), Vector3.one);

            float limbHandleSize = 0.01f * eyeCenterHeight;

            Handles.color = Color.white;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Vector3 handlePos = new Vector3(0f, (limbEllipse.ZRadius + limbEllipse.ZBias * limbEllipse.ZRadius) * eyeCenterHeight, 0f);
                Vector3 limbZRadius = Handles.Slider(handlePos, Vector3.up, limbHandleSize, Handles.SphereHandleCap, snapValue);
                float newLimbZRadius = (limbZRadius.y) / (eyeCenterHeight + limbEllipse.ZBias * eyeCenterHeight);
                newLimbZRadius = Mathf.Clamp(newLimbZRadius, Avatar.SoftEllipse.Constants.RadiusMin, Avatar.SoftEllipse.Constants.RadiusMax);

                if (check.changed)
                {
                    newEllipse.ZRadius = newLimbZRadius;
                    activeHandleLabelName = $"{name} Radius";
                    activeHandleLabelPosition = handlePos;
                    activeHandleLabelMatrix = Handles.matrix;
                }
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Vector3 handlePos = new Vector3(0f, (limbEllipse.ZRadius + limbEllipse.ZBias * limbEllipse.ZRadius + 0.02f) * eyeCenterHeight, 0f);
                Vector3 limbZBias = Handles.Slider(handlePos, Vector3.up, limbHandleSize, Handles.ConeHandleCap, snapValue);
                float newLimbZBias = (limbZBias.y - limbEllipse.ZRadius * eyeCenterHeight - 0.02f * eyeCenterHeight) / (limbEllipse.ZRadius * eyeCenterHeight);
                newLimbZBias = Mathf.Clamp(newLimbZBias, Avatar.SoftEllipse.Constants.BiasMin, Avatar.SoftEllipse.Constants.BiasMax);

                if (check.changed)
                {
                    newEllipse.ZBias = newLimbZBias;
                    activeHandleLabelName = $"{name} Offset";
                    activeHandleLabelPosition = handlePos;
                    activeHandleLabelMatrix = Handles.matrix;
                }
            }


            Handles.color = altColor;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Vector3 handlePos = new Vector3((limbEllipse.XRadius + limbEllipse.XBias * limbEllipse.XRadius) * eyeCenterHeight, 0f, 0f);
                Vector3 limbXRadius = Handles.Slider(handlePos, Vector3.right, limbHandleSize, Handles.SphereHandleCap, snapValue);
                float newLimbXRadius = (limbXRadius.x) / (eyeCenterHeight + limbEllipse.XBias * eyeCenterHeight);
                newLimbXRadius = Mathf.Clamp(newLimbXRadius, Avatar.SoftEllipse.Constants.RadiusMin, Avatar.SoftEllipse.Constants.RadiusMax);

                if (check.changed)
                {
                    newEllipse.XRadius = newLimbXRadius;
                    activeHandleLabelName = $"{name} Radius";
                    activeHandleLabelPosition = handlePos;
                    activeHandleLabelMatrix = Handles.matrix;
                }
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Vector3 handlePos = new Vector3((limbEllipse.XRadius + limbEllipse.XBias * limbEllipse.XRadius + 0.02f) * eyeCenterHeight, 0f, 0f);
                Vector3 limbXBias = Handles.Slider(handlePos, Vector3.right, limbHandleSize, Handles.ConeHandleCap, snapValue);
                float newLimbXBias = (limbXBias.x - limbEllipse.XRadius * eyeCenterHeight - 0.02f * eyeCenterHeight) / (limbEllipse.XRadius * eyeCenterHeight);
                newLimbXBias = Mathf.Clamp(newLimbXBias, Avatar.SoftEllipse.Constants.BiasMin, Avatar.SoftEllipse.Constants.BiasMax);

                if (check.changed)
                {
                    newEllipse.XBias = newLimbXBias;
                    activeHandleLabelName = $"{name} Offset";
                    activeHandleLabelPosition = handlePos;
                    activeHandleLabelMatrix = Handles.matrix;
                }
            }

            Handles.matrix = oldMatrix;
            return newEllipse;
        }

        bool BulgeHandles(Avatar.SoftBulge bulge, Avatar avatar, float bulgeYTop, float bulgeYBot, Vector3 sternumLocal, Vector3 hipsLocal, AnimationCurve spineZ, AnimationCurve ellipseX, AnimationCurve ellipseZ, AnimationCurve ellipseNegZ, float eyeCenterHeight, out Avatar.SoftBulge outBulge, bool mirror = false, bool onBack = false)
        {
            outBulge = new Avatar.SoftBulge();
            outBulge.Copy(bulge);
            bool changed = false;

            float onBackMult = onBack ? -1f : 1f;


            float mirrorMult = mirror ? -1f : 1f;
            float apexYAbs = Mathf.LerpUnclamped(bulgeYTop, bulgeYBot, bulge.apexY);
            float topToBotDiff = bulgeYBot - bulgeYTop;
            float minY = apexYAbs - (topToBotDiff * bulge.apexY) * bulge.upperY;
            float maxY = apexYAbs + (topToBotDiff * (1f - bulge.apexY)) * bulge.lowerY;



            Vector3 highLocal = minY < 1f ? Vector3.LerpUnclamped(sternumLocal, hipsLocal, minY) + spineZ.Evaluate(minY) * eyeCenterHeight * Vector3.forward : -avatar.CrotchBottom * eyeCenterHeight * (minY - 1f) * Vector3.up + hipsLocal;
            Vector3 apexLocal = apexYAbs < 1f ? Vector3.LerpUnclamped(sternumLocal, hipsLocal, apexYAbs) + spineZ.Evaluate(apexYAbs) * eyeCenterHeight * Vector3.forward : -avatar.CrotchBottom * eyeCenterHeight * (apexYAbs - 1f) * Vector3.up + hipsLocal;
            Vector3 lowLocal = maxY < 1f ? Vector3.LerpUnclamped(sternumLocal, hipsLocal, maxY) + spineZ.Evaluate(maxY) * eyeCenterHeight * Vector3.forward : -avatar.CrotchBottom * eyeCenterHeight * (maxY - 1f) * Vector3.up + hipsLocal;

            Vector3 highMidLocal = Vector3.LerpUnclamped(highLocal, apexLocal, .5f);
            Vector3 lowMidLocal = Vector3.LerpUnclamped(apexLocal, lowLocal, .5f);

            float minX = bulge.apexX - bulge.innerX;

            float innerRadians = minX * Mathf.Deg2Rad;
            float innerMidRadians = Mathf.LerpUnclamped(bulge.apexX, minX, .5f) * Mathf.Deg2Rad;
            float apexRadians = bulge.apexX * Mathf.Deg2Rad;
            float outerMidRadians = Mathf.LerpUnclamped(bulge.apexX, (bulge.apexX + bulge.outerX), .5f) * Mathf.Deg2Rad;
            float outerRadians = (bulge.apexX + bulge.outerX) * Mathf.Deg2Rad;




            float innerCos = Mathf.Cos(innerRadians) * onBackMult;
            float innerSin = Mathf.Sqrt(1f - (innerCos * innerCos)) * Mathf.Sign(innerRadians) * mirrorMult;
            float innerMidCos = Mathf.Cos(innerMidRadians) * onBackMult;
            float innerMidSin = Mathf.Sqrt(1f - (innerMidCos * innerMidCos)) * Mathf.Sign(innerMidRadians) * mirrorMult;
            float apexCos = Mathf.Cos(apexRadians) * onBackMult;
            float apexSin = Mathf.Sqrt(1f - (apexCos * apexCos)) * mirrorMult;

            float outerMidCos = Mathf.Cos(outerMidRadians) * onBackMult;
            float outerMidSin = Mathf.Sqrt(1f - (outerMidCos * outerMidCos)) * mirrorMult;
            float outerCos = Mathf.Cos(outerRadians) * onBackMult;
            float outerSin = Mathf.Sqrt(1f - (outerCos * outerCos)) * mirrorMult;


            float lowInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundLowerInner);
            float lowInnerCos = Mathf.Sqrt(1f - (lowInnerSin * lowInnerSin)) * onBackMult;
            float lowMidInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundLowerInner * .25f);
            float lowMidInnerCos = Mathf.Sqrt(1f - (lowMidInnerSin * lowMidInnerSin)) * onBackMult;
            float lowInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundLowerInner);
            float lowInnerMidCos = Mathf.Sqrt(1f - (lowInnerMidSin * lowInnerMidSin)) * onBackMult;
            float lowMidInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundLowerInner * .25f);
            float lowMidInnerMidCos = Mathf.Sqrt(1f - (lowMidInnerMidSin * lowMidInnerMidSin)) * onBackMult;

            float highInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundUpperInner);
            float highInnerCos = Mathf.Sqrt(1f - (highInnerSin * highInnerSin)) * onBackMult;
            float highMidInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundUpperInner * .25f);
            float highMidInnerCos = Mathf.Sqrt(1f - (highMidInnerSin * highMidInnerSin)) * onBackMult;
            float highInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundUpperInner);
            float highInnerMidCos = Mathf.Sqrt(1f - (highInnerMidSin * highInnerMidSin)) * onBackMult;
            float highMidInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundUpperInner * .25f);
            float highMidInnerMidCos = Mathf.Sqrt(1f - (highMidInnerMidSin * highMidInnerMidSin)) * onBackMult;

            if (minX < 0f)
            {
                if (innerSin * mirrorMult < 0f)
                {
                    innerSin = 0f;
                    innerCos = onBackMult;
                }
                if (innerMidSin * mirrorMult < 0f)
                {
                    innerMidSin = 0f;
                    innerMidCos = onBackMult;
                }
                if (lowInnerSin * mirrorMult < 0f)
                {
                    lowInnerSin = 0f;
                    lowInnerCos = onBackMult;
                }
                if (lowMidInnerSin * mirrorMult < 0f)
                {
                    lowMidInnerSin = 0f;
                    lowMidInnerCos = onBackMult;
                }
                if (lowInnerMidSin * mirrorMult < 0f)
                {
                    lowInnerMidSin = 0f;
                    lowInnerMidCos = onBackMult;
                }
                if (lowMidInnerMidSin * mirrorMult < 0f)
                {
                    lowMidInnerMidSin = 0f;
                    lowMidInnerMidCos = onBackMult;
                }
                if (highInnerSin * mirrorMult < 0f)
                {
                    highInnerSin = 0f;
                    highInnerCos = onBackMult;
                }
                if (highMidInnerSin * mirrorMult < 0f)
                {
                    highMidInnerSin = 0f;
                    highMidInnerCos = onBackMult;
                }
                if (highInnerMidSin * mirrorMult < 0f)
                {
                    highInnerMidSin = 0f;
                    highInnerMidCos = onBackMult;
                }
                if (highMidInnerMidSin * mirrorMult < 0f)
                {
                    highMidInnerMidSin = 0f;
                    highMidInnerMidCos = onBackMult;
                }
            }

            float lowOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundLowerOuter);
            float lowOuterSin = Mathf.Sqrt(1f - (lowOuterCos * lowOuterCos)) * mirrorMult;
            float lowMidOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundLowerOuter * .25f);
            float lowMidOuterSin = Mathf.Sqrt(1f - (lowMidOuterCos * lowMidOuterCos)) * mirrorMult;
            float lowOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundLowerOuter);
            float lowOuterMidSin = Mathf.Sqrt(1f - (lowOuterMidCos * lowOuterMidCos)) * mirrorMult;
            float lowMidOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundLowerOuter * .25f);
            float lowMidOuterMidSin = Mathf.Sqrt(1f - (lowMidOuterMidCos * lowMidOuterMidCos)) * mirrorMult;

            float highOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundUpperOuter);
            float highOuterSin = Mathf.Sqrt(1f - (highOuterCos * highOuterCos)) * onBackMult;
            float highMidOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundUpperOuter * .25f);
            float highMidOuterSin = Mathf.Sqrt(1f - (highMidOuterCos * highMidOuterCos)) * mirrorMult;
            float highOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundUpperOuter);
            float highOuterMidSin = Mathf.Sqrt(1f - (highOuterMidCos * highOuterMidCos)) * mirrorMult;
            float highMidOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundUpperOuter * .25f);
            float highMidOuterMidSin = Mathf.Sqrt(1f - (highMidOuterMidCos * highMidOuterMidCos)) * mirrorMult;

            Vector3 highInnerCoord = (avatar.GetSoftTorso(minY, new Vector2(highInnerSin, highInnerCos), ellipseX, ellipseZ, ellipseNegZ, out Vector3 softDisplace) + softDisplace) * eyeCenterHeight;

            Vector3 highApexCoord = (avatar.GetSoftTorso(minY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;

            Vector3 highOuterCoord = (avatar.GetSoftTorso(minY, new Vector2(highOuterSin, highOuterCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;

            float highMidY = Mathf.LerpUnclamped(minY, apexYAbs, .5f);


            Vector3 highMidApexCoord = (avatar.GetSoftTorso(highMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;








            Vector3 apexInnerCoord = (avatar.GetSoftTorso(apexYAbs, new Vector2(innerSin, innerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apexInnerMidCoord = (avatar.GetSoftTorso(apexYAbs, new Vector2(innerMidSin, innerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apexCoord = (avatar.GetSoftTorso(apexYAbs, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out Vector3 apexSoftDisplace) + apexSoftDisplace) * eyeCenterHeight;
            Vector3 apexOuterMidCoord = (avatar.GetSoftTorso(apexYAbs, new Vector2(outerMidSin, outerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apexOuterCoord = (avatar.GetSoftTorso(apexYAbs, new Vector2(outerSin, outerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;

            float lowMidY = Mathf.LerpUnclamped(apexYAbs, maxY, .5f);


            Vector3 lowMidApexCoord = (avatar.GetSoftTorso(lowMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;








            Vector3 lowInnerCoord = (avatar.GetSoftTorso(maxY, new Vector2(lowInnerSin, lowInnerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;

            Vector3 lowApexCoord = (avatar.GetSoftTorso(maxY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;

            Vector3 lowOuterCoord = (avatar.GetSoftTorso(maxY, new Vector2(lowOuterSin, lowOuterCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;

            Color apexColor = (bulge.rigidity < .5f) ? Color.LerpUnclamped(new Color(1f, 0f, 0f, 1f), new Color(.2f, 1f, 0f, 1f), bulge.rigidity * 2f) : Color.LerpUnclamped(new Color(.2f, 1f, 0f, 1f), new Color(0f, .5f, 1f, 1f), (bulge.rigidity - .5f) * 2f);
            Color halfwayColor = Color.LerpUnclamped(new Color(.2f, 1f, 0f, 1f), new Color(0f, .5f, 1f, 1f), bulge.rigidity);
            Color baseColor = new Color(0f, .5f, 1f, 1f);

            Handles.color = halfwayColor;
            Vector3 newSwellUpper = Handles.FreeMoveHandle(highMidApexCoord + highMidLocal, Quaternion.LookRotation(highMidApexCoord), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newSwellUpper != highMidApexCoord + highMidLocal)
            {
                changed = true;
                float delta = Vector3.Dot(newSwellUpper - (highMidApexCoord + highMidLocal), highMidApexCoord.normalized) * outBulge.apexZ * 100f / eyeCenterHeight;
                outBulge.swellUpper = Mathf.Clamp(outBulge.swellUpper + delta, 0f, .5f);
            }
            Vector3 newSwellLower = Handles.FreeMoveHandle(lowMidApexCoord + lowMidLocal, Quaternion.LookRotation(lowMidApexCoord), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newSwellLower != lowMidApexCoord + lowMidLocal)
            {
                changed = true;
                float delta = Vector3.Dot(newSwellLower - (lowMidApexCoord + lowMidLocal), lowMidApexCoord.normalized) * outBulge.apexZ * 100f / eyeCenterHeight;
                outBulge.swellLower = Mathf.Clamp(outBulge.swellLower + delta, 0f, .5f);
            }
            Vector3 newSwellInner = Handles.FreeMoveHandle(apexInnerMidCoord + apexLocal, Quaternion.LookRotation(apexInnerMidCoord), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newSwellInner != apexInnerMidCoord + highMidLocal)
            {
                changed = true;
                float delta = Vector3.Dot(newSwellInner - (apexInnerMidCoord + apexLocal), apexInnerMidCoord.normalized) * outBulge.apexZ * 100f / eyeCenterHeight;
                outBulge.swellInner = Mathf.Clamp(outBulge.swellInner + delta, 0f, .5f);
            }
            Vector3 newSwellOuter = Handles.FreeMoveHandle(apexOuterMidCoord + apexLocal, Quaternion.LookRotation(apexOuterMidCoord), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newSwellOuter != apexOuterMidCoord + highMidLocal)
            {
                changed = true;
                float delta = Vector3.Dot(newSwellOuter - (apexOuterMidCoord + apexLocal), apexOuterMidCoord.normalized) * outBulge.apexZ * 100f / eyeCenterHeight;
                outBulge.swellOuter = Mathf.Clamp(outBulge.swellOuter + delta, 0f, .5f);
            }

            Handles.color = baseColor;
            Vector3 newRoundUpperInner = Handles.FreeMoveHandle(highInnerCoord + highLocal, Quaternion.LookRotation(highInnerCoord), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newRoundUpperInner != highInnerCoord + highLocal)
            {
                changed = true;
                Vector3 delta = Quaternion.Inverse(Quaternion.LookRotation(highInnerCoord)) * (newRoundUpperInner - (highInnerCoord + highLocal)) * (outBulge.apexZ * 200f / eyeCenterHeight);
                outBulge.roundUpperInner = Mathf.Clamp(outBulge.roundUpperInner + delta.x, 0f, 1f);
                outBulge.upperY = Mathf.Clamp(outBulge.upperY + delta.y, .01f, 1f);
            }
            Vector3 newRoundUpperOuter = Handles.FreeMoveHandle(highOuterCoord + highLocal, Quaternion.LookRotation(highOuterCoord), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newRoundUpperOuter != highOuterCoord + highLocal)
            {
                changed = true;
                Vector3 delta = Quaternion.Inverse(Quaternion.LookRotation(highOuterCoord)) * (newRoundUpperOuter - (highOuterCoord + highLocal)) * (outBulge.apexZ * 200f / eyeCenterHeight);
                outBulge.roundUpperOuter = Mathf.Clamp(outBulge.roundUpperOuter - delta.x, 0f, 1f);
                outBulge.upperY = Mathf.Clamp(outBulge.upperY + delta.y, .01f, 1f);
            }
            Vector3 newRoundLowerInner = Handles.FreeMoveHandle(lowInnerCoord + lowLocal, Quaternion.LookRotation(lowInnerCoord.normalized), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newRoundLowerInner != highOuterCoord + lowLocal)
            {
                changed = true;
                Vector3 delta = Quaternion.Inverse(Quaternion.LookRotation(lowInnerCoord)) * (newRoundLowerInner - (lowInnerCoord + lowLocal)) * (outBulge.apexZ * 200f / eyeCenterHeight);
                outBulge.roundLowerInner = Mathf.Clamp(outBulge.roundLowerInner + delta.x, 0f, 1f);
                outBulge.lowerY = Mathf.Clamp(outBulge.lowerY - delta.y, .01f, 1f);
            }
            Vector3 newRoundLowerOuter = Handles.FreeMoveHandle(lowOuterCoord + lowLocal, Quaternion.LookRotation(lowOuterCoord), .003f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newRoundLowerOuter != lowOuterCoord + lowLocal)
            {
                changed = true;
                Vector3 delta = Quaternion.Inverse(Quaternion.LookRotation(lowOuterCoord)) * (newRoundLowerOuter - (lowOuterCoord + lowLocal)) * (outBulge.apexZ * 200f / eyeCenterHeight);
                outBulge.roundLowerOuter = Mathf.Clamp(outBulge.roundLowerOuter - delta.x, 0f, 1f);
                outBulge.lowerY = Mathf.Clamp(outBulge.lowerY - delta.y, .01f, 1f);
            }

            Handles.color = apexColor;
            Vector3 newApex = Handles.FreeMoveHandle(apexCoord + apexLocal, Quaternion.LookRotation(apexCoord), .005f * eyeCenterHeight, new Vector3(.001f, .001f, .001f), Handles.SphereHandleCap);
            if (newApex != apexCoord + apexLocal)
            {
                changed = true;
                Vector3 apexDelta = newApex - (apexCoord + apexLocal);
                Vector3 apexDeltaLocal = Quaternion.Inverse(Quaternion.LookRotation(apexCoord)) * apexDelta;
                outBulge.apexZ = apexDeltaLocal.z / eyeCenterHeight + outBulge.apexZ;
                outBulge.apexZ = Mathf.Clamp(outBulge.apexZ, 0f, .08f);
                outBulge.apexX = apexDeltaLocal.x * 100f / eyeCenterHeight + outBulge.apexX;
                outBulge.apexX = Mathf.Clamp(outBulge.apexX, 0f, 90f);
                outBulge.apexY = -apexDeltaLocal.y / (highLocal - lowLocal).magnitude + outBulge.apexY;
                outBulge.apexY = Mathf.Clamp(outBulge.apexY, .01f, .99f);
            }

            return changed;
        }

    }
}
