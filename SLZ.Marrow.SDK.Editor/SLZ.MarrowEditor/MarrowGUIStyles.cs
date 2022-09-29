using UnityEditor;
using UnityEngine;

namespace SLZ.MarrowEditor
{
    public static class MarrowGUIStyles
    {
        private static GUIStyle _defaultButton;
        public static GUIStyle DefaultButton
        {
            get
            {
                if (_defaultButton == null)
                {
                    _defaultButton = new GUIStyle(EditorStyles.miniButton)
                    {
                        stretchWidth = false
                    };
                    var paddingOffset = _defaultButton.padding;
                    int offset = (int)(EditorGUIUtility.singleLineHeight / 4f);
                    paddingOffset.left += offset;
                    paddingOffset.right += offset;
                }

                return _defaultButton;
            }
        }

        private static GUIStyle _defaultIconButton;
        public static GUIStyle DefaultIconButton
        {
            get
            {
                if (_defaultIconButton == null)
                {
                    _defaultIconButton = new GUIStyle(EditorStyles.miniButton)
                    {
                        stretchWidth = false
                    };
                    _defaultIconButton.padding = new RectOffset(0, 0, 0, 0);
                }

                return _defaultIconButton;
            }
        }

        private static GUIStyle _skinnyIconButton;
        public static GUIStyle SkinnyIconButton
        {
            get
            {
                if (_skinnyIconButton == null)
                {
                    _skinnyIconButton = new GUIStyle(EditorStyles.miniButton)
                    {
                        stretchWidth = false
                    };
                    _skinnyIconButton.padding = new RectOffset((int)-EditorGUIUtility.singleLineHeight / 4, (int)-EditorGUIUtility.singleLineHeight / 4, 0, 0);
                }

                return _skinnyIconButton;
            }
        }

        private static GUIStyle _boldFoldout;
        public static GUIStyle BoldFoldout
        {
            get
            {
                if (_boldFoldout == null)
                {
                    _boldFoldout = new GUIStyle(EditorStyles.foldout)
                    {
                        fontStyle = FontStyle.Bold
                    };
                }

                return _boldFoldout;
            }
        }


        private static GUIStyle _defaultRichText;
        public static GUIStyle DefaultRichText
        {
            get
            {
                if (_defaultRichText == null)
                {
                    _defaultRichText = new GUIStyle()
                    {
                        richText = true,
                        wordWrap = true,
                    };

                    _defaultRichText.padding = new RectOffset(0, 0, 0, 0);

                    if (EditorGUIUtility.isProSkin)
                    {
                        _defaultRichText.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
                    }
                    else
                    {
                        _defaultRichText.normal.textColor = Color.black;
                    }
                }

                return _defaultRichText;
            }
        }
    }
}