using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SLZ.MarrowEditor
{
    [CustomEditor(typeof(TextureStreamSettings))]
    public class TextureStreamSettingsEditor : Editor
    {
        private TextureStreamTool.TextureStreamToolResults toolResults;

        private bool preventFoldout = false;

        private bool enabledFoldout = false;
        private bool alreadyFoldout = false;
        private bool skippedFoldout = false;
        private bool disabledFoldout = false;
        private bool updatedFoldout = false;

        private bool enabledTextFoldout = false;
        private bool alreadyTextFoldout = false;
        private bool skippedTextFoldout = false;
        private bool disabledTextFoldout = false;
        private bool updatedTextFoldout = false;

        private static GUIStyle richFoldoutStyle = null;
        private static GUIContent deleteIcon = null;
        private static GUIStyle iconButtonStyle = null;

        public override void OnInspectorGUI()
        {
            if (richFoldoutStyle == null)
            {
                richFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                richFoldoutStyle.richText = true;
            }
            if (deleteIcon == null)
            {
                deleteIcon = EditorGUIUtility.IconContent("P4_DeletedRemote@2x");
                deleteIcon.tooltip = "Remove";
            }
            if (iconButtonStyle == null)
            {
                iconButtonStyle = new GUIStyle(GUI.skin.button);
                iconButtonStyle.padding = new RectOffset(0, 0, 0, 0);
            }

            if (GUILayout.Button("Analyze Textures", MarrowGUIStyles.DefaultButton))
            {
                toolResults = TextureStreamTool.Analyze();
            }

            if (GUILayout.Button("Fix Textures", MarrowGUIStyles.DefaultButton))
            {
                toolResults = TextureStreamTool.ApplyTextureStreamingToAllTextures();
            }

            preventFoldout = EditorGUILayout.Foldout(preventFoldout, "Prevent Streaming Textures");
            if (preventFoldout)
            {
                string removeTextureGuid = null;
                foreach (var textureGuid in TextureStreamSettings.Instance.textureStreamingPreventionGuids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(textureGuid);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (string.IsNullOrEmpty(assetPath))
                        {
                            if (GUILayout.Button(deleteIcon, iconButtonStyle, GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                            {
                                removeTextureGuid = textureGuid;
                            }
                            EditorGUILayout.TextField(textureGuid);
                        }
                        else
                        {
                            EditorGUILayout.TextField(assetPath);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(removeTextureGuid))
                {
                    TextureStreamSettings.Instance.SetTexturePreventedFromStreamingByGuid(removeTextureGuid, false);
                }
            }

            if (toolResults != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);

                EditorGUILayout.LabelField($"Total Textures: {toolResults.totalTextures}");

                DrawAnalysisSection("Enabled Streaming", toolResults.enabledList, toolResults.enabledFullText,
                    ref enabledFoldout, ref enabledTextFoldout);
                DrawAnalysisSection("Already Streaming", toolResults.alreadyList, toolResults.alreadyFullText,
                    ref alreadyFoldout, ref alreadyTextFoldout);
                DrawAnalysisSection("Skipped", toolResults.skippedList, toolResults.skippedFullText,
                    ref skippedFoldout, ref skippedTextFoldout);
                DrawAnalysisSection("Disabled Streaming", toolResults.disabledList, toolResults.disabledFullText,
                    ref disabledFoldout, ref disabledTextFoldout);
                DrawAnalysisSection("Updated", toolResults.updatedList, toolResults.updatedFullText,
                    ref updatedFoldout, ref updatedTextFoldout);
            }
        }

        private void DrawAnalysisSection(string sectionLabel, List<string> lines, string fullText, ref bool foldout, ref bool textFoldout)
        {
            foldout = EditorGUILayout.Foldout(foldout, $"<b>{sectionLabel}</b> <i>({lines.Count})</i>", richFoldoutStyle);
            if (foldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    textFoldout = EditorGUILayout.Foldout(textFoldout, "Full Text");
                    if (textFoldout)
                        using (new EditorGUI.IndentLevelScope())
                            EditorGUILayout.TextArea(fullText.TrimStart().TrimEnd());
                    foreach (var line in lines)
                    {
                        EditorGUILayout.TextField(line);
                    }
                }
            }
        }
    }
}