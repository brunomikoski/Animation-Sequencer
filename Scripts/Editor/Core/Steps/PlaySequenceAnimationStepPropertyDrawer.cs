using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomPropertyDrawer(typeof(PlaySequenceAnimationStep), true)]
    public sealed class PlaySequenceAnimationStepPropertyDrawer : AnimationStepBasePropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawBaseGUI(position, property, label);

            float originY = position.y;

            position.height = EditorGUIUtility.singleLineHeight;
            
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel--;
                
                position.height = EditorGUIUtility.singleLineHeight;
                position.y += base.GetPropertyHeight(property, label)+ EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.BeginDisabledGroup(true);
                SerializedProperty sequencerSerializedProperty = property.FindPropertyRelative("sequencer");

                string duration = "???";
                if (sequencerSerializedProperty.objectReferenceValue != null)
                {
                    AnimationSequencerController sequencer = sequencerSerializedProperty.objectReferenceValue as AnimationSequencerController;
                    duration = sequencer.Duration.ToString(CultureInfo.InvariantCulture);
                }
                EditorGUI.TextField(position, "Duration", duration);
                EditorGUI.EndDisabledGroup();
            }
            property.SetPropertyDrawerHeight(position.y - originY + EditorGUIUtility.singleLineHeight);
        }
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetPropertyDrawerHeight();
        }
    }
}
