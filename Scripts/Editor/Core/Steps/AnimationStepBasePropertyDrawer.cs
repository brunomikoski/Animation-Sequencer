#if DOTWEEN_ENABLED
using System;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomPropertyDrawer(typeof(AnimationStepBase), true)]
    public class AnimationStepBasePropertyDrawer : PropertyDrawer
    {
        protected void DrawBaseGUI(Rect position, SerializedProperty property, GUIContent label, params string[] excludedPropertiesNames)
        {
            if (GUI.Button(new Rect(position.width - 40, position.y+2, 80, EditorGUIUtility.singleLineHeight - 1), "Duplicate"))
            {
                DuplicateProperty(property);
            }

            float originY = position.y;

            position.height = EditorGUIUtility.singleLineHeight;
            
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true, EditorStyles.foldout);

            if (property.isExpanded)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel--;
                
                position.height = EditorGUIUtility.singleLineHeight;
                position.y +=  EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                foreach (SerializedProperty serializedProperty in property.GetChildren())
                {
                    bool shouldDraw = true;
                    for (int i = 0; i < excludedPropertiesNames.Length; i++)
                    {
                        string excludedPropertyName = excludedPropertiesNames[i];
                        if (serializedProperty.name.Equals(excludedPropertyName, StringComparison.Ordinal))
                        {
                            shouldDraw = false;
                            break;
                        }
                    }

                    if (!shouldDraw)
                        continue;

                    EditorGUI.PropertyField(position, serializedProperty);
                    position.y += EditorGUI.GetPropertyHeight(serializedProperty) + EditorGUIUtility.standardVerticalSpacing;

                }
                
                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
            }
            
            property.SetPropertyDrawerHeight(position.y - originY + EditorGUIUtility.singleLineHeight);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawBaseGUI(position, property, label);
        }
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetPropertyDrawerHeight();
        }

        void DuplicateProperty(SerializedProperty property)
        {
            var parentArray = GetParentArrayProperty(property);
            if (parentArray != null && parentArray.isArray)
            {
                int index = GetIndexInArray(property);

                object sourceObject = property.managedReferenceValue;
                object clonedObject = CloneManagedReference(sourceObject);

                if (clonedObject != null)
                {
                    parentArray.InsertArrayElementAtIndex(index);

                    var newElement = parentArray.GetArrayElementAtIndex(index + 1);
                    newElement.managedReferenceValue = clonedObject;

                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        SerializedProperty GetParentArrayProperty(SerializedProperty property)
        {
            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');
            if (lastDot < 0)
                return null;

            string arrayPath = path.Substring(0, lastDot);
            return property.serializedObject.FindProperty(arrayPath);
        }

        int GetIndexInArray(SerializedProperty property)
        {
            var path = property.propertyPath;
            var start = path.IndexOf("[") + 1;
            var end = path.IndexOf("]");
            var indexStr = path.Substring(start, end - start);
            return int.Parse(indexStr);
        }

        object CloneManagedReference(object obj)
        {
            if (obj == null) return null;

            var method = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return method.Invoke(obj, null);
        }
    }
}
#endif