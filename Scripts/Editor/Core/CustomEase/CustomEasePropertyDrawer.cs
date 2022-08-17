#if DOTWEEN_ENABLED
using DG.Tweening;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomPropertyDrawer(typeof(CustomEase))]
    public class CustomEasePropertyDrawer : PropertyDrawer
    {
        private static CustomEaseAdvancedDropdown easeDropdown = new CustomEaseAdvancedDropdown(new AdvancedDropdownState());

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty easeProperty = property.FindPropertyRelative("ease");

            if (easeProperty.enumValueIndex == (int)Ease.INTERNAL_Custom)
            {
                return EditorGUIUtility.singleLineHeight * 2
                    + EditorGUIUtility.standardVerticalSpacing;
            }

            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty easeProperty = property.FindPropertyRelative("ease");

            string displayName = "Custom";
            if (easeProperty.enumValueIndex != (int)Ease.INTERNAL_Custom)
            {
                displayName = ((Ease) easeProperty.enumValueIndex).ToString();
            }

            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();

            Rect popupRect = position;
            Rect buttonRect = position;

            buttonRect.width = 20;
            popupRect.width -= buttonRect.width;
            buttonRect.x += popupRect.width;
            buttonRect.height -= 1;

            Rect displayRect = EditorGUI.PrefixLabel(popupRect, new GUIContent("Ease"));
            GUIStyle leftAlignedButton = EditorStyles.miniButton;
            leftAlignedButton.alignment = TextAnchor.MiddleLeft;
            if (GUI.Button(displayRect, displayName, leftAlignedButton))
            {
                easeDropdown.Show(displayRect, item =>
                {
                    easeProperty.enumValueIndex = item.EaseEnumIndex;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            if (GUI.Button(buttonRect, "?", EditorStyles.miniButton))
            {
                Application.OpenURL("https://easings.net/");
            }

            EditorGUI.indentLevel++;

            if (easeProperty.enumValueIndex == (int)Ease.INTERNAL_Custom)
            {
                SerializedProperty curveProperty = property.FindPropertyRelative("curve");
                position.y += EditorGUIUtility.singleLineHeight
                    + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, curveProperty);
            }

            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif