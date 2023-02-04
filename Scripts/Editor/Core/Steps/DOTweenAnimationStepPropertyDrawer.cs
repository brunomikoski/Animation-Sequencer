#if DOTWEEN_ENABLED
using System;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomPropertyDrawer(typeof(TweenStep))]
    public class DOTweenAnimationStepPropertyDrawer : AnimationStepBasePropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void AddNewActionOfType(SerializedProperty actionsSerializedProperty, Type targetType)
        {
            actionsSerializedProperty.arraySize++;
            SerializedProperty arrayElement =
                actionsSerializedProperty.GetArrayElementAtIndex(actionsSerializedProperty.arraySize - 1);
            arrayElement.managedReferenceValue = Activator.CreateInstance(targetType);

            string NameOfDirection = SequencerAnimationBase.NameOfDirection;
            string NameOfEase = SequencerAnimationBase.NameOfEase;
            string NameOfIsRelative = SequencerAnimationBase.NameOfIsRelative;

            if (actionsSerializedProperty.arraySize > 1)
            {
                SerializedProperty previousElement =
                    actionsSerializedProperty.GetArrayElementAtIndex(actionsSerializedProperty.arraySize - 2);

                if (AnimationControllerDefaults.Instance.PreferUsingPreviousDirection)
                {
                    SerializedProperty previousDirection = previousElement.FindPropertyRelative(NameOfDirection);
                    if (previousDirection != null)
                    {
                        SerializedProperty currentDirection = arrayElement.FindPropertyRelative(NameOfDirection);
                        if (currentDirection != null)
                            currentDirection.enumValueIndex = previousDirection.enumValueIndex;
                    }
                }

                if (AnimationControllerDefaults.Instance.PreferUsingPreviousActionEasing)
                {
                    SerializedProperty previousEase = previousElement.FindPropertyRelative(NameOfEase)
                        .FindPropertyRelative(NameOfEase);
                    if (previousEase != null)
                    {
                        SerializedProperty currentEase = arrayElement.FindPropertyRelative(NameOfEase)
                            .FindPropertyRelative(NameOfEase);
                        if (currentEase != null)
                            currentEase.enumValueIndex = previousEase.enumValueIndex;
                    }
                }
                else
                {
                    SerializedProperty currentEase =
                        arrayElement.FindPropertyRelative(NameOfEase).FindPropertyRelative(NameOfEase);
                    if (currentEase != null)
                        currentEase.enumValueIndex = (int) AnimationControllerDefaults.Instance.DefaultEasing.Ease;
                }


                if (AnimationControllerDefaults.Instance.PreferUsingPreviousRelativeValue)
                {
                    SerializedProperty previousEase = previousElement.FindPropertyRelative(NameOfIsRelative);
                    if (previousEase != null)
                    {
                        SerializedProperty currentEase = arrayElement.FindPropertyRelative(NameOfIsRelative);
                        if (currentEase != null)
                            currentEase.boolValue = previousEase.boolValue;
                    }
                }
                else
                {
                    SerializedProperty currentEase =
                        arrayElement.FindPropertyRelative(NameOfEase).FindPropertyRelative(NameOfEase);
                    if (currentEase != null)
                        currentEase.enumValueIndex = (int) AnimationControllerDefaults.Instance.DefaultEasing.Ease;
                }
            }
            else
            {
                SerializedProperty currentEase =
                    arrayElement.FindPropertyRelative(NameOfEase).FindPropertyRelative(NameOfEase);
                if (currentEase != null)
                    currentEase.enumValueIndex = (int) AnimationControllerDefaults.Instance.DefaultEasing.Ease;


                SerializedProperty currentDirection = arrayElement.FindPropertyRelative(NameOfDirection);
                if (currentDirection != null)
                    currentDirection.enumValueIndex = (int) AnimationControllerDefaults.Instance.DefaultDirection;

                SerializedProperty isRelativeSerializedProperty = arrayElement.FindPropertyRelative(NameOfIsRelative);
                if (isRelativeSerializedProperty != null)
                    isRelativeSerializedProperty.boolValue = AnimationControllerDefaults.Instance.UseRelative;
            }


            actionsSerializedProperty.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawBaseGUI(position, property, label, TweenStep.NameOfActions, TweenStep.NameOfLoopCount,
                TweenStep.NameOfLoopType);

            float originHeight = position.y;
            if (property.isExpanded)
            {
                if (EditorGUI.indentLevel > 0)
                    position = EditorGUI.IndentedRect(position);

                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel--;

                EditorGUI.BeginChangeCheck();

                SerializedProperty actionsSerializedProperty = property.FindPropertyRelative(TweenStep.NameOfActions);
                SerializedProperty targetSerializedProperty =
                    property.FindPropertyRelative(GameObjectRelatedStep.NameOfTarget);
                position.y += base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty loopCountSerializedProperty =
                    property.FindPropertyRelative(TweenStep.NameOfLoopCount);
                EditorGUI.PropertyField(position, loopCountSerializedProperty);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                loopCountSerializedProperty.intValue =
                    Mathf.Clamp(loopCountSerializedProperty.intValue, -1, int.MaxValue);
                if (loopCountSerializedProperty.intValue != 0)
                {
                    if (loopCountSerializedProperty.intValue == -1)
                    {
                        Debug.LogWarning("Infinity Loops doesn't work well with sequence, the best way of doing " +
                                         "that is setting to the int.MaxValue, will end eventually, but will take a really " +
                                         "long time, more info here: https://github.com/Demigiant/dotween/issues/92");
                        loopCountSerializedProperty.intValue = int.MaxValue;
                    }

                    SerializedProperty loopTypeSerializedProperty =
                        property.FindPropertyRelative(TweenStep.NameOfLoopType);
                    EditorGUI.PropertyField(position, loopTypeSerializedProperty);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                position.height = EditorGUIUtility.singleLineHeight;
                if (GUI.Button(position, "Add Actions"))
                {
                    AnimationSequenceEditorGUIUtility.DOTweenActionsDropdown.Show(position, actionsSerializedProperty,
                        targetSerializedProperty.objectReferenceValue,
                        item => { AddNewActionOfType(actionsSerializedProperty, item.BaseDOTweenActionType); });
                }

                position.y += 10;

                if (actionsSerializedProperty.arraySize > 0)
                    position.y += 26;

                for (int i = 0; i < actionsSerializedProperty.arraySize; i++)
                {
                    SerializedProperty actionSerializedProperty = actionsSerializedProperty.GetArrayElementAtIndex(i);

                    bool guiEnabled = GUI.enabled;
                    DrawDeleteActionButton(position, property, i);

                    if (GUI.enabled)
                    {
                        bool isValidTargetForRequiredComponent =
                            IsValidTargetForRequiredComponent(targetSerializedProperty, actionSerializedProperty);
                        GUI.enabled = isValidTargetForRequiredComponent;
                    }

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

        private static bool IsValidTargetForRequiredComponent(SerializedProperty targetSerializedProperty,
            SerializedProperty actionSerializedProperty)
        {
            if (targetSerializedProperty.objectReferenceValue == null)
                return false;

            Type type = actionSerializedProperty.GetTypeFromManagedFullTypeName();
            return AnimationSequenceEditorGUIUtility.CanActionBeAppliedToTarget(type,
                targetSerializedProperty.objectReferenceValue as GameObject);
        }

        private static void DrawDeleteActionButton(Rect position, SerializedProperty property, int targetIndex)
        {
            Rect buttonPosition = position;
            buttonPosition.width = 24;
            buttonPosition.x += position.width - 34;
            if (GUI.Button(buttonPosition, "X", EditorStyles.miniButton))
            {
                EditorApplication.delayCall += () => { DeleteElementAtIndex(property, targetIndex); };
            }
        }

        private static void DeleteElementAtIndex(SerializedProperty serializedProperty, int targetIndex)
        {
            SerializedProperty actionsPropertyPath = serializedProperty.FindPropertyRelative(TweenStep.NameOfActions);
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
#endif