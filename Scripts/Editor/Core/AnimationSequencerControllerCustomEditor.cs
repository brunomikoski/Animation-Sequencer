using System;
using System.Collections.Generic;
using DG.DOTweenEditor;
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
        private bool isPreviewPlaying;
        private double lastFrameTime;
        private float frameDelta;
        private AnimationSequencerController[] activeSequencers = new AnimationSequencerController[0];

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
            CalculateTotalAnimationTime();
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
            StopPreview();
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
            EditorGUI.BeginChangeCheck();
            DrawBoxedArea("Settings", DrawSettings);
            DrawBoxedArea("Preview", DrawPreviewControls);
            bool wasGUIEnabled = GUI.enabled;
            if (sequencerController.IsPlaying)
                GUI.enabled = false;
            
            reorderableList.DoLayoutList();
            GUI.enabled = wasGUIEnabled;
            if (EditorGUI.EndChangeCheck())
                CalculateTotalAnimationTime();
        }

        private void DrawSettings()
        {
            SerializedProperty initializationModeSerializedProperty = serializedObject.FindProperty("initializeMode");
            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(initializationModeSerializedProperty);
                if (changedCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
        }

        private void CalculateTotalAnimationTime()
        {
            SerializedProperty durationSerializedProperty = serializedObject.FindProperty("duration");
            if (durationSerializedProperty == null)
                return;

            float sequenceDuration = 0;
            for (int i = 0; i < sequencerController.AnimationSteps.Length; i++)
            {
                AnimationStepBase animationStep = sequencerController.AnimationSteps[i];
                sequenceDuration += animationStep.Delay + animationStep.Duration;
            }

            if (!Mathf.Approximately(sequenceDuration, durationSerializedProperty.floatValue))
            {
                durationSerializedProperty.floatValue = sequenceDuration;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawPreviewControls()
        {
            if (!isPreviewPlaying)
            {
                GameObject sequencerGameObject = sequencerController.gameObject;
                EditorGUI.BeginDisabledGroup(!sequencerGameObject.activeSelf || !sequencerGameObject.activeInHierarchy);
                if (GUILayout.Button("Play"))
                {
                    Play();
                }
                
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (GUILayout.Button("Stop"))
                {
                    StopPreview();
                }
            }
        }

        private void Play()
        {
            if (!Application.isPlaying)
            {
                EditorApplication.update += EditorUpdate;
                DOTweenEditorPreview.Start();
                FindRelatedAnimationControllers();
                sequencerController.OnSequenceFinishedPlayingEvent += StopPreview;
                lastFrameTime = EditorApplication.timeSinceStartup;
                isPreviewPlaying = true;
            }

            sequencerController.PrepareForPlay(true);
            sequencerController.Play();
        }

        private void FindRelatedAnimationControllers()
        {
            if (Application.isPlaying)
                return;

            List<AnimationSequencerController> sequencers = new List<AnimationSequencerController>
            {
                sequencerController
            };
            for (int i = 0; i < sequencerController.AnimationSteps.Length; i++)
            {
                AnimationStepBase sequencerControllerAnimationStep = sequencerController.AnimationSteps[i];
                if (sequencerControllerAnimationStep is PlaySequenceAnimationStep playSequenceAnimationStep)
                    sequencers.Add(playSequenceAnimationStep.Target);
            }

            activeSequencers = sequencers.ToArray();
        }

        private void StopPreview()
        {
            sequencerController.OnSequenceFinishedPlayingEvent -= StopPreview;
            for (int i = 0; i < activeSequencers.Length; i++)
            {
                AnimationSequencerController animationSequencerController = activeSequencers[i];
                if (animationSequencerController == null)
                    continue;
                
                animationSequencerController.Stop();
                animationSequencerController.Complete();
            }
           
            isPreviewPlaying = false;
            
            if (!Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
                DOTweenEditorPreview.Stop(true);
            }
            
            Repaint();
        }

        private void EditorUpdate()
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (!isPreviewPlaying)
                return;
            
            frameDelta = (float) (EditorApplication.timeSinceStartup - lastFrameTime);
            lastFrameTime = EditorApplication.timeSinceStartup;

            for (int i = 0; i < activeSequencers.Length; i++)
            {
                AnimationSequencerController animationSequencerController = activeSequencers[i];
                if (animationSequencerController == null)
                    continue;
                
                animationSequencerController.UpdateStep(frameDelta);
            }
        }

        private void DrawBoxedArea(string title, Action additionalInspectorGUI)
        {
            using (new EditorGUILayout.VerticalScope("FrameBox"))
            {
                Rect rect = EditorGUILayout.GetControlRect();
                rect.x -= 4;
                rect.width += 8;
                rect.y -= 4;
                GUIStyle foldoutHeader = new GUIStyle(EditorStyles.foldoutHeader);
                    
                EditorGUI.LabelField(rect, title, foldoutHeader);
                EditorGUILayout.Space();
                additionalInspectorGUI.Invoke();
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
