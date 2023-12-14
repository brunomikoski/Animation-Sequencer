using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    public static class AnimationSequencerStyles
    {
        public static readonly GUIStyle InspectorSideMargins;
        public static readonly GUIStyle InspectorTitlebar;

        static AnimationSequencerStyles()
        {
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