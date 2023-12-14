using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    public static class AnimationSequencerStyles
    {
        public static readonly GUIStyle Badge;
        public static readonly GUIStyle InspectorSideMargins;
        public static readonly GUIStyle InspectorTitlebar;

        static AnimationSequencerStyles()
        {
            Badge = new GUIStyle(GUI.skin.label)
            {
                normal =
                {
                    background = EditorGUIUtility.whiteTexture,
                    textColor = new Color(0.1f, 0.1f, 0.1f),
                },
                fontSize = EditorStyles.miniLabel.fontSize,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(5, 5, 0, 0),
                margin = new RectOffset(5, 5, 0, 0),
                fixedHeight = 14,
            };
            InspectorTitlebar = GetStyle("IN Title", GUI.skin.label);
            InspectorSideMargins = new GUIStyle(EditorStyles.inspectorDefaultMargins)
            {
                margin = {top = 0, bottom = 0},
                padding = {top = 0, bottom = 0},
            };
        }

        private static GUIStyle GetStyle(string styleName, GUIStyle fallback)
        {
            return GUI.skin.FindStyle(styleName) ??
                   EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName) ??
                   fallback;
        }
    }
}