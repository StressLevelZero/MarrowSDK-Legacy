using System;
using System.Collections.Generic;
using SLZ.MarrowEditor;
using UnityEditor;
using UnityEngine;

public class MarrowProjectValidationWindow : EditorWindow
{
    private GUIStyle windowStyle = null;
    private GUIStyle boldLabelStyle = null;
    private GUIStyle issueBoxStyle = null;
    private GUIStyle wrapStyle = null;

    [NonSerialized] public List<MarrowProjectValidation.MarrowValidationRule> ruleIssues = new List<MarrowProjectValidation.MarrowValidationRule>();
    [NonSerialized] public bool validateChecked = false;
    [NonSerialized] public bool issuesExist = false;

    [MenuItem("Stress Level Zero/Void Tools/Validate Marrow Project Settings", false, 50)]
    public static void ShowWindow()
    {
        var window = GetWindow<MarrowProjectValidationWindow>();
        window.titleContent = new GUIContent(" Marrow Project Validation", EditorGUIUtility.IconContent("Error@2x").image);
        window.minSize = new Vector2(500.0f, 300.0f);
        window.CheckValidation(true);

        window.Show();
    }

    private void CacheStyles()
    {
        if (windowStyle == null)
        {
            windowStyle = new GUIStyle(EditorStyles.label)
            {
                margin = new RectOffset(5, 5, 5, 5),
            };
        }

        if (boldLabelStyle == null)
        {
            boldLabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                stretchWidth = false

            };
        }

        if (issueBoxStyle == null)
        {

            issueBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {

            };
            var paddingOffset = issueBoxStyle.padding;
            int offset = (int)(EditorGUIUtility.singleLineHeight / 4f);
            paddingOffset.bottom += offset;
            paddingOffset.top += offset;
            paddingOffset.left += offset;
            paddingOffset.right += offset;
        }

        if (wrapStyle == null)
        {
            wrapStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
            };
        }
    }

    public void CheckValidation(bool force = false)
    {
        if (!validateChecked || force)
        {
            MarrowProjectValidation.GetIssues(ruleIssues);
            issuesExist = ruleIssues.Count > 0;
            validateChecked = true;

            if (issuesExist)
            {
                titleContent = new GUIContent(" Marrow Project Validation", EditorGUIUtility.IconContent("Error@2x").image);
            }
            else
            {
                titleContent = new GUIContent(" Marrow Project Validation", EditorGUIUtility.IconContent("P4_CheckOutRemote@2x").image);
            }
        }
    }

    public void OnGUI()
    {
        CacheStyles();
        CheckValidation();

        using (new EditorGUILayout.VerticalScope(windowStyle))
        {
            EditorGUILayout.Space();

            if (ruleIssues.Count <= 0)
            {
                EditorGUILayout.LabelField("No issues found in project.");

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Cool", MarrowGUIStyles.DefaultButton))
                    {
                        Close();
                    }
                }
            }
            else
            {

                EditorGUILayout.LabelField("Issues found in project!");

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"{ruleIssues.Count} Issue{(ruleIssues.Count > 1 ? "s" : "")}", boldLabelStyle);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Fix All", MarrowGUIStyles.DefaultButton))
                    {

                        foreach (var rule in ruleIssues)
                        {
                            rule.FixRule();
                            validateChecked = false;
                        }
                    }
                }

                EditorGUILayout.Space();


                foreach (var issue in ruleIssues)
                {
                    using (new EditorGUILayout.HorizontalScope(issueBoxStyle))
                    {
                        GUILayout.Label(EditorGUIUtility.IconContent("Error"));
                        GUILayout.Label(issue.message, wrapStyle);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Fix", issue.fixMessage), MarrowGUIStyles.DefaultButton))
                        {
                            issue.FixRule();
                            validateChecked = false;
                        }
                    }
                }
            }



        }
    }


}
