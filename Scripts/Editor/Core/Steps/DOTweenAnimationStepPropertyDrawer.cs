using System;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomPropertyDrawer(typeof(DOTweenAnimationStep))]
    public class DOTweenAnimationStepPropertyDrawer : AnimationStepBasePropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void AddNewActionOfType(SerializedProperty actionsSerializedProperty, Type targetType)
        {
            actionsSerializedProperty.arraySize++;
            SerializedProperty arrayElement = actionsSerializedProperty.GetArrayElementAtIndex(actionsSerializedProperty.arraySize - 1);
            arrayElement.managedReferenceValue = Activator.CreateInstance(targetType);
            
            if (actionsSerializedProperty.arraySize > 1)
            {
                SerializedProperty previousElement = actionsSerializedProperty.GetArrayElementAtIndex(actionsSerializedProperty.arraySize - 2);
                SerializedProperty previousDirection = previousElement.FindPropertyRelative("direction");
                if (previousDirection != null)
                {
                    SerializedProperty currentDirection = arrayElement.FindPropertyRelative("direction");
                    if (currentDirection != null)
                        currentDirection.enumValueIndex = previousDirection.enumValueIndex;
                }

                SerializedProperty previousEase = previousElement.FindPropertyRelative("ease").FindPropertyRelative("ease");
                if (previousEase != null)
                {
                    SerializedProperty currentEase = arrayElement.FindPropertyRelative("ease").FindPropertyRelative("ease");
                    if (currentEase != null)
                        currentEase.enumValueIndex = previousEase.enumValueIndex;
                }
            }

            actionsSerializedProperty.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawBaseGUI(position, property, label, "actions", "loopCount", "loopType");

            float originHeight = position.y;
            if (property.isExpanded)
            {

                if (EditorGUI.indentLevel > 0)
                    position = EditorGUI.IndentedRect(position);

                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel--;

                EditorGUI.BeginChangeCheck();

                SerializedProperty actionsSerializedProperty = property.FindPropertyRelative("actions");
                SerializedProperty targetSerializedProperty = property.FindPropertyRelative("target");
                position.y += base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty loopCountSerializedProperty = property.FindPropertyRelative("loopCount");
                EditorGUI.PropertyField(position, loopCountSerializedProperty);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (loopCountSerializedProperty.intValue == -1 || loopCountSerializedProperty.intValue > 0)
                {
                    SerializedProperty loopTypeSerializedProperty = property.FindPropertyRelative("loopType");
                    EditorGUI.PropertyField(position, loopTypeSerializedProperty);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                position.height = EditorGUIUtility.singleLineHeight;
                if (GUI.Button(position, "Add Actions"))
                {
                    DOTweenActionEditorGUIUtility.DOTweenActionsDropdown.Show(position, actionsSerializedProperty, targetSerializedProperty.objectReferenceValue,
                        item =>
                        {
                            AddNewActionOfType(actionsSerializedProperty, item.BaseDOTweenActionType);
                        });
                }

                position.y += 10;

                if (actionsSerializedProperty.arraySize > 0)
                    position.y += 26;
                
                for (int i = 0; i < actionsSerializedProperty.arraySize; i++)
                {
                    SerializedProperty actionSerializedProperty = actionsSerializedProperty.GetArrayElementAtIndex(i);

                    bool guiEnabled = GUI.enabled;
                    DrawDeleteActionButton(position, property, i);
                    GUI.enabled = IsValidTargetForRequiredComponent(targetSerializedProperty, actionSerializedProperty);
                    
                    EditorGUI.PropertyField(position, actionSerializedProperty);
                    
                    
                    position.y += actionSerializedProperty.GetPropertyDrawerHeight();
                    
                    if (i < actionsSerializedProperty.arraySize - 1)
                        position.y += 30;

                    GUI.enabled = guiEnabled;
                }
                
                EditorGUI.indentLevel--;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel++;
                
                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
            }
            property.SetPropertyDrawerHeight(position.y - originHeight + EditorGUIUtility.singleLineHeight);
        }

        private static bool IsValidTargetForRequiredComponent(SerializedProperty targetSerializedProperty, SerializedProperty actionSerializedProperty)
        {
            if (targetSerializedProperty.objectReferenceValue == null)
                return false;

            Type type = actionSerializedProperty.GetTypeFromManagedFullTypeName();
            return DOTweenActionEditorGUIUtility.CanActionBeAppliedToTarget(type, targetSerializedProperty.objectReferenceValue as GameObject); 
        }

        private static void DrawDeleteActionButton(Rect position, SerializedProperty property, int targetIndex)
        {
            Rect buttonPosition = position;
            buttonPosition.width = 24;
            buttonPosition.x += position.width - 34;
            if (GUI.Button(buttonPosition, "X", EditorStyles.miniButton))
            {
                EditorApplication.delayCall += () =>
                {
                    DeleteElementAtIndex(property, targetIndex);
                };
            }
        }

        private static void DeleteElementAtIndex(SerializedProperty serializedProperty, int targetIndex)
        {
            SerializedProperty actionsPropertyPath = serializedProperty.FindPropertyRelative("actions");
            actionsPropertyPath.DeleteArrayElementAtIndex(targetIndex);
            SerializedPropertyExtensions.ClearPropertyCache(actionsPropertyPath.propertyPath);
            actionsPropertyPath.serializedObject.ApplyModifiedProperties();
            actionsPropertyPath.serializedObject.Update();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetPropertyDrawerHeight();
        }
    }
}
