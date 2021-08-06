using System;
using System.Collections.Generic;
using System.Linq;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomEditor(typeof(AnimationSequencerController))]
    public sealed class AnimationSequencerControllerCustomEditor : Editor
    {
        private ReorderableList reorderableList;
        
        private AnimationSequencerController sequencerController;

        private static AnimationStepAdvancedDropdown cachedAnimationStepsDropdown;
        private static AnimationStepAdvancedDropdown AnimationStepAdvancedDropdown
        {
            get
            {
                if (cachedAnimationStepsDropdown == null)
                    cachedAnimationStepsDropdown = new AnimationStepAdvancedDropdown(new AdvancedDropdownState());
                return cachedAnimationStepsDropdown;
            }
        }

        private bool showPreview = true;
        private bool showSettings = false;
        private bool showCallbacks = false;

        private void OnEnable()
        {
            sequencerController = target as AnimationSequencerController;
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("animationSteps"), true, true, true, true);
            reorderableList.drawElementCallback += OnDrawAnimationStep;
            reorderableList.elementHeightCallback += GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback += OnClickToAddNew;
            reorderableList.onRemoveCallback += OnClickToRemove;
            reorderableList.onReorderCallback += OnListOrderChanged;
            reorderableList.drawHeaderCallback += OnDrawerHeader;
            Repaint();
        }

        private void OnDisable()
        {
            reorderableList.drawElementCallback -= OnDrawAnimationStep;
            reorderableList.elementHeightCallback -= GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback -= OnClickToAddNew;
            reorderableList.onRemoveCallback -= OnClickToRemove;
            reorderableList.onReorderCallback -= OnListOrderChanged;
            reorderableList.drawHeaderCallback -= OnDrawerHeader;

            if (!Application.isPlaying)
                DOTweenEditorPreview.Stop();
        }

        private void OnDrawerHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Animation Steps", EditorStyles.foldoutHeader);
        }
        
        private void AddNewAnimationStepOfType(Type targetAnimationType)
        {
            SerializedProperty animationStepsProperty = reorderableList.serializedProperty;
            int targetIndex = animationStepsProperty.arraySize;
            animationStepsProperty.InsertArrayElementAtIndex(targetIndex);
            SerializedProperty arrayElementAtIndex = animationStepsProperty.GetArrayElementAtIndex(targetIndex);
            object managedReferenceValue = Activator.CreateInstance(targetAnimationType);
            arrayElementAtIndex.managedReferenceValue = managedReferenceValue;
        
            //TODO copy from last step would be better here.
            SerializedProperty targetSerializedProperty = arrayElementAtIndex.FindPropertyRelative("target");
            if (targetSerializedProperty != null)
                targetSerializedProperty.objectReferenceValue = (serializedObject.targetObject as AnimationSequencerController)?.gameObject;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnClickToRemove(ReorderableList list)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(list.index);
            SerializedPropertyExtensions.ClearPropertyCache(element.propertyPath);
            reorderableList.serializedProperty.DeleteArrayElementAtIndex(list.index);
            reorderableList.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        private void OnListOrderChanged(ReorderableList list)
        {
            SerializedPropertyExtensions.ClearPropertyCache(list.serializedProperty.propertyPath);
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        private void OnClickToAddNew(Rect buttonRect, ReorderableList list)
        {
            AnimationStepAdvancedDropdown.Show(buttonRect, OnNewAnimationStepTypeSelected);
        }

        private void OnNewAnimationStepTypeSelected(AnimationStepAdvancedDropdownItem animationStepAdvancedDropdownItem)
        {
            AddNewAnimationStepOfType(animationStepAdvancedDropdownItem.AnimationStepType);
        }

        public override void OnInspectorGUI()
        {
            DrawFoldoutArea("Settings", ref showSettings, DrawSettings);
            DrawFoldoutArea("Preview", ref showPreview, DrawPreviewControls);
            DrawFoldoutArea("Callback", ref showCallbacks, DrawCallbacks);
            bool wasGUIEnabled = GUI.enabled;
            if (DOTweenEditorPreview.isPreviewing)
                GUI.enabled = false;

            reorderableList.DoLayoutList();

            GUI.enabled = wasGUIEnabled;
        }

        private void DrawCallbacks()
        {
            bool wasGUIEnabled = GUI.enabled;
            if (DOTweenEditorPreview.isPreviewing)
                GUI.enabled = false;
            SerializedProperty onStartEventSerializedProperty = serializedObject.FindProperty("onStartEvent");
            SerializedProperty onFinishedEventSerializedProperty = serializedObject.FindProperty("onFinishedEvent");
            SerializedProperty onProgressEventSerializedProperty = serializedObject.FindProperty("onProgressEvent");

            
            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(onStartEventSerializedProperty);
                EditorGUILayout.PropertyField(onFinishedEventSerializedProperty);
                EditorGUILayout.PropertyField(onProgressEventSerializedProperty);
                
                if (changedCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
            
            GUI.enabled = wasGUIEnabled;
        }

        private void DrawSettings()
        {
            SerializedProperty initializationModeSerializedProperty = serializedObject.FindProperty("initializeMode");
            SerializedProperty updateTypeSerializedProperty = serializedObject.FindProperty("updateType");
            SerializedProperty timeScaleIndependentSerializedProperty = serializedObject.FindProperty("timeScaleIndependent");
            SerializedProperty autoKillSerializedProperty = serializedObject.FindProperty("autoKill");

            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(initializationModeSerializedProperty);
                EditorGUILayout.PropertyField(updateTypeSerializedProperty);
                EditorGUILayout.PropertyField(timeScaleIndependentSerializedProperty);
                EditorGUILayout.PropertyField(autoKillSerializedProperty);
                
                if (changedCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawPreviewControls()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            
            bool guiEnabled = GUI.enabled;
                GUI.enabled = DOTweenEditorPreview.isPreviewing;
                if (GUILayout.Button(DOTweenActionEditorGUIUtility.BackButtonGUIContent, GUILayout.Width(40),
                    GUILayout.Height(40)))
                {
                    sequencerController.Rewind();
                }

                GUI.enabled = true;

                if (!DOTweenEditorPreview.isPreviewing && !Application.isPlaying)
                {
                    if (GUILayout.Button(DOTweenActionEditorGUIUtility.PlayButtonGUIContent, GUILayout.Width(40),
                        GUILayout.Height(40)))
                    {
                        if (!Application.isPlaying)
                            DOTweenEditorPreview.Start();
                        sequencerController.Play();
                        if (!Application.isPlaying)
                            DOTweenEditorPreview.PrepareTweenForPreview(sequencerController.PlayingSequence);
                    }
                }
                else
                {
                    if (!sequencerController.IsPlaying)
                    {
                        if (GUILayout.Button(DOTweenActionEditorGUIUtility.PlayButtonGUIContent, GUILayout.Width(40),
                            GUILayout.Height(40)))
                        {
                            if (!sequencerController.IsPlaying)
                            {
                                sequencerController.Play();
                            }
                            else
                            {
                                sequencerController.TogglePause();
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(DOTweenActionEditorGUIUtility.PauseButtonGUIContent, GUILayout.Width(40),
                            GUILayout.Height(40)))
                        {
                            sequencerController.TogglePause();
                        }
                    }
                }

                GUI.enabled = DOTweenEditorPreview.isPreviewing;
                if (GUILayout.Button(DOTweenActionEditorGUIUtility.ForwardButtonGUIContent, GUILayout.Width(40),
                    GUILayout.Height(40)))
                {
                    sequencerController.Complete();
                }

                float elapsedPercentage = 0;
                if (sequencerController.PlayingSequence != null)
                    elapsedPercentage = sequencerController.PlayingSequence.ElapsedPercentage();

                if (!DOTweenEditorPreview.isPreviewing || sequencerController.PlayingSequence.IsPlaying() &&
                    !Mathf.Approximately(elapsedPercentage, 0) || !Mathf.Approximately(elapsedPercentage, 1))
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button(DOTweenActionEditorGUIUtility.StopButtonGUIContent, GUILayout.Width(40),
                    GUILayout.Height(40)))
                {
                    if (!Application.isPlaying)
                        DOTweenEditorPreview.Stop();
                }

                GUI.enabled = guiEnabled;

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            DrawTimeScaleSlider();
            DrawProgressSlider();

            if (DOTweenEditorPreview.isPreviewing)
            {
                EditorGUILayout.HelpBox("Please be aware that deselecting this game object will break the tween and your object will be broken, don't have the scene, or make changes on the preview before stopping it", MessageType.Info);
            }
        }

        private void DrawProgressSlider()
        {
            GUILayout.FlexibleSpace();
            bool guiEnabled = GUI.enabled;

            GUI.enabled = sequencerController.PlayingSequence != null && DOTweenEditorPreview.isPreviewing;

            EditorGUI.BeginChangeCheck();
            float tweenProgress = 1;

            if (sequencerController.PlayingSequence != null)
                tweenProgress = sequencerController.PlayingSequence.ElapsedPercentage();

            EditorGUILayout.LabelField("Progress");
            tweenProgress = EditorGUILayout.Slider(tweenProgress, 0, 1);

            if (EditorGUI.EndChangeCheck())
            {
                if (sequencerController.PlayingSequence != null)
                {
                    sequencerController.PlayingSequence.Goto(tweenProgress *
                                                          sequencerController.PlayingSequence.Duration());
                }
            }

            GUI.enabled = guiEnabled;
            GUILayout.FlexibleSpace();
        }

        private void DrawTimeScaleSlider()
        {
            GUILayout.FlexibleSpace();
            bool guiEnabled = GUI.enabled;

            GUI.enabled = sequencerController.PlayingSequence != null && DOTweenEditorPreview.isPreviewing;

            EditorGUI.BeginChangeCheck();
            float tweenTimescale = 1;

            if (sequencerController.PlayingSequence != null)
                tweenTimescale = sequencerController.PlayingSequence.timeScale;

            EditorGUILayout.LabelField("TimeScale");
            tweenTimescale = EditorGUILayout.Slider(tweenTimescale, 0, 2);

            if (EditorGUI.EndChangeCheck())
            {
                if (sequencerController.PlayingSequence != null)
                    sequencerController.PlayingSequence.timeScale = tweenTimescale;
            }

            GUI.enabled = guiEnabled;
            GUILayout.FlexibleSpace();
        }

        private void DrawFoldoutArea(string title,ref bool foldout, Action additionalInspectorGUI)
        {
            using (new EditorGUILayout.VerticalScope("FrameBox"))
            {
                Rect rect = EditorGUILayout.GetControlRect();
                rect.x += 10;
                rect.width -= 10;
                rect.y -= 4;
                
                foldout = EditorGUI.BeginFoldoutHeaderGroup(rect, foldout, title);
                
                if (foldout)
                    additionalInspectorGUI.Invoke();
                EditorGUI.EndFoldoutHeaderGroup();
            }
        }
        
        private void OnDrawAnimationStep(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty flowTypeSerializedProperty = element.FindPropertyRelative("flowType");

            if (element.TryGetTargetObjectOfProperty(out AnimationStepBase animationStepBase))
            {
                if (animationStepBase.IsPlaying)
                {
                    reorderableList.index = index;
                }
            }

            FlowType flowType = (FlowType)flowTypeSerializedProperty.enumValueIndex;

            int baseIdentLevel = EditorGUI.indentLevel;
            
            GUIContent guiContent = new GUIContent(element.displayName);
            if (animationStepBase != null)
                guiContent = new GUIContent(animationStepBase.GetDisplayNameForEditor(index + 1));

            if (flowType == FlowType.Join)
                EditorGUI.indentLevel = baseIdentLevel + 1;
            
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.x += 10;
            rect.width -= 20;

            EditorGUI.PropertyField(
                rect,
                element,
                guiContent,
                false
            );

            EditorGUI.indentLevel = baseIdentLevel;
        }
        
        private float GetAnimationStepHeight(int index)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return element.GetPropertyDrawerHeight();
            
        }
    }
}
