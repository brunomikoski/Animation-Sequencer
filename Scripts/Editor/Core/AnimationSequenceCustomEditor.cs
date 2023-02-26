#if DOTWEEN_ENABLED
using System;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomEditor(typeof(AnimationSequence), true)]
    public sealed class AnimationSequenceCustomEditor : Editor
    {
        private ReorderableList reorderableList;
        
        private AnimationSequence animationSequence;

        private static AnimationStepAdvancedDropdown cachedAnimationStepsDropdown;
        private static AnimationStepAdvancedDropdown AnimationStepAdvancedDropdown =>
            cachedAnimationStepsDropdown ??= new AnimationStepAdvancedDropdown(new AdvancedDropdownState());

        private bool showPreviewPanel = true;
        private bool showSettingsPanel;
        private bool showCallbacksPanel;
        private bool showSequenceSettingsPanel;
        private bool showStepsPanel = true;
        private float tweenTimeScale = 1f;
        private bool wasShowingStepsPanel;
        private bool justStartPreviewing;

        private void OnEnable()
        {
            animationSequence = target as AnimationSequence;
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfAnimationSteps), true, false, true, true);
            reorderableList.drawElementCallback += OnDrawAnimationStep;
            reorderableList.elementHeightCallback += GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback += OnClickToAddNew;
            reorderableList.onRemoveCallback += OnClickToRemove;
            reorderableList.onReorderCallback += OnListOrderChanged;
            reorderableList.drawHeaderCallback += OnDrawerHeader;
            EditorApplication.update += EditorUpdate;
            EditorApplication.playModeStateChanged += OnEditorPlayModeChanged;
            
