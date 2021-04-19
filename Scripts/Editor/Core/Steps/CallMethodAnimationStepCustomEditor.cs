using BrunoMikoski.AnimationSequencer;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CallMethodAnimationStep))]
public class CallMethodAnimationStepCustomEditor : AnimationStepBasePropertyDrawer
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
            SerializedProperty methodToCallSerializedProperty = property.FindPropertyRelative("methodToCall");
            float propertyHeight = EditorGUI.GetPropertyHeight(methodToCallSerializedProperty, label);
            position.y += propertyHeight;
        }
        
        property.SetPropertyDrawerHeight(position.y - originY + EditorGUIUtility.singleLineHeight);
    }
}