#if UNITY_2021_1_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabSaving += PrefabSaving;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabSaving += PrefabSaving;
#endif
            
            Repaint();
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void OnDisable()
        {
            reorderableList.drawElementCallback -= OnDrawAnimationStep;
            reorderableList.elementHeightCallback -= GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback -= OnClickToAddNew;
            reorderableList.onRemoveCallback -= OnClickToRemove;
            reorderableList.onReorderCallback -= OnListOrderChanged;
            reorderableList.drawHeaderCallback -= OnDrawerHeader;
            EditorApplication.playModeStateChanged -= OnEditorPlayModeChanged;
            EditorApplication.update -= EditorUpdate;

#if UNITY_2021_1_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabSaving -= PrefabSaving;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabSaving -= PrefabSaving;
#endif

            if (!Application.isPlaying)
            {
                if (DOTweenEditorPreview.isPreviewing)
                {
                    animationSequence.ResetToInitialState();
                    DOTweenEditorPreview.Stop();            
                }
            }
            
            tweenTimeScale = 1f;
        }

        private void EditorUpdate()
        {
            if (Application.isPlaying) 
                return;

            SerializedProperty progressSP = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfProgress);
            
            if (Mathf.Approximately(progressSP.floatValue, -1)) 
                return;
            
            SetProgress(progressSP.floatValue);
        }

        private void OnEditorPlayModeChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingEditMode)
            {
                if (DOTweenEditorPreview.isPreviewing)
                {
                    animationSequence.ResetToInitialState();
                    DOTweenEditorPreview.Stop();            
                }
            }
        }
        
        private void PrefabSaving(GameObject gameObject)
        {
            if (DOTweenEditorPreview.isPreviewing)
            {
                animationSequence.ResetToInitialState();
                DOTweenEditorPreview.Stop();            
            }
        }
        
        private void OnDrawerHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Animation Steps");
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
            SerializedProperty targetSerializedProperty = arrayElementAtIndex.FindPropertyRelative(GameObjectRelatedStep.NameOfTarget);
            if (targetSerializedProperty != null)
                targetSerializedProperty.objectReferenceValue = (serializedObject.targetObject as AnimationSequence)?.gameObject;
            
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
            if (animationSequence.IsResetRequired())
            {
                SetDefaults();
            }

            DrawFoldoutArea("Animation Sequence Settings", ref showSettingsPanel, DrawSettings);
            DrawFoldoutArea("Callback", ref showCallbacksPanel, DrawCallbacks);
            DrawFoldoutArea("Preview", ref showPreviewPanel, DrawPreviewControls);
            DrawFoldoutArea("Sequence Settings", ref showSequenceSettingsPanel, DrawSequenceSettings);
            DrawFoldoutArea("Steps", ref showStepsPanel, DrawAnimationSteps);
        }

        private void DrawAnimationSteps()
        {
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
            
            SerializedProperty onStartEventSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfOnStartEvent);
            SerializedProperty onFinishedEventSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfOnFinishedEvent);
            SerializedProperty onProgressEventSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfOnProgressEvent);

            
            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(onStartEventSerializedProperty);
                EditorGUILayout.PropertyField(onFinishedEventSerializedProperty);
                EditorGUILayout.PropertyField(onProgressEventSerializedProperty);
                
                if (changedCheck.changed) serializedObject.ApplyModifiedProperties();
            }
            
            GUI.enabled = wasGUIEnabled;
        }

        private void DrawSettings()
        {
            SerializedProperty autoPlayModeSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfAutoPlayMode);
            SerializedProperty pauseOnAwakeSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfStartPaused);

            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                AnimationSequence.AutoplayType autoplayMode = (AnimationSequence.AutoplayType)autoPlayModeSerializedProperty.enumValueIndex;
                EditorGUILayout.PropertyField(autoPlayModeSerializedProperty);

                if (autoplayMode != AnimationSequence.AutoplayType.Nothing)
                    EditorGUILayout.PropertyField(pauseOnAwakeSerializedProperty);
				
                DrawPlaybackSpeedSlider();
                
                if (changedCheck.changed) 
                    serializedObject.ApplyModifiedProperties();
            }
        }
		
        private void DrawPlaybackSpeedSlider()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            
            SerializedProperty playbackSpeedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfPlaybackSpeed);
            playbackSpeedProperty.floatValue = EditorGUILayout.Slider("Playback Speed", playbackSpeedProperty.floatValue, 0, 2);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                UpdateSequenceTimeScale();
            }

            GUILayout.FlexibleSpace();
        }
        
        private void UpdateSequenceTimeScale()
        {
            if (animationSequence.PlayingSequence == null)
                return;
            
            animationSequence.PlayingSequence.timeScale = animationSequence.PlaybackSpeed * tweenTimeScale;
        }
        
        private void DrawSequenceSettings()
        {
            bool wasEnabled = GUI.enabled; 
            if (DOTweenEditorPreview.isPreviewing)
                GUI.enabled = false;
            
            SerializedProperty updateTypeSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfUpdateType);
            SerializedProperty timeScaleIndependentSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfTimeScaleIndependent);
            SerializedProperty sequenceDirectionSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfPlayType);
            SerializedProperty loopsSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfLoops);
            SerializedProperty loopTypeSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfLoopType);
            SerializedProperty autoKillSerializedProperty = serializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfAutoKill);

            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(timeScaleIndependentSerializedProperty);
                EditorGUILayout.PropertyField(sequenceDirectionSerializedProperty);
                EditorGUILayout.PropertyField(updateTypeSerializedProperty);
                EditorGUILayout.PropertyField(autoKillSerializedProperty);

                EditorGUILayout.PropertyField(loopsSerializedProperty);

                if (loopsSerializedProperty.intValue != 0)
                {
                    EditorGUILayout.PropertyField(loopTypeSerializedProperty);
                }
 
                if (changedCheck.changed)
                {
                    loopsSerializedProperty.intValue = Mathf.Clamp(loopsSerializedProperty.intValue, -1, int.MaxValue);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUI.enabled = wasEnabled;
        }

        private void DrawPreviewControls()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            
            bool guiEnabled = GUI.enabled;

            GUIStyle previewButtonStyle = new GUIStyle(GUI.skin.button);
            previewButtonStyle.fixedWidth = previewButtonStyle.fixedHeight = 40;
            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.BackButtonGUIContent, previewButtonStyle))
            {
                if (!animationSequence.IsPlaying)
                    PlaySequence();

                animationSequence.Rewind();
            }

            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.StepBackGUIContent, previewButtonStyle))
            {
                if(!animationSequence.IsPlaying)
                    PlaySequence();

                StepBack();
            }

            if (animationSequence.IsPlaying)
            {
                if (GUILayout.Button(AnimationSequenceEditorGUIUtility.PauseButtonGUIContent, previewButtonStyle))
                {
                    animationSequence.Pause();
                }
            }
            else
            {
                if (GUILayout.Button(AnimationSequenceEditorGUIUtility.PlayButtonGUIContent, previewButtonStyle))
                {
                    PlaySequence();
                }
            }

            
            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.StepNextGUIContent, previewButtonStyle))
            {
                if(!animationSequence.IsPlaying)
                    PlaySequence();

                StepNext();
            }
            
            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.ForwardButtonGUIContent, previewButtonStyle))
            {
                if (!animationSequence.IsPlaying)
                    PlaySequence();

                animationSequence.Complete();
            }

            if (!Application.isPlaying)
            {
                GUI.enabled = DOTweenEditorPreview.isPreviewing;
                if (GUILayout.Button(AnimationSequenceEditorGUIUtility.StopButtonGUIContent, previewButtonStyle))
                {
                    animationSequence.Rewind();
                    DOTween.Kill(animationSequence.PlayingSequence);
                    DOTweenEditorPreview.Stop();
                    animationSequence.ResetToInitialState();
                    animationSequence.ClearPlayingSequence();
                    if (AnimationSequencerSettings.GetInstance().AutoHideStepsWhenPreviewing)
                        showStepsPanel = wasShowingStepsPanel;
                }
            }

            GUI.enabled = guiEnabled;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            DrawTimeScaleSlider();
            DrawProgressSlider();
        }

        private void StepBack()
        {
            if (!animationSequence.IsPlaying)
                PlaySequence();
            
            animationSequence.PlayingSequence.Goto((animationSequence.PlayingSequence.ElapsedPercentage() -
                                                      0.01f) * animationSequence.PlayingSequence.Duration());
        }

        private void StepNext()
        {
            if (!animationSequence.IsPlaying)
                PlaySequence();

            animationSequence.PlayingSequence.Goto((animationSequence.PlayingSequence.ElapsedPercentage() +
                                                      0.01f) * animationSequence.PlayingSequence.Duration());
        }

        private void PlaySequence()
        {
            justStartPreviewing = false;
            if (!Application.isPlaying)
            {
                if (!DOTweenEditorPreview.isPreviewing)
                {
                    justStartPreviewing = true;
                    DOTweenEditorPreview.Start();

                    animationSequence.Play();
                    
                    DOTweenEditorPreview.PrepareTweenForPreview(animationSequence.PlayingSequence);
                }
                else
                {
                    if (animationSequence.PlayingSequence == null)
                    {
                        animationSequence.Play();
                    }
                    else
                    {
                        if (!animationSequence.PlayingSequence.IsBackwards() &&
                            animationSequence.PlayingSequence.fullPosition >= animationSequence.PlayingSequence.Duration())
                        {
                            animationSequence.Rewind();
                        }
                        else if (animationSequence.PlayingSequence.IsBackwards() &&
                                 animationSequence.PlayingSequence.fullPosition <= 0f)
                        {
                            animationSequence.Complete();
                        }

                        animationSequence.TogglePause();
                    }
                }
            }
            else
            {
                if (animationSequence.PlayingSequence == null)
                    animationSequence.Play();
                else
                {
                    if (animationSequence.PlayingSequence.IsActive())
                        animationSequence.TogglePause();
                    else
                        animationSequence.Play();
                }
            }

            if (justStartPreviewing)
                wasShowingStepsPanel = showStepsPanel;
            
            showStepsPanel = !AnimationSequencerSettings.GetInstance().AutoHideStepsWhenPreviewing;
        }

        private void DrawProgressSlider()
        {
            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();
            float tweenProgress = 0;

            tweenProgress = GetCurrentSequencerProgress();

            EditorGUILayout.LabelField("Progress");
            tweenProgress = EditorGUILayout.Slider(tweenProgress, 0, 1);

            if (EditorGUI.EndChangeCheck())
            {
                SetProgress(tweenProgress);

                if (!Application.isPlaying)
                {
                    serializedObject.FindProperty("progress").floatValue = tweenProgress;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.FlexibleSpace();
        }

        private void SetProgress(float tweenProgress)
        {
            if (!animationSequence.IsPlaying)
                PlaySequence();

            animationSequence.PlayingSequence.Goto(tweenProgress *
                                                     animationSequence.PlayingSequence.Duration());
        }

        private float GetCurrentSequencerProgress()
        {
            float tweenProgress;
            if (animationSequence.PlayingSequence != null && animationSequence.PlayingSequence.IsActive())
                tweenProgress = animationSequence.PlayingSequence.ElapsedPercentage();
            else
                tweenProgress = 0;
            return tweenProgress;
        }

        private void SetCurrentSequenceProgress(float progress)
        {
            animationSequence.PlayingSequence.Goto(progress *
                                                     animationSequence.PlayingSequence.Duration());
        }

        private void DrawTimeScaleSlider()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.LabelField("TimeScale");
            tweenTimeScale = EditorGUILayout.Slider(tweenTimeScale, 0, 2);
			
            UpdateSequenceTimeScale();

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
                
                foldout = EditorGUI.Foldout(rect, foldout, title);
                
                if (foldout)
                    additionalInspectorGUI.Invoke();
            }
        }
        
        private void OnDrawAnimationStep(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty flowTypeSerializedProperty = element.FindPropertyRelative(AnimationStep.NameOfFlowType);

            if (!element.TryGetTargetObjectOfProperty(out AnimationStep animationStepBase))
                return;

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
            // DrawContextInputOnItem(element, index, rect);
        }

        private float GetAnimationStepHeight(int index)
        {
            if (index > reorderableList.serializedProperty.arraySize - 1)
                return EditorGUIUtility.singleLineHeight;
            
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return element.GetPropertyDrawerHeight();
        }

        private void SetDefaults()
        {
            animationSequence = target as AnimationSequence;
            if (animationSequence == null) return;
            
            animationSequence.AutoplayMode = AnimationSequenceDefaults.Instance.AutoplayMode;
            animationSequence.StartPaused = AnimationSequenceDefaults.Instance.PauseOnAwake;
            animationSequence.TimeScaleIndependent = AnimationSequenceDefaults.Instance.TimeScaleIndependent;
            animationSequence.TypeOfPlay = AnimationSequenceDefaults.Instance.PlayType;
            animationSequence.UpdateType = AnimationSequenceDefaults.Instance.UpdateType;
            animationSequence.AutoKill = AnimationSequenceDefaults.Instance.AutoKill;
            animationSequence.Loops = AnimationSequenceDefaults.Instance.Loops;
            animationSequence.ResetComplete();
        }
    }
}
#endif